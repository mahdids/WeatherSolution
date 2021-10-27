using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.Services.RestApi.Models;
using RH.Shared.HttpClient;

namespace RH.Services.RestApi.Controller
{
    public class ForecastWebController :Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<ForecastWebController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ForecastWebController(ILogger<ForecastWebController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index(bool gfs,int zoom,int x,int y ,DateTime time)
        {
            return View();
        }
        
    }
}
