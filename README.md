# PatriSystem

Sistema de punto de venta (POS) para pequeños comercios, construido con .NET 10 y Clean Architecture.

## Requisitos

- SDK de .NET 10 (`dotnet --version` debe empezar con `10.`)
- Docker Desktop

## Entorno local

1. Copia `.env.example` a `.env` y cambia `MSSQL_SA_PASSWORD` por una contraseña propia (mínimo 8 caracteres, con mayúscula, minúscula, número y símbolo). El archivo `.env` no se sube al repositorio.
2. Levanta los servicios: `docker compose up -d`
3. Comprueba que SQL Server esté `healthy`: `docker compose ps`

| Servicio | Puerto | Para qué sirve |
|---|---|---|
| SQL Server 2022 | 1433 | Base de datos |
| MailHog (SMTP) | 1025 | Recibe los correos que envía la API |
| MailHog (web) | 8025 | Bandeja de correos de prueba: http://localhost:8025 |

### Si el puerto 1433 está ocupado

Cambia `MSSQL_PORT` en `.env` (por ejemplo `14330`), vuelve a ejecutar `docker compose up -d` y usa ese mismo puerto en la cadena de conexión de la API (ver "Secretos en desarrollo").

### Apagar el entorno

- `docker compose down`: detiene los contenedores y **conserva** los datos.
- `docker compose down -v`: detiene los contenedores y **borra todos los datos** de SQL Server. Úsalo solo si quieres empezar desde cero.

## Configuración

La API valida su configuración al arrancar: si falta un valor obligatorio, no inicia y el mensaje indica cuál falta. Los secretos nunca van en `appsettings.json`.

| Nombre | Para qué sirve | Obligatoria | De dónde sale |
|---|---|---|---|
| `Database:ConnectionString` | Cadena de conexión a SQL Server | Sí | user-secrets (desarrollo) o variable de entorno |
| `MSSQL_SA_PASSWORD` | Contraseña del usuario `sa` del contenedor de SQL Server | Sí | Archivo `.env` (lo lee Docker Compose) |
| `MSSQL_PORT` | Puerto de SQL Server en tu máquina | No (por defecto `1433`) | Archivo `.env` |

### Secretos en desarrollo

```
dotnet user-secrets set "Database:ConnectionString" "Server=localhost,1433;Database=master;User Id=sa;Password=<tu-contraseña>;TrustServerCertificate=True" --project src/PatriSystem.Api
```

Usa la misma contraseña y el mismo puerto que definiste en `.env`.

> Mientras la API no tenga EF Core se usa la base `master`, que siempre existe. En el ticket de identidad (AUTH-01) pasará a `PatriSystem`. No crees tablas en `master`.

### Equivalencia con variables de entorno

En producción y en CI los valores se pasan como variables de entorno. El separador `:` se escribe con doble guion bajo:

| Configuración | Variable de entorno |
|---|---|
| `Database:ConnectionString` | `Database__ConnectionString` |

## Estado de la API (health checks)

| Endpoint | Qué responde | Comprueba |
|---|---|---|
| `GET /health/live` | `200 Healthy` si el proceso está vivo | Nada externo |
| `GET /health/ready` | `200 Healthy` si puede atender, `503 Unhealthy` si no | Conexión a SQL Server |

Cada respuesta incluye el encabezado `X-Correlation-Id`, que también aparece en los logs de esa petición. Las respuestas de error nunca incluyen detalles internos fuera de Development.