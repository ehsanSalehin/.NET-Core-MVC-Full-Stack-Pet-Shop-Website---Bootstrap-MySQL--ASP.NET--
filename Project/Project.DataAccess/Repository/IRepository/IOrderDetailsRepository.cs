﻿using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project.DataAccess.Repository.IRepository
{
    public interface IOrderDeatilsRepository : IRepository<OrderDetails>
    {
        void Update(OrderDetails obj);
    }
}
