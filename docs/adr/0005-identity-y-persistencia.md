# ADR-005: ASP.NET Core Identity con EF Core para cuentas y persistencia

- Estado: Aceptada
- Fecha: 2026-10-28

## Contexto

AUTH-01 necesita usuarios, roles y refresh tokens guardados en SQL Server. El proyecto anterior dejó dos lecciones: Domain dependía de Identity y JWT, y había credenciales escritas en un seeder. Las reglas vigentes son Clean Architecture con Domain sin paquetes (ADR-001), contraseña mínima de 10 caracteres con mayúscula, minúscula y número, y bloqueo tras 5 intentos fallidos durante 15 minutos.

## Decisión

- **Identity:** se usa ASP.NET Core Identity con EF Core (`Microsoft.AspNetCore.Identity.EntityFrameworkCore` 10.0.13) y `AddIdentityCore`, sin cookies ni páginas de login, porque la API usará JWT. Los endpoints de Identity no se usan: el flujo de registro con aprobación es propio.
- **Dónde vive:** `ApplicationUser`, `ApplicationRole` y `RefreshToken` están en `Infrastructure/Identity`, y `ApplicationDbContext` en `Infrastructure/Persistence`. `IdentityUser` es un tipo del paquete de Identity, así que no puede estar en Domain ni en Application. Los demás proyectos se enlazan por `Guid`.
- **Claves:** `Guid` en usuarios y roles.
- **Contraseña y bloqueo:** mínimo 10 caracteres, mayúscula, minúscula y número, sin exigir símbolo. Bloqueo a los 5 intentos fallidos por 15 minutos, también para usuarios nuevos. Hay pruebas de integración que lo comprueban.
- **Refresh token:** solo se guarda su hash en `RefreshTokens.TokenHash`, como `char(64)` con índice único. Ninguna columna contiene el token en claro. El hash será SHA-256 en hexadecimal y se calcula en AUTH-06, que emite los tokens; este ticket solo crea la columna. Elegir SHA-256 para tokens aleatorios largos es una práctica habitual, pero no se verificó contra una fuente en este ticket.
- **Migraciones:** manuales con `dotnet ef database update` (herramienta local `dotnet-ef` 10.0.13 en `dotnet-tools.json`). El comando crea la base `PatriSystem` si no existe, así que un clon limpio solo ejecuta ese paso del README. La API no migra al arrancar. Cuando exista despliegue se decidirá cómo migrar en producción.
- **Roles:** `Admin` y `Cajero` se crean al arrancar con un inicializador idempotente (`IdentitySeedHostedService`). El interruptor `Identity:SeedOnStartup` (por defecto `true`) permite apagarlo; las pruebas de integración lo apagan. Los roles no son secretos. El primer Admin se crea en AUTH-04, desde configuración.
- **Pkcs:** se mantiene la fijación de `System.Security.Cryptography.Pkcs` 10.0.13 del ADR-004. Sin ella, NuGet resuelve la 9.0.18 con los avisos NU1902 y NU1903.

## Consecuencias

### A favor

- Hash de contraseñas, bloqueo y tokens de confirmación vienen resueltos y mantenidos por Microsoft, en lugar de implementarlos a mano.
- Domain y Application siguen sin referenciar EF Core ni Identity; las pruebas de arquitectura siguen en verde.
- Si la base se filtra, los hashes de refresh token no sirven para iniciar sesión.

### En contra

- La API necesita SQL Server para arrancar: si la base no está disponible o falta aplicar la migración, falla al iniciar. Se prefirió fallar rápido y claro antes que arrancar sin roles.
- El usuario del negocio y `ApplicationUser` son clases distintas si algún día se necesita una entidad de usuario en Domain; habrá que mapearlas.
- Las tablas llevan los nombres por defecto de Identity (`AspNetUsers`, `AspNetRoles`, etc.).
- La unicidad del correo (`RequireUniqueEmail`) aún no está activada; entra en AUTH-03.
- Cambiar el tipo de clave después de tener datos sería costoso.

## Alternativas descartadas

- **Sistema de usuarios propio:** obliga a implementar y mantener el hash de contraseñas, el bloqueo y los tokens, justo lo que más conviene no reinventar.
- **`IdentityUser` con clave `string`:** es el valor por defecto, pero un `Guid` es más compacto y coherente con el resto de claves del proyecto.
- **Entidades de usuario en Domain heredando de `IdentityUser`:** repite el error del proyecto anterior (Domain dependiendo de Identity).
- **Migrar al arrancar la API:** no se usa en producción por riesgo (varias instancias, permisos de la base), así que se mantiene un paso explícito en todos los entornos.
- **Crear los roles con `HasData` en la migración:** funciona, pero separa los roles del arranque y AUTH-04 reutiliza el mismo inicializador para el primer Admin.