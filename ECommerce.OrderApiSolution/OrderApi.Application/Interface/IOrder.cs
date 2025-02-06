using eCommerce.SharedLibrary.Interface;
using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.Interface
{
    public interface IOrder : IGenericInterface<Order>
    {
        Task<IEnumerable<OrderDTO>> GetOrderdAsync(Expression <Func<Order, bool>> predicate);
    }
}
