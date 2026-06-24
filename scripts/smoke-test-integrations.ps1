param(
    [string]$Username = "admin",
    [string]$Password = "Admin123"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

function New-Client([string]$BaseUrl) {
    $handler = [System.Net.Http.HttpClientHandler]::new()
    if ($BaseUrl -like "https://*") { $handler.ServerCertificateCustomValidationCallback = { $true } }
    $client = [System.Net.Http.HttpClient]::new($handler)
    $client.BaseAddress = [Uri]::new($BaseUrl.TrimEnd("/") + "/")
    $client.Timeout = [TimeSpan]::FromSeconds(30)
    return $client
}

function Get-BaseUrl {
    $urls = @("http://localhost:5075", "http://localhost:5000", "https://localhost:7295", "https://localhost:5001")
    foreach ($url in $urls) {
        $client = New-Client $url
        try {
            $response = $client.GetAsync("swagger/index.html").GetAwaiter().GetResult()
            if ([int]$response.StatusCode -gt 0) { return $url }
        }
        catch { }
        finally { $client.Dispose() }
    }
    throw "No API instance detected."
}

function Invoke-Request {
    param(
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [int[]]$ExpectedStatusCodes = @(200, 204),
        [string]$Group = "INTEGRATIONS",
        [string]$Label = $Path
    )

    $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($Method), $Path.TrimStart("/"))
    if ($script:Token) {
        $request.Headers.Authorization = [System.Net.Http.Headers.AuthenticationHeaderValue]::new("Bearer", $script:Token)
    }
    if ($null -ne $Body) {
        $json = $Body | ConvertTo-Json -Depth 10
        $request.Content = [System.Net.Http.StringContent]::new($json, [System.Text.Encoding]::UTF8, "application/json")
    }

    try {
        $response = $script:Client.SendAsync($request).GetAwaiter().GetResult()
        $content = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        $statusCode = [int]$response.StatusCode
        $ok = $ExpectedStatusCodes -contains $statusCode
        $result = [pscustomobject]@{ Ok = $ok; StatusCode = $statusCode; Content = $content; Error = if ($ok) { $null } else { $content } }
    }
    catch {
        $result = [pscustomobject]@{ Ok = $false; StatusCode = 0; Content = $null; Error = $_.Exception.Message }
    }

    $script:Results.Add([pscustomobject]@{ Group = $Group; Endpoint = $Label; Ok = $result.Ok; StatusCode = $result.StatusCode; Error = $result.Error })
    if (-not $result.Ok) { throw "$Label failed with status $($result.StatusCode): $($result.Error)" }
    return $result
}

$baseUrl = Get-BaseUrl
$script:Client = New-Client $baseUrl
$script:Results = New-Object System.Collections.Generic.List[object]

try {
    Write-Host "Base URL: $baseUrl"
    $login = Invoke-Request -Group "AUTH" -Label "POST /api/v1/auth/login" -Method "POST" -Path "api/v1/auth/login" -Body @{ username = $Username; password = $Password }
    $script:Token = ($login.Content | ConvertFrom-Json).token
    $adminToken = $script:Token

    $me = Invoke-Request -Group "AUTH" -Label "GET /api/v1/me" -Method "GET" -Path "api/v1/me"
    $adminUserId = ($me.Content | ConvertFrom-Json).userId

    Invoke-Request -Label "POST /api/v1/integrations/test-email" -Method "POST" -Path "api/v1/integrations/test-email" -Body @{
        to = "smoke.integrations@test.local"
        subject = "Smoke integration email"
        body = "Mock email body"
    } | Out-Null

    Invoke-Request -Label "POST /api/v1/integrations/test-sms" -Method "POST" -Path "api/v1/integrations/test-sms" -Body @{
        phoneNumber = "8095553001"
        message = "Mock SMS message"
    } | Out-Null

    Invoke-Request -Label "POST /api/v1/integrations/test-whatsapp" -Method "POST" -Path "api/v1/integrations/test-whatsapp" -Body @{
        phoneNumber = "8095553001"
        message = "Mock WhatsApp message"
    } | Out-Null

    Invoke-Request -Label "POST /api/v1/integrations/test-push" -Method "POST" -Path "api/v1/integrations/test-push" -Body @{
        userId = $adminUserId
        title = "Smoke push"
        message = "Mock push message"
    } | Out-Null

    Invoke-Request -Label "GET /api/v1/integrations/test-distance" -Method "GET" -Path "api/v1/integrations/test-distance?originLat=18.4861&originLng=-69.9312&destinationLat=18.5001&destinationLng=-69.9002" | Out-Null

    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $guardianDocument = "ING$stamp"
    Invoke-Request -Group "SETUP" -Label "POST /api/v1/guardians" -Method "POST" -Path "api/v1/guardians" -Body @{
        documentType = 1
        documentNumber = $guardianDocument
        firstName = "Integration"
        lastName = "Guardian"
        phone = "8095553002"
        street = "Street"
        city = "Smoke"
        gender = 1
        sectorId = $null
    } | Out-Null

    $guardianLogin = Invoke-Request -Group "AUTH" -Label "POST /api/v1/auth/login (guardian)" -Method "POST" -Path "api/v1/auth/login" -Body @{ username = $guardianDocument; password = $guardianDocument }
    $script:Token = ($guardianLogin.Content | ConvertFrom-Json).token

    Invoke-Request -Group "AUTHZ" -Label "POST /api/v1/integrations/test-email unauthorized" -Method "POST" -Path "api/v1/integrations/test-email" -ExpectedStatusCodes @(403) -Body @{
        to = "blocked@test.local"
        subject = "Blocked"
        body = "Blocked"
    } | Out-Null

    $script:Token = $adminToken
}
finally {
    $script:Client.Dispose()
}

$script:Results | Select-Object Group, Endpoint, @{ Name = "Result"; Expression = { if ($_.Ok) { "OK" } else { "FAIL" } } }, StatusCode, Error | Format-Table -AutoSize
$failed = @($script:Results | Where-Object { -not $_.Ok })
Write-Host ""
Write-Host "SUMMARY"
Write-Host "Total checks: $($script:Results.Count)"
Write-Host "Successful: $($script:Results.Count - $failed.Count)"
Write-Host "Failed: $($failed.Count)"
if ($failed.Count -gt 0) {
    Write-Host ""
    Write-Host "Failures:"
    $failed | Select-Object Group, Endpoint, StatusCode, Error | Format-List
    exit 1
}


