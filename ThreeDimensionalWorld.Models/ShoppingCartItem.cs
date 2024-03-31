using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Продукт")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Материал")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Цвят")]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Количка")]
        public int ShoppingCartId { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително!")]
        [Display(Name = "Количество")]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [ForeignKey("MaterialId")]
        public Material? Material { get; set; }

        [ForeignKey("ColorId")]
        public MaterialColor? Color { get; set; }

        [ForeignKey("ShoppingCartId")]
        public ShoppingCart? ShoppingCart { get; set; }

        public decimal GetPricePerUnit()
        {
            if (Product == null || Material == null)
                throw new Exception("Couldn't calculate price");


            return Product.BasePrice * (1 + Material.PriceIncrease / 100m);
        }

        public decimal GetPrice()
        { 
            if (Product == null || Material == null)
                throw new Exception("Couldn't calculate price");

                
            return Product.BasePrice * (1 + Material.PriceIncrease / 100m) * Quantity; 
        }
    }
}
