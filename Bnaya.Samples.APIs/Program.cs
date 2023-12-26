using Bnaya.Samples.Common;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Register Redis ConnectionMultiplexer
services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

// Register the event publisher
services.AddSingleton<IEventPublisher, RedisEventPublisher>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
