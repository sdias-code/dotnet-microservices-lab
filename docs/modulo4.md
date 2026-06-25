
------------------------------
## Module 4 — Cloud Native Architecture & Advanced Observability 🌐📊
Neste módulo, nosso objetivo é elevar a maturidade do nosso cluster Kubernetes para o nível corporativo. Vamos remover dados sensíveis e configurações fixas de dentro dos nossos manifestos utilizando ConfigMaps e Secrets, implementar Escalabilidade Horizontal automática e instrumentar toda a arquitetura com os três pilares da observabilidade unificada via OpenTelemetry.
------------------------------
## 🏛️ Target Cloud-Native Architecture

                               Kubernetes Cluster (Minikube)
                      
                         [ Ingress Controller (api.local) ]
                                         |
               +-------------------------+-------------------------+

               | /api/catalog                                      | /api/pricing
               v                                                   v
     [ catalog-api-service ]                            [ pricing-api-service ]

               |                                                   |
      +--------+--------+                                 +--------+--------+

      |        |        | (Horizontal Scaling)            |        |        |
    [Pod]    [Pod]    [Pod]                             [Pod]    [Pod]    [Pod]

      |        |        |                                 |        |        |
      +--------+--------+                                 +--------+--------+

               |                                                   |
               +-----------------+               +-----------------+

                                 |               |
                                 v               v
                             [ rabbitmq-service ]
                                     |
                                     v
                        +------------+------------+

                        |                         |
               [ mongodb-service ]       [ redis-service ]

------------------------------
## 🎯 Core Objectives & Deliverables## 1. Configuration Management (ConfigMaps & Secrets)

* ConfigMaps: Extract non-sensitive application settings (such as environment names, target service URLs, and logging levels) into centralized k8s ConfigMap objects.
* Secrets: Securely encrypt database connection strings, root passwords, and message broker credentials using base64 k8s Secrets, mounting them safely as environment variables into .NET containers.

## 2. High Availability & Horizontal Scaling (HPA)

* Replication: Configure multiple initial replicas inside deployments to guarantee high availability during rollouts.
* Horizontal Pod Autoscaler (HPA): Deploy metric-driven autoscaling to dynamically multiply or shrink .NET container instances based on real-time CPU and Memory thresholds during high-throughput workloads.

## 3. OpenTelemetry Code Instrumentation (.NET Core)

* Distributed Tracing: Implement full request lifecycle visibility by auto-instrumenting HTTP requests, MongoDB clients, Redis multiplexers, and MassTransit pipelines using standard W3C Trace Contexts.
* Metrics Integration: Export performance indicators (such as request rates, processing duration, error margins, and custom business counters) directly to an out-of-process scraping target.

## 4. Local Cloud Infrastructure & Visualization (Telemetry Stack)

* Prometheus & Grafana: Deploy standard monitoring agents into the Minikube cluster to scrape live metrics and build unified executive dashboards tracking cluster resource utilization and microservices availability.
* Jaeger Tracing Endpoints: Setup a distributed tracing visualization panel to easily inspect structural latencies and trace integration events traversing across boundary limits.

------------------------------
## 🧠 Mastered Cloud-Native Concepts

* Decoupled Configuration: Understanding how separating application runtime properties from deployment code prevents secret leakage and allows identical binary image delivery across diverse testing/production stages.
* Elastic Resource Scaling: Mastering the difference between horizontal scale-out (adding instances) and vertical scale-up (adding power), while evaluating how k8s controls request distribution across active ephemeral pods.
* Distributed Tracing Propagation: Demystifying how cross-boundary protocols preserve trace identities using transport headers (traceparent) to link a single user interaction to multiple dependent backend activities.

------------------------------


