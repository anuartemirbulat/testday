namespace DAL.Core.Redis.BaseEntities
{
    public abstract class Document:IDocument
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
