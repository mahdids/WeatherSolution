using System;
using System.Collections.Generic;
using System.Text;

namespace RH.EntityFramework.Shared.Entities
{
    public class WindDimension
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsActive { get; set; }
    }
}
