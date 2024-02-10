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

        public required string Name { get; set; }

        public required string Description { get; set; }

        public List<MaterialColor> Colors { get; set; } = new List<MaterialColor>();

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceIncrease { get; set; }
    }
}
