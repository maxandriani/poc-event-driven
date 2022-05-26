# Detalhamento de Infraestrutura

## ServiceBus

Estrutura de tópicos e subscriptions necessárias.

* nf_ingress_tp
  * exportacao_worker_sub
  * nacionalizacao_worker_sub
  * importacao_worker_sub

* nf_update_monitor_tp
  * monitor_worker_sub

* regime_calcular_saldo_tp
  * calculador_saldo_worker_sub

## EventGrid

Estruturas de Tópicos necessárias.

* nf_incomming_count
* nf_processed_count
* calc_saldo_started
* calc_saldo_finished

## Aks Cluster

### Services

* **nf-ingestor-grpc-svc:** Poc.EventDriven.App.NfIngestorGrpcServer
* **aks-monitor-api-svc:** Poc.EventDriven.App.AksMonitorApi
* **regime-api-svc:** Poc.EventDriven.App.RegimeApi
* **regime-web-svc:**  

### Deployments

* **aks-monitor-api-dep:** Poc.EventDriven.App.AksMonitorApi
* **calculador-saldo-worker-dep:** Poc.EventDriven.App.CalculadoraSaldoWorker
* **nf-exportacao-worker-dep:** Poc.EventDriven.App.NfExportacaoWorker
* **nf-importacao-worker-dep:** Poc.EventDriven.App.NfImportacaoWorker
* **nf-ingestor-grpc-svc:** Poc.EventDriven.App.NfIngestorGrpcServer
* **nf-monitor-worker-dep:** Poc.EventDriven.App.NfMonitorWroker
* **nf-nacionalizacao-worker-dep:** Poc.EventDriven.App.NfNacionalizacaoWorker
* **regime-api:** Poc.EventDriven.App.RegimeApi
* **regime-web:**  

## Dockerhub

* **poc-eventdriven-regimesapi:** Poc.EventDriven.App.RegimeApi
