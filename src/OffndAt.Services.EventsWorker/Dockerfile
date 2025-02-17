FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OffndAt.Services.EventsWorker/OffndAt.Services.EventsWorker.csproj", "OffndAt.Services.EventsWorker/"]
COPY ["OffndAt.Infrastructure/OffndAt.Infrastructure.csproj", "OffndAt.Infrastructure/"]
COPY ["OffndAt.Persistence/OffndAt.Persistence.csproj", "OffndAt.Persistence/"]
RUN dotnet restore "OffndAt.Services.EventsWorker/OffndAt.Services.EventsWorker.csproj"
COPY . .
WORKDIR "/src/OffndAt.Services.EventsWorker"
RUN dotnet build "OffndAt.Services.EventsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OffndAt.Services.EventsWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OffndAt.Services.EventsWorker.dll"]
