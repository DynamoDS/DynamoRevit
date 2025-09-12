
@echo off
REM Modernized: Use dotnet restore for SDK-style projects and .NET 6+/8+ compatibility
REM ***********************************************************************

echo Restoring NuGet packages using dotnet CLI...

set SOLN=.\DynamoRevit.All.sln
if not exist %SOLN% (
	echo ERROR: Solution file %SOLN% not found.
	exit /b 1
)
dotnet restore %SOLN% --configfile .\Config\dynamo-nuget.config --packages .\packages\_packages
echo Done restoring packages.

REM === Create symlinks for key DLLs in packages directory (Aget legacy step) ===
setlocal EnableExtensions
set SYMLINKDIR=packages
set PKGROOT=packages\_packages

REM Dynamically create directory symlinks for all packages in packages.aget
for /f "tokens=1,2 delims=:" %%a in ('findstr /r /c:"^[ ]*\"[^"]\+\"[ ]*:[ ]*\"[^"]\+\"" Config\packages.aget') do (
	set "pkg=%%~a"
	set "ver=%%~b"
	setlocal enabledelayedexpansion
	set "pkg=!pkg:~1,-1!"
	set "ver=!ver:~1,-1!"
	set "srcdir=%PKGROOT%\!pkg!\!ver!"
	set "linkdir=%SYMLINKDIR%\!pkg!"
	if exist !linkdir! rmdir /s /q !linkdir!
	if exist !srcdir! mklink /D !linkdir! !srcdir!
	endlocal
)

goto :eof

:CreateSymlink
REM %1 = link name, %2 = target path
set LINKPATH=%SYMLINKDIR%\%1
set TARGETPATH=%2
if exist %LINKPATH% del %LINKPATH%
mklink %LINKPATH% %TARGETPATH%
exit /b 0