using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.Api.Consumers;
using Stock.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<OrderCreatedEventConsumer>();

    configurator.UsingRabbitMq(
        (_busRegistrationContext, _rabbitMQBusFactoryConfigurator) =>
        {
            _rabbitMQBusFactoryConfigurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

            _rabbitMQBusFactoryConfigurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue,
                e => { e.ConfigureConsumer<OrderCreatedEventConsumer>(_busRegistrationContext); });
        });
});

builder.Services.AddSingleton<MongoDbService>();

using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();

MongoDbService mongoDbService = scope.ServiceProvider.GetService<MongoDbService>();

var collection = mongoDbService.GetCollection<Stock.Api.Models.Entities.Stock>();

if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 2000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 1000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 3000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 5000 });
    await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid().ToString(), Count = 500 });
}

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