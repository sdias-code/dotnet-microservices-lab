
docker compose up -d --build

dotnet add package AspNetCore.HealthChecks.MongoDb --version 9.0.0
dotnet add package AspNetCore.HealthChecks.Redis --version 9.0.0
dotnet add package Serilog.AspNetCore --version 10.0.0
dotnet add package MassTransit.RabbitMQ --version 8.3.6
dotnet add package Microsoft.Extensions.Http.Resilience

RabbitMQ Management:
http://localhost:15672/


// kubernets
# Aplica todos os arquivos contidos na pasta k8s
 kubectl apply -f k8s/

# Verificar se os bancos subiram usando:
 kubectl get pods
 kubectl get pods -w
 kubectl get ingress


# Habilitando o Ingress no Minikube 
minikube addons enable ingress

minikube ip

minikube service rabbitmq-service

kubectl port-forward service/catalog-api-service 5001:8080
kubectl port-forward service/pricing-api-service 5002:8080

minikube tunnel

