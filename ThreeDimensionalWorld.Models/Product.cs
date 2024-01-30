using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        public required string Title { get; set; }

        [MinLength(10)]
        [MaxLength(5000)]
        public required string Description { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public double Discount { get; set; }
        
        public IEnumerable<ImageFile> Images { get; set; } = new List<ImageFile>();

        public IEnumerable<File3D> File3Ds { get; set; } = new List<File3D>();

        public IEnumerable<Cart> Carts { get; set; } = new List<Cart>();
    }
}
