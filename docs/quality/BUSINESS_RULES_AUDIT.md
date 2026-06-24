# Auditoria de reglas de negocio

Proyecto: Plataforma Web para el Sistema de Transporte Escolar TRAE  
Fase: QA-2 - Auditoria de reglas de negocio, logica duplicada e inconsistencias operativas  
Fecha: 2026-06-22

## Resumen

La revision se concentro en viajes, horarios, asistencia, pasajeros excepcionales, desvios, tracking, asignaciones, incidencias, notificaciones y auditoria. En general, las reglas criticas del ciclo de viaje ya estan ubicadas en Domain y los handlers coordinan correctamente repositorios, autorizacion operacional, auditoria y notificaciones.

Se corrigieron dos inconsistencias de asistencia:

1. Un estudiante marcado como `Boarded` podia cambiar a `Absent`, lo cual generaba un historial contradictorio.
2. Los comandos de asistencia no bloqueaban explicitamente actualizaciones cuando el viaje ya no estaba `InProgress`.

No se agregaron funcionalidades nuevas y no se requirieron migraciones.

## Matriz de hallazgos

| Area | Regla actual | Problema encontrado | Riesgo | Correccion aplicada | Estado |
|---|---|---|---|---|---|
| Trips - StartTrip | Solo `Scheduled` puede pasar a `InProgress`. | La regla esta en `Trip.Start()`. Handler valida asignacion, ruta, vehiculo, conductor, asistente, estudiantes y autorizacion. | Bajo. | Sin cambios funcionales. | Correcto |
| Trips - EndTrip | Solo `InProgress` puede finalizar. | Mensaje de dominio estaba en ingles. | Bajo; experiencia/API inconsistente. | Se cambio mensaje a español: `El viaje no está en progreso`. | Corregido |
| Trips - CancelTrip | Solo `Scheduled` puede cancelarse y requiere razon. | Regla esta en dominio y command usa reason. | Bajo. | Sin cambios. | Correcto |
| Trips - NotOperating | Solo `Scheduled` puede marcarse como no operativo y requiere razon. | Regla esta en dominio. | Bajo. | Sin cambios. | Correcto |
| Trips - Early start | Inicio anticipado requiere `ForceEarlyStart` y razon; solo Admin/Supervisor desde handler. | Regla repartida correctamente: permiso en Application, invariantes en Domain. | Bajo. | Sin cambios. | Correcto |
| Trips - Delay | `DelayMinutes`, `IsLate` y `PunctualityStatus` se calculan al iniciar. | No se detecto comparacion por numero magico; usa enum. | Bajo. | Sin cambios. | Correcto |
| TripSchedule materialization | Puede crear `Scheduled` o `NotOperating` segun calendario/dia. | Regla de fin de semana y NonSchoolDay esta en handler por depender de repositorios/calendario. | Bajo. | Sin cambios. | Correcto |
| NonSchoolDays | Dia global o por escuela puede marcar no operacion. | Aplicacion valida duplicados y materializacion lo respeta. | Bajo. | Sin cambios. | Correcto |
| TripStudentAttendance | `Expected -> Boarded -> DroppedOff`; `Expected -> Absent`. | Se permitia `Boarded -> Absent`. | Alto; historial de asistencia contradictorio. | `TripStudentAttendance.MarkAbsent()` ahora bloquea estudiantes ya abordados. | Corregido |
| TripStudentAttendance | Asistencia solo debe cambiar durante viaje en progreso. | Handler validaba permiso, pero no estado `InProgress` antes de mutar asistencia. | Alto; se podria modificar asistencia de viaje completado/cancelado si el usuario tenia permiso. | `TripStudentAttendanceCommandHandler` valida `trip.Status == InProgress`. | Corregido |
| Passenger snapshot | Al iniciar viaje se crean pasajeros esperados desde asignacion. | Regla valida duplicados con `ExistsForTripAsync` y bloquea viaje sin estudiantes activos. | Bajo. | Sin cambios. | Correcto |
| Pasajeros excepcionales | Solo en viaje `InProgress`, no duplicados, no modifica asignacion oficial. | Regla esta duplicada parcialmente entre handler y dominio, pero no es innecesaria: handler valida repositorio/autorizacion; dominio protege invariante. | Bajo. | Sin cambios. | Correcto |
| Pasajeros excepcionales | Deben iniciar `Boarded`, con razon y usuario registrador. | Regla esta en factory de dominio. | Bajo. | Sin cambios. | Correcto |
| Desvios de ruta | Solo se reportan en viaje `InProgress`. | Regla esta en `Trip.ReportRouteDeviation()`. | Bajo. | Sin cambios. | Correcto |
| Desvios de ruta | Guardian no puede reportar; Driver/Assistant solo asignados. | Autorizacion operacional centralizada en `ITripOperationAuthorizationService`. | Bajo. | Sin cambios. | Correcto |
| Tracking GPS | Solo se registra ubicacion en viaje `InProgress`. | Handler valida estado y autorizacion operacional. | Bajo. | Sin cambios. | Correcto |
| RouteAssignment - capacidad | No exceder capacidad y no reducir por debajo de estudiantes asignados. | Existe validacion en Application y dominio. No se elimino por defensa en profundidad: Application da mensajes de caso de uso; Domain conserva invariante. | Bajo. | Sin cambios. | Correcto |
| RouteAssignment - remover estudiante | No remover con trip `InProgress`. | Handler valida trips en progreso; dominio no conoce repositorio pero `RouteAssignment.Trips` tambien protege inicio activo. | Bajo. | Sin cambios. | Correcto |
| RouteAssignment - entidades activas | Driver, Vehicle, Assistant y Route deben estar activos segun flujo. | Validaciones viven en handlers porque dependen de repositorios/estado agregado. | Bajo. | Sin cambios. | Correcto |
| Incidents | No resolver cerrado/cancelado; no cerrar sin resolver. | Reglas estan en dominio `Incident`. | Bajo. | Sin cambios. | Correcto |
| Notifications | Marcar leida/no leida esta encapsulado en dominio. | No se detecto exposicion de entidades ni datos sensibles. | Bajo. | Sin cambios. | Correcto |
| Auditoria | Eventos criticos usan strings. | Hay nombres consistentes en viajes principales, pero strings no tipados pueden derivar en errores futuros. | Medio. | Documentado; no se creo enum para no agregar alcance nuevo. | Pendiente recomendado |
| Mensajes de error | Algunos mensajes antiguos aun aparecen sin acento o con encoding historico en archivos no modificados. | Bajo; afecta claridad, no reglas. | Bajo. | Se corrigieron mensajes tocados en esta fase. Se recomienda limpieza controlada separada. | Pendiente menor |

## Correcciones aplicadas

### 1. Bloqueo de asistencia contradictoria

Antes, `TripStudentAttendance.MarkAbsent()` solo bloqueaba estudiantes `DroppedOff`. Un estudiante ya `Boarded` podia pasar a `Absent`.

Ahora:

- `Boarded -> Absent` queda bloqueado.
- `DroppedOff -> Absent` sigue bloqueado.
- `Expected -> Absent` sigue permitido.
- `Absent -> Boarded` ya estaba bloqueado.
- `Expected -> DroppedOff` ya estaba bloqueado porque `DroppedOff` exige estado `Boarded`.

### 2. Bloqueo de mutaciones de asistencia fuera de viaje en progreso

Antes, los comandos de asistencia validaban permiso operativo y luego actualizaban el pasajero. Ahora el handler exige que el viaje este `InProgress` antes de obtener y modificar la asistencia.

Aplica a:

- `MarkStudentBoardedCommand`
- `MarkStudentAbsentCommand`
- `MarkStudentDroppedOffCommand`
- `UpdateTripStudentNotesCommand`

## Reglas revisadas sin cambios

### Estados de Trip

Las transiciones quedan protegidas por dominio:

- `Scheduled -> InProgress`
- `InProgress -> Completed`
- `Scheduled -> Cancelled`
- `Scheduled -> NotOperating`

El dominio bloquea:

- iniciar `Completed`, `Cancelled` o `NotOperating`
- finalizar si no esta `InProgress`
- cancelar si esta `InProgress`
- reportar desvio fuera de `InProgress`
- agregar pasajero excepcional fuera de `InProgress`

### Horarios y puntualidad

La regla esta bien distribuida:

- Domain calcula `DelayMinutes`, `IsLate`, `StartedEarly`, `EarlyStartReason` y `PunctualityStatus`.
- Application valida rol para forzar inicio anticipado.
- `SystemSettings` provee tolerancia configurable.

### Asignaciones

`RouteAssignment` conserva invariantes de capacidad, duplicados y asignacion basica. Application complementa con reglas que requieren infraestructura:

- entidades existentes
- actividad de conductor/vehiculo/asistente/ruta
- conflictos de horario
- escuela del estudiante
- trips activos

### Tracking

`UpdateVehicleLocationHandler` valida:

- Trip existente.
- Trip en `InProgress`.
- autorizacion operacional del usuario.

### Incidencias

Las transiciones principales se mantienen en dominio:

- asignar
- marcar en progreso
- resolver
- cerrar
- cancelar
- comentar

## Tests agregados

- `TripStudentAttendance_DoesNotAllowAbsentAfterBoarded`
- `MarkBoarded_Fails_WhenTripIsNotInProgress`
- `MarkAbsent_Fails_WhenStudentAlreadyBoarded`

## Recomendaciones pendientes

1. Crear constantes o enum interno para nombres de eventos de auditoria si se busca reducir errores tipograficos.
2. Hacer una limpieza separada de mensajes antiguos con acentos/encoding, usando una fase dedicada para evitar mezclar reglas con formato.
3. Mantener tests de ownership en handlers operativos, especialmente viajes, asistencia, desvios y tracking.
4. Evitar mover a Domain reglas que dependen de repositorios, usuario actual o configuracion externa; esas deben permanecer en Application.

