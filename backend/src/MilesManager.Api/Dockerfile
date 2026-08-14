FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and shared props
COPY MilesManager.sln ./
COPY Directory.Build.props ./

# Copy project files
COPY src/MilesManager.Api/MilesManager.Api.csproj src/MilesManager.Api/
COPY src/MilesManager.Application/MilesManager.Application.csproj src/MilesManager.Application/
COPY src/MilesManager.Domain/MilesManager.Domain.csproj src/MilesManager.Domain/
COPY src/MilesManager.Infrastructure/MilesManager.Infrastructure.csproj src/MilesManager.Infrastructure/
COPY tests/MilesManager.Tests/MilesManager.Tests.csproj tests/MilesManager.Tests/

# Restore packages
RUN dotnet restore MilesManager.sln

# Copy the remaining source code
COPY . .

# Publish
WORKDIR /src/src/MilesManager.Api

RUN dotnet publish -c Release -o /app/publish

# ============================================================
# Runtime image
# ============================================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_EnableDiagnostics=0

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MilesManager.Api.dll"]