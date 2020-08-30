using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Label;
using RH.Shared.Crawler.Tile;

namespace RH.Services.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IServiceProvider services, IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker ExecuteAsync");
            using var scope = _services.CreateScope();
            var dimensionManager = scope.ServiceProvider.GetRequiredService<IDimensionManager>();
            var tileCrawler = scope.ServiceProvider.GetRequiredService<ITileCrawler>();
            var labelCrawler = scope.ServiceProvider.GetRequiredService<ILabelCrawler>();
            if (_configuration["Worker:RegenerateDimension"] == "1")
                await RegenerateAllDimensionAsync(dimensionManager);
            if (_configuration["Worker:CrawlLabel"] == "1")
                await CrawlLabelAsync(dimensionManager, labelCrawler);
            if (_configuration["Worker:CrawlTile"] == "1")
                await CrawlTileAsync(dimensionManager, tileCrawler);

        }



        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker Starting");
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing Service");
            base.Dispose();
        }


        private async Task RegenerateAllDimensionAsync(IDimensionManager dimensionManager)
        {
            var result = await dimensionManager.RegenerateAllDimension(
                short.Parse(_configuration["Dimensions:Zoom:Min"]),
                short.Parse(_configuration["Dimensions:Zoom:Max"]),
                short.Parse(_configuration["Dimensions:TopLeft:X"]),
                short.Parse(_configuration["Dimensions:TopLeft:Y"]),
                short.Parse(_configuration["Dimensions:BottomRight:X"]),
                short.Parse(_configuration["Dimensions:BottomRight:Y"]));
        }

        private async Task CrawlTileAsync(IDimensionManager dimensionManager, ITileCrawler tileCrawler)
        {
            foreach (var dimension in dimensionManager.Dimensions)
            {
                await tileCrawler.CrawlDimensionContentAsync(dimension);
            }
        }
        private async Task CrawlLabelAsync(IDimensionManager dimensionManager, ILabelCrawler labelCrawler)
        {
            foreach (var dimension in dimensionManager.Dimensions)
            {
                await labelCrawler.CrawlDimensionContentAsync(dimension);
            }
        }
    }
}
