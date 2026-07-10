# PanComido — Backend API

API REST del sistema de gestión de restaurante PanComido.

| Stack | Versión |
|-------|---------|
| .NET | 8 |
| Entity Framework Core | 8 |
| PostgreSQL | 15 |
| Autenticación | JWT Bearer |
| Tiempo real | SignalR |

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para la base de datos)

## Inicio rápido

```bash
# 1. Levantar base de datos
docker compose up -d

# 2. Ir al proyecto de presentación
cd backend/PanComido.Presentacion

# 3. Restaurar dependencias
dotnet restore

# 4. Ejecutar
dotnet run
```

La API se levanta en `https://localhost:7204` (swagger en `/swagger`).

## Frontend

El frontend Angular de PanComido vive en un repositorio separado: [PanComido-frontend](https://github.com/Urielito1031/PanComido-frontend). 
