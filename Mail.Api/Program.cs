using Mail.Api.Consumers;
using MassTransit;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<PaymentFailedEventNotifyConsumer>();
    configurator.AddConsumer<PaymentCompletedEventNotifyConsumer>();
    
    configurator.UsingRabbitMq(
        (_busRegistrationContext, _rabbitMQBusFactoryConfigurator) =>
        {
            _rabbitMQBusFactoryConfigurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
            
            _rabbitMQBusFactoryConfigurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventSendEmailQueue,
                e => e.ConfigureConsumer<PaymentCompletedEventNotifyConsumer>(_busRegistrationContext));
            
            _rabbitMQBusFactoryConfigurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventSendEmailQueue,
                e => e.ConfigureConsumer<PaymentFailedEventNotifyConsumer>(_busRegistrationContext));
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
