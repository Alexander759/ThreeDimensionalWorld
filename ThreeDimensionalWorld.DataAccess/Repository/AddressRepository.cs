using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorld.DataAccess.Repository
{
    public class AddressRepository : Repository<Address>
    {
        public AddressRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
