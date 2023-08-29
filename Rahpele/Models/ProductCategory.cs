using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class ProductCategory
    {
        [Key]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? IconName { get; set; }
        public string? Description { get; set; }

        public DateTime? CreateDate { get; set; }
        public bool? IsDeleted { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public ProductCategory? Parent { get; set; }

        public ICollection<Product>? Products { get; set; }


    }
}
