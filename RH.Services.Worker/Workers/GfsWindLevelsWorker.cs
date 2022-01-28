using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.WindDimension;
using RH.EntityFramework.Repositories.Cycle;

namespace RH.Services.Worker.Workers
{
    class GfsWindLevelsWorker: BackgroundService
    {
        private readonly ILogger<GfsWindLevelsWorker> _logger;
        private readonly IServiceProvider _services;
        public GfsWindLevelsWorker(ILogger<GfsWindLevelsWorker> logger, IServiceProvider services)
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
                var cycleRepository = scope.ServiceProvider.GetRequiredService<ICycleRepository>();
                int roundConter = 0;
                var currentSetting = await systemSetting.GetCurrentSetting();
                var activeId = currentSetting.Id;
                var returnTime = "";
                DateTime? time = default;
                while (!stoppingToken.IsCancellationRequested)
                {

                    Thread.Sleep(5000);
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"GfsWindLevel Start Round {roundConter++} , Current:{DateTime.Now} ");
                    var cycle = new RH.EntityFramework.Shared.Entities.Cycle()
                    {
                        Type = "GFSWindLevel",
                        StartTime = DateTime.Now,
                        Compeleted = false,
                        dateTime = time == default ? null : time,
                    };
                    await cycleRepository.AddCycleAsync(cycle);
                    var counter = 0;
                    foreach (var dimension in dimensionManager.Dimensions)
                    {
                        if (activeId != systemSetting.ActiveSettingId)
                        {
                            currentSetting = await systemSetting.GetCurrentSetting();
                        }
                        var result = await gfsCrawler.CrawlDimensionContentLevelAsync(dimension, currentSetting,counter++);
                        if (result.Succeeded)
                            returnTime = result.Message;
                    }
                    time = DateTime.Parse(returnTime).AddDays(1).Date;
                    cycle.Compeleted = true;
                    cycle.EndTime = DateTime.Now;
                    await cycleRepository.AddCycleAsync(cycle);
                    _logger.LogInformation($"GfsWind End Round  {roundConter} , Current:{DateTime.Now} , Next:{time}");

                    while (time > DateTime.Now)
                    {
                        Thread.Sleep(currentSetting.CrawlingInterval);
                    }
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
