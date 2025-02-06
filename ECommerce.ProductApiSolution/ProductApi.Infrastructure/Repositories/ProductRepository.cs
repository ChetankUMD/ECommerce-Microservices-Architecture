using Azure;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Response = eCommerce.SharedLibrary.Responses.Response;

namespace ProductApi.Infrastructure.Repositories
{
    internal class ProductRepository(ProductDBContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // check if the product is already present
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if(getProduct is not null && !string.IsNullOrWhiteSpace(getProduct.Name))
                    return new Response(false, $"{entity.Name} is already present");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} successfully added to database");
                else
                    return new Response(false, $"Error occurred while adding {entity.Name}");
            }
            catch(Exception ex)
            {
                // log the exception
                LogException.LogExceptions(ex);

                // return error message to user
                return new Response(false, "Error occurred while adding the product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if(product is null)
                    return new Response(false, $"{entity.Name} not found");
                
                context.Products.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is deleted successfully");
            }
            catch (Exception ex)
            {
                // log the exception
                LogException.LogExceptions(ex);

                // return error message to user
                return new Response(false, "Error occurred while deleting the product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // log the exception
                LogException.LogExceptions(ex);

                // throw error message to user
                throw new Exception("Error occurred while finding the product by id");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                // log the exception
                LogException.LogExceptions(ex);

                // throw error message to user
                throw new InvalidOperationException("Error occurred while getting products");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var products = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return products is not null ? products : null!;

            }
            catch (Exception ex)
            {
                // log the exception
                LogException.LogExceptions(ex);

                // throw error message to user
                throw new InvalidOperationException("Error occurred while getting products");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} not present");
                
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Id} is updated successfully");
            }
            catch (Exception ex)
            {
                // log the exception
                LogException.LogExceptions(ex);

                // return error message to user
                return new Response(false, "Error occurred while updating the product");
            }
        }
    }
}
