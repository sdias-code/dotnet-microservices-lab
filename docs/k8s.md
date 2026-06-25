
------------------------------

# Module 3 — Container Orchestration with Kubernetes (k8s) 🚀
This documentation registers the migration of the e-commerce microservices infrastructure from a local `docker-compose` environment to a resilient, production-ready **Kubernetes (k8s)** cluster managed via **Minikube** on Ubuntu 24.04.
---## 🗺️ Execution Summary & Architecture Overview
The local deployment was structured using native Kubernetes objects to ensure data persistence, internal service discovery, decoupled networking, and self-healing resilience for the .NET Core applications.


[ Ingress Controller (api.local) ]
|
+-------------------------+-------------------------+
| /api/catalog | /api/pricing
v v
[ catalog-api-service ] [ pricing-api-service ]
| |
[ catalog-api-pod ] [ pricing-api-pod ]
| | | |
v v v v
[mongodb-pvc] [rabbitmq-service] [redis-service] [rabbitmq-service]


### Key Concepts Mastered:
* **Pods & Deployments**: Scaled .NET applications independently with specific resource bounds.
* **Services (ClusterIP & NodePort)**: Isolated databases internally while allowing controlled administrative access.
* **Persistent Volume Claims (PVC)**: Guaranteed stateful storage integrity for MongoDB and Redis.
* **Probes (Liveness & Readiness)**: Connected .NET Health Checks directly to k8s orchestration for automatic self-healing.
* **Ingress Routing**: Standardized external entry point on port 80 via NGINX Ingress reverse proxy using regex rewrites.

---

## 🛠️ Step-by-Step Implementation

### 1. Local Environment Preparation
* Installed and configured `kubectl` (v1.36) and `minikube` using the local native Docker driver.
* Verified cluster availability: `kubectl get nodes` pointing to `minikube` control-plane.

### 2. Data Infrastructure & Messaging Setup
Applied declarative manifests (`infra-data.yaml`) for database engines, cache, and messaging layers:
* **MongoDB**: Mounted over a 1Gi PersistentVolumeClaim (`mongodb-pvc`) exposing port 27017, complemented by **Mongo-Express** via NodePort (`30081`).
* **Redis**: Configured cache structure passing the `--appendonly yes` argument over a 1Gi PVC, complemented by **RedisInsight** on port `30540`.
* **RabbitMQ**: Provisioned the AMQP broker (`5672`) along with the web management dashboard exposed via NodePort (`31672`).

### 3. CI/CD & Image Hosting (Docker Hub)
* Standardized .NET 8 multi-stage build Dockerfiles.
* Authenticated and pushed high-utility, production-ready images to the remote registry account `sdiascode`:
  ```bash
  docker build -t sdiascode/catalog-api:latest -f Catalog.API/Dockerfile .
  docker push sdiascode/catalog-api:latest
  
  docker build -t sdiascode/pricing-api:latest -f Pricing.API/Dockerfile .
  docker push sdiascode/pricing-api:latest
  ```

### 4. Application Deployments & Native Resilience
Created `catalog-api.yml` and `pricing-api.yml` manifests injecting explicit environmental values mapping internal DNS strings (`mongodb-service`, `redis-service`, `rabbitmq-service`).

* **Resource Management (Noisy Neighbor Mitigation)**: Bound CPU and memory resources to avoid cluster starvation:
  * Requests: `128Mi` memory / `100m` CPU.
  * Limits: `256Mi` memory / `500m` CPU.
* **Health Probes Configuration**:
  * **Liveness Probe**: Monitored `/health` endpoint every 10s after a 15s initial startup delay to catch deadlocks.
  * **Readiness Probe**: Checked database/broker connectivity before routing user traffic to the pod.

### 5. Edge Routing & Ingress Configuration
* Enabled NGINX Ingress inside the cluster: `minikube addons enable ingress`.
* Provisioned `ingress.yml` using `ImplementationSpecific` paths with regular expressions (`nginx.ingress.kubernetes.io/use-regex: "true"`) and prefix stripping (`rewrite-target: /$1`).
* Mapped domain routing on host system (`/etc/hosts`) linking `minikube ip` to `api.local`.
* Initiated cluster-to-host bridge routing engine: `minikube tunnel`.

---

## 🔬 Practical Debugging & Troubleshooting History

### 1. NGINX Ingress Validation Block
* **Issue**: Error from server (BadRequest) during `ingress.yml` application. Recent NGINX Ingress controllers disallow regex routes combined with standard `Prefix` types.
* **Resolution**: Updated the path types to `ImplementationSpecific` and appended the explicit `use-regex` annotation flag.

### 2. Pricing.API `CrashLoopBackOff` Loop
* **Issue**: The application container crashed repeatedly due to MassTransit failing to resolve the message broker location. The C# source code contained a hardcoded host reference (`cfg.Host("rabbitmq")`).
* **Resolution**: Factored the `Program.cs` file to dynamically bind the broker host address to the `ConnectionStrings__RabbitMq` environment variable injected by the k8s container definition, ensuring smooth multi-environment execution.

---

## 🧪 Postman / REST Client Test Suite

Full production suite mapping exposed routes cleanly at `http://api.local` via proxy-pass routing rules:

```http
# ==============================================================================
# 🛍️ MICROSSERVIÇO: CATALOG.API
# ==============================================================================

### Register Product 1 (Triggers MassTransit Integration Event)
POST http://api.local/api/catalog/products
Content-Type: application/json

{
  "name": "Mouse 3 buttons NEWTOP",
  "description": "Mouse with 3 buttons and 1 wheel and USB connection"
}

###

### Register Product 2
POST http://api.local/api/catalog/products
Content-Type: application/json

{
  "name": "Webcam 1080p Logintech C920",
  "description": "Webcam with 1080p resolution and built-in microphone, perfect for video calls and streaming"
}

###



# ==============================================================================
# ⚡ MICROSSERVIÇO: PRICING.API
# ==============================================================================

### Set Base Price for Product 1
POST http://api.local/api/pricing/prices
Content-Type: application/json

{ 
  "productId": "6a3ca03ebb3c63948fb7e9b3", 
  "value": 228.90 
}

###

### Set Base Price for Product 2
POST http://api.local/api/pricing/prices
Content-Type: application/json

{ 
  "productId": "6a39ad29bc352605b53472d7", 
  "value": 355.50 
}

###


```

------------------------------


