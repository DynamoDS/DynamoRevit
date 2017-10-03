SET base=..\..\..\Dynamo\bin\AnyCPU\Debug\Revit_2017\
SET framework=net45

SET zt=DynamoVisualProgramming.DynamoRevit2017

rmdir /s /q .\%zt%\build
rmdir /s /q .\%zt%\content
rmdir /s /q .\%zt%\lib
rmdir /s /q .\%zt%\tools

mkdir .\%zt%\build\net45
mkdir .\%zt%\content
mkdir .\%zt%\lib\net45
mkdir .\%zt%\tools

copy %base%\RevitNodes.dll .\%zt%\lib\%framework%\RevitNodes.dll
copy %base%\RevitNodes.xml .\%zt%\build\%framework%\RevitNodes.xml

copy %base%\DynamoRevitDS.dll .\%zt%\lib\%framework%\DynamoRevitDS.dll
copy %base%\RevitServices.dll .\%zt%\lib\%framework%\RevitServices.dll
copy %base%\nodes\DSRevitNodesUI.dll .\%zt%\lib\%framework%\DSRevitNodesUI.dll

nuget pack .\%zt%\DynamoRevit2017.nuspec
