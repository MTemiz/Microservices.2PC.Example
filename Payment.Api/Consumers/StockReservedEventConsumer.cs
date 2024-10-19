using MassTransit;
using Shared.Events;

namespace Payment.Api.Consumers;

public class StockReservedEventConsumer(IPublishEndpoint _publishEndpoint) : IConsumer<StockReservedEvent>
{
    public Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        bool val = true;
        
        if (val)
        {
            PaymentCompletedEvent paymentCompletedEvent = new()
            {
                OrderId = context.Message.OrderId
            };

            _publishEndpoint.Publish(paymentCompletedEvent);

            Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Ödeme başarılı.");
        }
        else
        {
            //Ödemede sıkıntı olduğunu...
            PaymentFailedEvent paymentFailedEvent = new()
            {
                OrderId = context.Message.OrderId,
                Message = "Bakiye yetersiz..."
            };

            _publishEndpoint.Publish(paymentFailedEvent);

            Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Ödeme başarısız.");
        }
        return Task.CompletedTask;
    }
}