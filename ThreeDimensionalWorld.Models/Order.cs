using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class Order
    {
        public int Id { get; set; }

        public required string UserId { get; set; }

        public required int OrderItemId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey("OrderItemId")]
        public OrderItem? OrderItem { get; set; }
    }
}
