using SensorService.Infrastructure.Messaging;
using SensorService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SensorService.Application.Services;
using SensorService.Domain.Interfaces;
using SensorService.Infrastructure.Integrations;
using SensorService.Infrastructure.Repositories;
using SensorService.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core

builder.Services.AddDbContext<SensorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<SensorReadingAppService>();


builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<TalhaoRegistry>();
builder.Services.AddSingleton<ITalhaoValidator, RabbitMqTalhaoValidator>();
builder.Services.AddHostedService<TalhaoEventsConsumer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
