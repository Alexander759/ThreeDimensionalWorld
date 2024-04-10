using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class Material
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Име")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Описание")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Плътност")]
        [Range(typeof(decimal), "0", "10000", ErrorMessage = "Стойността за {0} трябва да бъде между {1} ​​и {2}.")]
        public required double Density { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Увеличение на цената")]
        [Range(typeof(decimal), "0", "10000", ErrorMessage = "Стойността за {0} трябва да бъде между {1} ​​и {2}.")]
        public decimal PriceIncrease { get; set; }

        public List<MaterialColor> Colors { get; set; } = new List<MaterialColor>();
    }
}
