﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RH.EntityFramework.Shared.Entities;

namespace RH.Services.RestApi.Models
{
    public class ForecastWebViewModel
    {
        public ForecastWebViewModel()
        {
            Forecasts = new List<Forecast>();
        }
        public bool HasData { get; set; }

        public List<Forecast> Forecasts { get; set; }
    }
}
