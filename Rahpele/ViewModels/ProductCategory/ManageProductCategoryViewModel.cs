using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.ProductCategory
{
    public class ManageProductCategoryViewModel
    {
        public Guid Id { get; set; }


        [Display(Name = "عنوان")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Display(Name = "عنوان آیکون")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(50, MinimumLength = 3)]
        public string IconName { get; set; }

        [Display(Name = "توضیحات")]
        [StringLength(500, MinimumLength = 3)]
        public string? Description { get; set; }

        [Display(Name = "والد")]
        public Guid? ParentId { get; set; }
    }
}
