using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interface;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var getorder = await GetByAsync(o => o.Id == entity.Id);
                if (getorder is not null && getorder.Id > 0)
                {
                    return new Response(false, $"{entity.Id} is already present");
                }

                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0 ? new Response(true, "Order placed successfully") :
                    new Response(false, "Error occurred while placing order");

            }
            catch(Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                // Diaplay scary-free message to client
                return new Response(false, "Error while placing the order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var getorder = await FindByIdAsync(entity.Id);
                if(getorder is null)
                {
                    return new Response(false, $"Order with ID {entity.Id} doesn't exist");
                }
                context.Orders.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"Order {entity.Id} successfully deleted");
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                // Diaplay scary-free message to client
                return new Response(false, "Error while placing the order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrderAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).ToListAsync();
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                // Diaplay scary-free message to client
                throw new Exception("Error while placing the order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders.FindAsync(id);
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                // Diaplay scary-free message to client
                throw new Exception("Error while placing the order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.AsNoTracking().ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                // Diaplay scary-free message to client
                throw new Exception("Error while placing the order");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync();
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                // Diaplay scary-free message to client
               throw new Exception("Error while placing the order");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if(order is null)
                    return new Response(false, $"Order {entity.Id} not present" );
                context.Entry(order).State = EntityState.Modified;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"Order {entity.Id} is successfully updated");
            }
            catch (Exception ex)
            {
                // Log Original Exception
                LogException.LogExceptions(ex);

                // Diaplay scary-free message to client
                return new Response(false, "Error while placing the order");
            }
        }
    }
}
