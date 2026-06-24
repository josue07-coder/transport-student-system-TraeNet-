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

function Write-Result {
    param(
        [string]$Name,
        [string]$Status,
        [string]$Detail = ""
    )

    $suffix = if ($Detail) { " - $Detail" } else { "" }
    Write-Host "[$Status] $Name$suffix"
}

$base = Resolve-BaseUrl $BaseUrl
Write-Host "API: $base"

try {
    $login = Invoke-Json -Method Post -Url "$base/api/v1/auth/login" -Headers @{} -Body @{
        username = "admin"
        password = "Admin123"
    }
    $token = $login.token
    if (-not $token) {
        throw "Login no devolviÃ³ token."
    }
    Write-Result "POST /api/v1/auth/login" "OK"
}
catch {
    Write-Result "POST /api/v1/auth/login" "FAIL" $_.Exception.Message
    exit 1
}

$headers = @{ Authorization = "Bearer $token" }

try {
    $trips = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/trips/by-status/InProgress" -Headers $headers)
    Write-Result "GET /api/v1/trips/by-status/InProgress" "OK" "Encontrados: $($trips.Count)"
}
catch {
    Write-Result "GET /api/v1/trips/by-status/InProgress" "FAIL" $_.Exception.Message
    exit 1
}

if ($trips.Count -eq 0) {
    Write-Result "POST /api/v1/trips/{tripId}/passengers/exceptional" "SKIP" "No hay viajes InProgress."
    exit 0
}

$selectedTrip = $null
$selectedStudent = $null
$selectedPassengers = @()

try {
    $studentsResponse = Invoke-Json -Method Get -Url "$base/api/v1/students?PageNumber=1&PageSize=100" -Headers $headers
    $students = if ($studentsResponse.items) { As-Array $studentsResponse.items } else { As-Array $studentsResponse }
    Write-Result "GET /api/v1/students" "OK" "Encontrados: $($students.Count)"
}
catch {
    Write-Result "GET /api/v1/students" "FAIL" $_.Exception.Message
    exit 1
}

foreach ($trip in $trips) {
    $passengers = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/trips/$($trip.id)/passengers" -Headers $headers)
    $passengerIds = @($passengers | ForEach-Object { "$($_.studentId)" })
    $candidate = $students | Where-Object { $passengerIds -notcontains "$($_.id)" } | Select-Object -First 1
    if ($candidate) {
        $selectedTrip = $trip
        $selectedStudent = $candidate
        $selectedPassengers = $passengers
        break
    }
}

if (-not $selectedTrip -or -not $selectedStudent) {
    Write-Result "POST /api/v1/trips/{tripId}/passengers/exceptional" "SKIP" "No se encontrÃ³ estudiante fuera del snapshot de viajes activos."
    exit 0
}

$body = @{
    studentId = $selectedStudent.id
    exceptionReason = "AbordÃ³ por cambio temporal autorizado."
    notes = "El tutor fue informado."
    boardedAt = (Get-Date).ToUniversalTime().ToString("o")
    latitude = 18.2081
    longitude = -71.1002
}

try {
    $created = Invoke-Json -Method Post -Url "$base/api/v1/trips/$($selectedTrip.id)/passengers/exceptional" -Headers $headers -Body $body
    if ($created.isExpectedPassenger -ne $false) {
        throw "La respuesta no marcÃ³ IsExpectedPassenger=false."
    }
    Write-Result "POST /api/v1/trips/{tripId}/passengers/exceptional" "OK" "StudentId: $($created.studentId)"
}
catch {
    Write-Result "POST /api/v1/trips/{tripId}/passengers/exceptional" "FAIL" $_.Exception.Message
    exit 1
}

try {
    $updatedPassengers = As-Array (Invoke-Json -Method Get -Url "$base/api/v1/trips/$($selectedTrip.id)/passengers" -Headers $headers)
    $exceptional = $updatedPassengers | Where-Object { "$($_.studentId)" -eq "$($selectedStudent.id)" -and $_.isExpectedPassenger -eq $false } | Select-Object -First 1
    if (-not $exceptional) {
        throw "No se encontrÃ³ el pasajero excepcional en la consulta de pasajeros."
    }
    Write-Result "GET /api/v1/trips/{tripId}/passengers" "OK" "Pasajero excepcional visible."
}
catch {
    Write-Result "GET /api/v1/trips/{tripId}/passengers" "FAIL" $_.Exception.Message
    exit 1
}

try {
    Invoke-Json -Method Post -Url "$base/api/v1/trips/$($selectedTrip.id)/passengers/exceptional" -Headers $headers -Body $body | Out-Null
    Write-Result "POST pasajero excepcional duplicado" "FAIL" "Se esperaba error controlado."
    exit 1
}
catch {
    Write-Result "POST pasajero excepcional duplicado" "OK" "Bloqueado como se esperaba."
}

Write-Host "Smoke test de pasajeros excepcionales finalizado."


