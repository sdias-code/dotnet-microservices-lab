## Modulo 4 - Cloud Native

### Objetivos

* Kubernetes
* ConfigMaps
* Secrets
* Deploy local com Minikube
* Escalabilidade horizontal

### Arquitetura Final

```text
                 Kubernetes

      +---------------------------+
      |      Catalogo.API         |
      +------------+--------------+
                   |
                   |
             RabbitMQ
                   |
                   v
      +---------------------------+
      |       Precos.API          |
      +------------+--------------+
                   |
        +----------+----------+
        |                     |
      MongoDB              Redis
```

---