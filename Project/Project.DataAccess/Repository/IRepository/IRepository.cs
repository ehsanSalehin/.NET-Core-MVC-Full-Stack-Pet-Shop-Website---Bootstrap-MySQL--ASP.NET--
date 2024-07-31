using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository.IRepository { 
//{
//        public interface IRepository <T> where T : class
//    {
//        //retake all the category
//        IEnumerable<T> GetAll(string? includeProperties = null);
//        //retake one category (فo display)==> we get a function with bool result
//        T Get(Expression<Func<T, bool>>? filter, string? includeProperties = null);
//        void Remove(T entity);
//        //remove multiple category
//        void RemoveRange(IEnumerable<T> entity);
//        void Add(T enitity);

//        void Update(T entity);
//    }
//}
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        T Get(Expression<Func<T, bool>> filter=null, string? includeProperties = null);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
