# Diccionario de Base de Datos Lógico

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Sistema: **TransportStudentSystem**

El diccionario lógico describe las entidades de datos con nombres comprensibles para usuarios técnicos y académicos. Los nombres se presentan en español, independientemente del nombre físico usado por SQL Server.

## Seguridad y usuarios

### Diccionario lógico: Usuarios

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del usuario. | Identificador único | Sí | Clave primaria. |
| Nombre de usuario | Nombre usado para iniciar sesión. | Texto corto | Sí | Único. |
| Nombre visible | Nombre mostrado en el sistema. | Texto | Sí | Perfil del usuario. |
| Correo electrónico | Correo asociado al usuario. | Correo | Sí | Único. |
| Contraseña cifrada | Hash seguro de la contraseña. | Texto protegido | Sí | No debe exponerse. |
| Imagen de perfil | Ruta o URL de la foto del usuario. | Texto | No | Opcional. |
| Estado activo | Indica si puede acceder al sistema. | Booleano | Sí | Control de acceso. |
| Rol | Rol asignado al usuario. | Relación | Sí | FK hacia Roles. |
| Perfil vinculado | Tutor, conductor o asistente asociado. | Relación opcional | No | Solo aplica para usuarios operativos. |
| Último acceso | Fecha del último login. | Fecha y hora | No | Auditoría de acceso. |

### Diccionario lógico: Roles

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del rol. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del rol. | Texto corto | Sí | Único. |
| Descripción | Detalle funcional del rol. | Texto | Sí | Puede quedar vacío desde dominio. |
| Permisos | Permisos asignados al rol. | Relación | No | Relación con RolePermissions. |

### Diccionario lógico: Permisos

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del permiso. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre técnico del permiso. | Texto corto | Sí | Único. |
| Descripción | Explicación del permiso. | Texto | No | Opcional. |
| Módulo | Módulo al que pertenece. | Texto corto | Sí | Ej.: Users, Reports, Trips. |

### Diccionario lógico: Permisos por rol

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Rol | Rol al que se asigna el permiso. | Relación | Sí | Parte de clave compuesta. |
| Permiso | Permiso asignado. | Relación | Sí | Parte de clave compuesta. |

## Gestión académica

### Diccionario lógico: Estudiantes

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del estudiante. | Identificador único | Sí | Clave primaria. |
| Código de estudiante | Código académico o interno. | Texto corto | Sí | ValueObject persistido. |
| Nombre | Nombre del estudiante. | Texto | Sí | Dato personal. |
| Apellido | Apellidos del estudiante. | Texto | Sí | Dato personal. |
| Escuela | Centro educativo asociado. | Relación | Sí | FK hacia Schools. |
| Grado | Grado escolar del estudiante. | Relación | Sí | FK hacia Grades. |
| Tutor | Responsable del estudiante. | Relación | Sí | FK hacia Guardians. |
| Foto | Ruta o URL de foto. | Texto | No | Opcional. |
| Estado activo | Indica si el estudiante participa en operación. | Booleano | Sí | Tiene filtro global `IsActive`. |

### Diccionario lógico: Tutores

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del tutor. | Identificador único | Sí | Clave primaria. |
| Tipo de documento | Tipo de identificación. | Catálogo/enum | Sí | Guardado como texto. |
| Número de documento | Documento del tutor. | Texto corto | Sí | Único. |
| Nombre | Nombre del tutor. | Texto | Sí | Dato personal. |
| Apellido | Apellido del tutor. | Texto | Sí | Dato personal. |
| Teléfono | Teléfono de contacto. | Texto corto | Sí | Requerido. |
| Dirección | Calle y ciudad. | Texto | Sí | ValueObject como columnas. |
| Género | Género registrado. | Catálogo/enum | Sí | Guardado como texto. |
| Sector | Sector de residencia. | Relación opcional | No | FK hacia Sectors. |
| Estado activo | Indica si el tutor está activo. | Booleano | Sí | Tiene filtro global. |

### Diccionario lógico: Centros educativos

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del centro. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del centro educativo. | Texto | Sí | Principal descriptor. |
| Director | Nombre del director. | Texto | Sí | Dato institucional. |
| Correo | Correo institucional. | Correo | Sí | ValueObject. |
| Teléfono | Teléfono institucional. | Texto corto | Sí | ValueObject. |
| Dirección | Calle y ciudad. | Texto | Sí | ValueObject. |
| Sector | Sector al que pertenece. | Relación | Sí | FK hacia Sectors. |
| Descripción | Información adicional. | Texto | No | Opcional. |
| Estado activo | Indica si el centro está activo. | Booleano | Sí | Tiene filtro global. |

### Diccionario lógico: Grados

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del grado. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del grado escolar. | Texto corto | Sí | Único por escuela. |
| Escuela | Centro educativo al que pertenece. | Relación | Sí | FK hacia Schools. |

### Diccionario lógico: Sectores

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del sector. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del sector. | Texto | Sí | Ubicación geográfica. |
| Ciudad | Ciudad del sector. | Texto corto | Sí | Requerida. |
| Provincia | Provincia del sector. | Texto corto | Sí | Requerida. |
| Distrito educativo | Distrito asociado. | Relación opcional | No | FK hacia SchoolDistricts. |

### Diccionario lógico: Distritos educativos

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del distrito. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre del distrito educativo. | Texto | Sí | Requerido. |
| Código | Código institucional. | Texto corto | Sí | Único. |
| Dirección | Calle y ciudad. | Texto | Sí | ValueObject. |
| Descripción | Detalle adicional. | Texto | No | Opcional. |

## Gestión de transporte

### Diccionario lógico: Vehículos

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del vehículo. | Identificador único | Sí | Clave primaria. |
| Placa | Número de placa. | Texto corto | Sí | Único. |
| Capacidad | Cantidad máxima de estudiantes. | Número entero | Sí | Debe ser mayor que cero. |
| Estado | Estado operativo del vehículo. | Catálogo/enum | Sí | Guardado como texto. |

### Diccionario lógico: Conductores

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del conductor. | Identificador único | Sí | Clave primaria. |
| Nombre y apellido | Datos personales del conductor. | Texto | Sí | Separado en nombre/apellido. |
| Documento | Tipo y número de documento. | Texto/catálogo | Sí | Número único. |
| Licencia | Número de licencia. | Texto corto | Sí | Único. |
| Teléfono | Teléfono de contacto. | Texto corto | Sí | ValueObject. |
| Correo | Correo electrónico. | Correo | No | Opcional. |
| Dirección | Calle y ciudad. | Texto | Sí | ValueObject. |
| Estado activo | Indica si puede operar. | Booleano | Sí | Tiene filtro global. |

### Diccionario lógico: Asistentes de transporte

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del asistente. | Identificador único | Sí | Clave primaria. |
| Documento | Tipo y número de documento. | Texto/catálogo | Sí | Número único. |
| Nombre y apellido | Datos personales del asistente. | Texto | Sí | Requeridos. |
| Teléfono | Teléfono de contacto. | Texto corto | Sí | ValueObject. |
| Correo | Correo electrónico. | Correo | No | Opcional. |
| Dirección | Calle y ciudad. | Texto | Sí | ValueObject. |
| Estado activo | Indica si puede participar en viajes. | Booleano | Sí | Tiene filtro global. |

### Diccionario lógico: Paradas

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la parada. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre de la parada. | Texto | Sí | Requerido. |
| Sector | Sector donde está ubicada. | Relación | Sí | FK hacia Sectors. |
| Dirección | Calle y ciudad. | Texto | Sí | ValueObject. |
| Coordenadas | Latitud y longitud. | Decimal | Sí | ValueObject. |

## Rutas y asignaciones

### Diccionario lógico: Rutas

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la ruta. | Identificador único | Sí | Clave primaria. |
| Nombre | Nombre de la ruta. | Texto | Sí | Requerido. |
| Escuela | Centro educativo asociado. | Relación | Sí | FK hacia Schools. |
| Horario operativo | Hora de inicio y fin. | Hora | Sí | ValueObject TimeRange. |
| Estado | Estado de la ruta. | Catálogo/enum | Sí | Guardado como texto. |

### Diccionario lógico: Paradas de ruta

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la relación ruta-parada. | Identificador único | Sí | Clave primaria. |
| Ruta | Ruta asociada. | Relación | Sí | FK hacia Routes. |
| Parada | Parada asociada. | Relación | Sí | FK hacia Stops. |
| Orden | Orden de visita de la parada. | Número entero | Sí | Único por ruta. |

### Diccionario lógico: Asignaciones de ruta

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la asignación. | Identificador único | Sí | Clave primaria. |
| Ruta | Ruta asignada. | Relación | Sí | FK hacia Routes. |
| Vehículo | Vehículo asignado. | Relación | Sí | FK hacia Vehicles. |
| Conductor | Conductor asignado. | Relación | Sí | FK hacia Drivers. |
| Asistente | Asistente asignado. | Relación opcional | No | FK hacia TransportAssistants. |
| Capacidad | Capacidad autorizada para la asignación. | Número entero | Sí | No debe exceder vehículo. |

### Diccionario lógico: Estudiantes por asignación

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Estudiante | Estudiante asignado. | Relación | Sí | Parte de clave compuesta. |
| Asignación de ruta | Asignación relacionada. | Relación | Sí | Parte de clave compuesta. |

## Viajes y programación

### Diccionario lógico: Viajes

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del viaje. | Identificador único | Sí | Clave primaria. |
| Asignación de ruta | Asignación usada por el viaje. | Relación | Sí | FK hacia RouteAssignments. |
| Programación | Horario que originó el viaje. | Relación opcional | No | FK hacia TripSchedules. |
| Dirección | Ida o regreso escolar. | Catálogo/enum | No | Guardado como texto. |
| Fecha de operación | Día del viaje. | Fecha | No | Usada en materialización. |
| Hora programada | Salida/llegada programada. | Fecha y hora | No | Programación. |
| Hora real | Inicio/finalización real. | Fecha y hora | No | Operación. |
| Estado | Estado del viaje. | Catálogo/enum | Sí | Scheduled, InProgress, Completed, etc. |
| Razones | Cancelación o no operación. | Texto | No | Trazabilidad. |

### Diccionario lógico: Programaciones de viaje

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la programación. | Identificador único | Sí | Clave primaria. |
| Asignación | Asignación de ruta programada. | Relación | Sí | FK hacia RouteAssignments. |
| Dirección | Tipo de viaje programado. | Catálogo/enum | Sí | ToSchool/FromSchool. |
| Hora salida/llegada | Horario planificado. | Hora | Sí/No | Llegada es opcional. |
| Vigencia | Fecha inicial y final. | Fecha | Sí/No | ValidTo opcional. |
| Días activos | Lunes a domingo. | Booleano | Sí | Al menos un día activo. |
| Estado activo | Indica si la programación opera. | Booleano | Sí | Control de uso. |

### Diccionario lógico: Asistencia de pasajeros por viaje

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Viaje | Viaje relacionado. | Relación | Sí | Parte de clave compuesta. |
| Estudiante | Estudiante relacionado. | Relación | Sí | Parte de clave compuesta. |
| Snapshot de estudiante | Nombre y código histórico. | Texto | Sí | No cambia si cambia Student. |
| Snapshot de tutor | Tutor histórico del estudiante. | Texto/relación | No | Puede ser nulo. |
| Estado de asistencia | Expected, Boarded, Absent, DroppedOff. | Catálogo/enum | Sí | Guardado como texto. |
| Fechas operativas | Abordaje y entrega. | Fecha y hora | No | Según estado. |
| Notas | Observación del viaje. | Texto | No | Opcional. |

### Diccionario lógico: Días sin operación escolar

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del día sin operación. | Identificador único | Sí | Clave primaria. |
| Fecha | Fecha no operativa. | Fecha | Sí | Global o por escuela. |
| Escuela | Escuela afectada. | Relación opcional | No | Nulo significa global. |
| Tipo de razón | Motivo clasificado. | Catálogo/enum | Sí | Guardado como texto. |
| Razón | Justificación textual. | Texto | Sí | Requerida. |
| Estado activo | Indica si aplica. | Booleano | Sí | Control histórico. |

## Monitoreo GPS

### Diccionario lógico: Ubicaciones de vehículo

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la ubicación. | Identificador único | Sí | Clave primaria. |
| Viaje | Viaje monitoreado. | Relación | Sí | FK hacia Trips. |
| Vehículo | Vehículo monitoreado. | Relación | Sí | FK hacia Vehicles. |
| Latitud y longitud | Coordenadas GPS. | Decimal | Sí | Precisión configurada. |
| Velocidad | Velocidad reportada. | Decimal | No | Opcional. |
| Rumbo | Dirección del movimiento. | Decimal | No | Opcional. |
| Fecha de registro | Momento de captura. | Fecha y hora | Sí | Índice de consulta. |
| Usuario reporta | Usuario que envía ubicación. | Relación opcional | No | FK hacia Users. |

## Incidencias

### Diccionario lógico: Incidencias

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de incidencia. | Identificador único | Sí | Clave primaria. |
| Título y descripción | Contenido de la incidencia. | Texto | Sí | Requerido. |
| Tipo, severidad y estado | Clasificación operativa. | Catálogo/enum | Sí | Enum por convención. |
| Entidades relacionadas | Viaje, asignación, vehículo, conductor, asistente. | Relación opcional | No | Trazabilidad. |
| Usuario reporta | Usuario que creó la incidencia. | Relación | Sí | FK hacia Users. |
| Responsable/resolutor | Usuarios que gestionan la incidencia. | Relación opcional | No | FK hacia Users. |
| Fechas de cierre | Resolución y cierre. | Fecha y hora | No | Según estado. |

### Diccionario lógico: Comentarios de incidencia

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del comentario. | Identificador único | Sí | Clave primaria. |
| Incidencia | Incidencia comentada. | Relación | Sí | FK hacia Incidents. |
| Usuario | Autor del comentario. | Relación | Sí | FK hacia Users. |
| Comentario | Texto del seguimiento. | Texto | Sí | Requerido. |

## Notificaciones

### Diccionario lógico: Notificaciones

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de la notificación. | Identificador único | Sí | Clave primaria. |
| Usuario | Destinatario. | Relación | Sí | FK hacia Users. |
| Título y mensaje | Contenido de la notificación. | Texto | Sí | Requerido. |
| Tipo y prioridad | Clasificación de la notificación. | Catálogo/enum | Sí | Enum por convención. |
| Leída | Estado de lectura. | Booleano | Sí | Control de bandeja. |
| Entidad relacionada | Tipo e Id relacionados. | Texto | No | Trazabilidad. |

## Reportes/Auditoría

### Diccionario lógico: Auditoría

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del log. | Identificador único | Sí | Clave primaria. |
| Usuario | Usuario que realizó la acción. | Relación opcional | No | Puede ser nulo. |
| Acción | Acción ejecutada. | Texto corto | Sí | Ej.: Created, Updated. |
| Entidad | Entidad afectada. | Texto corto | Sí | Nombre e identificador. |
| Valores | Antes y después. | Texto largo | No | JSON o texto. |
| Contexto | IP y UserAgent. | Texto | No | Auditoría técnica. |

## Configuración

### Diccionario lógico: Configuraciones del sistema

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único de configuración. | Identificador único | Sí | Clave primaria. |
| Clave | Nombre único del parámetro. | Texto corto | Sí | Único. |
| Valor | Valor almacenado. | Texto | Sí | Interpretado por DataType. |
| Categoría | Grupo funcional. | Texto corto | Sí | General, GPS, Backup, etc. |
| Tipo de dato | Tipo esperado del valor. | Texto corto | Sí | String, Bool, Int, etc. |
| Editable | Indica si puede cambiarse. | Booleano | Sí | Protege settings base. |

## Backups

### Diccionario lógico: Registros de backup

| Campo lógico | Descripción | Tipo de dato conceptual | Obligatorio | Observación |
|---|---|---|---|---|
| Identificador | Código único del respaldo. | Identificador único | Sí | Clave primaria. |
| Archivo | Nombre y ruta del archivo. | Texto | Sí | Registro lógico. |
| Tamaño | Tamaño en bytes. | Número largo | No | Se asigna al completar. |
| Estado | Estado del proceso. | Catálogo/enum | Sí | Guardado como texto. |
| Tipo | Manual o automático. | Catálogo/enum | Sí | Guardado como texto. |
| Usuario creador | Usuario que solicitó el backup. | Relación lógica | Sí | Guid del usuario. |
| Fechas | Creación y finalización. | Fecha y hora | Sí/No | Finalización opcional. |
