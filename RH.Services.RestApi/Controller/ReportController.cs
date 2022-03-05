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
                var d = !item.Compeleted && dblist.Any(x => x.Type == item.Type && x.EndTime != null)
                    ? item.StartTime.AddSeconds(dblist.Where(x => x.Type == item.Type && x.EndTime != null)
                        .Average(x => ((DateTime)x.EndTime - x.StartTime).TotalSeconds))
                    : item.EndTime;
                if (!item.Compeleted&& dblist.Any(x => x.Type == item.Type && x.EndTime != null))
                {
                    var t = dblist.Where(x => x.Type == item.Type && x.EndTime != null)
                        .Average(x => ((DateTime)x.EndTime - x.StartTime).TotalSeconds);
                }
                returnList.Add(new ReportWebViewModel()
                {
                    Type = item.Type,
                    StartTime = item.StartTime,
                    EndTime = !item.Compeleted&&dblist.Any(x=>x.Type==item.Type &&x.EndTime!=null)?item.StartTime.AddSeconds(dblist.Where(x=>x.Type==item.Type &&x.EndTime!=null).Average(x=>((DateTime)x.EndTime-x.StartTime).TotalSeconds)):item.StartTime.AddMinutes(250),
                    dateTime = item.dateTime,
                    Compeleted = item.Compeleted,
                    
                });
            }
            return View(returnList);
        }
    }
}
