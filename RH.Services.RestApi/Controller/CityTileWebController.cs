using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.EntityFramework.Shared.Entities;
using RH.Services.RestApi.Models;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast.CityTile;
using RH.Shared.Extensions;

namespace RH.Services.RestApi.Controller
{
    public class CityTileWebController :  Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<CityTileWebController> _logger;
        private readonly IDimensionManager _dimensionManager;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly EcmwfCityTileCrawler _ecmwfCrawler;
        private readonly GfsCityTileCrawler _gfsCrawler;

        public CityTileWebController(IDimensionManager dimensionManager, ISystemSettingRepository systemSettingRepository, EcmwfCityTileCrawler ecmwfCrawler, GfsCityTileCrawler gfsCrawler, ILogger<CityTileWebController> logger)
        {
            _dimensionManager = dimensionManager;
            _systemSettingRepository = systemSettingRepository;
            _ecmwfCrawler = ecmwfCrawler;
            _gfsCrawler = gfsCrawler;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string cityTileType,short zoom, short x, short y, DateTime date)
        {
            Dimension dimension;
            if (date == default)
            {
                date = DateTime.Now;
            }
            ViewBag.Borders = _dimensionManager.GetBorder();
            ViewBag.Zoom = zoom;
            ViewBag.X = x;
            ViewBag.Y = y;
            ViewBag.Date = date.ToString("yyyy-MM-ddTHH:mm:ss");
            ViewBag.cityTileType = string.IsNullOrEmpty(cityTileType) ? "GFS" : cityTileType;

            try
            {
                if (x == 0 && y == 0)
                {
                    return View(new CityTileWebViewModel());
                }

            

                dimension = await _dimensionManager.GetDimension(zoom, x, y, false);
                if (dimension == null)
                    return View(new CityTileWebViewModel());
                switch (cityTileType)
                {
                    case "ECMWF":
                        var result = await _ecmwfCrawler.GetDimensionContentByTimeAsync(dimension, date);

                        return View(new CityTileWebViewModel() { CityTiles = new List<CityTile>(result) ,HasData = true});

                    case "GFS":
                    default:
                        var result1 = await _gfsCrawler.GetDimensionContentByTimeAsync(dimension, date);

                        return View(new CityTileWebViewModel() { CityTiles = new List<CityTile>(result1), HasData = true });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"CityTileWebController/Index {zoom} {x} {y} {date}");
                return View(new CityTileWebViewModel());
            }
        }
    }
}
