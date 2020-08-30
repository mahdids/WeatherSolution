using System;
using System.Collections.Generic;
using System.Text;

namespace RH.EntityFramework.Shared.Entities
{
    public class WindyTime
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public long Start { get; set; }
        public short Step { get; set; }
    }
}
