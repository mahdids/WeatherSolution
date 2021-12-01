using System;
using System.Collections.Generic;
using System.Text;

namespace RH.EntityFramework.Shared.Entities
{
    public class Cycle
    {
        public long Id { get; set; }
        public string Type { get; set; }

        public bool Compeleted { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public DateTime? dateTime { get; set; }
    }
}
