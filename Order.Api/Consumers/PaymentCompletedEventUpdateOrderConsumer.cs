using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Models;
using Order.Api.Models.Enums;
using Shared.Events;

namespace Order.Api.Consumers;

public class PaymentCompletedEventUpdateOrderConsumer(OrderDbContext _orderDbContext) : IConsumer<PaymentCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(c => c.OrderId == context.Message.OrderId);

        order.OrderStatu = OrderStatus.Completed;
        
        await _orderDbContext.SaveChangesAsync();
        
        Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Sipariş {order.OrderStatu} olarak güncellendi.");
    }
}