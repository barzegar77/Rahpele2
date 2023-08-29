using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rahpele.Models
{
    public class ProductComment
    {
        [Key]
        public Guid Id { get; set; }

        public string? Text { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsDeleted { get; set; }


        public Guid? ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        public Guid? RegisteredUserId { get; set; }
        [ForeignKey(nameof(RegisteredUserId))]
        public User? RegisteredUser { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public AccountComment? Parent { get; set; }
    }
}
