param(
    [switch]$StopOnFailure
)

$ErrorActionPreference = "Continue"

$scripts = @(
    "smoke-test.ps1",
    "smoke-test-write-endpoints.ps1",
    "smoke-test-security.ps1",
    "smoke-test-security-authorization.ps1",
    "smoke-test-users.ps1",
    "smoke-test-operational-seed.ps1",
    "smoke-test-assistant-permissions.ps1",
    "smoke-test-notifications.ps1",
    "smoke-test-incidents.ps1",
    "smoke-test-tracking.ps1",
    "smoke-test-reports.ps1",
    "smoke-test-system-settings.ps1",
    "smoke-test-integrations.ps1",
    "smoke-test-backups.ps1",
    "smoke-test-trip-schedules.ps1",
    "smoke-test-non-school-days.ps1",
    "smoke-test-trip-attendance.ps1",
    "smoke-test-trip-start-rules.ps1",
    "smoke-test-trip-route-deviations.ps1",
    "smoke-test-exceptional-passengers.ps1",
    "smoke-test-concurrency-integrity.ps1",
    "smoke-test-audit-traceability.ps1",
    "smoke-test-performance-pagination.ps1"
)

$results = New-Object System.Collections.Generic.List[object]

Write-Host "QA-6 Full Smoke Test Runner"
Write-Host "Scripts directory: $PSScriptRoot"
Write-Host ""

foreach ($script in $scripts) {
    $path = Join-Path $PSScriptRoot $script

    if (-not (Test-Path $path)) {
        $results.Add([pscustomobject]@{
            Script = $script
            Status = "MISSING"
            ExitCode = $null
            DurationSeconds = 0
        })
        continue
    }

    Write-Host "Running $script ..."
    $startedAt = Get-Date

    & powershell -ExecutionPolicy Bypass -File $path
    $exitCode = $LASTEXITCODE

    $duration = [Math]::Round(((Get-Date) - $startedAt).TotalSeconds, 2)
    $status = if ($exitCode -eq 0) { "PASS" } else { "FAIL" }

    $results.Add([pscustomobject]@{
        Script = $script
        Status = $status
        ExitCode = $exitCode
        DurationSeconds = $duration
    })

    Write-Host "$script => $status ($duration s)"
    Write-Host ""

    if ($StopOnFailure -and $exitCode -ne 0) {
        break
    }
}

$results | Format-Table -AutoSize

$total = $results.Count
$passed = ($results | Where-Object { $_.Status -eq "PASS" }).Count
$failed = ($results | Where-Object { $_.Status -eq "FAIL" }).Count
$missing = ($results | Where-Object { $_.Status -eq "MISSING" }).Count

Write-Host ""
Write-Host "SUMMARY"
Write-Host "Total scripts: $total"
Write-Host "Passed: $passed"
Write-Host "Failed: $failed"
Write-Host "Missing: $missing"

if ($failed -gt 0 -or $missing -gt 0) {
    exit 1
}

exit 0


