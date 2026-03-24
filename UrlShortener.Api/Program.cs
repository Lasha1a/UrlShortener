using FluentValidation.AspNetCore;
using UrlShortener.Api;
using UrlShortener.Api.MiddleWare;
using UrlShortener.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddTransient<GlobalExceptionHandler>();

builder.Services.AddOpenApi("v1");

builder.Services.AddMainApiDi(builder.Configuration);

var app = builder.Build();

//initialize Cassandra keyspace and tables
var initializer = app.Services.GetRequiredService<CassandraInitializer>();
await initializer.InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();