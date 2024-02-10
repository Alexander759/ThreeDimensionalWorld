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
        IRepository<ProductImageFile> ProductImageFileRepository { get; set; }
        IRepository<ProductFile3D> ProductFile3DRepository { get; set; }

        IRepository<Category> CategoryRepository { get; set; }

        IRepository<MaterialColor> MaterialColorRepository { get; set; }
        IRepository<Material> MaterialRepository { get; set; }
        
        IRepository<ShoppingCartItem> ShoppingCartItemRepository { get; set; }
        IRepository<ShoppingCart> ShoppingCartRepository { get; set; }

        IRepository<OrderItem> OrderItemRepository { get; set; }
        IRepository<Order> OrderRepository { get; set; }

        void Save();
    }
}
