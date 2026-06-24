# Auditoria de concurrencia, transacciones e integridad de datos

Proyecto: Plataforma Web para el Sistema de Transporte Escolar TRAE  
Fase: QA-3 - Concurrencia, transacciones e integridad de datos  
Fecha: 2026-06-22

## Resumen ejecutivo

La auditoria reviso operaciones criticas de escritura en viajes, programacion, asistencia, asignaciones, incidentes, tracking, notificaciones y backups. Se detectaron riesgos de carrera en operaciones con patron `validar -> escribir`, ausencia de concurrencia optimista en entidades criticas, indices faltantes y advertencias de EF Core por filtros globales sobre entidades historicas requeridas.

Se implemento una unidad transaccional ligera basada en EF Core, indices de integridad, guardas previas de migracion, `RowVersion` en entidades criticas, respuesta HTTP `409 Conflict` para conflictos de concurrencia/unicidad y bloqueo de backup simultaneo por instancia.

No se modifico frontend.

## Cambios aplicados

- Se creo `IUnitOfWork` en Application.
- Se creo `UnitOfWork` en Infrastructure usando `AppDbContext.Database.BeginTransactionAsync`.
- Se registro `IUnitOfWork` en DI.
- Se envolvieron operaciones criticas con transacciones.
- Se agregaron indices/constraints EF Core:
  - `Trips.RouteAssignmentId` unico filtrado para `Status = 'InProgress'`.
  - `RouteStops.RouteId + StopId` unico.
  - `Notifications.UserId + IsRead + CreatedAt`.
  - `AuditLogs.UserId + CreatedAt`.
- Se agregaron guardas SQL antes de crear indices unicos para detectar datos duplicados existentes sin producir errores ambiguos.
- Se agrego `RowVersion` a `Trips`, `RouteAssignments`, `TripStudentAttendances`, `Incidents` y `BackupRecords`.
- Se ajusto `ExceptionMiddleware` para devolver `409 Conflict` ante `DbUpdateConcurrencyException` y violaciones de unicidad.
- Se bloqueo la ejecucion simultanea de backup manual en la misma instancia de API.
- Se ajusto el filtro global de `IActivatable` para no ocultar entidades historicas requeridas (`User`, `Student`, `Guardian`, `Driver`, `School`) y se mantuvo el filtrado activo explicito en repositorios.
- Se creo el smoke test `scripts/smoke-test-concurrency-integrity.ps1`.

## Matriz de auditoria

| Area | Operacion | Riesgo detectado | Impacto | Correccion aplicada | Requiere migracion | Estado |
|---|---|---|---|---|---|---|
| Trips | Iniciar viaje | Carrera entre `HasActiveTripAsync` y creacion/inicio del viaje. | Dos viajes `InProgress` para la misma asignacion. | Transaccion `Serializable` en `StartTripHandler`, unique filtered index por `RouteAssignmentId` cuando `Status = InProgress` y `RowVersion`. | Si | Corregido |
| Trips | Crear snapshot de pasajeros | `Trip` y `TripStudentAttendance` se guardaban como unidad logica sin transaccion explicita. | Viaje iniciado sin snapshot completo si falla una escritura intermedia. | `StartTripHandler` guarda inicio y snapshot dentro de transaccion. | No | Corregido |
| Trips | Finalizar/cancelar/NotOperating | Riesgo de transiciones invalidas. | Estados operativos inconsistentes. | Dominio valida transiciones y `RowVersion` detecta modificaciones concurrentes. | Si | Corregido |
| TripSchedules | Materializar viaje | Carrera entre validar duplicado y crear trip. | Duplicados para mismo `TripScheduleId + OperationDate`. | Transaccion `Serializable` y unique index filtrado existente. | No nueva | Corregido |
| TripSchedules | Crear/actualizar horarios | Horarios semanticamente solapados no identicos. | Confusion operativa. | Validaciones de Application; constraint temporal complejo queda documentado para fase futura. | No | Pendiente controlado |
| TripStudentAttendance | Snapshot de pasajeros | Duplicado `TripId + StudentId`. | Pasajero repetido en viaje. | PK compuesta existente protege duplicados. | No | Correcto |
| TripStudentAttendance | Agregar pasajero excepcional | Carrera entre `ExistsAsync` y `AddAsync`. | Duplicado por dos solicitudes simultaneas. | Transaccion `Serializable`; PK compuesta mantiene proteccion final. | No | Corregido |
| TripStudentAttendance | Cambiar asistencia | Estado puede cambiar conflictivamente en requests simultaneos. | Ultima escritura gana. | Reglas de estado en dominio, validacion `InProgress` y `RowVersion`. | Si | Corregido |
| RouteAssignments | Asignar estudiantes | Dos usuarios pueden validar capacidad simultaneamente. | Superar capacidad con estudiantes distintos. | Transaccion `Serializable` en `AssignStudentToRouteAssignmentHandler`; PK compuesta bloquea duplicado del mismo estudiante y `RowVersion` detecta modificaciones concurrentes. | Si | Corregido |
| RouteAssignments | Remover estudiante | Puede chocar con inicio de viaje simultaneo. | Remover estudiante mientras se inicia snapshot. | Handler bloquea si hay trip `InProgress`; unique active trip y transaccion de inicio reducen la carrera. | No | Mitigado |
| RouteStops | Agregar paradas | Existia orden unico, pero no StopId unico por ruta. | Misma parada repetida con distinto orden. | Unique index `RouteId + StopId` con guarda previa de duplicados. | Si | Corregido |
| Tracking | Registrar ubicacion | Tracking sobre viaje finalizado/cancelado. | Historial GPS inconsistente. | Handler valida `TripStatus.InProgress`. | No | Correcto |
| Tracking | Consultas por historial | Necesita indice por `TripId + RecordedAt`. | Lecturas lentas con muchos puntos GPS. | Indice existente verificado. | No | Correcto |
| Incidents | Transiciones de estado | Requests simultaneos pueden pisar estado. | Cierre/resolucion concurrente con resultado ultimo-gana. | Reglas de dominio protegen transiciones invalidas y `RowVersion` detecta escrituras concurrentes. | Si | Corregido |
| Notifications | Mark read / read all | Solicitudes simultaneas son idempotentes. | Bajo. | Sin cambio funcional; se agrego indice compuesto para bandeja por usuario/no leidas. | Si | Corregido |
| AuditLogs | Consultas por usuario/fecha | Faltaba indice compuesto. | Lentitud en auditoria por usuario. | Indice `UserId + CreatedAt`. | Si | Corregido |
| Backups | Crear backup manual | No habia bloqueo explicito de backup simultaneo. | Dos backups concurrentes consumen IO y generan registros paralelos. | `BackupService` usa `SemaphoreSlim` por instancia y devuelve error controlado si ya hay backup en ejecucion. | No | Corregido |
| EF query filters | Relaciones requeridas con entidades `IActivatable`. | EF podia ocultar entidades requeridas en historicos. | Lecturas historicas incompletas si User/Student/Guardian/Driver/School estaba inactivo. | Se excluyeron esas entidades del filtro global y se agrego filtrado activo explicito en repositorios operativos. | No | Corregido |

## Transacciones agregadas

### StartTripHandler

Se usa `IsolationLevel.Serializable` para revalidar viaje activo, crear/iniciar `Trip`, crear snapshot de pasajeros y guardar cambios atomicos.

### MaterializeTripScheduleHandler

Se usa `IsolationLevel.Serializable` para revalidar duplicado por schedule/fecha y crear `Trip Scheduled` o `Trip NotOperating`.

### AddExceptionalPassengerHandler

Se usa `IsolationLevel.Serializable` para revalidar existencia del pasajero, crear `TripStudentAttendance` y guardar asistencia.

### ReportTripRouteDeviationHandler

Se usa transaccion para persistir el desvio como unidad de escritura. Auditoria y notificacion quedan posteriores y no criticas.

### AssignStudentToRouteAssignmentHandler

Se usa `IsolationLevel.Serializable` para cargar asignacion y estudiante, validar capacidad, validar duplicado, validar conflicto de horario, agregar estudiante y guardar.

## Indices revisados

| Tabla | Indice/constraint | Estado |
|---|---|---|
| Trips | `TripScheduleId + OperationDate` unico filtrado | Existente |
| Trips | `RouteAssignmentId` unico filtrado por `Status = 'InProgress'` | Agregado |
| TripStudentAttendances | PK compuesta `TripId + StudentId` | Existente |
| StudentRouteAssignments | PK compuesta `StudentId + RouteAssignmentId` | Existente |
| RouteStops | `RouteId + StopOrder` unico | Existente |
| RouteStops | `RouteId + StopId` unico | Agregado |
| VehicleLocations | `TripId + RecordedAt` | Existente |
| Notifications | `UserId + IsRead + CreatedAt` | Agregado |
| AuditLogs | `UserId + CreatedAt` | Agregado |
| BackupRecords | `CreatedAt`, `Status`, `Type` | Existente |

## RowVersion

Se agrego concurrencia optimista (`RowVersion`) a:

- Trip
- RouteAssignment
- TripStudentAttendance
- Incident
- BackupRecord

El middleware de excepciones responde `409 Conflict` cuando EF detecta que un registro fue modificado por otro proceso.

## Aplicacion de migraciones

Se aplicaron correctamente:

- `20260622051701_AddConcurrencyIntegrityIndexes`
- `20260622053333_AddOptimisticConcurrencyAndQueryFilterFixes`

La migracion de indices incluye validaciones previas para detener la aplicacion con un mensaje claro si la base contiene duplicados incompatibles.

## Smoke test

Se creo `scripts/smoke-test-concurrency-integrity.ps1`.

Validaciones previstas:

- doble materializacion de horario/fecha
- doble inicio de viaje
- transicion invalida de asistencia
- tracking en viaje completado

## Riesgos pendientes

1. El bloqueo de backup simultaneo es por instancia de API. Si el sistema se despliega en multiples instancias, conviene reforzarlo con bloqueo distribuido o constraint/estado en base de datos.
2. EF Core mantiene advertencias informativas para owned types opcionales `Email` en Driver y TransportAssistant. No generan FK fantasma ni afectan integridad historica; se pueden revisar en una limpieza futura de owned value objects.
