FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OffndAt.Services.Background/OffndAt.Services.Background.csproj", "OffndAt.Services.Background/"]
COPY ["OffndAt.Infrastructure/OffndAt.Infrastructure.csproj", "OffndAt.Infrastructure/"]
RUN dotnet restore "OffndAt.Services.Background/OffndAt.Services.Background.csproj"
COPY . .
WORKDIR "/src/OffndAt.Services.Background"
RUN dotnet build "OffndAt.Services.Background.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OffndAt.Services.Background.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OffndAt.Services.Background.dll"]
