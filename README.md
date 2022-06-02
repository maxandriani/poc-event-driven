# poc-event-driven

Apresentação de conceitos básicos de orientação a eventos para stakeholders da Becomex.

## Build

```sh
docker buildx build -t maxandriani/poc.eventdriven.app.regimeapi:1.0.0 -f ./src/Poc.EventDriven.App.RegimeApi/Dockerfile --platform linux/amd64,linux/arm64 --push .

docker buildx build -t maxandriani/poc.eventdriven.app.dwnfconsolidacaoworker:1.0.0 -f ./src/Poc.EventDriven.App.DwNfConsolidacaoWorker/Dockerfile --platform linux/amd64,linux/arm64 --push .

docker buildx build -t maxandriani/poc.eventdriven.app.nfingestorgrpc:1.0.0 -f ./src/Poc.EventDriven.App.NfIngestorGrpcServer/Dockerfile --platform linux/amd64,linux/arm64 --push .
```

## Deploy Infra

### Resource Group

```sh
# Create Resource Group 
az deployment sub create --location=brazilsouth --template-file=infra/az/resource-g/rg.template.json --parameters=infra/az/resource-g/rg.parameters.json

# Create Service Bus
az deployment group create -g=poc-event-driven-rg --template-file=infra/az/service-bus/sb.template.json --parameters=infra/az/service-bus/db.parameters.json

# Create AKS Cluster

# Apply AKS Infra

```

## Migrations

```sh
## Regismes DB
dotnet ef database update -p src/Poc.EventDriven.App.RegimeApi/Poc.EventDriven.App.RegimeApi.csproj

## DwNf DB
dotnet ef database update -p src/Poc.EventDriven.App.DwNfConsolidadorWorker/Poc.EventDriven.App.DwNfConsolidadorWorker.csproj

```

## Gerador de Nfs

```sh
## Criar um cliente
Poc.EventDriven.App.GeradorNf cliente gerar 1 --cnpj "Custom cnpj" --regimes-url "api de regimes"

## Gerar algumas BOMs
Poc.EventDriven.App.GeradorNf bom gerar "cnpj" 10 --regimes-url "api de regimes"

## Gerar NFs
Poc.EventDriven.App.GeradorNf nf gerar "cnpj" 10000 6/2022 --regismes-url "api de regimes" --grpc-url "api de ingestão"

```
