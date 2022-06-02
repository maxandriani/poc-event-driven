using Poc.EventDriven.Boms;
using Poc.EventDriven.Clientes;
using Poc.EventDriven.Nfs;

using System.CommandLine;

var regimesApiOption = new Option<string>(new string[] { "--server-url", "-u" }, () => "http://raspberrypi:5001", "Endereço da api de clientes.");
var grpcApiOption = new Option<string>(new string[] { "--grpc-url", "-grpc" }, () => "http://raspberrypi:5002", "Endereço da api Grpc");

var clienteCnpjArg = new Argument<string>("cnpj", "Cnpj do cliente.");
var clienteGerarQuantidadeArg = new Argument<int>("quantidade", "O número de cliente a ser criado.");
var clienteGerarCnpjCmd = new Command("cnpj", "Gerar por cnpj")
{
    clienteCnpjArg
};
clienteGerarCnpjCmd.SetHandler((string serverUri, string cnpj) => ClientesCmdHandler.Gerar(serverUri, cnpj), regimesApiOption, clienteCnpjArg);
var clienteGerarCmd = new Command("gerar", "Criar novos clientes")
{
    clienteGerarQuantidadeArg,
    clienteGerarCnpjCmd
};
clienteGerarCmd.SetHandler((string serverUri, int quantidade) => ClientesCmdHandler.Gerar(serverUri, quantidade), regimesApiOption, clienteGerarQuantidadeArg);

var clienteListarCmd = new Command("listar", "Exibir clientes cadastrados.");
clienteListarCmd.SetHandler((string serverUri) => ClientesCmdHandler.Listar(serverUri), regimesApiOption);

var clienteCmd = new Command("cliente", "Gerenciar clientes")
{
    clienteGerarCmd,
    clienteListarCmd,
};


var bomGerarQuantidadeArg = new Argument<int>("quantidade", "Número de BOMs a gerar.");
var bomGerarProdutosOpt = new Option<string[]>("--produtos", () => Array.Empty<string>(), "Delimitar geração por Sku.");
var bomGerarCmd = new Command("gerar", "Gerar BOMs para um cliente")
{
    clienteCnpjArg,
    bomGerarQuantidadeArg,
    bomGerarProdutosOpt
};
bomGerarCmd.SetHandler(
    (string serverUri, string cnpj, int quantidade, string[] produtos)
        => BomsCmdHandler.Gerar(serverUri, cnpj, quantidade, produtos),
    regimesApiOption, clienteCnpjArg, bomGerarQuantidadeArg, bomGerarProdutosOpt);

var bomCmd = new Command("bom", "Gerenciar BOMs")
{
    bomGerarCmd
};

var nfGerarImpOpt = new Option<bool>(new string[] { "--importacao", "-i" }, () => false, "Apenas notas de improtacao");
var nfGerarExpOpt = new Option<bool>(new string[] { "--exportacao", "-e" }, () => false, "Apenas notas de exportacao");
var nfGerarNacOpt = new Option<bool>(new string[] { "--nacionalizacao", "-n" }, () => false, "Apenas notas de nacionalizacao");
var nfGerarQtdArg = new Argument<int>("quantidade", "O número de notas a serem geradas.");
var nfGerarPeriodo = new Argument<string>("período (mm/aaaa)", "O mês na qual as notas serão geradas");
var nfGerarFornecedoresOpt = new Option<string[]>(new string[] { "--fornecedores", "-f" }, () => Array.Empty<string>(), "Gerar Nfs para fornecedores específicos");
var nfGerarClientesOpt = new Option<string[]>(new string[] { "--clientes", "-c" }, () => Array.Empty<string>(), "Gerar Nfs para clientes específicos.");
var nfGerarCmd = new Command("gerar", "Criar novas notas fiscais")
{
    clienteCnpjArg,
    nfGerarQtdArg,
    nfGerarPeriodo,
    nfGerarImpOpt,
    nfGerarExpOpt,
    nfGerarNacOpt,
    nfGerarFornecedoresOpt,
    nfGerarClientesOpt
};
nfGerarCmd.SetHandler((string regimesUri, string grpcUri, string cnpj, int quantidade, string periodo, bool apenasImport, bool apenasExport, bool apenasNac, string[] fornecedores, string[] clientes) 
    => NfsCmdHandler.Gerar(regimesUri,
                           grpcUri,
                           cnpj,
                           quantidade,
                           periodo,
                           apenasImport,
                           apenasExport,
                           apenasNac,
                           fornecedores,
                           clientes), regimesApiOption, grpcApiOption, clienteCnpjArg, nfGerarQtdArg, nfGerarPeriodo, nfGerarImpOpt, nfGerarExpOpt, nfGerarNacOpt, nfGerarFornecedoresOpt, nfGerarClientesOpt);
var nfCmd = new Command("nf", "Gerenciar notas fiscais")
{
    nfGerarCmd
};

var rootCommand = new RootCommand("Gerador de Notas Fiscais 😎")
{
    clienteCmd,
    bomCmd,
    nfCmd
};
rootCommand.AddGlobalOption(regimesApiOption);
rootCommand.AddGlobalOption(grpcApiOption);

await rootCommand.InvokeAsync(args);
