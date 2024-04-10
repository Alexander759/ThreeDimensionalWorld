using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Заглавие")]
        [MinLength(2, ErrorMessage = "Заглавието трябва да бъде поне {1} символа.")]
        [MaxLength(50, ErrorMessage = "Заглавието не може да бъде по-дълго от {1} символа.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Описание")]
        [MinLength(100, ErrorMessage = "Описанието трябва да бъде поне {1} символа.")]
        [MaxLength(5000, ErrorMessage = "Описанието не може да бъде по-дълго от {1} символа.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Базова цена")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "Стойността за {0} трябва да бъде между {1} ​​и {2}.")]
        public decimal BasePrice { get; set; }
        
        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Ширина")]
        [Range(0.01, 2000, ErrorMessage = "Стойността за {0} трябва да бъде между {1} ​​и {2}.")]
        public double Width { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Range(0.01, 2000, ErrorMessage = "Стойността за {0} трябва да бъде между {1} ​​и {2}.")]
        [Display(Name = "Дължина")]
        public double Length { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Range(0.01, 2000, ErrorMessage = "Стойността за {0} трябва да бъде между {1} ​​и {2}.")]
        [Display(Name = "Височина")]
        public double Height { get; set; }

        [AllowNull]
        [Display(Name = "Линк към източника")]
        public string? AttributionLink { get; set; }

        [Display(Name = "Активен ли е")]
        public bool IsActive { get; set; }

        [ForeignKey("CategoryId")]
        [Display(Name = "Категория")]
        public Category? Category { get; set; }

        public IEnumerable<ProductFile> Files { get; set; } = new List<ProductFile>();

        public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
