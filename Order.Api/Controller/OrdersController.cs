using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Models;
using Order.Api.Models.Entities;
using Order.Api.ViewModels;
using Shared.Events;
using Shared.Messages;

namespace Order.Api.Controller;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(OrderDbContext dbContext, IPublishEndpoint publishEndpoint) : ControllerBase
{
    private readonly OrderDbContext _dbContext = dbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("hi");
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderVM createOrderVM)
    {
        Models.Entities.Order order = new()
        {
            OrderId = Guid.NewGuid(),
            BuyerId = createOrderVM.BuyerId,
            CreatedDate = DateTime.Now,
            OrderStatu = Models.Enums.OrderStatus.Suspend
        };

        order.OrderItems = createOrderVM.OrderItems.Select(oi =>
            new OrderItem()
            {
                Count = oi.Count,
                Price = oi.Price,
                ProductId = oi.ProductId
            }).ToList();

        order.TotalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Count);

        _dbContext.Orders.Add(order);

        await dbContext.SaveChangesAsync();

        OrderCreatedEvent orderCreatedEvent = new()
        {
            BuyerId = order.BuyerId,
            OrderId = order.OrderId,
            TotalPrice = order.TotalPrice,
            OrderItems = order.OrderItems.Select(oi =>
                new OrderItemMessage
                {
                    Count = oi.Count,
                    ProductId = oi.ProductId
                }).ToList()
        };

        await _publishEndpoint.Publish(orderCreatedEvent);
        
        Console.WriteLine($"{order.OrderId} -  Sipariş oluşturuldu.");
        
        return Ok();
    }
}