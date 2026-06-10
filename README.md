# .NET Microservices Lab

This repository documents my hands-on journey learning microservices architecture with .NET.

The project evolves through multiple modules, covering service-to-service communication, distributed data management, caching, messaging, containerization, orchestration, and cloud-native practices. Each module introduces new concepts and technologies commonly used in modern distributed systems.

### Topics Covered

- ASP.NET Core Minimal APIs
- MongoDB
- Redis
- Refit
- Docker
- RabbitMQ
- Kubernetes
- Cloud-Native Architecture
- Distributed Systems Patterns


## Module 1 — Microservices Fundamentals and Synchronous Communication

In this first module, a basic microservices architecture was implemented using .NET 10, focusing on separation of concerns, distributed persistence, and inter-service communication.

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
* Dependency Injection
* Repository Pattern
* Service Layer
* Containerization with Docker

### Skills and Concepts Developed

* Microservices design and separation of responsibilities
* Synchronous service-to-service communication
* NoSQL data persistence with MongoDB
* Distributed caching strategies with Redis
* Building modern APIs using Minimal APIs
* Structuring .NET applications following architectural best practices



## Roadmap

### Module 1 — Synchronous Communication ✅

* [x] Catalog.API
* [x] Pricing.API
* [x] MongoDB Persistence
* [x] Redis Distributed Cache
* [x] Refit HTTP Client
* [x] Docker Containers

### Module 2 — Event-Driven Architecture 🚧

* [ ] RabbitMQ
* [ ] Asynchronous Communication
* [ ] Publish/Subscribe Pattern
* [ ] Resilience Patterns

### Module 3 — Container Orchestration 📅

* [ ] Kubernetes
* [ ] Observability
* [ ] Health Checks
* [ ] Scalability

### Module 4 — Cloud-Native Deployment 📅

* [ ] Cloud Deployment
* [ ] CI/CD Pipeline
* [ ] Monitoring
* [ ] Production Environment
