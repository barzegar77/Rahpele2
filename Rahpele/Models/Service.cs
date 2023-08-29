using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class Service
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


        #region rel

        public Guid? ServiceCategoryId { get; set; }
        [ForeignKey(nameof(ServiceCategoryId))]
        public ServiceCategory? ServiceCategory { get; set; }


        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }


        public ICollection<ServicePlan?>?  ServicePlans { get; set; }

        public ICollection<ServiceRate?>? ServiceRates { get; set; }

        #endregion
    }
}
