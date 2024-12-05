﻿using MyBulky.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBulky.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository  :IRepository<ShoppingCart>
    {
        void Update (ShoppingCart obj);
    }
}