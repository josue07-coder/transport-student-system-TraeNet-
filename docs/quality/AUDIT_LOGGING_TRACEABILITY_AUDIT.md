# Auditoria de auditoria, logging y trazabilidad

Proyecto: Plataforma Web para el Sistema de Transporte Escolar TRAE  
Fase: QA-4 - Auditoria, logging y trazabilidad  
Fecha: 2026-06-22

## Resumen ejecutivo

Se reviso la trazabilidad actual del backend, incluyendo `AuditService`, `AuditLogs`, middleware de excepciones, logging de API, respuestas de error y acciones criticas auditadas. El sistema ya contaba con auditoria funcional de eventos de negocio, pero faltaba correlacion transversal entre request HTTP, logs de aplicacion y registros de auditoria.

Se corrigio la trazabilidad agregando correlation id por request, logging estructurado de requests, metadata HTTP en `AuditLogs`, redaccion basica de valores sensibles antes de persistir auditoria y respuestas de error con `traceId`/`correlationId`.

No se modifico frontend.

## Cambios aplicados

- Se agrego `CorrelationIdMiddleware`.
- Se agrega/propaga header `X-Correlation-ID`.
- Se usa `HttpContext.TraceIdentifier` como correlation id efectivo.
- Se agrego logging estructurado por request:
  - metodo HTTP
  - ruta
  - status code
  - duracion en milisegundos
  - correlation id en scope de logging
- Se actualizo `ExceptionMiddleware` para:
  - loguear validaciones, reglas de dominio, conflictos de concurrencia y errores no controlados
  - devolver `traceId` y `correlationId` en respuestas de error
  - evitar exponer detalles internos de excepciones no controladas
- Se extendio `AuditLog` con:
  - `CorrelationId`
  - `TraceId`
  - `RequestPath`
  - `HttpMethod`
- Se agrego indice por `AuditLogs.CorrelationId`.
- Se actualizo `AuditLogResponseDto` y mappings.
- Se agrego redaccion basica de claves sensibles en `AuditService`:
  - password
  - passwordHash
  - currentPassword
  - newPassword
  - confirmPassword
  - token
  - jwt
  - authorization
- Se agrego smoke test `scripts/smoke-test-audit-traceability.ps1`.

## Matriz de auditoria

| Area | Riesgo detectado | Impacto | Correccion aplicada | Requiere migracion | Estado |
|---|---|---|---|---|---|
| Correlacion HTTP | No existia correlation id consistente por request. | Dificultad para rastrear un error desde frontend hasta logs/backend. | `CorrelationIdMiddleware` con header `X-Correlation-ID`. | No | Corregido |
| Logging de requests | No habia log estructurado uniforme por request. | Baja observabilidad operativa. | Middleware registra metodo, ruta, status y duracion. | No | Corregido |
| Respuestas de error | Errores no incluian identificador rastreable. | Frontend/soporte no podia reportar un id util. | `traceId` y `correlationId` en respuestas de error. | No | Corregido |
| Errores internos | Excepciones genericas podian exponer mensajes internos. | Riesgo de fuga de informacion tecnica. | Mensaje generico para 500 y detalle completo solo en logs. | No | Corregido |
| AuditLogs | No guardaban metadata HTTP/correlation. | Auditoria no podia enlazarse con logs de request. | Campos de trazabilidad en `AuditLog`. | Si | Corregido |
| Datos sensibles | OldValues/NewValues podian recibir payloads con secretos si un handler los enviaba por error. | Riesgo de guardar contrasenas/tokens en auditoria. | Redaccion defensiva en `AuditService`. | No | Corregido |
| Longitudes | Valores largos de user-agent/action/entity podian fallar al guardar auditoria. | Auditoria no critica se perdia. | Truncado controlado en `AuditService`. | No | Corregido |

## Migracion

Se creo y aplico la migracion:

- `20260622055515_AddAuditTraceability`

Cambios:

- `AuditLogs.CorrelationId`
- `AuditLogs.TraceId`
- `AuditLogs.RequestPath`
- `AuditLogs.HttpMethod`
- indice `IX_AuditLogs_CorrelationId`

## Validacion prevista

- `dotnet build TransportStudentSystem.sln`
- `dotnet test TransportStudentSystem.sln`
- `scripts/smoke-test-audit-traceability.ps1`

## Riesgos pendientes

1. El logging actual usa providers `Console` y `Debug`. Para produccion conviene integrar un sink persistente/centralizado, por ejemplo Serilog con archivo, Seq, Elastic o Application Insights.
2. La redaccion defensiva cubre claves sensibles comunes. Si se agregan nuevos secretos o proveedores externos, se debe ampliar la lista.
3. No se auditan automaticamente todos los endpoints; se mantiene auditoria explicita por caso de uso critico para evitar ruido y datos innecesarios.
