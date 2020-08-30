namespace RH.EntityFramework.Shared.Entities
{
    public class Forecast
    {
        public long Id { get; set; }
        public long Start { get; set; }
        public short Step { get; set; }
        public string GFSContent { get; set; }
        public string ECMWFContent { get; set; }
        public int DimensionId { get; set; }

        public virtual Dimension Dimension { get; set; }
    }
}
