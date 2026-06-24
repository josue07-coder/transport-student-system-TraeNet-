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

    if ($null -eq $Response) {
        return @()
    }

    $itemsProperty = $Response.PSObject.Properties["items"]
    if ($itemsProperty) {
        return As-Array $itemsProperty.Value
    }

    $valueProperty = $Response.PSObject.Properties["value"]
    if ($valueProperty) {
        return As-Array $valueProperty.Value
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
        throw "Login de $Username no devolvio token."
    }

    return $login.token
}

$base = Resolve-BaseUrl $BaseUrl
Write-Host "API: $base"

$profiles = @(
    @{ Name = "Admin"; Username = "admin"; Password = "Admin123" },
    @{ Name = "Supervisor"; Username = "supervisor"; Password = "Supervisor123" },
    @{ Name = "Driver"; Username = "driver01"; Password = "Driver123" },
    @{ Name = "Assistant"; Username = "assistant01"; Password = "Assistant123" },
    @{ Name = "Guardian"; Username = "guardian01"; Password = "Guardian123" }
)

$tokens = @{}
foreach ($profile in $profiles) {
    try {
        $tokens[$profile.Name] = Login -Username $profile.Username -Password $profile.Password
        Write-Result "login $($profile.Username)" "OK"
    }
    catch {
        Write-Result "login $($profile.Username)" "FAIL" $_.Exception.Message
        exit 1
    }
}

$adminHeaders = @{ Authorization = "Bearer $($tokens.Admin)" }
$supervisorHeaders = @{ Authorization = "Bearer $($tokens.Supervisor)" }
$driverHeaders = @{ Authorization = "Bearer $($tokens.Driver)" }
$assistantHeaders = @{ Authorization = "Bearer $($tokens.Assistant)" }
$guardianHeaders = @{ Authorization = "Bearer $($tokens.Guardian)" }

try {
    [array]$scheduledTrips = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/trips/by-status/Scheduled" -Headers $adminHeaders)
    [array]$inProgressTrips = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/trips/by-status/InProgress" -Headers $adminHeaders)
    [array]$completedTrips = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/trips/by-status/Completed" -Headers $adminHeaders)

    if ($scheduledTrips.Count -lt 1 -or $inProgressTrips.Count -lt 1 -or $completedTrips.Count -lt 1) {
        throw "Se esperaban viajes Scheduled/InProgress/Completed. Encontrados S=$($scheduledTrips.Count), IP=$($inProgressTrips.Count), C=$($completedTrips.Count)."
    }

    Write-Result "viajes semilla por estado" "OK" "Scheduled=$($scheduledTrips.Count), InProgress=$($inProgressTrips.Count), Completed=$($completedTrips.Count)"
}
catch {
    Write-Result "viajes semilla por estado" "FAIL" $_.Exception.Message
    exit 1
}

$inProgressTrip = $null

try {
    [array]$assistantTrips = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/me/trips" -Headers $assistantHeaders)
    if ($assistantTrips.Count -lt 1) {
        throw "assistant01 no tiene viajes visibles."
    }

    $inProgressTrip = $assistantTrips |
        Where-Object { "$($_.status)" -eq "InProgress" -or "$($_.status)" -eq "2" } |
        Select-Object -First 1

    if (-not $inProgressTrip) {
        throw "assistant01 no tiene un viaje InProgress visible."
    }

    Write-Result "GET /api/v1/me/trips assistant" "OK" "Encontrados: $($assistantTrips.Count)"
}
catch {
    Write-Result "GET /api/v1/me/trips assistant" "FAIL" $_.Exception.Message
    exit 1
}

try {
    [array]$driverTrips = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/me/trips" -Headers $driverHeaders)
    if ($driverTrips.Count -lt 1) {
        throw "driver01 no tiene viajes visibles."
    }

    Write-Result "GET /api/v1/me/trips driver" "OK" "Encontrados: $($driverTrips.Count)"
}
catch {
    Write-Result "GET /api/v1/me/trips driver" "FAIL" $_.Exception.Message
    exit 1
}

try {
    [array]$guardianTrips = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/me/trips" -Headers $guardianHeaders)
    if ($guardianTrips.Count -lt 1) {
        throw "guardian01 no tiene viajes visibles."
    }

    Write-Result "GET /api/v1/me/trips guardian" "OK" "Encontrados: $($guardianTrips.Count)"
}
catch {
    Write-Result "GET /api/v1/me/trips guardian" "FAIL" $_.Exception.Message
    exit 1
}

try {
    [array]$passengers = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/trips/$($inProgressTrip.id)/passengers" -Headers $assistantHeaders)
    if ($passengers.Count -lt 1) {
        throw "El viaje InProgress no devolvio pasajeros."
    }

    Write-Result "GET /api/v1/trips/{tripId}/passengers assistant" "OK" "Pasajeros: $($passengers.Count)"
}
catch {
    Write-Result "GET /api/v1/trips/{tripId}/passengers assistant" "FAIL" $_.Exception.Message
    exit 1
}

try {
    $notifications = Get-Items (Invoke-Json -Method Get -Url "$base/api/v1/notifications" -Headers $assistantHeaders)
    Write-Result "GET /api/v1/notifications assistant" "OK" "Encontradas: $($notifications.Count)"
}
catch {
    Write-Result "GET /api/v1/notifications assistant" "FAIL" $_.Exception.Message
    exit 1
}

$cancelStatus = Invoke-Status -Method Put -Url "$base/api/v1/trips/$($inProgressTrip.id)/cancel" -Headers $assistantHeaders -Body @{
    reason = "Prueba de bloqueo operativo"
}
if ($cancelStatus -eq 401 -or $cancelStatus -eq 403 -or $cancelStatus -eq 400) {
    Write-Result "assistant no cancela viajes" "OK" "Bloqueado con status $cancelStatus"
}
else {
    Write-Result "assistant no cancela viajes" "FAIL" "Se esperaba bloqueo, status $cancelStatus"
    exit 1
}

$routeCreateStatus = Invoke-Status -Method Post -Url "$base/api/v1/routes" -Headers $assistantHeaders -Body @{
    name = "Ruta no permitida"
    schoolId = [guid]::NewGuid()
    startTime = "06:00"
    endTime = "07:00"
}
if ($routeCreateStatus -eq 401 -or $routeCreateStatus -eq 403 -or $routeCreateStatus -eq 400) {
    Write-Result "assistant no gestiona rutas" "OK" "Bloqueado con status $routeCreateStatus"
}
else {
    Write-Result "assistant no gestiona rutas" "FAIL" "Se esperaba bloqueo, status $routeCreateStatus"
    exit 1
}

try {
    $supervisorReports = Invoke-Json -Method Get -Url "$base/api/v1/reports/dashboard" -Headers $supervisorHeaders
    if ($null -eq $supervisorReports) {
        throw "Dashboard sin respuesta."
    }
    Write-Result "supervisor consulta dashboard" "OK"
}
catch {
    Write-Result "supervisor consulta dashboard" "FAIL" $_.Exception.Message
    exit 1
}

Write-Host "Smoke test de datos semilla operativos finalizado."


