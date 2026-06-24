# Fase QA-5: Auditoría de rendimiento, consultas, paginación e índices

## Objetivo

Revisar las consultas principales del backend de TransportStudentSystem para reducir riesgos de paginación inestable, carga innecesaria de entidades, consultas con grafos grandes y falta de índices en rutas críticas de lectura.

La auditoría se aplicó sin modificar frontend, sin cambiar endpoints y sin agregar funcionalidades nuevas.

## Hallazgos y correcciones

| Área | Riesgo detectado | Impacto | Corrección aplicada | Requiere migración | Estado |
|---|---|---|---|---|---|
| Paginación general | Varios repositorios usaban `Skip`/`Take` sin `OrderBy`. | Páginas con registros repetidos, saltados o no determinísticos según plan SQL. | Se agregó orden estable en repositorios principales: estudiantes, tutores, escuelas, grados, sectores, vehículos, conductores, asistentes, paradas, rutas, asignaciones y viajes. | No | Corregido |
| Límite de paginación | El límite existía en `PaginationRequest`, pero no tenía prueba explícita. | Riesgo de regresión futura que permita page size excesivo. | Se agregaron pruebas unitarias para normalizar `PageNumber` y limitar `PageSize` a 100. | No | Corregido |
| Trips | `TripRepository` cargaba un grafo grande con tracking en consultas de lectura. | Mayor consumo de memoria y riesgo de consultas Cartesian explosion por múltiples Includes. | Se agregó `AsNoTracking` para lecturas, `AsSplitQuery` para Includes múltiples y orden estable por fecha. | No | Corregido |
| RouteAssignments | `RouteAssignmentRepository` cargaba rutas, vehículo, conductor, asistente, estudiantes y trips en una consulta única. | Mayor costo SQL y memoria, especialmente en listados. | Se agregó `AsSplitQuery`, `AsNoTracking` en consultas de lectura y orden estable. | No | Corregido |
| Incidents | Consultas de incidentes con varios Includes podían generar consultas pesadas. | Respuestas lentas en historial y filtros. | Se agregó `AsSplitQuery` y `AsNoTracking` en filtros/listados. | No | Corregido |
| Catálogos y entidades base | Consultas de solo lectura mantenían tracking innecesario. | Uso de memoria innecesario en listados. | Se agregó `AsNoTracking` en listados y filtros de lectura. | No | Corregido |
| Trips por estado/fecha | Filtros frecuentes por `Status`, `StartTime`, `ScheduledDepartureTime`, `OperationDate` y `RouteAssignmentId`. | Escaneos de tabla al crecer historial de viajes. | Se agregaron índices sobre `Status`, `StartTime`, `ScheduledDepartureTime`, `OperationDate`, `RouteAssignmentId + Status` y `Status + StartTime`. | Sí | Corregido |
| Rutas | Filtros frecuentes por escuela, estado y nombre por escuela. | Consultas más lentas al crecer rutas. | Se agregaron índices sobre `SchoolId`, `Status` y `SchoolId + Name`. | Sí | Corregido |
| Estudiantes | Filtros frecuentes por escuela, grado, tutor, estado y código. | Consultas más lentas en módulos académicos y visibilidad por tutor. | Se agregaron índices sobre `SchoolId`, `GradeId`, `GuardianId`, `IsActive` y `StudentCode`. | Sí | Corregido |
| Incidencias | Filtros por estado/severidad normalmente se ordenan por fecha. | Escaneos o sorts costosos en bandejas operativas. | Se agregaron índices compuestos `Status + CreatedAt` y `Severity + CreatedAt`. | Sí | Corregido |
| Auditoría | Consultas por acción suelen analizarse por fecha. | Reportes de auditoría menos eficientes. | Se agregó índice compuesto `Action + CreatedAt`. | Sí | Corregido |
| Tracking, notificaciones y auditoría base | Ya existían índices por usuario, estado, fecha y entidad relacionada. | Bajo riesgo inmediato. | Se conservaron índices existentes. | No | Verificado |

## Índices agregados

Migración creada:

- `AddPerformanceQueryIndexes`

Índices principales incluidos:

- `Trips`: `Status`, `OperationDate`, `StartTime`, `ScheduledDepartureTime`, `RouteAssignmentId + Status`, `Status + StartTime`.
- `Students`: `IsActive`, `StudentCode`.
- `Routes`: `SchoolId + Name`, `Status`.
- `Incidents`: `Status + CreatedAt`, `Severity + CreatedAt`.
- `AuditLogs`: `Action + CreatedAt`.

Nota: EF Core ya crea índices convencionales para varias FKs. Por eso algunos índices configurados en el modelo no generaron operaciones nuevas en la migración.

## Consultas optimizadas

Se optimizaron consultas de lectura en:

- `TripRepository`
- `RouteAssignmentRepository`
- `IncidentRepository`
- `StudentRepository`
- `GuardianRepository`
- `DriverRepository`
- `VehicleRepository`
- `TransportAssistantRepository`
- `RouteRepository`
- `SchoolRepository`
- `GradeRepository`
- `SectorRepository`
- `StopRepository`

## Riesgos revisados posteriormente

| Riesgo | Recomendación |
|---|---|
| Algunos endpoints de filtro devuelven listas completas por compatibilidad. | Se mantuvo el contrato público para no romper frontend. La mitigación aplicada fue reducir el costo de consulta con `AsNoTracking`, orden estable, consultas livianas e índices. |
| `TripRepository` todavía usaba un grafo amplio para varios casos de uso. | Corregido: se separaron consultas base, consultas mínimas para visibilidad y consultas completas para detalle/operación. |
| Warnings EF por owned `Email` opcional en `Driver` y `TransportAssistant`. | Corregido: `Email` se mapea como ValueObject con conversión escalar a la misma columna `Email`, evitando el warning de dependiente opcional en table sharing. |
| Algunos reportes complejos pueden requerir índices adicionales según volumen real. | Riesgo operativo de monitoreo continuo; no requiere cambio inmediato sin métricas/planes reales de producción. |
| La migración cambia columnas indexadas de `nvarchar(max)` a `nvarchar(450)`. | Aplicada correctamente en desarrollo. Es adecuado para enum/status y códigos cortos. |

## Validación recomendada

1. Ejecutar `dotnet build TransportStudentSystem.sln`.
2. Ejecutar `dotnet test TransportStudentSystem.sln`.
3. Aplicar migración con `dotnet ef database update --project Transport.Infrastructure --startup-project Transport.API`.
4. Ejecutar `scripts/smoke-test-performance-pagination.ps1`.

Si el smoke test no puede autenticarse por `Failed to generate SSPI context`, se debe tratar como bloqueo ambiental de SQL Server y no como fallo de QA-5.
