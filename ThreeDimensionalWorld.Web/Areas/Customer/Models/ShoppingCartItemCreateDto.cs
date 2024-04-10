using System.ComponentModel.DataAnnotations;

namespace ThreeDimensionalWorld.Web.Areas.Customer.Models
{
    public class ShoppingCartItemCreateDto
    {
        public required int ProductId { get; set; }

        public required int MaterialId { get; set; }

        public required int ColorId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
