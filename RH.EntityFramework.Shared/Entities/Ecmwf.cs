﻿using System;
using Microsoft.EntityFrameworkCore;

namespace RH.EntityFramework.Shared.Entities
{
    public class CityTile
    {
        public long Id { get; set; }
        //public long Start { get; set; }
        //public short Step { get; set; }
        public string Location { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public DateTime RegisterDate { get; set; }
        [Comment("Kelvin")]
        public string DataString { get; set; }
        public int DimensionId { get; set; }

        public int WindyTimeId { get; set; }
        public virtual Dimension Dimension { get; set; }

        public virtual WindyTime WindyTime { get; set; }

        public DateTime WindyDateTime=> DateTimeOffset.FromUnixTimeMilliseconds(WindyTime.Start).DateTime;

    }

    public class Ecmwf : CityTile
    {

    }
}
