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

    public class GfsLevelForecast
    {
        public List<long> ts { get; set; }
        public List<double> wind_u_1000h { get; set; }
        public List<double> wind_u_950h { get; set; }
        public List<double> wind_u_925h { get; set; }
        public List<double> wind_u_900h { get; set; }
        public List<double> wind_u_850h { get; set; }
        public List<double> wind_u_800h { get; set; }
        public List<double> wind_u_700h { get; set; }
        public List<double> wind_u_600h { get; set; }
        public List<double> wind_u_500h { get; set; }
        public List<double> wind_u_400h { get; set; }
        public List<double> wind_u_300h { get; set; }
        public List<double> wind_u_200h { get; set; }
        public List<double> wind_u_150h { get; set; }

        public List<double> wind_v_1000h { get; set; }
        public List<double> wind_v_950h { get; set; }
        public List<double> wind_v_925h { get; set; }
        public List<double> wind_v_900h { get; set; }
        public List<double> wind_v_850h { get; set; }
        public List<double> wind_v_800h { get; set; }
        public List<double> wind_v_700h { get; set; }
        public List<double> wind_v_600h { get; set; }
        public List<double> wind_v_500h { get; set; }
        public List<double> wind_v_400h { get; set; }
        public List<double> wind_v_300h { get; set; }
        public List<double> wind_v_200h { get; set; }
        public List<double> wind_v_150h { get; set; }


        public List<double> dewpoint_1000h { get; set; }
        public List<double> dewpoint_950h { get; set; }
        public List<double> dewpoint_925h { get; set; }
        public List<double> dewpoint_900h { get; set; }
        public List<double> dewpoint_850h { get; set; }
        public List<double> dewpoint_800h { get; set; }
        public List<double> dewpoint_700h { get; set; }
        public List<double> dewpoint_600h { get; set; }
        public List<double> dewpoint_500h { get; set; }
        public List<double> dewpoint_400h { get; set; }
        public List<double> dewpoint_300h { get; set; }
        public List<double> dewpoint_200h { get; set; }
        public List<double> dewpoint_150h { get; set; }

        public List<double> rh_1000h { get; set; }
        public List<double> rh_950h { get; set; }
        public List<double> rh_925h { get; set; }
        public List<double> rh_900h { get; set; }
        public List<double> rh_850h { get; set; }
        public List<double> rh_800h { get; set; }
        public List<double> rh_700h { get; set; }
        public List<double> rh_600h { get; set; }
        public List<double> rh_500h { get; set; }
        public List<double> rh_400h { get; set; }
        public List<double> rh_300h { get; set; }
        public List<double> rh_200h { get; set; }
        public List<double> rh_150h { get; set; }

        public List<double> temp_1000h { get; set; }
        public List<double> temp_950h { get; set; }
        public List<double> temp_925h { get; set; }
        public List<double> temp_900h { get; set; }
        public List<double> temp_850h { get; set; }
        public List<double> temp_800h { get; set; }
        public List<double> temp_700h { get; set; }
        public List<double> temp_600h { get; set; }
        public List<double> temp_500h { get; set; }
        public List<double> temp_400h { get; set; }
        public List<double> temp_300h { get; set; }
        public List<double> temp_200h { get; set; }
        public List<double> temp_150h { get; set; }



    }
}