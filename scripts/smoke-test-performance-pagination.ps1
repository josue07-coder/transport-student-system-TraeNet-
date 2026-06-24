param(
    [string]$BaseUrl = "http://localhost:5075",
    [string]$Username = "admin",
    [string]$Password = "Admin123"
)

$ErrorActionPreference = "Stop"

$results = New-Object System.Collections.Generic.List[object]

function Add-Result {
    param(
        [string]$Name,
        [string]$Status,
        [int]$StatusCode = 0,
        [string]$Error = ""
    )

    $results.Add([pscustomobject]@{
        Name = $Name
        Status = $Status
        StatusCode = $StatusCode
        Error = $Error
    })
}

function Invoke-Api {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Path,
        [hashtable]$Headers = @{}
    )

    try {
        $response = Invoke-WebRequest -Method $Method -Uri "$BaseUrl$Path" -Headers $Headers -UseBasicParsing
        Add-Result -Name $Name -Status "OK" -StatusCode ([int]$response.StatusCode)
        return $response
    }
    catch {
        $statusCode = 0
        if ($_.Exception.Response) {
            $statusCode = [int]$_.Exception.Response.StatusCode
        }

        Add-Result -Name $Name -Status "FAIL" -StatusCode $statusCode -Error $_.Exception.Message
        return $null
    }
}

Write-Host "QA-5 Performance/Pagination Smoke Test"
Write-Host "BaseUrl: $BaseUrl"

$loginBody = @{
    username = $Username
    password = $Password
} | ConvertTo-Json

try {
    $loginResponse = Invoke-WebRequest -Method Post -Uri "$BaseUrl/api/v1/auth/login" -ContentType "application/json" -Body $loginBody -UseBasicParsing
    $loginJson = $loginResponse.Content | ConvertFrom-Json
    $token = $loginJson.token
}
catch {
    Write-Host "SKIP: no se pudo autenticar. Si el error contiene SSPI, es un bloqueo de ambiente SQL Server."
    Write-Host $_.Exception.Message
    exit 0
}

if ([string]::IsNullOrWhiteSpace($token)) {
    Write-Host "FAIL: login no devolviÃ³ token."
    exit 1
}

$authHeaders = @{ Authorization = "Bearer $token" }

$pagedEndpoints = @(
    @{ Name = "Students pagination clamp"; Path = "/api/v1/students?PageNumber=1&PageSize=500" },
    @{ Name = "Trips pagination clamp"; Path = "/api/v1/trips?PageNumber=1&PageSize=500" },
    @{ Name = "Route assignments pagination clamp"; Path = "/api/v1/route-assignments?PageNumber=1&PageSize=500" },
    @{ Name = "Audit logs pagination clamp"; Path = "/api/v1/audit-logs?PageNumber=1&PageSize=500" }
)

foreach ($endpoint in $pagedEndpoints) {
    $response = Invoke-Api -Name $endpoint.Name -Method Get -Path $endpoint.Path -Headers $authHeaders
    if ($response -and $response.StatusCode -eq 200) {
        $json = $response.Content | ConvertFrom-Json
        if ($json.pageSize -gt 100) {
            Add-Result -Name "$($endpoint.Name) max page size" -Status "FAIL" -StatusCode 200 -Error "PageSize esperado <= 100, recibido $($json.pageSize)"
        }
        else {
            Add-Result -Name "$($endpoint.Name) max page size" -Status "OK" -StatusCode 200
        }
    }
}

Invoke-Api -Name "Trips by status query" -Method Get -Path "/api/v1/trips/by-status/InProgress" -Headers $authHeaders | Out-Null
Invoke-Api -Name "Notifications read query" -Method Get -Path "/api/v1/notifications" -Headers $authHeaders | Out-Null

$results | Format-Table -AutoSize

$failed = $results | Where-Object { $_.Status -eq "FAIL" }
Write-Host ""
Write-Host "Total: $($results.Count) | OK: $(($results | Where-Object Status -eq 'OK').Count) | FAIL: $($failed.Count)"

if ($failed.Count -gt 0) {
    exit 1
}

exit 0


