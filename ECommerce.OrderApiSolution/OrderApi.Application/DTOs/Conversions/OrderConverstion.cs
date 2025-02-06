using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConverstion
    {
        public static Order ToEntity(OrderDTO order) => new()
        {
            Id = order.Id,
            ClientId = order.ClientId,
            ProductId = order.ProductId,
            OrderedDate = order.OrderDate,
            PurchaseQuantity = order.PurchaseQuantity,
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<OrderDTO>? orders)
        {
            // return single
            if(order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ClientId,
                    order.ProductId,
                    order.PurchaseQuantity,
                    order.OrderedDate);

                return (singleOrder, null);
            }

            //return list
            if(orders is not null || order is null)
            {
                var _orders = orders!.Select(o =>
                new OrderDTO(
                    o.Id,
                    o.ClientId,
                    o.ProductId,
                    o.PurchaseQuantity,
                    o.OrderDate));

                return (null, _orders);
            }

            return(null, null);
        }
    }
}
