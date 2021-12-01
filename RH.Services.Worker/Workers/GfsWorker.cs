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
using RH.EntityFramework.Repositories.Cycle;


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
                var cycleRepository = scope.ServiceProvider.GetRequiredService<ICycleRepository>();
                long currentWindyTime = 0;
                long nextWindyTime = 0;
                int roundConter = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    Thread.Sleep(5000);
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"GFS Start Round {roundConter++} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    var currentSetting = await systemSetting.GetCurrentSetting();
                    var cycle = new RH.EntityFramework.Shared.Entities.Cycle()
                    {
                        Type = "GFS",
                        StartTime = DateTime.Now,
                        Compeleted = false,
                    };
                    await cycleRepository.AddCycleAsync(cycle); 
                    foreach (var dimension in dimensionManager.Dimensions)
                    {
                        currentSetting = await systemSetting.GetCurrentSetting();
                        await gfsCrawler.CrawlDimensionContentAsync(dimension,currentSetting);
                    }

                    if (gfsCrawler.MaxTime.Start > nextWindyTime)
                    {
                        nextWindyTime = gfsCrawler.MaxTime.Start;
                    }
                    _logger.LogInformation($"GFS End Round {roundConter} , Current:{currentWindyTime} , Next:{nextWindyTime}");
                    cycle.Compeleted = true;
                    cycle.EndTime = DateTime.Now;
                    await cycleRepository.AddCycleAsync(cycle);
                    if (nextWindyTime == currentWindyTime)
                    {
                        nextWindyTime += 60 * 60 * 3;
                        var windyNow = DateTime.Now.ToWindyUnixTime(3);
                        while (windyNow.Start <= nextWindyTime && !stoppingToken.IsCancellationRequested)
                        {
                            await Task.Delay(currentSetting.CrawlingInterval, stoppingToken);
                            windyNow = DateTime.Now.ToWindyUnixTime(3);
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
