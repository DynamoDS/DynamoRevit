@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

:: set the working dir (default to current dir)
set wdir=%cd%
if not (%1)==() set wdir=%1

:: set the file extension (default to vb)
set extension=cs
if not (%2)==() set extension=%2

echo executing transform_all from %wdir%
pushd %wdir%
:: create a list of all the T4 templates in the working dir
dir *.tt /b > t4list.txt

echo the following T4 templates will be transformed:
type t4list.txt

:: Use texttransform.exe from the IDE if it is present, otherwise, use legacy locations. 

:: clear text transform path to undefine it
set TEXTTRANSFORMPATH=

:: use latest vswhere utility to locate in IDE, where it resides now. 
IF EXIST "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" (
   for /f "usebackq tokens=1* delims=: " %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -all`) do (
      if /i "%%i" == "installationPath" set TEXTTRANSFORMPATH="%%j\Common7\IDE\TextTransform.exe"
   )
)

:: not found in IDE, use legacy
IF NOT DEFINED TEXTTRANSFORMPATH (
   :: Try to determine VisualStudioVersion if not set (common in Jenkins builds)
   IF "%VisualStudioVersion%"=="" (
      :: Default to a common version, or try to detect from installed VS
      set VisualStudioVersion=17.0
   )
   set TEXTTRANSFORMPATH="%COMMONPROGRAMFILES(x86)%\microsoft shared\TextTemplating\%VisualStudioVersion%\TextTransform.exe"
   :: If still not found, try other common versions
   IF NOT EXIST %TEXTTRANSFORMPATH% (
      set TEXTTRANSFORMPATH="%COMMONPROGRAMFILES(x86)%\microsoft shared\TextTemplating\16.0\TextTransform.exe"
   )
   IF NOT EXIST %TEXTTRANSFORMPATH% (
      set TEXTTRANSFORMPATH="%COMMONPROGRAMFILES(x86)%\microsoft shared\TextTemplating\15.0\TextTransform.exe"
   )
)

:: Verify TextTransform.exe exists before attempting transformation
IF NOT DEFINED TEXTTRANSFORMPATH (
   echo ERROR: TextTransform.exe not found. Cannot transform T4 templates.
   echo Searched for Visual Studio TextTransform.exe but could not locate it.
   exit /b 1
)
IF NOT EXIST %TEXTTRANSFORMPATH% (
   echo ERROR: TextTransform.exe not found at: %TEXTTRANSFORMPATH%
   echo This is required for T4 template transformation. Please ensure Visual Studio is installed.
   exit /b 1
)

:: transform all the templates
set TRANSFORM_FAILED=0
for /f %%d in (t4list.txt) do (
set file_name=%%d
set file_name=!file_name:~0,-3!.%extension%
echo:  \--^> !file_name!
echo %TEXTTRANSFORMPATH% -out !file_name! %%d
    %TEXTTRANSFORMPATH% -out !file_name! %%d
    IF ERRORLEVEL 1 (
        echo ERROR: Failed to transform %%d
        set TRANSFORM_FAILED=1
    )
)

IF %TRANSFORM_FAILED%==1 (
    echo ERROR: One or more T4 template transformations failed.
    exit /b 1
)

:: delete T4 list and return to previous directory
del t4list.txt
popd

echo transformation complete
