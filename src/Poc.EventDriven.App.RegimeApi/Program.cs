using Poc.EventDriven.Boms;
using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Boms.Items;
using Poc.EventDriven.Boms.Items.Abstractions;
using Poc.EventDriven.Clientes;
using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Produtos;
using Poc.EventDriven.Produtos.Abstractions;
using Poc.EventDriven.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IEmpresaApiService, EmpresaApiService<IRegimesDbContext>>();
builder.Services.AddTransient<IClienteApiService, ClienteApiService>();
builder.Services.AddTransient<IProdutoApiService, ProdutoApiService>();
builder.Services.AddTransient<IBomApiService, BomApiService>();
builder.Services.AddTransient<IBomPartApiService, BomPartApiService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(config =>
{
    config.AddRegimesProfiles();
});

builder.Services.AddAppDbContextFactory<IRegimesDbContext, RegimesDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("Regimes"))
        .EnableDetailedErrors());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();

app.Run();
