# Estágio 1: Imagem base para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Estágio 2: Compilação e publicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto usando caminhos relativos ao contexto
COPY ["Pricing.API/Pricing.API.csproj", "Pricing.API/"]
RUN dotnet restore "Pricing.API/Pricing.API.csproj"

# Copia todo o código fonte e compila o projeto
COPY . .
WORKDIR "/src/Pricing.API"
RUN dotnet build "Pricing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pricing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 3: Geração da imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pricing.API.dll"]
