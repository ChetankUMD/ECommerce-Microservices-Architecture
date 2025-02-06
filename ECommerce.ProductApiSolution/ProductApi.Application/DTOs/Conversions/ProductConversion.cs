using ProductApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDTO product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity
        };

        // method to handle if product is in list or single
        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            // return single
            if(product is not null || products is null)
            {
                var singleProduct = new ProductDTO
                    (
                    product!.Id,
                    product.Name!,
                    product.Price,
                    product.Quantity
                    );
                return(singleProduct, null);
            }

            // if the product is a list
            if(products is not null)
            {
                var _products = products!.Select(p =>
                    new ProductDTO(p.Id, p.Name!, p.Price, p.Quantity)).ToList();
                return (null, _products);
            }

            return(null, null);
        }
    }
}
