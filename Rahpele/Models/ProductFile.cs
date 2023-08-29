using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class ProductFile
    {
        [Key]
        public Guid Id { get; set; }

        public string? FileName { get; set; }

        public DateTime? CreateDate { get; set; }

        public bool? IsDeleted { get; set; }

        public Guid? ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
