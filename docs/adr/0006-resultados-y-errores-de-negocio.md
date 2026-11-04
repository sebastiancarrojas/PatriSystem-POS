# ADR-006: Result<T> y errores tipados para los errores de negocio

- Estado: Aceptada
- Fecha: 2026-11-03

## Contexto

AUTH-02 necesita una forma uniforme de representar los errores de negocio, para que la API no mezcle excepciones y respuestas HTTP inconsistentes. Application no puede depender de ASP.NET Core (ADR-001), así que no puede devolver `ProblemDetails` ni códigos HTTP. El `GlobalExceptionHandler` ya existía y responde 500 sin detalles internos fuera de Development.

## Decisión

- **Resultado:** los casos de uso devuelven `Result<T>` (Application). Es inmutable y está en éxito (`Value`) o en fallo (`Failure`). Leer el estado contrario lanza `InvalidOperationException`. La creación pasa por la clase estática `Result` (`Success` y `Failure`), para no declarar miembros estáticos en el tipo genérico.
- **Errores:** `BusinessError` es un `record` abstracto con `Code` y `Description`. Los tipos son `ValidationError` (con `Failures`: campo → mensajes), `NotFoundError`, `ConflictError` y `UnauthorizedError`.
- **Mapeo a HTTP:** vive en Api (`BusinessErrorMapper`): validación 400, no autorizado 401, no encontrado 404 y conflicto 409. El de validación es `ValidationProblemDetails`; todos incluyen `code` en `Extensions`. Un tipo no mapeado lanza `NotSupportedException` y termina en 500.
- **No autorizado:** se usa 401. Si se necesita distinguir "sin permiso" (403), se añadirá un tipo propio en el ticket que lo requiera.
- **Validación:** FluentValidation 12.2.0 en Application, convertida con `ToValidationError()`. Se añadió a `AllowedApplicationPackages`. No se usa `FluentValidation.AspNetCore` ni la validación automática de MVC: la validación es explícita en el caso de uso.
- **Excepciones inesperadas:** siguen el camino del `GlobalExceptionHandler` existente, que no cambió. Una prueba de integración comprueba el 500 genérico sin `ex.Message`.

## Consecuencias

### A favor

- Los errores esperados son parte del tipo de retorno, no excepciones ocultas, y el compilador obliga a decidir qué hacer con ellos.
- Application sigue sin conocer HTTP, y el mapeo se prueba sin levantar un servidor.
- Las claves de `ValidationError` coinciden con las de `ValidationProblemDetails`, así que la conversión es directa.

### En contra

- Cada caso de uso debe comprobar `IsSuccess`, y olvidarlo es posible. Leer `Value` de un fallo lanza en lugar de fallar al compilar.
- Un error nuevo exige tocar tres sitios (el `record`, el mapper y su prueba).
- Las claves de `errors` son los nombres de propiedad de C#. No se verificó si el JSON real las devuelve en camelCase; las pruebas comparan sin distinguir mayúsculas.
- Hay un solo `Result<T>`. Una operación sin valor de retorno necesitaría un `Result` sin tipo, que no se añadió porque ningún caso lo pide todavía.

## Alternativas descartadas

- **Excepciones de negocio con un filtro global:** más cómodas de escribir, pero ocultan los errores esperados y los mezclan con los bugs.
- **Librerías como ErrorOr o FluentResults:** añaden una dependencia que Application tendría que permitir, para un tipo que son unas pocas líneas.
- **`FluentValidation.AspNetCore`:** el autor lo descontinuó (no se verificó en este ticket) y acopla la validación a MVC.
- **Mapear a HTTP dentro de Application:** rompe la regla de dependencias del ADR-001.