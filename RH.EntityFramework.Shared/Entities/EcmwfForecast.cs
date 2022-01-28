using System;
using Microsoft.EntityFrameworkCore;

namespace RH.EntityFramework.Shared.Entities
{

    public class Forecast
    {
        public long Id { get; set; }
        public int WindDimensionId { get; set; }
        public DateTime ReferenceTime { get; set; }
        public DateTime DateTime { get; set; }
        [Comment("Epoch Time")]
        public long OrigTs { get; set; }
        public string WeatherCode { get; set; }
        public double Mm { get; set; }
        [Comment("Millimeter")]
        public double SnowPrecip { get; set; }
        [Comment("Millimeter")]
        public double ConvPrecip { get; set; }
        public bool Rain { get; set; }
        public bool Snow { get; set; }
        [Comment("Kelvin")]
        public double Temperature { get; set; }
        [Comment("Kelvin")]
        public double DewPoint { get; set; }
        [Comment("m/s")]
        public double Wind { get; set; }
        [Comment("Degree")]
        public int WindDirection { get; set; }
        [Comment("Percent")]
        public int Rh { get; set; }
        [Comment("m/s")]
        public double Gust { get; set; }
        [Comment("Pascal")]
        public double Pressure { get; set; }
        public virtual WindDimension WindDimension { get; set; }
        public ForecastLevel Level { get; set; }

    }

    public class EcmwfForecast : Forecast
    {
        public new ForecastLevel Level = ForecastLevel.Surface;
    }

    public class GfsForecast : Forecast
    {
        
    }

    public enum ForecastLevel : byte
    {
        Surface=0,
        H1000=1,
        H950=2, 
        H925=3,
        H900=4,
        H850=5,
        H800=6,
        H700=7, 
        H600=8,
        H500=9,
        H400=10, 
        H300=11, 
        H200=12, 
        H150=13
    }


}
