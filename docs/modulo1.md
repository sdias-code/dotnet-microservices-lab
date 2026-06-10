# Projeto: Catálogo de Produtos com Arquitetura de Microserviços

**Objetivo:** estudar comunicação entre microserviços, persistência distribuída, cache, mensageria e orquestração em nuvem.

## Arquitetura Atual (Modulo 1)

```text
+-------------------+
|   Catalogo.API    |
|-------------------|
| MongoDB           |
| Refit Client      |
+---------+---------+
          |
          | HTTP
          v
+-------------------+
|    Precos.API     |
|-------------------|
| Redis Cache       |
+-------------------+
```

### Tecnologias

* .NET 10 / ASP.NET Core Minimal APIs
* MongoDB
* Redis
* Refit
* Docker
* GitHub

### Funcionalidades

* Cadastro de produtos
* Consulta de produtos
* Consulta de preços através de outro microserviço
* Cache distribuído com Redis

---

# Roadmap do Projeto

## Modulo 1 - Comunicação entre Microserviços ✅

### Objetivos

* Criar Catalogo.API
* Criar Precos.API
* Persistência no MongoDB
* Cache com Redis
* Comunicação síncrona via HTTP usando Refit

### Conceitos estudados

* Minimal APIs
* Dependency Injection
* Repository Pattern
* Service Layer
* Refit
* MongoDB Driver
* Redis Cache

---