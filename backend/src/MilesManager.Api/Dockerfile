FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/MilesManager.sln backend/
COPY backend/Directory.Build.props backend/
COPY backend/src/MilesManager.Api/MilesManager.Api.csproj backend/src/MilesManager.Api/
COPY backend/src/MilesManager.Application/MilesManager.Application.csproj backend/src/MilesManager.Application/
COPY backend/src/MilesManager.Domain/MilesManager.Domain.csproj backend/src/MilesManager.Domain/
COPY backend/src/MilesManager.Infrastructure/MilesManager.Infrastructure.csproj backend/src/MilesManager.Infrastructure/
COPY backend/tests/MilesManager.Tests/MilesManager.Tests.csproj backend/tests/MilesManager.Tests/

RUN dotnet restore backend/MilesManager.sln

COPY backend/ backend/
WORKDIR /src/backend/src/MilesManager.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MilesManager.Api.dll"]
