using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreeDimensionalWorld.Web.Areas.Admin.Models
{
    public class ShippingPriceVM
    {
        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Цена на поръчка")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
    }
}
