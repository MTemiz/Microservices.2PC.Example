var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/ready", () =>
{
    Console.WriteLine("Order service is ready");
    return false;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Order service is commited");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Order service is rollbacked");
});
app.Run();