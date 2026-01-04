FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OffndAt.Services.Api/OffndAt.Services.Api.csproj", "OffndAt.Services.Api/"]
COPY ["OffndAt.Application/OffndAt.Application.csproj", "OffndAt.Services.Application/"]
COPY ["OffndAt.Contracts/OffndAt.Contracts.csproj", "OffndAt.Contracts/"]
COPY ["OffndAt.Domain/OffndAt.Domain.csproj", "OffndAt.Domain/"]
COPY ["OffndAt.Infrastructure/OffndAt.Infrastructure.csproj", "OffndAt.Infrastructure/"]
COPY ["OffndAt.Persistence/OffndAt.Persistence.csproj", "OffndAt.Persistence/"]

RUN dotnet restore "OffndAt.Services.Api/OffndAt.Services.Api.csproj"

COPY . .
WORKDIR "/src/OffndAt.Services.Api"
RUN dotnet build "OffndAt.Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OffndAt.Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

USER $APP_UID

COPY --from=publish /app/publish .

# HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
#    CMD curl -f http://localhost:8080/health || exit 1

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "OffndAt.Services.Api.dll"]
