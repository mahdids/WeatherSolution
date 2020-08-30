namespace RH.EntityFramework.Shared.Entities
{
    public class Label
    {
        public int Id { get; set; }
        public string O { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int ExtraField1 { get; set; }
        public int ExtraField2 { get; set; }
        public string FullText { get; set; }
        public System.DateTime RegisterDate { get; set; }
        public int DimensionId { get; set; }

        public virtual Dimension Dimension { get; set; }
    }
}
