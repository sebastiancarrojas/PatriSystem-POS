# PatriSystem

Sistema de punto de venta (POS) para pequeños comercios, construido con .NET 10 y Clean Architecture.

[![CI](https://github.com/sebastiancarrojas/PatriSystem-POS/actions/workflows/ci.yml/badge.svg)](https://github.com/sebastiancarrojas/PatriSystem-POS/actions/workflows/ci.yml)

> **Estado:** en construcción. Hoy el proyecto tiene las fundaciones (arquitectura, entorno local, configuración, logs, salud, documentación de la API, pruebas e integración continua) y todavía no tiene funcionalidad de negocio.

## Requisitos

- **Git**
- **SDK de .NET 10, versión 10.0.204 o superior.** Comprueba con `dotnet --version`: una versión anterior no compila, porque `global.json` lo exige. Descarga en https://dotnet.microsoft.com/download/dotnet/10.0
- **Docker Desktop.** En Windows necesita WSL 2 y la virtualización activada.
- **Puertos libres:** 1433, 1025 y 8025.
- **Memoria:** SQL Server necesita al menos 2 GB disponibles para Docker.
- **Tiempo para la primera descarga:** la imagen de SQL Server es grande y puede tardar desde unos minutos hasta cerca de una hora según la conexión. Las siguientes veces ya está en caché.

## Cómo levantar el proyecto

1. **Clona el repositorio** y entra en la carpeta:

   ```
   git clone https://github.com/sebastiancarrojas/PatriSystem-POS.git
   cd PatriSystem-POS
   ```

2. **Crea tu archivo `.env`** a partir de la plantilla. Cambia `MSSQL_SA_PASSWORD` por una contraseña propia (mínimo 8 caracteres, con mayúscula, minúscula, número y símbolo). El archivo `.env` no se sube al repositorio:

   ```
   copy .env.example .env      # Windows
   cp .env.example .env        # Linux y macOS
   ```

3. **Levanta los servicios.** La primera vez descarga las imágenes, que es lo que más tarda. Espera a que SQL Server aparezca como `healthy`. Si `docker compose` dice que no puede conectar con el motor de Docker, abre Docker Desktop y espera a que indique *Engine running*:

   ```
   docker compose up -d
   docker compose ps
   ```

4. **Guarda la cadena de conexión** en los secretos de usuario. Usa la misma contraseña y el mismo puerto que definiste en `.env`:

   ```
   dotnet user-secrets set "Database:ConnectionString" "Server=localhost,1433;Database=PatriSystem;User Id=sa;Password=<tu-contraseña>;TrustServerCertificate=True" --project src/PatriSystem.Api
   ```

   > La base PatriSystem la crea el paso siguiente (migraciones). No uses la base master para nada del proyecto.

5. **Crea la base de datos y las tablas** con las migraciones de EF Core. El comando crea la base `PatriSystem` si no existe. Hay que repetirlo cuando llegue una migración nueva:

   ```
   dotnet tool restore
   dotnet ef database update --project src/PatriSystem.Infrastructure --startup-project src/PatriSystem.Api
   ```

6. **Arranca la API:**

   ```
   dotnet run --project src/PatriSystem.Api
   ```

7. **Comprueba que responde.** Abre `http://localhost:5062/health/ready` en el navegador: debe mostrar `Healthy`. En la consola verás un aviso `Failed to determine the https port for redirect`: es esperado por ahora, porque HTTPS todavía no está configurado, y no afecta.

Para ejecutar las pruebas: `dotnet test PatriSystem.slnx`.

### Servicios y puertos

| Servicio | Puerto | Para qué sirve |
|---|---|---|
| SQL Server 2022 | 1433 | Base de datos |
| MailHog (SMTP) | 1025 | Recibe los correos que envía la API |
| MailHog (web) | 8025 | Bandeja de correos de prueba: http://localhost:8025 |

### Si el puerto 1433 está ocupado

Cambia `MSSQL_PORT` en `.env` (por ejemplo `14330`), vuelve a ejecutar `docker compose up -d` y usa ese mismo puerto en la cadena de conexión del paso 4.

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
| `Identity:SeedOnStartup` | Crea los roles Admin y Cajero al arrancar, si no existen | No (por defecto `true`) | `appsettings` o variable de entorno |

En producción y en CI los valores se pasan como variables de entorno. El separador `:` se escribe con doble guion bajo:

| Configuración | Variable de entorno |
|---|---|
| `Database:ConnectionString` | `Database__ConnectionString` |
| `Identity:SeedOnStartup` | `Identity__SeedOnStartup` |

## Estado de la API (health checks)

| Endpoint | Qué responde | Comprueba |
|---|---|---|
| `GET /health/live` | `200 Healthy` si el proceso está vivo | Nada externo |
| `GET /health/ready` | `200 Healthy` si puede atender, `503 Unhealthy` si no | Conexión a SQL Server |

Cada respuesta incluye el encabezado `X-Correlation-Id`, que también aparece en los logs de esa petición. Las respuestas de error nunca incluyen detalles internos fuera de Development.

## Documentación de la API

Con la API en ejecución, en Development:

| URL | Qué es |
|---|---|
| `http://localhost:5062/scalar` | Interfaz interactiva (Scalar) |
| `http://localhost:5062/openapi/v1.json` | Documento OpenAPI |

La documentación no se publica fuera de Development. `GET /api/ping` es un endpoint de ejemplo que devuelve `{ "status": "ok" }`. La decisión está en el [ADR-003](docs/adr/0003-documentacion-api-openapi-scalar.md).

## Pruebas

```
dotnet test PatriSystem.slnx
```

| Proyecto | Tipo | Qué prueba |
|---|---|---|
| `PatriSystem.Application.Tests` | Unitarias y de arquitectura | Casos de uso y reglas, sin base de datos ni servidor web, y las reglas de dependencias entre capas |
| `PatriSystem.Api.IntegrationTests` | Integración | La API completa en memoria, con `WebApplicationFactory` |

Herramientas: xUnit, NSubstitute (dobles de prueba), Shouldly (aserciones) y NetArchTest (arquitectura). No se usa FluentAssertions por su cambio a licencia de pago.

Los nombres de las pruebas siguen el patrón `Method_Scenario_ExpectedResult`. Las pruebas de integración usan el entorno `Testing`, un valor falso de `Database:ConnectionString` y desactivan la siembra de roles con `Identity:SeedOnStartup=false`, por lo que las pruebas actuales no necesitan Docker ni secretos locales.

Las pruebas de arquitectura están en `tests/PatriSystem.Application.Tests/Architecture/` y hacen cumplir las reglas del [ADR-001](docs/adr/0001-clean-architecture.md): Domain no depende de nada, Application solo de Domain y de una lista explícita de paquetes, y ninguna de las dos usa EF Core, ASP.NET, Identity ni JWT. Si una capa necesita un paquete nuevo, se agrega a `AllowedApplicationPackages` en el mismo PR. Las reglas sobre tipos todavía no comprueban nada, porque Domain y Application casi no tienen código.

## Integración continua

Cada PR y cada push a `main` ejecutan el workflow `CI` (`.github/workflows/ci.yml`): restauración con caché, revisión de formato, compilación en Release con advertencias como errores y pruebas con cobertura (el informe queda como artefacto de la ejecución). Para reproducirlo en local:

```
dotnet format PatriSystem.slnx --verify-no-changes
dotnet build PatriSystem.slnx --configuration Release -warnaserror
dotnet test PatriSystem.slnx --configuration Release --collect:"XPlat Code Coverage"
```

## Estructura

```
PatriSystem-POS/
├── .github/                       Workflow de CI y plantillas de PR e issues
├── docs/                          Arquitectura, Definition of Done y ADR
├── src/
│   ├── PatriSystem.Domain/        Entidades y reglas puras, sin paquetes
│   ├── PatriSystem.Application/   Casos de uso, interfaces y validadores
│   ├── PatriSystem.Infrastructure/ Base de datos, identidad, tokens y correo
│   └── PatriSystem.Api/           Controladores, middleware y Program.cs
├── tests/
│   ├── PatriSystem.Application.Tests/
│   └── PatriSystem.Api.IntegrationTests/
├── .env.example                   Plantilla de variables para Docker Compose
├── CHANGELOG.md
├── CONTRIBUTING.md
├── Directory.Build.props          Reglas comunes de compilación
├── Directory.Packages.props       Versiones centralizadas de paquetes
├── docker-compose.yml
├── global.json                    Versión fija del SDK
└── PatriSystem.slnx
```

Las dependencias apuntan siempre hacia adentro. El diagrama de capas está en [`docs/architecture.md`](docs/architecture.md).

## Documentación del proyecto

| Documento | Qué contiene |
|---|---|
| [`CONTRIBUTING.md`](CONTRIBUTING.md) | Flujo de trabajo, reglas de `main` y convenciones de código |
| [`docs/architecture.md`](docs/architecture.md) | Diagrama de capas y reglas que se comprueban automáticamente |
| [`docs/definition-of-done.md`](docs/definition-of-done.md) | Cuándo se considera terminado un ticket y un sprint |
| [`CHANGELOG.md`](CHANGELOG.md) | Registro de cambios |
| [ADR-001](docs/adr/0001-clean-architecture.md) | Clean Architecture y sus costos |
| [ADR-002](docs/adr/0002-dotnet-10.md) | Por qué .NET 10 |
| [ADR-003](docs/adr/0003-documentacion-api-openapi-scalar.md) | OpenAPI nativo y Scalar |
| [ADR-004](docs/adr/0004-fijacion-pkcs-por-vulnerabilidad.md) | Fijación de un paquete por vulnerabilidad |
| [Plantilla de ADR](docs/adr/0000-plantilla.md) | Formato para registrar decisiones |

## Licencia

[MIT](LICENSE)

## Verificación

El proyecto se levantó en un equipo limpio, siguiendo solo este README, el 22 de octubre de 2026. **Esa prueba corresponde al estado anterior a AUTH-01**, cuando todavía no había migraciones ni base `PatriSystem`.

| Qué | Resultado |
|---|---|
| Sistema operativo | Windows 11 Pro |
| SDK de .NET | 10.0.204 (versión más baja verificada) |
| Descarga inicial de imágenes | unos 53 min con la conexión de la prueba (SQL Server: 627 MB de descarga y 2,35 GB en disco; MailHog: 146 MB y 572 MB) |
| Resto del proceso, sin contar la descarga | entre 15 y 20 min (estimado, no cronometrado) |
| `/health/ready` | `Healthy` |
| `dotnet test` | 10 pruebas superadas |

**Sin verificar a mano:** Linux y macOS (el CI compila y ejecuta las pruebas en Linux, pero los pasos de Docker y el comando `cp` no se probaron) y versiones del SDK anteriores a 10.0.204.

**Pendiente:** repetir la verificación en un equipo limpio con el flujo actual, que incluye el paso 5 (migraciones). Con AUTH-01 solo se comprobó en la máquina de desarrollo: migración aplicada, dos arranques de la API con los roles creados una sola vez y 16 pruebas superadas.