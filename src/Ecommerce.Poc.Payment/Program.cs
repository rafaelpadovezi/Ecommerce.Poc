using DotNetCore.CAP.Internal;
using Ecommerce.Poc.Payment;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ziggurat;
using Ziggurat.CapAdapter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<OrderCreatedConsumer>();
builder.Services.AddConsumerService<OrderCreatedMessage, OrderCreatedConsumerService>(
    options => options.UseMongoDbIdempotency("payment"));

var clientSettings = MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDB"));
clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
builder.Services.AddSingleton<IMongoClient>(
    new MongoClient(clientSettings));
builder.Services
    .AddCap(x =>
    {
        x.UseMongoDB(builder.Configuration.GetConnectionString("MongoDB"));

        x.DefaultGroupName = "payment";

        x.UseRabbitMQ(o =>
        {
            o.HostName = builder.Configuration.GetValue<string>("RabbitMQ:HostName");
            o.Port = builder.Configuration.GetValue<int>("RabbitMQ:Port");
            o.ExchangeName = builder.Configuration.GetValue<string>("RabbitMQ:ExchangeName");
            // To consume messages without the cap headers - https://cap.dotnetcore.xyz/user-guide/en/cap/messaging/#custom-headers
            o.CustomHeaders = e => new List<KeyValuePair<string, string>>
            {
                new(DotNetCore.CAP.Messages.Headers.MessageId, SnowflakeId.Default().NextId().ToString()),
                new(DotNetCore.CAP.Messages.Headers.MessageName, e.RoutingKey)
            };
        });
    })
    .AddSubscribeFilter<BootstrapFilter>(); // Enrich the message with the required information;

builder.Services.AddOpenTelemetryTracing((otelBuilder) => otelBuilder
    .AddAspNetCoreInstrumentation()
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(builder.Configuration.GetValue<string>("Otlp:ServiceName")))
    .AddCapInstrumentation()
    .AddMongoDBInstrumentation()
    .AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint"));
        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
    }));

var app = builder.Build();

app.MapGet("/", () => "Welcome to payment!");

app.Run();