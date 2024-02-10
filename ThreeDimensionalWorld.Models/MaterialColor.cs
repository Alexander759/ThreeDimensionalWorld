using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Models
{
    public class MaterialColor
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public List<Material> Materials { get; set; } = new List<Material>();
    }
}
