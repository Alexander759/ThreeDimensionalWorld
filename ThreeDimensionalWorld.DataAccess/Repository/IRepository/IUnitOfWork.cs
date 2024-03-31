using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorld.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IRepository<Product> ProductRepository { get; set; }
        IRepository<ProductFile> ProductFileRepository { get; set; }

        IRepository<Category> CategoryRepository { get; set; }

        IRepository<MaterialColor> MaterialColorRepository { get; set; }
        IRepository<Material> MaterialRepository { get; set; }
        
        IRepository<ShoppingCartItem> ShoppingCartItemRepository { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepository { get; set; }

        IRepository<OrderItem> OrderItemRepository { get; set; }
        IRepository<Order> OrderRepository { get; set; }

        IRepository<ApplicationUser> ApplicationUserRepository { get; set; }

        IRepository<Address> AddressRepository { get; set; }

        void Save();
    }
}
