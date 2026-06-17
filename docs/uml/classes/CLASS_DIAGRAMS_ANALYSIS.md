# Análisis de Diagramas de Clases

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Apartado: **4.4.2 Diagrama de Clases**

Los diagramas de clases fueron elaborados a partir de las entidades reales ubicadas en `Transport.Domain/Entities`, considerando relaciones configuradas mediante Entity Framework Core cuando aportan claridad. Para mantener legibilidad académica, se dividieron por módulos y se incluyeron solo atributos relevantes.

## Diagramas creados

| Diagrama | Módulo | Clases incluidas | Propósito |
|---|---|---|---|
| `class-security.puml` | Seguridad y usuarios | `User`, `Role`, `Permission`, `RolePermission`, `AuditLog` | Presenta la estructura de autenticación, roles, permisos y auditoría. |
| `class-academic.puml` | Gestión académica | `Student`, `Guardian`, `School`, `Grade`, `Sector`, `SchoolDistrict` | Muestra la organización académica y sus relaciones principales. |
| `class-transport.puml` | Transporte | `Vehicle`, `Driver`, `TransportAssistant`, `Stop`, `Route`, `RouteStop` | Describe recursos de transporte, paradas y composición de rutas. |
| `class-assignments.puml` | Rutas y asignaciones | `RouteAssignment`, `StudentRouteAssignment`, `Route`, `Vehicle`, `Driver`, `TransportAssistant`, `Student`, `Stop` | Explica cómo se asignan recursos y estudiantes a rutas. |
| `class-trips.puml` | Viajes y programación | `Trip`, `TripSchedule`, `TripStudentAttendance`, `NonSchoolDay`, `RouteAssignment`, `Student`, `School` | Representa programación, materialización, operación de viajes y snapshot de pasajeros. |
| `class-tracking-incidents.puml` | Monitoreo e incidencias | `VehicleLocation`, `Incident`, `IncidentComment`, `Notification`, `Trip`, `Vehicle`, `User` | Integra ubicación GPS, incidencias, comentarios y notificaciones. |
| `class-configuration-backups.puml` | Configuración y respaldos | `SystemSetting`, `BackupRecord` | Documenta configuración persistente y registros de backup. |

## Relaciones principales

- `User` pertenece a un `Role`; `Role` se asocia con `Permission` mediante `RolePermission`.
- `Student` se relaciona con `Guardian`, `School` y `Grade`.
- `School` pertenece a un `Sector`; `Sector` puede pertenecer a un `SchoolDistrict`.
- `Route` pertenece a `School` y contiene varios `RouteStop`.
- `RouteStop` relaciona una `Route` con una `Stop`.
- `RouteAssignment` relaciona `Route`, `Vehicle`, `Driver` y opcionalmente `TransportAssistant`.
- `StudentRouteAssignment` es la relación entre `Student` y `RouteAssignment`.
- `Trip` pertenece a `RouteAssignment` y puede originarse desde `TripSchedule`.
- `Trip` contiene múltiples `TripStudentAttendance` como snapshot histórico de pasajeros.
- `VehicleLocation` registra ubicaciones asociadas a `Trip` y `Vehicle`.
- `Incident` puede relacionarse con `Trip` y usuarios responsables.
- `Notification` pertenece a un `User`.

## Observaciones de modelado

- Las entidades que heredan de `BaseEntity` comparten `Id`, `CreatedAt` y `UpdatedAt`.
- Las entidades que implementan `IActivatable` poseen estado lógico `IsActive`.
- `StudentRouteAssignment` no tiene relación directa con `Stop` en el dominio actual; la ruta y sus paradas se obtienen a través de `RouteAssignment` y `Route`.
- Se omitieron métodos internos de dominio cuando no eran necesarios para explicar la estructura del modelo.

## Orden recomendado para la tesis

1. Seguridad y usuarios.
2. Gestión académica.
3. Transporte.
4. Rutas y asignaciones.
5. Viajes y programación.
6. Monitoreo e incidencias.
7. Configuración y respaldos.

Nota. Elaboración propia.
