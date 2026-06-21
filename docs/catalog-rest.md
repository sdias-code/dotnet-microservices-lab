## 📦 Microsserviço: Catalog.API

Responsável pelo gerenciamento do catálogo de produtos, persistência principal no banco de dados NoSQL e agregação de dados vindos de outros serviços.

### 🔌 Fluxo de Operações (REST Client)

O serviço escuta externamente na porta `5001` (e porta interna `8080` no container).

#### 📥 1. Cadastro de Produto (`POST /produtos`)

* **Fluxo:** 
`REST Client ──(HTTP POST :5001)──> Catalog.API ──> MongoDB`

* **Explicação Técnica:** 
  1. O cliente envia o payload contendo apenas os dados informacionais do produto (`nome` e `descricao`).
  2. A camada `ProdutoService` processa a entrada e delega ao `ProdutoRepository`.
  3. O `MongoContext` efetua a gravação do documento de forma isolada dentro do container do **MongoDB**.
  4. O banco gera e retorna o identificador único (`Id`), devolvendo o status `201 Created` para o client.

#### 📤 2. Consulta Detalhada com Agregação (`GET /detalhes-produto/{id}`)

* **Fluxo:** 
`REST Client ──(HTTP GET)──> Catalog.API ──(Refit HTTP)──> Pricing.API`

* **Explicação Técnica:**
  1. O cliente solicita os detalhes completos de um produto específico através do seu ID.
  2. O `Catalog.API` busca os dados cadastrais (nome e descrição) diretamente na coleção do **MongoDB**.
  3. Com os dados em mãos, o serviço utiliza um cliente **Refit** (`IPrecoApiClient`) para fazer uma chamada síncrona interna para `http://pricing-api:8080/precos/{produtoId}`.
  4. O `Catalog.API` intercepta o preço retornado pelo microsserviço de preços, monta um objeto unificado (*Data Aggregation*) e responde ao cliente com o status `200 OK`.

#### 📋 3. Listagem Geral (`GET /produtos`)

* **Fluxo:** 
`REST Client ──(HTTP GET)──> Catalog.API ──> MongoDB`
* **Explicação Técnica:** 
Realiza uma leitura limpa de todos os registros contidos na coleção do MongoDB. Para otimização de performance, esta listagem não efetua chamadas externas de preços, servindo apenas como catálogo rápido.
