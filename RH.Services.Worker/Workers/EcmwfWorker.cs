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
using RH.Shared.Crawler.Forecast;
using RH.Shared.Crawler.Forecast.CityTile;
using RH.Shared.Crawler.Label;
using RH.Shared.Crawler.Tile;
using RH.Shared.Extensions;

namespace RH.Services.Worker.Workers
{
    class EcmwfWorker: BackgroundService
    {
        private readonly ILogger<EcmwfWorker> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        public EcmwfWorker(ILogger<EcmwfWorker> logger, IServiceProvider services, IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                var dimensionManager = scope.ServiceProvider.GetRequiredService<IDimensionManager>();
                
                var esmwfCrawler = scope.ServiceProvider.GetRequiredService<EcmwfCityTileCrawler>();

                
                long currentWindyTime = 0;
                long nextWindyTime = 0;
                int roundConter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"Start Round {roundConter++} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    foreach (var dimension in dimensionManager.Dimensions)
                    {

                        await esmwfCrawler.CrawlDimensionContentAsync(dimension);
                    }

                   
                    if (esmwfCrawler.MaxTime.Start > nextWindyTime)
                    {
                        nextWindyTime = esmwfCrawler.MaxTime.Start;
                    }
                    _logger.LogInformation($"End Round {roundConter} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    if (nextWindyTime == currentWindyTime)
                    {
                        while (DateTime.Now.ToWindyUnixTime(3).Start <= currentWindyTime && !stoppingToken.IsCancellationRequested)
                        {
                            await Task.Delay(int.Parse(_configuration["CrawlingInterval"]), stoppingToken);
                        }
                    }
                    else
                    {
                        currentWindyTime = nextWindyTime;
                    }


                }
            }
        }



        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ECMWF Worker Starting");
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping ECMWF Worker");
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing ECMWF Worker");
            base.Dispose();
        }
        
    }
}
