using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.Location
{
    public class ManageCountryViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "نام کشور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(70)]
        public string Name { get; set; }
    }
}
