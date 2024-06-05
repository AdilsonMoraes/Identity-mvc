using System.ComponentModel.DataAnnotations;

namespace Identity.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
