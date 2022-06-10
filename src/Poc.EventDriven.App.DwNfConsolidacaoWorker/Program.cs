using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using Poc.EventDriven.Data;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.DwNf;
using Poc.EventDriven.DwNf.Abstractions;
using Poc.EventDriven.DwNf.Events;
using Poc.EventDriven.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAzureClients(config =>
{
    config.AddBlobServiceClient(builder.Configuration.GetConnectionString("NotasFiscaisStorage"))
        .WithName(NfStorageConsts.NfStorageName);

    config.AddServiceBusClient(builder.Configuration.GetConnectionString("ConsolidacaoServiceBusSubscription"))
        .WithName("ConsolidacaoServiceBusSubscription");
});

builder.Services.AddAppDbContextFactory<IDwNfDbContext, DwNfDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DwNfDatabase"))
        .EnableDetailedErrors());
    
builder.Services.AddAzureServiceBusBatchWorker<NfConsolidacaoEvent, NfDwConsolidadorService>()
    .WithClientName("ConsolidacaoServiceBusSubscription")
    .WithTopicSubscription(
        builder.Configuration.GetValue<string>("ServiceBus:ConsolidacaoServiceBusSubscription:TopicName"),
        builder.Configuration.GetValue<string>("ServiceBus:ConsolidacaoServiceBusSubscription:SubscriptionName"))
    .WithSessionReceiverOptions(options =>
    {
        builder.Configuration.GetSection("ServiceBus:ConsolidacaoServiceBusSubscription:ServiceBusReceiverOptions").Bind(options);
    });

builder.Services.AddWatchDogServices();

builder.Services.AddHealthChecks()
    .AddAzureBlobStorage(builder.Configuration.GetConnectionString("NotasFiscaisStorage"), NfStorageConsts.NfContainerName)
    .AddNpgSql(builder.Configuration.GetConnectionString("DwNfDatabase"))
    .AddWatchDogCheck();

var app = builder.Build();

app.MapHealthChecks("/healthz");

await app.RunAsync();
