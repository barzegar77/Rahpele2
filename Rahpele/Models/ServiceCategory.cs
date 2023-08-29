using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rahpele.Models
{
    public class ServiceCategory
    {
        [Key]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? IconName { get; set; }
        public string? Description { get; set; }


        //seo
        public string? CanonicalLink { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Slug { get; set; }


        public DateTime? CreateDate { get; set; }
        public bool? IsDeleted { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public ServiceCategory? Parent { get; set; }

        public ICollection<Service>? Services { get; set; }
    }
}
