$dbContextType = "SQLite"
$migrationName = Read-Host "Input migration name"

$projectFolder = [IO.Path]::GetFullPath("./Infrastructure/Data." + $dbContextType + "/");
$prevLocation = Get-Location
Set-Location $projectFolder

$outputPath = "Migrations"
$namespace = "TauResourceCalculator.Infrastructure.Data." + $dbContextType + ".Migrations"
dotnet ef migrations add $migrationName -o $outputPath -n $namespace

Set-Location $prevLocation
