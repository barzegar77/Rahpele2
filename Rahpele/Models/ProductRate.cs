using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class ProductRate
    {
        [Key]
        public Guid Id { get; set; }

        public byte? Score { get; set; }

        public Guid? ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        public Guid RegisteredUserId { get; set; }
        [ForeignKey(nameof(RegisteredUserId))]
        public User? RegisteredUser { get; set; }
    }
}
