@echo off
echo Starting Blazor development environment...

REM Open two new command prompt windows
start "Dotnet Watch" cmd /k "cd ..\WebApp && dotnet watch"
start "Tailwind CSS" cmd /k "cd ..\WebApp && npx tailwindcss -i tailwind.css -o wwwroot/min.css --watch"

echo Development environment launched. You can now close this window.