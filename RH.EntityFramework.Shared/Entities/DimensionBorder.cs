using System;
using System.Collections.Generic;
using System.Text;

namespace RH.EntityFramework.Shared.Entities
{
    public class DimensionBorder
    {
        public int MaxZ { get; set; }
        public int MinZ { get; set; }

        public int MaxY { get; set; }
        public int MinY { get; set; }

        public int MaxX { get; set; }
        public int MinX { get; set; }
    }
}
