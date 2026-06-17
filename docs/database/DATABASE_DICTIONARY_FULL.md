# Diccionario de Base de Datos Completo

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**

Este documento integra el resumen, el diccionario lógico y el diccionario físico del sistema.

---

# Resumen de Diccionarios de Base de Datos

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Sistema: **TransportStudentSystem**  
Motor de base de datos: **SQL Server**  
ORM: **Entity Framework Core**

Este resumen lista las tablas reales identificadas desde `AppDbContext`, entidades de dominio y configuraciones EF Core.

> Nota: no se consultÃ³ la base de datos en ejecuciÃ³n. La documentaciÃ³n se generÃ³ desde el modelo de cÃ³digo y configuraciones EF Core para evitar dependencia del entorno SQL Server.

| MÃ³dulo | Tabla | DescripciÃ³n | Diccionario lÃ³gico | Diccionario fÃ­sico |
|---|---|---|---|---|
| Seguridad y usuarios | Users | Usuarios autenticados del sistema. | SÃ­ | SÃ­ |
| Seguridad y usuarios | Roles | Roles base del sistema. | SÃ­ | SÃ­ |
| Seguridad y usuarios | Permissions | Permisos funcionales por mÃ³dulo. | SÃ­ | SÃ­ |
| Seguridad y usuarios | RolePermissions | RelaciÃ³n entre roles y permisos. | SÃ­ | SÃ­ |
| GestiÃ³n acadÃ©mica | Students | Estudiantes transportados. | SÃ­ | SÃ­ |
| GestiÃ³n acadÃ©mica | Guardians | Tutores o responsables de estudiantes. | SÃ­ | SÃ­ |
| GestiÃ³n acadÃ©mica | Schools | Centros educativos. | SÃ­ | SÃ­ |
| GestiÃ³n acadÃ©mica | Grades | Grados escolares. | SÃ­ | SÃ­ |
| GestiÃ³n acadÃ©mica | Sectors | Sectores geogrÃ¡ficos. | SÃ­ | SÃ­ |
| GestiÃ³n acadÃ©mica | SchoolDistricts | Distritos educativos. | SÃ­ | SÃ­ |
| GestiÃ³n de transporte | Vehicles | VehÃ­culos usados para transporte escolar. | SÃ­ | SÃ­ |
| GestiÃ³n de transporte | Drivers | Conductores del sistema. | SÃ­ | SÃ­ |
| GestiÃ³n de transporte | TransportAssistants | Asistentes de transporte. | SÃ­ | SÃ­ |
| GestiÃ³n de transporte | Stops | Paradas geogrÃ¡ficas. | SÃ­ | SÃ­ |
| Rutas y asignaciones | Routes | Rutas escolares. | SÃ­ | SÃ­ |
| Rutas y asignaciones | RouteStops | Paradas asociadas a una ruta. | SÃ­ | SÃ­ |
| Rutas y asignaciones | RouteAssignments | AsignaciÃ³n de ruta, vehÃ­culo, conductor y asistente. | SÃ­ | SÃ­ |
| Rutas y asignaciones | StudentRouteAssignments | Estudiantes asignados a una asignaciÃ³n de ruta. | SÃ­ | SÃ­ |
| Viajes y programaciÃ³n | Trips | Viajes operativos o programados. | SÃ­ | SÃ­ |
| Viajes y programaciÃ³n | TripSchedules | Horarios programados de viaje. | SÃ­ | SÃ­ |
| Viajes y programaciÃ³n | TripStudentAttendances | Snapshot histÃ³rico de pasajeros por viaje. | SÃ­ | SÃ­ |
| Viajes y programaciÃ³n | NonSchoolDays | DÃ­as sin clase o sin operaciÃ³n. | SÃ­ | SÃ­ |
| Monitoreo GPS | VehicleLocations | Ubicaciones GPS registradas durante viajes. | SÃ­ | SÃ­ |
| Incidencias | Incidents | Incidencias operativas reportadas. | SÃ­ | SÃ­ |
| Incidencias | IncidentComments | Comentarios de seguimiento de incidencias. | SÃ­ | SÃ­ |
| Notificaciones | Notifications | Notificaciones internas de usuarios. | SÃ­ | SÃ­ |
| Reportes/AuditorÃ­a | AuditLogs | Registro de acciones auditadas. | SÃ­ | SÃ­ |
| ConfiguraciÃ³n | SystemSettings | ParÃ¡metros configurables del sistema. | SÃ­ | SÃ­ |
| Backups | BackupRecords | Historial de respaldos generados. | SÃ­ | SÃ­ |

## Observaciones sobre campos inferidos

- `CreatedAt` y `UpdatedAt` provienen de `BaseEntity` en las entidades que heredan de dicha clase.
- Cuando EF Core no define explÃ­citamente `HasColumnType`, se documenta el tipo SQL como estimado por convenciÃ³n: `uniqueidentifier`, `datetime2`, `int`, `bit` o `nvarchar(max)`.
- Algunos enums se guardan como texto por configuraciÃ³n `.HasConversion<string>()`; otros se documentan como `int estimado` cuando no tienen conversiÃ³n explÃ­cita.
- Los ValueObjects configurados con `OwnsOne` se documentan como columnas fÃ­sicas reales, por ejemplo `Phone`, `Email`, `Street`, `City`, `Latitude`, `Longitude`, `StudentCode` y `LicenseNumber`.

---

# Diccionario de Base de Datos LÃ³gico

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Sistema: **TransportStudentSystem**

El diccionario lÃ³gico describe las entidades de datos con nombres comprensibles para usuarios tÃ©cnicos y acadÃ©micos. Los nombres se presentan en espaÃ±ol, independientemente del nombre fÃ­sico usado por SQL Server.

## Seguridad y usuarios

### Diccionario lÃ³gico: Usuarios

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del usuario. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre de usuario | Nombre usado para iniciar sesiÃ³n. | Texto corto | SÃ­ | Ãšnico. |
| Nombre visible | Nombre mostrado en el sistema. | Texto | SÃ­ | Perfil del usuario. |
| Correo electrÃ³nico | Correo asociado al usuario. | Correo | SÃ­ | Ãšnico. |
| ContraseÃ±a cifrada | Hash seguro de la contraseÃ±a. | Texto protegido | SÃ­ | No debe exponerse. |
| Imagen de perfil | Ruta o URL de la foto del usuario. | Texto | No | Opcional. |
| Estado activo | Indica si puede acceder al sistema. | Booleano | SÃ­ | Control de acceso. |
| Rol | Rol asignado al usuario. | RelaciÃ³n | SÃ­ | FK hacia Roles. |
| Perfil vinculado | Tutor, conductor o asistente asociado. | RelaciÃ³n opcional | No | Solo aplica para usuarios operativos. |
| Ãšltimo acceso | Fecha del Ãºltimo login. | Fecha y hora | No | AuditorÃ­a de acceso. |

### Diccionario lÃ³gico: Roles

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del rol. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre del rol. | Texto corto | SÃ­ | Ãšnico. |
| DescripciÃ³n | Detalle funcional del rol. | Texto | SÃ­ | Puede quedar vacÃ­o desde dominio. |
| Permisos | Permisos asignados al rol. | RelaciÃ³n | No | RelaciÃ³n con RolePermissions. |

### Diccionario lÃ³gico: Permisos

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del permiso. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre tÃ©cnico del permiso. | Texto corto | SÃ­ | Ãšnico. |
| DescripciÃ³n | ExplicaciÃ³n del permiso. | Texto | No | Opcional. |
| MÃ³dulo | MÃ³dulo al que pertenece. | Texto corto | SÃ­ | Ej.: Users, Reports, Trips. |

### Diccionario lÃ³gico: Permisos por rol

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Rol | Rol al que se asigna el permiso. | RelaciÃ³n | SÃ­ | Parte de clave compuesta. |
| Permiso | Permiso asignado. | RelaciÃ³n | SÃ­ | Parte de clave compuesta. |

## GestiÃ³n acadÃ©mica

### Diccionario lÃ³gico: Estudiantes

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del estudiante. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| CÃ³digo de estudiante | CÃ³digo acadÃ©mico o interno. | Texto corto | SÃ­ | ValueObject persistido. |
| Nombre | Nombre del estudiante. | Texto | SÃ­ | Dato personal. |
| Apellido | Apellidos del estudiante. | Texto | SÃ­ | Dato personal. |
| Escuela | Centro educativo asociado. | RelaciÃ³n | SÃ­ | FK hacia Schools. |
| Grado | Grado escolar del estudiante. | RelaciÃ³n | SÃ­ | FK hacia Grades. |
| Tutor | Responsable del estudiante. | RelaciÃ³n | SÃ­ | FK hacia Guardians. |
| Foto | Ruta o URL de foto. | Texto | No | Opcional. |
| Estado activo | Indica si el estudiante participa en operaciÃ³n. | Booleano | SÃ­ | Tiene filtro global `IsActive`. |

### Diccionario lÃ³gico: Tutores

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del tutor. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Tipo de documento | Tipo de identificaciÃ³n. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |
| NÃºmero de documento | Documento del tutor. | Texto corto | SÃ­ | Ãšnico. |
| Nombre | Nombre del tutor. | Texto | SÃ­ | Dato personal. |
| Apellido | Apellido del tutor. | Texto | SÃ­ | Dato personal. |
| TelÃ©fono | TelÃ©fono de contacto. | Texto corto | SÃ­ | Requerido. |
| DirecciÃ³n | Calle y ciudad. | Texto | SÃ­ | ValueObject como columnas. |
| GÃ©nero | GÃ©nero registrado. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |
| Sector | Sector de residencia. | RelaciÃ³n opcional | No | FK hacia Sectors. |
| Estado activo | Indica si el tutor estÃ¡ activo. | Booleano | SÃ­ | Tiene filtro global. |

### Diccionario lÃ³gico: Centros educativos

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del centro. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre del centro educativo. | Texto | SÃ­ | Principal descriptor. |
| Director | Nombre del director. | Texto | SÃ­ | Dato institucional. |
| Correo | Correo institucional. | Correo | SÃ­ | ValueObject. |
| TelÃ©fono | TelÃ©fono institucional. | Texto corto | SÃ­ | ValueObject. |
| DirecciÃ³n | Calle y ciudad. | Texto | SÃ­ | ValueObject. |
| Sector | Sector al que pertenece. | RelaciÃ³n | SÃ­ | FK hacia Sectors. |
| DescripciÃ³n | InformaciÃ³n adicional. | Texto | No | Opcional. |
| Estado activo | Indica si el centro estÃ¡ activo. | Booleano | SÃ­ | Tiene filtro global. |

### Diccionario lÃ³gico: Grados

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del grado. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre del grado escolar. | Texto corto | SÃ­ | Ãšnico por escuela. |
| Escuela | Centro educativo al que pertenece. | RelaciÃ³n | SÃ­ | FK hacia Schools. |

### Diccionario lÃ³gico: Sectores

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del sector. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre del sector. | Texto | SÃ­ | UbicaciÃ³n geogrÃ¡fica. |
| Ciudad | Ciudad del sector. | Texto corto | SÃ­ | Requerida. |
| Provincia | Provincia del sector. | Texto corto | SÃ­ | Requerida. |
| Distrito educativo | Distrito asociado. | RelaciÃ³n opcional | No | FK hacia SchoolDistricts. |

### Diccionario lÃ³gico: Distritos educativos

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del distrito. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre del distrito educativo. | Texto | SÃ­ | Requerido. |
| CÃ³digo | CÃ³digo institucional. | Texto corto | SÃ­ | Ãšnico. |
| DirecciÃ³n | Calle y ciudad. | Texto | SÃ­ | ValueObject. |
| DescripciÃ³n | Detalle adicional. | Texto | No | Opcional. |

## GestiÃ³n de transporte

### Diccionario lÃ³gico: VehÃ­culos

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del vehÃ­culo. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Placa | NÃºmero de placa. | Texto corto | SÃ­ | Ãšnico. |
| Capacidad | Cantidad mÃ¡xima de estudiantes. | NÃºmero entero | SÃ­ | Debe ser mayor que cero. |
| Estado | Estado operativo del vehÃ­culo. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |

### Diccionario lÃ³gico: Conductores

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del conductor. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre y apellido | Datos personales del conductor. | Texto | SÃ­ | Separado en nombre/apellido. |
| Documento | Tipo y nÃºmero de documento. | Texto/catÃ¡logo | SÃ­ | NÃºmero Ãºnico. |
| Licencia | NÃºmero de licencia. | Texto corto | SÃ­ | Ãšnico. |
| TelÃ©fono | TelÃ©fono de contacto. | Texto corto | SÃ­ | ValueObject. |
| Correo | Correo electrÃ³nico. | Correo | No | Opcional. |
| DirecciÃ³n | Calle y ciudad. | Texto | SÃ­ | ValueObject. |
| Estado activo | Indica si puede operar. | Booleano | SÃ­ | Tiene filtro global. |

### Diccionario lÃ³gico: Asistentes de transporte

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del asistente. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Documento | Tipo y nÃºmero de documento. | Texto/catÃ¡logo | SÃ­ | NÃºmero Ãºnico. |
| Nombre y apellido | Datos personales del asistente. | Texto | SÃ­ | Requeridos. |
| TelÃ©fono | TelÃ©fono de contacto. | Texto corto | SÃ­ | ValueObject. |
| Correo | Correo electrÃ³nico. | Correo | No | Opcional. |
| DirecciÃ³n | Calle y ciudad. | Texto | SÃ­ | ValueObject. |
| Estado activo | Indica si puede participar en viajes. | Booleano | SÃ­ | Tiene filtro global. |

### Diccionario lÃ³gico: Paradas

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de la parada. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre de la parada. | Texto | SÃ­ | Requerido. |
| Sector | Sector donde estÃ¡ ubicada. | RelaciÃ³n | SÃ­ | FK hacia Sectors. |
| DirecciÃ³n | Calle y ciudad. | Texto | SÃ­ | ValueObject. |
| Coordenadas | Latitud y longitud. | Decimal | SÃ­ | ValueObject. |

## Rutas y asignaciones

### Diccionario lÃ³gico: Rutas

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de la ruta. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Nombre | Nombre de la ruta. | Texto | SÃ­ | Requerido. |
| Escuela | Centro educativo asociado. | RelaciÃ³n | SÃ­ | FK hacia Schools. |
| Horario operativo | Hora de inicio y fin. | Hora | SÃ­ | ValueObject TimeRange. |
| Estado | Estado de la ruta. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |

### Diccionario lÃ³gico: Paradas de ruta

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de la relaciÃ³n ruta-parada. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Ruta | Ruta asociada. | RelaciÃ³n | SÃ­ | FK hacia Routes. |
| Parada | Parada asociada. | RelaciÃ³n | SÃ­ | FK hacia Stops. |
| Orden | Orden de visita de la parada. | NÃºmero entero | SÃ­ | Ãšnico por ruta. |

### Diccionario lÃ³gico: Asignaciones de ruta

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de la asignaciÃ³n. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Ruta | Ruta asignada. | RelaciÃ³n | SÃ­ | FK hacia Routes. |
| VehÃ­culo | VehÃ­culo asignado. | RelaciÃ³n | SÃ­ | FK hacia Vehicles. |
| Conductor | Conductor asignado. | RelaciÃ³n | SÃ­ | FK hacia Drivers. |
| Asistente | Asistente asignado. | RelaciÃ³n opcional | No | FK hacia TransportAssistants. |
| Capacidad | Capacidad autorizada para la asignaciÃ³n. | NÃºmero entero | SÃ­ | No debe exceder vehÃ­culo. |

### Diccionario lÃ³gico: Estudiantes por asignaciÃ³n

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Estudiante | Estudiante asignado. | RelaciÃ³n | SÃ­ | Parte de clave compuesta. |
| AsignaciÃ³n de ruta | AsignaciÃ³n relacionada. | RelaciÃ³n | SÃ­ | Parte de clave compuesta. |

## Viajes y programaciÃ³n

### Diccionario lÃ³gico: Viajes

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del viaje. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| AsignaciÃ³n de ruta | AsignaciÃ³n usada por el viaje. | RelaciÃ³n | SÃ­ | FK hacia RouteAssignments. |
| ProgramaciÃ³n | Horario que originÃ³ el viaje. | RelaciÃ³n opcional | No | FK hacia TripSchedules. |
| DirecciÃ³n | Ida o regreso escolar. | CatÃ¡logo/enum | No | Guardado como texto. |
| Fecha de operaciÃ³n | DÃ­a del viaje. | Fecha | No | Usada en materializaciÃ³n. |
| Hora programada | Salida/llegada programada. | Fecha y hora | No | ProgramaciÃ³n. |
| Hora real | Inicio/finalizaciÃ³n real. | Fecha y hora | No | OperaciÃ³n. |
| Estado | Estado del viaje. | CatÃ¡logo/enum | SÃ­ | Scheduled, InProgress, Completed, etc. |
| Razones | CancelaciÃ³n o no operaciÃ³n. | Texto | No | Trazabilidad. |

### Diccionario lÃ³gico: Programaciones de viaje

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de la programaciÃ³n. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| AsignaciÃ³n | AsignaciÃ³n de ruta programada. | RelaciÃ³n | SÃ­ | FK hacia RouteAssignments. |
| DirecciÃ³n | Tipo de viaje programado. | CatÃ¡logo/enum | SÃ­ | ToSchool/FromSchool. |
| Hora salida/llegada | Horario planificado. | Hora | SÃ­/No | Llegada es opcional. |
| Vigencia | Fecha inicial y final. | Fecha | SÃ­/No | ValidTo opcional. |
| DÃ­as activos | Lunes a domingo. | Booleano | SÃ­ | Al menos un dÃ­a activo. |
| Estado activo | Indica si la programaciÃ³n opera. | Booleano | SÃ­ | Control de uso. |

### Diccionario lÃ³gico: Asistencia de pasajeros por viaje

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Viaje | Viaje relacionado. | RelaciÃ³n | SÃ­ | Parte de clave compuesta. |
| Estudiante | Estudiante relacionado. | RelaciÃ³n | SÃ­ | Parte de clave compuesta. |
| Snapshot de estudiante | Nombre y cÃ³digo histÃ³rico. | Texto | SÃ­ | No cambia si cambia Student. |
| Snapshot de tutor | Tutor histÃ³rico del estudiante. | Texto/relaciÃ³n | No | Puede ser nulo. |
| Estado de asistencia | Expected, Boarded, Absent, DroppedOff. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |
| Fechas operativas | Abordaje y entrega. | Fecha y hora | No | SegÃºn estado. |
| Notas | ObservaciÃ³n del viaje. | Texto | No | Opcional. |

### Diccionario lÃ³gico: DÃ­as sin operaciÃ³n escolar

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del dÃ­a sin operaciÃ³n. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Fecha | Fecha no operativa. | Fecha | SÃ­ | Global o por escuela. |
| Escuela | Escuela afectada. | RelaciÃ³n opcional | No | Nulo significa global. |
| Tipo de razÃ³n | Motivo clasificado. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |
| RazÃ³n | JustificaciÃ³n textual. | Texto | SÃ­ | Requerida. |
| Estado activo | Indica si aplica. | Booleano | SÃ­ | Control histÃ³rico. |

## Monitoreo GPS

### Diccionario lÃ³gico: Ubicaciones de vehÃ­culo

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de la ubicaciÃ³n. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Viaje | Viaje monitoreado. | RelaciÃ³n | SÃ­ | FK hacia Trips. |
| VehÃ­culo | VehÃ­culo monitoreado. | RelaciÃ³n | SÃ­ | FK hacia Vehicles. |
| Latitud y longitud | Coordenadas GPS. | Decimal | SÃ­ | PrecisiÃ³n configurada. |
| Velocidad | Velocidad reportada. | Decimal | No | Opcional. |
| Rumbo | DirecciÃ³n del movimiento. | Decimal | No | Opcional. |
| Fecha de registro | Momento de captura. | Fecha y hora | SÃ­ | Ãndice de consulta. |
| Usuario reporta | Usuario que envÃ­a ubicaciÃ³n. | RelaciÃ³n opcional | No | FK hacia Users. |

## Incidencias

### Diccionario lÃ³gico: Incidencias

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de incidencia. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| TÃ­tulo y descripciÃ³n | Contenido de la incidencia. | Texto | SÃ­ | Requerido. |
| Tipo, severidad y estado | ClasificaciÃ³n operativa. | CatÃ¡logo/enum | SÃ­ | Enum por convenciÃ³n. |
| Entidades relacionadas | Viaje, asignaciÃ³n, vehÃ­culo, conductor, asistente. | RelaciÃ³n opcional | No | Trazabilidad. |
| Usuario reporta | Usuario que creÃ³ la incidencia. | RelaciÃ³n | SÃ­ | FK hacia Users. |
| Responsable/resolutor | Usuarios que gestionan la incidencia. | RelaciÃ³n opcional | No | FK hacia Users. |
| Fechas de cierre | ResoluciÃ³n y cierre. | Fecha y hora | No | SegÃºn estado. |

### Diccionario lÃ³gico: Comentarios de incidencia

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del comentario. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Incidencia | Incidencia comentada. | RelaciÃ³n | SÃ­ | FK hacia Incidents. |
| Usuario | Autor del comentario. | RelaciÃ³n | SÃ­ | FK hacia Users. |
| Comentario | Texto del seguimiento. | Texto | SÃ­ | Requerido. |

## Notificaciones

### Diccionario lÃ³gico: Notificaciones

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de la notificaciÃ³n. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Usuario | Destinatario. | RelaciÃ³n | SÃ­ | FK hacia Users. |
| TÃ­tulo y mensaje | Contenido de la notificaciÃ³n. | Texto | SÃ­ | Requerido. |
| Tipo y prioridad | ClasificaciÃ³n de la notificaciÃ³n. | CatÃ¡logo/enum | SÃ­ | Enum por convenciÃ³n. |
| LeÃ­da | Estado de lectura. | Booleano | SÃ­ | Control de bandeja. |
| Entidad relacionada | Tipo e Id relacionados. | Texto | No | Trazabilidad. |

## Reportes/AuditorÃ­a

### Diccionario lÃ³gico: AuditorÃ­a

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del log. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Usuario | Usuario que realizÃ³ la acciÃ³n. | RelaciÃ³n opcional | No | Puede ser nulo. |
| AcciÃ³n | AcciÃ³n ejecutada. | Texto corto | SÃ­ | Ej.: Created, Updated. |
| Entidad | Entidad afectada. | Texto corto | SÃ­ | Nombre e identificador. |
| Valores | Antes y despuÃ©s. | Texto largo | No | JSON o texto. |
| Contexto | IP y UserAgent. | Texto | No | AuditorÃ­a tÃ©cnica. |

## ConfiguraciÃ³n

### Diccionario lÃ³gico: Configuraciones del sistema

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico de configuraciÃ³n. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Clave | Nombre Ãºnico del parÃ¡metro. | Texto corto | SÃ­ | Ãšnico. |
| Valor | Valor almacenado. | Texto | SÃ­ | Interpretado por DataType. |
| CategorÃ­a | Grupo funcional. | Texto corto | SÃ­ | General, GPS, Backup, etc. |
| Tipo de dato | Tipo esperado del valor. | Texto corto | SÃ­ | String, Bool, Int, etc. |
| Editable | Indica si puede cambiarse. | Booleano | SÃ­ | Protege settings base. |

## Backups

### Diccionario lÃ³gico: Registros de backup

| Campo lÃ³gico | DescripciÃ³n | Tipo de dato conceptual | Obligatorio | ObservaciÃ³n |
|---|---|---|---|---|
| Identificador | CÃ³digo Ãºnico del respaldo. | Identificador Ãºnico | SÃ­ | Clave primaria. |
| Archivo | Nombre y ruta del archivo. | Texto | SÃ­ | Registro lÃ³gico. |
| TamaÃ±o | TamaÃ±o en bytes. | NÃºmero largo | No | Se asigna al completar. |
| Estado | Estado del proceso. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |
| Tipo | Manual o automÃ¡tico. | CatÃ¡logo/enum | SÃ­ | Guardado como texto. |
| Usuario creador | Usuario que solicitÃ³ el backup. | RelaciÃ³n lÃ³gica | SÃ­ | Guid del usuario. |
| Fechas | CreaciÃ³n y finalizaciÃ³n. | Fecha y hora | SÃ­/No | FinalizaciÃ³n opcional. |

---

# Diccionario de Base de Datos FÃ­sico

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Base de datos: **SQL Server**  
Fuente: entidades de dominio, `AppDbContext` y configuraciones EF Core.

> Los tipos marcados como **estimado** son inferidos por convenciones de EF Core cuando la configuraciÃ³n no define explÃ­citamente `HasColumnType`.

## Seguridad y usuarios

### Diccionario fÃ­sico: Users

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del usuario. |
| Username | nvarchar | 100 | No | UK | Nombre de usuario. |
| Name | nvarchar | 150 | No | - | Nombre visible. |
| Email | nvarchar | 150 | No | UK | Correo del usuario. |
| PasswordHash | nvarchar | max estimado | No | - | Hash de contraseÃ±a. |
| ProfileImageUrl | nvarchar | 300 | SÃ­ | - | Foto de perfil. |
| IsActive | bit | - | No | Ãndice global lÃ³gico | Estado activo. |
| RoleId | uniqueidentifier | - | No | FK | RelaciÃ³n con Roles. |
| GuardianId | uniqueidentifier | - | SÃ­ | FK | Perfil tutor vinculado. |
| DriverId | uniqueidentifier | - | SÃ­ | FK | Perfil conductor vinculado. |
| TransportAssistantId | uniqueidentifier | - | SÃ­ | FK | Perfil asistente vinculado. |
| LastLoginAt | datetime2 estimado | - | SÃ­ | - | Ãšltimo acceso. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Users.RoleId` se relaciona con `Roles.Id`.
- `Users.GuardianId`, `DriverId` y `TransportAssistantId` son relaciones opcionales con perfiles operativos.

### Diccionario fÃ­sico: Roles

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del rol. |
| Name | nvarchar | 100 | No | UK | Nombre del rol. |
| Description | nvarchar | 300 | SÃ­ | - | DescripciÃ³n del rol. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Roles` se relaciona con `Users` mediante `Users.RoleId`.
- `Roles` se relaciona con `Permissions` mediante `RolePermissions`.

### Diccionario fÃ­sico: Permissions

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del permiso. |
| Name | nvarchar | 100 | No | UK | Nombre del permiso. |
| Description | nvarchar | 300 | SÃ­ | - | DescripciÃ³n. |
| Module | nvarchar | 100 | No | - | MÃ³dulo asociado. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Permissions` se relaciona con `Roles` mediante `RolePermissions`.

### Diccionario fÃ­sico: RolePermissions

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| RoleId | uniqueidentifier | - | No | PK, FK | Rol relacionado. |
| PermissionId | uniqueidentifier | - | No | PK, FK | Permiso relacionado. |

Relaciones:
- Tabla puente entre `Roles` y `Permissions`.

## GestiÃ³n acadÃ©mica

### Diccionario fÃ­sico: Students

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del estudiante. |
| StudentCode | nvarchar | max estimado | No | - | CÃ³digo del estudiante. |
| FirstName | nvarchar | 250 | No | - | Nombre. |
| LastName | nvarchar | 300 | No | - | Apellido. |
| SchoolId | uniqueidentifier | - | No | FK | Escuela. |
| GradeId | uniqueidentifier | - | No | FK | Grado. |
| GuardianId | uniqueidentifier | - | No | FK | Tutor. |
| PhotoUrl | nvarchar | 300 | SÃ­ | - | Foto. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Students` se relaciona con `Schools`, `Grades`, `Guardians` y `StudentRouteAssignments`.

### Diccionario fÃ­sico: Guardians

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del tutor. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento como texto. |
| DocumentNumber | nvarchar | 50 | No | UK | NÃºmero de documento. |
| FirstName | nvarchar | 100 | No | - | Nombre. |
| LastName | nvarchar | 100 | No | - | Apellido. |
| Phone | nvarchar | 50 | No | - | TelÃ©fono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| PhotoUrl | nvarchar | 300 | SÃ­ | - | Foto. |
| Gender | nvarchar | max estimado | No | - | GÃ©nero como texto. |
| SectorId | uniqueidentifier | - | SÃ­ | FK | Sector. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Guardians` se relaciona con `Sectors`, `Students` y opcionalmente `Users`.

### Diccionario fÃ­sico: Schools

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del centro. |
| Name | nvarchar | 150 | No | - | Nombre del centro. |
| DirectorName | nvarchar | 150 | No | - | Director. |
| SectorId | uniqueidentifier | - | No | FK | Sector. |
| Description | nvarchar | 500 | SÃ­ | - | DescripciÃ³n. |
| ProfileImageUrl | nvarchar | 300 | SÃ­ | - | Imagen. |
| Email | nvarchar | 150 | No | - | Correo. |
| Phone | nvarchar | 50 | No | - | TelÃ©fono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Schools` se relaciona con `Sectors`, `Grades`, `Students`, `Routes` y `NonSchoolDays`.

### Diccionario fÃ­sico: Grades

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del grado. |
| Name | nvarchar | 100 | No | UK compuesto | Nombre del grado. |
| SchoolId | uniqueidentifier | - | No | FK, UK compuesto | Escuela. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Grades` se relaciona con `Schools` y `Students`.

### Diccionario fÃ­sico: Sectors

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del sector. |
| Name | nvarchar | 150 | No | Ãndice | Nombre. |
| City | nvarchar | 100 | No | Ãndice | Ciudad. |
| Province | nvarchar | 100 | No | - | Provincia. |
| SchoolDistrictId | uniqueidentifier | - | SÃ­ | FK | Distrito educativo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Sectors` se relaciona con `SchoolDistricts`, `Schools`, `Guardians` y `Stops`.

### Diccionario fÃ­sico: SchoolDistricts

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del distrito. |
| Name | nvarchar | 150 | No | - | Nombre. |
| Code | nvarchar | 50 | No | UK | CÃ³digo Ãºnico. |
| Description | nvarchar | 500 | SÃ­ | - | DescripciÃ³n. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `SchoolDistricts` se relaciona con `Sectors`.

## GestiÃ³n de transporte

### Diccionario fÃ­sico: Vehicles

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del vehÃ­culo. |
| PlateNumber | nvarchar | 20 | No | UK | Placa. |
| Capacity | int | - | No | - | Capacidad. |
| Status | nvarchar | max estimado | No | - | Estado como texto. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Vehicles` se relaciona con `RouteAssignments`, `Trips` indirectamente, `Incidents` y `VehicleLocations`.

### Diccionario fÃ­sico: Drivers

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del conductor. |
| FirstName | nvarchar | 100 | No | - | Nombre. |
| LastName | nvarchar | 100 | No | - | Apellido. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento. |
| DocumentNumber | nvarchar | 50 | No | UK | Documento. |
| LicenseNumber | nvarchar | 50 | No | UK | Licencia. |
| Phone | nvarchar | 50 | No | - | TelÃ©fono. |
| Email | nvarchar | 150 | SÃ­ | - | Correo. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| PhotoUrl | nvarchar | 300 | SÃ­ | - | Foto. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Drivers` se relaciona con `RouteAssignments`, `Users` e `Incidents`.

### Diccionario fÃ­sico: TransportAssistants

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del asistente. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento. |
| DocumentNumber | nvarchar | 50 | No | UK | Documento. |
| FirstName | nvarchar | 150 | No | - | Nombre. |
| LastName | nvarchar | 150 | No | - | Apellido. |
| Phone | nvarchar | 50 | No | - | TelÃ©fono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| Email | nvarchar | 150 | SÃ­ | - | Correo. |
| PhotoUrl | nvarchar | 300 | SÃ­ | - | Foto. |
| IsActive | bit | - | No | Filtro global | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `TransportAssistants` se relaciona con `RouteAssignments`, `Users` e `Incidents`.

### Diccionario fÃ­sico: Stops

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de parada. |
| Name | nvarchar | 150 | No | - | Nombre. |
| SectorId | uniqueidentifier | - | No | FK | Sector. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| Latitude | decimal estimado | - | No | - | Latitud. |
| Longitude | decimal estimado | - | No | - | Longitud. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Stops` se relaciona con `Sectors` y `RouteStops`.

## Rutas y asignaciones

### Diccionario fÃ­sico: Routes

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de ruta. |
| Name | nvarchar | 150 | No | - | Nombre. |
| SchoolId | uniqueidentifier | - | No | FK | Escuela. |
| Status | nvarchar | max estimado | No | - | Estado como texto. |
| StartTime | time estimado | - | No | - | Hora inicio. |
| EndTime | time estimado | - | No | - | Hora fin. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Routes` se relaciona con `Schools`, `RouteStops` y `RouteAssignments`.

### Diccionario fÃ­sico: RouteStops

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de parada en ruta. |
| RouteId | uniqueidentifier | - | No | FK, UK compuesto | Ruta. |
| StopId | uniqueidentifier | - | No | FK | Parada. |
| StopOrder | int | - | No | UK compuesto | Orden en ruta. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `RouteStops` se relaciona con `Routes` y `Stops`.

### Diccionario fÃ­sico: RouteAssignments

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de asignaciÃ³n. |
| RouteId | uniqueidentifier | - | No | FK | Ruta. |
| VehicleId | uniqueidentifier | - | No | FK | VehÃ­culo. |
| DriverId | uniqueidentifier | - | No | FK | Conductor. |
| TransportAssistantId | uniqueidentifier | - | SÃ­ | FK | Asistente. |
| VehicleCapacity | int | - | No | - | Capacidad asignada. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `RouteAssignments` se relaciona con `Routes`, `Vehicles`, `Drivers`, `TransportAssistants`, `StudentRouteAssignments`, `Trips` y `TripSchedules`.

### Diccionario fÃ­sico: StudentRouteAssignments

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| StudentId | uniqueidentifier | - | No | PK, FK | Estudiante. |
| RouteAssignmentId | uniqueidentifier | - | No | PK, FK | AsignaciÃ³n de ruta. |

Relaciones:
- Tabla puente entre `Students` y `RouteAssignments`.

## Viajes y programaciÃ³n

### Diccionario fÃ­sico: Trips

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del viaje. |
| RouteAssignmentId | uniqueidentifier | - | No | FK | AsignaciÃ³n de ruta. |
| TripScheduleId | uniqueidentifier | - | SÃ­ | FK, Ã­ndice | ProgramaciÃ³n. |
| Direction | nvarchar | 30 | SÃ­ | - | DirecciÃ³n. |
| OperationDate | date | - | SÃ­ | UK compuesto filtrado | Fecha de operaciÃ³n. |
| ScheduledDepartureTime | datetime2 estimado | - | SÃ­ | - | Salida programada. |
| ScheduledArrivalTime | datetime2 estimado | - | SÃ­ | - | Llegada programada. |
| StartTime | datetime2 estimado | - | SÃ­ | - | Inicio real. |
| EndTime | datetime2 estimado | - | SÃ­ | - | Fin real. |
| Status | nvarchar | max estimado | No | - | Estado como texto. |
| CancellationReason | nvarchar | 500 | SÃ­ | - | RazÃ³n de cancelaciÃ³n. |
| NonOperationReason | nvarchar | 200 | SÃ­ | - | RazÃ³n de no operaciÃ³n. |
| NonOperationNotes | nvarchar | 1000 | SÃ­ | - | Notas de no operaciÃ³n. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Trips` se relaciona con `RouteAssignments`, `TripSchedules`, `TripStudentAttendances`, `VehicleLocations` e `Incidents`.

### Diccionario fÃ­sico: TripSchedules

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de programaciÃ³n. |
| RouteAssignmentId | uniqueidentifier | - | No | FK, Ã­ndice | AsignaciÃ³n de ruta. |
| Direction | nvarchar | 30 | No | Ãndice compuesto | DirecciÃ³n. |
| DepartureTime | time | - | No | - | Hora salida. |
| ArrivalTime | time | - | SÃ­ | - | Hora llegada. |
| IsActive | bit | - | No | - | Estado activo. |
| ValidFrom | date | - | No | Ãndice compuesto | Vigencia inicial. |
| ValidTo | date | - | SÃ­ | - | Vigencia final. |
| Monday | bit | - | No | - | Lunes activo. |
| Tuesday | bit | - | No | - | Martes activo. |
| Wednesday | bit | - | No | - | MiÃ©rcoles activo. |
| Thursday | bit | - | No | - | Jueves activo. |
| Friday | bit | - | No | - | Viernes activo. |
| Saturday | bit | - | No | - | SÃ¡bado activo. |
| Sunday | bit | - | No | - | Domingo activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `TripSchedules` se relaciona con `RouteAssignments` y `Trips`.

### Diccionario fÃ­sico: TripStudentAttendances

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| TripId | uniqueidentifier | - | No | PK, FK, Ã­ndice | Viaje. |
| StudentId | uniqueidentifier | - | No | PK, FK, Ã­ndice | Estudiante. |
| StudentNameSnapshot | nvarchar | 250 | No | - | Nombre histÃ³rico. |
| StudentCodeSnapshot | nvarchar | 100 | No | - | CÃ³digo histÃ³rico. |
| GuardianIdSnapshot | uniqueidentifier | - | SÃ­ | Ãndice | Tutor histÃ³rico. |
| GuardianNameSnapshot | nvarchar | 250 | SÃ­ | - | Nombre tutor histÃ³rico. |
| Status | nvarchar | 30 | No | - | Estado asistencia. |
| BoardedAt | datetime2 estimado | - | SÃ­ | - | Fecha abordaje. |
| DroppedOffAt | datetime2 estimado | - | SÃ­ | - | Fecha entrega. |
| Notes | nvarchar | 500 | SÃ­ | - | Notas. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `TripStudentAttendances` se relaciona con `Trips` y `Students`.

### Diccionario fÃ­sico: NonSchoolDays

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Date | date | - | No | Ãndice, UK filtrado | Fecha. |
| SchoolId | uniqueidentifier | - | SÃ­ | FK, UK filtrado | Escuela afectada. |
| ReasonType | nvarchar | 50 | No | - | Tipo de razÃ³n. |
| Reason | nvarchar | 500 | No | - | RazÃ³n. |
| IsActive | bit | - | No | Ãndice | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `NonSchoolDays` se relaciona opcionalmente con `Schools`.

## Monitoreo GPS

### Diccionario fÃ­sico: VehicleLocations

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| TripId | uniqueidentifier | - | No | FK, Ã­ndice | Viaje. |
| VehicleId | uniqueidentifier | - | No | FK, Ã­ndice | VehÃ­culo. |
| Latitude | decimal | 9,6 | No | - | Latitud. |
| Longitude | decimal | 9,6 | No | - | Longitud. |
| Speed | decimal | 8,2 | SÃ­ | - | Velocidad. |
| Heading | decimal | 6,2 | SÃ­ | - | Rumbo. |
| RecordedAt | datetime2 estimado | - | No | Ãndice | Fecha de registro. |
| ReportedByUserId | uniqueidentifier | - | SÃ­ | FK | Usuario que reporta. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `VehicleLocations` se relaciona con `Trips`, `Vehicles` y `Users`.

## Incidencias

### Diccionario fÃ­sico: Incidents

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Title | nvarchar | 200 | No | - | TÃ­tulo. |
| Description | nvarchar | 2000 | No | - | DescripciÃ³n. |
| Type | int estimado | - | No | - | Tipo de incidencia. |
| Severity | int estimado | - | No | Ãndice | Severidad. |
| Status | int estimado | - | No | Ãndice | Estado. |
| TripId | uniqueidentifier | - | SÃ­ | FK, Ã­ndice | Viaje. |
| RouteAssignmentId | uniqueidentifier | - | SÃ­ | FK, Ã­ndice | AsignaciÃ³n. |
| VehicleId | uniqueidentifier | - | SÃ­ | FK | VehÃ­culo. |
| DriverId | uniqueidentifier | - | SÃ­ | FK | Conductor. |
| TransportAssistantId | uniqueidentifier | - | SÃ­ | FK | Asistente. |
| ReportedByUserId | uniqueidentifier | - | No | FK, Ã­ndice | Usuario reporta. |
| AssignedToUserId | uniqueidentifier | - | SÃ­ | FK | Usuario asignado. |
| ResolvedByUserId | uniqueidentifier | - | SÃ­ | FK | Usuario resolutor. |
| ResolvedAt | datetime2 estimado | - | SÃ­ | - | Fecha resoluciÃ³n. |
| ClosedAt | datetime2 estimado | - | SÃ­ | - | Fecha cierre. |
| CreatedAt | datetime2 estimado | - | No | Ãndice | Fecha de creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha de actualizaciÃ³n. |

Relaciones:
- `Incidents` se relaciona con viajes, asignaciones, recursos de transporte, usuarios y comentarios.

### Diccionario fÃ­sico: IncidentComments

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| IncidentId | uniqueidentifier | - | No | FK, Ã­ndice | Incidencia. |
| UserId | uniqueidentifier | - | No | FK, Ã­ndice | Autor. |
| Comment | nvarchar | 1000 | No | - | Comentario. |
| CreatedAt | datetime2 estimado | - | No | Ãndice | Fecha creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha actualizaciÃ³n. |

Relaciones:
- `IncidentComments` se relaciona con `Incidents` y `Users`.

## Notificaciones

### Diccionario fÃ­sico: Notifications

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| UserId | uniqueidentifier | - | No | FK, Ã­ndice | Usuario destinatario. |
| Title | nvarchar | 200 | No | - | TÃ­tulo. |
| Message | nvarchar | 1000 | No | - | Mensaje. |
| Type | int estimado | - | No | - | Tipo. |
| Priority | int estimado | - | No | - | Prioridad. |
| IsRead | bit | - | No | Ãndice | Estado lectura. |
| ReadAt | datetime2 estimado | - | SÃ­ | - | Fecha lectura. |
| RelatedEntityType | nvarchar | 150 | SÃ­ | Ãndice compuesto | Tipo entidad. |
| RelatedEntityId | nvarchar | 100 | SÃ­ | Ãndice compuesto | Id entidad. |
| CreatedAt | datetime2 estimado | - | No | Ãndice | Fecha creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha actualizaciÃ³n. |

Relaciones:
- `Notifications` se relaciona con `Users`.

## Reportes/AuditorÃ­a

### Diccionario fÃ­sico: AuditLogs

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| UserId | uniqueidentifier | - | SÃ­ | Ãndice | Usuario relacionado. |
| Username | nvarchar | 100 | SÃ­ | - | Nombre de usuario. |
| Action | nvarchar | 50 | No | Ãndice | AcciÃ³n. |
| EntityName | nvarchar | 150 | No | Ãndice compuesto | Entidad. |
| EntityId | nvarchar | 100 | SÃ­ | Ãndice compuesto | Id de entidad. |
| OldValues | nvarchar | max | SÃ­ | - | Valores anteriores. |
| NewValues | nvarchar | max | SÃ­ | - | Valores nuevos. |
| IpAddress | nvarchar | 100 | SÃ­ | - | IP. |
| UserAgent | nvarchar | 500 | SÃ­ | - | Navegador/agente. |
| CreatedAt | datetime2 estimado | - | No | Ãndice | Fecha creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha actualizaciÃ³n. |

Relaciones:
- No tiene FK explÃ­cita a `Users`; conserva `UserId` lÃ³gico para auditorÃ­a.

## ConfiguraciÃ³n

### Diccionario fÃ­sico: SystemSettings

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Key | nvarchar | 150 | No | UK | Clave de configuraciÃ³n. |
| Value | nvarchar | 1000 | No | - | Valor. |
| Description | nvarchar | 500 | SÃ­ | - | DescripciÃ³n. |
| Category | nvarchar | 100 | No | - | CategorÃ­a. |
| DataType | nvarchar | 50 | No | - | Tipo de dato. |
| IsEditable | bit | - | No | - | Editable. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha actualizaciÃ³n. |

Relaciones:
- Tabla independiente de configuraciÃ³n.

## Backups

### Diccionario fÃ­sico: BackupRecords

| Campo fÃ­sico | Tipo SQL Server | Longitud / precisiÃ³n | Nulo | Clave | DescripciÃ³n |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| FileName | nvarchar | 260 | No | - | Nombre archivo. |
| FilePath | nvarchar | 1000 | No | - | Ruta archivo. |
| FileSizeBytes | bigint estimado | - | SÃ­ | - | TamaÃ±o. |
| Status | nvarchar | max estimado | No | Ãndice | Estado como texto. |
| Type | nvarchar | max estimado | No | Ãndice | Tipo como texto. |
| ErrorMessage | nvarchar | 1000 | SÃ­ | - | Error. |
| CreatedByUserId | uniqueidentifier | - | No | - | Usuario creador. |
| CompletedAt | datetime2 estimado | - | SÃ­ | - | Fecha finalizaciÃ³n. |
| CreatedAt | datetime2 estimado | - | No | Ãndice | Fecha creaciÃ³n. |
| UpdatedAt | datetime2 estimado | - | SÃ­ | - | Fecha actualizaciÃ³n. |

Relaciones:
- No tiene FK explÃ­cita a `Users`; almacena el identificador del usuario creador.
