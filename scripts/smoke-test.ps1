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

    foreach ($url in @(
        "http://localhost:5000",
        "https://localhost:5001",
        "http://localhost:5075",
        "https://localhost:7295"
    )) {
        $urls.Add($url)
    }

    $urls |
        Select-Object -Unique |
        Sort-Object { if ($_ -like "http://*") { 0 } else { 1 } }
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

function Invoke-SmokeRequest {
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

    try {
        $response = $Client.SendAsync($request).GetAwaiter().GetResult()
        $content = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()

        return [pscustomobject]@{
            Ok = $response.IsSuccessStatusCode
            StatusCode = [int]$response.StatusCode
            Content = $content
            Error = if ($response.IsSuccessStatusCode) { $null } else { $content }
        }
    }
    catch {
        return [pscustomobject]@{
            Ok = $false
            StatusCode = 0
            Content = $null
            Error = $_.Exception.Message
        }
    }
}

function Find-ApiBaseUrl {
    foreach ($url in Get-CandidateUrls) {
        $client = New-HttpClient -BaseUrl $url
        try {
            $result = Invoke-SmokeRequest -Client $client -Method "GET" -Path "swagger/index.html"
            if ($result.StatusCode -gt 0) {
                $client.Dispose()
                return $url
            }
        }
        finally {
            $client.Dispose()
        }
    }

    throw "No API instance detected on launchSettings URLs or common localhost ports."
}

$baseUrl = Find-ApiBaseUrl
$client = New-HttpClient -BaseUrl $baseUrl

try {
    Write-Host "Base URL: $baseUrl"

    $results = New-Object System.Collections.Generic.List[object]

    $login = Invoke-SmokeRequest -Client $client -Method "POST" -Path "api/v1/auth/login" -Body @{
        username = $Username
        password = $Password
    }

    $results.Add([pscustomobject]@{
        Group = "AUTH"
        Endpoint = "POST /api/v1/auth/login"
        Ok = $login.Ok
        StatusCode = $login.StatusCode
        Error = $login.Error
    })

    if (-not $login.Ok) {
        throw "Login failed. Cannot continue authenticated smoke test."
    }

    $loginJson = $login.Content | ConvertFrom-Json
    $token = $loginJson.token

    if ([string]::IsNullOrWhiteSpace($token)) {
        throw "Login response did not include token."
    }

    $endpoints = @(
        @{ Group = "AUTH"; Path = "api/v1/me"; Label = "GET /api/v1/me" },
        @{ Group = "AUTH"; Path = "api/v1/users"; Label = "GET /api/v1/users" },
        @{ Group = "AUTH"; Path = "api/v1/roles"; Label = "GET /api/v1/roles" },
        @{ Group = "AUTH"; Path = "api/v1/permissions"; Label = "GET /api/v1/permissions" },

        @{ Group = "EDUCATION"; Path = "api/v1/sectors"; Label = "GET /api/v1/sectors" },
        @{ Group = "EDUCATION"; Path = "api/v1/schools"; Label = "GET /api/v1/schools" },
        @{ Group = "EDUCATION"; Path = "api/v1/grades"; Label = "GET /api/v1/grades" },
        @{ Group = "EDUCATION"; Path = "api/v1/guardians"; Label = "GET /api/v1/guardians" },
        @{ Group = "EDUCATION"; Path = "api/v1/students"; Label = "GET /api/v1/students" },

        @{ Group = "TRANSPORT"; Path = "api/v1/vehicles"; Label = "GET /api/v1/vehicles" },
        @{ Group = "TRANSPORT"; Path = "api/v1/drivers"; Label = "GET /api/v1/drivers" },
        @{ Group = "TRANSPORT"; Path = "api/v1/transport-assistants"; Label = "GET /api/v1/transport-assistants" },
        @{ Group = "TRANSPORT"; Path = "api/v1/stops"; Label = "GET /api/v1/stops" },
        @{ Group = "TRANSPORT"; Path = "api/v1/routes"; Label = "GET /api/v1/routes" },
        @{ Group = "TRANSPORT"; Path = "api/v1/route-assignments"; Label = "GET /api/v1/route-assignments" },
        @{ Group = "TRANSPORT"; Path = "api/v1/trips"; Label = "GET /api/v1/trips" },
        @{ Group = "TRANSPORT"; Path = "api/v1/trip-schedules"; Label = "GET /api/v1/trip-schedules" },
        @{ Group = "TRANSPORT"; Path = "api/v1/non-school-days"; Label = "GET /api/v1/non-school-days" },

        @{ Group = "OPERATIONS"; Path = "api/v1/notifications"; Label = "GET /api/v1/notifications" },
        @{ Group = "OPERATIONS"; Path = "api/v1/notifications/unread"; Label = "GET /api/v1/notifications/unread" },
        @{ Group = "OPERATIONS"; Path = "api/v1/incidents"; Label = "GET /api/v1/incidents" },
        @{ Group = "OPERATIONS"; Path = "api/v1/tracking/active-trips"; Label = "GET /api/v1/tracking/active-trips" },

        @{ Group = "REPORTS"; Path = "api/v1/reports/dashboard"; Label = "GET /api/v1/reports/dashboard" },
        @{ Group = "REPORTS"; Path = "api/v1/reports/trips?startDate=2026-01-01&endDate=2026-12-31"; Label = "GET /api/v1/reports/trips" },
        @{ Group = "REPORTS"; Path = "api/v1/reports/incidents?startDate=2026-01-01&endDate=2026-12-31"; Label = "GET /api/v1/reports/incidents" },
        @{ Group = "REPORTS"; Path = "api/v1/reports/audit-summary?startDate=2026-01-01&endDate=2026-12-31"; Label = "GET /api/v1/reports/audit-summary" },

        @{ Group = "ADMIN"; Path = "api/v1/audit-logs"; Label = "GET /api/v1/audit-logs" },
        @{ Group = "ADMIN"; Path = "api/v1/system-settings"; Label = "GET /api/v1/system-settings" },
        @{ Group = "ADMIN"; Path = "api/v1/backups"; Label = "GET /api/v1/backups" },
        @{ Group = "ADMIN"; Path = "api/v1/backups/latest"; Label = "GET /api/v1/backups/latest" },
        @{ Group = "ADMIN"; Path = "api/v1/integrations/test-distance?originLat=18.2081&originLng=-71.1002&destinationLat=18.5001&destinationLng=-69.9886"; Label = "GET /api/v1/integrations/test-distance" }
    )

    foreach ($endpoint in $endpoints) {
        $result = Invoke-SmokeRequest -Client $client -Method "GET" -Path $endpoint.Path -Token $token
        $results.Add([pscustomobject]@{
            Group = $endpoint.Group
            Endpoint = $endpoint.Label
            Ok = $result.Ok
            StatusCode = $result.StatusCode
            Error = $result.Error
        })
    }

    $results |
        Select-Object Group, Endpoint, @{Name="Result"; Expression={ if ($_.Ok) { "OK" } else { "FAIL" } }}, StatusCode, Error |
        Format-Table -AutoSize -Wrap

    $total = $results.Count
    $successful = ($results | Where-Object { $_.Ok }).Count
    $failed = $total - $successful

    Write-Host ""
    Write-Host "SUMMARY"
    Write-Host "Total endpoints tested: $total"
    Write-Host "Successful: $successful"
    Write-Host "Failed: $failed"

    if ($failed -gt 0) {
        Write-Host ""
        Write-Host "FAILURES"
        $results |
            Where-Object { -not $_.Ok } |
            Select-Object Group, Endpoint, StatusCode, Error |
            Format-Table -AutoSize -Wrap
        exit 1
    }
}
finally {
    $client.Dispose()
}


