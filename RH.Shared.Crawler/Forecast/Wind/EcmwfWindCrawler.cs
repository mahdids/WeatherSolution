using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RH.EntityFramework.Repositories.Wind;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Helper;
using RH.Shared.HttpClient;

namespace RH.Shared.Crawler.Forecast.Wind
{
    public class EcmwfWindCrawler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EcmwfWindRepository _ecmwfRepository;
        private readonly string _webBaseAddress;
        private readonly ILogger<EcmwfWindCrawler> _logger;

        public EcmwfWindCrawler(IHttpClientFactory httpClientFactory, EcmwfWindRepository ecmwfRepository, IConfiguration configuration, ILogger<EcmwfWindCrawler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _ecmwfRepository = ecmwfRepository;
            _webBaseAddress = configuration["Forecast:Wind:ECMWF"];
            _logger = logger;
        }
        public async Task<CrawlResult> CrawlDimensionContentAsync(EntityFramework.Shared.Entities.WindDimension dimension)
        {
            var webPath = $"{dimension.X}/{dimension.Y}";
            try
            {
                var client = _httpClientFactory.GetHttpClient(_webBaseAddress);
                var item = await client.GetAsync(webPath);
                if (item.StatusCode == HttpStatusCode.NotFound || item.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Crawl ECMWF Wind Record (No Content): {_webBaseAddress}/{webPath}");
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
        public async Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.WindDimension dimension)
        {
            var time = await _ecmwfRepository.GetLastExistTime(dimension.Id);
            var records = await _ecmwfRepository.GetContentByDimensionAndTime(dimension.Id, time);
            if (records.Count == 0)
            {
                return string.Empty;
            }
            var returnValue = SerializeEcmwfContent(records);
            return returnValue;
        }
        public async Task<string> GetDimensionContentByTimeAsync(EntityFramework.Shared.Entities.WindDimension dimension, long epocTime)
        {

            var time = await _ecmwfRepository.GetNearestTime(dimension.Id, epocTime);

            if (time == 0)
            {
                return string.Empty;
            }

            var records = await _ecmwfRepository.GetContentByDimensionAndEpoc(dimension.Id, time);
            if (records.Count == 0)
            {
                return string.Empty;
            }
            var returnValue = SerializeEcmwfContent(records);
            return returnValue;
        }
        private string SerializeEcmwfContent(List<EcmwfForecast> records)
        {
            return JsonConvert.SerializeObject(records);
        }


        private async Task<List<EcmwfForecast>> DeserializeEcmwfContent(int dimensionId, string content)
        {
            var returnValue = new List<EcmwfForecast>();
            WindRecord webRecord = JsonConvert.DeserializeObject<WindRecord>(content);
            var count = webRecord.Data.Hour.Count;
            for (int i = 0; i < count; i++)
            {
                var currentRecord = new EcmwfForecast()
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
