using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderDeatilsRepository OrderDeatils { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(db);
            Product = new ProductRepository(db);
            ShoppingCart = new ShoppingCartRepository(db);
            ApplicationUser = new ApplicationUserRepository(db);
            OrderHeader = new OrderHeaderRepository(db);
            OrderDeatils = new OrderDetailsRepository(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
