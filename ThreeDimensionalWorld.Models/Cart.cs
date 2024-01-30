using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public required string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
