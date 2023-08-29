using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rahpele.Models
{
    public class AccountRate
    {
        [Key]
        public Guid Id { get; set; }

        public int? Rate { get; set; }

        public Guid? AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        [InverseProperty(nameof(User.AccountRates))]
        public User? Account { get; set; }

        public Guid? RegisteredUserId { get; set; }
        [ForeignKey(nameof(RegisteredUserId))]
        public User? RegisteredUser { get; set; }
    }
}
