using Bnaya.Samples.Common;
using Bnaya.Samples.Jobs;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Register Redis ConnectionMultiplexer
services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

services.AddSingleton<IEventHandler, MyEventHandler>();
services.AddHostedService<Job>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/", () =>
{
    return $"Jobs: {DateTime.UtcNow}";
})
.WithOpenApi();

app.Run();
