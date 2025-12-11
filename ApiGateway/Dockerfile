FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# ВАЖНО: Копируем csproj из папки ApiGateway внутрь образа в папку ApiGateway
COPY ["ApiGateway/ApiGateway.csproj", "ApiGateway/"]
# Восстанавливаем зависимости
RUN dotnet restore "ApiGateway/ApiGateway.csproj"
# Копируем всё остальное
COPY . .
# Переходим в папку проекта внутри образа
WORKDIR "/src/ApiGateway"
# Собираем
RUN dotnet build "ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
