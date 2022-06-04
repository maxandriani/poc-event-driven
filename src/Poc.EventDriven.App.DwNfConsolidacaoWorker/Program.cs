using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using Poc.EventDriven.Data;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.DwNf;
using Poc.EventDriven.DwNf.Abstractions;
using Poc.EventDriven.DwNf.Events;

var builder = Host.CreateDefaultBuilder(args);
    // .ConfigureAppConfiguration(config =>
    // {
        // config.AddUserSecrets("b4e61047-7462-44ab-bc8f-68edb0e30909");
    // });

// Add services to the container.
builder.ConfigureServices((context, services) =>
{
    services.AddAzureClients(config =>
    {
        config.AddBlobServiceClient(context.Configuration.GetConnectionString("NotasFiscaisStorage"))
            .WithName(NfStorageConsts.NfStorageName);
    });

    services.AddAppDbContextFactory<IDwNfDbContext, DwNfDbContext>(options =>
        options
            .UseNpgsql(context.Configuration.GetConnectionString("DwNfDatabase"))
            .EnableDetailedErrors());
    
    services.AddAzureServiceBusBatchWorker<NfConsolidacaoEvent, NfDwConsolidadorService>()
        .WithConnectionString(context.Configuration.GetConnectionString("ConsolidacaoServiceBusSubscription"))
        .WithTopicSubscription(
            context.Configuration.GetValue<string>("ServiceBus:ConsolidacaoServiceBusSubscription:TopicName"),
            context.Configuration.GetValue<string>("ServiceBus:ConsolidacaoServiceBusSubscription:SubscriptionName"))
        .WithSessionReceiverOptions(options =>
        {
            context.Configuration.GetSection("ServiceBus:ConsolidacaoServiceBusSubscription:ServiceBusReceiverOptions").Bind(options);
        });
});

var app = builder.Build();


await app.RunAsync();
