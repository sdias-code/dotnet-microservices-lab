
------------------------------

# 📨 Documentação Técnica: Mensageria Assíncrona com RabbitMQ (Módulo 3)
Este módulo marca a transição da arquitetura para um modelo **Event-Driven (Baseado em Eventos)**, utilizando o **RabbitMQ** como message broker e o **MassTransit (v8.3.6)** como mecanismo de abstração e transporte para garantir a **consistência eventual** entre os serviços.
---## 📐 1. Design de Contratos Compartilhados (DRY)
Para evitar duplicidade de código e garantir que os microsserviços utilizem a mesma assinatura de dados, foi introduzido o projeto **`Shared.Contracts`** (Biblioteca de Classes).

* **Contrato em Inglês:** `ProductCreatedEvent`
* **Namespace Corporativo:** `Shared.Contracts.Events`
```csharp
namespace Shared.Contracts.Events;

public record ProductCreatedEvent(
    string Id,
    string Name,
    string Description
);
```> **Nota de Arquitetura:** O MassTransit utiliza o namespace completo do objeto para criar as *Exchanges* (roteadores) dentro do RabbitMQ. Ambos os microsserviços enxergam este contrato a partir da referência ao projeto compartilhado.
---## 🗺️ 2. Topologia e Fluxo de Dados de Ponta a Ponta
Quando um produto é cadastrado, o sistema funciona de forma não-bloqueante (assíncrona) seguindo os passos abaixo:
```text
[REST Client] ──(1. POST /produtos)──> [Catalog.API] ──(2. Grava)──> [MongoDB]
                                             │
                                   (3. Publish Event)
                                             │
                                             v
                                        [RabbitMQ]
                 (Exchange: Shared.Contracts.Events:ProductCreatedEvent)
                                             │
                                   (4. Deliver Message)
                                             │
                                             v
                                       [Pricing.API]
                               (Consumer: ProductCreatedConsumer)
                                             │
                                        (5. Salva)
                                             │
                                             v
                                        [Redis Cache]
```
### Detalhamento das Etapas:1. **Disparo do Cadastro:** O cliente envia uma requisição `POST http://localhost:5001/produtos`.
2. **Persistência Síncrona:** O `Catalog.API` grava as informações cadastrais do produto no **MongoDB**.
3. **Publicação do Evento:** O `Catalog.API` usa o barramento `IPublishEndpoint` para postar o evento `ProductCreatedEvent` no RabbitMQ e devolve imediatamente o status `201 Created` para o cliente (sem esperar pelo serviço de preços).4. **Entrega da Mensagem:** O RabbitMQ direciona a mensagem de forma automática para a fila assinada pelo serviço de preços.
5. **Consumo e Consistência Eventual:** O container `Pricing.API`, através do componente `ProductCreatedConsumer`, detecta a mensagem na fila, extrai o `Id` do produto criado e invoca internamente o método `_precoService.SalvarPrecoAsync(Id, 0.0m)` para registrar o preço inicial padrão (R$ 0,00) no **Redis Cache**.
---## 🛠️ 3. Componentes Implementados
### A. Produtor: `Catalog.API`
Integrado diretamente nas *Minimal APIs* através da injeção de dependência nativa:```csharp
// Dentro de ProdutoEndpoints.cs
private static async Task<IResult> CriarProduto(
    ProdutoRequest request,
    IProdutoService produtoService,
    IPublishEndpoint publishEndpoint) // Barramento injetado
{
    // ... lógica de gravação no Mongo ...
    await publishEndpoint.Publish(new ProductCreatedEvent(produto.Id!, produto.Nome, produto.Descricao));
}
```

### B. Consumidor: `Pricing.API`
Componente responsável por processar a mensagem de forma isolada:```csharp
// Dentro de ProductCreatedConsumer.cs
public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var jsonEvent = context.Message;
        await _precoService.SalvarPrecoAsync(jsonEvent.Id, 0.0m);
    }
}
```
---## 🧪 4. Guia de Testes para Amanhã
Para validar se o ecossistema está funcionando com maestria, siga este roteiro de testes:
1. **Monitore o Painel Administrativo:**
   * Acesse `http://localhost:15672` (Usuário/Senha: `guest`).
   * Vá na aba **Exchanges** e confirme a existência de `Shared.Contracts.Events:ProductCreatedEvent`.
   * Vá na aba **Queues** e veja a fila criada pelo MassTransit para o consumidor do preço.2. **Execute o Cadastro (POST):**
   * Dispare o `POST` no arquivo `teste_api/catalog.rest`.3. **Valide os Logs Cruzados:**
   * Execute `docker logs -f pricing-api`.
   * Procure pela linha: `Recebido evento de Produto Criado. ID: {id}, Nome: {nome}`.4. **Ateste a Consistência Eventual (GET):**
   * Faça um `GET http://localhost:5001/detalhes-produto/{id}` usando o ID gerado.
   * O resultado deve retornar o JSON unificado exibindo os dados do catálogo e o valor `0` que o consumidor salvou de forma automática no Redis.

------------------------------
Quando estiver pronto para continuar os testes ou quiser partir para as políticas de resiliência do Polly, me avise para darmos o próximo passo.

