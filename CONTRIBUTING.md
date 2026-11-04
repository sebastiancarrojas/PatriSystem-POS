# Cómo contribuir

Este documento describe cómo se trabaja en PatriSystem. Aunque el proyecto lo desarrolla una sola persona, se trabaja como en un equipo: ramas, PR, CI y documentación.

## Idioma

- **Documentación y mensajes para personas:** español.
- **Código** (clases, métodos, variables), **ramas y commits:** inglés.

## Flujo de trabajo

1. Cada ticket tiene un issue en el tablero. Se trabaja un ticket por rama y se abre un PR por ticket.
2. Máximo **2 tickets abiertos a la vez**.
3. La rama sale de `main` actualizado, con el prefijo según el tipo de trabajo: `feat/`, `fix/`, `docs/`, `chore/` o `test/`, seguido del código y el tema (por ejemplo `chore/fnd-08-ci`).
4. Los commits siguen [commits convencionales](https://www.conventionalcommits.org/es/v1.0.0/): `feat`, `fix`, `docs`, `chore` o `test`, y un mensaje corto en inglés (`chore: add CI workflow`).
5. El PR se abre desde la web de GitHub, que carga la plantilla con el checklist. Lleva `Closes #n` si la fusión termina el ticket, o `Refs #n` si no.
6. Se fusiona con **squash**, que deja un commit por ticket en `main`, y se borra la rama.

## Reglas de `main`

Estas reglas están configuradas en GitHub (Settings, Rules, `protect-main`) y **no se guardan en el repositorio**. Si se recrea el repo, hay que volver a configurarlas:

| Regla | Efecto |
|---|---|
| Pull request obligatorio | No se sube directo a `main`. Las aprobaciones requeridas son 0 porque el proyecto lo desarrolla una persona. |
| Check `Build and test` obligatorio | No se fusiona si el CI está en rojo. |
| Sin borrar la rama | `main` no se puede eliminar. |
| Sin push forzado | El historial no se puede reescribir. |

## Antes de abrir un PR

Ejecuta lo mismo que corre el CI:

```
dotnet format PatriSystem.slnx --verify-no-changes
dotnet build PatriSystem.slnx --configuration Release -warnaserror
dotnet test PatriSystem.slnx
```

El checklist completo está en [`docs/definition-of-done.md`](docs/definition-of-done.md).

## Documentación

Un PR que cambia comportamiento sin tocar la documentación no se fusiona. Eso incluye el README, y también un ADR cuando hay una decisión de diseño. Para escribir uno, copia [`docs/adr/0000-plantilla.md`](docs/adr/0000-plantilla.md).

## Convenciones de código

- **Pruebas:** nombre `Method_Scenario_ExpectedResult`. La regla CA1707 (sin guiones bajos) está desactivada solo en `tests/`.
- **Logs:** se escriben con `LoggerMessage` (la regla CA1848 lo exige). Nunca se registran contraseñas, tokens ni datos personales.
- **Errores:** las reglas están en [Errores y resultados](#errores-y-resultados). Fuera de Development, una respuesta de error no contiene detalles internos: el detalle va al log, con el `X-Correlation-Id` de la petición.
- **Secretos:** solo en user-secrets o variables de entorno. Nunca en archivos del repositorio.
- **Paquetes:** las versiones viven en `Directory.Packages.props`, nunca en los `.csproj`. Una advertencia de vulnerabilidad de NuGet (`NU1902`, `NU1903`) no se silencia: se resuelve y se documenta, como en el [ADR-004](docs/adr/0004-fijacion-pkcs-por-vulnerabilidad.md).
- **Analizadores:** una advertencia no se silencia sin explicar por qué. Las excepciones actuales son CS1591 solo en la API ([ADR-003](docs/adr/0003-documentacion-api-openapi-scalar.md)) y CA1707 solo en `tests/`.

## Errores y resultados

Hay dos tipos de fallo y cada uno sigue un camino distinto:

- **Errores de negocio esperados** (datos inválidos, algo que no existe, un conflicto, falta de permiso). No son excepciones: los casos de uso devuelven `Result<T>` con un `BusinessError`.
- **Excepciones inesperadas** (un bug, la base caída). Se dejan propagar. El `GlobalExceptionHandler` las registra y responde `500` genérico, sin `ex.Message` ni pila fuera de Development.

| Error (Application) | HTTP | Cuándo se usa |
|---|---|---|
| `ValidationError` | 400, con los campos en `errors` | Los datos de entrada no cumplen las reglas |
| `UnauthorizedError` | 401 | No hay identidad válida |
| `NotFoundError` | 404 | El recurso pedido no existe |
| `ConflictError` | 409 | La operación choca con el estado actual (por ejemplo, un duplicado) |

Reglas:

1. Application no conoce HTTP. El mapeo error → código vive en `Api/Errors/BusinessErrorMapper`, y los controladores lo usan con `result.Failure.ToActionResult()`.
2. Cada error lleva un `Code` estable (`user.not_found`) para quien consuma la API, y un `Description` legible. El código se escribe en inglés y se mantiene aunque cambie el texto.
3. La validación se escribe con FluentValidation en Application y se convierte con `ToValidationError()`.
4. Un error nuevo exige tres cambios en el mismo PR: el `record` en `Application/Results`, su caso en `BusinessErrorMapper` y su prueba en `Api.UnitTests`. Si falta el caso, el mapper lanza `NotSupportedException` y la API responde 500.
5. Leer `Value` de un `Result` fallido, o `Failure` de uno exitoso, lanza `InvalidOperationException`: es un error de programación. Se comprueba `IsSuccess` antes.
6. Los endpoints que lanzan errores a propósito para las pruebas viven en `tests/PatriSystem.Api.IntegrationTests/TestSupport`, nunca en `src/`.

## Arquitectura

Las dependencias apuntan siempre hacia adentro. Ver [`docs/architecture.md`](docs/architecture.md) y el [ADR-001](docs/adr/0001-clean-architecture.md). Las pruebas de arquitectura fallan si se rompe esa regla.