using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RH.EntityFramework.Repositories.Wind;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Helper;
using RH.Shared.HttpClient;

namespace RH.Shared.Crawler.Forecast.Wind
{
    public class GfsWindCrawler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GfsWindRepository _gfsRepository;
        private readonly ILogger<GfsWindCrawler> _logger;

        public GfsWindCrawler(IHttpClientFactory httpClientFactory, GfsWindRepository gfsRepository, ILogger<GfsWindCrawler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _gfsRepository = gfsRepository;
            //_webBaseAddress = configuration["Forecast:Wind:GFS"];
            _logger = logger;
        }
        public async Task<CrawlResult> CrawlDimensionContentAsync(
            EntityFramework.Shared.Entities.WindDimension dimension, SystemSettings currentSetting)
        {
            var webPath = $"{dimension.X}/{dimension.Y}";
            try
            {
                var client = _httpClientFactory.GetHttpClient(currentSetting.CrawlWebPath.ForecastWindGFS);
                var item = await client.GetAsync(webPath);
                if (item.StatusCode == HttpStatusCode.NotFound || item.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Crawl GFS Wind Record (No Content): {currentSetting.CrawlWebPath.ForecastWindGFS}/{webPath}");
                    return new CrawlResult() { Succeeded = true };
                }
                var contentString = await item.Content.ReadAsStringAsync(); // get the actual content stream
                var records = DeserializeGfsContent(dimension.Id, contentString);
                foreach (var record in records)
                {
                    await _gfsRepository.Add(record);
                }



                _logger.LogInformation($"Crawl GFS Wind Record : {currentSetting.CrawlWebPath.ForecastWindGFS}/{webPath}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Crawl GFS Wind Record : {currentSetting.CrawlWebPath.ForecastWindGFS}/{webPath}");
                return new CrawlResult() { Succeeded = false, Exception = e };
            }
            return new CrawlResult() { Succeeded = true };
        }
        public async Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.WindDimension dimension)
        {
            var time = await _gfsRepository.GetLastExistTime(dimension.Id);
            var records = await _gfsRepository.GetContentByDimensionAndTime(dimension.Id, time);
            if (records.Count == 0)
            {
                return string.Empty;
            }
            var returnValue = SerializeGfsContent(records);
            return returnValue;
        }
        public async Task<string> GetDimensionContentByTimeAsync(EntityFramework.Shared.Entities.WindDimension dimension, long epocTime)
        {

            var time = await _gfsRepository.GetNearestTime(dimension.Id, epocTime);

            if (time == 0)
            {
                return string.Empty;
            }

            var records = await _gfsRepository.GetContentByDimensionAndEpoc(dimension.Id, time);
            if (records.Count == 0)
            {
                return string.Empty;
            }
            var returnValue = SerializeGfsContent(records);
            return returnValue;
        }
        private string SerializeGfsContent(List<GfsForecast> records)
        {
            return JsonConvert.SerializeObject(records);
        }

        private List<GfsForecast> DeserializeGfsContent(int dimensionId, string content)
        {
            var returnValue = new List<GfsForecast>();
            WindRecord webRecord = JsonConvert.DeserializeObject<WindRecord>(content);
            var count = webRecord.Data.Hour.Count;
            for (int i = 0; i < count; i++)
            {
                var currentRecord = new GfsForecast()
                {
                    WindDimensionId = dimensionId,
                    DateTime = DateTime.Parse(webRecord.Data.OrigDate[i]),
                    OrigTs = webRecord.Data.OrigTs[i],
                    ReferenceTime = DateTime.Parse(webRecord.Header.RefTime),
                    ConvPrecip = webRecord.Data.ConvPrecip[i],
                    DewPoint = webRecord.Data.DewPoint[i],
                    Gust = webRecord.Data.Gust[i],
                    Mm = webRecord.Data.Mm[i],
                    Pressure = webRecord.Data.Pressure[i],
                    Rain = webRecord.Data.Rain[i],
                    Snow = webRecord.Data.Snow[i],
                    Rh = webRecord.Data.Rh[i],
                    SnowPrecip = webRecord.Data.SnowPrecip[i],
                    Temperature = webRecord.Data.Temp[i],
                    WeatherCode = webRecord.Data.Weathercode[i],
                    Wind = webRecord.Data.Wind[i],
                    WindDirection = webRecord.Data.WindDir[i]
                };
                returnValue.Add(currentRecord);
            }

            return returnValue;
        }


    }


}

