# 4.3.8 Diccionarios de Base de Datos Lógico y Físico

El presente apartado describe los diccionarios lógico y físico de la base de datos correspondiente a la **Plataforma Web para el Sistema de Transporte Escolar TRAE**. La información fue estructurada a partir del modelo de datos implementado en el sistema, tomando como referencia las entidades de dominio, el contexto de Entity Framework Core y las configuraciones de persistencia.

Debido a la cantidad de entidades del sistema, se presentan en este apartado las tablas principales del modelo de datos, mientras que las tablas complementarias pueden ser colocadas en anexos si la extensión del documento lo requiere.

El diccionario lógico utiliza nombres comprensibles en español y describe el significado funcional de cada campo. El diccionario físico conserva los nombres exactos de las columnas utilizadas en la base de datos SQL Server.

## A. Diccionarios principales para el Capítulo IV

## Seguridad y usuarios

Tabla X  
Diccionario lógico de la entidad Usuario

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del usuario. | Identificador único | Sí | Clave primaria. |
| Nombre de usuario | Credencial usada para iniciar sesión. | Texto corto | Sí | Único. |
| Nombre | Nombre visible del usuario. | Texto | Sí | Se muestra en el perfil. |
| Correo electrónico | Correo asociado al usuario. | Correo | Sí | Único. |
| Contraseña cifrada | Hash de la contraseña. | Texto protegido | Sí | No se expone al usuario. |
| Rol | Rol asignado al usuario. | Relación | Sí | Define permisos de acceso. |
| Estado | Indica si el usuario está activo. | Booleano | Sí | Controla acceso al sistema. |
| Imagen de perfil | Ruta de foto del usuario. | Texto | No | Opcional. |
| Último acceso | Fecha del último inicio de sesión. | Fecha y hora | No | Apoyo a auditoría. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Users

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del usuario. |
| Username | nvarchar | 100 | No | UK | Nombre de usuario. |
| Name | nvarchar | 150 | No | - | Nombre visible. |
| Email | nvarchar | 150 | No | UK | Correo electrónico. |
| PasswordHash | nvarchar | max estimado | No | - | Hash de contraseña. |
| ProfileImageUrl | nvarchar | 300 | Sí | - | Imagen de perfil. |
| IsActive | bit | - | No | - | Estado del usuario. |
| RoleId | uniqueidentifier | - | No | FK | Relación con Roles. |
| GuardianId | uniqueidentifier | - | Sí | FK | Tutor vinculado. |
| DriverId | uniqueidentifier | - | Sí | FK | Conductor vinculado. |
| TransportAssistantId | uniqueidentifier | - | Sí | FK | Asistente vinculado. |
| LastLoginAt | datetime2 estimado | - | Sí | - | Último acceso. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Rol

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del rol. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del rol. | Texto corto | Sí | Único. |
| Descripción | Detalle funcional del rol. | Texto | No | Describe su alcance. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Roles

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del rol. |
| Name | nvarchar | 100 | No | UK | Nombre del rol. |
| Description | nvarchar | 300 | Sí | - | Descripción. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## Gestión académica

Tabla X  
Diccionario lógico de la entidad Estudiante

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del estudiante. | Identificador único | Sí | Clave primaria. |
| Código de estudiante | Código interno del estudiante. | Texto corto | Sí | Value Object persistido. |
| Nombre | Nombre del estudiante. | Texto | Sí | Dato personal. |
| Apellido | Apellido del estudiante. | Texto | Sí | Dato personal. |
| Escuela | Centro educativo del estudiante. | Relación | Sí | Relación con Schools. |
| Grado | Grado escolar. | Relación | Sí | Relación con Grades. |
| Tutor | Tutor responsable. | Relación | Sí | Relación con Guardians. |
| Foto | Imagen del estudiante. | Texto | No | Opcional. |
| Estado | Indica si está activo. | Booleano | Sí | Tiene filtro de actividad. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Students

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del estudiante. |
| StudentCode | nvarchar | max estimado | No | - | Código del estudiante. |
| FirstName | nvarchar | 250 | No | - | Nombre. |
| LastName | nvarchar | 300 | No | - | Apellido. |
| SchoolId | uniqueidentifier | - | No | FK | Escuela. |
| GradeId | uniqueidentifier | - | No | FK | Grado. |
| GuardianId | uniqueidentifier | - | No | FK | Tutor. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| IsActive | bit | - | No | - | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Tutor

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del tutor. | Identificador único | Sí | Clave primaria. |
| Tipo de documento | Tipo de identificación. | Catálogo | Sí | Guardado como texto. |
| Número de documento | Documento del tutor. | Texto corto | Sí | Único. |
| Nombre | Nombre del tutor. | Texto | Sí | Dato personal. |
| Apellido | Apellido del tutor. | Texto | Sí | Dato personal. |
| Teléfono | Teléfono de contacto. | Texto corto | Sí | Requerido. |
| Dirección | Calle y ciudad. | Texto | Sí | Value Object. |
| Género | Género registrado. | Catálogo | Sí | Guardado como texto. |
| Sector | Sector de residencia. | Relación | No | Opcional. |
| Estado | Indica si está activo. | Booleano | Sí | Tiene filtro de actividad. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Guardians

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del tutor. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento. |
| DocumentNumber | nvarchar | 50 | No | UK | Número de documento. |
| FirstName | nvarchar | 100 | No | - | Nombre. |
| LastName | nvarchar | 100 | No | - | Apellido. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| Gender | nvarchar | max estimado | No | - | Género. |
| SectorId | uniqueidentifier | - | Sí | FK | Sector. |
| IsActive | bit | - | No | - | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Centro educativo

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del centro. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del centro educativo. | Texto | Sí | Requerido. |
| Director | Nombre del director. | Texto | Sí | Requerido. |
| Correo | Correo institucional. | Correo | Sí | Value Object. |
| Teléfono | Teléfono institucional. | Texto corto | Sí | Value Object. |
| Dirección | Calle y ciudad. | Texto | Sí | Value Object. |
| Sector | Sector asociado. | Relación | Sí | Relación con Sectors. |
| Descripción | Información adicional. | Texto | No | Opcional. |
| Estado | Indica si está activo. | Booleano | Sí | Tiene filtro de actividad. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Schools

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del centro. |
| Name | nvarchar | 150 | No | - | Nombre. |
| DirectorName | nvarchar | 150 | No | - | Director. |
| SectorId | uniqueidentifier | - | No | FK | Sector. |
| Description | nvarchar | 500 | Sí | - | Descripción. |
| ProfileImageUrl | nvarchar | 300 | Sí | - | Imagen. |
| Email | nvarchar | 150 | No | - | Correo. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| IsActive | bit | - | No | - | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Grado

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del grado. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del grado. | Texto corto | Sí | Único por escuela. |
| Escuela | Centro educativo al que pertenece. | Relación | Sí | Relación con Schools. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Grades

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del grado. |
| Name | nvarchar | 100 | No | UK compuesto | Nombre del grado. |
| SchoolId | uniqueidentifier | - | No | FK, UK compuesto | Escuela. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## Gestión de transporte

Tabla X  
Diccionario lógico de la entidad Vehículo

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del vehículo. | Identificador único | Sí | Clave primaria. |
| Placa | Número de placa. | Texto corto | Sí | Único. |
| Capacidad | Cantidad máxima de estudiantes. | Número entero | Sí | Debe ser mayor que cero. |
| Estado | Estado operativo. | Catálogo | Sí | Activo, inactivo o mantenimiento. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Vehicles

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del vehículo. |
| PlateNumber | nvarchar | 20 | No | UK | Placa. |
| Capacity | int | - | No | - | Capacidad. |
| Status | nvarchar | max estimado | No | - | Estado. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Conductor

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del conductor. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del conductor. | Texto | Sí | Requerido. |
| Apellido | Apellido del conductor. | Texto | Sí | Requerido. |
| Documento | Identificación del conductor. | Texto/catálogo | Sí | Número único. |
| Número de licencia | Licencia de conducir. | Texto corto | Sí | Único. |
| Teléfono | Teléfono de contacto. | Texto corto | Sí | Requerido. |
| Correo | Correo electrónico. | Correo | No | Opcional. |
| Dirección | Calle y ciudad. | Texto | Sí | Requerido. |
| Estado | Indica si puede operar. | Booleano | Sí | Tiene filtro de actividad. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Drivers

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del conductor. |
| FirstName | nvarchar | 100 | No | - | Nombre. |
| LastName | nvarchar | 100 | No | - | Apellido. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento. |
| DocumentNumber | nvarchar | 50 | No | UK | Número de documento. |
| LicenseNumber | nvarchar | 50 | No | UK | Número de licencia. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Email | nvarchar | 150 | Sí | - | Correo. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| IsActive | bit | - | No | - | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Asistente de transporte

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del asistente. | Identificador único | Sí | Clave primaria. |
| Documento | Identificación del asistente. | Texto/catálogo | Sí | Número único. |
| Nombre | Nombre del asistente. | Texto | Sí | Requerido. |
| Apellido | Apellido del asistente. | Texto | Sí | Requerido. |
| Teléfono | Teléfono de contacto. | Texto corto | Sí | Requerido. |
| Correo | Correo electrónico. | Correo | No | Opcional. |
| Dirección | Calle y ciudad. | Texto | Sí | Requerido. |
| Estado | Indica si puede operar. | Booleano | Sí | Tiene filtro de actividad. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla TransportAssistants

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del asistente. |
| DocumentType | nvarchar | max estimado | No | - | Tipo de documento. |
| DocumentNumber | nvarchar | 50 | No | UK | Número de documento. |
| FirstName | nvarchar | 150 | No | - | Nombre. |
| LastName | nvarchar | 150 | No | - | Apellido. |
| Phone | nvarchar | 50 | No | - | Teléfono. |
| Street | nvarchar | 200 | No | - | Calle. |
| City | nvarchar | 100 | No | - | Ciudad. |
| Email | nvarchar | 150 | Sí | - | Correo. |
| PhotoUrl | nvarchar | 300 | Sí | - | Foto. |
| IsActive | bit | - | No | - | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Parada

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la parada. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre de la parada. | Texto | Sí | Requerido. |
| Sector | Sector donde se ubica. | Relación | Sí | Relación con Sectors. |
| Dirección | Calle y ciudad. | Texto | Sí | Value Object. |
| Coordenadas | Latitud y longitud. | Decimal | Sí | Ubicación geográfica. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Stops

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
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

Nota. Elaboración propia.

## Rutas y asignaciones

Tabla X  
Diccionario lógico de la entidad Ruta

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la ruta. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre de la ruta. | Texto | Sí | Requerido. |
| Escuela | Centro educativo asociado. | Relación | Sí | Relación con Schools. |
| Hora de inicio | Inicio del horario operativo. | Hora | Sí | Parte del rango horario. |
| Hora de fin | Final del horario operativo. | Hora | Sí | Parte del rango horario. |
| Estado | Estado de la ruta. | Catálogo | Sí | Activa o inactiva. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Routes

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de ruta. |
| Name | nvarchar | 150 | No | - | Nombre. |
| SchoolId | uniqueidentifier | - | No | FK | Escuela. |
| Status | nvarchar | max estimado | No | - | Estado. |
| StartTime | time estimado | - | No | - | Hora de inicio. |
| EndTime | time estimado | - | No | - | Hora de fin. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Asignación de ruta

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la asignación. | Identificador único | Sí | Clave primaria. |
| Ruta | Ruta asignada. | Relación | Sí | Relación con Routes. |
| Vehículo | Vehículo asignado. | Relación | Sí | Relación con Vehicles. |
| Conductor | Conductor asignado. | Relación | Sí | Relación con Drivers. |
| Asistente | Asistente asignado. | Relación | No | Opcional. |
| Capacidad | Capacidad autorizada. | Número entero | Sí | Validada contra vehículo. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla RouteAssignments

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de asignación. |
| RouteId | uniqueidentifier | - | No | FK | Ruta. |
| VehicleId | uniqueidentifier | - | No | FK | Vehículo. |
| DriverId | uniqueidentifier | - | No | FK | Conductor. |
| TransportAssistantId | uniqueidentifier | - | Sí | FK | Asistente. |
| VehicleCapacity | int | - | No | - | Capacidad asignada. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Estudiante asignado a ruta

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Estudiante | Estudiante asignado. | Relación | Sí | Parte de clave compuesta. |
| Asignación de ruta | Asignación relacionada. | Relación | Sí | Parte de clave compuesta. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla StudentRouteAssignments

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| StudentId | uniqueidentifier | - | No | PK, FK | Estudiante. |
| RouteAssignmentId | uniqueidentifier | - | No | PK, FK | Asignación de ruta. |

Nota. Elaboración propia.

## Viajes y programación

Tabla X  
Diccionario lógico de la entidad Viaje

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del viaje. | Identificador único | Sí | Clave primaria. |
| Asignación de ruta | Asignación utilizada. | Relación | Sí | Relación con RouteAssignments. |
| Programación | Horario que originó el viaje. | Relación | No | Opcional. |
| Dirección | Sentido del viaje. | Catálogo | No | Ida o regreso. |
| Fecha de operación | Día del viaje. | Fecha | No | Usado en programación. |
| Hora programada | Salida y llegada programadas. | Fecha y hora | No | Opcional. |
| Hora real | Inicio y finalización real. | Fecha y hora | No | Operación. |
| Estado | Estado del viaje. | Catálogo | Sí | Scheduled, InProgress, Completed, etc. |
| Razón | Razón de cancelación o no operación. | Texto | No | Trazabilidad. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Trips

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador del viaje. |
| RouteAssignmentId | uniqueidentifier | - | No | FK | Asignación de ruta. |
| TripScheduleId | uniqueidentifier | - | Sí | FK, índice | Programación. |
| Direction | nvarchar | 30 | Sí | - | Dirección. |
| OperationDate | date | - | Sí | UK filtrado | Fecha de operación. |
| ScheduledDepartureTime | datetime2 estimado | - | Sí | - | Salida programada. |
| ScheduledArrivalTime | datetime2 estimado | - | Sí | - | Llegada programada. |
| StartTime | datetime2 estimado | - | Sí | - | Inicio real. |
| EndTime | datetime2 estimado | - | Sí | - | Fin real. |
| Status | nvarchar | max estimado | No | - | Estado. |
| CancellationReason | nvarchar | 500 | Sí | - | Razón de cancelación. |
| NonOperationReason | nvarchar | 200 | Sí | - | Razón de no operación. |
| NonOperationNotes | nvarchar | 1000 | Sí | - | Notas de no operación. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Programación de viaje

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la programación. | Identificador único | Sí | Clave primaria. |
| Asignación de ruta | Asignación programada. | Relación | Sí | Relación con RouteAssignments. |
| Dirección | Sentido del viaje. | Catálogo | Sí | Ida o regreso. |
| Hora de salida | Hora programada. | Hora | Sí | Requerida. |
| Hora de llegada | Hora estimada de llegada. | Hora | No | Opcional. |
| Vigencia | Fechas de inicio y fin. | Fecha | Sí/No | Fin opcional. |
| Días activos | Días de operación semanal. | Booleano | Sí | Al menos un día activo. |
| Estado | Indica si la programación está activa. | Booleano | Sí | Control operativo. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla TripSchedules

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador de programación. |
| RouteAssignmentId | uniqueidentifier | - | No | FK, índice | Asignación. |
| Direction | nvarchar | 30 | No | Índice compuesto | Dirección. |
| DepartureTime | time | - | No | - | Hora de salida. |
| ArrivalTime | time | - | Sí | - | Hora de llegada. |
| IsActive | bit | - | No | - | Estado activo. |
| ValidFrom | date | - | No | Índice compuesto | Vigencia inicial. |
| ValidTo | date | - | Sí | - | Vigencia final. |
| Monday | bit | - | No | - | Lunes. |
| Tuesday | bit | - | No | - | Martes. |
| Wednesday | bit | - | No | - | Miércoles. |
| Thursday | bit | - | No | - | Jueves. |
| Friday | bit | - | No | - | Viernes. |
| Saturday | bit | - | No | - | Sábado. |
| Sunday | bit | - | No | - | Domingo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Asistencia de pasajeros por viaje

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Viaje | Viaje relacionado. | Relación | Sí | Parte de clave compuesta. |
| Estudiante | Estudiante relacionado. | Relación | Sí | Parte de clave compuesta. |
| Nombre histórico | Nombre del estudiante al iniciar viaje. | Texto | Sí | Snapshot. |
| Código histórico | Código del estudiante al iniciar viaje. | Texto | Sí | Snapshot. |
| Tutor histórico | Tutor asociado al momento del viaje. | Texto/relación | No | Snapshot. |
| Estado de asistencia | Estado del pasajero. | Catálogo | Sí | Expected, Boarded, Absent, DroppedOff. |
| Fechas operativas | Abordaje y entrega. | Fecha y hora | No | Según el estado. |
| Notas | Observación del pasajero. | Texto | No | Opcional. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla TripStudentAttendances

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| TripId | uniqueidentifier | - | No | PK, FK | Viaje. |
| StudentId | uniqueidentifier | - | No | PK, FK | Estudiante. |
| StudentNameSnapshot | nvarchar | 250 | No | - | Nombre histórico. |
| StudentCodeSnapshot | nvarchar | 100 | No | - | Código histórico. |
| GuardianIdSnapshot | uniqueidentifier | - | Sí | Índice | Tutor histórico. |
| GuardianNameSnapshot | nvarchar | 250 | Sí | - | Nombre del tutor. |
| Status | nvarchar | 30 | No | - | Estado de asistencia. |
| BoardedAt | datetime2 estimado | - | Sí | - | Fecha de abordaje. |
| DroppedOffAt | datetime2 estimado | - | Sí | - | Fecha de entrega. |
| Notes | nvarchar | 500 | Sí | - | Notas. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

Tabla X  
Diccionario lógico de la entidad Día sin operación escolar

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del registro. | Identificador único | Sí | Clave primaria. |
| Fecha | Día sin operación. | Fecha | Sí | Puede ser global o por escuela. |
| Escuela | Escuela afectada. | Relación | No | Nulo indica global. |
| Tipo de razón | Motivo clasificado. | Catálogo | Sí | Feriado, clima, suspensión, etc. |
| Razón | Justificación textual. | Texto | Sí | Requerida. |
| Estado | Indica si el registro aplica. | Booleano | Sí | Permite conservar historial. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla NonSchoolDays

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Date | date | - | No | Índice | Fecha. |
| SchoolId | uniqueidentifier | - | Sí | FK | Escuela. |
| ReasonType | nvarchar | 50 | No | - | Tipo de razón. |
| Reason | nvarchar | 500 | No | - | Razón. |
| IsActive | bit | - | No | Índice | Estado activo. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## Monitoreo GPS

Tabla X  
Diccionario lógico de la entidad Ubicación de vehículo

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del registro GPS. | Identificador único | Sí | Clave primaria. |
| Viaje | Viaje monitoreado. | Relación | Sí | Relación con Trips. |
| Vehículo | Vehículo monitoreado. | Relación | Sí | Relación con Vehicles. |
| Latitud | Coordenada geográfica. | Decimal | Sí | Precisión definida. |
| Longitud | Coordenada geográfica. | Decimal | Sí | Precisión definida. |
| Velocidad | Velocidad reportada. | Decimal | No | Opcional. |
| Rumbo | Orientación del vehículo. | Decimal | No | Opcional. |
| Fecha de registro | Momento de captura. | Fecha y hora | Sí | Para historial GPS. |
| Usuario reporta | Usuario que envió la ubicación. | Relación | No | Opcional. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla VehicleLocations

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
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

Nota. Elaboración propia.

## Incidencias

Tabla X  
Diccionario lógico de la entidad Incidencia

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la incidencia. | Identificador único | Sí | Clave primaria. |
| Título | Título breve de la incidencia. | Texto | Sí | Requerido. |
| Descripción | Detalle de la incidencia. | Texto | Sí | Requerido. |
| Tipo | Clasificación de la incidencia. | Catálogo | Sí | Operativo. |
| Severidad | Nivel de gravedad. | Catálogo | Sí | Bajo, medio, alto, crítico. |
| Estado | Estado de gestión. | Catálogo | Sí | Abierta, en progreso, resuelta, etc. |
| Entidades relacionadas | Viaje, asignación o recurso afectado. | Relación | No | Opcional. |
| Usuario reporta | Usuario que creó la incidencia. | Relación | Sí | Requerido. |
| Responsable | Usuario asignado. | Relación | No | Opcional. |
| Fechas de cierre | Resolución y cierre. | Fecha y hora | No | Según avance. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Incidents

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Title | nvarchar | 200 | No | - | Título. |
| Description | nvarchar | 2000 | No | - | Descripción. |
| Type | int estimado | - | No | - | Tipo. |
| Severity | int estimado | - | No | Índice | Severidad. |
| Status | int estimado | - | No | Índice | Estado. |
| TripId | uniqueidentifier | - | Sí | FK | Viaje. |
| RouteAssignmentId | uniqueidentifier | - | Sí | FK | Asignación. |
| VehicleId | uniqueidentifier | - | Sí | FK | Vehículo. |
| DriverId | uniqueidentifier | - | Sí | FK | Conductor. |
| TransportAssistantId | uniqueidentifier | - | Sí | FK | Asistente. |
| ReportedByUserId | uniqueidentifier | - | No | FK | Usuario reporta. |
| AssignedToUserId | uniqueidentifier | - | Sí | FK | Usuario asignado. |
| ResolvedByUserId | uniqueidentifier | - | Sí | FK | Usuario resolutor. |
| ResolvedAt | datetime2 estimado | - | Sí | - | Fecha resolución. |
| ClosedAt | datetime2 estimado | - | Sí | - | Fecha cierre. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## Notificaciones

Tabla X  
Diccionario lógico de la entidad Notificación

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la notificación. | Identificador único | Sí | Clave primaria. |
| Usuario | Destinatario. | Relación | Sí | Relación con Users. |
| Título | Título de la notificación. | Texto | Sí | Requerido. |
| Mensaje | Contenido de la notificación. | Texto | Sí | Requerido. |
| Tipo | Clasificación de la notificación. | Catálogo | Sí | Sistema, viaje, incidente, etc. |
| Prioridad | Nivel de prioridad. | Catálogo | Sí | Baja, media, alta, crítica. |
| Leída | Estado de lectura. | Booleano | Sí | Control de bandeja. |
| Entidad relacionada | Referencia funcional opcional. | Texto | No | Trazabilidad. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla Notifications

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| UserId | uniqueidentifier | - | No | FK | Usuario destinatario. |
| Title | nvarchar | 200 | No | - | Título. |
| Message | nvarchar | 1000 | No | - | Mensaje. |
| Type | int estimado | - | No | - | Tipo. |
| Priority | int estimado | - | No | - | Prioridad. |
| IsRead | bit | - | No | Índice | Estado de lectura. |
| ReadAt | datetime2 estimado | - | Sí | - | Fecha de lectura. |
| RelatedEntityType | nvarchar | 150 | Sí | Índice compuesto | Tipo relacionado. |
| RelatedEntityId | nvarchar | 100 | Sí | Índice compuesto | Identificador relacionado. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## Auditoría

Tabla X  
Diccionario lógico de la entidad Auditoría

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del registro. | Identificador único | Sí | Clave primaria. |
| Usuario | Usuario que realizó la acción. | Relación | No | Puede ser nulo. |
| Nombre de usuario | Usuario textual registrado. | Texto corto | No | Apoyo histórico. |
| Acción | Acción ejecutada. | Texto corto | Sí | Created, Updated, etc. |
| Entidad | Entidad afectada. | Texto corto | Sí | Nombre de tabla o entidad. |
| Valores | Datos antes y después. | Texto largo | No | Sin información sensible. |
| Contexto | Dirección IP y agente. | Texto | No | Información técnica. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla AuditLogs

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| UserId | uniqueidentifier | - | Sí | Índice | Usuario. |
| Username | nvarchar | 100 | Sí | - | Nombre de usuario. |
| Action | nvarchar | 50 | No | Índice | Acción. |
| EntityName | nvarchar | 150 | No | Índice compuesto | Entidad. |
| EntityId | nvarchar | 100 | Sí | Índice compuesto | Identificador de entidad. |
| OldValues | nvarchar | max | Sí | - | Valores anteriores. |
| NewValues | nvarchar | max | Sí | - | Valores nuevos. |
| IpAddress | nvarchar | 100 | Sí | - | Dirección IP. |
| UserAgent | nvarchar | 500 | Sí | - | Agente de usuario. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## Configuración

Tabla X  
Diccionario lógico de la entidad Configuración del sistema

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la configuración. | Identificador único | Sí | Clave primaria. |
| Clave | Nombre único del parámetro. | Texto corto | Sí | Único. |
| Valor | Valor almacenado. | Texto | Sí | Interpretado por tipo. |
| Descripción | Detalle del parámetro. | Texto | No | Opcional. |
| Categoría | Grupo funcional. | Texto corto | Sí | General, GPS, Backup, etc. |
| Tipo de dato | Tipo esperado del valor. | Texto corto | Sí | String, Bool, Int, etc. |
| Editable | Indica si puede modificarse. | Booleano | Sí | Protege valores base. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla SystemSettings

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| Key | nvarchar | 150 | No | UK | Clave. |
| Value | nvarchar | 1000 | No | - | Valor. |
| Description | nvarchar | 500 | Sí | - | Descripción. |
| Category | nvarchar | 100 | No | - | Categoría. |
| DataType | nvarchar | 50 | No | - | Tipo de dato. |
| IsEditable | bit | - | No | - | Editable. |
| CreatedAt | datetime2 estimado | - | No | - | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## Backups

Tabla X  
Diccionario lógico de la entidad Registro de backup

| Campo lógico | Descripción | Tipo conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del respaldo. | Identificador único | Sí | Clave primaria. |
| Nombre de archivo | Nombre del archivo generado. | Texto | Sí | Requerido. |
| Ruta de archivo | Ubicación del respaldo. | Texto | Sí | Requerida. |
| Tamaño | Tamaño del archivo. | Número largo | No | Se asigna al completar. |
| Estado | Estado del proceso. | Catálogo | Sí | Pendiente, completado, fallido. |
| Tipo | Tipo de respaldo. | Catálogo | Sí | Manual o automático. |
| Error | Mensaje de error si falla. | Texto | No | Opcional. |
| Usuario creador | Usuario que solicitó el respaldo. | Identificador | Sí | Registro lógico. |
| Fecha de finalización | Momento en que terminó. | Fecha y hora | No | Opcional. |

Nota. Elaboración propia.

Tabla X  
Diccionario físico de la tabla BackupRecords

| Campo físico | Tipo de dato | Longitud / precisión | Nulo | Clave | Descripción |
|---|---|---|---|---|---|
| Id | uniqueidentifier | - | No | PK | Identificador. |
| FileName | nvarchar | 260 | No | - | Nombre del archivo. |
| FilePath | nvarchar | 1000 | No | - | Ruta del archivo. |
| FileSizeBytes | bigint estimado | - | Sí | - | Tamaño en bytes. |
| Status | nvarchar | max estimado | No | Índice | Estado. |
| Type | nvarchar | max estimado | No | Índice | Tipo. |
| ErrorMessage | nvarchar | 1000 | Sí | - | Error. |
| CreatedByUserId | uniqueidentifier | - | No | - | Usuario creador. |
| CompletedAt | datetime2 estimado | - | Sí | - | Fecha de finalización. |
| CreatedAt | datetime2 estimado | - | No | Índice | Fecha de creación. |
| UpdatedAt | datetime2 estimado | - | Sí | - | Fecha de actualización. |

Nota. Elaboración propia.

## B. Tablas complementarias recomendadas para anexos

Por extensión documental, las siguientes tablas se recomiendan para anexos, aunque forman parte del modelo real del sistema:

| Tabla | Motivo de ubicación en anexos |
|---|---|
| Permissions | Complementa la administración de roles y permisos. |
| RolePermissions | Tabla puente técnica entre roles y permisos. |
| RouteStops | Tabla de detalle para el orden de paradas por ruta. |
| IncidentComments | Tabla de seguimiento textual de incidencias. |
| Sectors | Catálogo geográfico de soporte para escuelas, tutores y paradas. |
| SchoolDistricts | Catálogo institucional de soporte para sectores. |

Nota. Elaboración propia.
