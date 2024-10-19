using MassTransit;
using Payment.Api.Consumers;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<StockReservedEventConsumer>();

    configurator.UsingRabbitMq(
        (_busRegistrationContext, _rabbitMQBusFactoryConfigurator) =>
        {
            _rabbitMQBusFactoryConfigurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

            _rabbitMQBusFactoryConfigurator.ReceiveEndpoint(RabbitMQSettings.Payment_StockReservedEventQueue,
                e => { e.ConfigureConsumer<StockReservedEventConsumer>(_busRegistrationContext); });
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

app.Run();
