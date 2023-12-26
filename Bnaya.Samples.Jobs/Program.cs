using Bnaya.Samples.Common;
using Bnaya.Samples.Jobs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;
using System.Reflection;

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

#region Open Telemetry

string serviceName = Assembly.GetExecutingAssembly()?.GetName().Name ?? throw new Exception("Assembly name");

builder.Logging.AddOpenTelemetry(options =>
{
    var resource = ResourceBuilder.CreateDefault()
                .AddService(serviceName);
    options.SetResourceBuilder(resource
                    .AddService(serviceName));
});

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource =>
            resource.AddService(serviceName))

      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddGrpcClientInstrumentation()
          .AddRedisInstrumentation()
          .AddSqlClientInstrumentation()
          .AddOtlpExporter()
          //.AddConsoleExporter()
          .SetSampler<AlwaysOnSampler>())
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter()
          .AddPrometheusExporter());
          //.AddConsoleExporter()

#endregion // Open Telemetry


var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

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
