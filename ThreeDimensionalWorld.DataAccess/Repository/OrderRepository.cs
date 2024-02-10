using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorld.DataAccess.Repository
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(ApplicationDbContext db)
            : base(db)
        {
        }
    }
}
