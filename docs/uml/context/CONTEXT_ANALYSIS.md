# Analisis del Diagrama de Contexto

## Proposito del diagrama

El diagrama de contexto representa la relacion entre la **Plataforma Web TRAE** y los actores externos que interactuan con el sistema. Su objetivo es mostrar, a nivel general, que informacion entra al sistema, que informacion sale y que responsabilidad tiene cada actor dentro de la operacion del transporte escolar.

Este diagrama esta pensado para documentacion academica de la tesis del proyecto **"Plataforma Web para el Sistema de Transporte Escolar TRAE"**.

## Sistema central

El sistema central es:

**Plataforma Web TRAE**

La plataforma integra funcionalidades reales implementadas en el sistema:

- Gestion academica.
- Gestion de usuarios, roles y permisos.
- Gestion de rutas, paradas, vehiculos, conductores y asistentes.
- Asignacion de estudiantes a rutas.
- Programacion y operacion de viajes.
- Registro de asistencia de pasajeros.
- Monitoreo GPS.
- Gestion de incidencias.
- Notificaciones internas.
- Reportes operativos.
- Auditoria y configuracion del sistema.

## Actores externos

| Actor | Descripcion |
| --- | --- |
| Administrador | Usuario encargado de la administracion general del sistema, gestion de usuarios, configuraciones, rutas, estudiantes, reportes, auditoria y backups. |
| Supervisor | Usuario responsable de supervisar la operacion diaria, monitorear viajes, gestionar incidencias, revisar reportes y supervisar rutas. |
| Conductor | Persona responsable de operar viajes asignados, iniciar y finalizar viajes, consultar pasajeros y actualizar ubicacion GPS. |
| Asistente de Transporte | Persona que acompana el viaje, registra asistencia de estudiantes, apoya la gestion de pasajeros y reporta incidencias. |
| Tutor | Responsable del estudiante que consulta informacion de sus estudiantes, viajes relacionados, ubicacion, notificaciones e incidencias. |
| Centro Educativo | Entidad academica que proporciona informacion relacionada con estudiantes, grados, escuelas y rutas asociadas. |
| Distrito Educativo 01-03 | Entidad supervisora que consulta indicadores, reportes operativos y estado general del sistema. |

## Entradas al sistema

Las entradas principales hacia la Plataforma Web TRAE son:

- Credenciales de acceso de usuarios.
- Datos de usuarios, roles y permisos.
- Datos academicos de estudiantes, tutores, grados, escuelas y sectores.
- Datos de transporte: vehiculos, conductores, asistentes, paradas y rutas.
- Asignaciones de rutas y estudiantes.
- Programaciones de viajes y dias sin operacion.
- Acciones operativas de viaje: inicio, finalizacion, asistencia y ubicacion GPS.
- Reportes de incidencias.
- Actualizaciones de configuracion general.
- Solicitudes de reportes y consultas.

## Salidas del sistema

Las salidas principales del sistema son:

- Paneles y reportes operativos.
- Informacion de estudiantes y viajes relacionados.
- Estado de rutas, asignaciones y viajes.
- Notificaciones internas.
- Alertas e incidencias.
- Ubicacion actual e historial GPS.
- Indicadores para supervision.
- Registros de auditoria.
- Respuestas de autenticacion mediante token JWT.

## Justificacion de las relaciones

- **Administrador**: tiene el mayor alcance funcional, porque administra usuarios, configuraciones, catalogos y consulta reportes.
- **Supervisor**: participa en supervision operativa, monitoreo, incidencias y reportes.
- **Conductor**: interactua con el sistema durante la ejecucion del viaje y actualiza ubicacion GPS.
- **Asistente de Transporte**: registra asistencia y apoya la operacion de estudiantes durante el viaje.
- **Tutor**: consume informacion relacionada con sus estudiantes y recibe notificaciones.
- **Centro Educativo**: se vincula al modulo academico, estudiantes, grados, escuelas y rutas.
- **Distrito Educativo 01-03**: utiliza la plataforma para supervision institucional mediante indicadores y reportes.

## Observacion

El diagrama se mantiene en un nivel de contexto, por lo que no detalla procesos internos ni endpoints especificos. Para esos detalles se complementa con los diagramas de casos de uso y diagramas de actividades ubicados en `docs/uml`.
