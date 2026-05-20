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

    $login = Invoke-SmokeRequest -Client $client -Method "POST" -Path "api/auth/login" -Body @{
        username = $Username
        password = $Password
    }

    $results.Add([pscustomobject]@{
        Group = "AUTH"
        Endpoint = "POST /api/auth/login"
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
        @{ Group = "AUTH"; Path = "api/me"; Label = "GET /api/me" },
        @{ Group = "AUTH"; Path = "api/users"; Label = "GET /api/users" },
        @{ Group = "AUTH"; Path = "api/roles"; Label = "GET /api/roles" },
        @{ Group = "AUTH"; Path = "api/permissions"; Label = "GET /api/permissions" },

        @{ Group = "EDUCATION"; Path = "api/sectors"; Label = "GET /api/sectors" },
        @{ Group = "EDUCATION"; Path = "api/schools"; Label = "GET /api/schools" },
        @{ Group = "EDUCATION"; Path = "api/grades"; Label = "GET /api/grades" },
        @{ Group = "EDUCATION"; Path = "api/guardians"; Label = "GET /api/guardians" },
        @{ Group = "EDUCATION"; Path = "api/students"; Label = "GET /api/students" },

        @{ Group = "TRANSPORT"; Path = "api/vehicles"; Label = "GET /api/vehicles" },
        @{ Group = "TRANSPORT"; Path = "api/drivers"; Label = "GET /api/drivers" },
        @{ Group = "TRANSPORT"; Path = "api/transport-assistants"; Label = "GET /api/transport-assistants" },
        @{ Group = "TRANSPORT"; Path = "api/stops"; Label = "GET /api/stops" },
        @{ Group = "TRANSPORT"; Path = "api/routes"; Label = "GET /api/routes" },
        @{ Group = "TRANSPORT"; Path = "api/route-assignments"; Label = "GET /api/route-assignments" },
        @{ Group = "TRANSPORT"; Path = "api/trips"; Label = "GET /api/trips" }
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
