## Módulo 1 — Fundamentos de Microserviços e Comunicação Síncrona

Neste primeiro módulo foi implementada uma arquitetura básica de microserviços utilizando .NET 10, com foco na separação de responsabilidades, persistência distribuída e comunicação entre serviços.

O projeto é composto por dois microserviços independentes:

* **Catalogo.API**: responsável pelo gerenciamento dos produtos e persistência dos dados no MongoDB.
* **Precos.API**: responsável pelo fornecimento de preços dos produtos, utilizando Redis para cache distribuído.

A comunicação entre os serviços foi implementada de forma síncrona via HTTP utilizando Refit, permitindo que o Catálogo consulte informações de preço diretamente no microserviço especializado.

### Principais implementações

* Arquitetura baseada em microserviços
* APIs desenvolvidas com ASP.NET Core Minimal APIs
* Persistência de dados com MongoDB
* Cache distribuído com Redis
* Comunicação entre microserviços utilizando Refit
* Injeção de dependência (Dependency Injection)
* Repository Pattern
* Service Layer
* Conteinerização com Docker

### Competências desenvolvidas

* Design e separação de responsabilidades em microserviços
* Comunicação síncrona entre serviços
* Persistência NoSQL com MongoDB
* Estratégias de cache com Redis
* Estruturação de APIs modernas utilizando Minimal APIs
* Organização de projetos seguindo boas práticas de arquitetura em .NET


Módulo 1 ✅
├── Comunicação síncrona entre microserviços
├── MongoDB
├── Redis
├── Refit
└── Docker

Módulo 2 🚧
├── RabbitMQ
├── Comunicação assíncrona
├── Event Driven Architecture
└── Resiliência

Módulo 3 📅
├── Kubernetes
├── Observabilidade
├── Health Checks
└── Escalabilidade

Módulo 4 📅
├── Deploy em Cloud
├── CI/CD
├── Monitoramento
└── Ambiente de Produção