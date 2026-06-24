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

function Invoke-SmokeRequest {
    param(
        [System.Net.Http.HttpClient]$Client,
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

    try {
        $response = $Client.SendAsync($request).GetAwaiter().GetResult()
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
            $result = Invoke-SmokeRequest -Client $client -Method "GET" -Path "swagger/index.html"
            if ($result.StatusCode -gt 0) {
                return $url
            }
        }
        finally {
            $client.Dispose()
        }
    }

    throw "No API instance detected on launchSettings URLs or common localhost ports."
}

function Get-ResponseId {
    param([string]$Content)

    $json = $Content | ConvertFrom-Json
    $guidValue = [Guid]::Empty
    if ($json -is [string] -and [Guid]::TryParse($json, [ref]$guidValue)) { return $json }
    if ($json.Id) { return [string]$json.Id }
    if ($json.id) { return [string]$json.id }

    throw "Response did not include Id. Content: $Content"
}

function Invoke-Step {
    param(
        [string]$Group,
        [string]$Label,
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [switch]$ReturnId,
        [switch]$AllowFailure
    )

    $result = Invoke-SmokeRequest -Client $script:Client -Method $Method -Path $Path -Body $Body -Token $script:Token
    $script:Results.Add([pscustomobject]@{
        Group = $Group
        Endpoint = $Label
        Ok = $result.Ok
        StatusCode = $result.StatusCode
        Error = $result.Error
    })

    if (-not $result.Ok -and -not $AllowFailure) {
        throw "$Label failed with status $($result.StatusCode): $($result.Error)"
    }

    if ($ReturnId -and $result.Ok) {
        return Get-ResponseId -Content $result.Content
    }

    return $null
}

function Add-SkippedStep {
    param(
        [string]$Group,
        [string]$Label,
        [string]$Reason
    )

    $script:Results.Add([pscustomobject]@{
        Group = $Group
        Endpoint = $Label
        Ok = $false
        StatusCode = 0
        Error = "SKIPPED: $Reason"
    })
}

function Invoke-ExpectedFailureStep {
    param(
        [string]$Group,
        [string]$Label,
        [string]$Method,
        [string]$Path,
        [object]$Body = $null
    )

    $result = Invoke-SmokeRequest -Client $script:Client -Method $Method -Path $Path -Body $Body -Token $script:Token
    $isExpectedFailure = -not $result.Ok -and $result.StatusCode -ge 400 -and $result.StatusCode -lt 500
    $script:Results.Add([pscustomobject]@{
        Group = $Group
        Endpoint = $Label
        Ok = $isExpectedFailure
        StatusCode = $result.StatusCode
        Error = if ($isExpectedFailure) { $null } else { $result.Error }
    })
}

$baseUrl = Find-ApiBaseUrl
$script:Client = New-HttpClient -BaseUrl $baseUrl
$script:Results = New-Object System.Collections.Generic.List[object]

try {
    Write-Host "Base URL: $baseUrl"

    $login = Invoke-SmokeRequest -Client $script:Client -Method "POST" -Path "api/v1/auth/login" -Body @{
        username = $Username
        password = $Password
    }

    $script:Results.Add([pscustomobject]@{
        Group = "AUTH"
        Endpoint = "POST /api/v1/auth/login"
        Ok = $login.Ok
        StatusCode = $login.StatusCode
        Error = $login.Error
    })

    if (-not $login.Ok) {
        throw "Login failed. Cannot continue write smoke test."
    }

    $script:Token = ($login.Content | ConvertFrom-Json).token
    if ([string]::IsNullOrWhiteSpace($script:Token)) {
        throw "Login response did not include token."
    }

    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $today = [DateTime]::UtcNow.Date

    $sectorId = Invoke-Step -Group "EDUCATION" -Label "POST /api/v1/sectors" -Method "POST" -Path "api/v1/sectors" -ReturnId -Body @{
        name = "Smoke Sector $stamp"
        province = "Smoke Province"
        city = "Smoke City"
    }

    $schoolId = Invoke-Step -Group "EDUCATION" -Label "POST /api/v1/schools" -Method "POST" -Path "api/v1/schools" -ReturnId -Body @{
        name = "Smoke School $stamp"
        directorName = "Smoke Director"
        email = "school.$stamp@smoke.local"
        phone = "8095550001"
        street = "Smoke Street"
        city = "Smoke City"
        sectorId = $sectorId
    }

    $gradeId = Invoke-Step -Group "EDUCATION" -Label "POST /api/v1/grades" -Method "POST" -Path "api/v1/grades" -ReturnId -Body @{
        name = "Smoke Grade $stamp"
        schoolId = $schoolId
    }

    $guardianId = Invoke-Step -Group "EDUCATION" -Label "POST /api/v1/guardians" -Method "POST" -Path "api/v1/guardians" -ReturnId -Body @{
        documentType = 1
        documentNumber = "G$stamp"
        firstName = "Smoke"
        lastName = "Guardian"
        phone = "8095550002"
        street = "Guardian Street"
        city = "Smoke City"
        gender = 1
        sectorId = $sectorId
    }

    $studentId = Invoke-Step -Group "EDUCATION" -Label "POST /api/v1/students" -Method "POST" -Path "api/v1/students" -ReturnId -Body @{
        firstName = "Smoke"
        lastName = "Student"
        schoolId = $schoolId
        gradeId = $gradeId
        guardianId = $guardianId
    }

    $vehicleId = Invoke-Step -Group "TRANSPORT" -Label "POST /api/v1/vehicles" -Method "POST" -Path "api/v1/vehicles" -ReturnId -Body @{
        plateNumber = "SMK$($stamp % 1000000)"
        capacity = 20
    }

    $driverId = Invoke-Step -Group "TRANSPORT" -Label "POST /api/v1/drivers" -Method "POST" -Path "api/v1/drivers" -ReturnId -Body @{
        firstName = "Smoke"
        lastName = "Driver"
        documentType = 1
        documentNumber = "D$stamp"
        licenseNumber = "LIC$stamp"
        phone = "8095550003"
        street = "Driver Street"
        city = "Smoke City"
        email = "driver.$stamp@smoke.local"
        photoUrl = $null
    }

    $assistantId = Invoke-Step -Group "TRANSPORT" -Label "POST /api/v1/transport-assistants" -Method "POST" -Path "api/v1/transport-assistants" -ReturnId -Body @{
        documentType = 1
        documentNumber = "A$stamp"
        firstName = "Smoke"
        lastName = "Assistant"
        phone = "8095550004"
        street = "Assistant Street"
        city = "Smoke City"
        email = "assistant.$stamp@smoke.local"
        photoUrl = $null
    }

    $stopId = Invoke-Step -Group "TRANSPORT" -Label "POST /api/v1/stops" -Method "POST" -Path "api/v1/stops" -ReturnId -Body @{
        name = "Smoke Stop $stamp"
        street = "Stop Street"
        city = "Smoke City"
        latitude = 18.4861
        longitude = -69.9312
        sectorId = $sectorId
    }

    $routeId = Invoke-Step -Group "TRANSPORT" -Label "POST /api/v1/routes" -Method "POST" -Path "api/v1/routes" -ReturnId -Body @{
        name = "Smoke Route $stamp"
        schoolId = $schoolId
        startTime = $today.AddHours(7).ToString("o")
        endTime = $today.AddHours(8).ToString("o")
    }

    Invoke-Step -Group "ROUTE_STOPS" -Label "POST /api/v1/routes/{routeId}/stops" -Method "POST" -Path "api/v1/routes/$routeId/stops" -AllowFailure -Body @{
        routeId = $routeId
        stopId = $stopId
        stopOrder = 1
    }

    Invoke-Step -Group "ROUTE_STOPS" -Label "PUT /api/v1/routes/{routeId}/stops/{stopId}/order" -Method "PUT" -Path "api/v1/routes/$routeId/stops/$stopId/order" -AllowFailure -Body @{
        routeId = $routeId
        stopId = $stopId
        stopOrder = 2
    }

    Invoke-Step -Group "ROUTE_STOPS" -Label "DELETE /api/v1/routes/{routeId}/stops/{stopId}" -Method "DELETE" -Path "api/v1/routes/$routeId/stops/$stopId" -AllowFailure

    Invoke-Step -Group "ROUTE_STOPS" -Label "POST /api/v1/routes/{routeId}/stops (re-add for assignment)" -Method "POST" -Path "api/v1/routes/$routeId/stops" -AllowFailure -Body @{
        routeId = $routeId
        stopId = $stopId
        stopOrder = 1
    }

    Invoke-Step -Group "TRANSPORT" -Label "PUT /api/v1/routes/{id} (activate for assignment rules)" -Method "PUT" -Path "api/v1/routes/$routeId" -Body @{
        id = $routeId
        name = "Smoke Route $stamp"
        schoolId = $schoolId
        startTime = $today.AddHours(7).ToString("o")
        endTime = $today.AddHours(8).ToString("o")
        status = 2
    }

    $assignmentId = Invoke-Step -Group "ROUTE_ASSIGNMENTS" -Label "POST /api/v1/route-assignments" -Method "POST" -Path "api/v1/route-assignments" -ReturnId -Body @{
        routeId = $routeId
        driverId = $driverId
        vehicleId = $vehicleId
        transportAssistantId = $assistantId
        vehicleCapacity = 20
    }

    Invoke-Step -Group "ROUTE_ASSIGNMENTS" -Label "POST /api/v1/route-assignments/{assignmentId}/students/{studentId}" -Method "POST" -Path "api/v1/route-assignments/$assignmentId/students/$studentId"

    Invoke-Step -Group "ROUTE_ASSIGNMENTS" -Label "DELETE /api/v1/route-assignments/{assignmentId}/students/{studentId}" -Method "DELETE" -Path "api/v1/route-assignments/$assignmentId/students/$studentId"

    Invoke-Step -Group "ROUTE_ASSIGNMENTS" -Label "POST /api/v1/route-assignments/{assignmentId}/students/{studentId} (re-assign for trip)" -Method "POST" -Path "api/v1/route-assignments/$assignmentId/students/$studentId"

    $tripId = Invoke-Step -Group "TRIPS" -Label "POST /api/v1/trips/start" -Method "POST" -Path "api/v1/trips/start" -ReturnId -AllowFailure -Body @{
        routeAssignmentId = $assignmentId
    }

    if ($tripId) {
        Invoke-Step -Group "TRIPS" -Label "PUT /api/v1/trips/{id}/end" -Method "PUT" -Path "api/v1/trips/$tripId/end"
    }
    else {
        Add-SkippedStep -Group "TRIPS" -Label "PUT /api/v1/trips/{id}/end" -Reason "POST /api/v1/trips/start did not create a trip"
    }

    $tripToCancelId = Invoke-Step -Group "TRIPS" -Label "POST /api/v1/trips/start (for cancel)" -Method "POST" -Path "api/v1/trips/start" -ReturnId -AllowFailure -Body @{
        routeAssignmentId = $assignmentId
    }

    if ($tripToCancelId) {
        Invoke-ExpectedFailureStep -Group "TRIPS" -Label "PUT /api/v1/trips/{id}/cancel (reject in-progress trip)" -Method "PUT" -Path "api/v1/trips/$tripToCancelId/cancel" -Body @{
            reason = "Smoke test cancellation attempt"
        }
    }
    else {
        Add-SkippedStep -Group "TRIPS" -Label "PUT /api/v1/trips/{id}/cancel" -Reason "POST /api/v1/trips/start did not create a trip"
    }

    $script:Results |
        Select-Object Group, Endpoint, @{Name="Result"; Expression={ if ($_.Ok) { "OK" } else { "FAIL" } }}, StatusCode, Error |
        Format-Table -AutoSize -Wrap

    $total = $script:Results.Count
    $successful = ($script:Results | Where-Object { $_.Ok }).Count
    $failed = $total - $successful

    Write-Host ""
    Write-Host "SUMMARY"
    Write-Host "Total endpoints tested: $total"
    Write-Host "Successful: $successful"
    Write-Host "Failed: $failed"

    if ($failed -gt 0) {
        Write-Host ""
        Write-Host "FAILURES"
        $script:Results |
            Where-Object { -not $_.Ok } |
            Select-Object Group, Endpoint, StatusCode, Error |
            Format-Table -AutoSize -Wrap
        exit 1
    }
}
finally {
    $script:Client.Dispose()
}


