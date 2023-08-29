using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.User
{
    public class ManageRoleToUserViewModel
    {
        [Display(Name = "شناسه کاربری")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        public Guid UserId { get; set; }
        public List<Guid?>? RoleIds { get; set; }
        public string? RoleIdsJson { get; set; }
    }
}
