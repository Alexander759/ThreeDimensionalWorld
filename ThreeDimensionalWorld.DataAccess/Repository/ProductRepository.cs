using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorld.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>
    {
        public ProductRepository(ApplicationDbContext db)
            : base(db)
        {
        }

        //TODO: override it
        public override void Update(Product entity)
        {
            base.Update(entity);
        }
    }
}
