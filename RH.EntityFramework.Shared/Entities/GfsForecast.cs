using System;

namespace RH.EntityFramework.Shared.Entities
{
    public class GfsForecast
    {
        public long Id { get; set; }
        public int WindDimensionId { get; set; }
        public DateTime ReferenceTime { get; set; }
        public DateTime DateTime { get; set; }
        public long OrigTs { get; set; }
        public string WeatherCode { get; set; }
        public double Mm { get; set; }
        public double SnowPrecip { get; set; }
        public double ConvPrecip { get; set; }
        public bool Rain { get; set; }
        public bool Snow { get; set; }
        public double Temperature { get; set; }
        public double DewPoint { get; set; }
        public double Wind { get; set; }
        public int WindDirection { get; set; }
        public int Rh { get; set; }
        public double Gust { get; set; }
        public double Pressure { get; set; }
        public virtual WindDimension WindDimension { get; set; }
    }
}
