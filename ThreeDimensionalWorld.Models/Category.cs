using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        public required string Name { get; set; }

        public required string Description { get; set; }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
