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

set DynamoPackages=%~dp0\packages\_packages
set CurrentDir=%~dp0
if %CurrentDir:~-1%==\ (
        set CurrentDir=%CurrentDir:~0,-1%
)


REM 1. set the directory to find nugetCLI ie. nuget.exe               
set NugetExe=%CurrentDir%\Tools\NugetCLI\nuget.exe

REM 2. download 3rdParty packages by Aget.py
:DownloadNugetPackages
    
    set AgetFile=%CurrentDir%\Tools\Aget\aget.exe
    set nugetConfig = %CurrentDir%\Config\dynamo-nuget.config

    echo Running Python script from !AgetFile! using dynamo-nuget.config file

    set PythonAget="!AgetFile!" -os win -config release -iset intel64 -toolchain v140 -linkage shared -packagesDir "!DynamoPackages!" -nuget "!NugetExe!" -framework net45 -nugetConfig "!nugetConfig!"

    call :TrackTime "[Aget] Downloading 3rdParty packages from Nuget and Artifactory server, might take a while if running for the first time."
    echo If any nuget packages is not in nuget.org server then the server is switched to Artifactory...

    :: Symlinks are generated here
    set ThirdPartyDir=%CurrentDir%\packages
    !PythonAget! -agettable "%CurrentDir%\Config\packages.aget" -refsDir !ThirdPartyDir!
    (call )
    if ERRORLEVEL 1 (
            echo ERROR: Failed to update Dynamo 3rdParty nuget packages in !ThirdPartyDir!\packages.aget
            exit /b 1
    )
    call :TrackTime "%~n0: finished successfully"
    endlocal
    exit /b 0
:TrackTime
echo ========================================================
echo %~1
time /t
echo ========================================================
