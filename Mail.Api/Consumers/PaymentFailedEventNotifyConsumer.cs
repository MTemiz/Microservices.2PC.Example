using MassTransit;
using Shared.Events;

namespace Mail.Api.Consumers;

public class PaymentFailedEventNotifyConsumer : IConsumer<PaymentFailedEvent>
{
    public Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Ödeme başarısız maili müşteriye gönderildi.");
        return Task.CompletedTask;
    }
}