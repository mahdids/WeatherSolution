using System;
using System.Collections.Generic;
using System.Text;

namespace RH.Shared.Crawler.Helper
{
    public class WindRecord
    {
        public Header Header { get; set; }
        public Data Data { get; set; }
    }

    public class Header
    {
        public string RefTime { get; set; }
        public string Update { get; set; }
        public int DaysAvail { get; set; }
    }
    public class Data
    {
        public List<string> Day { get; set; }
        public List<int> Hour { get; set; }
        public List<long> Ts { get; set; }

        public List<long> OrigTs { get; set; }

        public List<string> OrigDate { get; set; }

        public List<string> Weathercode { get; set; }

        public List<double> Mm { get; set; }
        public List<double> SnowPrecip { get; set; }
        public List<double> ConvPrecip { get; set; }
        public List<double> Temp { get; set; }
        public List<double> DewPoint { get; set; }
        public List<double> Wind { get; set; }
        public List<int> WindDir { get; set; }
        public List<int> Rh { get; set; }
        public List<double> Gust { get; set; }
        public List<double> Pressure { get; set; }
        public List<bool> Rain { get; set; }
        public List<bool> Snow { get; set; }


    }
}
