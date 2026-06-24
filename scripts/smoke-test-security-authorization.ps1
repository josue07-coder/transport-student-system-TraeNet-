param(
    [string]$BaseUrl
)

$ErrorActionPreference = "Stop"

function Resolve-BaseUrl {
    param([string]$PreferredBaseUrl)

    if ($PreferredBaseUrl) {
        return $PreferredBaseUrl.TrimEnd("/")
    }

    foreach ($candidate in @("http://localhost:5000", "https://localhost:5001", "http://localhost:5075")) {
        try {
            $response = Invoke-WebRequest -Uri "$candidate/swagger/v1/swagger.json" -Method Get -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                return $candidate
            }
        }
        catch {
        }
    }

    throw "No se encontro API activa."
}

function Invoke-Json {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        [object]$Body
    )

    $params = @{
        Method = $Method
        Uri = $Url
        Headers = $Headers
        TimeoutSec = 30
    }

    if ($null -ne $Body) {
        $params.ContentType = "application/json"
        $params.Body = ($Body | ConvertTo-Json -Depth 10)
    }

    Invoke-RestMethod @params
}

function Invoke-Status {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Headers,
        [object]$Body
    )

    try {
        Invoke-Json -Method $Method -Url $Url -Headers $Headers -Body $Body | Out-Null
        return 200
    }
    catch {
        if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
            return [int]$_.Exception.Response.StatusCode
        }

        throw
    }
}

function Login {
    param(
        [string]$Username,
        [string]$Password
    )

    $login = Invoke-Json -Method Post -Url "$base/api/v1/auth/login" -Headers @{} -Body @{
        username = $Username
        password = $Password
    }

    if (-not $login.token) {
        throw "Login no devolvio token para $Username."
    }

    return @{ Authorization = "Bearer $($login.token)" }
}

function Assert-Status {
    param(
        [string]$Name,
        [int]$Actual,
        [int[]]$Expected
    )

    if ($Expected -contains $Actual) {
        Write-Host "[PASS] $Name - Status $Actual"
        $script:Passed++
        return
    }

    Write-Host "[FAIL] $Name - Status $Actual, esperado: $($Expected -join ',')"
    $script:Failed++
}

$base = Resolve-BaseUrl $BaseUrl
Write-Host "API: $base"

$Passed = 0
$Failed = 0

$admin = Login "admin" "Admin123"
$supervisor = Login "supervisor" "Supervisor123"
$driver = Login "driver01" "Driver123"
$assistant = Login "assistant01" "Assistant123"
$guardian = Login "guardian01" "Guardian123"

foreach ($role in @(
    @{ Name = "Driver"; Headers = $driver },
    @{ Name = "Assistant"; Headers = $assistant },
    @{ Name = "Guardian"; Headers = $guardian }
)) {
    Assert-Status "$($role.Name) no puede GET /api/v1/students" (Invoke-Status Get "$base/api/v1/students" $role.Headers $null) @(403)
    Assert-Status "$($role.Name) no puede GET /api/v1/routes" (Invoke-Status Get "$base/api/v1/routes" $role.Headers $null) @(403)
    Assert-Status "$($role.Name) no puede GET /api/v1/users" (Invoke-Status Get "$base/api/v1/users" $role.Headers $null) @(403)
    Assert-Status "$($role.Name) no puede GET /api/v1/audit-logs" (Invoke-Status Get "$base/api/v1/audit-logs" $role.Headers $null) @(403)
    Assert-Status "$($role.Name) no puede GET /api/v1/system-settings" (Invoke-Status Get "$base/api/v1/system-settings" $role.Headers $null) @(403)
    Assert-Status "$($role.Name) no puede GET /api/v1/backups" (Invoke-Status Get "$base/api/v1/backups" $role.Headers $null) @(403)
}

Assert-Status "Assistant no puede cancelar viaje" `
    (Invoke-Status Put "$base/api/v1/trips/00000000-0000-0000-0000-000000000001/cancel" $assistant @{ reason = "No permitido" }) @(403)

Assert-Status "Guardian no puede modificar asistencia" `
    (Invoke-Status Put "$base/api/v1/trips/00000000-0000-0000-0000-000000000001/students/00000000-0000-0000-0000-000000000002/boarded" $guardian $null) @(403)

Assert-Status "Driver puede GET /api/v1/me/trips" (Invoke-Status Get "$base/api/v1/me/trips" $driver $null) @(200)
Assert-Status "Assistant puede GET /api/v1/me/trips" (Invoke-Status Get "$base/api/v1/me/trips" $assistant $null) @(200)
Assert-Status "Guardian puede GET /api/v1/me/students" (Invoke-Status Get "$base/api/v1/me/students" $guardian $null) @(200)
Assert-Status "Guardian puede GET /api/v1/tracking/my-students" (Invoke-Status Get "$base/api/v1/tracking/my-students" $guardian $null) @(200,400)

Assert-Status "Admin puede GET /api/v1/students" (Invoke-Status Get "$base/api/v1/students" $admin $null) @(200)
Assert-Status "Supervisor puede GET /api/v1/routes" (Invoke-Status Get "$base/api/v1/routes" $supervisor $null) @(200)
Assert-Status "Supervisor puede GET /api/v1/reports/dashboard" (Invoke-Status Get "$base/api/v1/reports/dashboard" $supervisor $null) @(200)
Assert-Status "Supervisor no puede GET /api/v1/backups" (Invoke-Status Get "$base/api/v1/backups" $supervisor $null) @(403)

Write-Host "Resumen: PASS=$Passed FAIL=$Failed"

if ($Failed -gt 0) {
    exit 1
}


