# Documentacion tecnica de la API

Plataforma Web para el Sistema de Transporte Escolar TRAE. Backend ASP.NET Core 8 con Clean Architecture, DDD, CQRS, MediatR, EF Core, SQL Server y autenticacion JWT Bearer.

## Version vigente

Todas las rutas publicas de la API usan versionamiento por URL:

- Base local habitual: `http://localhost:5075`
- Prefijo obligatorio: `/api/v1`
- Swagger: `/swagger`
- Documento OpenAPI: `/swagger/v1/swagger.json`

Las rutas antiguas `/api/...` sin version ya no estan disponibles.

## Autenticacion

La API usa JWT Bearer. Los endpoints protegidos requieren:

```http
Authorization: Bearer {token}
```

Claims principales emitidos:

- `NameIdentifier`: identificador del usuario.
- `Name`: username.
- `Role`: rol principal.
- Claims de permisos cuando aplican.

### Login

`POST /api/v1/auth/login`

```json
{
  "username": "admin",
  "password": "Admin123"
}
```

Respuesta:

```json
{
  "token": "jwt-token",
  "userId": "00000000-0000-0000-0000-000000000000",
  "username": "admin",
  "name": "Admin",
  "profileImageUrl": null,
  "role": "Admin"
}
```

## Convenciones

- Paginacion comun: `PageNumber` y `PageSize`.
- Respuestas DTO manuales; no se exponen entidades de dominio.
- Errores controlados por middleware:
  - `400 Bad Request`: validacion o regla de dominio.
  - `401 Unauthorized`: token ausente, invalido o credenciales invalidas.
  - `403 Forbidden`: usuario autenticado sin permiso.
  - `404 Not Found`: recurso inexistente.
  - `500 Internal Server Error`: error no controlado.
- Cargas de foto de perfil: `multipart/form-data`, tipos `jpg`, `jpeg`, `png`, `webp`, maximo `5 MB`.

## Roles principales

- `Admin`: administracion completa.
- `Supervisor`: gestion operativa y reportes.
- `Driver`: operaciones sobre sus viajes asignados.
- `TransportAssistant`: asistencia, pasajeros y operacion sobre viajes asignados.
- `Guardian`: informacion de sus estudiantes, viajes, ubicacion y notificaciones relacionadas.

---

## Auth

| Metodo | Ruta | Auth | Roles | Descripcion |
|---|---|---:|---|---|
| POST | `/api/v1/auth/login` | No | Publico | Autentica usuario y devuelve JWT. |
| POST | `/api/v1/auth/forgot-password` | No | Publico | Recuperacion simple de contrasena. |

## Me / Perfil

| Metodo | Ruta | Auth | Roles | Descripcion |
|---|---|---:|---|---|
| GET | `/api/v1/me` | Si | Todos | Perfil del usuario autenticado. |
| GET | `/api/v1/me/students` | Si | Guardian/Admin/Supervisor segun visibilidad | Estudiantes vinculados al usuario. |
| GET | `/api/v1/me/route-assignments` | Si | Driver/TransportAssistant/Guardian/Admin/Supervisor | Asignaciones visibles para el usuario. |
| GET | `/api/v1/me/trips` | Si | Driver/TransportAssistant/Guardian/Admin/Supervisor | Viajes visibles para el usuario. |
| PUT | `/api/v1/me/profile` | Si | Todos | Actualiza nombre, email y URL de foto. |
| PUT | `/api/v1/me/change-password` | Si | Todos | Cambia contrasena. |
| PUT | `/api/v1/me/photo` | Si | Todos | Actualiza URL de foto de perfil. |
| POST | `/api/v1/me/photo/upload` | Si | Todos | Sube foto real y actualiza perfil. Maximo 5 MB. |

## Usuarios, roles y permisos

| Metodo | Ruta | Auth | Roles | Descripcion |
|---|---|---:|---|---|
| POST | `/api/v1/users` | Si | Admin | Crea usuario administrativo manual. |
| GET | `/api/v1/users` | Si | Admin, Supervisor | Lista usuarios paginados. |
| GET | `/api/v1/users/{id}` | Si | Admin, Supervisor | Detalle de usuario. |
| GET | `/api/v1/users/by-role/{roleId}` | Si | Admin, Supervisor | Usuarios por rol. |
| GET | `/api/v1/users/by-active/{isActive}` | Si | Admin, Supervisor | Usuarios por estado. |
| PUT | `/api/v1/users/{id}` | Si | Admin, Supervisor | Actualiza usuario. |
| PUT | `/api/v1/users/{id}/activate` | Si | Admin, Supervisor | Activa usuario. |
| PUT | `/api/v1/users/{id}/deactivate` | Si | Admin, Supervisor | Desactiva usuario. |
| PUT | `/api/v1/users/{id}/reset-password` | Si | Admin, Supervisor | Reinicia contrasena. |
| GET | `/api/v1/roles` | Si | Admin | Lista roles. |
| GET | `/api/v1/roles/{id}` | Si | Admin | Detalle de rol. |
| PUT | `/api/v1/roles/{id}` | Si | Admin | Actualiza rol. |
| POST | `/api/v1/roles/{roleId}/permissions/{permissionId}` | Si | Admin | Asigna permiso a rol. |
| DELETE | `/api/v1/roles/{roleId}/permissions/{permissionId}` | Si | Admin | Remueve permiso del rol. |
| GET | `/api/v1/permissions` | Si | Admin, Supervisor | Lista permisos. |
| GET | `/api/v1/permissions/{id}` | Si | Admin, Supervisor | Detalle de permiso. |

## Gestion academica

### Sectores

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/sectors` | Admin, Supervisor | Crea sector. |
| GET | `/api/v1/sectors` | Admin, Supervisor | Lista sectores. |
| GET | `/api/v1/sectors/{id}` | Admin, Supervisor | Detalle de sector. |
| PUT | `/api/v1/sectors/{id}` | Admin, Supervisor | Actualiza sector. |
| DELETE | `/api/v1/sectors/{id}` | Admin, Supervisor | Desactiva/elimina sector segun regla. |

### Escuelas

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/schools` | Admin, Supervisor | Crea escuela. |
| GET | `/api/v1/schools` | Admin, Supervisor | Lista escuelas. |
| GET | `/api/v1/schools/{id}` | Admin, Supervisor | Detalle de escuela. |
| GET | `/api/v1/schools/by-sector/{sectorId}` | Admin, Supervisor | Escuelas por sector. |
| PUT | `/api/v1/schools/{id}` | Admin, Supervisor | Actualiza escuela. |
| DELETE | `/api/v1/schools/{id}` | Admin, Supervisor | Desactiva escuela si no tiene dependencias activas. |

### Grados

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/grades` | Admin, Supervisor | Crea grado. |
| GET | `/api/v1/grades` | Admin, Supervisor | Lista grados. |
| GET | `/api/v1/grades/{id}` | Admin, Supervisor | Detalle de grado. |
| GET | `/api/v1/grades/by-school/{schoolId}` | Admin, Supervisor | Grados por escuela. |
| PUT | `/api/v1/grades/{id}` | Admin, Supervisor | Actualiza grado. |
| DELETE | `/api/v1/grades/{id}` | Admin, Supervisor | Elimina/desactiva grado si no tiene estudiantes activos. |

### Tutores

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/guardians` | Admin, Supervisor | Crea tutor y usuario vinculado. |
| GET | `/api/v1/guardians` | Admin, Supervisor | Lista tutores. |
| GET | `/api/v1/guardians/{id}` | Admin, Supervisor | Detalle de tutor. |
| GET | `/api/v1/guardians/document/{documentNumber}` | Admin, Supervisor | Tutor por documento. |
| PUT | `/api/v1/guardians/{id}` | Admin, Supervisor | Actualiza tutor y sincroniza usuario si cambia documento. |
| DELETE | `/api/v1/guardians/{id}` | Admin, Supervisor | Desactiva tutor si no tiene estudiantes activos. |

### Estudiantes

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/students` | Admin, Supervisor | Crea estudiante. |
| GET | `/api/v1/students` | Admin, Supervisor | Lista estudiantes. |
| GET | `/api/v1/students/{id}` | Admin, Supervisor | Detalle de estudiante. |
| GET | `/api/v1/students/code/{code}` | Admin, Supervisor | Estudiante por codigo. |
| GET | `/api/v1/students/by-grade/{gradeId}` | Admin, Supervisor | Estudiantes por grado. |
| GET | `/api/v1/students/by-school/{schoolId}` | Admin, Supervisor | Estudiantes por escuela. |
| GET | `/api/v1/students/by-guardian/{guardianId}` | Admin, Supervisor | Estudiantes por tutor. |
| GET | `/api/v1/students/{studentId}/attendance-history` | Si | Admin, Supervisor, Guardian, Driver, TransportAssistant | Historial de asistencia visible segun reglas. |
| PUT | `/api/v1/students/{id}` | Admin, Supervisor | Actualiza estudiante. |
| DELETE | `/api/v1/students/{id}` | Admin, Supervisor | Desactiva estudiante si no tiene viaje/asignacion activa. |

## Gestion de transporte

### Vehiculos

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/vehicles` | Admin, Supervisor | Crea vehiculo. |
| GET | `/api/v1/vehicles` | Admin, Supervisor | Lista vehiculos. |
| GET | `/api/v1/vehicles/{id}` | Admin, Supervisor | Detalle de vehiculo. |
| GET | `/api/v1/vehicles/plate/{plateNumber}` | Admin, Supervisor | Vehiculo por placa. |
| GET | `/api/v1/vehicles/by-status/{status}` | Admin, Supervisor | Vehiculos por estado. |
| PUT | `/api/v1/vehicles/{id}` | Admin, Supervisor | Actualiza vehiculo. |
| DELETE | `/api/v1/vehicles/{id}` | Admin, Supervisor | Desactiva vehiculo si no tiene operaciones activas. |

### Conductores

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/drivers` | Admin, Supervisor | Crea conductor y usuario vinculado. |
| GET | `/api/v1/drivers` | Admin, Supervisor | Lista conductores. |
| GET | `/api/v1/drivers/{id}` | Admin, Supervisor | Detalle de conductor. |
| GET | `/api/v1/drivers/license/{licenseNumber}` | Admin, Supervisor | Conductor por licencia. |
| GET | `/api/v1/drivers/by-active/{isActive}` | Admin, Supervisor | Conductores por estado. |
| PUT | `/api/v1/drivers/{id}` | Admin, Supervisor | Actualiza conductor y sincroniza usuario si cambia documento. |
| DELETE | `/api/v1/drivers/{id}` | Admin, Supervisor | Desactiva conductor si no tiene operaciones activas. |

### Asistentes de transporte

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/transport-assistants` | Admin, Supervisor | Crea asistente y usuario vinculado. |
| GET | `/api/v1/transport-assistants` | Admin, Supervisor | Lista asistentes. |
| GET | `/api/v1/transport-assistants/{id}` | Admin, Supervisor | Detalle de asistente. |
| GET | `/api/v1/transport-assistants/document/{documentNumber}` | Admin, Supervisor | Asistente por documento. |
| GET | `/api/v1/transport-assistants/by-active/{isActive}` | Admin, Supervisor | Asistentes por estado. |
| PUT | `/api/v1/transport-assistants/{id}` | Admin, Supervisor | Actualiza asistente y sincroniza usuario si cambia documento. |
| DELETE | `/api/v1/transport-assistants/{id}` | Admin, Supervisor | Desactiva asistente si no tiene operaciones activas. |

### Paradas

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/stops` | Admin, Supervisor | Crea parada. |
| GET | `/api/v1/stops` | Admin, Supervisor | Lista paradas. |
| GET | `/api/v1/stops/{id}` | Admin, Supervisor | Detalle de parada. |
| GET | `/api/v1/stops/by-sector/{sectorId}` | Admin, Supervisor | Paradas por sector. |
| GET | `/api/v1/stops/by-city/{city}` | Admin, Supervisor | Paradas por ciudad. |
| PUT | `/api/v1/stops/{id}` | Admin, Supervisor | Actualiza parada. |
| DELETE | `/api/v1/stops/{id}` | Admin, Supervisor | Elimina/desactiva parada. |

## Rutas y asignaciones

### Rutas

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/routes` | Admin, Supervisor | Crea ruta. |
| GET | `/api/v1/routes` | Admin, Supervisor | Lista rutas. |
| GET | `/api/v1/routes/{id}` | Admin, Supervisor | Detalle de ruta. |
| GET | `/api/v1/routes/by-school/{schoolId}` | Admin, Supervisor | Rutas por escuela. |
| GET | `/api/v1/routes/by-status/{status}` | Admin, Supervisor | Rutas por estado. |
| PUT | `/api/v1/routes/{id}` | Admin, Supervisor | Actualiza ruta. |
| DELETE | `/api/v1/routes/{id}` | Admin, Supervisor | Desactiva ruta si no tiene viaje activo. |
| POST | `/api/v1/routes/{routeId}/stops` | Admin, Supervisor | Agrega parada a ruta. |
| DELETE | `/api/v1/routes/{routeId}/stops/{stopId}` | Admin, Supervisor | Remueve parada de ruta. |
| PUT | `/api/v1/routes/{routeId}/stops/{stopId}/order` | Admin, Supervisor | Cambia orden de parada. |

### Asignaciones de ruta

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/route-assignments` | Admin, Supervisor | Crea asignacion de ruta, conductor, vehiculo y asistente opcional. |
| GET | `/api/v1/route-assignments` | Admin, Supervisor | Lista asignaciones. |
| GET | `/api/v1/route-assignments/{id}` | Admin, Supervisor | Detalle de asignacion. |
| GET | `/api/v1/route-assignments/by-route/{routeId}` | Admin, Supervisor | Asignaciones por ruta. |
| GET | `/api/v1/route-assignments/by-driver/{driverId}` | Admin, Supervisor | Asignaciones por conductor. |
| GET | `/api/v1/route-assignments/by-vehicle/{vehicleId}` | Admin, Supervisor | Asignaciones por vehiculo. |
| PUT | `/api/v1/route-assignments/{id}` | Admin, Supervisor | Actualiza asignacion si no tiene viajes. |
| DELETE | `/api/v1/route-assignments/{id}` | Admin, Supervisor | Elimina si no tiene viajes asociados. |
| POST | `/api/v1/route-assignments/{assignmentId}/students/{studentId}` | Admin, Supervisor | Asigna estudiante respetando capacidad y reglas. |
| DELETE | `/api/v1/route-assignments/{assignmentId}/students/{studentId}` | Admin, Supervisor | Remueve estudiante si no hay viaje en progreso. |

## Viajes, programacion y asistencia

### Programacion de viajes

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| GET | `/api/v1/trip-schedules` | Admin, Supervisor | Lista horarios de viaje. |
| GET | `/api/v1/trip-schedules/{id}` | Admin, Supervisor | Detalle de horario. |
| GET | `/api/v1/trip-schedules/by-assignment/{routeAssignmentId}` | Admin, Supervisor | Horarios por asignacion. |
| GET | `/api/v1/trip-schedules/by-assignment/{routeAssignmentId}/active` | Admin, Supervisor | Horarios activos por asignacion. |
| POST | `/api/v1/trip-schedules` | Admin, Supervisor | Crea horario. |
| POST | `/api/v1/trip-schedules/{id}/materialize` | Admin, Supervisor | Crea viaje Scheduled o NotOperating para una fecha. |
| PUT | `/api/v1/trip-schedules/{id}` | Admin, Supervisor | Actualiza horario. |
| PUT | `/api/v1/trip-schedules/{id}/activate` | Admin, Supervisor | Activa horario. |
| PUT | `/api/v1/trip-schedules/{id}/deactivate` | Admin, Supervisor | Desactiva horario. |
| DELETE | `/api/v1/trip-schedules/{id}` | Admin, Supervisor | Elimina o desactiva si tiene viajes. |

### Dias sin clase / no operacion

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| GET | `/api/v1/non-school-days` | Admin, Supervisor | Lista dias no escolares. |
| GET | `/api/v1/non-school-days/{id}` | Admin, Supervisor | Detalle de dia no escolar. |
| GET | `/api/v1/non-school-days/by-date-range` | Admin, Supervisor | Consulta por rango de fechas. |
| GET | `/api/v1/non-school-days/active?date=yyyy-MM-dd&schoolId={id}` | Admin, Supervisor | Valida dia activo global o por escuela. |
| POST | `/api/v1/non-school-days` | Admin, Supervisor | Crea dia no escolar. |
| PUT | `/api/v1/non-school-days/{id}` | Admin, Supervisor | Actualiza dia no escolar. |
| PUT | `/api/v1/non-school-days/{id}/deactivate` | Admin, Supervisor | Desactiva dia no escolar. |
| DELETE | `/api/v1/non-school-days/{id}` | Admin, Supervisor | Desactiva/elimina segun regla. |

### Viajes

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/trips/start` | Admin, Supervisor, Driver, TransportAssistant | Inicia viaje y crea snapshot de pasajeros. |
| PUT | `/api/v1/trips/{id}/end` | Admin, Supervisor, Driver, TransportAssistant | Finaliza viaje en progreso. |
| PUT | `/api/v1/trips/{id}/cancel` | Admin, Supervisor | Cancela viaje programado con razon. |
| PUT | `/api/v1/trips/{id}/not-operating` | Admin, Supervisor | Marca viaje programado como no operativo. |
| GET | `/api/v1/trips` | Admin, Supervisor | Lista viajes. |
| GET | `/api/v1/trips/{id}` | Si | Segun visibilidad | Detalle de viaje. |
| GET | `/api/v1/trips/by-route-assignment/{routeAssignmentId}` | Admin, Supervisor | Viajes por asignacion. |
| GET | `/api/v1/trips/by-status/{status}` | Admin, Supervisor | Viajes por estado. |
| GET | `/api/v1/trips/by-date-range` | Admin, Supervisor | Viajes por rango de fechas. |
| GET | `/api/v1/trips/active/by-route-assignment/{routeAssignmentId}` | Admin, Supervisor | Viaje activo por asignacion. |

### Pasajeros y asistencia

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| GET | `/api/v1/trips/{tripId}/passengers` | Si | Admin, Supervisor, Driver, TransportAssistant, Guardian segun visibilidad | Pasajeros snapshot del viaje. |
| GET | `/api/v1/trips/{tripId}/attendance` | Si | Admin, Supervisor, Driver, TransportAssistant, Guardian segun visibilidad | Resumen de pase de lista. |
| GET | `/api/v1/trips/{tripId}/attendance/search-students?query=` | Si | Admin, Supervisor, Driver, TransportAssistant | Busca estudiantes para asistencia. |
| POST | `/api/v1/trips/{tripId}/attendance/mark-boarded` | Si | Admin, Supervisor, Driver, TransportAssistant | Marca abordado o registra excepcional con razon. |
| POST | `/api/v1/trips/{tripId}/attendance/mark-absent` | Si | Admin, Supervisor, Driver, TransportAssistant | Marca ausente. |
| POST | `/api/v1/trips/{tripId}/attendance/mark-dropped-off` | Si | Admin, Supervisor, Driver, TransportAssistant | Marca descendido. |
| POST | `/api/v1/trips/{tripId}/attendance/close` | Si | Admin, Supervisor, Driver, TransportAssistant | Cierra pase de lista y marca ausentes esperados. |
| POST | `/api/v1/trips/{tripId}/passengers/exceptional` | Si | Admin, Supervisor, Driver, TransportAssistant | Agrega pasajero excepcional. |
| PUT | `/api/v1/trips/{tripId}/students/{studentId}/boarded` | Si | Admin, Supervisor, Driver, TransportAssistant | Flujo legacy para marcar abordado. |
| PUT | `/api/v1/trips/{tripId}/students/{studentId}/absent` | Si | Admin, Supervisor, Driver, TransportAssistant | Flujo legacy para marcar ausente. |
| PUT | `/api/v1/trips/{tripId}/students/{studentId}/dropped-off` | Si | Admin, Supervisor, Driver, TransportAssistant | Flujo legacy para marcar descendido. |
| PUT | `/api/v1/trips/{tripId}/students/{studentId}/notes` | Si | Admin, Supervisor, Driver, TransportAssistant | Actualiza notas de asistencia. |

### Desvios de ruta

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/trips/{tripId}/route-deviations` | Si | Admin, Supervisor, Driver, TransportAssistant | Reporta desvio en viaje InProgress. |
| GET | `/api/v1/trips/{tripId}/route-deviations` | Si | Admin, Supervisor, Driver, TransportAssistant | Lista desvios de un viaje. |
| GET | `/api/v1/trips/route-deviations/{id}` | Si | Admin, Supervisor, Driver, TransportAssistant | Detalle de desvio. |

## Tracking GPS

| Metodo | Ruta | Auth | Roles | Descripcion |
|---|---|---:|---|---|
| POST | `/api/v1/tracking/location` | Si | Admin, Supervisor, Driver, TransportAssistant | Registra ubicacion de viaje en progreso. |
| GET | `/api/v1/tracking/trips/{tripId}/current-location` | Si | Segun visibilidad | Ultima ubicacion de viaje. |
| GET | `/api/v1/tracking/trips/{tripId}/history` | Si | Segun visibilidad | Historial GPS. |
| GET | `/api/v1/tracking/active-trips` | Si | Admin, Supervisor, Driver, TransportAssistant | Ubicaciones de viajes activos visibles. |
| GET | `/api/v1/tracking/my-students` | Si | Guardian | Ubicacion de viajes de sus estudiantes. |

## Incidencias

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| POST | `/api/v1/incidents` | Todos autenticados segun visibilidad | Reporta incidencia. |
| GET | `/api/v1/incidents` | Admin, Supervisor | Lista incidencias. |
| GET | `/api/v1/incidents/{id}` | Segun visibilidad | Detalle de incidencia. |
| GET | `/api/v1/incidents/by-status/{status}` | Admin, Supervisor | Incidencias por estado. |
| GET | `/api/v1/incidents/by-severity/{severity}` | Admin, Supervisor | Incidencias por severidad. |
| GET | `/api/v1/incidents/by-trip/{tripId}` | Segun visibilidad | Incidencias por viaje. |
| GET | `/api/v1/incidents/by-route-assignment/{routeAssignmentId}` | Admin, Supervisor | Incidencias por asignacion. |
| GET | `/api/v1/incidents/my-reported` | Todos autenticados | Incidencias reportadas por el usuario. |
| PUT | `/api/v1/incidents/{id}/assign/{userId}` | Admin, Supervisor | Asigna responsable. |
| PUT | `/api/v1/incidents/{id}/in-progress` | Admin, Supervisor o asignado segun handler | Marca en progreso. |
| PUT | `/api/v1/incidents/{id}/resolve` | Admin, Supervisor o asignado | Resuelve incidencia. |
| PUT | `/api/v1/incidents/{id}/close` | Admin, Supervisor | Cierra incidencia resuelta. |
| PUT | `/api/v1/incidents/{id}/cancel` | Admin, Supervisor | Cancela incidencia. |
| POST | `/api/v1/incidents/{id}/comments` | Todos autenticados segun visibilidad | Agrega comentario. |

## Notificaciones

| Metodo | Ruta | Auth | Roles | Descripcion |
|---|---|---:|---|---|
| GET | `/api/v1/notifications` | Si | Todos | Lista notificaciones visibles. |
| GET | `/api/v1/notifications/unread` | Si | Todos | Notificaciones no leidas. |
| PUT | `/api/v1/notifications/{id}/read` | Si | Todos segun ownership | Marca una como leida. |
| PUT | `/api/v1/notifications/read-all` | Si | Todos | Marca todas como leidas. |

## Reportes

Todos requieren `Admin` o `Supervisor`.

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/v1/reports/dashboard` | Dashboard general. |
| GET | `/api/v1/reports/trips?startDate=&endDate=` | Reporte de viajes. |
| GET | `/api/v1/reports/students-by-route/{routeId}` | Estudiantes por ruta. |
| GET | `/api/v1/reports/incidents?startDate=&endDate=` | Reporte de incidencias. |
| GET | `/api/v1/reports/drivers-performance?startDate=&endDate=` | Desempeno de conductores. |
| GET | `/api/v1/reports/vehicles-usage?startDate=&endDate=` | Uso de vehiculos. |
| GET | `/api/v1/reports/audit-summary?startDate=&endDate=` | Resumen de auditoria. |
| GET | `/api/v1/reports/attendance/low-presence?startDate=&endDate=&maximumPresencePercentage=` | Estudiantes con baja presencia. |

## Auditoria

Todos requieren `Admin` o `Supervisor`.

| Metodo | Ruta | Descripcion |
|---|---|---|
| GET | `/api/v1/audit-logs` | Lista logs de auditoria. |
| GET | `/api/v1/audit-logs/by-user/{userId}` | Auditoria por usuario. |
| GET | `/api/v1/audit-logs/by-entity/{entityName}/{entityId}` | Auditoria por entidad. |
| GET | `/api/v1/audit-logs/by-action/{action}` | Auditoria por accion. |
| GET | `/api/v1/audit-logs/by-date-range?startDate=&endDate=` | Auditoria por rango. |

## Configuracion del sistema

| Metodo | Ruta | Roles | Descripcion |
|---|---|---|---|
| GET | `/api/v1/system-settings` | Admin, Supervisor | Lista configuraciones. |
| GET | `/api/v1/system-settings/{id}` | Admin, Supervisor | Configuracion por id. |
| GET | `/api/v1/system-settings/key/{key}` | Admin, Supervisor | Configuracion por clave. |
| GET | `/api/v1/system-settings/by-category/{category}` | Admin, Supervisor | Configuraciones por categoria. |
| POST | `/api/v1/system-settings` | Admin | Crea configuracion. |
| PUT | `/api/v1/system-settings/{id}` | Admin | Actualiza configuracion editable. |
| DELETE | `/api/v1/system-settings/{id}` | Admin | Elimina configuracion no base. |

## Integraciones externas mock/local

Todos requieren `Admin`.

| Metodo | Ruta | Descripcion |
|---|---|---|
| POST | `/api/v1/integrations/test-email` | Prueba servicio mock de email. |
| POST | `/api/v1/integrations/test-sms` | Prueba servicio mock de SMS. |
| POST | `/api/v1/integrations/test-whatsapp` | Prueba servicio mock de WhatsApp. |
| POST | `/api/v1/integrations/test-push` | Prueba servicio mock de push. |
| GET | `/api/v1/integrations/test-distance?originLat=&originLng=&destinationLat=&destinationLng=` | Calcula distancia aproximada. |

## Backups

Todos requieren `Admin`.

| Metodo | Ruta | Descripcion |
|---|---|---|
| POST | `/api/v1/backups/manual` | Crea backup logico seguro. |
| GET | `/api/v1/backups` | Historial paginado. |
| GET | `/api/v1/backups/latest` | Ultimo backup. |
| GET | `/api/v1/backups/{id}` | Detalle de backup. |
| POST | `/api/v1/backups/{id}/restore` | Placeholder: restore no implementado en esta fase. |

## Ejemplos rapidos

### Subir foto de perfil

`POST /api/v1/me/photo/upload`

Content-Type: `multipart/form-data`

Campo:

- `file`: imagen `jpg`, `jpeg`, `png` o `webp`, maximo `5 MB`.

Respuesta:

```json
{
  "profileImageUrl": "/uploads/profile-photos/archivo.png"
}
```

### Materializar viaje

`POST /api/v1/trip-schedules/{id}/materialize`

```json
{
  "operationDate": "2026-06-08"
}
```

### Marcar asistencia

`POST /api/v1/trips/{tripId}/attendance/mark-boarded`

```json
{
  "studentId": "00000000-0000-0000-0000-000000000000",
  "exceptionReason": "Motivo obligatorio solo si no pertenece a la ruta"
}
```

## Observaciones de mantenimiento

- Si se agregan controllers o rutas, actualizar este documento y los smoke tests correspondientes.
- La fuente de verdad tecnica sigue siendo Swagger en `/swagger/v1/swagger.json`.
- Las reglas de visibilidad finales se aplican en controllers y handlers; el frontend no debe confiar solo en ocultar rutas.
