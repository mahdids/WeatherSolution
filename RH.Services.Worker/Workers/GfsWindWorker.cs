using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast.CityTile;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.WindDimension;
using RH.Shared.Extensions;

namespace RH.Services.Worker.Workers
{
    class GfsWindWorker : BackgroundService
    {
        private readonly ILogger<GfsWindWorker> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        public GfsWindWorker(ILogger<GfsWindWorker> logger, IServiceProvider services, IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                var dimensionManager = scope.ServiceProvider.GetRequiredService<IWindDimensionManager>();
                var gfsCrawler = scope.ServiceProvider.GetRequiredService<GfsWindCrawler>();

                long currentWindyTime = 0;
                long nextWindyTime = 0;
                int roundConter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"GfsWind Start Round {roundConter++} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    foreach (var dimension in dimensionManager.Dimensions)
                    {

                        await gfsCrawler.CrawlDimensionContentAsync(dimension);
                    }
                    
                    _logger.LogInformation($"End Round {roundConter} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    
                }
            }
        }



        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GFS Wind Worker Starting");
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping GFS Wind Worker");
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing GFS Wind Worker");
            base.Dispose();
        }

    }
}
