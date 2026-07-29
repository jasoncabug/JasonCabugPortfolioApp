using PortfolioApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Ensure Authentication runs BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "PortfolioApp API is online!");
app.MapControllers();

app.Run();