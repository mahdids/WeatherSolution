using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RH.Shared.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RH.Services.RestApi.Controller
{
    public class TilesWebController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<TilesWebController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public IActionResult Index()
        {
            return View();
        }
    }
}
