using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int MaterialId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [ForeignKey("MaterialId")]
        public Material? Material { get; set; }

        [NotMapped]
        public decimal Price 
        { 
            get 
            {
                if (Product == null || Material == null)
                    throw new Exception("Couldn't calculate price");

                return Product.BasePrice + (1 + Material.PriceIncrease); 
            } 
        }
    }
}
