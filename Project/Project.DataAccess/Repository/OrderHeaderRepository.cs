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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) :base(db)  //base(db) ==> pass to repository
        
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.OrderHeader.Update(obj);
        }

        void IOrderHeaderRepository.UpdateStatus(int id, string orderStatus, string? paymentStatus)
        {
            var orderFromDb = _db.OrderHeader.FirstOrDefault(u => u.Id == id);
            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        void IOrderHeaderRepository.UpdateStripePayment(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.OrderHeader.FirstOrDefault(u => u.Id == id);
            if(!string.IsNullOrEmpty(sessionId)) {
                orderFromDb.SessionId= sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntetId = paymentIntentId;
                orderFromDb.PaymentDate=DateTime.Now;
            }
        }
    }
}
