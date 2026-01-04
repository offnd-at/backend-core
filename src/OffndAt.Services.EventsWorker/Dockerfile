FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OffndAt.Services.EventsWorker/OffndAt.Services.EventsWorker.csproj", "OffndAt.Services.EventsWorker/"]
COPY ["OffndAt.Application/OffndAt.Application.csproj", "OffndAt.Services.Application/"]
COPY ["OffndAt.Contracts/OffndAt.Contracts.csproj", "OffndAt.Contracts/"]
COPY ["OffndAt.Domain/OffndAt.Domain.csproj", "OffndAt.Domain/"]
COPY ["OffndAt.Infrastructure/OffndAt.Infrastructure.csproj", "OffndAt.Infrastructure/"]
COPY ["OffndAt.Persistence/OffndAt.Persistence.csproj", "OffndAt.Persistence/"]

RUN dotnet restore "OffndAt.Services.EventsWorker/OffndAt.Services.EventsWorker.csproj"

COPY . .
WORKDIR "/src/OffndAt.Services.EventsWorker"
RUN dotnet build "OffndAt.Services.EventsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OffndAt.Services.EventsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

USER $APP_UID

COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "OffndAt.Services.EventsWorker.dll"]
