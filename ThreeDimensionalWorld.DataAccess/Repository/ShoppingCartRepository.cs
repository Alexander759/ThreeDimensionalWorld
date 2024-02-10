using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorld.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>
    {
        public ShoppingCartRepository(ApplicationDbContext db) 
            : base(db)
        {
        }
    }
}
