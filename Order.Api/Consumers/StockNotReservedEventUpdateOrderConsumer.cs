using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Models;
using Order.Api.Models.Enums;
using Shared.Events;

namespace Order.Api.Consumers;

public class StockNotReservedEventUpdateOrderConsumer(OrderDbContext _orderDbContext) : IConsumer<StockNotReservedEvent>
{
    public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(c => c.OrderId == context.Message.OrderId);

        order.OrderStatu = OrderStatus.Failed;

        await _orderDbContext.SaveChangesAsync();
        
        Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Sipariş {order.OrderStatu} olarak güncellendi.");
    }
}