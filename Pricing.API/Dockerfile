# Stage 1: Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage 2: Build and publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy Shared structure (Required for the reference to exist)
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]

# Copy project file using paths relative to build context
COPY ["Pricing.API/Pricing.API.csproj", "Pricing.API/"]
RUN dotnet restore "Pricing.API/Pricing.API.csproj"

# Copy all source code and build the project
COPY . .
WORKDIR "/src/Pricing.API"
RUN dotnet build "Pricing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pricing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Generate final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pricing.API.dll"]
