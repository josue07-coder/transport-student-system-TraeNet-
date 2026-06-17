# Resumen de Diccionarios de Base de Datos

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Sistema: **TransportStudentSystem**  
Motor de base de datos: **SQL Server**  
ORM: **Entity Framework Core**

Este resumen lista las tablas reales identificadas desde `AppDbContext`, entidades de dominio y configuraciones EF Core.

> Nota: no se consultó la base de datos en ejecución. La documentación se generó desde el modelo de código y configuraciones EF Core para evitar dependencia del entorno SQL Server.

| Módulo | Tabla | Descripción | Diccionario lógico | Diccionario físico |
|---|---|---|---|---|
| Seguridad y usuarios | Users | Usuarios autenticados del sistema. | Sí | Sí |
| Seguridad y usuarios | Roles | Roles base del sistema. | Sí | Sí |
| Seguridad y usuarios | Permissions | Permisos funcionales por módulo. | Sí | Sí |
| Seguridad y usuarios | RolePermissions | Relación entre roles y permisos. | Sí | Sí |
| Gestión académica | Students | Estudiantes transportados. | Sí | Sí |
| Gestión académica | Guardians | Tutores o responsables de estudiantes. | Sí | Sí |
| Gestión académica | Schools | Centros educativos. | Sí | Sí |
| Gestión académica | Grades | Grados escolares. | Sí | Sí |
| Gestión académica | Sectors | Sectores geográficos. | Sí | Sí |
| Gestión académica | SchoolDistricts | Distritos educativos. | Sí | Sí |
| Gestión de transporte | Vehicles | Vehículos usados para transporte escolar. | Sí | Sí |
| Gestión de transporte | Drivers | Conductores del sistema. | Sí | Sí |
| Gestión de transporte | TransportAssistants | Asistentes de transporte. | Sí | Sí |
| Gestión de transporte | Stops | Paradas geográficas. | Sí | Sí |
| Rutas y asignaciones | Routes | Rutas escolares. | Sí | Sí |
| Rutas y asignaciones | RouteStops | Paradas asociadas a una ruta. | Sí | Sí |
| Rutas y asignaciones | RouteAssignments | Asignación de ruta, vehículo, conductor y asistente. | Sí | Sí |
| Rutas y asignaciones | StudentRouteAssignments | Estudiantes asignados a una asignación de ruta. | Sí | Sí |
| Viajes y programación | Trips | Viajes operativos o programados. | Sí | Sí |
| Viajes y programación | TripSchedules | Horarios programados de viaje. | Sí | Sí |
| Viajes y programación | TripStudentAttendances | Snapshot histórico de pasajeros por viaje. | Sí | Sí |
| Viajes y programación | NonSchoolDays | Días sin clase o sin operación. | Sí | Sí |
| Monitoreo GPS | VehicleLocations | Ubicaciones GPS registradas durante viajes. | Sí | Sí |
| Incidencias | Incidents | Incidencias operativas reportadas. | Sí | Sí |
| Incidencias | IncidentComments | Comentarios de seguimiento de incidencias. | Sí | Sí |
| Notificaciones | Notifications | Notificaciones internas de usuarios. | Sí | Sí |
| Reportes/Auditoría | AuditLogs | Registro de acciones auditadas. | Sí | Sí |
| Configuración | SystemSettings | Parámetros configurables del sistema. | Sí | Sí |
| Backups | BackupRecords | Historial de respaldos generados. | Sí | Sí |

## Observaciones sobre campos inferidos

- `CreatedAt` y `UpdatedAt` provienen de `BaseEntity` en las entidades que heredan de dicha clase.
- Cuando EF Core no define explícitamente `HasColumnType`, se documenta el tipo SQL como estimado por convención: `uniqueidentifier`, `datetime2`, `int`, `bit` o `nvarchar(max)`.
- Algunos enums se guardan como texto por configuración `.HasConversion<string>()`; otros se documentan como `int estimado` cuando no tienen conversión explícita.
- Los ValueObjects configurados con `OwnsOne` se documentan como columnas físicas reales, por ejemplo `Phone`, `Email`, `Street`, `City`, `Latitude`, `Longitude`, `StudentCode` y `LicenseNumber`.
