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

function Get-NextWeekday {
    param([DayOfWeek]$Day = [DayOfWeek]::Monday)

    $date = [DateTime]::Today
    while ($date.DayOfWeek -ne $Day) {
        $date = $date.AddDays(1)
    }

    return $date.ToString("yyyy-MM-dd")
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
    throw "No se encontrÃ³ API activa. Levanta Transport.API antes de ejecutar este smoke test."
}

Write-Host "API detectada: $baseUrl"

$login = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/auth/login" -Body @{
    username = $Username
    password = $Password
}

if (-not $login.Ok) {
    Write-Host "LOGIN FAIL $($login.StatusCode): $($login.Content)"
    throw "No se pudo autenticar. Si aparece 'Failed to generate SSPI context', es el problema SQL Server local documentado."
}

$token = $login.Data.token
if (-not $token) { throw "Login no devolviÃ³ token." }

if ($RouteAssignmentId -eq [Guid]::Empty) {
    $assignments = Invoke-JsonRequest -Client $client -Method GET -Path "/api/v1/route-assignments?PageNumber=1&PageSize=10" -Token $token
    if (-not $assignments.Ok) {
        throw "No se pudieron consultar asignaciones: $($assignments.StatusCode) $($assignments.Content)"
    }

    $firstAssignment = Get-Items $assignments.Data | Select-Object -First 1
    if (-not $firstAssignment) {
        throw "No hay RouteAssignments disponibles. Ejecuta primero smoke-test-write-endpoints.ps1 o pasa -RouteAssignmentId."
    }

    $RouteAssignmentId = [Guid]$firstAssignment.id
}

$operationDate = Get-NextWeekday
$scheduleBody = @{
    routeAssignmentId = $RouteAssignmentId
    direction = "ToSchool"
    departureTime = "06:30:00"
    arrivalTime = "07:30:00"
    validFrom = $operationDate
    validTo = $null
    monday = $true
    tuesday = $true
    wednesday = $true
    thursday = $true
    friday = $true
    saturday = $false
    sunday = $false
}

$createSchedule = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/trip-schedules" -Body $scheduleBody -Token $token
if (-not $createSchedule.Ok) {
    throw "Crear schedule fallÃ³: $($createSchedule.StatusCode) $($createSchedule.Content)"
}

$scheduleId = $createSchedule.Data.id
Write-Host "Schedule creado: $scheduleId"

$materializeBody = @{ operationDate = $operationDate; tripScheduleId = $scheduleId }
$materialize = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/trip-schedules/$scheduleId/materialize" -Body $materializeBody -Token $token
if (-not $materialize.Ok) {
    throw "Materializar trip fallÃ³: $($materialize.StatusCode) $($materialize.Content)"
}

Write-Host "Trip materializado: $($materialize.Data.id) Status=$($materialize.Data.status)"
if ($materialize.Data.status -ne "Scheduled" -and $materialize.Data.status -ne 1) {
    throw "El trip materializado no quedÃ³ en estado Scheduled."
}

$duplicate = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/trip-schedules/$scheduleId/materialize" -Body $materializeBody -Token $token
if ($duplicate.Ok) {
    throw "Se permitiÃ³ materializar un duplicado para el mismo schedule/date."
}

Write-Host "Duplicado rechazado correctamente: $($duplicate.StatusCode)"
Write-Host "smoke-test-trip-schedules OK"


