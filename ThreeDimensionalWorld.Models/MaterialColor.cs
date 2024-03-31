using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class MaterialColor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Име")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [RegularExpression("^#?([a-f0-9]{6}|[a-f0-9]{3})$", ErrorMessage = "Невалиден код за цвят")]
        [Display(Name = "Цвят")]
        public required string ColorCode { get; set; }

        public List<Material> Materials { get; set; } = new List<Material>();
    }
}
