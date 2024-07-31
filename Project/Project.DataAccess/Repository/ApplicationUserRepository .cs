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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) :base(db)  //base(db) ==> pass to repository
        {
            _db = db;
        }

        public void Update(ApplicationUser obj)
        {
            _db.ApplicationUser.Update(obj);
        }
    }
}
