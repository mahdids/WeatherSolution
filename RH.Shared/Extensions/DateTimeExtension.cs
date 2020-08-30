using System;
using System.Collections.Generic;
using System.Text;
using RH.Shared.Common;

namespace RH.Shared.Extensions
{
    public static class DateTimeExtension
    {
        public static WindyUnixTime ToWindyUnixTime(this DateTime dateTime,short step)
        {
            var utcDateTime = dateTime.ToUniversalTime();
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            var startDate=new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day);
            var interval = (short) ((utcDateTime - startDate).Hours / step);
            var refrenceDateTime = startDate.AddHours(interval * step);
            var unixDateTime = (long)(refrenceDateTime- epoch).TotalMilliseconds;

            return new WindyUnixTime(unixDateTime,step);
        }
    }
}
