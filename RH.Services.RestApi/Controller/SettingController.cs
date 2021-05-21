using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RH.EntityFramework.Repositories.Settings;
using RH.EntityFramework.Shared.Entities;

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

       
       

        // GET: SettingController/Create
        public async Task<ActionResult> Create()
        {
            var activeSetting = await _settingRepository.GetActiveSystemSetting();
            var newModel = new SystemSettings()
            {
                CrawlingInterval = activeSetting.CrawlingInterval,
                Dimension = activeSetting.Dimension,
                WindDimensions = activeSetting.WindDimensions,
                BaseWorkerSetting = activeSetting.BaseWorkerSetting,
                CrawlWebPath = activeSetting.CrawlWebPath
            };
            return View(newModel);
        }

        // POST: SettingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SystemSettings model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                await _settingRepository.AddDimensionAsync(model);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SettingController/Edit/5
        
    }
}
