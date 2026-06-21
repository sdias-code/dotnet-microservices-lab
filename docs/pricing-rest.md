## 💰 Microsserviço: Pricing.API

Responsável pelo ciclo de vida, gerenciamento de valores monetários dos produtos e controle da camada de cache distribuído de alta performance.

### 🔌 Fluxo de Operações (REST Client)

O serviço escuta externamente na porta `5002` (e porta interna `8080` no container).

#### 📥 1. Cadastro/Atualização de Preço (`POST /precos`)

* **Fluxo:** 
`REST Client ──(HTTP POST :5002)──> Pricing.API ──> Redis Cache`

* **Explicação Técnica:**
  1. O cliente envia um payload contendo o vínculo do `produtoId` (gerado previamente pelo MongoDB) e o seu respectivo `valor`.
  2. O endpoint mapeia a requisição para o `IPrecoService`.
  3. O `IPrecoRepository` invoca o `AddStackExchangeRedisCache` para salvar essa informação no banco em memória **Redis**.
  4. O dado é guardado de forma estruturada utilizando a chave de instância `"Pricing_{produtoId}"`, retornando `201 Created`.

#### 📤 2. Consulta de Preço por Produto (`GET /precos/{produtoId}`)

* **Fluxo:** 
`REST Client/Catalog.API ──(HTTP GET)──> Pricing.API ──> Redis Cache`

* **Explicação Técnica:**
  1. Este endpoint pode ser chamado tanto externamente pelo REST Client para auditoria quanto internamente pelo `Catalog.API` (via cliente Refit).
  2. O serviço recebe o parâmetro `{produtoId}` e consulta a existência da chave correspondente no container do **Redis**.
  3. **Cache Hit:** Se o preço já estiver na memória, o Redis o entrega instantaneamente em poucos milissegundos.
  4. **Cache Miss:** Caso a chave tenha expirado ou não exista, o serviço aciona a lógica de fallback ou base auxiliar para recompor o preço, atualiza o cache e retorna o JSON final com status `200 OK`.
