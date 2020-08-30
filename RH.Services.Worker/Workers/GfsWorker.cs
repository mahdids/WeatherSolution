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
using RH.Shared.Crawler.Label;
using RH.Shared.Crawler.Tile;
using RH.Shared.Extensions;

namespace RH.Services.Worker.Workers
{
    class GfsWorker : BackgroundService
    {
        private readonly ILogger<GfsWorker> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        public GfsWorker(ILogger<GfsWorker> logger, IServiceProvider services, IConfiguration configuration)
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
                var gfsCrawler = scope.ServiceProvider.GetRequiredService<WindyGfsCrawler>();

                long currentWindyTime = 0;
                long nextWindyTime = 0;
                int roundConter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"Start Round {roundConter++} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    foreach (var dimension in dimensionManager.Dimensions)
                    {

                        await gfsCrawler.CrawlDimensionContentAsync(dimension);
                    }

                    if (gfsCrawler.MaxTime.Start > nextWindyTime)
                    {
                        nextWindyTime = gfsCrawler.MaxTime.Start;
                    }
                    _logger.LogInformation($"End Round {roundConter} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    if (nextWindyTime == currentWindyTime)
                    {
                        while (DateTime.Now.ToWindyUnixTime(3).Start <= currentWindyTime && !stoppingToken.IsCancellationRequested)
                        {
                            await Task.Delay(60000, stoppingToken);
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
            _logger.LogInformation("GFS Worker Starting");
            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping GFS Worker");
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing GFS Worker");
            base.Dispose();
        }
       
    }
}
