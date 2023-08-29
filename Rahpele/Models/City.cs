using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class City
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Guid? ProvinceId { get; set; }
        [ForeignKey(nameof(ProvinceId))]
        public Province? Province { get; set; }

        public ICollection<Town?>? Towns { get; set; }
    }
}
