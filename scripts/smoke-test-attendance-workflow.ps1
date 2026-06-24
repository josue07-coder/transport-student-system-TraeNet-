param(
    [string]$BaseUrl
)

$ErrorActionPreference = "Stop"

function Resolve-BaseUrl {
    param([string]$PreferredBaseUrl)

    if ($PreferredBaseUrl) {
        return $PreferredBaseUrl.TrimEnd("/")
    }

    $candidates = @("http://localhost:5075", "http://localhost:5000", "https://localhost:5001")
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

function Login {
    param(
        [string]$Username,
        [string]$Password
    )

    $login = Invoke-Json -Method Post -Url "$base/api/v1/auth/login" -Headers @{} -Body @{
        username = $Username
        password = $Password
    }

    if (-not $login.token) {
        throw "Login no devolvio token."
    }

    return $login.token
}

$base = Resolve-BaseUrl $BaseUrl
Write-Host "API: $base"

try {
    $assistantToken = Login -Username "assistant01" -Password "Assistant123"
    Write-Result "POST /api/v1/auth/login assistant01" "OK"
}
catch {
    Write-Result "POST /api/v1/auth/login assistant01" "SKIP" "No se pudo autenticar assistant01. Posible ambiente SQL/SSPI o datos semilla pendientes. $($_.Exception.Message)"
    exit 0
}

$assistantHeaders = @{ Authorization = "Bearer $assistantToken" }

try {
    $myTrips = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/me/trips" -Headers $assistantHeaders)
    Write-Result "GET /api/v1/me/trips" "OK" "Viajes: $($myTrips.Count)"
}
catch {
    Write-Result "GET /api/v1/me/trips" "FAIL" $_.Exception.Message
    exit 1
}

$trip = $myTrips | Where-Object { $_.status -eq "InProgress" -or "$($_.status)" -eq "InProgress" } | Select-Object -First 1
if (-not $trip) {
    $trip = $myTrips | Select-Object -First 1
}

if (-not $trip) {
    Write-Result "flujo de asistencia" "SKIP" "assistant01 no tiene viajes asignados."
    exit 0
}

$tripId = $trip.id

try {
    $attendance = Invoke-Json -Method Get -Url "$base/api/v1/trips/$tripId/attendance" -Headers $assistantHeaders
    Write-Result "GET /api/v1/trips/{tripId}/attendance" "OK" "Esperados: $($attendance.expectedCount), presentes: $($attendance.presentCount), ausentes: $($attendance.absentCount)"
}
catch {
    Write-Result "GET /api/v1/trips/{tripId}/attendance" "FAIL" $_.Exception.Message
    exit 1
}

try {
    $search = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/trips/$tripId/attendance/search-students?query=a" -Headers $assistantHeaders)
    Write-Result "GET /api/v1/trips/{tripId}/attendance/search-students" "OK" "Resultados: $($search.Count)"
}
catch {
    Write-Result "GET /api/v1/trips/{tripId}/attendance/search-students" "FAIL" $_.Exception.Message
    exit 1
}

$assignedCandidate = $search | Where-Object { $_.isAssignedToRoute -eq $true -and $_.alreadyRegisteredInTrip -eq $true } | Select-Object -First 1
if ($assignedCandidate) {
    try {
        $boarded = Invoke-Json -Method Post -Url "$base/api/v1/trips/$tripId/attendance/mark-boarded" -Headers $assistantHeaders -Body @{
            studentId = $assignedCandidate.studentId
        }
        Write-Result "POST /api/v1/trips/{tripId}/attendance/mark-boarded" "OK" "StudentId: $($boarded.studentId)"
    }
    catch {
        Write-Result "POST /api/v1/trips/{tripId}/attendance/mark-boarded" "FAIL" $_.Exception.Message
        exit 1
    }
}
else {
    Write-Result "POST /api/v1/trips/{tripId}/attendance/mark-boarded" "SKIP" "No hay estudiante esperado disponible en la busqueda."
}

$exceptionalCandidate = $search | Where-Object { $_.isAssignedToRoute -eq $false -and $_.alreadyRegisteredInTrip -eq $false } | Select-Object -First 1
if ($exceptionalCandidate) {
    try {
        $exceptional = Invoke-Json -Method Post -Url "$base/api/v1/trips/$tripId/attendance/mark-boarded" -Headers $assistantHeaders -Body @{
            studentId = $exceptionalCandidate.studentId
            exceptionReason = "Prueba funcional de pasajero excepcional"
        }
        Write-Result "POST /api/v1/trips/{tripId}/attendance/mark-boarded excepcional" "OK" "IsExpectedPassenger: $($exceptional.isExpectedPassenger)"
    }
    catch {
        Write-Result "POST /api/v1/trips/{tripId}/attendance/mark-boarded excepcional" "FAIL" $_.Exception.Message
        exit 1
    }
}
else {
    Write-Result "pasajero excepcional" "SKIP" "No hay estudiante externo disponible en la busqueda."
}

try {
    Invoke-Json -Method Post -Url "$base/api/v1/trips/$tripId/attendance/close" -Headers $assistantHeaders -Body $null | Out-Null
    Write-Result "POST /api/v1/trips/{tripId}/attendance/close" "OK"
}
catch {
    Write-Result "POST /api/v1/trips/{tripId}/attendance/close" "FAIL" $_.Exception.Message
    exit 1
}

$historyCandidate = if ($assignedCandidate) { $assignedCandidate } else { $exceptionalCandidate }
if ($historyCandidate) {
    try {
        $history = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/students/$($historyCandidate.studentId)/attendance-history" -Headers $assistantHeaders)
        Write-Result "GET /api/v1/students/{studentId}/attendance-history" "OK" "Registros: $($history.Count)"
    }
    catch {
        Write-Result "GET /api/v1/students/{studentId}/attendance-history" "FAIL" $_.Exception.Message
        exit 1
    }
}
else {
    Write-Result "GET /api/v1/students/{studentId}/attendance-history" "SKIP" "No hubo estudiante candidato."
}

try {
    $notifications = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/notifications" -Headers $assistantHeaders)
    Write-Result "GET /api/v1/notifications" "OK" "Notificaciones: $($notifications.Count)"
}
catch {
    Write-Result "GET /api/v1/notifications" "FAIL" $_.Exception.Message
    exit 1
}

Write-Host "Smoke attendance workflow finalizado."


