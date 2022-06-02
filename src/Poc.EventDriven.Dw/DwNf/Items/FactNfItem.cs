using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.DwNf.Items;

public class FactNfItem
{
    public int Id { get; set; }

    public int DimNfId { get; set; }
    public DimNf? DimNf { get; set; }
    public int DimSkuId { get; set; }
    public DimSku? DimSku { get; set; }

    public int DimEmpresaId { get; set; }
    public DimEmpresa? DimEmpresa { get; set; }
    public int? DimExportadorId { get; set; }
    public DimEmpresa? DimExportador { get; set; }
    public int DimEmissorId { get; set; }
    public DimEmpresa? DimEmissor { get; set; }
    public int DimEmissaoId { get; set; }
    public DimTempo? DimEmissao { get; set; }
    public int DimTipoOperacaoId { get; set; }
    public DimTipoOperacao? DimTipoOperacao { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int ValorBrl { get; set; }
    public int ValorIcmsBrl { get; set; }
    public int ValorIcmsstBrl { get; set; }
    public int ValorIiBrl { get; set; }
    public int ValorIpiBrl { get; set; }
    public int ValorPisBrl { get; set; }
    public int ValorCofinsBrl { get; set; }
}
