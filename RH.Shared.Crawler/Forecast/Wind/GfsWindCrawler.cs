using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public GfsWindCrawler(IHttpClientFactory httpClientFactory, GfsWindRepository gfsRepository, ILogger<GfsWindCrawler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _gfsRepository = gfsRepository;
            //_webBaseAddress = configuration["Forecast:Wind:GFS"];
            _logger = logger;
        }
        public async Task<CrawlResult> CrawlDimensionContentAsync(EntityFramework.Shared.Entities.WindDimension dimension, SystemSettings currentSetting)
        {
            var webPath = $"{dimension.X}/{dimension.Y}";
            var returnTime = "";
            try
            {
                var client = _httpClientFactory.GetHttpClient(currentSetting.CrawlWebPath.ForecastWindGFS);
                var item = await client.GetAsync(webPath);
                if (item.StatusCode == HttpStatusCode.NotFound || item.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Crawl GFS Wind Record (No Content): {currentSetting.CrawlWebPath.ForecastWindGFS}/{webPath}");
                    return new CrawlResult() { Succeeded = true };
                }
                if (item.StatusCode == HttpStatusCode.Forbidden || item.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation($"Crawl GFS Wind Record  (Forbidden): {currentSetting.CrawlWebPath.ForecastWindGFS}/{webPath}");
                    return new CrawlResult() { Succeeded = true };
                }
                var contentString = await item.Content.ReadAsStringAsync(); // get the actual content stream
                var records = DeserializeGfsContent(dimension.Id, contentString);
                foreach (var record in records)
                {
                    returnTime = record.ReferenceTime.ToString();
                    await _gfsRepository.Add(record);
                }



                _logger.LogInformation($"Crawl GFS Wind Record : {currentSetting.CrawlWebPath.ForecastWindGFS}/{webPath}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Crawl GFS Wind Record : {currentSetting.CrawlWebPath.ForecastWindGFS}/{webPath}");
                return new CrawlResult() { Succeeded = false, Exception = e };
            }
            return new CrawlResult() { Succeeded = true, Message = returnTime };
        }

        public async Task<CrawlResult> CrawlDimensionContentLevelAsync(EntityFramework.Shared.Entities.WindDimension dimension, SystemSettings currentSetting, int counter)
        {
            var keys = currentSetting.CrawlWebPath.ForecastWindGFSLevelKeys.Split(';', StringSplitOptions.RemoveEmptyEntries);


            var content = "{"
                          + $"   \"lat\": {dimension.X}," +
                          $"    \"lon\": {dimension.Y}," +
                          $"    \"model\": \"gfs\"," +
                          $"    \"parameters\": [\"wind\", \"dewpoint\", \"rh\",\"temp\"]," +//,\"\",\"\",\"\"
                          $"    \"levels\": [ \"1000h\", \"950h\", \"925h\", \"900h\", \"850h\", \"800h\", \"700h\", \"600h\", \"500h\", \"400h\", \"300h\", \"200h\", \"150h\"]," +
                          $"    \"key\": \"{keys[counter % keys.Length]}\""
                          + "}";
            var returnTime = "";
            try
            {

                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var client = _httpClientFactory.GetHttpClient(currentSetting.CrawlWebPath.ForecastWindGFSLevel);
                var item = await client.PostAsync(currentSetting.CrawlWebPath.ForecastWindGFSLevel, byteContent);
                if (item.StatusCode == HttpStatusCode.NotFound || item.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Crawl GFS Wind Level Record (No Content): {content}");
                    return new CrawlResult() { Succeeded = true };
                }
                if (item.StatusCode == HttpStatusCode.Forbidden || item.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation($"Crawl GFS Wind Level Record  (Forbidden): {content}");
                    return new CrawlResult() { Succeeded = true };
                }
                var contentString = await item.Content.ReadAsStringAsync(); // get the actual content stream
                var records = DeserializeGfsLevelContent(dimension.Id, contentString);
                foreach (var record in records)
                {
                    returnTime = record.ReferenceTime.ToString();
                    await _gfsRepository.Add(record);
                }


                _logger.LogInformation($"Crawl GFS Wind Level Record : {content}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Crawl GFS Wind Level Record : {content}");
                return new CrawlResult() { Succeeded = false, Exception = e };
            }
            return new CrawlResult() { Succeeded = true, Message = returnTime };
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

        public async Task<List<GfsForecast>> GetDimensionContentByTimeAsync(
            EntityFramework.Shared.Entities.WindDimension dimension, ForecastLevel level, DateTime dateTime)
        {

            var time = await _gfsRepository.GetNearestTime(dimension.Id, dateTime);

            if (time == null)
            {
                return new List<GfsForecast>();
            }

            var records = await _gfsRepository.GetContentByDimensionAndDateTime(dimension.Id,level, (DateTime)time);
            return records;

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
                    Level = ForecastLevel.Surface,
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
        private List<GfsForecast> DeserializeGfsLevelContent(int dimensionId, string contentString)
        {
            var content = contentString
                .Replace("-1000h", "_1000h")
                .Replace("-950h", "_950h")
                .Replace("-925h", "_925h")
                .Replace("-900h", "_900h")
                .Replace("-850h", "_850h")
                .Replace("-800h", "_800h")
                .Replace("-700h", "_700h")
                .Replace("-600h", "_600h")
                .Replace("-500h", "_500h")
                .Replace("-400h", "_400h")
                .Replace("-300h", "_300h")
                .Replace("-200h", "_200h")
                .Replace("-150h", "_150h");
            GfsLevelForecast webRecord = JsonConvert.DeserializeObject<GfsLevelForecast>(content);
            var returnValue = new List<GfsForecast>();
            var count = webRecord.ts.Count;
            var origTs = webRecord.ts[0];
            for (int i = 0; i < count; i++)
            {
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H1000, webRecord.ts[i], webRecord.wind_u_1000h[i], webRecord.wind_v_1000h[i], webRecord.temp_1000h[i], webRecord.dewpoint_1000h[i], webRecord.rh_1000h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H950, webRecord.ts[i], webRecord.wind_u_950h[i], webRecord.wind_v_950h[i], webRecord.temp_950h[i], webRecord.dewpoint_950h[i], webRecord.rh_950h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H925, webRecord.ts[i], webRecord.wind_u_925h[i], webRecord.wind_v_925h[i], webRecord.temp_925h[i], webRecord.dewpoint_925h[i], webRecord.rh_925h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H900, webRecord.ts[i], webRecord.wind_u_900h[i], webRecord.wind_v_900h[i], webRecord.temp_900h[i], webRecord.dewpoint_900h[i], webRecord.rh_900h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H850, webRecord.ts[i], webRecord.wind_u_850h[i], webRecord.wind_v_850h[i], webRecord.temp_850h[i], webRecord.dewpoint_850h[i], webRecord.rh_850h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H800, webRecord.ts[i], webRecord.wind_u_800h[i], webRecord.wind_v_800h[i], webRecord.temp_800h[i], webRecord.dewpoint_800h[i], webRecord.rh_800h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H700, webRecord.ts[i], webRecord.wind_u_700h[i], webRecord.wind_v_700h[i], webRecord.temp_700h[i], webRecord.dewpoint_700h[i], webRecord.rh_700h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H600, webRecord.ts[i], webRecord.wind_u_600h[i], webRecord.wind_v_600h[i], webRecord.temp_600h[i], webRecord.dewpoint_600h[i], webRecord.rh_600h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H500, webRecord.ts[i], webRecord.wind_u_500h[i], webRecord.wind_v_500h[i], webRecord.temp_500h[i], webRecord.dewpoint_500h[i], webRecord.rh_500h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H400, webRecord.ts[i], webRecord.wind_u_400h[i], webRecord.wind_v_400h[i], webRecord.temp_400h[i], webRecord.dewpoint_400h[i], webRecord.rh_400h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H300, webRecord.ts[i], webRecord.wind_u_300h[i], webRecord.wind_v_300h[i], webRecord.temp_300h[i], webRecord.dewpoint_300h[i], webRecord.rh_300h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H200, webRecord.ts[i], webRecord.wind_u_200h[i], webRecord.wind_v_200h[i], webRecord.temp_200h[i], webRecord.dewpoint_200h[i], webRecord.rh_200h[i]));
                returnValue.Add(LevelRecordCreator(origTs, dimensionId, ForecastLevel.H150, webRecord.ts[i], webRecord.wind_u_150h[i], webRecord.wind_v_150h[i], webRecord.temp_150h[i], webRecord.dewpoint_150h[i], webRecord.rh_150h[i]));

            }

            return returnValue;
        }

        private GfsForecast LevelRecordCreator(long origTs, int dimensionId, ForecastLevel level, long ts, double windU, double windV, double temp, double dewPoint, double rh)
        {
            var wind = WindCalculator(windU, windV);
            return new GfsForecast()
            {
                WindDimensionId = dimensionId,
                Level = level,
                DateTime = epoch.AddMilliseconds(ts),
                OrigTs = origTs,
                ReferenceTime = epoch.AddMilliseconds(origTs),
                ConvPrecip = 0,
                DewPoint = dewPoint,
                Gust = 0,
                Mm = 0,
                Pressure = 0,
                Rain = false,
                Snow = false,
                Rh = (int)rh,
                SnowPrecip = 0,
                Temperature = temp,
                WeatherCode = string.Empty,
                Wind = wind.speed,
                WindDirection = wind.direction
            };
        }


        private (double speed, int direction) WindCalculator(double u, double v)
        {
            var degree = (180 + (180 / Math.PI) * Math.Atan2(v, u)) % 360;
            var speed = Math.Sqrt(u * u + v * v);
            return (speed, (int)degree);
        }
    }

}

