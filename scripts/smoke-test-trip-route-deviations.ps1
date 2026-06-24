param(
    [string]$BaseUrl
)

$ErrorActionPreference = "Stop"

function Resolve-BaseUrl {
    param([string]$PreferredBaseUrl)

    if ($PreferredBaseUrl) {
        return $PreferredBaseUrl.TrimEnd("/")
    }

    $candidates = @(
        "http://localhost:5000",
        "https://localhost:5001",
        "http://localhost:5075"
    )

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

    throw "No se encontrÃ³ API activa en los puertos configurados."
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

function Write-Result {
    param(
        [string]$Name,
        [string]$Status,
        [string]$Detail = ""
    )

    $suffix = if ($Detail) { " - $Detail" } else { "" }
    Write-Host "[$Status] $Name$suffix"
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

$base = Resolve-BaseUrl $BaseUrl
Write-Host "API: $base"

$loginBody = @{
    username = "admin"
    password = "Admin123"
}

try {
    $login = Invoke-Json -Method Post -Url "$base/api/v1/auth/login" -Headers @{} -Body $loginBody
    $token = $login.token
    if (-not $token) {
        throw "Login no devolviÃ³ token."
    }
    Write-Result "POST /api/v1/auth/login" "OK"
}
catch {
    Write-Result "POST /api/v1/auth/login" "FAIL" $_.Exception.Message
    Write-Host "Smoke test bloqueado antes de probar desvÃ­os. Revise conectividad SQL/SSPI si aplica."
    exit 1
}

$headers = @{
    Authorization = "Bearer $token"
}

$inProgressTrips = @()
try {
    $inProgressTrips = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/trips/by-status/InProgress" -Headers $headers)
    Write-Result "GET /api/v1/trips/by-status/InProgress" "OK" "Encontrados: $($inProgressTrips.Count)"
}
catch {
    Write-Result "GET /api/v1/trips/by-status/InProgress" "FAIL" $_.Exception.Message
    exit 1
}

if ($inProgressTrips.Count -eq 0) {
    Write-Result "POST /api/v1/trips/{tripId}/route-deviations" "SKIP" "No hay viajes InProgress para probar."
    exit 0
}

$trip = $inProgressTrips[0]
$tripId = $trip.id
$deviationBody = @{
    reasonType = "RoadClosed"
    reason = "Puente cerrado"
    notes = "Se tomÃ³ una vÃ­a alterna por seguridad."
    latitude = 18.2081
    longitude = -71.1002
}

try {
    $deviation = Invoke-Json -Method Post -Url "$base/api/v1/trips/$tripId/route-deviations" -Headers $headers -Body $deviationBody
    if (-not $deviation.id) {
        throw "La respuesta no incluyÃ³ Id de desvÃ­o."
    }
    Write-Result "POST /api/v1/trips/{tripId}/route-deviations" "OK" "DeviationId: $($deviation.id)"
}
catch {
    Write-Result "POST /api/v1/trips/{tripId}/route-deviations" "FAIL" $_.Exception.Message
    exit 1
}

try {
    $deviations = @(Invoke-Json -Method Get -Url "$base/api/v1/trips/$tripId/route-deviations" -Headers $headers)
    if ($deviations.Count -eq 0) {
        throw "No devolviÃ³ desviaciones para el viaje."
    }
    Write-Result "GET /api/v1/trips/{tripId}/route-deviations" "OK" "Encontrados: $($deviations.Count)"
}
catch {
    Write-Result "GET /api/v1/trips/{tripId}/route-deviations" "FAIL" $_.Exception.Message
    exit 1
}

try {
    $detail = Invoke-Json -Method Get -Url "$base/api/v1/trips/route-deviations/$($deviation.id)" -Headers $headers
    if ($detail.id -ne $deviation.id) {
        throw "El detalle no coincide con el desvÃ­o creado."
    }
    Write-Result "GET /api/v1/trips/route-deviations/{id}" "OK"
}
catch {
    Write-Result "GET /api/v1/trips/route-deviations/{id}" "FAIL" $_.Exception.Message
    exit 1
}

try {
    $completedTrips = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/trips/by-status/Completed" -Headers $headers)
    if ($completedTrips.Count -eq 0) {
        Write-Result "POST desviaciÃ³n en viaje finalizado" "SKIP" "No hay viajes Completed para validar bloqueo."
    }
    else {
        $completedTripId = $completedTrips[0].id
        try {
            Invoke-Json -Method Post -Url "$base/api/v1/trips/$completedTripId/route-deviations" -Headers $headers -Body $deviationBody | Out-Null
            Write-Result "POST desviaciÃ³n en viaje finalizado" "FAIL" "Se esperaba error controlado."
            exit 1
        }
        catch {
            Write-Result "POST desviaciÃ³n en viaje finalizado" "OK" "Bloqueado como se esperaba."
        }
    }
}
catch {
    Write-Result "GET /api/v1/trips/by-status/Completed" "FAIL" $_.Exception.Message
    exit 1
}

Write-Host "Smoke test de desvÃ­os finalizado."


