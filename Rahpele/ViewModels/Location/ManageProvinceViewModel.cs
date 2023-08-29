using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.Location
{
    public class ManageProvinceViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "نام استان")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(70)]
        public string Name { get; set; }

        [Display(Name = "کشور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        public Guid CountryId { get; set; }
    }
}
