using Poc.EventDriven.Services;
using Microsoft.Extensions.Azure;
using Poc.EventDriven.DwNf.Events;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
builder.Services.AddAzureClients(options =>
{
    options
        .AddServiceBusClient(builder.Configuration.GetConnectionString("NotasFiscaisSb"))
            .WithName("NotasFiscaisSb");

    options.AddBlobServiceClient(builder.Configuration.GetConnectionString("NotasFiscaisStorage"))
            .WithName("NotasFiscaisStorage");
});

builder.Services.AddAzureServiceBusDispatcher("NotasFiscaisSb")
    .WithEvent<NfConsolidacaoEvent>(builder.Configuration.GetValue<string>("ServiceBus:NotasFiscaisSb:NfConsolidacaoEvent"));

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<NfIngressService>().AllowAnonymous();
//app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
