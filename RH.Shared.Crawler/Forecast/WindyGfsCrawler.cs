using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RH.EntityFramework.Repositories.Forecast.GFS;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Helper;
using RH.Shared.HttpClient;

namespace RH.Shared.Crawler.Forecast
{
    public class WindyGfsCrawler : IForecastCrawler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGfsRepository _gfsRepository;
        private readonly string _webBaseAddress;
        private readonly ILogger<WindyGfsCrawler> _logger;
        private WindyTime _maxTime=new WindyTime();
        private WindyTime _lastTime = new WindyTime();

        public WindyGfsCrawler(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WindyGfsCrawler> logger, IGfsRepository gfsRepository)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _gfsRepository = gfsRepository;
            _webBaseAddress = configuration["Forecast:Windy:GFS"];
        }

        public WindyTime MaxTime
        {
            get => _maxTime;
        }

        public async Task<CrawlResult> CrawlDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension)
        {
            var webPath = $"{dimension.Zoom}/{dimension.X}/{dimension.Y}";
            try
            {
                var client = _httpClientFactory.GetHttpClient(_webBaseAddress);
                var item = await client.GetAsync(webPath);
                if (item.StatusCode==HttpStatusCode.NotFound||item.StatusCode==HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Crawl GFS Record (No Content): {_webBaseAddress}/{webPath}");
                    return new CrawlResult(){Succeeded = true};
                }
                var contentString = await item.Content.ReadAsStringAsync(); // get the actual content stream
                var records =await DeserializeGfsContent(dimension.Id,contentString);
                foreach (var record in records)
                {
                    await _gfsRepository.Add(record);
                }

                
                
                _logger.LogInformation($"Crawl GFS Record : {_webBaseAddress}/{webPath}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Crawl GFS Exception : {_webBaseAddress}/{webPath}");
                return new CrawlResult() { Succeeded = false, Exception = e };
            }
            return new CrawlResult() { Succeeded = true };
        }
       
        public async Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension)
        {
            var time = await _gfsRepository.GetLastExistTime(dimension.Id);
            if (time == null)
            {
                await CrawlDimensionContentAsync(dimension);
                //var result= await GetDimensionContentAsync(dimension);
            }
            time = await _gfsRepository.GetLastExistTime(dimension.Id);
            if (time==null)
            {
                return string.Empty;
            }
            var records = await _gfsRepository.GetContentByDimensionAndTime(dimension.Id, time.Id);
            if (records.Count == 0)
            {
                return string.Empty;
            }
            var returnValue = SerializeGfsContent(records, time);
            return returnValue;
        }

        public async Task<string> GetDimensionContentByTimeAsync(EntityFramework.Shared.Entities.Dimension dimension, long epocTime)
        {
            var prevDay = epocTime - 86400000;
            var nextDay = epocTime + 86400000;
            var time = await _gfsRepository.GetExistTime(dimension.Id, prevDay, nextDay);

            if (time.Count==0 )
            {
                return string.Empty;
            }

            var nearestTimeDiff = time.Min(x => Math.Abs(x.Start - epocTime));
            var nearestTime = time.FirstOrDefault(x => Math.Abs(x.Start - epocTime) == nearestTimeDiff);
            var records = await _gfsRepository.GetContentByDimensionAndTime(dimension.Id, nearestTime.Id);
            if (records.Count == 0)
            {
                return string.Empty;
            }
            var returnValue = SerializeGfsContent(records, nearestTime);
            return returnValue;
        }

        private async Task<List<Gfs>> DeserializeGfsContent(int dimensionId, string content)
        {
            var returnValue=new List<Gfs>();
            var records = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var start = long.Parse(records["start"].ToString());
            records.Remove("start");

            var step = short.Parse(records["step"].ToString());
            records.Remove("step");
            if (_lastTime.Start!=start)
            {
                _lastTime= await _gfsRepository.GetTime(start, step);
                if (_maxTime.Start<_lastTime.Start)
                {
                    _maxTime = _lastTime;
                }
            }

            foreach (var record in records)
            {
                var locPart = record.Key.Split("/");
                returnValue.Add(new Gfs()
                {
                    DimensionId = dimensionId,
                    RegisterDate = DateTime.Now,
                    Location = record.Key,
                    X = double.Parse(locPart[0]),
                    Y = double.Parse(locPart[1]),
                    DataString = record.Value.ToString().Replace("\r\n",""),
                    WindyTimeId = _lastTime.Id
                });
            }

            return returnValue;
        }
        private string SerializeGfsContent(List<Gfs> records, WindyTime time)
        {
            var returnValue = new Dictionary<string, object>();
            returnValue.Add("step", time.Step);
            returnValue.Add("start", time.Start);
            foreach (var record in records)
            {
                var data = JsonConvert.DeserializeObject<List<int>>(record.DataString);
                returnValue.Add(record.Location, data);
            }

            return JsonConvert.SerializeObject(returnValue);
        }

       
    }
}
