using AutoMapper;

using Poc.EventDriven.Boms;
using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Boms.Items;
using Poc.EventDriven.Boms.Items.Abstractions;
using Poc.EventDriven.Clientes;
using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Produtos;
using Poc.EventDriven.Produtos.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ProfileDependencyInjectionExtensions
{
    public static IMapperConfigurationExpression AddRegimesProfiles(this IMapperConfigurationExpression mapper)
    {
        mapper.CreateMap<Empresa, EmpresaDto>().ReverseMap();
        mapper.CreateMap<Empresa, CreateUpdateEmpresaDto>().ReverseMap();
        mapper.CreateMap<Cliente, ClienteDto>().ReverseMap();
        mapper.CreateMap<Cliente, CreateUpdateClienteDto>().ReverseMap();
        mapper.CreateMap<Produto, ProdutoDto>().ReverseMap();
        mapper.CreateMap<Produto, CreateUpdateProdutoDto>().ReverseMap();
        mapper.CreateMap<Bom, BomDto>().ReverseMap();
        mapper.CreateMap<Bom, CreateUpdateBomDto>().ReverseMap();
        mapper.CreateMap<BomPart, BomPartDto>().ReverseMap();
        mapper.CreateMap<BomPart, CreateUpdateBomPartDto>().ReverseMap();

        return mapper;
    }
}
