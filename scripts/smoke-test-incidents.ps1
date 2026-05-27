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
                return $url
            }
        }
        finally {
            $client.Dispose()
        }
    }

    throw "No API instance detected on launchSettings URLs or common localhost ports."
}

function Get-ResponseId {
    param([string]$Content)

    $json = $Content | ConvertFrom-Json
    if ($json.Id) { return [string]$json.Id }
    if ($json.id) { return [string]$json.id }
    throw "Response did not include Id. Content: $Content"
}

function Invoke-Step {
    param(
        [string]$Group,
        [string]$Label,
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [switch]$ReturnId,
        [switch]$AllowFailure
    )

    $result = Invoke-SmokeRequest -Client $script:Client -Method $Method -Path $Path -Body $Body -Token $script:Token
    $script:Results.Add([pscustomobject]@{
        Group = $Group
        Endpoint = $Label
        Ok = $result.Ok
        StatusCode = $result.StatusCode
        Error = $result.Error
    })

    if (-not $result.Ok -and -not $AllowFailure) {
        throw "$Label failed with status $($result.StatusCode): $($result.Error)"
    }

    if ($ReturnId -and $result.Ok) {
        return Get-ResponseId -Content $result.Content
    }

    return $result
}

$baseUrl = Find-ApiBaseUrl
$script:Client = New-HttpClient -BaseUrl $baseUrl
$script:Results = New-Object System.Collections.Generic.List[object]

try {
    Write-Host "Base URL: $baseUrl"

    $login = Invoke-SmokeRequest -Client $script:Client -Method "POST" -Path "api/auth/login" -Body @{
        username = $Username
        password = $Password
    }

    $script:Results.Add([pscustomobject]@{
        Group = "AUTH"
        Endpoint = "POST /api/auth/login"
        Ok = $login.Ok
        StatusCode = $login.StatusCode
        Error = $login.Error
    })

    if (-not $login.Ok) {
        throw "Login failed. Cannot continue incidents smoke test."
    }

    $script:Token = ($login.Content | ConvertFrom-Json).token

    $me = Invoke-Step -Group "AUTH" -Label "GET /api/me" -Method "GET" -Path "api/me"
    $adminUserId = ($me.Content | ConvertFrom-Json).userId

    $incidentId = Invoke-Step -Group "INCIDENTS" -Label "POST /api/incidents" -Method "POST" -Path "api/incidents" -ReturnId -Body @{
        title = "Smoke incident"
        description = "Smoke incident description"
        type = 9
        severity = 2
        tripId = $null
        routeAssignmentId = $null
        vehicleId = $null
        driverId = $null
        transportAssistantId = $null
    }

    Invoke-Step -Group "INCIDENTS" -Label "GET /api/incidents" -Method "GET" -Path "api/incidents?PageNumber=1&PageSize=10" | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "GET /api/incidents/{id}" -Method "GET" -Path "api/incidents/$incidentId" | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "PUT /api/incidents/{id}/assign/{userId}" -Method "PUT" -Path "api/incidents/$incidentId/assign/$adminUserId" | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "PUT /api/incidents/{id}/in-progress" -Method "PUT" -Path "api/incidents/$incidentId/in-progress" | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "POST /api/incidents/{id}/comments" -Method "POST" -Path "api/incidents/$incidentId/comments" -Body @{
        comment = "Smoke follow-up comment"
    } | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "PUT /api/incidents/{id}/resolve" -Method "PUT" -Path "api/incidents/$incidentId/resolve" | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "PUT /api/incidents/{id}/close" -Method "PUT" -Path "api/incidents/$incidentId/close" | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "GET /api/incidents/by-status/{status}" -Method "GET" -Path "api/incidents/by-status/4" | Out-Null
    Invoke-Step -Group "INCIDENTS" -Label "GET /api/incidents/my-reported" -Method "GET" -Path "api/incidents/my-reported" | Out-Null
}
finally {
    $script:Client.Dispose()
}

$script:Results |
    Select-Object Group, Endpoint, @{ Name = "Result"; Expression = { if ($_.Ok) { "OK" } else { "FAIL" } } }, StatusCode, Error |
    Format-Table -AutoSize

$total = $script:Results.Count
$failed = @($script:Results | Where-Object { -not $_.Ok })
$successful = $total - $failed.Count

Write-Host ""
Write-Host "SUMMARY"
Write-Host "Total checks: $total"
Write-Host "Successful: $successful"
Write-Host "Failed: $($failed.Count)"

if ($failed.Count -gt 0) {
    Write-Host ""
    Write-Host "Failures:"
    $failed | Select-Object Group, Endpoint, StatusCode, Error | Format-List
    exit 1
}
