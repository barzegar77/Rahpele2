using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class AccountFollow
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? FollowerId { get; set; }
        [ForeignKey(nameof(FollowerId))]
        [InverseProperty(nameof(User.Followers))]
        public User? Follower { get; set; }

        public Guid? FollowingId { get; set; }
        [ForeignKey(nameof(FollowingId))]
        [InverseProperty(nameof(User.Following))]
        public User? Following { get; set; }
    }
}
