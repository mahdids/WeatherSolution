using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RH.Services.RestApi.Models
{
    public class ReportWebViewModel
    {
        public string Type { get; set; }

        public bool Compeleted { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public DateTime? dateTime { get; set; }

        public string TimeDiff => Compeleted ? $"{(EndTime.Value - StartTime).Minutes}" : "-";

        public string ColorClass => Compeleted ? "" : "table-info";
        public string Status { get; set; }
    }
}
