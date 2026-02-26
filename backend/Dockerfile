# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder
WORKDIR /src

# Copy solution and project files
COPY Hypesoft.Challenge.sln .
COPY src/Hypesoft.API/Hypesoft.API.csproj ./src/Hypesoft.API/
COPY src/Hypesoft.Application/Hypesoft.Application.csproj ./src/Hypesoft.Application/
COPY src/Hypesoft.Domain/Hypesoft.Domain.csproj ./src/Hypesoft.Domain/
COPY src/Hypesoft.Infrastructure/Hypesoft.Infrastructure.csproj ./src/Hypesoft.Infrastructure/
COPY tests/Hypesoft.UnitTests/Hypesoft.UnitTests.csproj ./tests/Hypesoft.UnitTests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the solution
RUN dotnet build -c Release

# Publish
RUN dotnet publish src/Hypesoft.API/Hypesoft.API.csproj -c Release -o /app/publish --no-build

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=builder /app/publish .

EXPOSE 5000

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "Hypesoft.API.dll"]
