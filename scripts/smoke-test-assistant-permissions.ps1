param(
    [string]$BaseUrl
)

$ErrorActionPreference = "Stop"

function Resolve-BaseUrl {
    param([string]$PreferredBaseUrl)

    if ($PreferredBaseUrl) {
        return $PreferredBaseUrl.TrimEnd("/")
    }

    $candidates = @("http://localhost:5000", "https://localhost:5001", "http://localhost:5075")
    foreach ($candidate in $candidates) {
        try {
            $response = Invoke-WebRequest -Uri "$candidate/swagger/v1/swagger.json" -Method Get -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                return $candidate
            }
        }
        catch {
        }
    }

    throw "No se encontro API activa en los puertos configurados."
}

function Invoke-Json {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        [object]$Body
    )

    $params = @{
        Method = $Method
        Uri = $Url
        Headers = $Headers
        TimeoutSec = 30
    }

    if ($null -ne $Body) {
        $params.ContentType = "application/json"
        $params.Body = ($Body | ConvertTo-Json -Depth 10)
    }

    Invoke-RestMethod @params
}

function Invoke-Status {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        [object]$Body
    )

    try {
        Invoke-Json -Method $Method -Url $Url -Headers $Headers -Body $Body | Out-Null
        return 200
    }
    catch {
        if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
            return [int]$_.Exception.Response.StatusCode
        }
        throw
    }
}

function As-Array {
    param([object]$Value)

    if ($null -eq $Value) {
        return @()
    }

    if ($Value -is [System.Array]) {
        return $Value
    }

    return @($Value)
}

function Get-Items {
    param([object]$Response)

    if ($Response.items) {
        return As-Array $Response.items
    }

    return As-Array $Response
}

function Write-Result {
    param(
        [string]$Name,
        [string]$Status,
        [string]$Detail = ""
    )

    $suffix = if ($Detail) { " - $Detail" } else { "" }
    Write-Host "[$Status] $Name$suffix"
}

function Login-WithRetry {
    param(
        [string]$Username,
        [string]$Password,
        [int]$Attempts = 5
    )

    $lastError = $null
    for ($i = 1; $i -le $Attempts; $i++) {
        try {
            $login = Invoke-Json -Method Post -Url "$base/api/v1/auth/login" -Headers @{} -Body @{
                username = $Username
                password = $Password
            }

            if ($login.token) {
                return $login
            }

            $lastError = "Login no devolvio token."
        }
        catch {
            $lastError = $_.Exception.Message
        }

        Start-Sleep -Seconds 2
    }

    throw $lastError
}

$base = Resolve-BaseUrl $BaseUrl
Write-Host "API: $base"

try {
    $adminLogin = Login-WithRetry -Username "admin" -Password "Admin123"
    $adminToken = $adminLogin.token
    Write-Result "POST /api/v1/auth/login admin" "OK"
}
catch {
    Write-Result "POST /api/v1/auth/login admin" "FAIL" $_.Exception.Message
    exit 1
}

$adminHeaders = @{ Authorization = "Bearer $adminToken" }

try {
    $roles = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/roles" -Headers $adminHeaders)
    $assistantRole = $roles | Where-Object { $_.name -eq "TransportAssistant" } | Select-Object -First 1
    if (-not $assistantRole) {
        Write-Result "buscar rol TransportAssistant" "SKIP" "No existe rol TransportAssistant."
        exit 0
    }
}
catch {
    Write-Result "GET /api/v1/roles" "FAIL" $_.Exception.Message
    exit 1
}

try {
    $assistantUsers = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/users/by-role/$($assistantRole.id)" -Headers $adminHeaders)
    if ($assistantUsers.Count -eq 0) {
        Write-Result "buscar usuarios TransportAssistant" "SKIP" "No hay usuarios asistentes."
        exit 0
    }
}
catch {
    Write-Result "GET /api/v1/users/by-role/{assistantRoleId}" "FAIL" $_.Exception.Message
    exit 1
}

$assistantToken = $null
$assistantUser = $null
$assistantCandidates = @($assistantUsers | Select-Object -First 3)
foreach ($candidate in $assistantCandidates) {
    try {
        $login = Login-WithRetry -Username $candidate.username -Password $candidate.username -Attempts 1
        if ($login.token) {
            $assistantToken = $login.token
            $assistantUser = $candidate
            break
        }
    }
    catch {
    }
}

if (-not $assistantToken) {
    Write-Result "login assistant" "SKIP" "No se pudo iniciar sesion con los primeros $($assistantCandidates.Count) asistentes usando username como password inicial."
    exit 0
}

Write-Result "POST /api/v1/auth/login assistant" "OK" "Username: $($assistantUser.username)"
$assistantHeaders = @{ Authorization = "Bearer $assistantToken" }

try {
    $myTrips = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/me/trips" -Headers $assistantHeaders)
    Write-Result "GET /api/v1/me/trips assistant" "OK" "Encontrados: $($myTrips.Count)"
}
catch {
    Write-Result "GET /api/v1/me/trips assistant" "FAIL" $_.Exception.Message
    exit 1
}

if ($myTrips.Count -eq 0) {
    Write-Result "permisos operativos assistant" "SKIP" "El asistente no tiene viajes asignados."
    exit 0
}

$assignedTrip = $myTrips[0]
$assignedTripId = $assignedTrip.id

try {
    $passengers = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/trips/$assignedTripId/passengers" -Headers $assistantHeaders)
    Write-Result "GET /api/v1/trips/{tripId}/passengers assistant" "OK" "Pasajeros: $($passengers.Count)"
}
catch {
    Write-Result "GET /api/v1/trips/{tripId}/passengers assistant" "FAIL" $_.Exception.Message
    exit 1
}

$cancelStatus = Invoke-Status -Method Put -Url "$base/api/v1/trips/$assignedTripId/cancel" -Headers $assistantHeaders -Body @{
    reason = "Cancelacion no permitida para assistant"
}
if ($cancelStatus -eq 401 -or $cancelStatus -eq 403 -or $cancelStatus -eq 400) {
    Write-Result "PUT /api/v1/trips/{id}/cancel assistant" "OK" "Bloqueado con status $cancelStatus"
}
else {
    Write-Result "PUT /api/v1/trips/{id}/cancel assistant" "FAIL" "Se esperaba bloqueo, status $cancelStatus"
    exit 1
}

if ($assignedTrip.status -eq "InProgress" -or "$($assignedTrip.status)" -eq "1") {
    try {
        $deviation = Invoke-Json -Method Post -Url "$base/api/v1/trips/$assignedTripId/route-deviations" -Headers $assistantHeaders -Body @{
            reasonType = "Other"
            reason = "Prueba de permiso de asistente"
            notes = "Smoke test assistant permissions"
            latitude = 18.2081
            longitude = -71.1002
        }
        Write-Result "POST /api/v1/trips/{tripId}/route-deviations assistant" "OK" "DeviationId: $($deviation.id)"
    }
    catch {
        Write-Result "POST /api/v1/trips/{tripId}/route-deviations assistant" "FAIL" $_.Exception.Message
        exit 1
    }
}
else {
    Write-Result "POST /api/v1/trips/{tripId}/route-deviations assistant" "SKIP" "El viaje asignado no esta InProgress."
}

try {
    $allTrips = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/trips" -Headers $adminHeaders)
    if ($allTrips.Count -eq 0) {
        Write-Result "operar viaje no asignado" "SKIP" "No hay viajes para contrastar permisos."
    }
    else {
        $assignedIds = @($myTrips | ForEach-Object { "$($_.id)" })
        $otherTrip = $allTrips | Where-Object { $assignedIds -notcontains "$($_.id)" } | Select-Object -First 1
        if ($otherTrip) {
            $status = Invoke-Status -Method Get -Url "$base/api/v1/trips/$($otherTrip.id)/passengers" -Headers $assistantHeaders -Body $null
            if ($status -eq 401 -or $status -eq 403 -or $status -eq 400) {
                Write-Result "assistant opera viaje no asignado" "OK" "Bloqueado con status $status"
            }
            else {
                Write-Result "assistant opera viaje no asignado" "FAIL" "Se esperaba bloqueo, status $status"
                exit 1
            }
        }
        else {
            Write-Result "assistant opera viaje no asignado" "SKIP" "No se encontro un viaje no asignado al asistente."
        }
    }
}
catch {
    Write-Result "validar viaje no asignado" "FAIL" $_.Exception.Message
    exit 1
}

Write-Host "Smoke test de permisos de asistente finalizado."


