using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        public string? Description { get; set; }
        public int? Views { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreateDate { get; set; }


        //seo
        public string? CanonicalLink { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug { get; set; }


        public Guid? RegisteredUserId { get; set; }
        [ForeignKey(nameof(RegisteredUserId))]
        [InverseProperty(nameof(User.UserProducts))]
        public User? RegisteredUser { get; set; }

        public Guid? ProductCategoryId { get; set; }
        [ForeignKey(nameof(ProductCategoryId))]
        public ProductCategory? ProductCategory { get; set; }

        public ICollection<ProductComment>? ProductComments { get; set; }
        public ICollection<ProductRate>? ProductLikes { get; set; }

        public ICollection<ProductFile>? ProductFiles { get; set; }
    }
}
