using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderDeatilsRepository OrderDeatils { get; }
        IOrderHeaderRepository OrderHeader { get; }

        void Save();//since we are having save in all the repositories =>unitofwork (we only define it once)
    }
}
