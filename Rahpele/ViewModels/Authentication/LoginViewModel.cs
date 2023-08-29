using System.ComponentModel.DataAnnotations;

namespace Rahpele.ViewModels.Authentication
{
    public class LoginViewModel
    {
        [Display(Name = "شماره همراه")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        public string PhoneNumber { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool? RememberMe { get; set; }
    }
}