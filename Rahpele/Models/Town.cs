using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class Town
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Guid? CityId { get; set; }
        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }
    }
}
