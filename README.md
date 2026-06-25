# .NET Microservices Lab

A production-ready microservices architecture built with **.NET 8/9**, demonstrating enterprise-grade design patterns, asynchronous messaging, cloud-native orchestration, and self-healing resilience.

[![.NET](https://shields.io)](https://microsoft.com)
[![Docker](https://shields.io)](https://docker.com)
[![Kubernetes](https://shields.io)](https://kubernetes.io)

---

## 🚀 Overview

This repository serves as a practical laboratory for highly scalable, distributed systems. It showcases how to handle high-throughput scenarios, data consistency across boundaries, resilient communication, and professional container orchestration using a local production-like cloud ecosystem.

## 🛠️ Tech Stack & Ecosystem

- **Backend:** .NET 8/9 C#, ASP.NET Core Web API, Minimal APIs
- **Data & Storage:** MongoDB (NoSQL Catalogue), Redis (Distributed Caching)
- **Messaging & Event-Driven:** RabbitMQ / MassTransit (Asynchronous communication & DLQ engine)
- **Containerization & Orchestration:** Docker, Kubernetes (k8s), Minikube
- **Edge Routing & Ingress:** NGINX Ingress Controller
- **Observability:** OpenTelemetry, Seq / Jaeger (Distributed Tracing), Native .NET Health Checks

---

## 🏛️ Architecture & Patterns

The project strictly follows modern architectural blueprints used by global enterprise companies:

- **Clean Architecture:** Strict separation of concerns, keeping domain logic independent of external frameworks, messaging brokers, or databases.
- **CQRS (Command Query Responsibility Segregation):** Optimized read and write database structures utilizing **MediatR**.
- **Event-Driven Architecture:** Loose coupling between microservices using the asynchronous Publish-Subscribe pattern.
- **Outbox Pattern:** Ensuring reliable message delivery and eventual consistency during network or downstream service failures.
- **Edge Gateway Routing:** Native Kubernetes Ingress managing reverse-proxy configurations, regular expression rewrites, and unified access paths.
- **Self-Healing & Resource Bounds:** Native Kubernetes Liveness and Readiness probes linked to .NET Health Checks to guarantee automatic error recovery and prevent cluster starvation (Noisy Neighbor issues).

---

## 📦 Services Included

- **Catalog.API:** Manages product catalog data backed by a non-relational MongoDB data layer and triggers real-time integration events upon updates.
- **Pricing.API:** Handles product tier prices leveraging high-performance Refit calls, background event consumers, and low-latency Redis caching.

---

## ⚙️ Getting Started

### Prerequisites
- [.NET 8 SDK / .NET 9 SDK](https://microsoft.com)
- [Docker Engine](https://docker.com)
- [Minikube](https://k8s.io)
- [kubectl](https://kubernetes.io)

### Running with Kubernetes (Minikube)

1. Clone this repository:
   ```bash
   git clone https://github.com
   cd dotnet-microservices-lab/src
   ```

2. Start your local Kubernetes cluster and enable the Ingress Controller:
   ```bash
   minikube start --driver=docker
   minikube addons enable ingress
   ```

3. Deploy the database infrastructure, messaging broker, and applications:
   ```bash
   kubectl apply -f k8s/
   ```

4. Configure your local domain mapping. Get your cluster IP via `minikube ip` and append it to your system `/etc/hosts` file:
   ```text
   # Example entry inside /etc/hosts
   192.168.49.2  api.local
   ```

5. Open a native network bridge in a separate terminal tab:
   ```bash
   minikube tunnel
   ```

6. Access your microservices suite transparently on the standard HTTP port (80) using your custom routing:
   * **Catalog API :** `http://api.local/api/catalog/products`
   * **Pricing API :** `http://api.local/api/pricing/prices`

---

## 🧑‍💻 Author

**S. Dias**  
*.NET & Distributed Systems Specialist*  
- GitHub: [@sdiascode](https://github.com)  
- LinkedIn: [https://www.linkedin.com/in/sdias2026/](https://www.linkedin.com/in/sdias2026/)
