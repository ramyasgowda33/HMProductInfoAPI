namespace HMProductInfoAPI.Models
{
    public abstract class Entity
    {
        //public Guid Id { get; set; }
        public string CreatedByUser { get; set; } = "unknown";
        public DateTimeOffset CreatedDate { get; set; }
    }
}
