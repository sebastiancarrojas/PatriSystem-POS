# Arquitectura

PatriSystem sigue Clean Architecture: las dependencias apuntan siempre hacia adentro. La decisión y sus costos están en el [ADR-001](adr/0001-clean-architecture.md).

## Capas

La flecha significa "depende de".

```mermaid
flowchart TD
    Api["<b>PatriSystem.Api</b><br/>Controladores, middleware y Program.cs"]
    Infrastructure["<b>PatriSystem.Infrastructure</b><br/>Opciones de base de datos y chequeo de salud"]
    Application["<b>PatriSystem.Application</b><br/>Registro de servicios (casos de uso previstos)"]
    Domain["<b>PatriSystem.Domain</b><br/>Sin código todavía"]

    AppTests["PatriSystem.Application.Tests"]
    ApiTests["PatriSystem.Api.IntegrationTests"]

    Api --> Application
    Api --> Infrastructure
    Infrastructure --> Application
    Application --> Domain

    AppTests -.-> Application
    AppTests -.-> Domain
    ApiTests -.-> Api
```

Las flechas punteadas son los proyectos de pruebas.

## Qué hay en cada capa hoy y qué se prevé

| Capa | Hoy | Previsto |
|---|---|---|
| Domain | Nada | Entidades y reglas puras |
| Application | `DependencyInjection` | Casos de uso, interfaces y validadores |
| Infrastructure | `DatabaseOptions` y `DatabaseHealthCheck` | EF Core, Identity, JWT y correo |
| Api | Controlador de ejemplo (`ping`), middleware de correlación y de errores, documentación OpenAPI | Controladores de cada caso de uso |

## Reglas que se comprueban automáticamente

Las pruebas de `tests/PatriSystem.Application.Tests/Architecture/` fallan si se rompe alguna:

- Domain no referencia proyectos, paquetes ni frameworks.
- Application solo referencia Domain, y solo usa los paquetes de una lista explícita.
- Domain y Application no usan EF Core, ASP.NET, Identity ni JWT.
- Las capas internas no usan las externas.

## Decisiones asociadas

- **Domain no tiene clase `DependencyInjection`:** no registra nada y debe quedar sin paquetes. Es una excepción consciente al criterio "cada capa" del ticket FND-02.
- **Infrastructure se referencia desde Api** solo para registrar la inyección de dependencias.
- **Estructura:** el código de producción vive en `src/` y las pruebas en `tests/`.