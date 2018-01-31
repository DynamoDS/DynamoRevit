setlocal
set needDeploy=0
set prefix=revit-d4r-

for /f %%i in ('git rev-parse HEAD') do set hashId=%%i

for /f %%a in ('git log -m -1 --name-only --pretty %hashId%') do (
    IF "%%a" == ".version" set needDeploy=1
)

set /p deployVersion=<.version
set folder=%prefix%%deployVersion%
echo %folder%

echo %needDeploy%

IF %needDeploy%==1 (
    echo "start to deploy"
    echo %folder%
    IF EXIST %folder% (
        rmdir /s /q %folder%
    ) 

    call mkdir %folder%
    call xcopy /y /i /s "../Dynamo/bin/AnyCPU/release" "./%folder%/release"
    call xcopy /y /i /s "../Dynamo/bin/AnyCPU/debug" "./%folder%/debug"
    call xcopy /y /i /s "../Dynamo/bin/AnyCPU/debug" "./%folder%/slow_debug"
    call "C:\Program Files\7-Zip\7z.exe" a -tzip "%folder%.zip" ".\%folder%\release" ".\%folder%\debug" ".\%folder%\slow_debug"
    call "C:\DeployTool\art.exe" upload "%folder%.zip" "team-revit-generic/revit-d4r/" --url=https://art-bobcat.autodesk.com/artifactory --user= --password=
)