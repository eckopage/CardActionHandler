# 🏗️ Etap 1: Podstawowy obraz z .NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 🏗️ Etap 2: Obraz z .NET SDK do budowania aplikacji
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiowanie pliku projektu do odpowiedniego katalogu
COPY ["Card.API/Card.API.csproj", "Card.API/"]

COPY ["Services/Card/Card.Application/Card.Application.csproj", "Services/Card/Card.Application/"]
COPY ["Services/Basket/Basket.Core/Basket.Core.csproj", "Services/Basket/Basket.Core/"]
COPY ["Services/Basket/Basket.Infrastructure/Basket.Infrastructure.csproj", "Services/Basket/Basket.Infrastructure/"]
COPY ["Infrastructure/EventBus.Messages/EventBus.Messages.csproj", "Infrastructure/EventBus.Messages/"]
COPY ["Infrastructure/Common.Logging/Common.Logging.csproj", "Infrastructure/Common.Logging/"]

RUN dotnet restore "Card.API/Card.API.csproj"

# Kopiowanie całej zawartości projektu
COPY . .
WORKDIR "/src/Card.API"

# Kompilacja aplikacji
RUN dotnet build "Card.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 🏗️ Etap 3: Publikowanie aplikacji
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Card.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 🏗️ Etap 4: Finalny obraz do uruchamiania aplikacji
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Card.API.dll"]
