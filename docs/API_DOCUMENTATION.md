# Documentacion tecnica de la API

Backend .NET con Clean Architecture, CQRS, MediatR, EF Core y JWT Bearer.

Esta guia documenta los endpoints encontrados en los controllers de `Transport.API`. Las rutas se muestran en minusculas para facilitar consumo desde frontend; ASP.NET Core no distingue mayusculas/minusculas por defecto.

## Convenciones generales

- Base URL local habitual: `http://localhost:{puerto}` o `https://localhost:{puerto}` segun `launchSettings.json`.
- Formato de autenticacion: `Authorization: Bearer {token}`.
- Paginacion: los endpoints paginados aceptan `PageNumber` y `PageSize`.
- Respuesta paginada esperada:

```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 125,
  "totalPages": 13,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "items": []
}
```

- Errores comunes:
  - `400 Bad Request`: validacion FluentValidation o regla de dominio.
  - `401 Unauthorized`: token ausente, expirado o invalido.
  - `403 Forbidden`: usuario autenticado sin rol permitido.
  - `404 Not Found`: recurso no encontrado cuando el controller lo devuelve explicitamente.
  - `500 Internal Server Error`: error inesperado.

## Autenticacion JWT

### Login

`POST /api/auth/login`

Body:

```json
{
  "username": "admin",
  "password": "Admin123"
}
```

Respuesta esperada:

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

### Uso del token

Enviar el token recibido en cada endpoint protegido:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Claims principales

El token incluye los claims principales usados por la API:

- `NameIdentifier`: id del usuario.
- `Name`: username.
- `Role`: rol actual del usuario.
- Claims de permisos cuando estan disponibles.

## Resumen de modulos

- Auth
- Me/Profile
- Users
- Roles
- Permissions
- Education
- Transportation
- Notifications
- Incidents
- Tracking
- Reports
- System Settings
- Integrations
- Backups
- Audit Logs

---

## Auth

| Metodo | Ruta | Auth | Roles | Body | Descripcion | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/auth/login` | No | Publico | `LoginCommand` | Autentica usuario y devuelve JWT. | `LoginResponseDto` | 400 credenciales invalidas, usuario inactivo |
| POST | `/api/auth/forgot-password` | No | Publico | `ForgotPasswordCommand` | Recuperacion simple de password por documento y telefono/email. | `204 NoContent` | 400 datos no coinciden |

Body `ForgotPasswordCommand`:

```json
{
  "documentNumber": "00100000000",
  "phoneOrEmail": "8095550000",
  "newPassword": "Nueva123",
  "confirmPassword": "Nueva123"
}
```

---

## Me/Profile

Todos requieren JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/me` | Si | Cualquier usuario | No | No | `MeProfileDto` | 401 |
| GET | `/api/me/students` | Si | Guardian/Admin/Supervisor segun reglas | No | No | Lista de estudiantes visibles | 400 si el usuario no tiene tutor asociado |
| GET | `/api/me/route-assignments` | Si | Driver/TransportAssistant/Admin/Supervisor | No | No | Lista de asignaciones visibles | 400/403 visibilidad |
| GET | `/api/me/trips` | Si | Guardian/Driver/TransportAssistant/Admin/Supervisor | No | No | Lista de viajes visibles | 400/403 visibilidad |
| PUT | `/api/me/profile` | Si | Cualquier usuario | `UpdateMeProfileCommand` | No | `204 NoContent` | 400 validacion |
| PUT | `/api/me/change-password` | Si | Cualquier usuario | `ChangePasswordCommand` | No | `204 NoContent` | 400 password actual invalido |
| PUT | `/api/me/photo` | Si | Cualquier usuario | `UpdateProfilePhotoCommand` | No | `204 NoContent` | 400 validacion |

Ejemplo update profile:

```json
{
  "name": "Maria Perez",
  "email": "maria@example.com",
  "profileImageUrl": "https://cdn.example.com/profile.jpg"
}
```

Ejemplo change password:

```json
{
  "currentPassword": "Admin123",
  "newPassword": "Nueva123",
  "confirmPassword": "Nueva123"
}
```

---

## Users

Controller protegido con `Admin,Supervisor`.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/users` | Si | Admin, Supervisor | No | `PageNumber`, `PageSize` | `PaginatedResponse<UserResponseDto>` | 401, 403 |
| GET | `/api/users/{id}` | Si | Admin, Supervisor | No | `id` guid | `UserDetailDto` | 404/400 no encontrado |
| GET | `/api/users/by-role/{roleId}` | Si | Admin, Supervisor | No | `roleId` guid | Lista `UserResponseDto` | 400 |
| GET | `/api/users/by-active/{isActive}` | Si | Admin, Supervisor | No | `isActive` bool | Lista `UserResponseDto` | 400 |
| PUT | `/api/users/{id}` | Si | Admin, Supervisor | `UpdateUserCommand` | `id` guid | `204 NoContent` | 400 si id no coincide |
| PUT | `/api/users/{id}/activate` | Si | Admin, Supervisor | No | `id` guid | `204 NoContent` | 400 |
| PUT | `/api/users/{id}/deactivate` | Si | Admin, Supervisor | No | `id` guid | `204 NoContent` | 400 |
| PUT | `/api/users/{id}/reset-password` | Si | Admin, Supervisor | `ResetUserPasswordCommand` | `id` guid | `204 NoContent` | 400 validacion |

Ejemplo `UpdateUserCommand`:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Usuario Demo",
  "email": "demo@example.com",
  "roleId": "00000000-0000-0000-0000-000000000000",
  "isActive": true,
  "profileImageUrl": null
}
```

---

## Roles

Controller protegido con `Admin`.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/roles` | Si | Admin | No | No | Lista `RoleResponseDto` | 401, 403 |
| GET | `/api/roles/{id}` | Si | Admin | No | `id` guid | `RoleDetailDto` | 404/400 |
| PUT | `/api/roles/{id}` | Si | Admin | `UpdateRoleCommand` | `id` guid | `204 NoContent` | 400 id no coincide |
| POST | `/api/roles/{roleId}/permissions/{permissionId}` | Si | Admin | No | `roleId`, `permissionId` | `204 NoContent` | 400 duplicado/no encontrado |
| DELETE | `/api/roles/{roleId}/permissions/{permissionId}` | Si | Admin | No | `roleId`, `permissionId` | `204 NoContent` | 400 no encontrado |

---

## Permissions

Controller protegido con `Admin,Supervisor`.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/permissions` | Si | Admin, Supervisor | No | No | Lista `PermissionResponseDto` | 401, 403 |
| GET | `/api/permissions/{id}` | Si | Admin, Supervisor | No | `id` guid | `PermissionResponseDto` | 404/400 |

---

## Education

### Students

Controller protegido con JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/students` | Si | Cualquier autenticado | `CreateStudentCommand` | No | `201 Created` con id | 400 reglas de Guardian/School/Grade |
| GET | `/api/students` | Si | Segun visibilidad | No | `PageNumber`, `PageSize` | `PaginatedResponse<StudentResponseDto>` | 401, 403 |
| GET | `/api/students/{id}` | Si | Segun visibilidad | No | `id` guid | `StudentDetailDto` | 400/403 si no visible |
| GET | `/api/students/code/{code}` | Si | Segun visibilidad | No | `code` string | `StudentDetailDto` | 400/403 |
| GET | `/api/students/by-grade/{gradeId}` | Si | Segun visibilidad | No | `gradeId` guid | Lista `StudentResponseDto` | 400 |
| GET | `/api/students/by-school/{schoolId}` | Si | Segun visibilidad | No | `schoolId` guid | Lista `StudentResponseDto` | 400 |
| GET | `/api/students/by-guardian/{guardianId}` | Si | Segun visibilidad | No | `guardianId` guid | Lista `StudentResponseDto` | 400/403 |
| PUT | `/api/students/{id}` | Si | Cualquier autenticado | `UpdateStudentCommand` | `id` guid | `204 NoContent` | 400 id no coincide/reglas |
| DELETE | `/api/students/{id}` | Si | Cualquier autenticado | No | `id` guid | `204 NoContent` | 400 si tiene asignacion/trip activo |

Ejemplo student:

```json
{
  "firstName": "Luis",
  "lastName": "Garcia",
  "schoolId": "00000000-0000-0000-0000-000000000000",
  "gradeId": "00000000-0000-0000-0000-000000000000",
  "guardianId": "00000000-0000-0000-0000-000000000000"
}
```

### Guardians

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/guardians` | No | Publico/Revisar | `CreateGuardianCommand` | No | `201 Created` con id | 400 documento duplicado |
| GET | `/api/guardians` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<GuardianResponseDto>` | 400 |
| GET | `/api/guardians/{id}` | No | Publico/Revisar | No | `id` guid | `GuardianDetailDto` | 404/400 |
| GET | `/api/guardians/document/{documentNumber}` | No | Publico/Revisar | No | `documentNumber` | `GuardianDetailDto` | 404/400 |
| PUT | `/api/guardians/{id}` | No | Publico/Revisar | `UpdateGuardianCommand` | `id` guid | `204 NoContent` | 400 id no coincide/documento duplicado |
| DELETE | `/api/guardians/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene estudiantes activos |

Ejemplo guardian:

```json
{
  "documentType": 1,
  "documentNumber": "00100000000",
  "firstName": "Ana",
  "lastName": "Perez",
  "phone": "8095550000",
  "street": "Calle 1",
  "city": "Santo Domingo",
  "gender": 1,
  "sectorId": "00000000-0000-0000-0000-000000000000",
  "photoUrl": null
}
```

### Schools

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/schools` | No | Publico/Revisar | `CreateSchoolCommand` | No | `201 Created` con id | 400 nombre duplicado en sector |
| GET | `/api/schools` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<SchoolResponseDto>` | 400 |
| GET | `/api/schools/{id}` | No | Publico/Revisar | No | `id` guid | `SchoolResponseDto`/detalle | 404/400 |
| GET | `/api/schools/by-sector/{sectorId}` | No | Publico/Revisar | No | `sectorId` guid | Lista `SchoolResponseDto` | 400 |
| PUT | `/api/schools/{id}` | No | Publico/Revisar | `UpdateSchoolCommand` | `id` guid | `204 NoContent` | 400 reglas |
| DELETE | `/api/schools/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene estudiantes/rutas activas |

Body create school:

```json
{
  "name": "Colegio Central",
  "directorName": "Director Demo",
  "email": "school@example.com",
  "phone": "8095551111",
  "street": "Av. Principal",
  "city": "Santo Domingo",
  "sectorId": "00000000-0000-0000-0000-000000000000"
}
```

### Grades

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/grades` | No | Publico/Revisar | `CreateGradeCommand` | No | `201 Created` con id | 400 duplicado/school inactiva |
| GET | `/api/grades` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<GradeResponseDto>` | 400 |
| GET | `/api/grades/{id}` | No | Publico/Revisar | No | `id` guid | `GradeResponseDto`/detalle | 404/400 |
| GET | `/api/grades/by-school/{schoolId}` | No | Publico/Revisar | No | `schoolId` guid | Lista `GradeResponseDto` | 400 |
| PUT | `/api/grades/{id}` | No | Publico/Revisar | `UpdateGradeCommand` | `id` guid | `204 NoContent` | 400 reglas |
| DELETE | `/api/grades/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene estudiantes |

### Sectors

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/sectors` | No | Publico/Revisar | `CreateSectorCommand` | No | `201 Created` con id | 400 validacion |
| GET | `/api/sectors` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<SectorResponseDto>` | 400 |
| GET | `/api/sectors/{id}` | No | Publico/Revisar | No | `id` guid | `SectorResponseDto`/detalle | 404/400 |
| PUT | `/api/sectors/{id}` | No | Publico/Revisar | `UpdateSectorCommand` | `id` guid | `204 NoContent` | 400 reglas |
| DELETE | `/api/sectors/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene schools/guardians/stops |

Body create sector:

```json
{
  "name": "Sector Norte",
  "province": "Santo Domingo",
  "city": "Santo Domingo"
}
```

---

## Transportation

### Vehicles

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/vehicles` | No | Publico/Revisar | `CreateVehicleCommand` | No | `201 Created` con id | 400 placa duplicada/capacidad |
| GET | `/api/vehicles` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<VehicleResponseDto>` | 400 |
| GET | `/api/vehicles/{id}` | No | Publico/Revisar | No | `id` guid | `VehicleDetailDto` | 404/400 |
| GET | `/api/vehicles/plate/{plateNumber}` | No | Publico/Revisar | No | `plateNumber` | `VehicleDetailDto` | 404/400 |
| GET | `/api/vehicles/by-status/{status}` | No | Publico/Revisar | No | `status` enum | Lista `VehicleResponseDto` | 400 |
| PUT | `/api/vehicles/{id}` | No | Publico/Revisar | `UpdateVehicleCommand` | `id` guid | `204 NoContent` | 400 reglas |
| DELETE | `/api/vehicles/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene trip/asignacion activa |

Body create vehicle:

```json
{
  "plateNumber": "A123456",
  "capacity": 30
}
```

### Drivers

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/drivers` | No | Publico/Revisar | `CreateDriverCommand` | No | `201 Created` con id | 400 documento/licencia duplicada |
| GET | `/api/drivers` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<DriverResponseDto>` | 400 |
| GET | `/api/drivers/{id}` | No | Publico/Revisar | No | `id` guid | `DriverDetailDto` | 404/400 |
| GET | `/api/drivers/license/{licenseNumber}` | No | Publico/Revisar | No | `licenseNumber` | `DriverDetailDto` | 404/400 |
| GET | `/api/drivers/by-active/{isActive}` | No | Publico/Revisar | No | `isActive` bool | Lista `DriverResponseDto` | 400 |
| PUT | `/api/drivers/{id}` | No | Publico/Revisar | `UpdateDriverCommand` | `id` guid | `204 NoContent` | 400 reglas |
| DELETE | `/api/drivers/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene trip/asignacion activa |

Body create driver:

```json
{
  "firstName": "Carlos",
  "lastName": "Diaz",
  "documentType": 1,
  "documentNumber": "00200000000",
  "licenseNumber": "LIC-12345",
  "phone": "8095552222",
  "street": "Calle 2",
  "city": "Santo Domingo",
  "email": "driver@example.com",
  "photoUrl": null
}
```

### Transport Assistants

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/transport-assistants` | No | Publico/Revisar | `CreateTransportAssistantCommand` | No | `201 Created` con id | 400 documento duplicado |
| GET | `/api/transport-assistants` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<TransportAssistantResponseDto>` | 400 |
| GET | `/api/transport-assistants/{id}` | No | Publico/Revisar | No | `id` guid | `TransportAssistantDetailDto` | 404/400 |
| GET | `/api/transport-assistants/document/{documentNumber}` | No | Publico/Revisar | No | `documentNumber` | `TransportAssistantDetailDto` | 404/400 |
| GET | `/api/transport-assistants/by-active/{isActive}` | No | Publico/Revisar | No | `isActive` bool | Lista `TransportAssistantResponseDto` | 400 |
| PUT | `/api/transport-assistants/{id}` | No | Publico/Revisar | `UpdateTransportAssistantCommand` | `id` guid | `204 NoContent` | 400 reglas |
| DELETE | `/api/transport-assistants/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene trip/asignacion activa |

### Stops

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/stops` | No | Publico/Revisar | `CreateStopCommand` | No | `201 Created` con id | 400 validacion |
| GET | `/api/stops` | No | Publico/Revisar | No | `PageNumber`, `PageSize` | `PaginatedResponse<StopResponseDto>` | 400 |
| GET | `/api/stops/{id}` | No | Publico/Revisar | No | `id` guid | `StopDetailDto` | 404/400 |
| GET | `/api/stops/by-sector/{sectorId}` | No | Publico/Revisar | No | `sectorId` guid | Lista `StopResponseDto` | 400 |
| GET | `/api/stops/by-city/{city}` | No | Publico/Revisar | No | `city` | Lista `StopResponseDto` | 400 |
| PUT | `/api/stops/{id}` | No | Publico/Revisar | `UpdateStopCommand` | `id` guid | `204 NoContent` | 400 reglas |
| DELETE | `/api/stops/{id}` | No | Publico/Revisar | No | `id` guid | `204 NoContent` | 400 si tiene route stops |

Body create stop:

```json
{
  "name": "Parada Central",
  "street": "Av. Principal",
  "city": "Santo Domingo",
  "latitude": 18.4861,
  "longitude": -69.9312,
  "sectorId": "00000000-0000-0000-0000-000000000000"
}
```

### Routes

Controller protegido con JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/routes` | Si | Cualquier autenticado | `CreateRouteCommand` | No | `201 Created` con id | 400 school inexistente/inactiva, horario invalido, nombre duplicado |
| GET | `/api/routes` | Si | Segun visibilidad | No | `PageNumber`, `PageSize` | `PaginatedResponse<RouteResponseDto>` | 401/403 |
| GET | `/api/routes/{id}` | Si | Segun visibilidad | No | `id` guid | `RouteDetailDto` | 400/403 |
| GET | `/api/routes/by-school/{schoolId}` | Si | Segun visibilidad | No | `schoolId` guid | Lista `RouteResponseDto` | 400 |
| GET | `/api/routes/by-status/{status}` | Si | Segun visibilidad | No | `status` enum | Lista `RouteResponseDto` | 400 |
| PUT | `/api/routes/{id}` | Si | Cualquier autenticado | `UpdateRouteCommand` | `id` guid | `204 NoContent` | 400 trip activo/horario/nombre |
| DELETE | `/api/routes/{id}` | Si | Cualquier autenticado | No | `id` guid | `204 NoContent` | 400 trip activo |
| POST | `/api/routes/{routeId}/stops` | Si | Cualquier autenticado | `AddStopToRouteCommand` | `routeId` guid | `204 NoContent` | 400 stop no existe, orden duplicado, trip activo |
| DELETE | `/api/routes/{routeId}/stops/{stopId}` | Si | Cualquier autenticado | No | `routeId`, `stopId` | `204 NoContent` | 400 trip activo/ruta activa con una parada |
| PUT | `/api/routes/{routeId}/stops/{stopId}/order` | Si | Cualquier autenticado | `UpdateRouteStopOrderCommand` | `routeId`, `stopId` | `204 NoContent` | 400 orden duplicado/trip activo |

Body create route:

```json
{
  "name": "Ruta Norte",
  "schoolId": "00000000-0000-0000-0000-000000000000",
  "startTime": "2026-05-28T07:00:00",
  "endTime": "2026-05-28T08:00:00"
}
```

Body add/update route stop:

```json
{
  "routeId": "00000000-0000-0000-0000-000000000000",
  "stopId": "00000000-0000-0000-0000-000000000000",
  "stopOrder": 1
}
```

### Route Assignments

Controller protegido con JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/route-assignments` | Si | Cualquier autenticado | `CreateRouteAssignmentCommand` | No | `201 Created` con id | 400 entidades inactivas/conflicto horario/capacidad |
| GET | `/api/route-assignments` | Si | Segun visibilidad | No | `PageNumber`, `PageSize` | `PaginatedResponse<RouteAssignmentResponseDto>` | 401/403 |
| GET | `/api/route-assignments/{id}` | Si | Segun visibilidad | No | `id` guid | `RouteAssignmentDetailDto` | 400/403 |
| GET | `/api/route-assignments/by-route/{routeId}` | Si | Segun visibilidad | No | `routeId` guid | Lista `RouteAssignmentResponseDto` | 400 |
| GET | `/api/route-assignments/by-driver/{driverId}` | Si | Segun visibilidad | No | `driverId` guid | Lista `RouteAssignmentResponseDto` | 400/403 |
| GET | `/api/route-assignments/by-vehicle/{vehicleId}` | Si | Segun visibilidad | No | `vehicleId` guid | Lista `RouteAssignmentResponseDto` | 400 |
| PUT | `/api/route-assignments/{id}` | Si | Cualquier autenticado | `UpdateRouteAssignmentCommand` | `id` guid | `204 NoContent` | 400 si tiene trips/conflictos |
| DELETE | `/api/route-assignments/{id}` | Si | Cualquier autenticado | No | `id` guid | `204 NoContent` | 400 si tiene trips |
| POST | `/api/route-assignments/{assignmentId}/students/{studentId}` | Si | Cualquier autenticado | No | `assignmentId`, `studentId` | `204 NoContent` | 400 duplicado/capacidad/student inactivo |
| DELETE | `/api/route-assignments/{assignmentId}/students/{studentId}` | Si | Cualquier autenticado | No | `assignmentId`, `studentId` | `204 NoContent` | 400 no encontrado |

Body route assignment:

```json
{
  "routeId": "00000000-0000-0000-0000-000000000000",
  "driverId": "00000000-0000-0000-0000-000000000000",
  "vehicleId": "00000000-0000-0000-0000-000000000000",
  "transportAssistantId": null,
  "vehicleCapacity": 30
}
```

### Trips

Controller protegido con JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/trips/start` | Si | Cualquier autenticado | `StartTripCommand` | No | `201 Created` con id | 400 ya existe trip activo/sin estudiantes/sin paradas |
| PUT | `/api/trips/{id}/end` | Si | Cualquier autenticado | No | `id` guid | `204 NoContent` | 400 si no esta en progreso |
| PUT | `/api/trips/{id}/cancel` | Si | Cualquier autenticado | No | `id` guid | `204 NoContent` | 400 estado invalido |
| GET | `/api/trips` | Si | Segun visibilidad | No | `PageNumber`, `PageSize` | `PaginatedResponse<TripResponseDto>` | 401/403 |
| GET | `/api/trips/{id}` | Si | Segun visibilidad | No | `id` guid | `TripDetailDto` | 400/403 |
| GET | `/api/trips/by-route-assignment/{routeAssignmentId}` | Si | Segun visibilidad | No | `routeAssignmentId` guid | Lista `TripResponseDto` | 400/403 |
| GET | `/api/trips/by-status/{status}` | Si | Segun visibilidad | No | `status` enum | Lista `TripResponseDto` | 400/403 |
| GET | `/api/trips/by-date-range` | Si | Segun visibilidad | No | `startDate`, `endDate` | Lista `TripResponseDto` | 400 rango invalido |
| GET | `/api/trips/active/by-route-assignment/{routeAssignmentId}` | Si | Segun visibilidad | No | `routeAssignmentId` guid | `TripDetailDto` o null | 400/403 |

Body start trip:

```json
{
  "routeAssignmentId": "00000000-0000-0000-0000-000000000000"
}
```

---

## Notifications

Todos requieren JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/notifications` | Si | Admin/Supervisor todas; otros solo propias | No | `PageNumber`, `PageSize` | `PaginatedResponse<NotificationResponseDto>` | 401 |
| GET | `/api/notifications/unread` | Si | Usuario autenticado | No | No | Lista `NotificationResponseDto` | 401 |
| PUT | `/api/notifications/{id}/read` | Si | Usuario autenticado | No | `id` guid | `204 NoContent` | 400/403 no visible |
| PUT | `/api/notifications/read-all` | Si | Usuario autenticado | No | No | `204 NoContent` | 401 |

---

## Incidents

Todos requieren JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/incidents` | Si | Cualquier autenticado | `ReportIncidentCommand` | No | `201 Created` con id | 400 validacion/visibilidad |
| GET | `/api/incidents` | Si | Admin/Supervisor todos; otros segun visibilidad | No | `PageNumber`, `PageSize` | `PaginatedResponse<IncidentResponseDto>` | 403 |
| GET | `/api/incidents/{id}` | Si | Segun visibilidad | No | `id` guid | `IncidentDetailDto` | 400/403 |
| GET | `/api/incidents/by-status/{status}` | Si | Segun visibilidad | No | `status` enum | Lista `IncidentResponseDto` | 400 |
| GET | `/api/incidents/by-severity/{severity}` | Si | Segun visibilidad | No | `severity` enum | Lista `IncidentResponseDto` | 400 |
| GET | `/api/incidents/by-trip/{tripId}` | Si | Segun visibilidad | No | `tripId` guid | Lista `IncidentResponseDto` | 400/403 |
| GET | `/api/incidents/by-route-assignment/{routeAssignmentId}` | Si | Segun visibilidad | No | `routeAssignmentId` guid | Lista `IncidentResponseDto` | 400/403 |
| GET | `/api/incidents/my-reported` | Si | Usuario autenticado | No | No | Lista `IncidentResponseDto` | 401 |
| PUT | `/api/incidents/{id}/assign/{userId}` | Si | Admin, Supervisor | No | `id`, `userId` | `204 NoContent` | 403/400 |
| PUT | `/api/incidents/{id}/in-progress` | Si | Segun handler | No | `id` guid | `204 NoContent` | 400 estado invalido |
| PUT | `/api/incidents/{id}/resolve` | Si | Admin/Supervisor/asignado | No | `id` guid | `204 NoContent` | 403/400 |
| PUT | `/api/incidents/{id}/close` | Si | Admin, Supervisor | No | `id` guid | `204 NoContent` | 400 si no resuelto |
| PUT | `/api/incidents/{id}/cancel` | Si | Admin, Supervisor | No | `id` guid | `204 NoContent` | 400 estado invalido |
| POST | `/api/incidents/{id}/comments` | Si | Segun visibilidad | `AddIncidentCommentCommand` | `id` guid | `204 NoContent` | 400 comentario requerido |

Body report incident:

```json
{
  "title": "Retraso en ruta",
  "description": "La ruta presenta un retraso de 10 minutos.",
  "type": 1,
  "severity": 2,
  "tripId": null,
  "routeAssignmentId": null,
  "vehicleId": null,
  "driverId": null,
  "transportAssistantId": null
}
```

---

## Tracking

Todos requieren JWT.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/tracking/location` | Si | Admin, Supervisor, Driver asignado, Assistant asignado | `UpdateVehicleLocationCommand` | No | `204 NoContent` | 400 trip no en progreso/ubicacion invalida, 403 |
| GET | `/api/tracking/trips/{tripId}/current-location` | Si | Segun visibilidad | No | `tripId` guid | `CurrentTripLocationDto` | 400/403 |
| GET | `/api/tracking/trips/{tripId}/history` | Si | Segun visibilidad | No | `tripId` guid | Lista `VehicleLocationResponseDto` | 400/403 |
| GET | `/api/tracking/active-trips` | Si | Admin, Supervisor, Driver, Assistant | No | No | Lista ubicaciones actuales | 401/403 |
| GET | `/api/tracking/my-students` | Si | Guardian | No | No | Lista ubicaciones de estudiantes propios | 400 si usuario no tiene Guardian asociado |

Body update location:

```json
{
  "tripId": "00000000-0000-0000-0000-000000000000",
  "latitude": 18.4861,
  "longitude": -69.9312,
  "speed": 42.5,
  "heading": 90
}
```

---

## Reports

Controller protegido con `Admin,Supervisor`.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/reports/dashboard` | Si | Admin, Supervisor | No | No | `DashboardReportDto` | 401, 403 |
| GET | `/api/reports/trips` | Si | Admin, Supervisor | No | `startDate`, `endDate` | Lista `TripReportDto` | 400 rango invalido |
| GET | `/api/reports/students-by-route/{routeId}` | Si | Admin, Supervisor | No | `routeId` guid | `StudentsByRouteReportDto` | 400 |
| GET | `/api/reports/incidents` | Si | Admin, Supervisor | No | `startDate`, `endDate` | Lista `IncidentReportDto` | 400 |
| GET | `/api/reports/drivers-performance` | Si | Admin, Supervisor | No | `startDate`, `endDate` | Lista `DriverPerformanceReportDto` | 400 |
| GET | `/api/reports/vehicles-usage` | Si | Admin, Supervisor | No | `startDate`, `endDate` | Lista `VehicleUsageReportDto` | 400 |
| GET | `/api/reports/audit-summary` | Si | Admin, Supervisor | No | `startDate`, `endDate` | Lista `AuditSummaryReportDto` | 400 |

Ejemplo de rango:

```http
GET /api/reports/trips?startDate=2026-05-01&endDate=2026-05-28
```

---

## System Settings

GET protegido con `Admin,Supervisor`. Mutaciones protegidas con `Admin`.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/system-settings` | Si | Admin, Supervisor | No | No | Lista `SystemSettingResponseDto` | 401, 403 |
| GET | `/api/system-settings/{id}` | Si | Admin, Supervisor | No | `id` guid | `SystemSettingResponseDto` | 404/400 |
| GET | `/api/system-settings/key/{key}` | Si | Admin, Supervisor | No | `key` | `SystemSettingResponseDto` | 404/400 |
| GET | `/api/system-settings/by-category/{category}` | Si | Admin, Supervisor | No | `category` | Lista `SystemSettingResponseDto` | 400 |
| POST | `/api/system-settings` | Si | Admin | `CreateSystemSettingCommand` | No | `201 Created` con id | 400 key duplicada |
| PUT | `/api/system-settings/{id}` | Si | Admin | `UpdateSystemSettingCommand` | `id` guid | `204 NoContent` | 400 no editable/id no coincide |
| DELETE | `/api/system-settings/{id}` | Si | Admin | No | `id` guid | `204 NoContent` | 400 setting base/no editable |

Body create setting:

```json
{
  "key": "GPS.SpeedLimitKmH",
  "value": "80",
  "description": "Limite de velocidad para alertas.",
  "category": "GPS",
  "dataType": "int",
  "isEditable": true
}
```

---

## Integrations

Controller protegido con `Admin`. Son endpoints de prueba para adaptadores mock/locales; no envian proveedores reales en esta fase.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/integrations/test-email` | Si | Admin | `TestEmailRequest` | No | `200 OK` | 403 |
| POST | `/api/integrations/test-sms` | Si | Admin | `TestPhoneMessageRequest` | No | `200 OK` | 403 |
| POST | `/api/integrations/test-whatsapp` | Si | Admin | `TestPhoneMessageRequest` | No | `200 OK` | 403 |
| POST | `/api/integrations/test-push` | Si | Admin | `TestPushRequest` | No | `200 OK` | 403 |
| GET | `/api/integrations/test-distance` | Si | Admin | No | `originLat`, `originLng`, `destinationLat`, `destinationLng` | Distancia/tiempo estimado | 400 parametros invalidos |

Body test email:

```json
{
  "to": "demo@example.com",
  "subject": "Prueba",
  "body": "Mensaje de prueba"
}
```

---

## Backups

Controller protegido con `Admin`.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| POST | `/api/backups/manual` | Si | Admin | No | No | `BackupRecordResponseDto` | 400 si falla creacion |
| GET | `/api/backups` | Si | Admin | No | `PageNumber`, `PageSize` | `PaginatedResponse<BackupRecordResponseDto>` | 403 |
| GET | `/api/backups/latest` | Si | Admin | No | No | `BackupRecordResponseDto` o null | 403 |
| GET | `/api/backups/{id}` | Si | Admin | No | `id` guid | `BackupRecordResponseDto` | 404/400 |
| POST | `/api/backups/{id}/restore` | Si | Admin | No | `id` guid | Error controlado/placeholder | 400 restore no implementado |

Nota: restore destructivo no esta implementado en esta fase.

---

## Audit Logs

Controller protegido con `Admin,Supervisor`.

| Metodo | Ruta | Auth | Roles | Body | Parametros | Respuesta | Errores comunes |
|---|---|---:|---|---|---|---|---|
| GET | `/api/audit-logs` | Si | Admin, Supervisor | No | `PageNumber`, `PageSize` | `PaginatedResponse<AuditLogResponseDto>` | 401, 403 |
| GET | `/api/audit-logs/by-user/{userId}` | Si | Admin, Supervisor | No | `userId` guid | Lista `AuditLogResponseDto` | 400 |
| GET | `/api/audit-logs/by-entity/{entityName}/{entityId}` | Si | Admin, Supervisor | No | `entityName`, `entityId` | Lista `AuditLogResponseDto` | 400 |
| GET | `/api/audit-logs/by-action/{action}` | Si | Admin, Supervisor | No | `action` | Lista `AuditLogResponseDto` | 400 |
| GET | `/api/audit-logs/by-date-range` | Si | Admin, Supervisor | No | `startDate`, `endDate` | Lista `AuditLogResponseDto` | 400 rango invalido |

---

## Respuestas DTO principales

### Detail DTOs

Los endpoints de detalle devuelven DTOs planos. No exponen ValueObjects completos. Ejemplos:

- `StudentDetailDto`: `studentCode`, `schoolName`, `gradeName`, `guardianName`, `isActive`.
- `GuardianDetailDto`: `documentType`, `documentNumber`, `street`, `city`, `sectorName`.
- `DriverDetailDto`: `licenseNumber`, `phone`, `email`, `street`, `city`.
- `RouteDetailDto`: datos de ruta y lista de stops.
- `RouteAssignmentDetailDto`: route, driver, vehicle, assistant y students asignados.
- `TripDetailDto`: assignment, route, driver, vehicle y estado.

### Enums

Los parametros enum pueden enviarse como numero o texto si la configuracion JSON lo permite. Para evitar ambiguedad, el frontend puede consultar Swagger para los valores exactos de:

- `DocumentType`
- `Gender`
- `VehicleStatus`
- `RouteStatus`
- `TripStatus`
- `IncidentType`
- `IncidentSeverity`
- `IncidentStatus`
- `NotificationType`
- `NotificationPriority`

## Endpoints marcados para revisar

Los controllers de `Guardians`, `Schools`, `Grades`, `Sectors`, `Vehicles`, `Drivers`, `TransportAssistants` y `Stops` no muestran atributo `[Authorize]` a nivel de controller en el codigo revisado. En esta documentacion se marcaron como `Publico/Revisar` porque pueden depender de reglas globales, filtros, middleware o decisiones futuras de seguridad.

## Conteo aproximado

Se detectaron aproximadamente 165 acciones HTTP documentables en `Transport.API\Controllers`.
