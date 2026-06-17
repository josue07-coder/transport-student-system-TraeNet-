# Diccionario de Base de Datos Físico

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Base de datos: **SQL Server**  
Fuente: entidades de dominio, `AppDbContext` y configuraciones EF Core.

> Los tipos marcados como **estimado** son inferidos por convenciones de EF Core cuando la configuración no define explícitamente `HasColumnType`.

## Seguridad y usuarios

### Diccionario físico: Users

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del usuario. |
| Username | nvarchar | 100 | No | UK | Nombre de usuario. |
| Name | nvarchar | 150 | No | - | Nombre visible. |
| Email | nvarchar | 150 | No | UK | Correo del usuario. |
| PasswordHash | nvarchar | max estimado | No | - | Hash de contraseña. |
| ProfileImageUrl | nvarchar | 300 | Sí | - | Foto de perfil. |
| IsActive | bit | - | No | Índice global lógico | Estado activo. |
| RoleId | uniqueidentifier | - | No | FK | Relación con Roles. |
| GuardianId | uniqueidentifier | - | Sí | FK | Perfil tutor vinculado. |
| DriverId | uniqueidentifier | - | Sí | FK | Perfil conductor vinculado. |
| TransportAssistantId | uniqueidentifier | - | Sí | FK | Perfil asistente vinculado. |
| LastLoginAt | datetime2 estimado | - | Sí | - | Último acceso. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Users.RoleId` se relaciona con `Roles.Id`.
- `Users.GuardianId`, `DriverId` y `TransportAssistantId` son relaciones opcionales con perfiles operativos.

### Diccionario físico: Roles

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del rol. |
| Name | nvarchar | 100 | No | UK | Nombre del rol. |
| Description | nvarchar | 300 | Sí | - | Descripción del rol. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Roles` se relaciona con `Users` mediante `Users.RoleId`.
- `Roles` se relaciona con `Permissions` mediante `RolePermissions`.

### Diccionario físico: Permissions

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del permiso. |
| Name | nvarchar | 100 | No | UK | Nombre del permiso. |
| Description | nvarchar | 300 | Sí | - | Descripción. |
| Module | nvarchar | 100 | No | - | Módulo asociado. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Permissions` se relaciona con `Roles` mediante `RolePermissions`.

### Diccionario físico: RolePermissions

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| RoleId | uniqueidentifier | - | No | PK, FK | Rol relacionado. |
| PermissionId | uniqueidentifier | - | No | PK, FK | Permiso relacionado. |

Relaciones:
- Tabla puente entre `Roles` y `Permissions`.

## Gestión académica

### Diccionario físico: Students

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del estudiante. |
| StudentCode | nvarchar | max estimado | No | - | Código del estudiante. |
| FirstName | nvarchar | 250 | No | - | Nombre. |
| LastName | nvarchar | 300 | No | - | Apellido. |
| SchoolId | uniqueidentifier | - | No | FK | Escuela. |
| GradeId | uniqueidentifier | - | No | FK | Grado. |
| GuardianId | uniqueidentifier | - | No | FK | Tutor. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Students` se relaciona con `Schools`, `Grades`, `Guardians` y `StudentRouteAssignments`.

### Diccionario físico: Guardians

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del tutor. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento como texto. |
| DocumentNumber | nvarchar | 50 | No | UK | Número de documento. |
| FirstName | nvarchar | 100 | No | - | Nombre. |
| LastName | nvarchar | 100 | No | - | Apellido. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| Gender | nvarchar | max estimado | No | - | Género como texto. |
| SectorId | uniqueidentifier | - | Sí | FK | Sector. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Guardians` se relaciona con `Sectors`, `Students` y opcionalmente `Users`.

### Diccionario físico: Schools

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del centro. |
| Name | nvarchar | 150 | No | - | Nombre del centro. |
| DirectorName | nvarchar | 150 | No | - | Director. |
| SectorId | uniqueidentifier | - | No | FK | Sector. |
| Description | nvarchar | 500 | Sí | - | Descripción. |
| ProfileImageUrl | nvarchar | 300 | Sí | - | Imagen. |
| Email | nvarchar | 150 | No | - | Correo. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Schools` se relaciona con `Sectors`, `Grades`, `Students`, `Routes` y `NonSchoolDays`.

### Diccionario físico: Grades

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del grado. |
| Name | nvarchar | 100 | No | UK compuesto | Nombre del grado. |
| SchoolId | uniqueidentifier | - | No | FK, UK compuesto | Escuela. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Grades` se relaciona con `Schools` y `Students`.

### Diccionario físico: Sectors

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del sector. |
| Name | nvarchar | 150 | No | Índice | Nombre. |
| City | nvarchar | 100 | No | Índice | Ciudad. |
| Province | nvarchar | 100 | No | - | Provincia. |
| SchoolDistrictId | uniqueidentifier | - | Sí | FK | Distrito educativo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Sectors` se relaciona con `SchoolDistricts`, `Schools`, `Guardians` y `Stops`.

### Diccionario físico: SchoolDistricts

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del distrito. |
| Name | nvarchar | 150 | No | - | Nombre. |
| Code | nvarchar | 50 | No | UK | Código único. |
| Description | nvarchar | 500 | Sí | - | Descripción. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `SchoolDistricts` se relaciona con `Sectors`.

## Gestión de transporte

### Diccionario físico: Vehicles

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del vehículo. |
| PlateNumber | nvarchar | 20 | No | UK | Placa. |
| Capacity | int | - | No | - | Capacidad. |
| Status | nvarchar | max estimado | No | - | Estado como texto. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Vehicles` se relaciona con `RouteAssignments`, `Trips` indirectamente, `Incidents` y `VehicleLocations`.

### Diccionario físico: Drivers

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del conductor. |
| FirstName | nvarchar | 100 | No | - | Nombre. |
| LastName | nvarchar | 100 | No | - | Apellido. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento. |
| DocumentNumber | nvarchar | 50 | No | UK | Documento. |
| LicenseNumber | nvarchar | 50 | No | UK | Licencia. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Email | nvarchar | 150 | Sí | - | Correo. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Drivers` se relaciona con `RouteAssignments`, `Users` e `Incidents`.

### Diccionario físico: TransportAssistants

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del asistente. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento. |
| DocumentNumber | nvarchar | 50 | No | UK | Documento. |
| FirstName | nvarchar | 150 | No | - | Nombre. |
| LastName | nvarchar | 150 | No | - | Apellido. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| Email | nvarchar | 150 | Sí | - | Correo. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `TransportAssistants` se relaciona con `RouteAssignments`, `Users` e `Incidents`.

### Diccionario físico: Stops

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de parada. |
| Name | nvarchar | 150 | No | - | Nombre. |
| SectorId | uniqueidentifier | - | No | FK | Sector. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| Latitude | decimal estimado | - | No | - | Latitud. |
| Longitude | decimal estimado | - | No | - | Longitud. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Stops` se relaciona con `Sectors` y `RouteStops`.

## Rutas y asignaciones

### Diccionario físico: Routes

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de ruta. |
| Name | nvarchar | 150 | No | - | Nombre. |
| SchoolId | uniqueidentifier | - | No | FK | Escuela. |
| Status | nvarchar | max estimado | No | - | Estado como texto. |
| StartTime | time estimado | - | No | - | Hora inicio. |
| EndTime | time estimado | - | No | - | Hora fin. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Routes` se relaciona con `Schools`, `RouteStops` y `RouteAssignments`.

### Diccionario físico: RouteStops

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de parada en ruta. |
| RouteId | uniqueidentifier | - | No | FK, UK compuesto | Ruta. |
| StopId | uniqueidentifier | - | No | FK | Parada. |
| StopOrder | int | - | No | UK compuesto | Orden en ruta. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `RouteStops` se relaciona con `Routes` y `Stops`.

### Diccionario físico: RouteAssignments

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de asignación. |
| RouteId | uniqueidentifier | - | No | FK | Ruta. |
| VehicleId | uniqueidentifier | - | No | FK | Vehículo. |
| DriverId | uniqueidentifier | - | No | FK | Conductor. |
| TransportAssistantId | uniqueidentifier | - | Sí | FK | Asistente. |
| VehicleCapacity | int | - | No | - | Capacidad asignada. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `RouteAssignments` se relaciona con `Routes`, `Vehicles`, `Drivers`, `TransportAssistants`, `StudentRouteAssignments`, `Trips` y `TripSchedules`.

### Diccionario físico: StudentRouteAssignments

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| StudentId | uniqueidentifier | - | No | PK, FK | Estudiante. |
| RouteAssignmentId | uniqueidentifier | - | No | PK, FK | Asignación de ruta. |

Relaciones:
- Tabla puente entre `Students` y `RouteAssignments`.

## Viajes y programación

### Diccionario físico: Trips

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del viaje. |
| RouteAssignmentId | uniqueidentifier | - | No | FK | Asignación de ruta. |
| TripScheduleId | uniqueidentifier | - | Sí | FK, índice | Programación. |
| Direction | nvarchar | 30 | Sí | - | Dirección. |
| OperationDate | date | - | Sí | UK compuesto filtrado | Fecha de operación. |
| ScheduledDepartureTime | datetime2 estimado | - | Sí | - | Salida programada. |
| ScheduledArrivalTime | datetime2 estimado | - | Sí | - | Llegada programada. |
| StartTime | datetime2 estimado | - | Sí | - | Inicio real. |
| EndTime | datetime2 estimado | - | Sí | - | Fin real. |
| Status | nvarchar | max estimado | No | - | Estado como texto. |
| CancellationReason | nvarchar | 500 | Sí | - | Razón de cancelación. |
| NonOperationReason | nvarchar | 200 | Sí | - | Razón de no operación. |
| NonOperationNotes | nvarchar | 1000 | Sí | - | Notas de no operación. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Trips` se relaciona con `RouteAssignments`, `TripSchedules`, `TripStudentAttendances`, `VehicleLocations` e `Incidents`.

### Diccionario físico: TripSchedules

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de programación. |
| RouteAssignmentId | uniqueidentifier | - | No | FK, índice | Asignación de ruta. |
| Direction | nvarchar | 30 | No | Índice compuesto | Dirección. |
| DepartureTime | time | - | No | - | Hora salida. |
| ArrivalTime | time | - | Sí | - | Hora llegada. |
| IsActive | bit | - | No | - | Estado activo. |
| ValidFrom | date | - | No | Índice compuesto | Vigencia inicial. |
| ValidTo | date | - | Sí | - | Vigencia final. |
| Monday | bit | - | No | - | Lunes activo. |
| Tuesday | bit | - | No | - | Martes activo. |
| Wednesday | bit | - | No | - | Miércoles activo. |
| Thursday | bit | - | No | - | Jueves activo. |
| Friday | bit | - | No | - | Viernes activo. |
| Saturday | bit | - | No | - | Sábado activo. |
| Sunday | bit | - | No | - | Domingo activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `TripSchedules` se relaciona con `RouteAssignments` y `Trips`.

### Diccionario físico: TripStudentAttendances

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| TripId | uniqueidentifier | - | No | PK, FK, índice | Viaje. |
| StudentId | uniqueidentifier | - | No | PK, FK, índice | Estudiante. |
| StudentNameSnapshot | nvarchar | 250 | No | - | Nombre histórico. |
| StudentCodeSnapshot | nvarchar | 100 | No | - | Código histórico. |
| GuardianIdSnapshot | uniqueidentifier | - | Sí | Índice | Tutor histórico. |
| GuardianNameSnapshot | nvarchar | 250 | Sí | - | Nombre tutor histórico. |
| Status | nvarchar | 30 | No | - | Estado asistencia. |
| BoardedAt | datetime2 estimado | - | Sí | - | Fecha abordaje. |
| DroppedOffAt | datetime2 estimado | - | Sí | - | Fecha entrega. |
| Notes | nvarchar | 500 | Sí | - | Notas. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `TripStudentAttendances` se relaciona con `Trips` y `Students`.

### Diccionario físico: NonSchoolDays

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Date | date | - | No | Índice, UK filtrado | Fecha. |
| SchoolId | uniqueidentifier | - | Sí | FK, UK filtrado | Escuela afectada. |
| ReasonType | nvarchar | 50 | No | - | Tipo de razón. |
| Reason | nvarchar | 500 | No | - | Razón. |
| IsActive | bit | - | No | Índice | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `NonSchoolDays` se relaciona opcionalmente con `Schools`.

## Monitoreo GPS

### Diccionario físico: VehicleLocations

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| TripId | uniqueidentifier | - | No | FK, índice | Viaje. |
| VehicleId | uniqueidentifier | - | No | FK, índice | Vehículo. |
| Latitude | decimal | 9,6 | No | - | Latitud. |
| Longitude | decimal | 9,6 | No | - | Longitud. |
| Speed | decimal | 8,2 | Sí | - | Velocidad. |
| Heading | decimal | 6,2 | Sí | - | Rumbo. |
| RecordedAt | datetime2 estimado | - | No | Índice | Fecha de registro. |
| ReportedByUserId | uniqueidentifier | - | Sí | FK | Usuario que reporta. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `VehicleLocations` se relaciona con `Trips`, `Vehicles` y `Users`.

## Incidencias

### Diccionario físico: Incidents

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Title | nvarchar | 200 | No | - | Título. |
| Description | nvarchar | 2000 | No | - | Descripción. |
| Type | int estimado | - | No | - | Tipo de incidencia. |
| Severity | int estimado | - | No | Índice | Severidad. |
| Status | int estimado | - | No | Índice | Estado. |
| TripId | uniqueidentifier | - | Sí | FK, índice | Viaje. |
| RouteAssignmentId | uniqueidentifier | - | Sí | FK, índice | Asignación. |
| VehicleId | uniqueidentifier | - | Sí | FK | Vehículo. |
| DriverId | uniqueidentifier | - | Sí | FK | Conductor. |
| TransportAssistantId | uniqueidentifier | - | Sí | FK | Asistente. |
| ReportedByUserId | uniqueidentifier | - | No | FK, índice | Usuario reporta. |
| AssignedToUserId | uniqueidentifier | - | Sí | FK | Usuario asignado. |
| ResolvedByUserId | uniqueidentifier | - | Sí | FK | Usuario resolutor. |
| ResolvedAt | datetime2 estimado | - | Sí | - | Fecha resolución. |
| ClosedAt | datetime2 estimado | - | Sí | - | Fecha cierre. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Relaciones:
- `Incidents` se relaciona con viajes, asignaciones, recursos de transporte, usuarios y comentarios.

### Diccionario físico: IncidentComments

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| IncidentId | uniqueidentifier | - | No | FK, índice | Incidencia. |
| UserId | uniqueidentifier | - | No | FK, índice | Autor. |
| Comment | nvarchar | 1000 | No | - | Comentario. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha actualización. |

Relaciones:
- `IncidentComments` se relaciona con `Incidents` y `Users`.

## Notificaciones

### Diccionario físico: Notifications

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| UserId | uniqueidentifier | - | No | FK, índice | Usuario destinatario. |
| Title | nvarchar | 200 | No | - | Título. |
| Message | nvarchar | 1000 | No | - | Mensaje. |
| Type | int estimado | - | No | - | Tipo. |
| Priority | int estimado | - | No | - | Prioridad. |
| IsRead | bit | - | No | Índice | Estado lectura. |
| ReadAt | datetime2 estimado | - | Sí | - | Fecha lectura. |
| RelatedEntityType | nvarchar | 150 | Sí | Índice compuesto | Tipo entidad. |
| RelatedEntityId | nvarchar | 100 | Sí | Índice compuesto | Id entidad. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha actualización. |

Relaciones:
- `Notifications` se relaciona con `Users`.

## Reportes/Auditoría

### Diccionario físico: AuditLogs

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| UserId | uniqueidentifier | - | Sí | Índice | Usuario relacionado. |
| Username | nvarchar | 100 | Sí | - | Nombre de usuario. |
| Action | nvarchar | 50 | No | Índice | Acción. |
| EntityName | nvarchar | 150 | No | Índice compuesto | Entidad. |
| EntityId | nvarchar | 100 | Sí | Índice compuesto | Id de entidad. |
| OldValues | nvarchar | max | Sí | - | Valores anteriores. |
| NewValues | nvarchar | max | Sí | - | Valores nuevos. |
| IpAddress | nvarchar | 100 | Sí | - | IP. |
| UserAgent | nvarchar | 500 | Sí | - | Navegador/agente. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha actualización. |

Relaciones:
- No tiene FK explícita a `Users`; conserva `UserId` lógico para auditoría.

## Configuración

### Diccionario físico: SystemSettings

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Key | nvarchar | 150 | No | UK | Clave de configuración. |
| Value | nvarchar | 1000 | No | - | Valor. |
| Description | nvarchar | 500 | Sí | - | Descripción. |
| Category | nvarchar | 100 | No | - | Categoría. |
| DataType | nvarchar | 50 | No | - | Tipo de dato. |
| IsEditable | bit | - | No | - | Editable. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha actualización. |

Relaciones:
- Tabla independiente de configuración.

## Backups

### Diccionario físico: BackupRecords

| Campo físico | Tipo SQL Server | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| FileName | nvarchar | 260 | No | - | Nombre archivo. |
| FilePath | nvarchar | 1000 | No | - | Ruta archivo. |
| FileSizeBytes | bigint estimado | - | Sí | - | Tamaño. |
| Status | nvarchar | max estimado | No | Índice | Estado como texto. |
| Type | nvarchar | max estimado | No | Índice | Tipo como texto. |
| ErrorMessage | nvarchar | 1000 | Sí | - | Error. |
| CreatedByUserId | uniqueidentifier | - | No | - | Usuario creador. |
| CompletedAt | datetime2 estimado | - | Sí | - | Fecha finalización. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha actualización. |

Relaciones:
- No tiene FK explícita a `Users`; almacena el identificador del usuario creador.
