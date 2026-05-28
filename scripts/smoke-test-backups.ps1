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
        [string]$Group = "BACKUPS",
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
    $login = Invoke-Request -Group "AUTH" -Label "POST /api/auth/login" -Method "POST" -Path "api/auth/login" -Body @{ username = $Username; password = $Password }
    $script:Token = ($login.Content | ConvertFrom-Json).token
    $adminToken = $script:Token

    $backup = Invoke-Request -Label "POST /api/backups/manual" -Method "POST" -Path "api/backups/manual"
    $backupRecord = $backup.Content | ConvertFrom-Json

    Invoke-Request -Label "GET /api/backups" -Method "GET" -Path "api/backups?PageNumber=1&PageSize=10" | Out-Null
    Invoke-Request -Label "GET /api/backups/latest" -Method "GET" -Path "api/backups/latest" | Out-Null
    Invoke-Request -Label "GET /api/backups/{id}" -Method "GET" -Path "api/backups/$($backupRecord.id)" | Out-Null
    Invoke-Request -Label "POST /api/backups/{id}/restore placeholder" -Method "POST" -Path "api/backups/$($backupRecord.id)/restore" -ExpectedStatusCodes @(400) | Out-Null

    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $guardianDocument = "BKG$stamp"
    Invoke-Request -Group "SETUP" -Label "POST /api/guardians" -Method "POST" -Path "api/guardians" -Body @{
        documentType = 1
        documentNumber = $guardianDocument
        firstName = "Backup"
        lastName = "Guardian"
        phone = "8095554001"
        street = "Street"
        city = "Smoke"
        gender = 1
        sectorId = $null
    } | Out-Null

    $guardianLogin = Invoke-Request -Group "AUTH" -Label "POST /api/auth/login (guardian)" -Method "POST" -Path "api/auth/login" -Body @{ username = $guardianDocument; password = $guardianDocument }
    $script:Token = ($guardianLogin.Content | ConvertFrom-Json).token
    Invoke-Request -Group "AUTHZ" -Label "POST /api/backups/manual unauthorized" -Method "POST" -Path "api/backups/manual" -ExpectedStatusCodes @(403) | Out-Null

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
