param(
    [string]$BaseUrl = "http://localhost:5075",
    [string]$AdminUsername = "admin",
    [string]$AdminPassword = "Admin123"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Net.Http

$handler = [System.Net.Http.HttpClientHandler]::new()
$client = [System.Net.Http.HttpClient]::new($handler)
$client.BaseAddress = [Uri]::new($BaseUrl.TrimEnd("/") + "/")
$client.Timeout = [TimeSpan]::FromSeconds(30)

function Invoke-Request {
    param(
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

    $response = $client.SendAsync($request).GetAwaiter().GetResult()
    $content = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
    return [pscustomobject]@{
        Ok = $response.IsSuccessStatusCode
        StatusCode = [int]$response.StatusCode
        Content = $content
    }
}

function Get-Token {
    param([string]$Username, [string]$Password)

    $response = Invoke-Request -Method "POST" -Path "api/v1/auth/login" -Body @{
        username = $Username
        password = $Password
    }
    if (-not $response.Ok) {
        throw "Login failed for $Username. Status $($response.StatusCode): $($response.Content)"
    }

    return ($response.Content | ConvertFrom-Json).token
}

function Assert-Status {
    param([string]$Check, [object]$Response, [int[]]$Expected)

    $ok = $Expected -contains $Response.StatusCode
    $script:Results.Add([pscustomobject]@{
        Check = $Check
        Result = if ($ok) { "OK" } else { "FAIL" }
        StatusCode = $Response.StatusCode
        Expected = $Expected -join ","
        Error = if ($ok) { $null } else { $Response.Content }
    })
}

$script:Results = New-Object System.Collections.Generic.List[object]

try {
    $adminToken = Get-Token -Username $AdminUsername -Password $AdminPassword
    $rolesResponse = Invoke-Request -Method "GET" -Path "api/v1/roles" -Token $adminToken
    Assert-Status -Check "Admin gets roles" -Response $rolesResponse -Expected @(200)

    $roles = $rolesResponse.Content | ConvertFrom-Json
    $supervisorRole = $roles | Where-Object { $_.name -eq "Supervisor" } | Select-Object -First 1
    if (-not $supervisorRole) {
        throw "Supervisor role was not found."
    }

    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $username = "supervisor$stamp"
    $password = "Supervisor123"
    $body = @{
        username = $username
        name = "Supervisor Smoke Test"
        email = "$username@trae.local"
        password = $password
        roleId = $supervisorRole.id
        profileImageUrl = $null
    }

    $create = Invoke-Request -Method "POST" -Path "api/v1/users" -Token $adminToken -Body $body
    Assert-Status -Check "Admin creates supervisor" -Response $create -Expected @(201)

    $supervisorToken = Get-Token -Username $username -Password $password
    $me = Invoke-Request -Method "GET" -Path "api/v1/me" -Token $supervisorToken
    Assert-Status -Check "Supervisor gets profile" -Response $me -Expected @(200)

    $duplicate = Invoke-Request -Method "POST" -Path "api/v1/users" -Token $adminToken -Body $body
    Assert-Status -Check "Duplicate username is rejected" -Response $duplicate -Expected @(400)

    $forbidden = Invoke-Request -Method "POST" -Path "api/v1/users" -Token $supervisorToken -Body @{
        username = "forbidden$stamp"
        name = "Forbidden User"
        email = "forbidden$stamp@trae.local"
        password = "Supervisor123"
        roleId = $supervisorRole.id
        profileImageUrl = $null
    }
    Assert-Status -Check "Supervisor cannot create users" -Response $forbidden -Expected @(403)

    $script:Results | Format-Table -AutoSize -Wrap
    $failed = ($script:Results | Where-Object { $_.Result -eq "FAIL" }).Count
    Write-Host ""
    Write-Host "SUMMARY"
    Write-Host "Total checks: $($script:Results.Count)"
    Write-Host "Successful: $($script:Results.Count - $failed)"
    Write-Host "Failed: $failed"

    if ($failed -gt 0) {
        exit 1
    }
}
finally {
    $client.Dispose()
}


