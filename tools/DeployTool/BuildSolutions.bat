call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\amd64\vcvars64.bat" > PreBuild.log

set toolFolder=C:\DeployTool
set DynamoSolution=..\Dynamo\src\Dynamo.All.sln
set DynamoRevitSolution=..\DynamoRevit\src\DynamoRevit.2013.sln

%toolFolder%\nuget.exe restore %DynamoSolution%
msbuild %DynamoSolution% /t:build /p:Configuration=Debug /p:Platform="Any CPU" > DynamoBuildDebug.log
msbuild %DynamoSolution% /t:build /p:Configuration=Release /p:Platform="Any CPU" > DynamoBuildRelease.log
msbuild %DynamoSolution% /t:build /p:Configuration=Release /p:Platform="Any CPU" > DynamoBuildRelease.log

%toolFolder%\nuget.exe restore %DynamoRevitSolution%
msbuild %DynamoRevitSolution% /t:build /p:Configuration=Debug /p:Platform="Any CPU" > DynamoRevitBuildDebug.log
msbuild %DynamoRevitSolution% /t:build /p:Configuration=Release /p:Platform="Any CPU" > DynamoRevitBuildRelease.log
msbuild %DynamoRevitSolution% /t:build /p:Configuration=Release /p:Platform="Any CPU" > DynamoRevitBuildRelease.log
