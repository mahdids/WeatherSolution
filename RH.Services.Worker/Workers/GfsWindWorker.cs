using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.WindDimension;

namespace RH.Services.Worker.Workers
{
    class GfsWindWorker : BackgroundService
    {
        private readonly ILogger<GfsWindWorker> _logger;
        private readonly IServiceProvider _services;
        public GfsWindWorker(ILogger<GfsWindWorker> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _services.CreateScope())
            {
                var systemSetting = scope.ServiceProvider.GetRequiredService<ISystemSettingRepository>();
                var dimensionManager = scope.ServiceProvider.GetRequiredService<IWindDimensionManager>();
                var gfsCrawler = scope.ServiceProvider.GetRequiredService<GfsWindCrawler>();

                long currentWindyTime = 0;
                long nextWindyTime = 0;
                int roundConter = 0;
                var currentSetting = await systemSetting.GetCurrentSetting();
                var activeId = currentSetting.Id;

                while (!stoppingToken.IsCancellationRequested)
                {

                    Thread.Sleep(5000);
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"GfsWind Start Round {roundConter++} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    foreach (var dimension in dimensionManager.Dimensions)
                    {
                        if (activeId != systemSetting.ActiveSettingId)
                        {
                            currentSetting = await systemSetting.GetCurrentSetting();
                        }
                        await gfsCrawler.CrawlDimensionContentAsync(dimension,currentSetting);
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
