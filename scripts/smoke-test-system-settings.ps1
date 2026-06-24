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
        [string]$Group = "SETTINGS",
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

    $all = Invoke-Request -Label "GET /api/v1/system-settings" -Method "GET" -Path "api/v1/system-settings"
    $settings = @($all.Content | ConvertFrom-Json)
    $baseSetting = $settings | Where-Object { $_.isEditable -eq $false } | Select-Object -First 1

    Invoke-Request -Label "GET /api/v1/system-settings/by-category/General" -Method "GET" -Path "api/v1/system-settings/by-category/General" | Out-Null
    Invoke-Request -Label "GET /api/v1/system-settings/key/General.SystemName" -Method "GET" -Path "api/v1/system-settings/key/General.SystemName" | Out-Null

    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $custom = Invoke-Request -Label "POST /api/v1/system-settings" -Method "POST" -Path "api/v1/system-settings" -Body @{
        key = "Smoke.Custom.$stamp"
        value = "initial"
        description = "Smoke custom setting"
        category = "Smoke"
        dataType = "String"
        isEditable = $true
    }
    $customSetting = $custom.Content | ConvertFrom-Json

    Invoke-Request -Label "PUT /api/v1/system-settings/{id}" -Method "PUT" -Path "api/v1/system-settings/$($customSetting.id)" -Body @{
        value = "updated"
        description = "Updated smoke custom setting"
        category = "Smoke"
        dataType = "String"
        isEditable = $true
    } | Out-Null

    Invoke-Request -Label "DELETE /api/v1/system-settings/{id}" -Method "DELETE" -Path "api/v1/system-settings/$($customSetting.id)" | Out-Null

    $nonEditable = Invoke-Request -Label "POST /api/v1/system-settings non-editable" -Method "POST" -Path "api/v1/system-settings" -Body @{
        key = "Smoke.Locked.$stamp"
        value = "locked"
        description = "Smoke non-editable setting"
        category = "Smoke"
        dataType = "String"
        isEditable = $false
    }
    $nonEditableSetting = $nonEditable.Content | ConvertFrom-Json

    Invoke-Request -Label "PUT /api/v1/system-settings/{id} non-editable" -Method "PUT" -Path "api/v1/system-settings/$($nonEditableSetting.id)" -ExpectedStatusCodes @(400) -Body @{
        value = "blocked"
        description = "Blocked update"
        category = "Smoke"
        dataType = "String"
        isEditable = $false
    } | Out-Null

    $guardianDocument = "SSG$stamp"
    Invoke-Request -Group "SETUP" -Label "POST /api/v1/sectors" -Method "POST" -Path "api/v1/sectors" -Body @{ name = "Settings Sector $stamp"; province = "Smoke"; city = "Smoke" } | Out-Null
    $guardian = Invoke-Request -Group "SETUP" -Label "POST /api/v1/guardians" -Method "POST" -Path "api/v1/guardians" -Body @{ documentType = 1; documentNumber = $guardianDocument; firstName = "Settings"; lastName = "Guardian"; phone = "8095552002"; street = "Street"; city = "Smoke"; gender = 1; sectorId = $null }

    $guardianLogin = Invoke-Request -Group "AUTH" -Label "POST /api/v1/auth/login (guardian)" -Method "POST" -Path "api/v1/auth/login" -Body @{ username = $guardianDocument; password = $guardianDocument }
    $script:Token = ($guardianLogin.Content | ConvertFrom-Json).token
    Invoke-Request -Group "AUTHZ" -Label "POST /api/v1/system-settings unauthorized" -Method "POST" -Path "api/v1/system-settings" -ExpectedStatusCodes @(403) -Body @{
        key = "Smoke.Forbidden.$stamp"
        value = "no"
        description = "Forbidden"
        category = "Smoke"
        dataType = "String"
        isEditable = $true
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


