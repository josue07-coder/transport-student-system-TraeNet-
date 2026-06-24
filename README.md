# TransportStudentSystem - Plataforma Web TRAE

Backend de la **Plataforma Web para el Sistema de Transporte Escolar TRAE**, desarrollado en ASP.NET Core bajo una arquitectura limpia orientada a dominio. El sistema centraliza la gestion academica, operativa y administrativa del transporte escolar: usuarios, roles, estudiantes, tutores, rutas, asignaciones, viajes, asistencia, ubicacion GPS, incidencias, notificaciones, reportes, auditoria, configuracion, integraciones y respaldos.

## Tabla de Contenido

- [Descripcion General](#descripcion-general)
- [Stack Tecnologico](#stack-tecnologico)
- [Arquitectura](#arquitectura)
- [Modulos Implementados](#modulos-implementados)
- [Requisitos Previos](#requisitos-previos)
- [Configuracion Local](#configuracion-local)
- [Base de Datos y Migraciones](#base-de-datos-y-migraciones)
- [Ejecucion del Proyecto](#ejecucion-del-proyecto)
- [Versionamiento de API](#versionamiento-de-api)
- [Swagger y Autenticacion](#swagger-y-autenticacion)
- [Pruebas Automatizadas](#pruebas-automatizadas)
- [Smoke Tests](#smoke-tests)
- [Usuarios Semilla](#usuarios-semilla)
- [Documentacion del Proyecto](#documentacion-del-proyecto)
- [Buenas Practicas y Seguridad](#buenas-practicas-y-seguridad)
- [Solucion de Problemas](#solucion-de-problemas)

## Descripcion General

TransportStudentSystem es el backend principal de TRAE. Expone una API REST versionada para que un frontend web pueda administrar y monitorear la operacion diaria del transporte escolar.

El sistema permite:

- Administrar usuarios, roles y permisos.
- Registrar estudiantes, tutores, centros educativos, grados y sectores.
- Gestionar vehiculos, conductores, asistentes de transporte, paradas y rutas.
- Crear asignaciones de ruta con conductor, vehiculo, asistente y estudiantes.
- Programar viajes por ruta y materializarlos por fecha.
- Controlar dias no escolares o sin operacion.
- Iniciar, finalizar, cancelar o marcar viajes como no operativos.
- Registrar asistencia por viaje, pasajeros esperados y pasajeros excepcionales.
- Registrar desvio de ruta, incidencias, notificaciones y tracking GPS.
- Consultar reportes operativos, auditoria y respaldos logicos.

## Stack Tecnologico

### Backend

- **.NET 8**
- **ASP.NET Core 8**
- **ASP.NET Core Web API**
- **JWT Bearer Authentication**
- **Swagger / OpenAPI** con Swashbuckle

### Arquitectura y Patrones

- **Clean Architecture**
- **Domain-Driven Design (DDD)**
- **CQRS**
- **MediatR**
- **Repository Pattern**
- **DTOs manuales**
- **FluentValidation**
- **Middleware centralizado de excepciones**
- **Auditoria y trazabilidad**

### Persistencia

- **Entity Framework Core 8**
- **SQL Server**
- **EF Core Migrations**
- Configuraciones explicitas por entidad
- Restricciones, indices y relaciones configuradas desde Infrastructure

### Seguridad

- JWT Bearer
- Autorizacion por roles
- Roles principales:
  - `Admin`
  - `Supervisor`
  - `Driver`
  - `TransportAssistant`
  - `Guardian`
- Password hashing seguro
- Validacion de ownership/visibilidad en operaciones sensibles

### Pruebas

- **xUnit**
- **FluentAssertions**
- **Moq**
- **Microsoft.AspNetCore.Mvc.Testing**
- **EF Core InMemory** para pruebas de integracion/API
- Smoke tests con PowerShell

### Paquetes Principales

- `MediatR`
- `FluentValidation`
- `FluentValidation.DependencyInjectionExtensions`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `System.IdentityModel.Tokens.Jwt`
- `Swashbuckle.AspNetCore`

## Arquitectura

El repositorio sigue una separacion por capas:

```text
Transport.Domain/          Entidades, value objects, enums y excepciones de dominio
Transport.Application/     CQRS, commands, queries, DTOs, validators e interfaces
Transport.Infrastructure/  EF Core, DbContext, configuraciones, repositorios y servicios
Transport.API/             Controllers, middleware, DI, Swagger, JWT y servicios API
Transport.Shared/          Componentes compartidos
tests/                     Pruebas automatizadas
scripts/                   Smoke tests y validaciones funcionales
docs/                      Documentacion tecnica, UML, QA y base de datos
```

### Responsabilidades por Capa

| Capa | Responsabilidad |
|---|---|
| Domain | Reglas de negocio, entidades, value objects, enums y excepciones. |
| Application | Casos de uso CQRS, DTOs, validaciones e interfaces de repositorios/servicios. |
| Infrastructure | Persistencia EF Core, repositorios, servicios externos mock/locales y seeders. |
| API | Controllers, autenticacion, autorizacion, Swagger, middlewares y configuracion HTTP. |

### Reglas Arquitectonicas

- No usar AutoMapper.
- No exponer entidades de dominio desde los controllers.
- Usar DTOs manuales para entrada/salida.
- Usar MediatR para commands y queries.
- Usar FluentValidation mediante pipeline.
- Definir interfaces en Application e implementarlas en Infrastructure.
- Mantener configuraciones EF Core explicitas.
- Evitar columnas FK fantasma como `RoleId1`, `GuardianId1`, `RouteAssignmentId1`, etc.

## Modulos Implementados

### Seguridad y Usuarios

- Auth/Login
- Forgot password simple
- Perfil `/me`
- Cambio de contrasena
- Upload real de foto de perfil
- Usuarios administrativos manuales
- Roles
- Permisos
- Visibilidad por rol

### Gestion Academica

- Sectores
- Centros educativos
- Grados
- Tutores
- Estudiantes
- Historial de asistencia por estudiante

### Transporte

- Vehiculos
- Conductores
- Asistentes de transporte
- Paradas
- Rutas
- Paradas por ruta
- Asignaciones de ruta
- Estudiantes asignados a ruta

### Operacion de Viajes

- Estados de viaje:
  - `Scheduled`
  - `InProgress`
  - `Completed`
  - `Cancelled`
  - `NotOperating`
- Programacion de viajes (`TripSchedule`)
- Materializacion de viajes
- Calendario de dias no escolares (`NonSchoolDay`)
- Control de horarios y atraso
- Puntualidad del viaje
- Inicio anticipado justificado
- Finalizacion/cancelacion/no operacion

### Asistencia y Seguridad del Estudiante

- Snapshot de pasajeros por viaje
- Pasajeros esperados
- Pasajeros excepcionales
- Pase de lista por viaje
- Marcar estudiante como abordado, ausente o descendido
- Cierre de asistencia
- Notificaciones al tutor
- Historial de asistencia

### Monitoreo y Operacion

- Tracking GPS por viaje
- Ubicacion actual
- Historial de ubicaciones
- Desvios de ruta con justificacion
- Incidencias operativas
- Comentarios de incidencias
- Notificaciones internas

### Administracion y Analitica

- Reportes operativos
- Dashboard
- Auditoria
- Configuracion del sistema
- Integraciones mock/locales:
  - Email
  - SMS
  - WhatsApp
  - Push
  - Maps
  - File storage
- Backups logicos

## Requisitos Previos

Para ejecutar el proyecto localmente se necesita:

- **Windows 10/11** o ambiente compatible con .NET.
- **.NET SDK 8** instalado.
- **SQL Server** o **SQL Server Express**.
- **Visual Studio 2022**, **Rider** o **VS Code**.
- **PowerShell** para smoke tests.
- Herramienta EF Core instalada si se trabajara con migraciones:

```powershell
dotnet tool install --global dotnet-ef
```

Si ya esta instalada:

```powershell
dotnet tool update --global dotnet-ef
```

## Configuracion Local

Los archivos principales de configuracion estan en:

```text
Transport.API/appsettings.json
Transport.API/appsettings.Development.json
Transport.API/Properties/launchSettings.json
```

### Connection String

El proyecto usa la clave:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVIDOR;Database=TransportDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

Para desarrollo local se puede usar:

- Autenticacion integrada de Windows:

```text
Server=localhost\\SQLEXPRESS;Database=TransportDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;
```

- SQL Authentication:

```text
Server=localhost\\SQLEXPRESS;Database=TransportDB;User Id=usuario;Password=contrasena;TrustServerCertificate=True;Encrypt=False;
```

No se recomienda versionar credenciales reales.

### JWT

Configuracion requerida:

```json
{
  "Jwt": {
    "Key": "clave-segura-de-longitud-suficiente",
    "Issuer": "TransportStudentSystem",
    "Audience": "TransportStudentSystem",
    "ExpiresInMinutes": "60"
  }
}
```

La misma configuracion debe ser usada para firmar y validar tokens.

### Archivos Estaticos y Uploads

El upload de foto de perfil guarda archivos localmente en:

```text
wwwroot/uploads/profile-photos
```

El endpoint acepta:

- `image/jpeg`
- `image/png`
- `image/webp`

Tamano maximo:

```text
5 MB
```

## Base de Datos y Migraciones

Crear o actualizar la base de datos:

```powershell
dotnet ef database update --project Transport.Infrastructure --startup-project Transport.API
```

Crear una migracion nueva:

```powershell
dotnet ef migrations add NombreMigracion --project Transport.Infrastructure --startup-project Transport.API
```

Importante:

- No ejecutar migraciones en produccion sin revision.
- Revisar que no se generen FKs fantasma.
- Revisar `AppDbContextModelSnapshot.cs` despues de cambios grandes.

## Ejecucion del Proyecto

Restaurar paquetes:

```powershell
dotnet restore TransportStudentSystem.sln
```

Compilar:

```powershell
dotnet build TransportStudentSystem.sln
```

Ejecutar API por HTTP:

```powershell
dotnet run --project Transport.API --launch-profile http
```

URL habitual:

```text
http://localhost:5075
```

Ejecutar por HTTPS:

```powershell
dotnet run --project Transport.API --launch-profile https
```

URLs habituales:

```text
https://localhost:7295
http://localhost:5075
```

## Versionamiento de API

Todas las rutas vigentes usan:

```text
/api/v1
```

Ejemplos:

```text
POST /api/v1/auth/login
GET  /api/v1/me
GET  /api/v1/students
GET  /api/v1/trips
POST /api/v1/me/photo/upload
```

Las rutas antiguas sin version (`/api/...`) no estan disponibles.

## Swagger y Autenticacion

Swagger UI:

```text
http://localhost:5075/swagger
```

Documento OpenAPI:

```text
http://localhost:5075/swagger/v1/swagger.json
```

Para probar endpoints protegidos:

1. Ejecutar `POST /api/v1/auth/login`.
2. Copiar el token JWT.
3. Presionar `Authorize` en Swagger.
4. Ingresar:

```text
Bearer {token}
```

## Pruebas Automatizadas

Ejecutar todas las pruebas:

```powershell
dotnet test TransportStudentSystem.sln
```

Proyectos de pruebas:

```text
tests/Transport.Domain.Tests
tests/Transport.Application.Tests
tests/Transport.API.Tests
```

Cobertura principal:

- Reglas de dominio.
- Handlers de Application.
- Autorizacion.
- Rutas API.
- Reglas de viaje, asistencia, tracking, auditoria y seguridad.

## Smoke Tests

Los smoke tests estan en:

```text
scripts/
```

Ejemplos:

```powershell
./scripts/smoke-test.ps1
./scripts/smoke-test-write-endpoints.ps1
./scripts/smoke-test-security-authorization.ps1
./scripts/smoke-test-operational-seed.ps1
./scripts/smoke-test-attendance-workflow.ps1
./scripts/smoke-test-trip-route-deviations.ps1
```

Algunos smoke tests requieren:

- API ejecutandose localmente.
- SQL Server disponible.
- Migraciones aplicadas.
- Datos semilla cargados.

## Usuarios Semilla

En ambiente `Development`, el sistema incluye seeders para datos base y operativos.

Usuarios esperados para pruebas:

| Rol | Username |
|---|---|
| Admin | `admin` |
| Supervisor | `supervisor` |
| Driver | `driver01` |
| TransportAssistant | `assistant01` |
| Guardian | `guardian01` |

Las contrasenas son de desarrollo. Deben cambiarse o deshabilitarse en ambientes productivos.

## Documentacion del Proyecto

Documentacion principal:

| Documento | Ruta |
|---|---|
| Documentacion API | `docs/API_DOCUMENTATION.md` |
| Requerimientos frontend | `docs/FRONTEND_REQUIREMENTS.md` |
| Troubleshooting SQL Server SSPI | `docs/SQLSERVER_SSPI_TROUBLESHOOTING.md` |
| Revision logica de rutas/viajes | `docs/TRIP_ASSIGNMENT_LOGIC_REVIEW.md` |
| Rediseno operativo de viajes | `docs/TRIP_OPERATION_REDESIGN_PROPOSAL.md` |
| Auditorias QA | `docs/quality/` |
| Seguridad backend | `docs/security/` |
| Diccionarios de base de datos | `docs/database/` |
| Diagramas UML | `docs/uml/` |

## Buenas Practicas y Seguridad

- No usar credenciales semilla en produccion.
- No guardar contrasenas en texto plano.
- No exponer entidades de dominio en respuestas HTTP.
- No usar AutoMapper.
- Mantener DTOs manuales.
- Proteger endpoints administrativos con roles.
- Validar ownership en endpoints operativos.
- No confiar solo en ocultar rutas desde el frontend.
- Mantener Swagger y smoke tests alineados con `/api/v1`.
- No ejecutar `database update` en produccion sin revision previa.
- No versionar archivos `bin`, `obj`, `.vs`, logs temporales o secretos.

## Solucion de Problemas

### Error: `Failed to generate SSPI context`

Ocurre por problemas de autenticacion integrada con SQL Server. Revisar:

```text
docs/SQLSERVER_SSPI_TROUBLESHOOTING.md
```

Opciones:

- Corregir autenticacion integrada.
- Usar SQL Authentication en desarrollo.
- Validar servicio SQL Server y SPN si aplica.

### Build bloqueado por `Transport.API.exe`

Si aparece un error indicando que `Transport.API.exe` esta siendo usado por otro proceso:

```powershell
Get-Process Transport.API -ErrorAction SilentlyContinue
Stop-Process -Id {PID} -Force
dotnet build TransportStudentSystem.sln
```

### Token JWT invalido en Swagger

Verificar:

- Usar token nuevo.
- Revisar `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`.
- Confirmar que Swagger usa `Bearer {token}`.
- Cerrar sesion/limpiar token viejo en Swagger si persiste.

## Estado Actual

El backend se encuentra versionado bajo `/api/v1`, con Swagger JWT Bearer, modulos operativos completos, pruebas automatizadas, smoke tests y documentacion tecnica en `docs/`.
