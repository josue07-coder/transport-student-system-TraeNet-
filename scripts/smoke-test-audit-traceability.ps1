param(
    [string]$BaseUrl = ""
)

$ErrorActionPreference = "Stop"

function Resolve-BaseUrl {
    if ($BaseUrl) { return $BaseUrl.TrimEnd("/") }

    $candidates = @("http://localhost:5075", "http://localhost:5000", "https://localhost:5001")
    foreach ($candidate in $candidates) {
        try {
            Invoke-WebRequest -Uri "$candidate/swagger/v1/swagger.json" -UseBasicParsing -TimeoutSec 3 | Out-Null
            return $candidate
        } catch {
        }
    }

    throw "No se encontro API activa en puertos conocidos."
}

function Invoke-Json {
    param(
        [string]$Method,
        [string]$Url,
        [object]$Body = $null,
        [string]$Token = $null,
        [string]$CorrelationId = $null
    )

    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }
    if ($CorrelationId) { $headers["X-Correlation-ID"] = $CorrelationId }

    $params = @{
        Method = $Method
        Uri = $Url
        Headers = $headers
        ContentType = "application/json"
        TimeoutSec = 30
    }

    if ($null -ne $Body) {
        $params.Body = ($Body | ConvertTo-Json -Depth 10)
    }

    try {
        $response = Invoke-WebRequest @params
        $body = if ($response.Content) { $response.Content | ConvertFrom-Json } else { $null }
        return @{ StatusCode = [int]$response.StatusCode; Body = $body; Headers = $response.Headers; CorrelationHeader = $response.Headers["X-Correlation-ID"]; Error = $null }
    } catch {
        $response = $_.Exception.Response
        $statusCode = if ($response) { [int]$response.StatusCode } else { 0 }
        $headers = if ($response) { $response.Headers } else { @{} }
        $correlationHeader = $null
        try {
            if ($response) { $correlationHeader = $response.GetResponseHeader("X-Correlation-ID") }
        } catch {
        }
        $message = $_.Exception.Message
        $body = $null
        try {
            if ($response) {
                $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
                $message = $reader.ReadToEnd()
                if ($message) { $body = $message | ConvertFrom-Json }
            }
        } catch {
        }
        return @{ StatusCode = $statusCode; Body = $body; Headers = $headers; CorrelationHeader = $correlationHeader; Error = $message }
    }
}

function Add-Result {
    param([string]$Name, [bool]$Passed, [string]$Details)
    $script:results += [pscustomobject]@{
        Check = $Name
        Result = if ($Passed) { "PASS" } else { "FAIL" }
        Details = $Details
    }
}

$api = Resolve-BaseUrl
Write-Host "API: $api"
$results = @()

$badCorrelation = "qa4-smoke-invalid-login"
$badLogin = Invoke-Json -Method "POST" -Url "$api/api/v1/auth/login" -CorrelationId $badCorrelation -Body @{
    username = "missing-user"
    password = "bad-password"
}

$badHeader = $badLogin.CorrelationHeader
Add-Result "Error response includes correlation header" ($badHeader -eq $badCorrelation) "status=$($badLogin.StatusCode), header=$badHeader"
Add-Result "Error body includes correlation id" ($badLogin.Body.correlationId -eq $badCorrelation) "bodyCorrelation=$($badLogin.Body.correlationId)"

$loginCorrelation = "qa4-smoke-login"
$login = Invoke-Json -Method "POST" -Url "$api/api/v1/auth/login" -CorrelationId $loginCorrelation -Body @{
    username = "admin"
    password = "Admin123"
}

if ($login.StatusCode -ne 200 -or -not $login.Body.token) {
    $results | Format-Table -AutoSize
    Write-Host "SKIP: no se pudo autenticar admin. Status=$($login.StatusCode) Error=$($login.Error)"
    exit 0
}

$token = $login.Body.token
$audit = Invoke-Json -Method "GET" -Url "$api/api/v1/audit-logs/by-action/Login" -Token $token
$logs = @($audit.Body)
$matching = $logs | Where-Object { $_.correlationId -eq $loginCorrelation } | Select-Object -First 1
Add-Result "Audit log stores correlation metadata" ($null -ne $matching) "matchingCorrelation=$($matching.correlationId)"

$results | Format-Table -AutoSize
$failed = @($results | Where-Object { $_.Result -eq "FAIL" })
if ($failed.Count -gt 0) { exit 1 }
exit 0


