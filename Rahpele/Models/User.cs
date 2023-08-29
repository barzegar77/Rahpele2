using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? HashedPassword { get; set; }
        public string? AvatarThumbnail { get; set; }
        public string? AvatarStandard { get; set; }

        public string? Email { get; set; }
        public bool? IsEmailConfirmed { get; set; }
        public string? EmailConfirmationCode { get; set; }
        public DateTime? EmailConfirmationSentDateTime { get; set; }

        public string? PhoneNumber { get; set; }
        public bool? IsPhoneNumberConfirmed { get; set; }
        public string? PhoneNumberConfirmationCode { get; set; }
        public DateTime? PhoneNumberConfirmationSentDateTime { get; set; }

        public string? FullName { get; set; }
        public string? Biography { get; set; }
        public string? WebSiteLink { get; set; }
        public string? BusinessPhoneNumber { get; set; }

        public int? SubscriptionType { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public bool? IsSubscriptionActive { get; set; }
        public string? PaymentHistory { get; set; }

        public DateTime? RegistrationDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? LastVisitTime { get; set; }
        public bool? IsDeleted { get; set; }



        //seo
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug { get; set; }


        #region Relationships
        public Guid? CityId { get; set; }
        [ForeignKey(nameof(CityId))]
        public City? City { get; set; }

        public Guid? CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }

        public ICollection<Product>? UserProducts { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }


        [InverseProperty(nameof(AccountRate.Account))]
        public ICollection<AccountComment>? AccountComments { get; set; }


        [InverseProperty(nameof(AccountRate.Account))]
        public ICollection<AccountRate>? AccountRates { get; set; }


        [InverseProperty(nameof(AccountFollow.Follower))]
        public ICollection<AccountFollow>? Followers { get; set; }


        [InverseProperty(nameof(AccountFollow.Following))]
        public ICollection<AccountFollow>? Following { get; set; }

        #endregion
    }
}
