param([string]$BaseUrl = 'http://localhost:5238')
$ErrorActionPreference = 'Stop'
foreach ($route in @('/health/live', '/', '/calculator', '/lab/converter', '/lab/trivia', '/lab/timer', '/lab/todo')) {
  $response = Invoke-WebRequest -Uri "$BaseUrl$route" -UseBasicParsing
  if ($response.StatusCode -ne 200) { throw "FAILED: $route => $($response.StatusCode)" }
  Write-Host "PASS $route ($($response.StatusCode))"
}
Write-Host 'Hosted HTTP smoke checks passed.'
