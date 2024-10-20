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
    Console.WriteLine("Stock service is ready");
    return true;
});

app.MapGet("/commit", () =>
{
    Console.WriteLine("Stock service is commited");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Stock service is rollbacked");
});

app.Run();