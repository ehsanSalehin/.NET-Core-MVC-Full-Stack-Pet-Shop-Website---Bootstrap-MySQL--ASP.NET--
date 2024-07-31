using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using Project.Models;

namespace Project.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDeatilsRepository
    {
        private ApplicationDbContext _db;
        public OrderDetailsRepository(ApplicationDbContext db) :base(db)  //base(db) ==> pass to repository
        {
            _db = db;
        }

        public void Update(OrderDetails obj)
        {
            _db.OrderDetails.Update(obj);
        }
    }
}
