@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

set projectPath=TauResourceCalculator.BlazorServer
cd %projectPath%

set dbContextType=SQLite
set /p migrationName="Введите имя миграции:"

set outputPath=Data/%dbContextType%/Migrations
set namespace=TauResourceCalculator.BlazorServer.Data.%dbContextType%.Migrations

dotnet ef migrations add %migrationName% -o %outputPath% -n %namespace%
