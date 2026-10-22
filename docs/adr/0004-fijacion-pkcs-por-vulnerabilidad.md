# ADR-004: Fijación de System.Security.Cryptography.Pkcs por vulnerabilidad

- Estado: Aceptada
- Fecha: 2026-10-20

## Contexto

`Microsoft.Data.SqlClient` 7.1.1, que usa el chequeo de salud de la base de datos, trae como dependencia indirecta `System.Security.Cryptography.Pkcs` 9.0.18. El 13 de octubre de 2026 se publicaron dos avisos de seguridad sobre ese paquete (GHSA-846f-f764-76mx, gravedad alta, y GHSA-2fmv-rv2c-59x7, gravedad moderada). La auditoría de NuGet los reporta como `NU1903` y `NU1902`, y como el proyecto trata las advertencias como errores, la restauración dejó de funcionar.

La versión corregida para la línea 10.x se tomó de una base de datos de vulnerabilidades de terceros (10.0.13), no de los avisos de Microsoft directamente.

## Decisión

Se activa la fijación transitiva de Central Package Management (`CentralPackageTransitivePinningEnabled`) y se declara `System.Security.Cryptography.Pkcs` en la versión **10.0.13** en `Directory.Packages.props`. Así, cualquier proyecto que reciba el paquete de forma indirecta obtiene la versión corregida.

No se silencian `NU1902` ni `NU1903`.

## Consecuencias

### A favor

- La restauración y el CI vuelven a pasar sin ocultar la vulnerabilidad.
- La fijación es global: cuando se agregue EF Core, que también traerá SqlClient, queda cubierto.

### En contra

- Es un parche: SqlClient pide una versión anterior y la solución fuerza una más nueva. La compatibilidad solo se comprobó con la compilación y con la prueba manual del chequeo de salud, no de forma exhaustiva.
- Hay que acordarse de quitarlo.

### Cuándo revisarlo

- Al empezar AUTH-01, cuando se agregue EF Core y se actualicen sus paquetes.
- Cada vez que se actualice `Microsoft.Data.SqlClient`: si la nueva versión ya trae un Pkcs corregido, se quita la fijación.

## Alternativas descartadas

- **Suprimir `NU1902` y `NU1903` con `NoWarn`:** esconde la vulnerabilidad en lugar de resolverla.
- **Fijar 9.0.21, la versión corregida de la línea 9.x:** otros proyectos con el mismo problema la usaron. Se prefirió la 10.0.13 por coincidir con la línea de .NET 10 que usa el resto de paquetes.
- **Esperar una versión nueva de SqlClient:** deja el build roto mientras tanto.