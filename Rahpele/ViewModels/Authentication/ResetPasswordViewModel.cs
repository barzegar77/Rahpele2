using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.Authentication
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "شماره همراه")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(11, ErrorMessage = "{0} باید 11 کاراکتر باشد.", MinimumLength = 11)]
        public string PhoneNumber { get; set; }


        [Display(Name = "کد تایید")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(6, ErrorMessage = "{0} باید 6 کاراکتر باشد.", MinimumLength = 6)]
        public string VerificationCode { get; set; }


        [Display(Name = "رمز عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(25, ErrorMessage = "{0} باید بین {2} کاراکتر تا {1} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "تکرار رمز عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "کلمه عبور و تکرار آن یکی نیست")]
        public string RePassword { get; set; }
    }
}
