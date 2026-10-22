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
- **Errores:** fuera de Development, una respuesta de error no contiene detalles internos. El detalle va al log, con el `X-Correlation-Id` de la petición.
- **Secretos:** solo en user-secrets o variables de entorno. Nunca en archivos del repositorio.
- **Paquetes:** las versiones viven en `Directory.Packages.props`, nunca en los `.csproj`. Una advertencia de vulnerabilidad de NuGet (`NU1902`, `NU1903`) no se silencia: se resuelve y se documenta, como en el [ADR-004](docs/adr/0004-fijacion-pkcs-por-vulnerabilidad.md).
- **Analizadores:** una advertencia no se silencia sin explicar por qué. Las excepciones actuales son CS1591 solo en la API ([ADR-003](docs/adr/0003-documentacion-api-openapi-scalar.md)) y CA1707 solo en `tests/`.

## Arquitectura

Las dependencias apuntan siempre hacia adentro. Ver [`docs/architecture.md`](docs/architecture.md) y el [ADR-001](docs/adr/0001-clean-architecture.md). Las pruebas de arquitectura fallan si se rompe esa regla.