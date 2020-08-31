using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RH.EntityFramework.Repositories.Forecast.ECMWF;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Helper;
using RH.Shared.HttpClient;

namespace RH.Shared.Crawler.Forecast
{
    public class WindyEcmwfCrawler:IForecastCrawler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEcmwfRepository _ecmwfRepository;
        private readonly string _webBaseAddress;
        private readonly ILogger<WindyEcmwfCrawler> _logger;
        private WindyTime _maxTime = new WindyTime();
        private WindyTime _lastTime = new WindyTime();
        public WindyEcmwfCrawler(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WindyEcmwfCrawler> logger, IEcmwfRepository ecmwfRepository)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _ecmwfRepository = ecmwfRepository;
            _webBaseAddress = configuration["Forecast:Windy:ECMWF"];
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
                if (item.StatusCode == HttpStatusCode.NotFound || item.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Crawl ECMWF Record  (No Content): {_webBaseAddress}/{webPath}");
                    return new CrawlResult() { Succeeded = true };
                }
                var contentString = await item.Content.ReadAsStringAsync(); // get the actual content stream
                var records = await DeserializeEcmwfContent(dimension.Id, contentString);
                foreach (var record in records)
                {
                    await _ecmwfRepository.Add(record);
                }
                _logger.LogInformation($"Crawl ECMWF Record : {_webBaseAddress}/{webPath}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Crawl ECMWF Exception : {_webBaseAddress}/{webPath}");
                return new CrawlResult() { Succeeded = false, Exception = e };
            }
            return new CrawlResult() { Succeeded = true };
        }

        public async Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension)
        {
            var time = await _ecmwfRepository.GetLastExistTime(dimension.Id);
            if (time == null)
                return string.Empty;
            var records = await _ecmwfRepository.GetContentByDimensionAndTime(dimension.Id, time.Id);
            if (records.Count==0)
            {
                return string.Empty;
            }
            var returnValue = SerializeEcmwfContent(records,time);
            return returnValue;
        }

        private string SerializeEcmwfContent(List<Ecmwf> records, WindyTime time)
        {
            var returnValue=new Dictionary<string,object>();
            returnValue.Add("step", time.Step);
            returnValue.Add("start",time.Start);
            foreach (var record in records)
            {
                var data = JsonConvert.DeserializeObject<List<int>>(record.DataString);
                returnValue.Add(record.Location,data);    
            }

            return JsonConvert.SerializeObject(returnValue);
        }

        private async Task<List<Ecmwf>> DeserializeEcmwfContent(int dimensionId, string content)
        {
            var returnValue = new List<Ecmwf>();
            var records = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var start = long.Parse(records["start"].ToString());
            records.Remove("start");

            var step = short.Parse(records["step"].ToString());
            records.Remove("step");
            if (_lastTime.Start != start)
            {
                _lastTime = await _ecmwfRepository.GetTime(start, step);
                if (_maxTime.Start < _lastTime.Start)
                {
                    _maxTime = _lastTime;
                }
            }
            foreach (var record in records)
            {
                var locPart = record.Key.Split("/");
                returnValue.Add(new Ecmwf()
                {
                    DimensionId = dimensionId,
                    RegisterDate = DateTime.Now,
                    Location = record.Key,
                    X = double.Parse(locPart[0]),
                    Y = double.Parse(locPart[1]),
                    DataString = record.Value.ToString().Replace("\r\n", ""),
                    WindyTimeId = _lastTime.Id
                });
            }

            return returnValue;
        }
    }
}
