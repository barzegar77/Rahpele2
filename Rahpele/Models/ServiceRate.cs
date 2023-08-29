using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class ServiceRate
    {
        [Key]
        public Guid Id { get; set; }

        public byte? Score { get; set; }

        public Guid? ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public Service? Service { get; set; }

        public Guid RegisteredUserId { get; set; }
        [ForeignKey(nameof(RegisteredUserId))]
        public User? RegisteredUser { get; set; }
    }
}
