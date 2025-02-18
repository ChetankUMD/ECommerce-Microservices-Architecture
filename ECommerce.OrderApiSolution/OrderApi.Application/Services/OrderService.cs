using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interface;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        //Get Product
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call product API using http client 
            // Redirect this call to API gateway since product api is not responding to outsiders.
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode) return null!;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        //GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call product API using http client 
            // Redirect this call to API gateway since product api is not responding to outsiders.
            var getUser = await httpClient.GetAsync($"/api/products/{userId}");
            if (!getUser.IsSuccessStatusCode) return null!;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        //Get order by Id
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            // Prepare order
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0) return null!;

            //Get retry pipline
            var retryPipline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Prepare product
            var productDTO = await retryPipline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare Client
            var appUserDTO = await retryPipline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // Populate Order Details
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        // Get order by client Id
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all client's orders
            var orders = await orderInterface.GetOrderAsync(o=> o.ClientId == clientId);
            if (!orders.Any()) return null!;

            // Convert from entity to DTO
            var (_, _orders) = OrderConverstion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
