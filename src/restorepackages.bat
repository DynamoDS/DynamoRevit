@echo off

REM ***********************************************************************
REM This bat file uses Aget: https://git.autodesk.com/Dynamo/Aget to 
REM download the nuget packages from closest Artifactory server for 
REM
REM Santa Clara: https://art-bobcat.autodesk.com
REM Novi:        https://art-cougar.autodesk.com
REM Singapore:   https://art-lion.autodesk.com
REM Shanghai:    https://art-panda.autodesk.com
REM Neuchatel:   https://art-chamois.autodesk.com
REM
REM ***********************************************************************

setlocal EnableDelayedExpansion
setlocal EnableExtensions

set D4RPackagePath=%~dp0\packages
set DynamoNugetPath = 
set CurrentDir=%~dp0
if %CurrentDir:~-1%==\ (
        set CurrentDir=%CurrentDir:~0,-1%
)

REM 1. setup the global nuget package location: D4RPackages
:SetupGlobalPackageLocation
    if "!D4RPackages!"=="" (
            echo The environment variable: D4RPackages was not defined, will use the default location: %D4RPackagePath% to store the global nuget packages for Dynamo4Revit.
            set D4RPackages=%D4RPackagePath%
    )
    REM remove "" in D4RPackages
    echo Break Test
    set D4RPackages=%D4RPackages:"=%


REM 2. find the closest AF server: AF_MIRROR
rem :FindClosestAFServer
rem     REM 2.1. use the system environment variable as first choice
::     if "!AF_MIRROR!"=="" (
rem             REM 2.2. read the closest AF server from git adsk config as second choice
::             FOR /F "delims=" %%i IN ('git config adsk.artifactory.server') DO set AF_MIRROR=https://%%i
::             ::FOR /F "delims=" %%i IN ('git config adsk.artifactory.server') DO set AF_MIRROR=https://www.nuget.org/packages
rem             echo Break here
rem             ::set AF_MIRROR = https://www.nuget.org/packages
rem             REM 2.3. set the AF_MIRROR to default one: art-bobcat as last choice
::             if "!AF_MIRROR!"=="" (
rem                     set AF_MIRROR=https://art-bobcat.autodesk.com
rem                     ::set AF_MIRROR=https://www.nuget.org/packages
rem                     echo WARNING: Please install the Autodesk Git bundle or set the environment variable: AF_MIRROR to the closest Artifactory server.
::             )
::     )
rem     echo AF_MIRROR: !AF_MIRROR!
REM 2. Set AF_MIRROR to the online public nuget server
set AF_MIRROR = "https://www.nuget.org/api/v2"
REM 3. download nuget.exe from closest AF server
:DownloadNuget
                
set NugetExe=%CurrentDir%\Tools\NugetCLI\nuget.exe
REM 4. download Python 3.x which is required by Aget
:DownloadPython352
    set Python352Exe=!D4RPackages!\python\tools\python.exe
    if not exist "!Python352Exe!" (
            echo Downloading python 3.5.2 to !D4RPackages!
            !NugetExe! install python -Version 3.5.2 -ExcludeVersion -OutputDirectory !D4RPackages!
    )
    if ERRORLEVEL 1 (
            echo ERROR: Failed to download python 3.5.2 to !D4RPackages!
            exit /b 1
   )

REM 5 download 3rdParty packages by Aget.py
:DownloadNugetPackages
    
                set AgetFile=%CurrentDir%\Tools\aget\aget.py
    set PythonAget="!Python352Exe!" "!AgetFile!" -os win -config release -iset intel64 -toolchain v140 -linkage shared -serverURL https://www.nuget.org/api/v2 -packagesDir "!D4RPackages!" -nuget "!NugetExe!" -framework net45

    call :TrackTime "[Aget] Downloading 3rdParty packages from Artifactory server: !AF_MIRROR!, might take a while if running for the first time."
    ::"https://www.nuget.org/api/v2"
    :: Symlinks are generated here
    set ThirdPartyDir=%CurrentDir%\Tools\3rdParty
    !PythonAget! -agettable "!ThirdPartyDir!\link3rdparty.aget" -refsDir !ThirdPartyDir!
    if ERRORLEVEL 1 (
            echo ERROR: Failed to update Dynamo4Revit 3rdParty nuget packages in !ThirdPartyDir!\link3rdparty.aget
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
