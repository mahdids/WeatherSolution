using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RH.Services.RestApi.Models
{
    public class ForecastWebViewModel
    {
        public int Zoom { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsGfs { get; set; }
    }
}
