# Analisis de diagramas de actividades

Este documento resume los diagramas de actividades generados para el punto 4.3.4 del Capitulo IV de la tesis de la Plataforma Web para el Sistema de Transporte Escolar TRAE.

Los diagramas se basan en funcionalidades reales implementadas en el backend y documentadas para el frontend. Cada diagrama representa un proceso principal para mantener legibilidad en documentos Word tamano carta.

## Diagramas creados

| Archivo | Proceso representado | Descripcion |
| --- | --- | --- |
| `activity-login.puml` | Inicio de sesion | Representa el flujo de autenticacion: ingreso de credenciales, validacion, generacion de JWT, auditoria y manejo de error. |
| `activity-manage-students.puml` | Gestion de estudiantes | Describe consulta, creacion, edicion y desactivacion de estudiantes, incluyendo validaciones academicas y confirmacion. |
| `activity-route-assignment.puml` | Asignacion de ruta | Muestra la seleccion de ruta, vehiculo, conductor, asistente y estudiantes, con validacion de capacidad y reglas operativas. |
| `activity-trip-schedule-materialization.puml` | Programacion y materializacion de viaje | Explica la creacion de horarios, seleccion de fecha, validacion de calendario escolar y creacion de viajes `Scheduled` o `NotOperating`. |
| `activity-trip-operation.puml` | Operacion de viaje | Resume el inicio de viaje, snapshot de pasajeros, registro GPS, asistencia de estudiantes y finalizacion en estado `Completed`. |
| `activity-incident-management.puml` | Gestion de incidencias | Cubre reporte, validacion, notificacion, asignacion de responsable, cambio de estado, resolucion y cierre. |
| `activity-notifications.puml` | Gestion de notificaciones | Representa la generacion automatica de notificaciones, consulta por usuario y marcado como leidas. |
| `activity-reports.puml` | Consulta de reportes | Describe seleccion de reporte, filtros, consulta de datos, presentacion de resultados y exportacion si aplica. |

## Orden recomendado para la tesis

1. Inicio de sesion.
2. Gestion de estudiantes.
3. Asignacion de ruta.
4. Programacion y materializacion de viaje.
5. Operacion de viaje.
6. Gestion de incidencias.
7. Gestion de notificaciones.
8. Consulta de reportes.

Este orden acompana el flujo natural del sistema: autenticacion, administracion academica, configuracion operativa, ejecucion del transporte, comunicacion y supervision.

## Criterios de diseno usados

- Un proceso principal por diagrama.
- Textos en espanol.
- Uso de swimlanes para diferenciar usuario, sistema y base de datos.
- Diagramas compactos para facilitar insercion en Word.
- No se incluyeron funciones no implementadas como geocercas, chat en tiempo real, ETA avanzado o proveedores externos reales.

## Observaciones

- La exportacion de reportes se muestra como actividad condicional porque el backend implementa consultas de reportes, mientras que la generacion de archivos exportables depende del frontend o de una fase posterior.
- La materializacion de viajes ya contempla fines de semana y dias sin operacion, creando viajes `NotOperating` para conservar trazabilidad.
- La operacion de viaje incluye snapshot de pasajeros mediante `TripStudentAttendance`, lo que evita que cambios posteriores en la asignacion alteren el historico del viaje.
