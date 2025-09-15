@echo off
REM ***********************************************************************
REM This bat file uses Aget: https://git.autodesk.com/Dynamo/Aget to 
REM download the nuget packages from NuGet server or closest Artifactory server for 
REM
REM NuGet Server: https://www.nuget.org
REM Santa Clara: https://art-bobcat.autodesk.com
REM
REM ***********************************************************************

setlocal EnableDelayedExpansion
setlocal EnableExtensions

REM 1. set variable values
set DynamoPackages=%~dp0\packages\_packages
echo %DynamoPackages%
set CurrentDir=%~dp0
if %CurrentDir:~-1%==\ (
    set CurrentDir=%CurrentDir:~0,-1%
)
set ConfigDir=%CurrentDir%\Config
set SymLinksDir=%CurrentDir%\packages

set NugetExe=%CurrentDir%\Tools\NugetCLI\nuget.exe
set AgetFile=%CurrentDir%\Tools\Aget\aget.exe
set NugetConfig=%ConfigDir%\dynamo-nuget.config

REM 2. Restore NuGet packages using dotnet CLI
set SolutionFile=%CurrentDir%\DynamoRevit.All.sln
if not exist "%SolutionFile%" (
    echo ERROR: Solution file %SolutionFile% not found.
    exit /b 1
)
call :TrackTime "[dotnet] Restoring NuGet packages for solution, might take a while if running for the first time."
REM Use nuget.exe restore to ensure packages go to the correct folder
set NugetExePath=%CurrentDir%\..\..\DynamoRevitUtils\tools\NuGet.exe
if not exist "%NugetExePath%" set NugetExePath=%CurrentDir%\tools\NuGet.exe
if not exist "%NugetExePath%" (
    echo ERROR: NuGet.exe not found at %NugetExePath%
    exit /b 1
)
call :TrackTime "[nuget.exe] Restoring NuGet packages for solution, might take a while if running for the first time."
"%NugetExePath%" restore "%SolutionFile%" -ConfigFile "%NugetConfig%" -PackagesDirectory "%DynamoPackages%"
if ERRORLEVEL 1 (
    echo ERROR: nuget.exe restore failed.
    exit /b 1
)

REM 3. Create symlinks for each package as specified in packages.aget
set PKGROOT=%CurrentDir%\packages\_packages
set SYMLINKDIR=%CurrentDir%\packages
set AGETFILE=%ConfigDir%\packages.aget

REM Parse packages.aget and create symlinks
REM Only process lines that look like "PackageName": "Version"
for /f "tokens=1,2 delims=:" %%a in ('findstr /r /c:"^[ ]*\"[A-Za-z0-9_.-]*\"[ ]*:[ ]*\"[A-Za-z0-9_.-]*\"" "%AGETFILE%"') do (
    set "pkg=%%~a"
    set "ver=%%~b"
    setlocal enabledelayedexpansion
    rem Remove spaces and quotes
    set "pkg=!pkg: =!"
    set "pkg=!pkg:~1,-1!"
    set "ver=!ver: =!"
    set "ver=!ver:~1,-1!"
    set "srcdir=%PKGROOT%\!pkg!\!ver!"
    set "linkdir=%SYMLINKDIR%\!pkg!"
    echo Processing package !pkg! version !ver!
    if exist !linkdir! (
        if exist !linkdir!\NUL (
            echo Removing directory or directory symlink: !linkdir!
            rmdir /s /q !linkdir!
        ) else (
            echo Removing file or file symlink: !linkdir!
            del /f /q !linkdir!
        )
    )
    if exist !srcdir! (
        echo Creating symlink: !linkdir! -> !srcdir!
        mklink /D !linkdir! !srcdir!
    ) else (
        echo WARNING: Source directory !srcdir! does not exist, skipping symlink.
    )
    endlocal
)

call :TrackTime "%~n0: exiting from batch script"
endlocal
exit /b 0
:TrackTime
echo ========================================================
echo %~1
time /t
echo ========================================================