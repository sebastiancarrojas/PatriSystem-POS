# ADR-003: Documentación de la API con OpenAPI nativo y Scalar

- Estado: Aceptada
- Fecha: 2026-10-18

## Contexto

La API necesita documentación interactiva para desarrollo, con comentarios XML y un esquema Bearer preparado para el JWT que llega en AUTH-05. Se evaluaron dos opciones con tres criterios: compatibilidad con .NET 10, soporte de comentarios XML y mantenimiento activo.

## Decisión

Se usa el generador de OpenAPI del propio framework (`Microsoft.AspNetCore.OpenApi`) y **Scalar** como interfaz (`Scalar.AspNetCore`).

- El documento OpenAPI se publica en `/openapi/v1.json` y la interfaz en `/scalar`, **solo en Development**.
- Los comentarios XML están activados únicamente en el proyecto de la API.
- El esquema `Bearer` (tipo `http`, formato JWT) se declara en un transformador de documento. Todavía no protege ningún endpoint.
- `GET /api/ping` sirve de endpoint de ejemplo y está disponible en todos los entornos.

## Comparación

| Criterio | OpenAPI nativo + Scalar | Swashbuckle |
|---|---|---|
| Compatibilidad con .NET 10 | Sí, es la solución oficial del framework | Sí, hay versión 10.x publicada |
| Comentarios XML | Sí desde .NET 10, mediante un generador de código | Sí |
| Mantenimiento | Lo mantiene el equipo de ASP.NET Core; Scalar es un proyecto aparte | Proyecto comunitario con releases frecuentes |

## Consecuencias

### A favor

- El generador es de primera mano y es el que la plantilla de .NET 9 y 10 adoptó en lugar de Swashbuckle.
- Generador e interfaz están separados: la interfaz se puede cambiar sin tocar el documento.
- El documento usa OpenAPI 3.1, el valor por defecto de .NET 10.

### En contra

- Scalar es una dependencia de terceros adicional.
- Los comentarios XML no se leen en endpoints escritos como lambdas, por lo que los endpoints se escriben como controladores.
- Swashbuckle sigue siendo común en proyectos existentes, así que en otros equipos habrá que adaptarse a él. El estándar (OpenAPI) es el mismo.

### Decisión asociada: advertencia CS1591

Con comentarios XML activados, el compilador exige comentar todo miembro público. El proyecto trata las advertencias como errores, así que se **suprime CS1591 solo en el proyecto de la API**. Los endpoints se comentan a medida que existen; las clases de infraestructura (middleware, registro de servicios) no se comentan por obligación.

## Alternativas descartadas

- **Swashbuckle:** válida y mantenida, pero reúne generador e interfaz en un solo paquete y no es la opción que adopta la plantilla actual.
- **Documentación en todos los entornos:** descartada para no exponer la superficie de la API fuera de desarrollo.