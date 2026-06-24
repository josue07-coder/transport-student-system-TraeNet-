# Backend Endpoint Security Audit

Proyecto: Plataforma Web para el Sistema de Transporte Escolar TRAE  
Fase: QA-1 - Auditoria backend de seguridad, autorizacion y exposicion de endpoints  
Fecha: 2026-06-21

## Resumen ejecutivo

La auditoria reviso los controllers de la API para verificar que la seguridad real del backend no dependa del frontend. Se detectaron endpoints generales de consulta y administracion que estaban protegidos solo con `[Authorize]` o sin restriccion de rol explicita, permitiendo que usuarios operativos como Driver, TransportAssistant o Guardian accedieran a listados administrativos.

Se endurecieron los endpoints generales de administracion, catalogos, rutas, transporte, asignaciones, viajes globales e incidencias globales con restricciones por rol. Los flujos operativos se conservaron mediante endpoints especificos y validaciones de ownership en handlers, por ejemplo `/api/me/trips`, `/api/me/students`, tracking por tutor, pasajeros de viaje y operaciones de viaje asignado.

No se agregaron funcionalidades nuevas, no se modifico frontend y no se crearon migraciones.

## Reglas aplicadas

- Admin: acceso administrativo completo.
- Supervisor: gestion operativa, educacion, transporte, rutas, asignaciones, horarios, viajes, incidencias, reportes y notificaciones.
- Driver: solo viajes asignados, GPS, inicio/finalizacion, incidencias y desvios de sus viajes.
- TransportAssistant: solo viajes asignados, pasajeros/asistencia, pasajeros excepcionales, GPS, incidencias y desvios de sus viajes. No cancela viajes ni marca no operacion.
- Guardian: solo sus estudiantes, viajes/ubicacion relacionados y notificaciones propias.

## Hallazgos principales

1. `StudentsController` y `RoutesController` permitian listados generales a cualquier usuario autenticado. Se restringieron a `Admin,Supervisor`.
2. `SchoolsController`, `GradesController`, `SectorsController` y `StopsController` no tenian autorizacion por rol en el controller. Se restringieron a `Admin,Supervisor`.
3. `VehiclesController`, `DriversController`, `TransportAssistantsController`, `GuardiansController` y `RouteAssignmentsController` estaban demasiado amplios para roles operativos. Se restringieron a `Admin,Supervisor`.
4. `TripsController` exponia listados/filtros globales a cualquier autenticado. Se restringieron listados globales a `Admin,Supervisor` y se mantuvieron endpoints operativos bajo validacion de ownership.
5. Mutaciones de asistencia de pasajeros no tenian restriccion de rol en controller. Se limitaron a `Admin,Supervisor,Driver,TransportAssistant`; Guardian queda bloqueado antes de llegar al handler.
6. `IncidentsController` exponia consultas globales por estado, severidad y asignacion. Se restringieron a `Admin,Supervisor`; crear y consultar incidentes propios/relacionados queda autenticado y validado en handlers.

## Matriz de endpoints auditados

| Controller | Endpoint | Metodo HTTP | Roles actuales | Roles recomendados | Riesgo | Accion aplicada |
|---|---:|---:|---|---|---|---|
| AuthController | `/api/auth/login` | POST | Publico | Publico | Bajo | Sin cambios |
| AuthController | `/api/auth/forgot-password` | POST | Publico | Publico | Medio | Sin cambios; flujo publico esperado |
| MeController | `/api/me` | GET | Autenticado | Autenticado | Bajo | Sin cambios; datos del usuario actual |
| MeController | `/api/me/profile` | PUT | Autenticado | Autenticado | Bajo | Sin cambios; usuario actual |
| MeController | `/api/me/change-password` | PUT | Autenticado | Autenticado | Bajo | Sin cambios |
| MeController | `/api/me/photo` | PUT | Autenticado | Autenticado | Bajo | Sin cambios |
| MeController | `/api/me/photo/upload` | POST | Autenticado | Autenticado | Bajo | Sin cambios; usuario actual |
| MeController | `/api/me/students` | GET | Autenticado | Guardian/Admin/Supervisor segun handler | Medio | Sin cambios; handler limita visibilidad |
| MeController | `/api/me/route-assignments` | GET | Autenticado | Driver/Assistant/Guardian/Admin/Supervisor segun handler | Medio | Sin cambios; handler limita visibilidad |
| MeController | `/api/me/trips` | GET | Autenticado | Driver/Assistant/Guardian/Admin/Supervisor segun handler | Medio | Sin cambios; handler limita visibilidad |
| UsersController | `/api/users` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| UsersController | `/api/users` | POST | Admin | Admin | Bajo | Verificado |
| UsersController | `/api/users/{id}` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| UsersController | `/api/users/by-role/{roleId}` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| UsersController | `/api/users/by-active/{isActive}` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| UsersController | `/api/users/{id}` | PUT | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| UsersController | `/api/users/{id}/activate` | PUT | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| UsersController | `/api/users/{id}/deactivate` | PUT | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| UsersController | `/api/users/{id}/reset-password` | PUT | Admin,Supervisor | Admin,Supervisor | Alto | Verificado |
| RolesController | `/api/roles` | GET | Admin | Admin | Bajo | Verificado |
| RolesController | `/api/roles/{id}` | GET | Admin | Admin | Bajo | Verificado |
| RolesController | `/api/roles/{id}` | PUT | Admin | Admin | Medio | Verificado |
| RolesController | `/api/roles/{roleId}/permissions/{permissionId}` | POST/DELETE | Admin | Admin | Alto | Verificado |
| PermissionsController | `/api/permissions` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| PermissionsController | `/api/permissions/{id}` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| StudentsController | `/api/students` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| StudentsController | `/api/students/{id}` | GET | Admin,Supervisor | Admin,Supervisor; Guardian por `/api/me/students` | Alto | Endurecido en controller |
| StudentsController | `/api/students/code/{code}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| StudentsController | `/api/students/by-grade/{gradeId}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| StudentsController | `/api/students/by-school/{schoolId}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| StudentsController | `/api/students/by-guardian/{guardianId}` | GET | Admin,Supervisor | Admin,Supervisor; Guardian por `/api/me/students` | Alto | Endurecido en controller |
| StudentsController | `/api/students` | POST | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| StudentsController | `/api/students/{id}` | PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| GuardiansController | `/api/guardians*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| SchoolsController | `/api/schools*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| GradesController | `/api/grades*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| SectorsController | `/api/sectors*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| SchoolDistrictsController | No encontrado | N/A | N/A | N/A | Bajo | No existe controller en API actual |
| VehiclesController | `/api/vehicles*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| DriversController | `/api/drivers*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| TransportAssistantsController | `/api/transport-assistants*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| StopsController | `/api/stops*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| RoutesController | `/api/routes` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| RoutesController | `/api/routes/{id}` | GET | Admin,Supervisor | Admin,Supervisor; Driver/Assistant por `/api/me/trips` | Alto | Endurecido en controller |
| RoutesController | `/api/routes/by-school/{schoolId}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| RoutesController | `/api/routes/by-status/{status}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido en controller |
| RoutesController | `/api/routes` | POST | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| RoutesController | `/api/routes/{id}` | PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| RoutesController | `/api/routes/{routeId}/stops*` | POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| RouteAssignmentsController | `/api/route-assignments*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor; operativos por `/api/me/trips` | Alto | Endurecido en controller |
| RouteAssignmentsController | `/api/route-assignments/{assignmentId}/students/{studentId}` | POST/DELETE | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| TripsController | `/api/trips` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| TripsController | `/api/trips/{id}` | GET | Autenticado + handler | Admin/Supervisor o usuario relacionado | Medio | Sin cambios; ownership en handler |
| TripsController | `/api/trips/by-route-assignment/{routeAssignmentId}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| TripsController | `/api/trips/by-status/{status}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| TripsController | `/api/trips/by-date-range` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| TripsController | `/api/trips/active/by-route-assignment/{routeAssignmentId}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| TripsController | `/api/trips/start` | POST | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Medio | Endurecido por metodo; ownership en handler |
| TripsController | `/api/trips/{id}/end` | PUT | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Medio | Endurecido por metodo; ownership en handler |
| TripsController | `/api/trips/{id}/cancel` | PUT | Admin,Supervisor | Admin,Supervisor | Alto | Verificado; Assistant bloqueado |
| TripsController | `/api/trips/{id}/not-operating` | PUT | Admin,Supervisor | Admin,Supervisor | Alto | Verificado |
| TripsController | `/api/trips/{tripId}/passengers` | GET | Autenticado + handler | Usuario relacionado | Medio | Sin cambios; ownership en handler |
| TripsController | `/api/trips/{tripId}/students/{studentId}/boarded` | PUT | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Alto | Endurecido por metodo; Guardian bloqueado |
| TripsController | `/api/trips/{tripId}/students/{studentId}/absent` | PUT | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Alto | Endurecido por metodo |
| TripsController | `/api/trips/{tripId}/students/{studentId}/dropped-off` | PUT | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Alto | Endurecido por metodo |
| TripsController | `/api/trips/{tripId}/students/{studentId}/notes` | PUT | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Medio | Endurecido por metodo |
| TripsController | `/api/trips/{tripId}/passengers/exceptional` | POST | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Medio | Verificado; ownership en handler |
| TripsController | `/api/trips/{tripId}/route-deviations` | POST/GET | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Medio | Verificado; ownership en handler |
| TripsController | `/api/trips/route-deviations/{id}` | GET | Admin,Supervisor,Driver,TransportAssistant | Admin,Supervisor,Driver asignado, Assistant asignado | Medio | Verificado; ownership en handler |
| TripSchedulesController | `/api/trip-schedules*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| TripSchedulesController | `/api/trip-schedules/{id}/materialize` | POST | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| NonSchoolDaysController | `/api/non-school-days*` | GET/POST/PUT/DELETE | Admin,Supervisor | Admin,Supervisor | Medio | Verificado |
| TrackingController | `/api/tracking/location` | POST | Autenticado + handler | Admin/Supervisor/Driver/Assistant asignado | Medio | Sin cambios; ownership en handler |
| TrackingController | `/api/tracking/trips/{tripId}/current-location` | GET | Autenticado + handler | Usuario relacionado | Medio | Sin cambios; ownership en handler |
| TrackingController | `/api/tracking/trips/{tripId}/history` | GET | Autenticado + handler | Usuario relacionado | Medio | Sin cambios; ownership en handler |
| TrackingController | `/api/tracking/active-trips` | GET | Autenticado + handler | Admin/Supervisor; operativos segun handler | Medio | Sin cambios; handler controla visibilidad |
| TrackingController | `/api/tracking/my-students` | GET | Autenticado + handler | Guardian | Bajo | Sin cambios; endpoint especifico del tutor |
| IncidentsController | `/api/incidents` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| IncidentsController | `/api/incidents` | POST | Autenticado + handler | Usuario relacionado | Medio | Sin cambios; reportes operativos |
| IncidentsController | `/api/incidents/{id}` | GET | Autenticado + handler | Usuario relacionado | Medio | Sin cambios; ownership en handler |
| IncidentsController | `/api/incidents/by-status/{status}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| IncidentsController | `/api/incidents/by-severity/{severity}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| IncidentsController | `/api/incidents/by-trip/{tripId}` | GET | Autenticado + handler | Usuario relacionado | Medio | Sin cambios; ownership en handler |
| IncidentsController | `/api/incidents/by-route-assignment/{routeAssignmentId}` | GET | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| IncidentsController | `/api/incidents/my-reported` | GET | Autenticado | Autenticado | Bajo | Sin cambios |
| IncidentsController | `/api/incidents/{id}/assign/{userId}` | PUT | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| IncidentsController | `/api/incidents/{id}/in-progress` | PUT | Autenticado + handler | Usuario autorizado | Medio | Sin cambios |
| IncidentsController | `/api/incidents/{id}/resolve` | PUT | Autenticado + handler | Admin/Supervisor/Asignado | Medio | Sin cambios |
| IncidentsController | `/api/incidents/{id}/close` | PUT | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| IncidentsController | `/api/incidents/{id}/cancel` | PUT | Admin,Supervisor | Admin,Supervisor | Alto | Endurecido por metodo |
| IncidentsController | `/api/incidents/{id}/comments` | POST | Autenticado + handler | Usuario relacionado | Medio | Sin cambios |
| NotificationsController | `/api/notifications*` | GET/PUT | Autenticado + handler | Notificaciones propias; Admin/Supervisor segun handler | Bajo | Sin cambios |
| ReportsController | `/api/reports*` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| AuditLogsController | `/api/audit-logs*` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| SystemSettingsController | `/api/system-settings` | GET | Admin,Supervisor | Admin,Supervisor | Bajo | Verificado |
| SystemSettingsController | `/api/system-settings*` | POST/PUT/DELETE | Admin | Admin | Medio | Verificado |
| BackupsController | `/api/backups*` | GET/POST | Admin | Admin | Alto | Verificado |
| ExternalIntegrationsController | `/api/integrations/test-*` | POST/GET | Admin | Admin | Alto | Verificado |

## Handlers criticos revisados

- StartTrip y EndTrip: mantienen autorizacion operacional por usuario asignado mediante servicio de autorizacion de viajes.
- TripStudentAttendance: las mutaciones de asistencia quedan limitadas por controller a Admin, Supervisor, Driver y TransportAssistant, y por handler al viaje asignado.
- AddExceptionalPassenger: mantiene validacion de viaje en progreso, estudiante, duplicado y autorizacion operacional.
- TripRouteDeviation: mantiene validacion de viaje en progreso y autorizacion operacional; Guardian no puede reportar.
- Tracking: mantiene validacion por viaje asignado o relacion con estudiantes del tutor.
- Incidents: operaciones globales quedaron en Admin/Supervisor; acciones relacionadas conservan validacion contextual.

## Tests agregados o actualizados

- Driver, TransportAssistant y Guardian no pueden `GET /api/students`.
- Driver, TransportAssistant y Guardian no pueden `GET /api/routes`.
- TransportAssistant no puede cancelar viajes.
- Guardian no puede modificar asistencia de pasajeros.
- Driver y TransportAssistant pueden acceder a `/api/me/trips`.
- Guardian puede acceder a `/api/me/students`.
- Admin y Supervisor mantienen acceso a `/api/students` y `/api/routes`.

## Riesgos pendientes

- Algunas lecturas operativas dependen correctamente de validaciones en handlers porque requieren ownership dinamico. Se recomienda mantener tests de regresion para esos handlers.
- El sistema aun usa roles en atributos y permisos en estructura preparada; una fase posterior podria migrar endpoints sensibles a policies basadas en permissions.
- Todo endpoint nuevo debe partir de una regla deny-by-default: Admin/Supervisor para superficies generales y endpoints `/api/me/*` o handlers con ownership para usuarios operativos.

