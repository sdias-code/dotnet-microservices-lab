# .NET Microservices Lab

This repository documents my hands-on journey learning microservices architecture with .NET.

The project evolves through multiple modules, covering service-to-service communication, distributed data management, caching, messaging, containerization, orchestration, and cloud-native practices. Each module introduces new concepts and technologies commonly used in modern distributed systems.

### Topics Covered

- ASP.NET Core Minimal APIs
- MongoDB
- Redis
- Refit
- Docker & Docker Compose
- RabbitMQ & MassTransit (Pub/Sub)
- Resilience Patterns (Polly v8)
- Dead Letter Queues (DLQ) & Scheduled Redelivery
- Cloud-Native Architecture
- Distributed Systems Patterns

## Module 1 — Microservices Fundamentals and Synchronous Communication

In this first module, a basic microservices architecture was implemented using .NET, focusing on separation of concerns, distributed persistence, and inter-service communication.

The solution is composed of two independent microservices:

* **Catalog.API**: responsible for product management and data persistence using MongoDB.
* **Pricing.API**: responsible for providing product pricing information, using Redis as a distributed cache.

Communication between the services was implemented synchronously over HTTP using Refit, allowing the Catalog service to retrieve pricing information directly from the specialized Pricing service.

### Key Implementations
* Microservices-based architecture
* APIs built with ASP.NET Core Minimal APIs
* Data persistence with MongoDB
* Distributed caching with Redis
* Inter-service communication using Refit
* Containerization with Docker

---

## Module 2 — Event-Driven Architecture and Resilience 🆕

In the second module, the architecture evolved into an asynchronous, event-driven ecosystem. The communication became decoupled through a message broker, ensuring that high-throughput operations do not choke the system, while advanced resilience frameworks protect the application from cascading failures and network partitions.

### Key Implementations
* **Asynchronous Messaging**: Implemented the Publish/Subscribe pattern using **RabbitMQ** as the message broker and **MassTransit** as the high-level abstraction library.
* **Eventual Consistency**: When a product is saved in the Catalog database, a `ProductCreatedEvent` is published, causing the Pricing service to automatically initialize a default price in Redis asynchronously.
* **Advanced Error Handling & DLQ**: Configured an automated **Scheduled Redelivery** mechanism using RabbitMQ delayed message exchanges, creating active retry windows before routing corrupted or unprocessable messages to a permanent **Dead Letter Queue (DLQ)**.
* **HTTP Fault Tolerance**: Injected **Polly v8** standard resilience handlers into the Refit HTTP pipeline, applying Exponential Backoff Retries, a 30-second Request Timeout strategy, and a **Circuit Breaker** to safe-guard communication when endpoints become transiently unavailable.
* **Infrastructure Reliability**: Adjusted Kestrel and StackExchange.Redis drivers with non-blocking failover routines (`abortConnect=false`) and automated Docker Compose health-checks to enable seamless environment self-healing.

> 📄 For an in-depth look at how these strategies handle infrastructure crashes and network failures, check out the specialized [Resilience & DLQ Documentation](docs/dlq.md).

---

## Roadmap

### Module 1 — Synchronous Communication ✅
* [x] Catalog.API
* [x] Pricing.API
* [x] MongoDB Persistence
* [x] Redis Distributed Cache
* [x] Refit HTTP Client
* [x] Docker Containers

### Module 2 — Event-Driven Architecture ✅ 🆕
* [x] RabbitMQ Integration
* [x] MassTransit Abstraction
* [x] Asynchronous Communication (Pub/Sub)
* [x] Resilience Patterns (Polly & Circuit Breaker)
* [x] Active Dead Letter Queues (DLQ) & Redelivery

### Module 3 — Container Orchestration 🚧
* [ ] Kubernetes Deployment
* [ ] Advanced Health Checks & Probes
* [ ] Observability & Metrics
* [ ] Scalability & Load Balancing

### Module 4 — Cloud-Native Deployment 📅
* [ ] Cloud Deployment
* [ ] CI/CD Pipeline
* [ ] Monitoring & Distributed Tracing
* [ ] Production Environment
