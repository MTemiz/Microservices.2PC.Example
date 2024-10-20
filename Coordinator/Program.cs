using Coordinator.Context;
using Coordinator.Services;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddHttpClient("Order.Api", client => client.BaseAddress = new Uri("https://localhost:5011/"));
builder.Services.AddHttpClient("Payment.Api", client => client.BaseAddress = new Uri("https://localhost:6011/"));
builder.Services.AddHttpClient("Stock.Api", client => client.BaseAddress = new Uri("https://localhost:7011/"));

builder.Services.AddTransient<ITransactionService, TransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1
    var transactionId = await transactionService.CreateTransactionAsync();

    await transactionService.PrepareServicesAsync(transactionId);

    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

    if (transactionState)
    {
        // Phase 2
        await transactionService.CommitAsync(transactionId);

        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
    }

    if (!transactionState)
    {
        await transactionService.RollbackAsync(transactionId);
    }
});

app.Run();