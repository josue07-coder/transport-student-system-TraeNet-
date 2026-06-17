# SQL Server SSPI Troubleshooting

## Síntoma

La API levanta correctamente y Swagger responde `200`, pero cualquier endpoint que toca base de datos falla. En smoke tests, el primer fallo aparece en:

```text
POST /api/auth/login
500 Internal Server Error
Failed to generate SSPI context.
```

## Connection string detectada

Archivo:

```text
Transport.API/appsettings.json
```

Connection string:

```text
Server=DESKTOP-Q0FVL2N\SQLEXPRESS;Database=TransportDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;
```

No se detectó `Password` en esta cadena.

`appsettings.Development.json` no define `ConnectionStrings`, por lo que en desarrollo se hereda la cadena de `appsettings.json`.

## Causa probable

La cadena usa autenticación integrada de Windows:

```text
Trusted_Connection=True
```

Eso equivale a:

```text
Integrated Security=True
```

El error `Failed to generate SSPI context` ocurre antes de autenticar contra SQL Server de forma normal. Usualmente indica un problema con Kerberos/SSPI, SPN, cuenta de Windows, nombre del servidor, servicio SQL Server o contexto de dominio/red.

En desarrollo local con `SQLEXPRESS`, las causas comunes son:

- SQL Server se está resolviendo con un nombre de máquina/SPN que Windows no puede validar.
- La sesión de Windows tiene credenciales Kerberos dañadas o vencidas.
- El servicio SQL Server corre bajo una cuenta que no puede registrar SPN.
- El equipo cambió de red, dominio, nombre o DNS.
- Se está usando un nombre de servidor distinto al esperado.
- SQL Server acepta conexiones, pero falla la negociación de autenticación integrada.

## Opción A: reparar autenticación integrada

Usa esta opción si quieres seguir usando Windows Authentication.

### 1. Verificar que SQL Server está accesible

Con PowerShell:

```powershell
Test-NetConnection DESKTOP-Q0FVL2N -Port 1433
```

Para SQL Express con instancia nombrada, también verifica que el servicio esté activo:

```powershell
Get-Service *SQL*
```

### 2. Probar conexión con sqlcmd

Si tienes `sqlcmd` instalado:

```powershell
sqlcmd -S "DESKTOP-Q0FVL2N\SQLEXPRESS" -d "TransportDB" -E -Q "SELECT 1"
```

Si falla con el mismo SSPI, el problema está fuera de la API.

### 3. Probar con localhost

En local, a veces evita SPN/nombre de máquina:

```text
Server=localhost\SQLEXPRESS;Database=TransportDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;
```

O:

```text
Server=.\SQLEXPRESS;Database=TransportDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;
```

Recomendación: probar primero en `appsettings.Development.json` para no tocar la configuración base.

### 4. Limpiar tickets Kerberos

En una terminal:

```powershell
klist purge
```

Luego cierra y abre Visual Studio/terminal, levanta la API y prueba login.

### 5. Revisar SPN

Para diagnosticar SPNs:

```powershell
setspn -L DESKTOP-Q0FVL2N
```

Si SQL Server corre bajo una cuenta de dominio, revisar SPN de esa cuenta. En entornos locales simples, suele ser más práctico usar `localhost\SQLEXPRESS` o SQL Authentication.

## Opción B: usar SQL Authentication en desarrollo

Usa esta opción si quieres evitar problemas SSPI en tu máquina local.

Ejemplo seguro de formato, sin contraseña real:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=TransportDB;User Id=transport_dev;Password=YOUR_LOCAL_PASSWORD;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

Pasos:

1. Habilitar SQL Server Authentication en SQL Server.
2. Crear un login local, por ejemplo `transport_dev`.
3. Dar permisos sobre `TransportDB`.
4. Colocar la cadena en `Transport.API/appsettings.Development.json` o en User Secrets, no en `appsettings.json` si contiene contraseña.

Ejemplo con User Secrets:

```powershell
cd Transport.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\SQLEXPRESS;Database=TransportDB;User Id=transport_dev;Password=YOUR_LOCAL_PASSWORD;TrustServerCertificate=True;Encrypt=False;"
```

Luego ejecutar:

```powershell
dotnet run --project Transport.API --launch-profile http
```

## Comandos de verificación después de corregir

### 1. Probar login

```powershell
$body = @{ username = "admin"; password = "Admin123" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5075/api/auth/login" -Method Post -ContentType "application/json" -Body $body
```

### 2. Ejecutar smoke básico

```powershell
.\scripts\smoke-test.ps1
```

### 3. Ejecutar todos los smoke tests

```powershell
.\scripts\smoke-test.ps1
.\scripts\smoke-test-write-endpoints.ps1
.\scripts\smoke-test-security.ps1
.\scripts\smoke-test-notifications.ps1
.\scripts\smoke-test-incidents.ps1
.\scripts\smoke-test-tracking.ps1
.\scripts\smoke-test-reports.ps1
.\scripts\smoke-test-system-settings.ps1
.\scripts\smoke-test-integrations.ps1
.\scripts\smoke-test-backups.ps1
.\scripts\smoke-test-users.ps1
```

## Recomendación para este proyecto

Para desarrollo local, la opción más estable suele ser:

1. Mantener `appsettings.json` sin credenciales.
2. Usar `appsettings.Development.json` o User Secrets para la conexión local.
3. Si el equipo no está en dominio o hay problemas Kerberos recurrentes, usar SQL Authentication local.
4. Si se mantiene Windows Auth, cambiar temporalmente `DESKTOP-Q0FVL2N\SQLEXPRESS` por `localhost\SQLEXPRESS` o `.\SQLEXPRESS` y probar.

## Nota sobre Fase 15

Este error no está relacionado con el rediseño de estados de `Trip`. La API compila y levanta; el fallo ocurre cuando EF Core intenta abrir conexión con SQL Server usando autenticación integrada.
