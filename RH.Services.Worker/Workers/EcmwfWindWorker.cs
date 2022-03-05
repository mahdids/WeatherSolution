using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.EntityFramework.Repositories.Cycle;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.Helper;
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
                var systemSetting = scope.ServiceProvider.GetRequiredService<ISystemSettingRepository>();

                var dimensionManager = scope.ServiceProvider.GetRequiredService<IWindDimensionManager>();
                var cycleRepository = scope.ServiceProvider.GetRequiredService<ICycleRepository>();
                var ecmwfCrawler = scope.ServiceProvider.GetRequiredService<EcmwfWindCrawler>();

                int roundCounter = 0;
                var currentSetting = await systemSetting.GetCurrentSetting();
                var activeId = currentSetting.Id;
                var returnTime = "";
                DateTime? time = default;
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (activeId != systemSetting.ActiveSettingId)
                    {
                        currentSetting = await systemSetting.GetCurrentSetting();
                    }
                    Thread.Sleep(5000);
                    dimensionManager.ReloadDimensions();
                    _logger.LogInformation($"EcmwfWind Start Round {roundCounter++} , Current:{DateTime.Now} ");
                    var cycle = new RH.EntityFramework.Shared.Entities.Cycle()
                    {
                        Type = "EcmwfWind",
                        StartTime = DateTime.Now,
                        Compeleted = false,
                        dateTime = time == default ? null : time,
                    };
                    await cycleRepository.AddCycleAsync(cycle);
                    //foreach (var dimension in dimensionManager.Dimensions)
                    //{ }

                    var len = dimensionManager.Dimensions.Count;
                    var currentround = 0;
                    CrawlResult result = new CrawlResult() { Succeeded = true };
                    for (int i = 0; i < len; i++)
                    {
                        var dimension = dimensionManager.Dimensions[i];
                        if (activeId != systemSetting.ActiveSettingId)
                        {
                            currentSetting = await systemSetting.GetCurrentSetting();
                        }

                        if (currentround == 0)
                        {
                            result = await ecmwfCrawler.CrawlDimensionContentAsync(dimension, currentSetting);
                            if (result.Succeeded)
                                returnTime = result.Message;
                            currentround++;
                        }
                        else
                        {
                            if (result.Succeeded)
                            {
                                result = await ecmwfCrawler.SetDimensionContentAsync(dimension, result.Message);
                                if (result.Succeeded)
                                    returnTime = result.Message;
                            }

                            currentround++;
                        }
                        if (currentround == currentSetting.Resolution)
                            currentround = 0;
                    }
                    time = DateTime.Parse(returnTime).AddHours(3);
                    cycle.Compeleted = true;
                    cycle.EndTime = DateTime.Now;
                    await cycleRepository.AddCycleAsync(cycle);
                    _logger.LogInformation($"EcmwfWind End Round {roundCounter} , Current:{DateTime.Now} , Next:{time}");
                    while (time > DateTime.Now)
                    {
                        Thread.Sleep(currentSetting.CrawlingInterval);
                    }
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
