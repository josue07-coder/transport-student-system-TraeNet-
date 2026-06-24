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
        [switch]$AllowFailure,
        [string]$Group = "TRACKING",
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
        $ok = $response.IsSuccessStatusCode
        $result = [pscustomobject]@{ Ok = $ok; StatusCode = [int]$response.StatusCode; Content = $content; Error = if ($ok) { $null } else { $content } }
    }
    catch {
        $result = [pscustomobject]@{ Ok = $false; StatusCode = 0; Content = $null; Error = $_.Exception.Message }
    }

    $script:Results.Add([pscustomobject]@{ Group = $Group; Endpoint = $Label; Ok = $result.Ok; StatusCode = $result.StatusCode; Error = $result.Error })
    if (-not $result.Ok -and -not $AllowFailure) { throw "$Label failed with status $($result.StatusCode): $($result.Error)" }
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
    $loginBody = @{ username = $Username; password = $Password }
    $login = Invoke-Request -Group "AUTH" -Label "POST /api/v1/auth/login" -Method "POST" -Path "api/v1/auth/login" -Body $loginBody
    $script:Token = ($login.Content | ConvertFrom-Json).token
    $adminToken = $script:Token

    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $today = [DateTime]::UtcNow.Date

    $sectorId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/sectors" -Method "POST" -Path "api/v1/sectors" -ReturnId -Body @{ name = "Track Sector $stamp"; province = "Smoke"; city = "Smoke" }
    $schoolId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/schools" -Method "POST" -Path "api/v1/schools" -ReturnId -Body @{ name = "Track School $stamp"; directorName = "Director"; email = "track.school.$stamp@smoke.local"; phone = "8095550001"; street = "Street"; city = "Smoke"; sectorId = $sectorId }
    $gradeId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/grades" -Method "POST" -Path "api/v1/grades" -ReturnId -Body @{ name = "Track Grade $stamp"; schoolId = $schoolId }
    $guardianId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/guardians" -Method "POST" -Path "api/v1/guardians" -ReturnId -Body @{ documentType = 1; documentNumber = "TG$stamp"; firstName = "Track"; lastName = "Guardian"; phone = "8095550002"; street = "Street"; city = "Smoke"; gender = 1; sectorId = $sectorId }
    $studentId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/students" -Method "POST" -Path "api/v1/students" -ReturnId -Body @{ firstName = "Track"; lastName = "Student"; schoolId = $schoolId; gradeId = $gradeId; guardianId = $guardianId; photoUrl = $null }
    $vehicleId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/vehicles" -Method "POST" -Path "api/v1/vehicles" -ReturnId -Body @{ plateNumber = "TRK$($stamp.ToString().Substring($stamp.ToString().Length - 6))"; model = "Bus"; brand = "Smoke"; capacity = 20; status = 1 }
    $driverId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/drivers" -Method "POST" -Path "api/v1/drivers" -ReturnId -Body @{ documentType = 1; documentNumber = "TD$stamp"; firstName = "Track"; lastName = "Driver"; licenseNumber = "TL$stamp"; phone = "8095550003"; street = "Street"; city = "Smoke"; email = "track.driver.$stamp@smoke.local"; photoUrl = $null }
    $assistantId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/transport-assistants" -Method "POST" -Path "api/v1/transport-assistants" -ReturnId -Body @{ documentType = 1; documentNumber = "TA$stamp"; firstName = "Track"; lastName = "Assistant"; phone = "8095550004"; street = "Street"; city = "Smoke"; email = "track.assistant.$stamp@smoke.local"; photoUrl = $null }
    $stopId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/stops" -Method "POST" -Path "api/v1/stops" -ReturnId -Body @{ name = "Track Stop $stamp"; street = "Street"; city = "Smoke"; latitude = 18.4861; longitude = -69.9312; sectorId = $sectorId }
    $routeId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/routes" -Method "POST" -Path "api/v1/routes" -ReturnId -Body @{ name = "Track Route $stamp"; schoolId = $schoolId; startTime = $today.AddHours(10).ToString("o"); endTime = $today.AddHours(11).ToString("o") }
    Invoke-Request -Group "SETUP" -Label "POST /api/v1/routes/{routeId}/stops" -Method "POST" -Path "api/v1/routes/$routeId/stops" -Body @{ routeId = $routeId; stopId = $stopId; stopOrder = 1 } | Out-Null
    Invoke-Request -Group "SETUP" -Label "PUT /api/v1/routes/{id}" -Method "PUT" -Path "api/v1/routes/$routeId" -Body @{ id = $routeId; name = "Track Route $stamp"; schoolId = $schoolId; startTime = $today.AddHours(10).ToString("o"); endTime = $today.AddHours(11).ToString("o"); status = 2 } | Out-Null
    $assignmentId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/route-assignments" -Method "POST" -Path "api/v1/route-assignments" -ReturnId -Body @{ routeId = $routeId; vehicleId = $vehicleId; driverId = $driverId; transportAssistantId = $assistantId; vehicleCapacity = 20 }
    Invoke-Request -Group "SETUP" -Label "POST /api/v1/route-assignments/{assignmentId}/students/{studentId}" -Method "POST" -Path "api/v1/route-assignments/$assignmentId/students/$studentId" | Out-Null
    $tripId = Invoke-Request -Group "SETUP" -Label "POST /api/v1/trips/start" -Method "POST" -Path "api/v1/trips/start" -ReturnId -Body @{ routeAssignmentId = $assignmentId }

    Invoke-Request -Label "POST /api/v1/tracking/location" -Method "POST" -Path "api/v1/tracking/location" -Body @{ tripId = $tripId; latitude = 18.4861; longitude = -69.9312; speed = 35; heading = 90 } | Out-Null
    Invoke-Request -Label "GET /api/v1/tracking/trips/{tripId}/current-location" -Method "GET" -Path "api/v1/tracking/trips/$tripId/current-location" | Out-Null
    Invoke-Request -Label "GET /api/v1/tracking/trips/{tripId}/history" -Method "GET" -Path "api/v1/tracking/trips/$tripId/history" | Out-Null
    Invoke-Request -Label "GET /api/v1/tracking/active-trips" -Method "GET" -Path "api/v1/tracking/active-trips" | Out-Null

    $guardianLogin = Invoke-Request -Group "AUTH" -Label "POST /api/v1/auth/login (guardian)" -Method "POST" -Path "api/v1/auth/login" -Body @{ username = "TG$stamp"; password = "TG$stamp" }
    $script:Token = ($guardianLogin.Content | ConvertFrom-Json).token
    Invoke-Request -Label "GET /api/v1/tracking/my-students (guardian)" -Method "GET" -Path "api/v1/tracking/my-students" | Out-Null

    $script:Token = $adminToken
    Invoke-Request -Group "CLEANUP" -Label "PUT /api/v1/trips/{id}/end" -Method "PUT" -Path "api/v1/trips/$tripId/end" -AllowFailure | Out-Null
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


