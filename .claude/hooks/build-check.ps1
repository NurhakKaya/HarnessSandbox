# Runs when Claude finishes responding. Blocks if the build fails.
$null = [Console]::In.ReadToEnd()   # hook input JSON (not needed here)

Set-Location $env:CLAUDE_PROJECT_DIR

# Only build if C#, Razor or project files changed
$changed = git status --porcelain -uall |
    Where-Object { $_ -match '\.(cs|cshtml|csproj)$' }
if (-not $changed) { exit 0 }

$output = dotnet build --nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) {
    $errors = $output | Select-String ': error ' | Select-Object -First 20
    [Console]::Error.WriteLine(
        "dotnet build failed. Fix these errors before finishing:`n" +
        ($errors -join "`n"))
    exit 2
}
exit 0