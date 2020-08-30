using System.Collections.Generic;

namespace RH.EntityFramework.Shared.Entities
{
    public class Dimension
    {
        public Dimension()
        {
            this.Labels = new HashSet<Label>();
            this.Forecasts = new HashSet<Forecast>();
        }

        public int Id { get; set; }
        public short Zoom { get; set; }
        public short X { get; set; }
        public short Y { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Label> Labels { get; set; }
        public virtual ICollection<Forecast> Forecasts { get; set; }
    }
}
