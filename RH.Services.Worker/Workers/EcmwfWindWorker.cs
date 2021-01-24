using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.WindDimension;

namespace RH.Services.Worker.Workers
{
    class EcmwfWindWorker : BackgroundService
    {
        private readonly ILogger<EcmwfWindWorker> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        public EcmwfWindWorker(ILogger<EcmwfWindWorker> logger, IServiceProvider services, IConfiguration configuration)
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
                var ecmwfCrawler = scope.ServiceProvider.GetRequiredService<EcmwfWindCrawler>();

                long currentWindyTime = 0;
                long nextWindyTime = 0;
                int roundConter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"EcwmfWind Start Round {roundConter++} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    foreach (var dimension in dimensionManager.Dimensions)
                    {

                        await ecmwfCrawler.CrawlDimensionContentAsync(dimension);
                    }

                    _logger.LogInformation($"End Round {roundConter} , Current:{currentWindyTime} , Next:{nextWindyTime}");

                }
            }
        }



        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ecwmf Wind Worker Starting");
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Ecwmf Wind Worker");
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing Ecwmf Wind Worker");
            base.Dispose();
        }

    }
}
