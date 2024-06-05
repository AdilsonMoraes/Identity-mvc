namespace Identity.Domain.Entities
{
    public class AuditableEntity : BaseEntity
    {
        public string CreatedBy { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDateTime { get; set; }
    }
}
