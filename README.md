# poc-event-driven

Apresentação de conceitos básicos de orientação a eventos para stakeholders da Becomex.

## Build

```sh
docker buildx build -t maxandriani/poc.eventdriven.app.regimeapi:1.0.0 -f ./src/Poc.EventDriven.App.RegimeApi/Dockerfile --platform linux/amd64,linux/arm64 --push .
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
