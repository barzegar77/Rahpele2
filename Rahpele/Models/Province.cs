using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class Province
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Guid? CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }

        public ICollection<City?>? Cities { get; set; }
    }
}
