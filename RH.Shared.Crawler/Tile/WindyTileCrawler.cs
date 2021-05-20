using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Helper;
using RH.Shared.HttpClient;

namespace RH.Shared.Crawler.Tile
{
    public class WindyTileCrawler : ITileCrawler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        //private readonly string _webBaseAddress;
        //private readonly string _directoryBaseAddress;
        private readonly ILogger<WindyTileCrawler> _logger;

        public WindyTileCrawler(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WindyTileCrawler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            //_directoryBaseAddress = configuration["TilePath:Windy:Directory"];
            //_webBaseAddress = configuration["TilePath:Windy:Web"];
        }

        public async Task<CrawlResult> CrawlDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension,
            SystemSettings currentSetting)
        {
            var webPath = $"{dimension.Zoom}/{dimension.X}/{dimension.Y}.png";
            var directoryPath = $"{currentSetting.CrawlWebPath.TileDirectoryPath}\\{dimension.Zoom}\\{dimension.X}";
            var directoryInfo = Directory.CreateDirectory(directoryPath);
            var filePath = directoryInfo.FullName + $"\\{dimension.Y}.png";

            try
            {
                var client = _httpClientFactory.GetHttpClient(currentSetting.CrawlWebPath.TileWebPath);
                var item = await client.GetAsync(webPath);
                var contentStream = await item.Content.ReadAsStreamAsync(); // get the actual content stream
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await contentStream.CopyToAsync(stream);
                }
                _logger.LogInformation( $"Crawl Tile Succeeded : {currentSetting.CrawlWebPath.TileWebPath}/{webPath}");
                return new CrawlResult()
                {
                    Succeeded = true
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Crawl Tile Exception : {currentSetting.CrawlWebPath.TileWebPath}/{webPath}");
                return new CrawlResult() { Succeeded = false, Exception = e };
            }
        }

        public FileStream GetDimensionContentAsync(EntityFramework.Shared.Entities.Dimension dimension, SystemSettings currentSetting)
        {
            var webPath = $"{dimension.Zoom}/{dimension.X}/{dimension.Y}.png";
            var filePath =  $"{currentSetting.CrawlWebPath.TileDirectoryPath}\\{dimension.Zoom}\\{dimension.X}\\{dimension.Y}.png";
            if (!File.Exists(filePath))
                return null;
            return File.OpenRead(filePath);
        }
    }
}
