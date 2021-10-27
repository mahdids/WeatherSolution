using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Label;
using RH.Shared.Crawler.Tile;
using RH.Shared.Crawler.WindDimension;

namespace RH.Services.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _services;

        public Worker(ILogger<Worker> logger, IServiceProvider services, IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker ExecuteAsync");
            using var scope = _services.CreateScope();
            var systemSetting = scope.ServiceProvider.GetRequiredService<ISystemSettingRepository>();

            var dimensionManager = scope.ServiceProvider.GetRequiredService<IDimensionManager>();
            var windDimensionManager = scope.ServiceProvider.GetRequiredService<IWindDimensionManager>();
            var tileCrawler = scope.ServiceProvider.GetRequiredService<ITileCrawler>();
            var labelCrawler = scope.ServiceProvider.GetRequiredService<ILabelCrawler>();
            var lastId = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var systemSettingActiveSettingId = systemSetting.ActiveSettingId;
                if (lastId != systemSettingActiveSettingId)
                {
                    lastId = systemSettingActiveSettingId;
                    var currentSetting = await systemSetting.GetCurrentSetting();
                    if (currentSetting.BaseWorkerSetting.RegenerateDimension)
                        await RegenerateAllDimensionAsync(dimensionManager, currentSetting);
                    if (currentSetting.BaseWorkerSetting.RegenerateWindDimension)
                        await RegenerateAllWindDimensionAsync(windDimensionManager, currentSetting);
                    if (currentSetting.BaseWorkerSetting.ReCrawlLabel)
                        await CrawlLabelAsync(dimensionManager, labelCrawler, currentSetting);
                    if (currentSetting.BaseWorkerSetting.ReCrawlTileImage)
                        await CrawlTileAsync(dimensionManager, tileCrawler, currentSetting);


                }
                Thread.Sleep(5000);
            }
            

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


        private async Task RegenerateAllDimensionAsync(IDimensionManager dimensionManager,
            SystemSettings currentSetting)
        {
            var result = await dimensionManager.RegenerateAllDimension(
                (short)currentSetting.Dimension.MinZoom,
                (short)currentSetting.Dimension.MaxZoom,
                (short)currentSetting.Dimension.TopLeft.X,
                (short)currentSetting.Dimension.TopLeft.Y,
                (short)currentSetting.Dimension.BottomRight.X,
                (short)currentSetting.Dimension.BottomRight.Y);

        }
        private async Task RegenerateAllWindDimensionAsync(IWindDimensionManager windDimensionManager,
            SystemSettings currentSetting)
        {
            var result = await windDimensionManager.RegenerateAllDimension(
                currentSetting.WindDimensions.XInterval,
                currentSetting.WindDimensions.YInterval,
                currentSetting.WindDimensions.TopLeft.X,
                currentSetting.WindDimensions.TopLeft.Y,
                currentSetting.WindDimensions.BottomRight.X,
                currentSetting.WindDimensions.BottomRight.Y);
        }
        private async Task CrawlTileAsync(IDimensionManager dimensionManager, ITileCrawler tileCrawler,
            SystemSettings currentSetting)
        {
            foreach (var dimension in dimensionManager.Dimensions)
            {
                await tileCrawler.CrawlDimensionContentAsync(dimension, currentSetting);
            }
        }
        private async Task CrawlLabelAsync(IDimensionManager dimensionManager, ILabelCrawler labelCrawler,
            SystemSettings currentSetting)
        {
            foreach (var dimension in dimensionManager.Dimensions)
            {
                await labelCrawler.CrawlDimensionContentAsync(dimension, currentSetting);
            }
        }
    }
}
