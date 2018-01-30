set toolFolder=C:\DeployTool

set referenceFolderBase=..\DynamoRevit
set libFolder=lib
set revitVersion=Revit 2019
set netVersion=net452

set destinationFolder=%referenceFolderBase%\%libFolder%\%revitVersion%\%netVersion%

md "%destinationFolder%"

%toolFolder%\NuGet.exe install -prerelease RevitAPI -ConfigFile %toolFolder%\NuGet.Config
%toolFolder%\NuGet.exe install -prerelease RevitAPIUI -ConfigFile %toolFolder%\NuGet.Config

FOR /D %%G IN (RevitAPI.*) DO (
      copy "%%G\lib\net452\*.*" "%destinationFolder%\*.*"
      rmdir %%G /s /q
      )

FOR /D %%G IN (RevitAPIUI.*) DO (
      copy "%%G\lib\net452\*.*" "%destinationFolder%\*.*"
      rmdir %%G /s /q
      )

