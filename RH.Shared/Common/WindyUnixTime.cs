using System;
using System.Collections.Generic;
using System.Text;
using RH.Shared.Extensions;

namespace RH.Shared.Common
{
    public class WindyUnixTime
    {
        public WindyUnixTime()
        {
        }

        public WindyUnixTime(long start, short step)
        {
            Start = start;
            Step = step;
        }
        
        public long Start { get; set; }
        public short Step { get; set; }
    }
}
