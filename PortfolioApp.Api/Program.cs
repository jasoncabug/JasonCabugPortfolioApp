using Microsoft.OpenApi.Models;
using PortfolioApp.Api.Infrastructure;
using PortfolioApp.Application;
using PortfolioApp.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Layer Service Registrations
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

// Configure OpenAPI with JWT Security Scheme metadata for Scalar
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "PortfolioApp API",
            Version = "v1"
        };

        var bearerScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter your JWT token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = bearerScheme;

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Enable Global Exception Handler middleware
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Expose OpenAPI JSON document
    app.MapOpenApi();

    // Map Scalar Interactive API Reference
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("PortfolioApp API Documentation")
               .WithTheme(ScalarTheme.Purple)
               .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();