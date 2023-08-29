using Rahpele.Models;
using System.ComponentModel.DataAnnotations;

namespace Rahpele.ViewModels.RolePermission
{
    public class ManageRoleViewModel
    {
        public Guid Id { get; set; }


        [Display(Name = "عنوان")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        public List<Guid?>? PermissionIds { get; set; }
        public string? PermissionIdsJson { get; set; }
    }
}
