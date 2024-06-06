FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OffndAt.Services.Api/OffndAt.Services.Api.csproj", "OffndAt.Services.Api/"]
COPY ["OffndAt.Application/OffndAt.Application.csproj", "OffndAt.Services.Api/"]
COPY ["OffndAt.Contracts/OffndAt.Contracts.csproj", "OffndAt.Contracts/"]
COPY ["OffndAt.Domain/OffndAt.Domain.csproj", "OffndAt.Domain/"]
COPY ["OffndAt.Infrastructure/OffndAt.Infrastructure.csproj", "OffndAt.Infrastructure/"]
COPY ["OffndAt.Persistence/OffndAt.Persistence.csproj", "OffndAt.Persistence/"]
RUN dotnet restore "OffndAt.Services.Api/OffndAt.Services.Api.csproj"
COPY . .
WORKDIR "/src/OffndAt.Services.Api"
RUN dotnet build "OffndAt.Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OffndAt.Services.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OffndAt.Services.Api.dll"]
