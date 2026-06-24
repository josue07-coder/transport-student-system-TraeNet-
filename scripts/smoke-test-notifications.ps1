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

    return $result
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
        throw "Login failed. Cannot continue notifications smoke test."
    }

    $script:Token = ($login.Content | ConvertFrom-Json).token
    $stamp = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $today = [DateTime]::UtcNow.Date

    $sectorId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/sectors" -Method "POST" -Path "api/v1/sectors" -ReturnId -Body @{
        name = "Notif Sector $stamp"
        province = "Smoke Province"
        city = "Smoke City"
    }

    $schoolId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/schools" -Method "POST" -Path "api/v1/schools" -ReturnId -Body @{
        name = "Notif School $stamp"
        directorName = "Notif Director"
        email = "notif.school.$stamp@smoke.local"
        phone = "8095550001"
        street = "Smoke Street"
        city = "Smoke City"
        sectorId = $sectorId
    }

    $gradeId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/grades" -Method "POST" -Path "api/v1/grades" -ReturnId -Body @{
        name = "Notif Grade $stamp"
        schoolId = $schoolId
    }

    $guardianId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/guardians" -Method "POST" -Path "api/v1/guardians" -ReturnId -Body @{
        documentType = 1
        documentNumber = "NG$stamp"
        firstName = "Notif"
        lastName = "Guardian"
        phone = "8095550002"
        street = "Guardian Street"
        city = "Smoke City"
        gender = 1
        sectorId = $sectorId
    }

    $studentId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/students" -Method "POST" -Path "api/v1/students" -ReturnId -Body @{
        firstName = "Notif"
        lastName = "Student"
        schoolId = $schoolId
        gradeId = $gradeId
        guardianId = $guardianId
        photoUrl = $null
    }

    $vehicleId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/vehicles" -Method "POST" -Path "api/v1/vehicles" -ReturnId -Body @{
        plateNumber = "NTF$($stamp.ToString().Substring($stamp.ToString().Length - 6))"
        model = "Notif Bus"
        brand = "Smoke"
        capacity = 20
        status = 1
    }

    $driverId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/drivers" -Method "POST" -Path "api/v1/drivers" -ReturnId -Body @{
        documentType = 1
        documentNumber = "ND$stamp"
        firstName = "Notif"
        lastName = "Driver"
        licenseNumber = "NL$stamp"
        phone = "8095550003"
        street = "Driver Street"
        city = "Smoke City"
        email = "notif.driver.$stamp@smoke.local"
        photoUrl = $null
    }

    $assistantId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/transport-assistants" -Method "POST" -Path "api/v1/transport-assistants" -ReturnId -Body @{
        documentType = 1
        documentNumber = "NA$stamp"
        firstName = "Notif"
        lastName = "Assistant"
        phone = "8095550004"
        street = "Assistant Street"
        city = "Smoke City"
        email = "notif.assistant.$stamp@smoke.local"
        photoUrl = $null
    }

    $stopId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/stops" -Method "POST" -Path "api/v1/stops" -ReturnId -Body @{
        name = "Notif Stop $stamp"
        street = "Stop Street"
        city = "Smoke City"
        latitude = 18.4861
        longitude = -69.9312
        sectorId = $sectorId
    }

    $routeId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/routes" -Method "POST" -Path "api/v1/routes" -ReturnId -Body @{
        name = "Notif Route $stamp"
        schoolId = $schoolId
        startTime = $today.AddHours(8).ToString("o")
        endTime = $today.AddHours(9).ToString("o")
    }

    Invoke-Step -Group "SETUP" -Label "POST /api/v1/routes/{routeId}/stops" -Method "POST" -Path "api/v1/routes/$routeId/stops" -Body @{
        routeId = $routeId
        stopId = $stopId
        stopOrder = 1
    } | Out-Null

    Invoke-Step -Group "SETUP" -Label "PUT /api/v1/routes/{id}" -Method "PUT" -Path "api/v1/routes/$routeId" -Body @{
        id = $routeId
        name = "Notif Route $stamp"
        schoolId = $schoolId
        startTime = $today.AddHours(8).ToString("o")
        endTime = $today.AddHours(9).ToString("o")
        status = 2
    } | Out-Null

    $assignmentId = Invoke-Step -Group "SETUP" -Label "POST /api/v1/route-assignments" -Method "POST" -Path "api/v1/route-assignments" -ReturnId -Body @{
        routeId = $routeId
        vehicleId = $vehicleId
        driverId = $driverId
        transportAssistantId = $assistantId
        vehicleCapacity = 20
    }

    Invoke-Step -Group "NOTIFICATIONS" -Label "POST /api/v1/route-assignments/{assignmentId}/students/{studentId}" -Method "POST" -Path "api/v1/route-assignments/$assignmentId/students/$studentId" | Out-Null

    $tripId = Invoke-Step -Group "NOTIFICATIONS" -Label "POST /api/v1/trips/start" -Method "POST" -Path "api/v1/trips/start" -ReturnId -Body @{
        routeAssignmentId = $assignmentId
    }

    $notificationsResult = Invoke-Step -Group "NOTIFICATIONS" -Label "GET /api/v1/notifications" -Method "GET" -Path "api/v1/notifications?PageNumber=1&PageSize=10" -AllowFailure
    $unreadResult = Invoke-Step -Group "NOTIFICATIONS" -Label "GET /api/v1/notifications/unread" -Method "GET" -Path "api/v1/notifications/unread" -AllowFailure

    if ($notificationsResult.Ok) {
        $notifications = $notificationsResult.Content | ConvertFrom-Json
        $firstNotification = $notifications.items | Select-Object -First 1
        if ($firstNotification) {
            Invoke-Step -Group "NOTIFICATIONS" -Label "PUT /api/v1/notifications/{id}/read" -Method "PUT" -Path "api/v1/notifications/$($firstNotification.id)/read" | Out-Null
        }
    }

    Invoke-Step -Group "NOTIFICATIONS" -Label "PUT /api/v1/notifications/read-all" -Method "PUT" -Path "api/v1/notifications/read-all" -AllowFailure | Out-Null

    Invoke-Step -Group "CLEANUP" -Label "PUT /api/v1/trips/{id}/end" -Method "PUT" -Path "api/v1/trips/$tripId/end" -AllowFailure | Out-Null
}
finally {
    $script:Client.Dispose()
}

$script:Results |
    Select-Object Group, Endpoint, @{ Name = "Result"; Expression = { if ($_.Ok) { "OK" } else { "FAIL" } } }, StatusCode, Error |
    Format-Table -AutoSize

$total = $script:Results.Count
$failed = @($script:Results | Where-Object { -not $_.Ok })
$successful = $total - $failed.Count

Write-Host ""
Write-Host "SUMMARY"
Write-Host "Total checks: $total"
Write-Host "Successful: $successful"
Write-Host "Failed: $($failed.Count)"

if ($failed.Count -gt 0) {
    Write-Host ""
    Write-Host "Failures:"
    $failed | Select-Object Group, Endpoint, StatusCode, Error | Format-List
    exit 1
}


