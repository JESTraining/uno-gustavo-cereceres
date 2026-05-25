using Microsoft.Extensions.Options;
using NotificationEngine.Features.Events;
using NotificationEngine.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NotificationEngineContext>(
    options=> options.UseNpgsql(builder.Configuration.GetConnectionString("Database"))
);

builder.Services.AddStackExchangeRedisCache(options=>
options.Configuration = builder.Configuration.GetConnectionString("Cache"));

builder.Services.AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
