
# Resiliência e Dead Letter Queue (DLQ) com MassTransit e Polly
Esta documentação descreve a estratégia de resiliência e tratamento de falhas implementada na comunicação entre os microsserviços **Catalog.API** e **Pricing.API**, utilizando uma arquitetura baseada em eventos com **RabbitMQ/MassTransit** e chamadas síncronas resilientes com **Polly v8**.

---## 🏗️ Arquitetura do Sistema e Fluxo de Dados

O sistema é composto por dois microsserviços principais operando em um ecossistema Docker:

1. **Catalog.API (Produtor / Agregador)**: 
Responsável pelo cadastro de produtos no MongoDB. 
Publica o evento `ProductCreatedEvent` de forma assíncrona no RabbitMQ e consome dados de preço de forma síncrona via HTTP (Refit).

2. **Pricing.API (Consumidor / Provedor)**: 
Escuta o RabbitMQ, consome o evento de criação de produto e registra um preço inicial padrão (`0.0m`) no Redis Cache. Provê um endpoint HTTP para consulta de preços.


[Cliente HTTP]
│ (POST /products)
▼
┌──────────────┐ 1. Salva no banco ┌───────────────┐
│ Catalog.API │ ──────────────────────────> │ MongoDB │
└──────┬───────┘ └───────────────┘
│
│ 2. publishEndpoint.Publish(ProductCreatedEvent)
▼
┌────────────────────────────────────────────────────────────┐
│ RABBITMQ (Message Broker) │
│ │
│ [Exchange] Shared.Contracts.Events:ProductCreatedEvent │
│ │ │
│ │ (Roteamento Automático via Binding) │
│ ▼ │
│ [Queue] Pricing.API.Consumers:ProductCreatedConsumer │
└───────┬────────────────────────────────────────────────────┘
│
│ 3. MassTransit (Consumo Assíncrono com Redelivery)
▼
┌──────────────┐ 4. SavePriceAsync() ┌───────────────┐
│ Pricing.API │ ──────────────────────────> │ Redis (Cache) │
└──────────────┘ └───────────────┘


---

## 🛡️ Estratégia de Resiliência Implementada

A resiliência do sistema foi testada e validada sob cenários de indisponibilidade severa de infraestrutura (simulando quedas físicas dos containers Redis e Pricing.API).

### 1. Resiliência Assíncrona: DLQ & Scheduled Redelivery (MassTransit)
Se o microsserviço de preços receber o evento de criação de produto e o banco de dados **Redis estiver fora do ar**, o MassTransit aplica uma estratégia de reenvio em dois níveis para evitar a perda da mensagem:

*   **Nível 1 (Retry Imediato)**: O consumidor falha e tenta reprocessar a mensagem imediatamente **3 vezes**, aguardando 2 segundos entre as tentativas.

*   **Nível 2 (Scheduled Redelivery / DLQ Ativa)**: Se o erro persistir, a mensagem é removida temporariamente da fila principal e agendada usando o plugin de mensagens atrasadas do RabbitMQ. Ela entra em janelas de espera progressivas de **1, 2 e 3 minutos** antes de tentar novamente.

*   **DLQ Definitiva (`_error`)**: Se todas as janelas falharem, a mensagem é movida para a fila de erro para auditoria manual.

### 2. Resiliência Síncrona: HTTP Resilience Handler (Polly v8)
Quando o `Catalog.API` tenta consultar o preço de um produto via HTTP e o `Pricing.API` enfrenta lentidão ou indisponibilidade, o pipeline do **Polly v8** entra em ação:

*   **Standard Resilience**: Configurado com *Retry* Exponencial (3 tentativas) e um *Circuit Breaker* (Disjuntor) com janela de amostragem de 30 segundos.

*   **Tratamento de Timeouts**: Se a requisição travar por mais de 30 segundos devido a problemas de infraestrutura interna do provedor, o Polly estoura uma exceção controlada `TimeoutRejectedException`.

*   **Problem Details (RFC 9110)**: O catálogo intercepta falhas de rede (`HttpRequestException`), cancelamentos (`TaskCanceledException`) e timeouts do Polly, convertendo erros brutos do sistema em respostas HTTP amigáveis (`502 Bad Gateway` ou `503 Service Unavailable`).

---

## 🧪 Casos de Teste Executados e Resultados Práticos

### Cenário A: POST de Produto com `Pricing.API` desligado
*   **Ação**: O container `pricing-api` foi parado via Docker e um produto foi criado.

*   **Comportamento**: O `Catalog.API` salvou com sucesso no MongoDB e postou o evento no RabbitMQ. A mensagem ficou retida em estado **`Ready: 1`** na fila física do broker.

*   **Resultado**: Assim que o container `pricing-api` foi iniciado, ele restabeleceu a conexão, puxou a mensagem pendente da fila de forma retroativa e processou o preço no Redis. O sistema atingiu a **Consistência Eventual**.

### Cenário B: GET de Detalhes com `Redis` desligado (Travamento em Background)
*   **Ação**: O container do `redis` foi parado e uma consulta de detalhes de produto foi enviada ao catálogo.

*   **Comportamento**: A API de preços travou tentando conectar ao Redis. O Polly no catálogo cronometrou 30 segundos e encerrou a requisição pendente.

*   **Resultado**: O endpoint tratou a exceção e retornou um erro limpo para o cliente:
    ```json
    {
      "title": "Bad Gateway",
      "status": 502,
      "detail": "Pricing service returned an error."
    }
    ```
    O processo principal não sofreu *crash* e o container do catálogo manteve-se saudável (`healthy`).

### Cenário C: Auto-Recuperação do Driver Redis
*   **Ação**: Ajustada a string de conexão do Redis adicionando os parâmetros `abortConnect=false,connectRetry=5`.

*   **Resultado**: Garantiu que, após a restauração do container do Redis, o microsserviço de preços conseguisse restabelecer as conexões de cache automaticamente em segundo plano, reprocessando as mensagens que estavam aguardando nas janelas de *Redelivery*.

---

## 📝 Lições Aprendidas de Engenharia de Software
*   **Nomes Baseados em Eventos**: Consumidores do MassTransit devem ser nomeados com base no evento que escutam (ex: `ProductCreatedConsumer`), garantindo o desacoplamento semântico entre quem publica e quem consome.

*   **Evitar o Descarte Silencioso**: Sem uma fila física gerada pelo consumidor ativa no broker, mensagens enviadas para Exchanges sofrem *drop*. É obrigatório que o consumidor conecte-se ao broker ao menos uma vez para estabelecer a topologia física das filas.

*   **Estratégia de Conexão Não-Bloqueante**: Bancos em memória como Redis exigem configurações explícitas de não-abortividade de conexão para evitar que microsserviços sofram desligamentos fatais durante instabilidades de inicialização de infraestrutura.


