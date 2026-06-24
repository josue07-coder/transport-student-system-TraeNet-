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

function Invoke-Request {
    param(
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [string]$Token = $script:AdminToken
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
        $response = $script:Client.SendAsync($request).GetAwaiter().GetResult()
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
            $script:Client = $client
            $result = Invoke-Request -Method "GET" -Path "swagger/index.html" -Token $null
            if ($result.StatusCode -gt 0) {
                return $url
            }
        }
        finally {
            $client.Dispose()
            $script:Client = $null
        }
    }

    throw "No API instance detected on launchSettings URLs or common localhost ports."
}

function Get-Token {
    param([string]$User, [string]$Pass)

    $login = Invoke-Request -Method "POST" -Path "api/v1/auth/login" -Token $null -Body @{
        username = $User
        password = $Pass
    }

    if (-not $login.Ok) {
        throw "Login failed for $User. Status $($login.StatusCode): $($login.Error)"
    }

    return ($login.Content | ConvertFrom-Json).token
}

function Get-ResponseId {
    param([string]$Content)

    $json = $Content | ConvertFrom-Json
    if ($json.Id) { return [string]$json.Id }
    if ($json.id) { return [string]$json.id }
    if ($json -is [string]) { return $json }
    throw "Response did not include Id. Content: $Content"
}

function Invoke-Create {
    param([string]$Path, [object]$Body)

    $result = Invoke-Request -Method "POST" -Path $Path -Body $Body
    if (-not $result.Ok) {
        throw "Create $Path failed. Status $($result.StatusCode): $($result.Error)"
    }

    return Get-ResponseId -Content $result.Content
}

function Add-Result {
    param(
        [string]$Actor,
        [string]$Check,
        [object]$Result,
        [bool]$ExpectedSuccess
    )

    $ok = if ($ExpectedSuccess) { $Result.Ok } else { -not $Result.Ok }
    $script:Results.Add([pscustomobject]@{
        Actor = $Actor
        Check = $Check
        Result = if ($ok) { "OK" } else { "FAIL" }
        StatusCode = $Result.StatusCode
        Error = if ($ok) { $null } else { $Result.Error }
    })
}

$baseUrl = Find-ApiBaseUrl
$script:Client = New-HttpClient -BaseUrl $baseUrl
$script:Results = New-Object System.Collections.Generic.List[object]

try {
    Write-Host "Base URL: $baseUrl"

    $script:AdminToken = Get-Token -User $Username -Pass $Password
    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $today = [DateTime]::UtcNow.Date

    $sectorId = Invoke-Create -Path "api/v1/sectors" -Body @{
        name = "Security Sector $stamp"
        province = "Security Province"
        city = "Security City"
    }

    $schoolId = Invoke-Create -Path "api/v1/schools" -Body @{
        name = "Security School $stamp"
        directorName = "Security Director"
        email = "security.school.$stamp@test.local"
        phone = "8095551001"
        street = "Security Street"
        city = "Security City"
        sectorId = $sectorId
    }

    $gradeId = Invoke-Create -Path "api/v1/grades" -Body @{
        name = "Security Grade $stamp"
        schoolId = $schoolId
    }

    $guardianDocument = "SG$stamp"
    $otherGuardianDocument = "SGX$stamp"

    $guardianId = Invoke-Create -Path "api/v1/guardians" -Body @{
        documentType = 1
        documentNumber = $guardianDocument
        firstName = "Security"
        lastName = "Guardian"
        phone = "8095551002"
        street = "Guardian Street"
        city = "Security City"
        gender = 1
        sectorId = $sectorId
    }

    $otherGuardianId = Invoke-Create -Path "api/v1/guardians" -Body @{
        documentType = 1
        documentNumber = $otherGuardianDocument
        firstName = "Other"
        lastName = "Guardian"
        phone = "8095551007"
        street = "Other Guardian Street"
        city = "Security City"
        gender = 1
        sectorId = $sectorId
    }

    $studentId = Invoke-Create -Path "api/v1/students" -Body @{
        firstName = "Security"
        lastName = "Student"
        schoolId = $schoolId
        gradeId = $gradeId
        guardianId = $guardianId
    }

    $otherStudentId = Invoke-Create -Path "api/v1/students" -Body @{
        firstName = "Other"
        lastName = "Student"
        schoolId = $schoolId
        gradeId = $gradeId
        guardianId = $otherGuardianId
    }

    $vehicleId = Invoke-Create -Path "api/v1/vehicles" -Body @{
        plateNumber = "SEC$($stamp % 1000000)"
        capacity = 20
    }

    $driverDocument = "SD$stamp"
    $driverId = Invoke-Create -Path "api/v1/drivers" -Body @{
        firstName = "Security"
        lastName = "Driver"
        documentType = 1
        documentNumber = $driverDocument
        licenseNumber = "SECLIC$stamp"
        phone = "8095551003"
        street = "Driver Street"
        city = "Security City"
        email = "security.driver.$stamp@test.local"
        photoUrl = $null
    }

    $assistantDocument = "SA$stamp"
    $assistantId = Invoke-Create -Path "api/v1/transport-assistants" -Body @{
        documentType = 1
        documentNumber = $assistantDocument
        firstName = "Security"
        lastName = "Assistant"
        phone = "8095551004"
        street = "Assistant Street"
        city = "Security City"
        email = "security.assistant.$stamp@test.local"
        photoUrl = $null
    }

    $stopId = Invoke-Create -Path "api/v1/stops" -Body @{
        name = "Security Stop $stamp"
        street = "Stop Street"
        city = "Security City"
        latitude = 18.4861
        longitude = -69.9312
        sectorId = $sectorId
    }

    $routeId = Invoke-Create -Path "api/v1/routes" -Body @{
        name = "Security Route $stamp"
        schoolId = $schoolId
        startTime = $today.AddHours(9).ToString("o")
        endTime = $today.AddHours(10).ToString("o")
    }

    $addStop = Invoke-Request -Method "POST" -Path "api/v1/routes/$routeId/stops" -Body @{
        routeId = $routeId
        stopId = $stopId
        stopOrder = 1
    }
    if (-not $addStop.Ok) { throw "Add stop failed: $($addStop.Error)" }

    $activateRoute = Invoke-Request -Method "PUT" -Path "api/v1/routes/$routeId" -Body @{
        id = $routeId
        name = "Security Route $stamp"
        schoolId = $schoolId
        startTime = $today.AddHours(9).ToString("o")
        endTime = $today.AddHours(10).ToString("o")
        status = 2
    }
    if (-not $activateRoute.Ok) { throw "Activate route failed: $($activateRoute.Error)" }

    $assignmentId = Invoke-Create -Path "api/v1/route-assignments" -Body @{
        routeId = $routeId
        driverId = $driverId
        vehicleId = $vehicleId
        transportAssistantId = $assistantId
        vehicleCapacity = 20
    }

    $assignStudent = Invoke-Request -Method "POST" -Path "api/v1/route-assignments/$assignmentId/students/$studentId"
    if (-not $assignStudent.Ok) { throw "Assign student failed: $($assignStudent.Error)" }

    $tripId = Invoke-Create -Path "api/v1/trips/start" -Body @{
        routeAssignmentId = $assignmentId
    }

    $guardianToken = Get-Token -User $guardianDocument -Pass $guardianDocument
    $driverToken = Get-Token -User $driverDocument -Pass $driverDocument
    $assistantToken = Get-Token -User $assistantDocument -Pass $assistantDocument

    Add-Result -Actor "Guardian" -Check "GET own student" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/students/$studentId" -Token $guardianToken)

    Add-Result -Actor "Guardian" -Check "GET foreign student is denied" -ExpectedSuccess $false `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/students/$otherStudentId" -Token $guardianToken)

    Add-Result -Actor "Guardian" -Check "GET my students" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/me/students" -Token $guardianToken)

    Add-Result -Actor "Guardian" -Check "GET related trip" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/trips/$tripId" -Token $guardianToken)

    Add-Result -Actor "Driver" -Check "GET my assignments" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/me/route-assignments" -Token $driverToken)

    Add-Result -Actor "Driver" -Check "GET assigned route" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/routes/$routeId" -Token $driverToken)

    Add-Result -Actor "Driver" -Check "GET assigned trip" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/trips/$tripId" -Token $driverToken)

    Add-Result -Actor "Assistant" -Check "GET my assignments" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/me/route-assignments" -Token $assistantToken)

    Add-Result -Actor "Assistant" -Check "GET assigned route" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/routes/$routeId" -Token $assistantToken)

    Add-Result -Actor "Assistant" -Check "GET assigned trip" -ExpectedSuccess $true `
        -Result (Invoke-Request -Method "GET" -Path "api/v1/trips/$tripId" -Token $assistantToken)

    $script:Results | Format-Table -AutoSize -Wrap

    $total = $script:Results.Count
    $successful = ($script:Results | Where-Object { $_.Result -eq "OK" }).Count
    $failed = $total - $successful

    Write-Host ""
    Write-Host "SUMMARY"
    Write-Host "Total checks: $total"
    Write-Host "Successful: $successful"
    Write-Host "Failed: $failed"

    if ($failed -gt 0) {
        exit 1
    }
}
finally {
    if ($script:Client) {
        $script:Client.Dispose()
    }
}


