using MassTransit;
using Shared.Events;

namespace Mail.Api.Consumers;

public class PaymentCompletedEventNotifyConsumer : IConsumer<PaymentCompletedEvent>
{
    public Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Başrılı ödeme maili müşteriye gönderilmiştir.");

        return Task.CompletedTask;
    }
}