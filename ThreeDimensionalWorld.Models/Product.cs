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
        [MinLength(10, ErrorMessage = "Описанието трябва да бъде поне {1} символа.")]
        [MaxLength(5000, ErrorMessage = "Описанието не може да бъде по-дълго от {1} символа.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Базова цена")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Ширина")]
        public double Width { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Дължина")]
        public double Length { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Височина")]
        public double Height { get; set; }

        [AllowNull]
        [Display(Name = "Линк към източника")]
        public string? AttributionLink { get; set; }

        [Display(Name = "Активен ли е")]
        public bool IsActive { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public IEnumerable<ProductFile> Files { get; set; } = new List<ProductFile>();

        public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
