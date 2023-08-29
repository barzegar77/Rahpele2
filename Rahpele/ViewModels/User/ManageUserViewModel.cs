using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.User
{
    public class ManageUserViewModel
    {
        public Guid? Id { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(100)]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(100, MinimumLength = 3)]
        public string? Password { get; set; }

        [Display(Name = "ایمیل")]
        [StringLength(150, MinimumLength = 5)]
        public string? Email { get; set; }

        [Display(Name = "تاییدیه ایمیل")]
        public bool IsEmailConfirmed { get; set; } = false;

        [Display(Name = "شماره موبایل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Display(Name = "تاییدیه شماره موبایل")]
        public bool IsPhoneNumberConfirmed { get; set; } = false;

        [Display(Name = "نام و نام خانوادگی")]
        [StringLength(150, MinimumLength = 3)]
        public string? FullName { get; set; }

        [Display(Name = "بیوگرافی")]
        [StringLength(500, MinimumLength = 3)]
        public string? Biography { get; set; }

        [Display(Name = "لینک وبسایت")]
        [StringLength(100, MinimumLength = 5)]
        public string? WebSiteLink { get; set; }

        [Display(Name = "شماره بیزنسی")]
        [StringLength(9)]
        public string? BusinessPhoneNumber { get; set; }

        [Display(Name = "وضعیت حساب کاربری")]
        public bool IsDeleted { get; set; } = false;
    }
}
