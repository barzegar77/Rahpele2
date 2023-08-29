using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.Location
{
    public class ManageCityViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "نام شهر")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(70)]
        public string Name { get; set; }

        [Display(Name = "استان مربوطه")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        public Guid ProvinceId { get; set; }
    }
}
