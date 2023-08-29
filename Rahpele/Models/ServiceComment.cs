using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class ServiceComment
    {
        [Key]
        public Guid Id { get; set; }

        public string? Text { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsDeleted { get; set; }


        public Guid? ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public Service? Service { get; set; }

        public Guid? RegisteredUserId { get; set; }
        [ForeignKey(nameof(RegisteredUserId))]
        public User? RegisteredUser { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public AccountComment? Parent { get; set; }
    }
}
