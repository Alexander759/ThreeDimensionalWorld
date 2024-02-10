using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class ProductFile3D
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public required int Order { get; set; }

        public double ProportionX { get; set; }
        public double ProportionY { get; set; }
        public double ProportionZ { get; set; }

        public required int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
