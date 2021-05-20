using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast.CityTile;
using RH.Shared.Extensions;

namespace RH.Services.Worker.Workers
{
    class GfsWorker : BackgroundService
    {
        private readonly ILogger<GfsWorker> _logger;
        private readonly IServiceProvider _services;
        public GfsWorker(ILogger<GfsWorker> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                var systemSetting = scope.ServiceProvider.GetRequiredService<ISystemSettingRepository>();

                var dimensionManager = scope.ServiceProvider.GetRequiredService<IDimensionManager>();
                var gfsCrawler = scope.ServiceProvider.GetRequiredService<GfsCityTileCrawler>();

                long currentWindyTime = 0;
                long nextWindyTime = 0;
                int roundConter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(5000);
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"Start Round {roundConter++} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    var currentSetting = await systemSetting.GetCurrentSetting();
                    foreach (var dimension in dimensionManager.Dimensions)
                    {
                        currentSetting = await systemSetting.GetCurrentSetting();
                        await gfsCrawler.CrawlDimensionContentAsync(dimension,currentSetting);
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
                            await Task.Delay(currentSetting.CrawlingInterval, stoppingToken);
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
