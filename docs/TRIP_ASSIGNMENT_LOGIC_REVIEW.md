# Revisión técnica: rutas, asignaciones y viajes

## A. Resumen general

El módulo operativo está construido alrededor de cuatro conceptos principales:

- `Route`: define una ruta escolar, su escuela, horario operativo, estado y secuencia de paradas.
- `RouteAssignment`: vincula una ruta activa con conductor, vehículo, asistente opcional, capacidad y estudiantes asignados.
- `Trip`: representa una ejecución concreta de una asignación. El flujo actual lo crea y lo inicia en una sola operación.
- `VehicleLocation`: registra puntos GPS asociados a un viaje en progreso.

El flujo esperado y soportado actualmente es:

1. Crear una ruta inactiva para una escuela activa.
2. Agregar una o más paradas con orden único.
3. Activar la ruta mediante actualización.
4. Crear una asignación para la ruta activa indicando conductor, vehículo, capacidad y asistente opcional.
5. Asignar estudiantes activos de la misma escuela.
6. Iniciar un viaje cuando la ruta, recursos y estudiantes cumplen las reglas operativas.
7. Registrar y consultar posiciones GPS durante el viaje.
8. Finalizar o cancelar el viaje.

La implementación sigue CQRS con MediatR, repositorios y mapeo manual a DTOs. Los controllers no devuelven entidades de dominio.

## B. Flujo actual paso a paso

### 1. Crear ruta

`CreateRouteHandler` valida que la escuela exista y esté activa, y que el nombre no esté repetido dentro de la misma escuela. Construye un `TimeRange`, que exige `EndTime > StartTime`, duración mínima de 10 minutos y máxima de 4 horas. La ruta nace con estado `Inactive`.

Endpoint: `POST /api/routes`

### 2. Agregar paradas

`AddStopToRouteHandler` carga la ruta con sus paradas, bloquea la operación si existe un viaje en progreso y verifica que `StopId` exista. `Route.AddStop` impide repetir una parada o reutilizar un orden dentro de la ruta. El repositorio marca explícitamente el nuevo `RouteStop` como `Added`.

Endpoint: `POST /api/routes/{routeId}/stops`

### 3. Activar ruta

`UpdateRouteHandler` permite dejar la ruta en `Active` o `Inactive`. `Route.Activate()` exige al menos una parada. La validez del horario se vuelve a comprobar al construir `TimeRange`.

Endpoint: `PUT /api/routes/{id}`

### 4. Crear asignación

`CreateRouteAssignmentHandler` valida:

- ruta existente y activa;
- conductor existente y activo;
- vehículo existente y con estado `Active`;
- asistente existente y activo, si fue indicado;
- capacidad mayor que cero y no superior a la capacidad del vehículo;
- ausencia de cruces de horario para conductor, vehículo y asistente.

Endpoint: `POST /api/route-assignments`

### 5. Asignar estudiantes

`AssignStudentToRouteAssignmentHandler` valida:

- asignación existente;
- estudiante existente y activo;
- ruta activa;
- escuela del estudiante igual a escuela de la ruta;
- estudiante no repetido dentro de la asignación;
- capacidad disponible;
- ausencia de otro assignment activo con horario cruzado para ese estudiante.

Endpoint: `POST /api/route-assignments/{assignmentId}/students/{studentId}`

### 6. Iniciar viaje

`StartTripHandler` valida:

- asignación existente;
- ausencia de otro `Trip` en estado `InProgress` para la asignación;
- ruta activa con al menos una parada;
- conductor activo;
- vehículo con estado `Active`;
- asistente activo, si existe;
- al menos un estudiante asignado.

Después llama `RouteAssignment.StartTrip()`, que crea un `Trip`, lo cambia inmediatamente de `Pending` a `InProgress` y lo agrega a la colección. El handler también agrega explícitamente el viaje al repositorio para asegurar un `INSERT`.

Endpoint: `POST /api/trips/start`

### 7. Actualizar tracking GPS

`UpdateVehicleLocationHandler` permite registrar un punto solo para un viaje `InProgress`. La ubicación guarda viaje, vehículo, coordenadas, velocidad opcional, rumbo opcional, usuario reportante y fecha UTC.

Puede reportar ubicación:

- `Admin`;
- `Supervisor`;
- conductor de la asignación;
- asistente de la asignación.

Si la velocidad supera el setting `GPS.SpeedLimitKmH` y las alertas están habilitadas, se notifica al rol `Supervisor`.

Endpoint: `POST /api/tracking/location`

### 8. Finalizar o cancelar

`Trip.End()` solo permite finalizar viajes `InProgress`. `Trip.Cancel()` permite cancelar `Pending` o `InProgress`, pero no `Completed` ni `Cancelled`.

Endpoints:

- `PUT /api/trips/{id}/end`
- `PUT /api/trips/{id}/cancel`

## C. Modelo conceptual actual

### Route

Representa la definición operativa de una ruta escolar. Pertenece a una escuela, contiene horario, estado, paradas ordenadas y assignments. Su activación exige al menos una parada.

### RouteStop

Representa la unión entre `Route` y `Stop`, con un `StopOrder`. La configuración EF declara relaciones explícitas y un índice único `(RouteId, StopOrder)`.

### RouteAssignment

Representa la asignación operativa de recursos a una ruta: conductor, vehículo, asistente opcional, capacidad y estudiantes. No tiene estado propio ni vigencia propia; en la práctica, su condición de “activa” se infiere de `Route.Status == Active`.

### StudentRouteAssignment

Representa la relación muchos-a-muchos entre estudiante y asignación. Usa PK compuesta `(StudentId, RouteAssignmentId)`. No tiene estado, fecha de alta, fecha de baja ni historial.

### Trip

Representa una ejecución concreta de un assignment. Tiene estados `Pending`, `InProgress`, `Completed` y `Cancelled`. El flujo normal crea el viaje y lo inicia inmediatamente, por lo que `Pending` casi no tiene vida persistida.

### Vehicle, Driver y TransportAssistant

- `Vehicle` usa `VehicleStatus`: `Active`, `Inactive`, `Maintenance`.
- `Driver` implementa `IActivatable` mediante `IsActive`.
- `TransportAssistant` implementa `IActivatable` mediante `IsActive`.

### Student

Implementa `IActivatable`, pertenece a escuela, grado y tutor. Puede participar en varias asignaciones mediante `StudentRouteAssignment`.

### VehicleLocation

Representa un punto GPS histórico. La ubicación actual se obtiene tomando el registro más reciente por `TripId`; no existe una tabla separada de “ubicación actual”.

## D. Reglas de negocio actuales

### Rutas

- Nombre requerido y único por escuela.
- Escuela requerida, existente y activa.
- `EndTime` debe ser mayor que `StartTime`.
- Duración mínima: 10 minutos.
- Duración máxima: 4 horas.
- Una ruta solo se activa si tiene paradas.
- No se actualiza ni desactiva si tiene viaje en progreso.
- No se remueven ni reordenan paradas de una ruta activa.
- No se agregan, remueven ni reordenan paradas si hay viaje en progreso.
- Una parada no se repite dentro de la ruta.
- `StopOrder` no se repite dentro de la ruta.

### Asignaciones

- Solo se crean o actualizan para rutas activas.
- Conductor, vehículo y asistente opcional deben existir y estar activos.
- Capacidad requerida, positiva y no superior a la del vehículo.
- Al actualizar, la capacidad no puede quedar por debajo del número de estudiantes.
- No se permiten cruces de horario para conductor, vehículo ni asistente en rutas activas.
- No se actualiza ni elimina un assignment que tenga cualquier viaje asociado.

### Estudiantes asignados

- El estudiante debe existir y estar activo.
- Debe pertenecer a la misma escuela de la ruta.
- No se duplica dentro de un assignment.
- No puede excederse la capacidad.
- No puede estar en assignments activos con horarios cruzados.

### Viajes

- Solo se inicia para una asignación existente.
- Solo se inicia si la ruta está activa y tiene paradas.
- Solo se inicia si conductor, vehículo y asistente opcional están activos.
- Solo se inicia si hay estudiantes.
- No se permiten dos viajes `InProgress` para el mismo assignment.
- Solo se finaliza un viaje `InProgress`.
- Solo se cancela un viaje `Pending` o `InProgress`.

### Tracking

- Solo se registra ubicación para viajes `InProgress`.
- Latitud entre `-90` y `90`.
- Longitud entre `-180` y `180`.
- Velocidad no negativa.
- Rumbo entre `0` y `360`.
- `Admin` y `Supervisor` pueden consultar y actualizar cualquier viaje.
- `Driver` y `TransportAssistant` solo pueden actualizar o ver sus viajes.
- `Guardian` puede consultar viajes asociados a sus estudiantes, pero no registrar ubicaciones.

## E. Validaciones por operación

### Al crear ruta

Se valida escuela existente y activa, nombre único por escuela y `TimeRange` válido.

### Al actualizar ruta

Se bloquea si existe viaje en progreso. Se valida escuela activa, nombre único y horario válido. Solo acepta destino `Active` o `Inactive`.

### Al agregar parada

Se valida ruta, ausencia de viaje en progreso, existencia de parada, `StopOrder > 0`, parada no repetida y orden no repetido.

### Al remover parada

Se valida ruta, ausencia de viaje en progreso y existencia del stop. El dominio bloquea la remoción si la ruta está activa.

### Al reordenar parada

Se valida ruta, ausencia de viaje en progreso, orden positivo, existencia del stop y orden no repetido. El dominio bloquea el cambio si la ruta está activa.

### Al crear asignación

Se valida existencia y actividad de recursos, ruta activa, capacidad y conflictos horarios.

### Al actualizar asignación

Se bloquea si tiene cualquier viaje histórico. Se repiten las validaciones de creación, se excluye el assignment actual al detectar conflictos y se protege la cantidad de estudiantes asignados.

### Al eliminar asignación

La eliminación es física, pero solo se permite si no existen viajes asociados. La configuración EF elimina en cascada los `StudentRouteAssignment`.

### Al asignar estudiante

Se valida existencia, actividad, escuela, duplicado, capacidad y cruce horario. Se audita y se notifica al tutor si tiene usuario vinculado.

### Al remover estudiante

Se valida assignment, estudiante y membresía actual. Se audita y se notifica al tutor. No se bloquea por viajes históricos o en progreso.

### Al iniciar viaje

Se ejecutan validaciones en handler y nuevamente algunas invariantes en dominio. El viaje se persiste directamente como `InProgress`.

### Al finalizar viaje

Se carga el detalle, `Trip.End()` valida el estado y se notifica a tutores.

### Al cancelar viaje

Se carga el detalle, `Trip.Cancel()` valida el estado y se notifica a conductor, asistente, tutores y supervisores.

### Al actualizar ubicación

Se valida viaje `InProgress`, acceso del usuario y límites geográficos. Se persiste un punto histórico y opcionalmente se emite alerta de velocidad.

## F. Posibles inconsistencias o riesgos

### Riesgos funcionales

1. **`RouteAssignment` no tiene estado propio.** Los conflictos horarios y varias reglas interpretan como assignment activo cualquiera cuya ruta esté activa. No es posible retirar un assignment manteniendo la ruta activa ni distinguir vigencia histórica.

2. **El historial de pasajeros no queda congelado por viaje.** `Trip` consulta estudiantes desde la colección mutable de `RouteAssignment`. Si se remueve un estudiante después de viajar, reportes, visibilidad del tutor y notificaciones históricas pueden dejar de reflejar quién viajó realmente.

3. **Se pueden remover estudiantes durante un viaje en progreso.** `RemoveStudentFromRouteAssignmentHandler` no bloquea assignments con trip activo. Esto puede alterar visibilidad y destinatarios de notificaciones antes de finalizar el viaje.

4. **`Route.Assign(...)` parece obsoleto y tiene parámetros cruzados.** Construye `RouteAssignment(Id, driverId, vehicleId, capacity)`, pero el constructor espera `(routeId, vehicleId, driverId, capacity)`. No se encontraron usos actuales fuera del dominio, pero es una trampa para futuras llamadas.

5. **Actualizar una ruta activa mediante `PUT` resulta impracticable.** `UpdateRouteHandler` llama `ChangeSchool` y `UpdateOperatingHours` antes de aplicar el nuevo estado. Ambos métodos rechazan toda ruta activa, incluso si los valores enviados no cambian. La desactivación operativa funciona por `DELETE`, pero el comportamiento de `PUT` puede confundir al frontend.

6. **Agregar paradas es más permisivo que removerlas o reordenarlas.** `Route.AddStop` no bloquea rutas activas, mientras los otros métodos sí. Una ruta activa sin viaje puede recibir nuevas paradas, pero no ajustar orden ni removerlas.

7. **`Pending` existe, pero el flujo normal no lo persiste.** `StartTrip` crea y arranca el viaje en la misma transacción. El estado puede ser útil en el futuro, pero actualmente no modela planificación.

### Riesgos de concurrencia

1. Las comprobaciones de viaje activo, capacidad y conflictos horarios son secuencias “consultar y luego guardar”. Dos requests simultáneos pueden superar capacidad o iniciar dos viajes antes de que uno observe el cambio del otro.

2. El índice único `(RouteId, StopOrder)` protege el orden de paradas a nivel BD, pero no existe una protección equivalente para “un solo trip `InProgress` por assignment”.

3. La PK compuesta de `StudentRouteAssignment` evita duplicado exacto dentro del mismo assignment, pero una carrera puede terminar como error de BD en lugar de error de dominio controlado.

### Riesgos EF Core

1. Las relaciones revisadas son explícitas y no se detectaron columnas fantasma `RouteId1`, `RouteAssignmentId1`, `StudentId1`, `StopId1`, `DriverId1`, `VehicleId1`, `TransportAssistantId1` o `SchoolId1` en configuraciones ni snapshot.

2. Hay declaraciones redundantes pero compatibles de algunas relaciones en ambos extremos (`Route -> Assignments`, `RouteAssignment -> Trips`, `RouteAssignment -> Students`). EF Core las puede unificar, pero conviene mantener un único punto de configuración por relación para reducir riesgo de divergencia futura.

3. `RouteConfiguration` configura `Route -> School` explícitamente con `SchoolId`, sin navigation property en `Route`. No genera FK fantasma, aunque una navegación explícita podría mejorar legibilidad.

4. Los `RouteAssignment` se eliminan físicamente cuando no tienen viajes. Esto elimina también sus memberships de estudiantes. Es consistente con el modelo actual, pero limita auditoría operativa.

### Riesgos de rendimiento y frontend

1. `GetActiveTripsLocationsHandler` consulta la última ubicación una vez por viaje visible. Es un patrón N+1 tolerable a baja escala, pero mejorable.

2. Varios mensajes presentan mojibake (`AsignaciÃ³n`, `estÃ¡`, `ubicaciÃ³n`). No cambia reglas, pero degrada errores visibles para frontend.

3. Los endpoints usan `[Authorize]` y la visibilidad se filtra en queries. Las mutaciones principales no aplican restricciones finas por rol en controller; dependen de autenticación y reglas operativas, no de autorización administrativa.

4. El endpoint `GET /api/tracking/my-students` solo aplica a usuarios Guardian. Para otros roles devuelve error controlado, no una lista vacía.

## G. Recomendaciones de mejora

### Críticas

1. Agregar historial de pasajeros por viaje, por ejemplo `TripStudentAttendance` o snapshot equivalente. No depender de la membresía mutable del assignment para consultas históricas.

2. Bloquear remoción de estudiantes mientras exista un trip `InProgress`, salvo que se modele explícitamente una baja operativa con trazabilidad.

3. Introducir protección de concurrencia para inicio de viaje y asignación de estudiantes. Como mínimo, usar transacción adecuada y traducir conflictos de BD a errores controlados.

4. Retirar o corregir `Route.Assign(...)` para evitar intercambiar `DriverId` y `VehicleId` si alguien lo reutiliza.

### Importantes

1. Agregar `AssignmentStatus` y vigencia al assignment. La ruta y su asignación son conceptos distintos: una ruta puede seguir activa aunque cambie conductor o vehículo.

2. Definir una transición explícita para editar rutas activas: exigir desactivación previa o permitir cambios seguros mediante métodos de dominio consistentes.

3. Decidir una política única para paradas de rutas activas: permitir todos los cambios seguros sin trip activo o bloquear todos hasta desactivar.

4. Aplicar autorización por rol o permiso a comandos operativos sensibles, no solo `[Authorize]`.

5. Corregir los textos mojibakeados de handlers en una limpieza separada.

### Opcionales

1. Optimizar consulta de ubicaciones actuales para evitar N+1.

2. Consolidar configuraciones EF redundantes en un único extremo por relación.

3. Persistir `Pending` solo si se incorpora planificación o despacho previo al inicio.

## H. Propuesta de modelo ideal si se necesita cambiar

### ¿RouteAssignment debería tener Status propio?

Sí. Conviene introducir `AssignmentStatus`, por ejemplo `Draft`, `Active`, `Suspended`, `Completed`, `Cancelled`. También debería tener vigencia (`ValidFrom`, `ValidTo`) o una referencia a programación. Esto separa la vida de la ruta de la vida del equipo asignado.

### ¿Trip debería crearse como Pending antes o solo al iniciar?

Depende del objetivo operativo:

- Si solo se registra ejecución real, crear directamente `InProgress` simplifica el modelo.
- Si se necesita planificar, despachar, confirmar asistencia o mostrar próximos viajes, conviene persistir `Pending` y ofrecer una transición `Start`.

El estado actual mezcla ambos enfoques: conserva `Pending`, pero no lo utiliza de forma persistente.

### ¿Debe existir una entidad Schedule?

Sí, si habrá viajes diarios o recurrentes. `Route.OperatingHours` describe una franja, pero no días de semana, calendario escolar, excepciones, feriados ni múltiples recorridos por día. Una entidad `Schedule` o `RouteSchedule` debería modelar recurrencia y excepciones.

### ¿Debe existir AssignmentStatus?

Sí. Permite reemplazar conductor, vehículo o asistente sin desactivar la ruta y sin perder historial. También mejora conflictos horarios y consultas operativas.

### ¿Debe existir TripStudentAttendance?

Sí. Debe capturar al menos:

- `TripId`;
- `StudentId`;
- estado esperado/presente/ausente;
- timestamps de abordaje y descenso si se necesitan;
- observaciones opcionales.

Así el historial de cada viaje permanece estable aunque cambien assignments posteriores.

### ¿Cómo manejar viajes diarios o recurrentes?

Usar:

1. `Route` como definición del recorrido.
2. `RouteSchedule` como calendario recurrente.
3. `RouteAssignment` como recursos vigentes para una programación.
4. `Trip` como instancia generada para una fecha concreta.

### ¿Cómo manejar historial de estudiantes por viaje?

Crear snapshot o attendance al crear/iniciar el viaje. No consultar pasajeros históricos desde la colección viva de `RouteAssignment`.

### ¿Cómo manejar cambios de ruta con viajes existentes?

Conservar inmutables los datos operativos relevantes del viaje iniciado: ruta, secuencia de paradas, assignment y pasajeros. Los cambios posteriores deben aplicar a futuras instancias, no reinterpretar viajes históricos.

## I. Conclusión

La lógica actual es **parcialmente correcta**.

Para operación básica, el flujo está bien protegido: rutas con horarios válidos, stops requeridos, recursos activos, conflictos horarios, capacidad, escuela del estudiante, un único viaje en progreso por assignment y tracking con visibilidad por rol.

Sin embargo, el modelo todavía necesita ajustes antes de considerarse robusto para operación real: falta estado propio de assignment, snapshot de pasajeros por viaje y protección frente a concurrencia. También conviene resolver las asimetrías de edición de rutas activas y retirar el método obsoleto `Route.Assign(...)`.

## Referencias revisadas

### Dominio

- `Transport.Domain/Entities/Route.cs`
- `Transport.Domain/Entities/RouteStop.cs`
- `Transport.Domain/Entities/RouteAssignment.cs`
- `Transport.Domain/Entities/StudentRouteAssignment.cs`
- `Transport.Domain/Entities/Trip.cs`
- `Transport.Domain/Entities/Vehicle.cs`
- `Transport.Domain/Entities/Driver.cs`
- `Transport.Domain/Entities/TransportAssistant.cs`
- `Transport.Domain/Entities/Student.cs`
- `Transport.Domain/Entities/VehicleLocation.cs`
- `Transport.Domain/ValueObjects/TimeRange.cs`

### Application

- Handlers de `Routes`, `RouteAssignments`, `Trips` y `Tracking`
- Validators de rutas, asignaciones, viajes y tracking
- `Transport.Application/Common/Security/VisibilityService.cs`

### Infrastructure

- Repositories de rutas, assignments, trips, estudiantes, conductores, vehículos, asistentes y ubicaciones
- Configurations EF de `Route`, `RouteStop`, `RouteAssignment`, `StudentRouteAssignment`, `Trip` y `VehicleLocation`

### API

- `RoutesController`
- `RouteAssignmentsController`
- `TripsController`
- `TrackingController`
