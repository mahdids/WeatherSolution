using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;

namespace RH.Services.RestApi.Controller
{
    public class SettingController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ILogger<SettingController> _logger;
        private readonly ISystemSettingRepository _settingRepository;
        public SettingController(ISystemSettingRepository settingRepository, ILogger<SettingController> logger)
        {
            _logger = logger;
            _settingRepository = settingRepository;
        }

        // GET: SettingController
        public async Task<ActionResult> Index()
        {
            //try
            //{
                var settings =await _settingRepository.GetSystemSettingsAsync();
            //}
            //catch (Exception e)
            //{
                

            //}
            return View(settings);
        }

        // GET: SettingController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var setting = await _settingRepository[id];
            return View(setting);
        }

        // GET: SettingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SettingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SettingController/Edit/5
        
    }
}
