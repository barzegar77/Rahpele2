using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.RolePermission
{
    public class ManagePermissionViewModel
    {
        public Guid Id { get; set; }


        [Display(Name = "عنوان")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Display(Name = "والد")]
        public Guid? ParentId { get; set; }
    }
}
