using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Messages;
using Stock.Api.Services;

namespace Stock.Api.Consumers;

public class OrderCreatedEventConsumer(MongoDbService _mongoDbService, ISendEndpointProvider _sendEndpointProvider, IPublishEndpoint _publishEndpoint)
    : IConsumer<OrderCreatedEvent>
{
    private IMongoCollection<Stock.Api.Models.Entities.Stock> _stockCollection =
        _mongoDbService.GetCollection<Stock.Api.Models.Entities.Stock>();

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        List<bool> stockResult = new();

        foreach (OrderItemMessage orderItem in context.Message.OrderItems)
        {
            stockResult.Add((await _stockCollection.FindAsync(s =>
                s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());
        }

        if (stockResult.TrueForAll(sr => sr.Equals(true)))
        {
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                Stock.Api.Models.Entities.Stock stock =
                    await (await _stockCollection.FindAsync(c => c.ProductId == orderItem.ProductId))
                        .FirstOrDefaultAsync();

                stock.Count -= orderItem.Count;

                await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
            }

            StockReservedEvent stockReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                TotalPrice = context.Message.TotalPrice
            };

            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));

            await sendEndpoint.Send(stockReservedEvent);

            Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Stok rezerv edildi.");
        }
        else
        {
            StockNotReservedEvent stockNotReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                Message = "Stock not reserved message"
            };

            _publishEndpoint.Publish(stockNotReservedEvent);
            
            Console.WriteLine($"{DateTime.Now}:{context.Message.OrderId} -  Stok rezerv edilemedi.");
        }
    }
}