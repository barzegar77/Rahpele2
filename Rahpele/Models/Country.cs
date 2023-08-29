using System.ComponentModel.DataAnnotations;

namespace Rahpele.Models
{
    public class Country
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public ICollection<Province?>? Provinces { get; set; }
    }
}
