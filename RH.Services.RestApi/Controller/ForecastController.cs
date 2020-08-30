using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Forecast;

namespace RH.Services.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        private readonly IDimensionManager _dimensionManager;
        private readonly ILogger<TilesController> _logger;

        private readonly WindyEcmwfCrawler _ecmwfCrawler;
        private readonly WindyGfsCrawler _gfsCrawler;

        public ForecastController(IDimensionManager dimensionManager, ILogger<TilesController> logger, WindyEcmwfCrawler ecmwfCrawler, WindyGfsCrawler gfsCrawler)
        {
            _dimensionManager = dimensionManager;
            _logger = logger;
            _ecmwfCrawler = ecmwfCrawler;
            _gfsCrawler = gfsCrawler;
        }

        [HttpGet("gfs/{zoom}/{x}/{y}")]
        public async Task<IActionResult> GetGfs(short zoom, short x, short y)
        {
            var dimension = await _dimensionManager.GetDimension(zoom, x, y);
            if (dimension == null)
                return NotFound();
            var result = await _gfsCrawler.GetDimensionContentAsync(dimension);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            return Ok(result);


        }

        [HttpGet("ecmwf/{zoom}/{x}/{y}")]
        public async Task<IActionResult> GetEcmwf(short zoom, short x, short y)
        {
            var dimension = await _dimensionManager.GetDimension(zoom, x, y);
            if (dimension == null)
                return NoContent();
            var result =await _ecmwfCrawler.GetDimensionContentAsync(dimension);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            return Ok(result);

        }
    }
}
