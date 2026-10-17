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

Cambia `MSSQL_PORT` en `.env` (por ejemplo `14330`) y vuelve a ejecutar `docker compose up -d`. Cuando la API tenga su cadena de conexión (FND-04), deberá usar ese mismo puerto.

### Apagar el entorno

- `docker compose down`: detiene los contenedores y **conserva** los datos.
- `docker compose down -v`: detiene los contenedores y **borra todos los datos** de SQL Server. Úsalo solo si quieres empezar desde cero.