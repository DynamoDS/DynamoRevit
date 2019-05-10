@echo off

set SOLUTIONDIR=%~1

set SamplesPackage=%SOLUTIONDIR%packages\DynamoVisualProgramming.DynamoSamples\Samples
set BasicSampleFiles=%SamplesPackage%\en-US\Basics
set CoreSampleFiles=%SamplesPackage%\en-US\Core
set GeometrySampleFiles=%SamplesPackage%\en-US\Geometry
set ImportExportSampleFiles=%SamplesPackage%\en-US\ImportExport
set RevitSampleFiles=%SamplesPackage%\en-US\Revit
set SampleDataFiles=%SamplesPackage%\Data
set SampleTestFolder=%SOLUTIONDIR%..\test\System\Samples

IF EXIST %SampleTestFolder% (
	xcopy /y /i /s "%BasicSampleFiles%\*" "%SampleTestFolder%"
	xcopy /y /i /s "%CoreSampleFiles%\*" "%SampleTestFolder%"
	xcopy /y /i /s "%GeometrySampleFiles%\*" "%SampleTestFolder%"
	xcopy /y /i /s "%ImportExportSampleFiles%\*" "%SampleTestFolder%"
	xcopy /y /i /s "%RevitSampleFiles%\*" "%SampleTestFolder%"
	xcopy /y /i /s "%SampleDataFiles%\*.*" "%SampleTestFolder%"
)
