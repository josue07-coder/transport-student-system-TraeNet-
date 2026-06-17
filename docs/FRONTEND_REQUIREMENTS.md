# Requerimientos tecnicos del frontend

Documento guia para construir un frontend profesional que consuma la API del sistema de transporte escolar.

Fuente principal: `docs/API_DOCUMENTATION.md`.

## 1. Objetivo del frontend

Construir una aplicacion web responsive, segura y usable para administrar el sistema completo de transporte escolar:

- Autenticacion con JWT.
- Administracion de usuarios, roles y permisos.
- Gestion educativa: estudiantes, tutores, escuelas, grados y sectores.
- Gestion operativa de transporte: vehiculos, conductores, asistentes, paradas, rutas, asignaciones y viajes.
- Seguimiento GPS de viajes activos.
- Notificaciones internas.
- Gestion de incidentes operativos.
- Reportes, auditoria, configuracion, integraciones y backups.

El frontend debe respetar la visibilidad por rol:

- `Admin`: acceso total.
- `Supervisor`: gestion operativa general y reportes, sin administracion total de roles sensibles.
- `Guardian`: estudiantes propios, viajes relacionados, ubicacion de sus estudiantes, notificaciones e incidentes propios.
- `Driver`: asignaciones, rutas y viajes donde es conductor.
- `TransportAssistant`: asignaciones, rutas y viajes donde es asistente.

## 2. Stack recomendado

### Opcion recomendada

- React + TypeScript.
- Vite para SPA rapida y simple.
- Tailwind CSS.
- TanStack Query para server state.
- Zustand para estado global liviano.
- React Hook Form + Zod para formularios y validacion.
- Axios para cliente HTTP.
- Leaflet/OpenStreetMap para mapas sin dependencia comercial inicial.
- Recharts para dashboards y reportes.
- React Router para rutas protegidas.
- Sonner o React Hot Toast para feedback.
- Lucide React para iconografia.

### Alternativa

- Next.js si se requiere SSR, rutas file-based, metadata avanzada o despliegue full-stack.
- Google Maps si el proyecto necesita geocoding/rutas comerciales, mapas mas precisos o integracion futura con Places.

## 3. Arquitectura sugerida

```text
src/
  api/
    http.ts
    endpoints.ts
    pagination.ts
    errors.ts
  auth/
    authStore.ts
    token.ts
    ProtectedRoute.tsx
    RoleGuard.tsx
  components/
    data-table/
    forms/
    layout/
    map/
    feedback/
    ui/
  features/
    auth/
    me/
    users/
    roles/
    permissions/
    education/
      students/
      guardians/
      schools/
      grades/
      sectors/
    transportation/
      vehicles/
      drivers/
      transport-assistants/
      stops/
      routes/
      route-assignments/
      trips/
    notifications/
    incidents/
    tracking/
    reports/
    audit-logs/
    system-settings/
    integrations/
    backups/
  layouts/
    AppLayout.tsx
    AuthLayout.tsx
    DashboardLayout.tsx
  pages/
  routes/
    router.tsx
    routeConfig.ts
  stores/
  types/
    api.ts
    auth.ts
    enums.ts
    dto.ts
  utils/
    dates.ts
    formatters.ts
    permissions.ts
    validators.ts
```

## 4. Cliente API

### Axios

Crear `src/api/http.ts` con:

- `baseURL` desde variable de entorno `VITE_API_URL`.
- Interceptor request para agregar `Authorization: Bearer {token}`.
- Interceptor response para normalizar errores.
- Manejo de `401` con logout automatico o redireccion a `/login`.
- Manejo de `403` con pantalla de acceso denegado.

### Convencion de paginacion

Todos los listados paginados deben enviar:

```ts
{
  PageNumber: number;
  PageSize: number;
}
```

Modelo de respuesta:

```ts
type PaginatedResponse<T> = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  items: T[];
};
```

## 5. Manejo de JWT

### Login

Consumir:

- `POST /api/auth/login`

Guardar:

- `token`
- `userId`
- `username`
- `name`
- `profileImageUrl`
- `role`

### Donde guardar el token

Recomendado para fase inicial:

- `localStorage` o `sessionStorage`, segun decision de UX.
- Zustand para estado en memoria.

Recomendacion de seguridad futura:

- Migrar a cookie `HttpOnly` si el backend se adapta.
- Implementar refresh token en fase posterior.

### Expiracion

El frontend debe:

- Detectar `401`.
- Limpiar sesion local.
- Redirigir a `/login`.
- Mostrar mensaje: "Tu sesion expiro. Inicia sesion nuevamente."

### Logout

Debe:

- Eliminar token y datos de usuario.
- Cancelar queries activas si aplica.
- Redirigir a `/login`.

## 6. Manejo de errores

| Codigo | Comportamiento frontend |
|---|---|
| 400 | Mostrar errores de validacion o regla de negocio en formulario/toast. |
| 401 | Cerrar sesion y redirigir a login. |
| 403 | Mostrar pagina "Acceso denegado". |
| 404 | Mostrar empty state o pagina "No encontrado". |
| 500 | Mostrar mensaje generico y registrar detalle en consola solo en desarrollo. |

El frontend debe soportar errores con estructuras distintas, porque algunos pueden venir de middleware, validacion o reglas de dominio.

## 7. Componentes reutilizables

- `DataTable`: columnas configurables, loading, empty state, paginacion, acciones por fila.
- `Modal`: crear, editar, confirmar, detalle.
- `Form`: wrapper con React Hook Form.
- `TextInput`, `NumberInput`, `DateInput`, `DateTimeInput`.
- `Select`, `AsyncSelect`, `EnumSelect`.
- `Badge`.
- `StatusChip`: estados de ruta, viaje, vehiculo, incidente, backup.
- `MapView`: Leaflet/Google Maps.
- `MapMarker`.
- `NotificationBell`.
- `Sidebar`.
- `Topbar`.
- `RoleGuard`.
- `PermissionGuard` preparado para permisos futuros.
- `ConfirmDialog`.
- `PageHeader`.
- `FilterBar`.
- `StatCard`.
- `ChartCard`.
- `AuditTimeline`.
- `ErrorState`.
- `EmptyState`.
- `LoadingState`.

## 8. Diseno UI/UX

El sistema debe sentirse como una herramienta operativa profesional:

- Interfaz limpia, sobria y orientada a productividad.
- Dashboard moderno con metricas, tablas y alertas visibles.
- Responsive completo: desktop, tablet y mobile.
- Navegacion lateral en desktop y drawer/bottom navigation en mobile.
- Formularios compactos, con validacion visible y mensajes claros.
- Estados operativos con colores consistentes.
- Mapas amplios y faciles de leer.
- Acciones destructivas siempre con confirmacion.
- Evitar exponer IDs tecnicos salvo en detalle/debug.

## 9. Navegacion por rol

### Admin

Menu recomendado:

- Dashboard
- Education
- Transportation
- Tracking GPS
- Incidents
- Notifications
- Reports
- Users
- Roles
- Permissions
- Audit Logs
- System Settings
- Integrations
- Backups
- Profile

### Supervisor

- Dashboard
- Education
- Transportation
- Tracking GPS
- Incidents
- Notifications
- Reports
- Audit Logs
- System Settings solo lectura
- Profile

### Guardian

- My Dashboard
- My Students
- My Trips
- Tracking
- Incidents
- Notifications
- Profile

### Driver

- My Dashboard
- My Route Assignments
- My Trips
- Tracking update
- Incidents
- Notifications
- Profile

### TransportAssistant

- My Dashboard
- My Route Assignments
- My Trips
- Tracking update
- Incidents
- Notifications
- Profile

## 10. Dashboards

### Admin dashboard

Endpoints:

- `GET /api/reports/dashboard`
- `GET /api/notifications/unread`
- `GET /api/incidents/by-status/{status}`
- `GET /api/tracking/active-trips`

Debe mostrar:

- Totales generales.
- Viajes activos.
- Incidentes abiertos y criticos.
- Notificaciones no leidas.
- Accesos rapidos a usuarios, reportes, backups y configuracion.
- Grafico de viajes por estado.
- Mapa de trips activos.

### Supervisor dashboard

Endpoints:

- `GET /api/reports/dashboard`
- `GET /api/tracking/active-trips`
- `GET /api/incidents`
- `GET /api/notifications/unread`

Debe mostrar:

- Operacion del dia.
- Alertas de incidentes.
- Rutas activas.
- Conductores/vehiculos en operacion.
- Accesos a reportes.

### Guardian dashboard

Endpoints:

- `GET /api/me`
- `GET /api/me/students`
- `GET /api/me/trips`
- `GET /api/tracking/my-students`
- `GET /api/notifications/unread`

Debe mostrar:

- Estudiantes propios.
- Viajes activos de sus estudiantes.
- Ubicacion actual si existe trip en progreso.
- Historial breve de viajes.
- Notificaciones recientes.

### Driver dashboard

Endpoints:

- `GET /api/me`
- `GET /api/me/route-assignments`
- `GET /api/me/trips`
- `GET /api/notifications/unread`

Debe mostrar:

- Asignacion activa.
- Ruta y paradas.
- Botones para iniciar/finalizar/cancelar viaje segun estado.
- Acceso rapido para reportar incidente.
- Accion para actualizar ubicacion.

### TransportAssistant dashboard

Endpoints:

- `GET /api/me`
- `GET /api/me/route-assignments`
- `GET /api/me/trips`
- `GET /api/notifications/unread`

Debe mostrar:

- Asignaciones donde participa.
- Viajes activos.
- Estudiantes asignados.
- Incidentes/notificaciones.
- Tracking si tiene permiso operativo.

## 11. Paginas publicas

### Login

Endpoint:

- `POST /api/auth/login`

Elementos:

- Username.
- Password.
- Boton ingresar.
- Link a forgot password.
- Mensajes de error de credenciales.

Validaciones:

- Username requerido.
- Password requerido.

### Forgot Password

Endpoint:

- `POST /api/auth/forgot-password`

Campos:

- DocumentNumber.
- PhoneOrEmail.
- NewPassword.
- ConfirmPassword.

Validaciones:

- Documento requerido.
- Telefono/email requerido.
- Password minimo 6 caracteres.
- Confirmacion igual.

## 12. Paginas protegidas

Todas las paginas internas deben exigir JWT. El layout debe consultar `/api/me` al iniciar si hay token guardado.

Rutas sugeridas:

```text
/dashboard
/me
/users
/roles
/permissions
/students
/guardians
/schools
/grades
/sectors
/vehicles
/drivers
/transport-assistants
/stops
/routes
/route-assignments
/trips
/notifications
/incidents
/tracking
/reports
/audit-logs
/system-settings
/integrations
/backups
```

## 13. Modulos frontend a construir

### Auth

Pantallas:

- Login.
- Forgot Password.

Acciones:

- Iniciar sesion.
- Recuperar password simple.
- Logout.

Endpoints:

- `POST /api/auth/login`
- `POST /api/auth/forgot-password`
- `GET /api/me`

Roles:

- Publico para login/forgot.
- Todos para perfil.

Componentes:

- `LoginForm`
- `ForgotPasswordForm`
- `AuthLayout`

Validaciones:

- Campos requeridos.
- Password minimo 6 en forgot password.

### Profile / Me

Pantallas:

- Perfil de usuario.
- Editar perfil.
- Cambiar password.
- Cambiar foto.
- Mis estudiantes.
- Mis asignaciones.
- Mis viajes.

Endpoints:

- `GET /api/me`
- `GET /api/me/students`
- `GET /api/me/route-assignments`
- `GET /api/me/trips`
- `PUT /api/me/profile`
- `PUT /api/me/change-password`
- `PUT /api/me/photo`

Roles:

- Todos autenticados.

Componentes:

- `ProfileCard`
- `ProfileForm`
- `ChangePasswordForm`
- `PhotoUploader`
- `MyTripsList`

Validaciones:

- Email valido si se edita.
- Password actual requerido.
- Confirmacion de password.
- URL de foto valida si aplica.

### Users

Pantallas:

- Listado de usuarios.
- Detalle de usuario.
- Editar usuario.
- Activar/desactivar.
- Reset password.
- Filtros por rol y estado.

Endpoints:

- `GET /api/users`
- `GET /api/users/{id}`
- `GET /api/users/by-role/{roleId}`
- `GET /api/users/by-active/{isActive}`
- `PUT /api/users/{id}`
- `PUT /api/users/{id}/activate`
- `PUT /api/users/{id}/deactivate`
- `PUT /api/users/{id}/reset-password`

Roles:

- Admin, Supervisor.

Componentes:

- `UsersTable`
- `UserDetailDrawer`
- `UserForm`
- `ResetPasswordModal`
- `RoleSelect`

Validaciones:

- Nombre requerido.
- Email valido.
- Rol requerido.
- Password minimo 6 en reset.

### Roles

Pantallas:

- Lista de roles.
- Detalle de rol con permisos.
- Editar rol.
- Asignar/remover permisos.

Endpoints:

- `GET /api/roles`
- `GET /api/roles/{id}`
- `PUT /api/roles/{id}`
- `POST /api/roles/{roleId}/permissions/{permissionId}`
- `DELETE /api/roles/{roleId}/permissions/{permissionId}`

Roles:

- Admin.

Componentes:

- `RolesTable`
- `RoleForm`
- `PermissionsPicker`

Validaciones:

- Nombre requerido.
- Evitar duplicar permiso en UI.

### Permissions

Pantallas:

- Lista de permisos.
- Detalle simple.

Endpoints:

- `GET /api/permissions`
- `GET /api/permissions/{id}`

Roles:

- Admin, Supervisor.

Componentes:

- `PermissionsTable`
- `PermissionBadge`

Validaciones:

- Solo lectura en esta fase.

### Students

Pantallas:

- Listado paginado.
- Crear estudiante.
- Editar estudiante.
- Detalle.
- Filtros por grado, escuela y tutor.
- Desactivar/eliminar.

Endpoints:

- `GET /api/students`
- `GET /api/students/{id}`
- `GET /api/students/code/{code}`
- `GET /api/students/by-grade/{gradeId}`
- `GET /api/students/by-school/{schoolId}`
- `GET /api/students/by-guardian/{guardianId}`
- `POST /api/students`
- `PUT /api/students/{id}`
- `DELETE /api/students/{id}`

Roles:

- Admin, Supervisor para gestion general.
- Guardian solo lectura de sus estudiantes via `/api/me/students` y reglas de visibilidad.

Componentes:

- `StudentsTable`
- `StudentForm`
- `StudentDetail`
- `GuardianSelect`
- `SchoolSelect`
- `GradeSelect`

Validaciones:

- Nombre y apellido requeridos.
- Guardian requerido.
- School requerido.
- Grade requerido.
- Grade debe pertenecer a School, idealmente filtrar grades por school.

### Guardians

Pantallas:

- Listado.
- Crear/editar tutor.
- Detalle.
- Buscar por documento.
- Desactivar.

Endpoints:

- `GET /api/guardians`
- `GET /api/guardians/{id}`
- `GET /api/guardians/document/{documentNumber}`
- `POST /api/guardians`
- `PUT /api/guardians/{id}`
- `DELETE /api/guardians/{id}`

Roles:

- Admin, Supervisor recomendado.
- Revisar porque controller no declara `[Authorize]`.

Componentes:

- `GuardiansTable`
- `GuardianForm`
- `DocumentTypeSelect`
- `GenderSelect`
- `SectorSelect`

Validaciones:

- Documento requerido.
- Nombre/apellido requeridos.
- Telefono requerido.
- Ciudad/calle requeridas.
- Email si existe debe ser valido.

### Schools

Pantallas:

- Listado.
- Crear/editar escuela.
- Detalle.
- Filtro por sector.

Endpoints:

- `GET /api/schools`
- `GET /api/schools/{id}`
- `GET /api/schools/by-sector/{sectorId}`
- `POST /api/schools`
- `PUT /api/schools/{id}`
- `DELETE /api/schools/{id}`

Roles:

- Admin, Supervisor recomendado.
- Revisar auth real.

Componentes:

- `SchoolsTable`
- `SchoolForm`
- `SectorSelect`

Validaciones:

- Nombre requerido.
- Director requerido.
- Email valido.
- Telefono requerido.
- Sector requerido.

### Grades

Pantallas:

- Listado.
- Crear/editar grado.
- Filtro por escuela.

Endpoints:

- `GET /api/grades`
- `GET /api/grades/{id}`
- `GET /api/grades/by-school/{schoolId}`
- `POST /api/grades`
- `PUT /api/grades/{id}`
- `DELETE /api/grades/{id}`

Roles:

- Admin, Supervisor recomendado.

Componentes:

- `GradesTable`
- `GradeForm`
- `SchoolSelect`

Validaciones:

- Nombre requerido.
- School requerido.

### Sectors

Pantallas:

- Listado.
- Crear/editar sector.
- Detalle.

Endpoints:

- `GET /api/sectors`
- `GET /api/sectors/{id}`
- `POST /api/sectors`
- `PUT /api/sectors/{id}`
- `DELETE /api/sectors/{id}`

Roles:

- Admin, Supervisor recomendado.

Componentes:

- `SectorsTable`
- `SectorForm`

Validaciones:

- Name requerido.
- Province requerido.
- City requerido.

### Vehicles

Pantallas:

- Listado.
- Crear/editar vehiculo.
- Detalle.
- Filtros por placa y status.
- Cambiar estado/desactivar.

Endpoints:

- `GET /api/vehicles`
- `GET /api/vehicles/{id}`
- `GET /api/vehicles/plate/{plateNumber}`
- `GET /api/vehicles/by-status/{status}`
- `POST /api/vehicles`
- `PUT /api/vehicles/{id}`
- `DELETE /api/vehicles/{id}`

Roles:

- Admin, Supervisor recomendado.

Componentes:

- `VehiclesTable`
- `VehicleForm`
- `VehicleStatusChip`

Validaciones:

- Placa requerida.
- Capacidad mayor que 0.

### Drivers

Pantallas:

- Listado.
- Crear/editar conductor.
- Detalle.
- Buscar por licencia.
- Filtro activo/inactivo.
- Desactivar.

Endpoints:

- `GET /api/drivers`
- `GET /api/drivers/{id}`
- `GET /api/drivers/license/{licenseNumber}`
- `GET /api/drivers/by-active/{isActive}`
- `POST /api/drivers`
- `PUT /api/drivers/{id}`
- `DELETE /api/drivers/{id}`

Roles:

- Admin, Supervisor recomendado.

Componentes:

- `DriversTable`
- `DriverForm`
- `DocumentTypeSelect`

Validaciones:

- Documento requerido.
- Licencia requerida.
- Telefono requerido.
- Direccion requerida.
- Email opcional valido.

### TransportAssistants

Pantallas:

- Listado.
- Crear/editar asistente.
- Detalle.
- Buscar por documento.
- Filtro activo/inactivo.
- Desactivar.

Endpoints:

- `GET /api/transport-assistants`
- `GET /api/transport-assistants/{id}`
- `GET /api/transport-assistants/document/{documentNumber}`
- `GET /api/transport-assistants/by-active/{isActive}`
- `POST /api/transport-assistants`
- `PUT /api/transport-assistants/{id}`
- `DELETE /api/transport-assistants/{id}`

Roles:

- Admin, Supervisor recomendado.

Componentes:

- `TransportAssistantsTable`
- `TransportAssistantForm`

Validaciones:

- Documento requerido.
- Nombre/apellido requeridos.
- Telefono requerido.
- Direccion requerida.
- Email opcional valido.

### Stops

Pantallas:

- Listado.
- Crear/editar parada.
- Detalle con mapa.
- Filtros por sector y ciudad.

Endpoints:

- `GET /api/stops`
- `GET /api/stops/{id}`
- `GET /api/stops/by-sector/{sectorId}`
- `GET /api/stops/by-city/{city}`
- `POST /api/stops`
- `PUT /api/stops/{id}`
- `DELETE /api/stops/{id}`

Roles:

- Admin, Supervisor recomendado.

Componentes:

- `StopsTable`
- `StopForm`
- `MapPicker`
- `SectorSelect`

Validaciones:

- Nombre requerido.
- Direccion requerida.
- Latitud entre -90 y 90.
- Longitud entre -180 y 180.

### Routes

Pantallas:

- Listado de rutas.
- Crear/editar ruta.
- Detalle de ruta con paradas.
- Gestionar paradas de ruta.
- Reordenar paradas.
- Filtros por school y status.

Endpoints:

- `GET /api/routes`
- `GET /api/routes/{id}`
- `GET /api/routes/by-school/{schoolId}`
- `GET /api/routes/by-status/{status}`
- `POST /api/routes`
- `PUT /api/routes/{id}`
- `DELETE /api/routes/{id}`
- `POST /api/routes/{routeId}/stops`
- `DELETE /api/routes/{routeId}/stops/{stopId}`
- `PUT /api/routes/{routeId}/stops/{stopId}/order`

Roles:

- Admin, Supervisor para gestion.
- Driver/TransportAssistant solo lectura segun sus asignaciones.

Componentes:

- `RoutesTable`
- `RouteForm`
- `RouteStopsManager`
- `StopOrderList`
- `RouteMap`
- `RouteStatusChip`

Validaciones:

- Nombre requerido.
- School requerido.
- EndTime mayor que StartTime.
- Duracion entre 10 minutos y 4 horas.
- StopOrder unico.

### RouteAssignments

Pantallas:

- Listado.
- Crear/editar asignacion.
- Detalle con ruta, driver, vehicle, assistant y students.
- Asignar/remover estudiante.
- Filtros por ruta, conductor y vehiculo.

Endpoints:

- `GET /api/route-assignments`
- `GET /api/route-assignments/{id}`
- `GET /api/route-assignments/by-route/{routeId}`
- `GET /api/route-assignments/by-driver/{driverId}`
- `GET /api/route-assignments/by-vehicle/{vehicleId}`
- `POST /api/route-assignments`
- `PUT /api/route-assignments/{id}`
- `DELETE /api/route-assignments/{id}`
- `POST /api/route-assignments/{assignmentId}/students/{studentId}`
- `DELETE /api/route-assignments/{assignmentId}/students/{studentId}`

Roles:

- Admin, Supervisor para gestion.
- Driver/TransportAssistant solo sus asignaciones.
- Guardian solo asignaciones relacionadas a sus estudiantes.

Componentes:

- `RouteAssignmentsTable`
- `RouteAssignmentForm`
- `AssignedStudentsList`
- `AssignStudentModal`
- `CapacityIndicator`

Validaciones:

- Route requerido.
- Driver requerido.
- Vehicle requerido.
- Capacidad mayor que 0.
- Capacidad no mayor que vehicle capacity.
- No permitir estudiante duplicado.

### Trips

Pantallas:

- Listado de viajes.
- Detalle de viaje.
- Iniciar viaje.
- Finalizar viaje.
- Cancelar viaje.
- Filtros por asignacion, estado y rango de fechas.

Endpoints:

- `GET /api/trips`
- `GET /api/trips/{id}`
- `GET /api/trips/by-route-assignment/{routeAssignmentId}`
- `GET /api/trips/by-status/{status}`
- `GET /api/trips/by-date-range`
- `GET /api/trips/active/by-route-assignment/{routeAssignmentId}`
- `POST /api/trips/start`
- `PUT /api/trips/{id}/end`
- `PUT /api/trips/{id}/cancel`

Roles:

- Admin, Supervisor para todos.
- Driver/TransportAssistant sus viajes.
- Guardian viajes relacionados.

Componentes:

- `TripsTable`
- `TripDetail`
- `TripStatusChip`
- `TripActions`
- `DateRangeFilter`

Validaciones:

- Solo mostrar acciones permitidas por estado.
- Confirmar cancelacion.

### Notifications

Pantallas:

- Campana de notificaciones.
- Lista de notificaciones.
- No leidas.
- Marcar como leida.
- Marcar todas como leidas.

Endpoints:

- `GET /api/notifications`
- `GET /api/notifications/unread`
- `PUT /api/notifications/{id}/read`
- `PUT /api/notifications/read-all`

Roles:

- Todos autenticados.

Componentes:

- `NotificationBell`
- `NotificationsList`
- `NotificationItem`
- `PriorityBadge`

Validaciones:

- No aplicar formularios; manejar estados de lectura.

### Incidents

Pantallas:

- Listado.
- Reportar incidente.
- Detalle.
- Asignar usuario.
- Marcar en progreso.
- Resolver.
- Cerrar.
- Cancelar.
- Comentarios.
- Filtros por estado, severidad, trip, assignment.

Endpoints:

- `GET /api/incidents`
- `GET /api/incidents/{id}`
- `GET /api/incidents/by-status/{status}`
- `GET /api/incidents/by-severity/{severity}`
- `GET /api/incidents/by-trip/{tripId}`
- `GET /api/incidents/by-route-assignment/{routeAssignmentId}`
- `GET /api/incidents/my-reported`
- `POST /api/incidents`
- `PUT /api/incidents/{id}/assign/{userId}`
- `PUT /api/incidents/{id}/in-progress`
- `PUT /api/incidents/{id}/resolve`
- `PUT /api/incidents/{id}/close`
- `PUT /api/incidents/{id}/cancel`
- `POST /api/incidents/{id}/comments`

Roles:

- Todos pueden reportar.
- Admin/Supervisor gestionan todos.
- Driver/Assistant/Guardian solo relacionados.

Componentes:

- `IncidentsTable`
- `IncidentForm`
- `IncidentDetail`
- `IncidentTimeline`
- `IncidentComments`
- `SeverityChip`
- `IncidentStatusChip`

Validaciones:

- Title requerido.
- Description requerido.
- Type requerido.
- Severity requerido.
- Comment requerido.

### Tracking GPS

Pantallas:

- Mapa de viajes activos.
- Ubicacion actual de trip.
- Historial de ubicaciones.
- Vista de mis estudiantes.
- Panel de actualizacion de ubicacion para driver/assistant.

Endpoints:

- `POST /api/tracking/location`
- `GET /api/tracking/trips/{tripId}/current-location`
- `GET /api/tracking/trips/{tripId}/history`
- `GET /api/tracking/active-trips`
- `GET /api/tracking/my-students`

Roles:

- Admin/Supervisor monitorean todos.
- Driver/Assistant ven y actualizan viajes asignados.
- Guardian ve viajes de sus estudiantes.

Componentes:

- `MapView`
- `TripLocationMarker`
- `LocationHistoryPolyline`
- `ActiveTripsMap`
- `LocationUpdatePanel`

Validaciones:

- Latitud entre -90 y 90.
- Longitud entre -180 y 180.
- Speed mayor o igual a 0.
- Heading entre 0 y 360.

### Reports

Pantallas:

- Dashboard report.
- Viajes por rango.
- Estudiantes por ruta.
- Incidentes por rango.
- Desempeno de conductores.
- Uso de vehiculos.
- Auditoria resumida.

Endpoints:

- `GET /api/reports/dashboard`
- `GET /api/reports/trips`
- `GET /api/reports/students-by-route/{routeId}`
- `GET /api/reports/incidents`
- `GET /api/reports/drivers-performance`
- `GET /api/reports/vehicles-usage`
- `GET /api/reports/audit-summary`

Roles:

- Admin, Supervisor.

Componentes:

- `ReportDateRangePicker`
- `DashboardCards`
- `TripsReportTable`
- `DriversPerformanceChart`
- `VehiclesUsageChart`
- `AuditSummaryChart`

Validaciones:

- StartDate requerido.
- EndDate requerido.
- EndDate >= StartDate.
- Rango maximo recomendado: 1 anio.

### Audit Logs

Pantallas:

- Listado paginado.
- Filtros por usuario, entidad, accion y rango de fechas.
- Detalle de evento.

Endpoints:

- `GET /api/audit-logs`
- `GET /api/audit-logs/by-user/{userId}`
- `GET /api/audit-logs/by-entity/{entityName}/{entityId}`
- `GET /api/audit-logs/by-action/{action}`
- `GET /api/audit-logs/by-date-range`

Roles:

- Admin, Supervisor.

Componentes:

- `AuditLogsTable`
- `AuditFilters`
- `AuditDetailDrawer`

Validaciones:

- Rango de fechas valido.

### System Settings

Pantallas:

- Lista agrupada por categoria.
- Detalle de setting.
- Crear setting custom.
- Editar setting.
- Eliminar setting editable.

Endpoints:

- `GET /api/system-settings`
- `GET /api/system-settings/{id}`
- `GET /api/system-settings/key/{key}`
- `GET /api/system-settings/by-category/{category}`
- `POST /api/system-settings`
- `PUT /api/system-settings/{id}`
- `DELETE /api/system-settings/{id}`

Roles:

- Admin, Supervisor lectura.
- Admin escritura.

Componentes:

- `SettingsTable`
- `SettingsCategoryTabs`
- `SettingForm`
- `DataTypeSelect`

Validaciones:

- Key requerido.
- Value requerido.
- Category requerido.
- DataType requerido.
- Bloquear edicion si `isEditable = false`.

### Integrations

Pantallas:

- Panel de pruebas de integraciones.
- Test email.
- Test SMS.
- Test WhatsApp.
- Test push.
- Test distance/maps.

Endpoints:

- `POST /api/integrations/test-email`
- `POST /api/integrations/test-sms`
- `POST /api/integrations/test-whatsapp`
- `POST /api/integrations/test-push`
- `GET /api/integrations/test-distance`

Roles:

- Admin.

Componentes:

- `IntegrationTestPanel`
- `EmailTestForm`
- `PhoneMessageTestForm`
- `PushTestForm`
- `DistanceTestForm`

Validaciones:

- Email valido.
- Telefono requerido.
- Mensaje requerido.
- Coordenadas validas.

### Backups

Pantallas:

- Historial de backups.
- Crear backup manual.
- Detalle de backup.
- Estado del ultimo backup.
- Restore placeholder deshabilitado o con advertencia.

Endpoints:

- `POST /api/backups/manual`
- `GET /api/backups`
- `GET /api/backups/latest`
- `GET /api/backups/{id}`
- `POST /api/backups/{id}/restore`

Roles:

- Admin.

Componentes:

- `BackupsTable`
- `BackupStatusChip`
- `CreateBackupButton`
- `BackupDetailDrawer`

Validaciones:

- Confirmar creacion de backup.
- No mostrar restore como accion normal hasta que este implementado.

## 14. Rutas protegidas y guards

Implementar:

- `ProtectedRoute`: requiere token.
- `RoleGuard`: valida rol.
- `VisibilityGuard`: helper para ocultar acciones por rol.

Ejemplo de matriz:

| Modulo | Admin | Supervisor | Guardian | Driver | TransportAssistant |
|---|---:|---:|---:|---:|---:|
| Dashboard | Si | Si | Si | Si | Si |
| Users | Si | Si | No | No | No |
| Roles | Si | No | No | No | No |
| Permissions | Si | Si | No | No | No |
| Education | Si | Si | Parcial | No | No |
| Transportation | Si | Si | Parcial lectura | Parcial | Parcial |
| Tracking | Si | Si | Mis estudiantes | Mis viajes | Mis viajes |
| Reports | Si | Si | No | No | No |
| Audit Logs | Si | Si | No | No | No |
| System Settings | Si | Lectura | No | No | No |
| Integrations | Si | No | No | No | No |
| Backups | Si | No | No | No | No |

## 15. Prioridades de desarrollo

### Fase frontend 1: Auth + Layout + Dashboard base

- Login.
- Forgot password.
- Auth store.
- Axios interceptors.
- Protected routes.
- Layout principal.
- Sidebar por rol.
- `/api/me`.
- Dashboard basico por rol.

### Fase frontend 2: Education module

- Students.
- Guardians.
- Schools.
- Grades.
- Sectors.
- Selects dependientes.
- Validaciones de formularios.

### Fase frontend 3: Transportation module

- Vehicles.
- Drivers.
- TransportAssistants.
- Stops.
- Routes y route stops.
- RouteAssignments y assigned students.

### Fase frontend 4: Tracking + Trips

- Trips.
- Acciones start/end/cancel.
- Tracking map.
- Current location.
- History.
- My students tracking.

### Fase frontend 5: Notifications + Incidents

- Notification bell.
- Notifications center.
- Incident reporting.
- Incident workflow.
- Comments.

### Fase frontend 6: Reports + Audit + Settings

- Reports dashboard.
- Charts.
- Audit logs.
- System settings.
- Integrations test panel.
- Backups panel.

### Fase frontend 7: Polish, responsive, testing

- Mobile polish.
- Empty/loading/error states.
- Accessibility pass.
- E2E tests.
- Error boundary.
- Performance cleanup.

## 16. Criterios de aceptacion

### Seguridad y auth

- Login funcional con JWT.
- Token enviado automaticamente en endpoints protegidos.
- Logout limpia sesion.
- Rutas protegidas bloquean usuarios no autenticados.
- Rutas por rol bloquean usuarios no autorizados.
- `401` redirige a login.
- `403` muestra acceso denegado.

### CRUDs principales

- CRUD funcional de Education.
- CRUD funcional de Transportation.
- Formularios con validacion antes de enviar.
- Reglas de negocio del backend mostradas como mensajes claros.
- Tablas con paginacion.
- Filtros principales funcionando.

### Operacion

- Crear y consultar route assignments.
- Asignar/remover estudiantes.
- Iniciar/finalizar/cancelar trips segun estado.
- Reportar y gestionar incidentes.
- Notificaciones visibles y marcables como leidas.

### Tracking

- Mapa renderiza correctamente.
- Ubicacion actual visible.
- Historial visible.
- Guardian ve ubicacion de sus estudiantes.
- Driver/Assistant pueden actualizar ubicacion si estan asignados.

### Reportes y administracion

- Dashboard visible para Admin/Supervisor.
- Reportes por rango funcionando.
- Audit logs filtrables.
- Settings visibles y editables segun rol.
- Integrations test panel restringido a Admin.
- Backups restringido a Admin.

### UX

- Responsive completo.
- Estados loading/empty/error en todas las pantallas.
- Confirmacion para acciones destructivas.
- Estados visuales claros para activo/inactivo, en progreso, completado, cancelado, critico.
- Navegacion por rol sin opciones inutiles.

## 17. Recomendaciones clave para iniciar

1. Empezar por `Auth`, `http.ts`, `ProtectedRoute`, `RoleGuard` y `AppLayout`.
2. Generar tipos TypeScript manuales desde los DTOs documentados antes de construir pantallas.
3. Crear componentes base antes de los CRUDs: `DataTable`, `Modal`, `Form`, `Select`, `StatusChip`.
4. Usar TanStack Query desde el primer modulo para cache, invalidacion y loading states.
5. Implementar primero flujos verticales completos pequenos: login -> dashboard -> students list -> student detail.
6. Mantener mapas y reportes para fases posteriores, cuando datos y roles ya esten estables.
7. Revisar Swagger en desarrollo para confirmar valores exactos de enums y shape final de algunos commands.

## 18. Endpoints base por modulo

Referencia rapida:

- Auth: `/api/auth/*`
- Me/Profile: `/api/me/*`
- Users: `/api/users/*`
- Roles: `/api/roles/*`
- Permissions: `/api/permissions/*`
- Education: `/api/students`, `/api/guardians`, `/api/schools`, `/api/grades`, `/api/sectors`
- Transportation: `/api/vehicles`, `/api/drivers`, `/api/transport-assistants`, `/api/stops`, `/api/routes`, `/api/route-assignments`, `/api/trips`
- Notifications: `/api/notifications`
- Incidents: `/api/incidents`
- Tracking: `/api/tracking`
- Reports: `/api/reports`
- Audit Logs: `/api/audit-logs`
- System Settings: `/api/system-settings`
- Integrations: `/api/integrations`
- Backups: `/api/backups`

## 19. Riesgos y puntos a revisar

- Algunos controllers operativos/educativos no declaran `[Authorize]` actualmente. El frontend debe asumir proteccion por rol en UI, pero backend deberia revisarse antes de produccion.
- Los enums deben confirmarse contra Swagger para renderizar labels correctos.
- Restore de backups es placeholder; no debe presentarse como funcional.
- Refresh token no existe todavia; manejar expiracion con logout.
- La visibilidad fina vive en backend; el frontend solo debe ocultar accesos para UX, no confiar en eso como seguridad.
