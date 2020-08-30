using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Tile;

namespace RH.Services.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly IDimensionManager _dimensionManager;
        private readonly ILogger<TilesController> _logger;
        private readonly ITileCrawler _tileCrawler;


        public TilesController(IDimensionManager dimensionManager, ILogger<TilesController> logger, ITileCrawler tileCrawler)
        {
            _dimensionManager = dimensionManager;
            _logger = logger;
            _tileCrawler = tileCrawler;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return NotFound();
        }

        [HttpGet("{zoom}/{x}/{y}")]
        public async Task<IActionResult> Get(short zoom, short x, short y)
        {
            var dimension = await _dimensionManager.GetDimension(zoom, x, y);
            if (dimension == null)
                return NotFound();

            var stream =  _tileCrawler.GetDimensionContentAsync(dimension);
            if (stream == null)
            {
                return NotFound();
            }

            return File(stream, "image/png");
        }
    }
}
