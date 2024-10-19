using Coordinator.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddHttpClient("Order.Api", client=> client.BaseAddress = new Uri("https://localhost:7230/"));
builder.Services.AddHttpClient("Payment.Api", client=> client.BaseAddress = new Uri("https://localhost:7052/"));
builder.Services.AddHttpClient("Stock.Api", client=> client.BaseAddress = new Uri("https://localhost:7011/"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();