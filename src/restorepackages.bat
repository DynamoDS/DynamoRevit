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

REM 2. download 3rdParty packages by Aget.exe
    echo Running Python script from %AgetFile% using dynamo-nuget.config file
    set PythonAget="%AgetFile%" -os win -config release -iset intel64 -toolchain v140 -linkage shared -packagesDir "%DynamoPackages%" -nuget "%NugetExe%" -framework NET80 -nugetConfig "%NugetConfig%"

    call :TrackTime "[Aget] Downloading NuGet packages from the NuGet Gallery and the Artifactory server, might take a while if running for the first time."
    echo If any package is not found in the NuGet Gallery, redirect to look up in the Artifactory server...

    :: Symlinks are generated here
    %PythonAget% -agettable "%ConfigDir%\packages.aget" -refsDir "%SymLinksDir%"
    if ERRORLEVEL 1 (
        echo ERROR: Failed to update Dynamo 3rdParty nuget packages in packages.aget
        exit /b 1
    )
    call :TrackTime "%~n0: exiting from batch script"
    endlocal
    exit /b 0
:TrackTime
echo ========================================================
echo %~1
time /t
echo ========================================================