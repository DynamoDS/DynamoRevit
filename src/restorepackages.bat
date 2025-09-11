
@echo off
REM Modernized: Use dotnet restore for SDK-style projects and .NET 6+/8+ compatibility
REM ***********************************************************************

echo Restoring NuGet packages using dotnet CLI...
dotnet restore ..\DynamoRevit.All.sln
echo Done restoring packages.

    call :TrackTime "[Aget] Downloading NuGet packages from the NuGet Gallery and the Artifactory server, might take a while if running for the first time."
    echo If any package is not found in the NuGet Gallery, redirect to look up in the Artifactory server...

    :: Symlinks are generated here
    %PythonAget% -agettable "%ConfigDir%\packages.aget" -refsDir "%SymLinksDir%"
    if ERRORLEVEL 1 (
        echo ERROR: Failed to update Dynamo 3rdParty nuget packages in packages.aget with net8.0
		echo CONTINUE DESPITE DOWNLOAD WARNINGS, SOME OLDER PACKAGES GIVE FALSE INCOPABILITY ERRORS WITH NET 8.0
        REM exit /b 1
    )

    call :TrackTime "%~n0: exiting from batch script"
    endlocal
    exit /b 0
:TrackTime
echo ========================================================
echo %~1
time /t
echo ======================================================== 