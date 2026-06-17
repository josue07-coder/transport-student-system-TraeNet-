param(
    [string]$Username = "admin",
    [string]$Password = "Admin123",
    [Guid]$RouteAssignmentId = [Guid]::Empty
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

function Get-CandidateUrls {
    $urls = New-Object System.Collections.Generic.List[string]
    $launchSettingsPath = Join-Path $PSScriptRoot "..\Transport.API\Properties\launchSettings.json"

    if (Test-Path $launchSettingsPath) {
        $launchSettings = Get-Content $launchSettingsPath -Raw | ConvertFrom-Json
        foreach ($profile in $launchSettings.profiles.PSObject.Properties.Value) {
            if ($profile.applicationUrl) {
                foreach ($url in ($profile.applicationUrl -split ";")) {
                    if (-not [string]::IsNullOrWhiteSpace($url)) {
                        $urls.Add($url.TrimEnd("/"))
                    }
                }
            }
        }
    }

    foreach ($url in @("http://localhost:5000", "https://localhost:5001", "http://localhost:5075", "https://localhost:7295")) {
        $urls.Add($url)
    }

    $urls | Select-Object -Unique | Sort-Object { if ($_ -like "http://*") { 0 } else { 1 } }
}

function New-HttpClient {
    param([string]$BaseUrl)

    $handler = [System.Net.Http.HttpClientHandler]::new()
    if ($BaseUrl -like "https://*") {
        $handler.ServerCertificateCustomValidationCallback = { $true }
    }

    $client = [System.Net.Http.HttpClient]::new($handler)
    $client.BaseAddress = [Uri]::new($BaseUrl.TrimEnd("/") + "/")
    $client.Timeout = [TimeSpan]::FromSeconds(20)
    return $client
}

function Invoke-JsonRequest {
    param(
        [System.Net.Http.HttpClient]$Client,
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [string]$Token = $null
    )

    $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($Method), $Path.TrimStart("/"))
    if ($Token) {
        $request.Headers.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new("Bearer", $Token)
    }

    if ($null -ne $Body) {
        $json = $Body | ConvertTo-Json -Depth 10
        $request.Content = [System.Net.Http.StringContent]::new($json, [System.Text.Encoding]::UTF8, "application/json")
    }

    $response = $Client.SendAsync($request).GetAwaiter().GetResult()
    $content = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    $data = $null
    if (-not [string]::IsNullOrWhiteSpace($content)) {
        try { $data = $content | ConvertFrom-Json } catch { $data = $content }
    }

    return [pscustomobject]@{
        Ok = $response.IsSuccessStatusCode
        StatusCode = [int]$response.StatusCode
        Content = $content
        Data = $data
    }
}

function Get-Items {
    param($Data)

    if ($null -eq $Data) { return @() }
    if ($Data.items) { return @($Data.items) }
    if ($Data.Items) { return @($Data.Items) }
    if ($Data -is [array]) { return @($Data) }
    return @($Data)
}

$baseUrl = $null
$client = $null
foreach ($url in Get-CandidateUrls) {
    try {
        $candidate = New-HttpClient $url
        $swagger = Invoke-JsonRequest -Client $candidate -Method GET -Path "/swagger/v1/swagger.json"
        if ($swagger.Ok) {
            $baseUrl = $url
            $client = $candidate
            break
        }
        $candidate.Dispose()
    } catch {
    }
}

if (-not $client) {
    throw "No se encontró API activa. Levanta Transport.API antes de ejecutar este smoke test."
}

Write-Host "API detectada: $baseUrl"

$login = Invoke-JsonRequest -Client $client -Method POST -Path "/api/auth/login" -Body @{
    username = $Username
    password = $Password
}

if (-not $login.Ok) {
    Write-Host "LOGIN FAIL $($login.StatusCode): $($login.Content)"
    throw "No se pudo autenticar. Si aparece 'Failed to generate SSPI context', es el problema SQL Server local documentado."
}

$token = $login.Data.token
$operationDate = "2026-06-10"

$createDay = Invoke-JsonRequest -Client $client -Method POST -Path "/api/non-school-days" -Token $token -Body @{
    date = $operationDate
    reasonType = "Holiday"
    reason = "Feriado de prueba"
}

if (-not $createDay.Ok) {
    throw "Crear NonSchoolDay falló: $($createDay.StatusCode) $($createDay.Content)"
}

Write-Host "NonSchoolDay creado: $($createDay.Data.id)"

$range = Invoke-JsonRequest -Client $client -Method GET -Path "/api/non-school-days/by-date-range?startDate=$operationDate&endDate=$operationDate" -Token $token
if (-not $range.Ok) {
    throw "Consulta por rango falló: $($range.StatusCode) $($range.Content)"
}

if ($RouteAssignmentId -eq [Guid]::Empty) {
    $assignments = Invoke-JsonRequest -Client $client -Method GET -Path "/api/route-assignments?PageNumber=1&PageSize=10" -Token $token
    if (-not $assignments.Ok) {
        throw "No se pudieron consultar asignaciones: $($assignments.StatusCode) $($assignments.Content)"
    }

    $firstAssignment = Get-Items $assignments.Data | Select-Object -First 1
    if (-not $firstAssignment) {
        throw "No hay RouteAssignments disponibles. Ejecuta primero smoke-test-write-endpoints.ps1 o pasa -RouteAssignmentId."
    }

    $RouteAssignmentId = [Guid]$firstAssignment.id
}

$schedule = Invoke-JsonRequest -Client $client -Method POST -Path "/api/trip-schedules" -Token $token -Body @{
    routeAssignmentId = $RouteAssignmentId
    direction = "ToSchool"
    departureTime = "06:30:00"
    arrivalTime = "07:30:00"
    validFrom = "2026-06-01"
    validTo = $null
    monday = $true
    tuesday = $true
    wednesday = $true
    thursday = $true
    friday = $true
    saturday = $false
    sunday = $false
}

if (-not $schedule.Ok) {
    throw "Crear TripSchedule falló: $($schedule.StatusCode) $($schedule.Content)"
}

$scheduleId = $schedule.Data.id
$trip = Invoke-JsonRequest -Client $client -Method POST -Path "/api/trip-schedules/$scheduleId/materialize" -Token $token -Body @{
    tripScheduleId = $scheduleId
    operationDate = $operationDate
}

if (-not $trip.Ok) {
    throw "Materializar viaje falló: $($trip.StatusCode) $($trip.Content)"
}

Write-Host "Trip materializado: $($trip.Data.id) Status=$($trip.Data.status)"
if ($trip.Data.status -ne "NotOperating" -and $trip.Data.status -ne 5) {
    throw "El trip materializado en NonSchoolDay no quedó NotOperating."
}

Write-Host "smoke-test-non-school-days OK"
