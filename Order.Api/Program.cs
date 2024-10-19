using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Consumers;
using Order.Api.Models;
using Shared;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<PaymentCompletedEventUpdateOrderConsumer>();
    configurator.AddConsumer<StockNotReservedEventUpdateOrderConsumer>();
    configurator.AddConsumer<PaymentFailedEventUpdateOrderConsumer>();
    
    configurator.UsingRabbitMq(
        (_busRegistrationContext, _rabbitMQBusFactoryConfigurator) =>
        {
            _rabbitMQBusFactoryConfigurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

            _rabbitMQBusFactoryConfigurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue,
                e => e.ConfigureConsumer<PaymentCompletedEventUpdateOrderConsumer>(_busRegistrationContext));

            _rabbitMQBusFactoryConfigurator.ReceiveEndpoint(RabbitMQSettings.Order_StockNotReservedEventQueue,
                e => e.ConfigureConsumer<StockNotReservedEventUpdateOrderConsumer>(_busRegistrationContext));

            _rabbitMQBusFactoryConfigurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventQueue,
                e => e.ConfigureConsumer<PaymentFailedEventUpdateOrderConsumer>(_busRegistrationContext));
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();