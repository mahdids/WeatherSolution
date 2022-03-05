using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.Services.RestApi.Models;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Label;

namespace RH.Services.RestApi.Controller
{
    public class TilesWebController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<TilesWebController> _logger;
        private readonly ISystemSettingRepository _settingRepository;
        private readonly IDimensionManager _dimensionManager;
        private ILabelCrawler _labelCrawler;
        public TilesWebController(ILogger<TilesWebController> logger, ISystemSettingRepository settingRepository, IDimensionManager dimensionManager, ILabelCrawler labelCrawler)
        {
            _logger = logger;
            _settingRepository = settingRepository;
            _dimensionManager = dimensionManager;
            _labelCrawler = labelCrawler;
        }

        public async Task<IActionResult> Index(short zoom, short x, short y)
        {

            ViewBag.Borders = _dimensionManager.GetBorder();
            ViewBag.Zoom = zoom;
            ViewBag.X = x;
            ViewBag.Y = y;
            try
            {
                if (x==0&&y==0&&zoom==0)
                {
                    return View(new TilesViewModel() );
                }
                var dimension = await _dimensionManager.GetDimension(zoom, x, y);
                if (dimension == null)
                    return View(new TilesViewModel() );
                var labels = await _labelCrawler.GetDimensionContentAsync(dimension);
                return View(new TilesViewModel() { HasData = true,Src = $"TilesWeb/GetImage?path={zoom}\\{x}\\{y}" ,Labels = labels});
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"TilesWebController/Index {zoom} {x} {y} ");
                return View(new TilesViewModel());
            }
        }

        public async Task<FileResult> GetImage(string path)
        {
            var setting = await _settingRepository.GetCurrentSetting();
            var filePath = $"{setting.CrawlWebPath.TileDirectoryPath}\\{path}.png";
            if (!System.IO.File.Exists(filePath))
                return null;
            
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "image/png");
        }
    }
}
