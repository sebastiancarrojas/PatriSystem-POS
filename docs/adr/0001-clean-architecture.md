# ADR-001: Clean Architecture

- Estado: Aceptada
- Fecha: 2026-10-15

## Contexto

PatriSystem es un punto de venta (POS) que se reconstruye desde cero. En el proyecto anterior la capa de dominio dependía de Identity y de JWT, lo que mezclaba las reglas del negocio con detalles técnicos: no se podían probar sin infraestructura y cambiar el login obligaba a tocar el dominio. Se necesita una estructura donde el compilador impida repetir ese error.

## Decisión

Se adopta Clean Architecture con cuatro capas y dependencias siempre hacia adentro:

- **Domain:** entidades y reglas puras, sin paquetes NuGet.
- **Application:** casos de uso, interfaces y validadores. Referencia solo a Domain.
- **Infrastructure:** EF Core, Identity, JWT y correo. Referencia a Application.
- **API:** controladores y `Program.cs`. Referencia a Application e Infrastructure, esta última solo para registrar la inyección de dependencias.

Las pruebas viven en `tests/` (`Application.Tests` y `Api.IntegrationTests`) y el código de producción en `src/`. La configuración común está en `global.json`, `Directory.Build.props` y `Directory.Packages.props`.

Cada capa expone un método de registro en una clase `DependencyInjection` (`AddApplication`, `AddInfrastructure`, `AddApi`). **Domain no tiene esa clase**: no registra nada y debe permanecer sin paquetes. Es una excepción consciente al criterio "cada capa tiene una clase DependencyInjection" del ticket FND-02.

## Consecuencias

### A favor

- Domain no puede depender de EF Core, Identity ni ASP.NET: el compilador lo impide.
- Las reglas de negocio se prueban sin base de datos ni servidor web.
- Se puede cambiar una pieza técnica (base de datos, proveedor de tokens) sin tocar el negocio.
- La estructura se puede verificar automáticamente con pruebas de arquitectura (FND-11).

### En contra

- Más proyectos y archivos para un sistema pequeño.
- Más código de conexión (interfaces, registro de servicios, mapeos entre capas).
- Mayor curva de aprendizaje y más decisiones sobre dónde va cada cosa.
- Si una capa o interfaz no aporta nada, se anota para la retrospectiva en lugar de crearla por costumbre.

## Alternativas descartadas

- **Un solo proyecto:** más simple, pero repite el problema del proyecto anterior: nada impide mezclar negocio y técnica.
- **Arquitectura en N capas tradicional (presentación, negocio, datos):** la capa de negocio termina dependiendo de la de datos, lo contrario de lo que se busca.