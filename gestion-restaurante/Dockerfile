#empaquetar el backend 
# para generar contenedor que posteriormente va a leer Render

# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY gestion-restaurante/gestion-restaurante.Server/ gestion-restaurante/gestion-restaurante.Server/
RUN dotnet restore "gestion-restaurante/gestion-restaurante.Server/gestion-restaurante.Server.csproj"

COPY . .
WORKDIR "/src/gestion-restaurante/gestion-restaurante.Server"
RUN dotnet publish "gestion-restaurante.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Producción (Runtime liviano)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "gestion-restaurante.Server.dll"]