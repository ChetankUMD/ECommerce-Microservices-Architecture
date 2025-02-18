using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interface;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController(IOrder orderInterface, IOrderService orderService): ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            // Get all the orders
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
                return NotFound("No Order found in the databse");
            var(_, list) = OrderConverstion.FromEntity(null, orders);
            return (!list!.Any()) ? NotFound() :Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            // Get order by id
            var order =await orderInterface.FindByIdAsync(id);
            if (order is null)
                return NotFound($"Order {id} not present");
            var (singleOrder, _) = OrderConverstion.FromEntity(order, null);
            return singleOrder is not null ? singleOrder : NotFound($"Order {id} not found");
        }

        [HttpGet("details/{id:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int id)
        {
            if (id <= 0) return BadRequest("Invalid data provided");
            // Get order by id
            var orderdeatil = await orderService.GetOrderDetails(id);
            return orderdeatil is not null ? orderdeatil : NotFound($"Order {id} not found");
        }

        [HttpGet("client/{clientID:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientID)
        {
            if (clientID <= 0) return BadRequest("Invalid client Id provided");
            var orders = await orderService.GetOrdersByClientId(clientID);
            return !orders.Any()? NotFound(null) : Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO order)
        {
            // Check if the all the annotation are passed
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert to entity
            var orderEntity = OrderConverstion.ToEntity(order);
            var response = await orderInterface.CreateAsync(orderEntity);
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO order)
        {
            // Check if the all the annotation are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Convert to entity
            var orderEntity = OrderConverstion.ToEntity(order);
            var response = await orderInterface.UpdateAsync(orderEntity);
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO order)
        {
            // Convert to entity
            var getEntity = OrderConverstion.ToEntity(order);
            var response = await orderInterface.DeleteAsync(getEntity);
            return response.flag is true ? Ok(response) : BadRequest(response) ;
        }
    }
}
