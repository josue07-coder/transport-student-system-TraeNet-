# Análisis de Diagramas de Secuencia

Proyecto: **Plataforma Web para el Sistema de Transporte Escolar TRAE**  
Apartado: **4.4.1 Diagrama de Secuencia (ver los mensajes de los objetos)**

Los diagramas de secuencia representan la interacción entre actores, frontend, controladores, servicios o handlers, repositorios y base de datos. Fueron elaborados a partir de funcionalidades reales implementadas en el sistema.

| Diagrama | Objetivo | Participantes | Descripción resumida |
|---|---|---|---|
| `sequence-login` | Representar la autenticación de usuario. | Usuario, Frontend, AuthController, AuthService, UserRepository, Database. | Muestra el ingreso de credenciales, validación del usuario, generación del JWT y respuesta al frontend. |
| `sequence-student-registration` | Representar el registro de estudiante. | Administrador, Frontend, StudentsController, CreateStudentHandler, StudentRepository, Database. | Describe la creación de un estudiante, validación de datos relacionados y confirmación del registro. |
| `sequence-route-assignment` | Representar la asignación de ruta. | Supervisor, Frontend, RouteAssignmentsController, AssignmentService, Repositories, Database. | Muestra la selección de ruta, vehículo, conductor, asistente y validaciones antes de guardar la asignación. |
| `sequence-trip-materialization` | Representar la materialización de viajes programados. | Supervisor, Frontend, TripSchedulesController, MaterializeTripScheduleHandler, TripRepository, Database. | Describe cómo se valida una fecha y se crea un viaje `Scheduled` o `NotOperating`. |
| `sequence-start-trip` | Representar el inicio de un viaje. | Conductor, Frontend, TripsController, StartTripHandler, TripRepository, AttendanceRepository, Database. | Incluye validación de estado, generación de snapshot de pasajeros y cambio de estado a `InProgress`. |
| `sequence-trip-tracking` | Representar el envío de ubicación GPS. | Conductor, Frontend, TrackingController, TrackingService, VehicleLocationRepository, Database. | Muestra la validación de coordenadas y almacenamiento de ubicación para monitoreo. |
| `sequence-incident-management` | Representar el registro de incidencias. | Asistente, Frontend, IncidentsController, IncidentService, NotificationService, Database. | Describe el reporte de una incidencia, su almacenamiento y la generación de notificación al supervisor. |
| `sequence-notifications` | Representar la gestión de notificaciones. | Usuario, Frontend, NotificationsController, NotificationRepository, Database. | Muestra la consulta de notificaciones y el marcado de una notificación como leída. |

## Recomendación de uso en la tesis

Se recomienda insertar los diagramas en el mismo orden en que aparecen en este documento. Dicho orden sigue el flujo natural de la plataforma: autenticación, gestión académica, configuración operativa, programación de viajes, operación, monitoreo, incidencias y comunicación.

Nota. Elaboración propia.
