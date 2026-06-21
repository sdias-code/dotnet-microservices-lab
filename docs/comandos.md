
docker compose up -d --build

dotnet add package AspNetCore.HealthChecks.MongoDb --version 9.0.0
dotnet add package AspNetCore.HealthChecks.Redis --version 9.0.0
dotnet add package Serilog.AspNetCore --version 10.0.0
