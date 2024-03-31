using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeDimensionalWorld.DataAccess.Data;
using ThreeDimensionalWorld.DataAccess.Repository.IRepository;
using ThreeDimensionalWorld.Models;

namespace ThreeDimensionalWorld.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            ProductRepository = new ProductRepository(_db);
            ProductFileRepository = new ProductFileRepository(_db);

            CategoryRepository = new CategoryRepository(_db);

            MaterialColorRepository = new MaterialColorRepository(_db);
            MaterialRepository = new MaterialRepository(_db);

            ShoppingCartItemRepository = new ShoppingCartItemRepository(_db);
            ShoppingCartRepository = new ShoppingCartRepository(_db);

            OrderItemRepository = new OrderItemRepository(_db);
            OrderRepository = new OrderRepository(_db);

            ApplicationUserRepository = new ApplicationUserRepository(_db);

            AddressRepository = new AddressRepository(_db);
        }

        public IRepository<Product> ProductRepository { get; set; }
        public IRepository<ProductFile> ProductFileRepository { get; set; }
        
        public IRepository<Category> CategoryRepository { get; set; }
        
        public IRepository<MaterialColor> MaterialColorRepository { get; set; }
        public IRepository<Material> MaterialRepository { get; set; }
        
        public IRepository<ShoppingCartItem> ShoppingCartItemRepository { get; set; }
        public IRepository<ShoppingCart> ShoppingCartRepository { get; set; }
        
        public IRepository<OrderItem> OrderItemRepository { get; set; }
        public IRepository<Order> OrderRepository { get; set; }

        public IRepository<ApplicationUser> ApplicationUserRepository { get; set; }
        
        public IRepository<Address> AddressRepository { get; set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
