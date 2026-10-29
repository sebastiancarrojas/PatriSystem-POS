# Registro de cambios

Todos los cambios relevantes de este proyecto se documentan en este archivo.

El formato sigue [Keep a Changelog 1.1.0](https://keepachangelog.com/es-ES/1.1.0/). El proyecto adoptará [Versionado Semántico](https://semver.org/lang/es/) con la primera versión publicada, la 1.0.0, que corresponde al cierre del MVP.

## [Sin publicar]

### Añadido

- Repositorio público con licencia MIT, `.gitignore` y `.editorconfig`.
- Protección de `main`: PR obligatorio, check `Build and test` obligatorio, sin borrar la rama y sin push forzado.
- Solución `PatriSystem.slnx` con Clean Architecture: `Domain`, `Application`, `Infrastructure` y `Api` en `src/`, y dos proyectos de pruebas en `tests/`.
- Configuración central de la solución: `global.json` (SDK 10), `Directory.Build.props` (advertencias como errores) y `Directory.Packages.props` (versiones centralizadas).
- Entorno local con Docker Compose: SQL Server 2022 y MailHog.
- Configuración validada al arrancar (`ValidateOnStart`) y secretos con user-secrets.
- Logs con Serilog (consola en Development, JSON en el resto), identificador de correlación en la respuesta y en los logs, y manejador global de excepciones con `ProblemDetails` sin detalles internos fuera de Development.
- Endpoints de salud `/health/live` y `/health/ready`, este último con comprobación de SQL Server.
- Documentación OpenAPI con Scalar (solo en Development), esquema Bearer declarado y endpoint de ejemplo `GET /api/ping`.
- Entorno de pruebas con xUnit, NSubstitute y Shouldly, con una prueba unitaria y una de integración de ejemplo.
- Pruebas de arquitectura que hacen cumplir las reglas del ADR-001.
- Integración continua con GitHub Actions: formato, compilación en Release con advertencias como errores y pruebas con cobertura.
- Documentación base: README, CONTRIBUTING, plantillas de PR y de issues, Definition of Done, diagrama de arquitectura y ADR 0001 a 0004.
- Tablero del proyecto con backlog, milestones por sprint y etiquetas.
- Identidad y persistencia (AUTH-01): ASP.NET Core Identity con EF Core sobre SQL Server y claves `Guid`. `ApplicationUser`, `ApplicationRole`, `RefreshToken` y `ApplicationDbContext` viven en Infrastructure; Domain y Application no cambian.
- Migración `InitialIdentity`: tablas de Identity y `RefreshTokens`, que guarda solo el hash del token (`char(64)` con índice único), nunca el valor en claro.
- Política de contraseña (mínimo 10 caracteres, mayúscula, minúscula y número) y de bloqueo (5 intentos fallidos, 15 minutos), con pruebas de integración.
- Los roles `Admin` y `Cajero` se crean al arrancar la API, de forma idempotente. La opción `Identity:SeedOnStartup` permite apagarlo.
- Herramienta local `dotnet-ef` (`dotnet-tools.json`) para las migraciones.
- [ADR-005](docs/adr/0005-identity-y-persistencia.md) y diagrama del modelo de datos en `docs/architecture.md`.

### Cambiado

- La API necesita SQL Server con la migración aplicada para arrancar, porque crea los roles al iniciar.
- La cadena de conexión local pasa de la base `master` a `PatriSystem`, y el README añade el paso de migraciones.

### Seguridad

- Se fijó `System.Security.Cryptography.Pkcs` en 10.0.13 por avisos de vulnerabilidad en una dependencia indirecta de `Microsoft.Data.SqlClient` ([ADR-004](docs/adr/0004-fijacion-pkcs-por-vulnerabilidad.md)).
- Se revisó la fijación de Pkcs al agregar EF Core: sigue siendo necesaria, porque sin ella NuGet resuelve la 9.0.18 con los avisos `NU1902` y `NU1903` ([ADR-004](docs/adr/0004-fijacion-pkcs-por-vulnerabilidad.md)).