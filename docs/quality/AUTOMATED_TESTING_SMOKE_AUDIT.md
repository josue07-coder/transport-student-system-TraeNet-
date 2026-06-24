# Fase QA-6: Pruebas automatizadas y smoke tests completos

## Objetivo

Fortalecer la verificación automatizada del backend de TransportStudentSystem sin modificar frontend, sin agregar funcionalidades y sin acceder directamente a SQL Server desde las pruebas.

La validación se divide en:

- Pruebas unitarias de dominio y aplicación.
- Pruebas de integración HTTP con `WebApplicationFactory` y base InMemory.
- Smoke tests HTTP contra la API local.

## Criterios aplicados

| Criterio | Aplicación |
|---|---|
| No tocar SQL Server desde tests automatizados | Las pruebas de API usan `WebApplicationFactory` con EF Core InMemory. |
| No modificar frontend | No se modificó ningún archivo frontend. |
| No cambiar endpoints | Solo se añadieron pruebas y se amplió el script smoke principal. |
| Reutilizar pruebas existentes | Se amplió `ApiIntegrationTests.cs` en lugar de crear un proyecto nuevo. |
| Reutilizar smoke tests existentes | Se mantuvieron los scripts existentes y se creó un runner consolidado. |

## Cobertura automatizada agregada

| Área | Cobertura |
|---|---|
| Auth y perfil | Login, `/api/me`, endpoints protegidos sin token. |
| Usuarios, roles y permisos | Lectura con Admin y bloqueo a Guardian. |
| Gestión académica | Lectura de sectores, escuelas, grados, tutores y estudiantes. |
| Transporte | Lectura de vehículos, conductores, asistentes, paradas, rutas, asignaciones y viajes. |
| Programación de viajes | Lectura de trip schedules y non-school-days. |
| Operación | Notificaciones, incidencias, tracking activo y endpoints operativos básicos. |
| Reportes | Dashboard y reportes con filtros de fecha. |
| Auditoría | Lectura de audit logs con Admin. |
| Configuración | Lectura de system settings. |
| Backups | Lectura de backups y latest backup. |
| Integraciones | Endpoint de cálculo de distancia mock. |
| Seguridad por rol | Guardian, Driver y Assistant bloqueados en módulos administrativos/catálogos. |
| Errores controlados | Endpoints por Id inexistente devuelven 4xx y no 500. |

## Archivos actualizados

- `tests/Transport.API.Tests/ApiIntegrationTests.cs`
- `scripts/smoke-test.ps1`

## Archivos creados

- `scripts/smoke-test-all.ps1`
- `docs/quality/AUTOMATED_TESTING_SMOKE_AUDIT.md`

## Smoke tests disponibles

El runner consolidado ejecuta los scripts existentes:

- `smoke-test.ps1`
- `smoke-test-write-endpoints.ps1`
- `smoke-test-security.ps1`
- `smoke-test-security-authorization.ps1`
- `smoke-test-users.ps1`
- `smoke-test-operational-seed.ps1`
- `smoke-test-assistant-permissions.ps1`
- `smoke-test-notifications.ps1`
- `smoke-test-incidents.ps1`
- `smoke-test-tracking.ps1`
- `smoke-test-reports.ps1`
- `smoke-test-system-settings.ps1`
- `smoke-test-integrations.ps1`
- `smoke-test-backups.ps1`
- `smoke-test-trip-schedules.ps1`
- `smoke-test-non-school-days.ps1`
- `smoke-test-trip-attendance.ps1`
- `smoke-test-trip-start-rules.ps1`
- `smoke-test-trip-route-deviations.ps1`
- `smoke-test-exceptional-passengers.ps1`
- `smoke-test-concurrency-integrity.ps1`
- `smoke-test-audit-traceability.ps1`
- `smoke-test-performance-pagination.ps1`

Comando:

`powershell -ExecutionPolicy Bypass -File scripts/smoke-test-all.ps1`

## Observaciones

- Las pruebas automatizadas no dependen de la base real local.
- Los smoke tests sí ejercitan la API local por HTTP. No consultan ni manipulan SQL Server directamente, pero la API puede leer/escribir datos según el endpoint probado.
- Si la API local falla en login por `Failed to generate SSPI context` o 500 asociado a SQL Server, el bloqueo es ambiental y no de la suite de pruebas InMemory.

## Resultado esperado

- `dotnet build TransportStudentSystem.sln` debe compilar sin errores.
- `dotnet test TransportStudentSystem.sln` debe ejecutar la suite completa.
- `scripts/smoke-test-all.ps1` debe entregar resumen por script cuando la API y SQL Server local estén operativos.

## Resultado de ejecución QA-6

| Validación | Resultado |
|---|---|
| `dotnet build TransportStudentSystem.sln` | Correcto, 0 errores, 0 warnings. |
| `dotnet test TransportStudentSystem.sln --no-build` | Correcto, 221/221 pruebas superadas. |
| `scripts/smoke-test-all.ps1` | Ejecutado contra API local. 23 scripts detectados, 0 faltantes. |
| Smoke tests HTTP | 3 scripts PASS, 20 scripts FAIL por bloqueo en `POST /api/auth/login` con 500. |

El fallo de smoke tests ocurre antes de probar la mayoría de módulos, durante autenticación contra la API local. Las respuestas incluyen error genérico 500 con `traceId` y `correlationId`, consistente con el bloqueo ambiental de SQL Server/SSPI ya documentado en fases anteriores. La suite automatizada InMemory no depende de SQL Server y quedó completamente verde.
