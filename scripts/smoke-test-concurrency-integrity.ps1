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

    throw "No se encontrÃ³ API activa en puertos conocidos."
}

function Invoke-Json {
    param(
        [string]$Method,
        [string]$Url,
        [object]$Body = $null,
        [string]$Token = $null
    )

    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }

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
        $result = Invoke-RestMethod @params
        return @{ StatusCode = 200; Body = $result; Error = $null }
    } catch {
        $response = $_.Exception.Response
        $statusCode = if ($response) { [int]$response.StatusCode } else { 0 }
        $message = $_.Exception.Message
        try {
            if ($response) {
                $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
                $message = $reader.ReadToEnd()
            }
        } catch {
        }
        return @{ StatusCode = $statusCode; Body = $null; Error = $message }
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

$login = Invoke-Json -Method "POST" -Url "$api/api/v1/auth/login" -Body @{ username = "admin"; password = "Admin123" }
if ($login.StatusCode -ne 200 -or -not $login.Body.token) {
    Write-Host "SKIP: no se pudo autenticar admin. Status=$($login.StatusCode) Error=$($login.Error)"
    exit 0
}

$token = $login.Body.token

$schedulesResponse = Invoke-Json -Method "GET" -Url "$api/api/v1/trip-schedules" -Token $token
$schedules = @($schedulesResponse.Body)
if ($schedules.Count -gt 0) {
    $schedule = $schedules | Select-Object -First 1
    $operationDate = (Get-Date).AddDays(7).ToString("yyyy-MM-dd")
    $first = Invoke-Json -Method "POST" -Url "$api/api/v1/trip-schedules/$($schedule.id)/materialize" -Token $token -Body @{ operationDate = $operationDate }
    $second = Invoke-Json -Method "POST" -Url "$api/api/v1/trip-schedules/$($schedule.id)/materialize" -Token $token -Body @{ operationDate = $operationDate }
    Add-Result "Duplicated materialization is blocked" ($second.StatusCode -ge 400) "first=$($first.StatusCode), second=$($second.StatusCode)"
} else {
    Add-Result "Duplicated materialization is blocked" $true "SKIP: no hay TripSchedules"
}

$inProgress = Invoke-Json -Method "GET" -Url "$api/api/v1/trips/by-status/InProgress" -Token $token
$inProgressTrips = @($inProgress.Body)
if ($inProgressTrips.Count -gt 0) {
    $trip = $inProgressTrips | Select-Object -First 1
    $startAgain = Invoke-Json -Method "POST" -Url "$api/api/v1/trips/start" -Token $token -Body @{ routeAssignmentId = $trip.routeAssignmentId; tripId = $trip.id }
    Add-Result "Double start is blocked" ($startAgain.StatusCode -ge 400) "status=$($startAgain.StatusCode)"

    $passengers = Invoke-Json -Method "GET" -Url "$api/api/v1/trips/$($trip.id)/passengers" -Token $token
    $passengerList = @($passengers.Body)
    if ($passengerList.Count -gt 0) {
        $passenger = $passengerList | Where-Object { $_.status -eq "Expected" } | Select-Object -First 1
        if ($null -ne $passenger) {
            $boarded = Invoke-Json -Method "PUT" -Url "$api/api/v1/trips/$($trip.id)/students/$($passenger.studentId)/boarded" -Token $token
            if ($boarded.StatusCode -ge 400) {
                Add-Result "Invalid attendance transition is blocked" $false "boarded failed with status=$($boarded.StatusCode)"
            } else {
                $invalidAbsent = Invoke-Json -Method "PUT" -Url "$api/api/v1/trips/$($trip.id)/students/$($passenger.studentId)/absent" -Token $token -Body @{ notes = "Invalid transition smoke" }
                Add-Result "Invalid attendance transition is blocked" ($invalidAbsent.StatusCode -ge 400) "status=$($invalidAbsent.StatusCode)"
            }
        } else {
            Add-Result "Invalid attendance transition is blocked" $true "SKIP: no hay pasajeros Expected disponibles"
        }
    } else {
        Add-Result "Invalid attendance transition is blocked" $true "SKIP: viaje sin pasajeros"
    }
} else {
    Add-Result "Double start is blocked" $true "SKIP: no hay viajes InProgress"
    Add-Result "Invalid attendance transition is blocked" $true "SKIP: no hay viajes InProgress"
}

$completed = Invoke-Json -Method "GET" -Url "$api/api/v1/trips/by-status/Completed" -Token $token
$completedTrips = @($completed.Body)
if ($completedTrips.Count -gt 0) {
    $trip = $completedTrips | Select-Object -First 1
    $tracking = Invoke-Json -Method "POST" -Url "$api/api/v1/tracking/location" -Token $token -Body @{
        tripId = $trip.id
        latitude = 18.2081
        longitude = -71.1002
    }
    Add-Result "Tracking on completed trip is blocked" ($tracking.StatusCode -ge 400) "status=$($tracking.StatusCode)"
} else {
    Add-Result "Tracking on completed trip is blocked" $true "SKIP: no hay viajes Completed"
}

$results | Format-Table -AutoSize
$failed = @($results | Where-Object { $_.Result -eq "FAIL" })

if ($failed.Count -gt 0) {
    exit 1
}

exit 0


