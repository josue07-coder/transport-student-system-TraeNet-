param(
    [string]$Username = "admin",
    [string]$Password = "Admin123"
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
    $client.Timeout = [TimeSpan]::FromSeconds(30)
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

function Add-Result {
    param(
        [System.Collections.Generic.List[object]]$Results,
        [string]$Name,
        [string]$Status,
        [string]$Details = ""
    )

    $Results.Add([pscustomobject]@{
        Test = $Name
        Status = $Status
        Details = $Details
    })
}

function Get-ScheduledDeparture {
    param($Trip)

    if ($Trip.scheduledDepartureTime) {
        return [DateTime]$Trip.scheduledDepartureTime
    }

    return [DateTime]$Trip.ScheduledDepartureTime
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
    throw "No se encontro API activa en los puertos configurados."
}

$results = [System.Collections.Generic.List[object]]::new()
Write-Host "API detectada: $baseUrl"

$login = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/auth/login" -Body @{
    username = $Username
    password = $Password
}

if (-not $login.Ok) {
    Add-Result $results "login admin" "FAIL" "HTTP $($login.StatusCode): $($login.Content)"
    $results | Format-Table -AutoSize
    exit 1
}

$token = $login.Data.token
Add-Result $results "login admin" "OK"

$scheduledResponse = Invoke-JsonRequest -Client $client -Method GET -Path "/api/v1/trips/by-status/Scheduled" -Token $token
if (-not $scheduledResponse.Ok) {
    Add-Result $results "consultar viajes Scheduled" "FAIL" "HTTP $($scheduledResponse.StatusCode): $($scheduledResponse.Content)"
    $results | Format-Table -AutoSize
    exit 1
}

$trips = Get-Items $scheduledResponse.Data | Where-Object { $_.scheduledDepartureTime -or $_.ScheduledDepartureTime }
$now = [DateTime]::UtcNow
$tolerance = 5

$normalTrip = $trips | Where-Object {
    $scheduled = Get-ScheduledDeparture $_
    $now -ge $scheduled.AddMinutes(-$tolerance) -and $now -le $scheduled
} | Select-Object -First 1

$earlyTrip = $trips | Where-Object {
    $scheduled = Get-ScheduledDeparture $_
    $now -lt $scheduled.AddMinutes(-$tolerance)
} | Select-Object -First 1

$lateTrip = $trips | Where-Object {
    $scheduled = Get-ScheduledDeparture $_
    $now -gt $scheduled.AddMinutes(10)
} | Select-Object -First 1

if ($normalTrip) {
    $response = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/trips/start" -Token $token -Body @{
        tripId = $normalTrip.id
        routeAssignmentId = $normalTrip.routeAssignmentId
    }
    Add-Result $results "inicio normal" ($(if ($response.Ok) { "OK" } else { "FAIL" })) "HTTP $($response.StatusCode)"
} else {
    Add-Result $results "inicio normal" "SKIP" "No hay viaje Scheduled dentro de tolerancia."
}

if ($earlyTrip) {
    $blocked = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/trips/start" -Token $token -Body @{
        tripId = $earlyTrip.id
        routeAssignmentId = $earlyTrip.routeAssignmentId
    }
    Add-Result $results "inicio temprano bloqueado" ($(if (-not $blocked.Ok) { "OK" } else { "FAIL" })) "HTTP $($blocked.StatusCode)"

    $forced = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/trips/start" -Token $token -Body @{
        tripId = $earlyTrip.id
        routeAssignmentId = $earlyTrip.routeAssignmentId
        forceEarlyStart = $true
        earlyStartReason = "Inicio anticipado autorizado por smoke test"
    }
    Add-Result $results "inicio anticipado autorizado" ($(if ($forced.Ok) { "OK" } else { "FAIL" })) "HTTP $($forced.StatusCode)"
} else {
    Add-Result $results "inicio temprano bloqueado" "SKIP" "No hay viaje Scheduled antes de tolerancia."
    Add-Result $results "inicio anticipado autorizado" "SKIP" "No hay viaje Scheduled antes de tolerancia."
}

if ($lateTrip) {
    $response = Invoke-JsonRequest -Client $client -Method POST -Path "/api/v1/trips/start" -Token $token -Body @{
        tripId = $lateTrip.id
        routeAssignmentId = $lateTrip.routeAssignmentId
    }
    Add-Result $results "inicio tardio" ($(if ($response.Ok) { "OK" } else { "FAIL" })) "HTTP $($response.StatusCode)"
} else {
    Add-Result $results "inicio tardio" "SKIP" "No hay viaje Scheduled con atraso mayor a 10 minutos."
}

$results | Format-Table -AutoSize

$failed = @($results | Where-Object { $_.Status -eq "FAIL" })
if ($failed.Count -gt 0) {
    exit 1
}


