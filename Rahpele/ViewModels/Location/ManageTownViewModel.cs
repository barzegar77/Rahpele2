using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rahpele.ViewModels.Location
{
    public class ManageTownViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "نام محله")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        [StringLength(70)]
        public string Name { get; set; }

        [Display(Name = "شهر مربوطه")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا {0} را وارد نمایید")]
        public Guid CityId { get; set; }
    }
}
