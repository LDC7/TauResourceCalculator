[Console]::InputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$projectPath = "TauResourceCalculator.BlazorServer/TauResourceCalculator.BlazorServer.csproj"

$dbContextType = "SQLite"
$dbContextName = "Application" + $dbContextType + "DbContext"

$migrationName = Read-Host "Введите имя миграции:"

$outputPath = "./Data/" + $dbContextType + "/Migrations/"
$namespace = "TauResourceCalculator.BlazorServer.Data." + $dbContextType + ".Migrations"

dotnet ef migrations add $migrationName -o $outputPath -n $namespace -p $projectPath -s $projectPath -c $dbContextName
