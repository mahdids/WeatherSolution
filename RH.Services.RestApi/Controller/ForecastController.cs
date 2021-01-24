using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Shared.Entities;
using RH.Shared.Crawler.Forecast.Wind;
using RH.Shared.Crawler.WindDimension;

namespace RH.Services.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        private readonly IWindDimensionManager _dimensionManager;
        private readonly ILogger<ForecastController> _logger;

        private readonly EcmwfWindCrawler _ecmwfCrawler;
        private readonly GfsWindCrawler _gfsCrawler;

        public ForecastController(IWindDimensionManager dimensionManager, ILogger<ForecastController> logger, EcmwfWindCrawler ecmwfCrawler, GfsWindCrawler gfsCrawler)
        {
            _dimensionManager = dimensionManager;
            _logger = logger;
            _ecmwfCrawler = ecmwfCrawler;
            _gfsCrawler = gfsCrawler;
        }

        [HttpGet("gfs/{x}/{y}")]
        public async Task<IActionResult> GetGfs(short x, short y, long epocTime)
        {
            WindDimension dimension;
            string result;
            if (epocTime == 0)
            {
                dimension = await _dimensionManager.GetDimension( x, y);
                if (dimension == null)
                    return NotFound();
                result = await _gfsCrawler.GetDimensionContentAsync(dimension);
                if (string.IsNullOrEmpty(result))
                    return NoContent();
                return Ok(result);
            }
            dimension = await _dimensionManager.GetDimension(x, y, false);
            if (dimension == null)
                return NotFound();
            result = await _gfsCrawler.GetDimensionContentByTimeAsync(dimension, epocTime);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            return Ok(result);

        }

        [HttpGet("ecmwf/{x}/{y}")]
        public async Task<IActionResult> GetEcmwf(short x, short y, long epocTime)
        {
            WindDimension dimension;
            string result;
            if (epocTime == 0)
            {
                dimension = await _dimensionManager.GetDimension(x, y);
                if (dimension == null)
                    return NotFound();
                result = await _ecmwfCrawler.GetDimensionContentAsync(dimension);
                if (string.IsNullOrEmpty(result))
                    return NoContent();
                return Ok(result);
            }
            dimension = await _dimensionManager.GetDimension(x, y, false);
            if (dimension == null)
                return NotFound();
            result = await _ecmwfCrawler.GetDimensionContentByTimeAsync(dimension, epocTime);
            if (string.IsNullOrEmpty(result))
                return NoContent();
            return Ok(result);

        }
    }
}
