using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast;
using RH.Shared.Crawler.Forecast.CityTile;

namespace RH.Services.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityTileController : ControllerBase
    {
        private readonly IDimensionManager _dimensionManager;
        private readonly ILogger<TilesController> _logger;

        private readonly EcmwfCityTileCrawler _ecmwfCrawler;
        private readonly GfsCityTileCrawler _gfsCrawler;

        public CityTileController(IDimensionManager dimensionManager, ILogger<TilesController> logger, EcmwfCityTileCrawler ecmwfCrawler, GfsCityTileCrawler gfsCrawler)
        {
            _dimensionManager = dimensionManager;
            _logger = logger;
            _ecmwfCrawler = ecmwfCrawler;
            _gfsCrawler = gfsCrawler;
        }

        [HttpGet("gfs/{zoom}/{x}/{y}")]
        public async Task<IActionResult> GetGfs(short zoom, short x, short y,long epocTime)
        {
            Dimension dimension;
            string result;
            if (epocTime == 0)
            {
                dimension = await _dimensionManager.GetDimension(zoom, x, y);
                if (dimension == null)
                    return NotFound();
                result = await _gfsCrawler.GetDimensionContentAsync(dimension);
                if (string.IsNullOrEmpty(result))
                    return NoContent();
                return Ok(result);
            }
            dimension = await _dimensionManager.GetDimension(zoom, x, y,false);
            if (dimension == null)
                return NotFound();
            result = await _gfsCrawler.GetDimensionContentByTimeAsync(dimension,epocTime);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            return Ok(result);

        }

        [HttpGet("ecmwf/{zoom}/{x}/{y}")]
        public async Task<IActionResult> GetEcmwf(short zoom, short x, short y, long epocTime)
        {
            Dimension dimension;
            string result;
            if (epocTime == 0)
            {
                dimension = await _dimensionManager.GetDimension(zoom, x, y);
                if (dimension == null)
                    return NotFound();
                result = await _ecmwfCrawler.GetDimensionContentAsync(dimension);
                if (string.IsNullOrEmpty(result))
                    return NoContent();
                return Ok(result);
            }
            dimension = await _dimensionManager.GetDimension(zoom, x, y, false);
            if (dimension == null)
                return NotFound();
            result = await _ecmwfCrawler.GetDimensionContentByTimeAsync(dimension, epocTime);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            return Ok(result);
        }
    }
}
