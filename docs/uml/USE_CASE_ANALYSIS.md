# Analisis de casos de uso

Este documento resume los actores y casos de uso reales identificados en `TransportStudentSystem`, tomando como base los controllers de `Transport.API`, las restricciones de `[Authorize]`, los endpoints implementados y la documentacion funcional existente en `docs/API_DOCUMENTATION.md` y `docs/FRONTEND_REQUIREMENTS.md`.

No se incluyen funcionalidades no implementadas, como chat en tiempo real, geocercas, ETA avanzado, exportacion PDF/Excel o integraciones externas reales.

## Actores identificados

| Actor | Rol tecnico | Justificacion |
| --- | --- | --- |
| Administrador | `Admin` | Tiene acceso administrativo completo: usuarios, roles, permisos, configuracion, integraciones, backups, reportes, auditoria y operacion general. |
| Supervisor | `Supervisor` | Supervisa operacion, reportes, auditoria, configuracion de lectura, calendario, programacion, incidentes y transporte. No puede ejecutar funciones exclusivas de Admin como crear usuarios administrativos, probar integraciones o ejecutar backups. |
| Conductor | `Driver` | Opera viajes asignados: inicia/finaliza viajes, actualiza ubicacion GPS, consulta sus asignaciones/viajes, reporta incidentes y actualiza estados de pasajeros. |
| Asistente de Transporte | `TransportAssistant` | Acompana la operacion del viaje: consulta asignaciones/viajes asignados, actualiza ubicacion y asistencia de pasajeros, reporta incidentes. |
| Tutor | `Guardian` | Consulta estudiantes propios, viajes relacionados, ubicacion de viajes de sus estudiantes, notificaciones e incidentes propios. |

## Casos de uso encontrados por modulo

### Auth y perfil

- Iniciar sesion.
- Recuperar contrasena.
- Consultar perfil autenticado.
- Actualizar perfil.
- Cambiar contrasena.
- Actualizar foto por URL.
- Subir foto de perfil por archivo.
- Consultar estudiantes, asignaciones y viajes propios desde `/api/me`.

### Administracion

- Crear usuario administrativo.
- Consultar, actualizar, activar y desactivar usuarios.
- Restablecer contrasena de usuario.
- Consultar roles.
- Actualizar roles.
- Asignar y remover permisos a roles.
- Consultar permisos.
- Consultar y administrar configuracion general.
- Probar integraciones mock.
- Ejecutar y consultar backups.

### Educacion

- Gestionar sectores.
- Gestionar escuelas.
- Gestionar grados.
- Gestionar tutores.
- Gestionar estudiantes.
- Consultar filtros educativos por sector, escuela, grado, tutor, codigo o documento.

### Transporte

- Gestionar vehiculos.
- Gestionar conductores.
- Gestionar asistentes de transporte.
- Gestionar paradas.
- Gestionar rutas.
- Agregar, remover y ordenar paradas de ruta.
- Gestionar asignaciones de ruta.
- Asignar y remover estudiantes de una asignacion.
- Gestionar programaciones de viaje.
- Materializar viajes desde programacion.
- Gestionar dias sin operacion.
- Iniciar, finalizar, cancelar y marcar viajes como sin operacion.
- Consultar y actualizar asistencia de pasajeros.
- Actualizar y consultar tracking GPS.

### Supervision, reportes y auditoria

- Consultar dashboard general.
- Consultar reportes de viajes.
- Consultar estudiantes por ruta.
- Consultar incidentes.
- Consultar desempeno de conductores.
- Consultar uso de vehiculos.
- Consultar resumen de auditoria.
- Consultar auditoria detallada.
- Monitorear viajes activos y ubicaciones.

### Notificaciones e incidentes

- Consultar notificaciones.
- Consultar notificaciones no leidas.
- Marcar una o todas las notificaciones como leidas.
- Reportar incidentes.
- Consultar incidentes por estado, severidad, viaje o asignacion.
- Consultar incidentes propios reportados.
- Asignar, avanzar, resolver, cerrar y cancelar incidentes.
- Agregar comentarios a incidentes.

## Diagramas generados

### `use-case-general.puml`

Diagrama de alto nivel. Resume los casos de uso principales por actor sin detallar cada endpoint individual. Es el diagrama recomendado para presentar la vision global del sistema en la tesis.

### `use-case-administration.puml`

Diagrama enfocado en administracion, seguridad, configuracion, auditoria, integraciones y backups. Separa claramente capacidades exclusivas del Administrador frente a capacidades compartidas con Supervisor.

### `use-case-transportation.puml`

Diagrama operativo del transporte. Incluye catalogos de transporte, rutas, paradas, asignaciones, programacion de viajes, calendario escolar, ciclo de vida de viajes, pasajeros y tracking GPS.

### `use-case-supervision.puml`

Diagrama para supervision administrativa y operativa. Agrupa dashboard, reportes, auditoria, monitoreo GPS, incidentes y notificaciones. Aplica principalmente a Administrador y Supervisor.

### `use-case-guardian.puml`

Diagrama especifico del Tutor. Refleja el acceso restringido a estudiantes propios, viajes relacionados, ubicacion de sus estudiantes, notificaciones e incidentes reportados.

## Cantidad aproximada de casos de uso por actor

La cuenta agrupa endpoints relacionados en casos de uso funcionales para evitar duplicar operaciones CRUD como casos separados cuando representan una misma intencion de usuario.

| Actor | Cantidad aproximada | Principales areas |
| --- | ---: | --- |
| Administrador | 43 | Seguridad, administracion, educacion, transporte, reportes, auditoria, configuracion, integraciones, backups, supervision. |
| Supervisor | 35 | Supervision, reportes, auditoria, educacion, transporte, programacion, calendario, incidentes y usuarios no exclusivos. |
| Conductor | 14 | Perfil, notificaciones, viajes asignados, pasajeros, tracking GPS e incidentes. |
| Asistente de Transporte | 14 | Perfil, notificaciones, viajes asignados, pasajeros, tracking GPS e incidentes. |
| Tutor | 18 | Perfil, estudiantes propios, viajes relacionados, tracking de estudiantes, notificaciones e incidentes propios. |

## Observaciones importantes

1. Varios controllers educativos y de catalogos de transporte no declaran `[Authorize]` directamente en el controller, aunque el frontend y las reglas de aplicacion asumen control por rol. Para tesis se documentan como funcionalidades del sistema, pero conviene revisar seguridad antes de produccion.

2. Los endpoints con `[Authorize]` sin `Roles` usan visibilidad fina en handlers y servicios, especialmente en `Me`, `Trips`, `RouteAssignments`, `Tracking`, `Notifications` e `Incidents`.

3. `Admin` y `Supervisor` comparten muchos casos de supervision, pero `Admin` conserva capacidades exclusivas: crear usuarios administrativos, administrar roles, administrar integraciones y backups.

4. `Driver` y `TransportAssistant` comparten casi la misma superficie operativa, con la diferencia de que cada uno accede segun su vinculo con la asignacion de ruta.

5. `Guardian` no administra datos maestros. Su alcance real esta en consulta de estudiantes propios, seguimiento de viajes, ubicacion, notificaciones e incidentes.

6. La programacion de viajes, dias sin operacion y snapshot de pasajeros ya forman parte del backend; por eso aparecen en los diagramas de transporte.

7. Las integraciones externas existentes son mocks o pruebas administrativas; no representan envio real de email, SMS, WhatsApp o push en produccion.

8. Backup implementa respaldo logico inicial y consulta de historial; restore destructivo no esta implementado como funcionalidad real.

## Modulos involucrados

- `AuthController`
- `MeController`
- `UsersController`
- `RolesController`
- `PermissionsController`
- `SectorsController`
- `SchoolsController`
- `GradesController`
- `GuardiansController`
- `StudentsController`
- `VehiclesController`
- `DriversController`
- `TransportAssistantsController`
- `StopsController`
- `RoutesController`
- `RouteAssignmentsController`
- `TripSchedulesController`
- `NonSchoolDaysController`
- `TripsController`
- `TrackingController`
- `NotificationsController`
- `IncidentsController`
- `ReportsController`
- `AuditLogsController`
- `SystemSettingsController`
- `ExternalIntegrationsController`
- `BackupsController`

## Recomendacion para la tesis

Usar `use-case-general.puml` como diagrama principal del capitulo de analisis. Luego usar los diagramas por modulo para ampliar:

1. Administracion y seguridad.
2. Transporte y operacion.
3. Supervision.
4. Tutor.

Esta separacion mantiene trazabilidad con el backend real y evita que un unico diagrama sea demasiado denso para leerse correctamente.
