using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            // Get all product from database
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return NotFound("No Product detected in the database");

            // convert data from entity to DTO and return
            var(_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No Product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // Get a single product
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound($"Product {id} not found");

            // convert data from entity to DTO and return
            var (singleProduct, _) = ProductConversion.FromEntity(product, null!);
            return singleProduct is not null ? Ok(singleProduct) : NotFound($"Product {id} not found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // check model is all annotations are passed
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            //contvert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.CreateAsync(getEntity);
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // check model is all annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //contvert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            // Convert to entity
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.DeleteAsync(getEntity);
            return response.flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
