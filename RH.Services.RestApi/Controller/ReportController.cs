using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RH.EntityFramework.Repositories.Cycle;
using RH.Services.RestApi.Models;

namespace RH.Services.RestApi.Controller
{
    public class ReportController : Microsoft.AspNetCore.Mvc.Controller
    {
        private ICycleRepository _cycleRepository;

        public ReportController(ICycleRepository cycleRepository)
        {
            this._cycleRepository = cycleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var dblist = await _cycleRepository.GetCyclesAsync(1, 100);
            var returnList = new List<ReportWebViewModel>();
            foreach (var item in dblist)
            {
                returnList.Add(new ReportWebViewModel()
                {
                    Type = item.Type,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    dateTime = item.dateTime,
                    Compeleted = item.Compeleted
                });
            }
            return View(returnList);
        }
    }
}
