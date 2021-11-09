using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Shared.Entities;
using RH.Services.RestApi.Models;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.WindDimension;
using RH.Shared.HttpClient;

namespace RH.Services.RestApi.Controller
{
    public class ForecastWebController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<ForecastWebController> _logger;
        private readonly IWindDimensionManager _dimensionManager;

        private readonly EcmwfWindCrawler _ecmwfCrawler;
        private readonly GfsWindCrawler _gfsCrawler;

        public ForecastWebController(ILogger<ForecastWebController> logger, IWindDimensionManager dimensionManager, EcmwfWindCrawler ecmwfCrawler, GfsWindCrawler gfsCrawler)
        {
            _logger = logger;
            _dimensionManager = dimensionManager;
            _ecmwfCrawler = ecmwfCrawler;
            _gfsCrawler = gfsCrawler;
        }

        public async Task<IActionResult> Index(string forecastType, double x, double y, DateTime date)

        {
            WindDimension dimension;
            if (date == default)
            {
                date = DateTime.Now;
            }
            ViewBag.X = x;
            ViewBag.Y = y;
            ViewBag.Date = date.ToString("yyyy-MM-ddTHH:mm:ss");
            ViewBag.forecastType = string.IsNullOrEmpty(forecastType) ? "GFS" : forecastType;


            if (x == 0 && y == 0)
            {
                return View(new ForecastWebViewModel());
            }

            try
            {
                dimension = await _dimensionManager.GetDimension(x, y, false);
                if (dimension == null)
                    return View(new ForecastWebViewModel());

                switch (forecastType)
                {
                    case "ECMWF":
                        var result = await _ecmwfCrawler.GetDimensionContentByTimeAsync(dimension, date);

                        return View(new ForecastWebViewModel() { Forecasts = new List<Forecast>(result),HasData  = true});

                    case "GFS":
                    default:
                        var result1 = await _gfsCrawler.GetDimensionContentByTimeAsync(dimension, date);

                        return View(new ForecastWebViewModel() { Forecasts = new List<Forecast>(result1), HasData = true });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"ForecastWebController/Index  {x} {y} {date}");
                return View(new ForecastWebViewModel());
            }

        }

    }
}
