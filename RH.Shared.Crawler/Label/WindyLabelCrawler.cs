using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RH.EntityFramework.Repositories.Label;
using RH.Shared.Crawler.Helper;
using RH.Shared.HttpClient;

namespace RH.Shared.Crawler.Label
{
    public class WindyLabelCrawler : ILabelCrawler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILabelRepository _labelRepository;
        private readonly string _webBaseAddress;
        private readonly ILogger<WindyLabelCrawler> _logger;

        public WindyLabelCrawler(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WindyLabelCrawler> logger, ILabelRepository labelRepository)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _labelRepository = labelRepository;
            _webBaseAddress = configuration["LabelPath:Windy"];
        }

        public async Task<CrawlResult> CrawlDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension)
        {
            var webPath = $"{dimension.Zoom}/{dimension.X}/{dimension.Y}.json";
            try
            {
                var client = _httpClientFactory.GetHttpClient(_webBaseAddress);
                var item = await client.GetAsync(webPath);
                var contentString = await item.Content.ReadAsStringAsync(); // get the actual content stream
                var cityRecords = JsonConvert.DeserializeObject<List<List<object>>>(contentString);
                foreach (var record in cityRecords)
                {
                    try
                    {
                        var label = new EntityFramework.Shared.Entities.Label()
                        {
                            DimensionId = dimension.Id,
                            RegisterDate = DateTime.Now,
                            O = record[0].ToString(),
                            Name = record[1].ToString(),
                            Type = record[2].ToString(),
                            X = Convert.ToDouble(record[3].ToString()),
                            Y = Convert.ToDouble(record[4].ToString()),
                            ExtraField1 = Convert.ToInt32(record[5].ToString()),
                            ExtraField2 = Convert.ToInt32(record[6].ToString()),
                        };
                        await _labelRepository.Add(label);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Crawl Label Exception : {contentString}");
                    }
                }
                return new CrawlResult(){Succeeded = true};
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Crawl Label Exception : {_webBaseAddress}/{webPath}");
                return new CrawlResult() { Succeeded = false, Exception = e };
            }
        }

        public async Task<string> GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension)
        {
            var returnValue = "[";
            var labels =await _labelRepository.GetLabelsByDimensionId(dimension.Id);
            if (labels.Count==0)
            {
                await CrawlDimensionContentAsync(dimension);
                labels = await _labelRepository.GetLabelsByDimensionId(dimension.Id);
            }
            foreach (var label in labels)
            {
                if (returnValue!="[")
                {
                    returnValue += ",";
                }
                returnValue +=
                    $"[\"{label.O}\", \"{label.Name}\", \"{label.Type}\", {label.X}, {label.Y}, {label.ExtraField1}, {label.ExtraField2}]";
            }

            returnValue += "]";
            return returnValue;
        }
    }
}
