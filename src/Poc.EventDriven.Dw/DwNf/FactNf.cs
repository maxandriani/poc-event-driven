namespace Poc.EventDriven.DwNf;

public class FactNf
{
    public int Id { get; set; }

    public int DimEmpresaId { get; set; }
    public DimEmpresa? DimEmpresa { get; set; }
    public int DimExportadorId { get; set; }
    public DimEmpresa? DimExportador { get; set; }
    public int DimEmissorId { get; set; }
    public DimEmpresa? DimEmissor { get; set; }
    public int DimEmissaoId { get; set; }
    public DimTempo? DimEmissao { get; set; }
    public int DimOperacaoId { get; set; }
    public DimTipoOperacao? DimOperacao { get; set; }

    public Guid Chave { get; set; }
    public int Numero { get; set; }
    public int Serie { get; set; }
    public string BlobAddress { get; set; } = string.Empty;

    public int ValorTotalBrl { get; set; }
    public int ValorIcmsBrl { get; set; }
    public int ValorIcmsstBrl { get; set; }
    public int ValorIiBrl { get; set; }
    public int ValorIpiBrl { get; set; }
    public int ValorPisBrl { get; set; }
    public int ValorCofinsBrl { get; set; }
}
