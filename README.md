# TransportStudentSystem - Plataforma Web TRAE

Backend para la Plataforma Web del Sistema de Transporte Escolar TRAE. El sistema administra usuarios, roles, estudiantes, tutores, centros educativos, rutas, asignaciones, viajes, asistencia, tracking GPS, incidencias, notificaciones, auditoria, reportes, configuracion, integraciones mock/locales y backups logicos.

## Tecnologia

- ASP.NET Core 8
- Clean Architecture
- DDD
- CQRS + MediatR
- Entity Framework Core
- SQL Server
- FluentValidation
- JWT Bearer Authentication
- xUnit, FluentAssertions y Moq para pruebas

## Estructura del repositorio

```text
Transport.Domain/          Entidades, value objects, enums y reglas de dominio
Transport.Application/     CQRS, DTOs, validators, interfaces y casos de uso
Transport.Infrastructure/  EF Core, DbContext, configuraciones, repositorios y servicios externos
Transport.API/             Controllers, middleware, DI, Swagger y servicios API
tests/                     Pruebas automatizadas de Domain, Application y API
scripts/                   Smoke tests PowerShell
docs/                      Documentacion tecnica, UML, seguridad, calidad y base de datos
```

## Arquitectura y reglas

- Los controllers no devuelven entidades de dominio.
- No se usa AutoMapper.
- Se usan DTOs manuales.
- La capa Application define interfaces de repositorios.
- Infrastructure implementa repositorios y EF Core.
- FluentValidation se ejecuta mediante pipeline existente.
- Las reglas criticas se mantienen en dominio o handlers segun responsabilidad.

## Versionamiento de API

La API usa versionamiento por URL. Todas las rutas vigentes usan el prefijo:

```text
/api/v1
```

Ejemplos:

```text
POST /api/v1/auth/login
GET  /api/v1/me
GET  /api/v1/trips
POST /api/v1/me/photo/upload
```

Las rutas antiguas sin version (`/api/...`) ya no estan disponibles.

## Swagger

Con la API en ejecucion:

```text
http://localhost:5075/swagger
http://localhost:5075/swagger/v1/swagger.json
```

Swagger soporta JWT Bearer mediante el boton `Authorize`.

## Autenticacion

Endpoint de login:

```http
POST /api/v1/auth/login
```

Body de ejemplo:

```json
{
  "username": "admin",
  "password": "Admin123"
}
```

Usar el token como:

```http
Authorization: Bearer {token}
```

## Modulos principales

- Auth y perfil de usuario
- Usuarios, roles y permisos
- Gestion academica: sectores, escuelas, grados, tutores y estudiantes
- Transporte: vehiculos, conductores, asistentes, paradas y rutas
- Asignaciones de rutas y estudiantes
- Programacion y materializacion de viajes
- Calendario escolar y dias sin operacion
- Operacion de viajes, asistencia y pasajeros excepcionales
- Desvios de ruta
- Tracking GPS
- Incidencias operativas
- Notificaciones internas
- Reportes y analitica
- Auditoria y trazabilidad
- Configuracion general del sistema
- Integraciones externas mock/locales
- Backups logicos

## Documentacion

- API: `docs/API_DOCUMENTATION.md`
- Requerimientos frontend: `docs/FRONTEND_REQUIREMENTS.md`
- Solucion de problemas SQL Server SSPI: `docs/SQLSERVER_SSPI_TROUBLESHOOTING.md`
- Revision de logica de viajes/asignaciones: `docs/TRIP_ASSIGNMENT_LOGIC_REVIEW.md`
- Propuesta de rediseno operativo de viajes: `docs/TRIP_OPERATION_REDESIGN_PROPOSAL.md`
- Auditorias QA: `docs/quality/`
- Auditoria de seguridad: `docs/security/`
- Diccionarios de base de datos: `docs/database/`
- Diagramas UML: `docs/uml/`

## Configuracion local

1. Revisar connection string en:

```text
Transport.API/appsettings.json
Transport.API/appsettings.Development.json
```

2. Si usa autenticacion integrada de SQL Server y aparece `Failed to generate SSPI context`, revisar:

```text
docs/SQLSERVER_SSPI_TROUBLESHOOTING.md
```

3. Aplicar migraciones cuando corresponda:

```powershell
dotnet ef database update --project Transport.Infrastructure --startup-project Transport.API
```

## Ejecucion

```powershell
dotnet run --project Transport.API --launch-profile http
```

URL local habitual:

```text
http://localhost:5075
```

## Build y pruebas

Compilar solucion:

```powershell
dotnet build TransportStudentSystem.sln
```

Ejecutar pruebas automatizadas:

```powershell
dotnet test TransportStudentSystem.sln
```

## Smoke tests

Los smoke tests estan en `scripts/`. Algunos dependen de que la API este levantada y de que SQL Server este disponible.

Ejemplos:

```powershell
./scripts/smoke-test.ps1
./scripts/smoke-test-write-endpoints.ps1
./scripts/smoke-test-security-authorization.ps1
./scripts/smoke-test-attendance-workflow.ps1
```

## Usuarios semilla de desarrollo

En ambiente `Development` el proyecto cuenta con seeding operativo para facilitar pruebas funcionales. Los usuarios esperados para pruebas incluyen:

- `admin`
- `supervisor`
- `driver01`
- `assistant01`
- `guardian01`

Las contrasenas iniciales son de desarrollo y deben cambiarse para ambientes reales.

## Notas de seguridad

- No usar credenciales semilla en produccion.
- No guardar contrasenas en texto plano.
- No exponer entidades de dominio desde API.
- Proteger los endpoints administrativos con roles.
- Mantener actualizados los smoke tests de autorizacion cuando cambien permisos.

## Estado actual

El backend tiene APIs versionadas bajo `/api/v1`, Swagger JWT Bearer, pruebas automatizadas y documentacion tecnica en `docs/`.
