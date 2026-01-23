FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["OffndAt.Services.EventsWorker/OffndAt.Services.EventsWorker.csproj", "OffndAt.Services.EventsWorker/"]
COPY ["OffndAt.Services.EventsWorker/packages.lock.json", "OffndAt.Services.EventsWorker/"]
COPY ["OffndAt.Application/OffndAt.Application.csproj", "OffndAt.Application/"]
COPY ["OffndAt.Application/packages.lock.json", "OffndAt.Application/"]
COPY ["OffndAt.Contracts/OffndAt.Contracts.csproj", "OffndAt.Contracts/"]
COPY ["OffndAt.Contracts/packages.lock.json", "OffndAt.Contracts/"]
COPY ["OffndAt.Domain/OffndAt.Domain.csproj", "OffndAt.Domain/"]
COPY ["OffndAt.Domain/packages.lock.json", "OffndAt.Domain/"]
COPY ["OffndAt.Infrastructure/OffndAt.Infrastructure.csproj", "OffndAt.Infrastructure/"]
COPY ["OffndAt.Infrastructure/packages.lock.json", "OffndAt.Infrastructure/"]
COPY ["OffndAt.Persistence/OffndAt.Persistence.csproj", "OffndAt.Persistence/"]
COPY ["OffndAt.Persistence/packages.lock.json", "OffndAt.Persistence/"]

RUN dotnet restore "OffndAt.Services.EventsWorker/OffndAt.Services.EventsWorker.csproj" --locked-mode

COPY . .
WORKDIR "/src/OffndAt.Services.EventsWorker"
RUN dotnet build "OffndAt.Services.EventsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OffndAt.Services.EventsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app

ARG ApplicationSettings__Version
ENV ApplicationSettings__Version=$ApplicationSettings__Version

USER $APP_UID

COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "OffndAt.Services.EventsWorker.dll"]
