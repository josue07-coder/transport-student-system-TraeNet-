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
        [switch]$ReturnId,
        [int[]]$ExpectedStatusCodes = @(200, 204),
        [string]$Group = "REPORTS",
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

    if ($ReturnId -and $result.Ok) {
        $json = $result.Content | ConvertFrom-Json
        $parsedGuid = [Guid]::Empty
        if ($json -is [string] -and [Guid]::TryParse($json, [ref]$parsedGuid)) { return $json }
        if ($json.id) { return [string]$json.id }
        if ($json.Id) { return [string]$json.Id }
        throw "Response did not include Id. Content: $($result.Content)"
    }

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

    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $today = [DateTime]::UtcNow.Date
    $startDate = $today.AddDays(-7).ToString("yyyy-MM-dd")
    $endDate = $today.AddDays(1).ToString("yyyy-MM-dd")

    $sectorId = Invoke-Request -Group "SETUP" -Label "POST /api/sectors" -Method "POST" -Path "api/sectors" -ReturnId -Body @{ name = "Reports Sector $stamp"; province = "Smoke"; city = "Smoke" }
    $schoolId = Invoke-Request -Group "SETUP" -Label "POST /api/schools" -Method "POST" -Path "api/schools" -ReturnId -Body @{ name = "Reports School $stamp"; directorName = "Director"; email = "reports.school.$stamp@smoke.local"; phone = "8095551001"; street = "Street"; city = "Smoke"; sectorId = $sectorId }
    $gradeId = Invoke-Request -Group "SETUP" -Label "POST /api/grades" -Method "POST" -Path "api/grades" -ReturnId -Body @{ name = "Reports Grade $stamp"; schoolId = $schoolId }
    $guardianDocument = "RG$stamp"
    $guardianId = Invoke-Request -Group "SETUP" -Label "POST /api/guardians" -Method "POST" -Path "api/guardians" -ReturnId -Body @{ documentType = 1; documentNumber = $guardianDocument; firstName = "Reports"; lastName = "Guardian"; phone = "8095551002"; street = "Street"; city = "Smoke"; gender = 1; sectorId = $sectorId }
    $studentId = Invoke-Request -Group "SETUP" -Label "POST /api/students" -Method "POST" -Path "api/students" -ReturnId -Body @{ firstName = "Reports"; lastName = "Student"; schoolId = $schoolId; gradeId = $gradeId; guardianId = $guardianId; photoUrl = $null }
    $vehicleId = Invoke-Request -Group "SETUP" -Label "POST /api/vehicles" -Method "POST" -Path "api/vehicles" -ReturnId -Body @{ plateNumber = "RPT$($stamp.ToString().Substring($stamp.ToString().Length - 6))"; model = "Bus"; brand = "Smoke"; capacity = 20; status = 1 }
    $driverId = Invoke-Request -Group "SETUP" -Label "POST /api/drivers" -Method "POST" -Path "api/drivers" -ReturnId -Body @{ documentType = 1; documentNumber = "RD$stamp"; firstName = "Reports"; lastName = "Driver"; licenseNumber = "RL$stamp"; phone = "8095551003"; street = "Street"; city = "Smoke"; email = "reports.driver.$stamp@smoke.local"; photoUrl = $null }
    $assistantId = Invoke-Request -Group "SETUP" -Label "POST /api/transport-assistants" -Method "POST" -Path "api/transport-assistants" -ReturnId -Body @{ documentType = 1; documentNumber = "RA$stamp"; firstName = "Reports"; lastName = "Assistant"; phone = "8095551004"; street = "Street"; city = "Smoke"; email = "reports.assistant.$stamp@smoke.local"; photoUrl = $null }
    $stopId = Invoke-Request -Group "SETUP" -Label "POST /api/stops" -Method "POST" -Path "api/stops" -ReturnId -Body @{ name = "Reports Stop $stamp"; street = "Street"; city = "Smoke"; latitude = 18.4861; longitude = -69.9312; sectorId = $sectorId }
    $routeId = Invoke-Request -Group "SETUP" -Label "POST /api/routes" -Method "POST" -Path "api/routes" -ReturnId -Body @{ name = "Reports Route $stamp"; schoolId = $schoolId; startTime = $today.AddHours(12).ToString("o"); endTime = $today.AddHours(13).ToString("o") }
    Invoke-Request -Group "SETUP" -Label "POST /api/routes/{routeId}/stops" -Method "POST" -Path "api/routes/$routeId/stops" -Body @{ routeId = $routeId; stopId = $stopId; stopOrder = 1 } | Out-Null
    Invoke-Request -Group "SETUP" -Label "PUT /api/routes/{id}" -Method "PUT" -Path "api/routes/$routeId" -Body @{ id = $routeId; name = "Reports Route $stamp"; schoolId = $schoolId; startTime = $today.AddHours(12).ToString("o"); endTime = $today.AddHours(13).ToString("o"); status = 2 } | Out-Null
    $assignmentId = Invoke-Request -Group "SETUP" -Label "POST /api/route-assignments" -Method "POST" -Path "api/route-assignments" -ReturnId -Body @{ routeId = $routeId; vehicleId = $vehicleId; driverId = $driverId; transportAssistantId = $assistantId; vehicleCapacity = 20 }
    Invoke-Request -Group "SETUP" -Label "POST /api/route-assignments/{assignmentId}/students/{studentId}" -Method "POST" -Path "api/route-assignments/$assignmentId/students/$studentId" | Out-Null
    $tripId = Invoke-Request -Group "SETUP" -Label "POST /api/trips/start" -Method "POST" -Path "api/trips/start" -ReturnId -Body @{ routeAssignmentId = $assignmentId }

    Invoke-Request -Label "GET /api/reports/dashboard" -Method "GET" -Path "api/reports/dashboard" | Out-Null
    Invoke-Request -Label "GET /api/reports/trips" -Method "GET" -Path "api/reports/trips?startDate=$startDate&endDate=$endDate" | Out-Null
    Invoke-Request -Label "GET /api/reports/students-by-route/{routeId}" -Method "GET" -Path "api/reports/students-by-route/$routeId" | Out-Null
    Invoke-Request -Label "GET /api/reports/incidents" -Method "GET" -Path "api/reports/incidents?startDate=$startDate&endDate=$endDate" | Out-Null
    Invoke-Request -Label "GET /api/reports/drivers-performance" -Method "GET" -Path "api/reports/drivers-performance?startDate=$startDate&endDate=$endDate" | Out-Null
    Invoke-Request -Label "GET /api/reports/vehicles-usage" -Method "GET" -Path "api/reports/vehicles-usage?startDate=$startDate&endDate=$endDate" | Out-Null
    Invoke-Request -Label "GET /api/reports/audit-summary" -Method "GET" -Path "api/reports/audit-summary?startDate=$startDate&endDate=$endDate" | Out-Null

    $guardianLogin = Invoke-Request -Group "AUTH" -Label "POST /api/auth/login (guardian)" -Method "POST" -Path "api/auth/login" -Body @{ username = $guardianDocument; password = $guardianDocument }
    $script:Token = ($guardianLogin.Content | ConvertFrom-Json).token
    Invoke-Request -Group "AUTHZ" -Label "GET /api/reports/dashboard unauthorized" -Method "GET" -Path "api/reports/dashboard" -ExpectedStatusCodes @(403) | Out-Null

    $script:Token = $adminToken
    Invoke-Request -Group "CLEANUP" -Label "PUT /api/trips/{id}/end" -Method "PUT" -Path "api/trips/$tripId/end" | Out-Null
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
