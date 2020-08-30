using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Label;
using RH.Shared.Crawler.Dimension;
using RH.Shared.Crawler.Label;

namespace RH.Services.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        
        private ILabelCrawler LabelCrawler { get; }
        private readonly IDimensionManager _dimensionManager;

        private readonly ILogger<LabelsController> _logger;

        public LabelsController(ILogger<LabelsController> logger, ILabelCrawler labelCrawler, IDimensionManager dimensionManager)
        {
            _logger = logger;
            LabelCrawler = labelCrawler;
            _dimensionManager = dimensionManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("[]");
        }

        [HttpGet("{zoom}/{x}/{y}")]
        public async Task<IActionResult> Get(short zoom, short x, short y)
        {
            var dimension = await _dimensionManager.GetDimension(zoom, x, y);
            if (dimension == null)
                return Ok("[]");
            var returnValue =await LabelCrawler.GetDimensionContentAsync(dimension);
            return Ok(returnValue);
        }
    }
}
