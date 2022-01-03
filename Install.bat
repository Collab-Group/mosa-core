@if (@CodeSection == @Batch) @then

@echo off

:init
 setlocal DisableDelayedExpansion
 set cmdInvoke=1
 set winSysFolder=System32
 set "batchPath=%~0"
 for %%k in (%0) do set batchName=%%~nk
 set "vbsGetPrivileges=%temp%\OEgetPriv_%batchName%.vbs"
 setlocal EnableDelayedExpansion

:checkPrivileges
  NET FILE 1>NUL 2>NUL
  if '%errorlevel%' == '0' ( goto gotPrivileges ) else ( goto getPrivileges )

:getPrivileges
  if '%1'=='ELEV' (echo ELEV & shift /1 & goto gotPrivileges)

  ECHO Set UAC = CreateObject^("Shell.Application"^) > "%vbsGetPrivileges%"
  ECHO args = "ELEV " >> "%vbsGetPrivileges%"
  ECHO For Each strArg in WScript.Arguments >> "%vbsGetPrivileges%"
  ECHO args = args ^& strArg ^& " "  >> "%vbsGetPrivileges%"
  ECHO Next >> "%vbsGetPrivileges%"

  if '%cmdInvoke%'=='1' goto InvokeCmd 

  ECHO UAC.ShellExecute "!batchPath!", args, "", "runas", 1 >> "%vbsGetPrivileges%"
  goto ExecElevation

:InvokeCmd
  ECHO args = "/c """ + "!batchPath!" + """ " + args >> "%vbsGetPrivileges%"
  ECHO UAC.ShellExecute "%SystemRoot%\%winSysFolder%\cmd.exe", args, "", "runas", 1 >> "%vbsGetPrivileges%"

:ExecElevation
 "%SystemRoot%\%winSysFolder%\WScript.exe" "%vbsGetPrivileges%" %*
 exit /B

:gotPrivileges
 setlocal & cd /d %~dp0
 if '%1'=='ELEV' (del "%vbsGetPrivileges%" 1>nul 2>nul  &  shift /1)

:: DEFS
set ESC=
set app="%ProgramFiles%\MOSA-Core"
:: END DEFS

:: LOCATE VS

echo Locating VS

set "_Key=HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders"

for /f tokens^=3 %%i in ('%__APPDIR__%reg.exe query "!_Key!"^|find/i "Personal"')do <con: call set "_docs_folder=%%~i"
for /F "tokens=* USEBACKQ" %%F in (
    `"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -property catalog.productLineVersion`
) do (set VSVer=%%F)

set VSDoc="!_docs_folder!\Visual Studio %VSVer%"

if not exist %VSDoc% (
    echo Visual Studio is not installed, aborting
    pause
exit
)
if %errorlevel%==1 (
    echo Visual Studio is not installed, aborting
    pause
    exit
)
:: MAIN

set numOpts=0
for %%a in (INSTALL UNINSTALL UPDATE) do (
   set /A numOpts+=1
   set "option[!numOpts!]=%%a"
)
set /A numOpts+=1
set "option[!numOpts!]=QUIT"

doskey /REINSTALL
cscript //nologo /E:JScript "%~F0" EnterOpts
for /L %%i in (1,1,%numOpts%) do set /P "opt="

cls

cscript //nologo /E:JScript "%~F0"
set /P "opt=Select the desired task: "
cls

if "%opt%"=="QUIT" exit
if "%opt%"=="INSTALL" goto :Install
if "%opt%"=="UNINSTALL" goto :Uninstall
if "%opt%"=="UPDATE" goto :Update

:Install

if exist %app% goto :Update

echo Installing MOSA-Core

if exist "bin\NUL" (
    echo.
    echo Removing Bin Folder
    rd /s /q "bin"
)

cd src

goto :LocateMSBuild

:LocateMSBuild

echo.
echo Locating MSBuild

::pushd "%~d0%~p0"
for /F "tokens=* USEBACKQ" %%F in (
    `"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`
) do (SET msbuild=%%F)
::popd
goto :BuildMOSA

:BuildMOSA
echo.
echo Building MOSA

cd Mosa

dotnet restore

"%msbuild%" Mosa.sln /t:Build /p:Configuration=Debug;Platform="Mixed Platforms" -m
cd ..

:BuildCompiler
echo.
echo Building MOSA Compiler
cd Compiler

dotnet restore

"%msbuild%" Compiler.sln /t:Build /p:Configuration=Debug;Platform="Mixed Platforms" -m
cd ..

echo.
if "%errorlevel%" == "1" (
echo %ESC%[31mCompilation Failed!%ESC%[0m
echo.
pause
exit
) else (
echo %ESC%[32mSuccessful Compilation!%ESC%[0m
echo.
)

set /P VSIntergration="Install Visual Studio Intergration? ( y/n )  "
if /I %VSIntergration%=="y" goto :InstallVSIntergration
if /I %VSIntergration%=="n" goto :Files
goto :Files

:InstallVSIntergration
echo.
echo Installing VS Intergration

if not exist %VSDoc%\ProjectTemplates\Mosa Project"\NUL" mkdir %VSDoc%\ProjectTemplates\"Mosa Project"\
xcopy Templates\Mosa.VisualStudio.ProjectTemplate\*.* %VSDoc%\ProjectTemplates\"Mosa Project"\
if not exist %VSDoc%\ProjectTemplates\Mosa Project\Properties"\NUL" mkdir %VSDoc%\ProjectTemplates\"Mosa Project"\Properties\
xcopy Templates\Mosa.VisualStudio.ProjectTemplate\Properties\*.*"\NUL" %VSDoc%\ProjectTemplates\"Mosa Project"\Properties\

if not exist %VSDoc%\ProjectTemplates\Mosa Project GUI"\NUL" mkdir %VSDoc%\ProjectTemplates\"Mosa Project GUI"
xcopy Templates\Mosa.VisualStudio.GUI.ProjectTemplate\*.* %VSDoc%\ProjectTemplates\Mosa Project GUI
if not exist %VSDoc%\ProjectTemplates\Mosa Project GUI\Properties"\NUL" mkdir %VSDoc%\ProjectTemplates\"Mosa Project GUI"\Properties
xcopy Templates\Mosa.VisualStudio.GUI.ProjectTemplate\Properties\*.* %VSDoc%\ProjectTemplates\"Mosa Project GUI"\Properties

:Files

echo.
echo Installing...

if not exist %app%"\NUL" mkdir %app%
if not exist %app%\bin"\NUL" mkdir %app%\bin
xcopy bin\* %app%\bin /s /E
if not exist %app%\ASM"\NUL" mkdir %app%\ASM
xcopy ASM\*.o %app%\ASM /s /E
if not exist %app%\Tools\nasm"\NUL" mkdir %app%\Tools\nasm
xcopy Tools\nasm\* %app%\Tools\nasm /s /E
if not exist %app%\Tools\ndisasm"\NUL" mkdir %app%\Tools\ndisasm
xcopy Tools\ndisasm %app%\Tools\ndisasm /s /E
if not exist %app%\Tools\mkisofs"\NUL" mkdir %app%\Tools\mkisofs
xcopy Tools\mkisofs %app%\Tools\mkisofs /s /E
if not exist %app%\Tools\syslinux"\NUL" mkdir %app%\Tools\syslinux
xcopy Tools\syslinux %app%\Tools\syslinux /s /E
if not exist %app%\Tools\virtualbox"\NUL" mkdir %app%\Tools\virtualbox
xcopy Tools\virtualbox %app%\Tools\virtualbox /s /E
if not exist %app%\Tools\vmware"\NUL" mkdir %app%\Tools\vmware
xcopy Tools\vmware %app%\Tools\vmware /s /E
if not exist %app%\Tools\imdisk"\NUL" mkdir %app%\Tools\imdisk
xcopy Tools\imdisk %app%\Tools\imdisk /s /E
if not exist %app%\Tools\grub2"\NUL" mkdir %app%\Tools\grub2
xcopy Tools\grub2 %app%\Tools\grub2 /s /E

echo.
if "%errorlevel%" == "1" (
echo %ESC%[31mInstallation Failed!%ESC%[0m
echo.
pause
exit
) else (
echo %ESC%[32mSuccessful Installation!%ESC%[0m
echo.
)
pause
exit

:Uninstall

echo.
echo Uninstalling MOSA-Core

if exist %app% rd /s /q %app%
if exist %VSDoc%\ProjectTemplates\"Mosa Project" rd /s /q %VSDoc%\ProjectTemplates\"Mosa Project"
if exist %VSDoc%\ProjectTemplates\"Mosa Project GUI" rd /s /q %VSDoc%\ProjectTemplates\"Mosa Project GUI"

if "%errorlevel%" == "1" (
echo %ESC%[31mUninstallation Failed!%ESC%[0m
echo.
pause
exit
) else (
echo %ESC%[32mSuccessful Uninstallation!%ESC%[0m
echo.
)

pause
exit

:Update

echo.
echo Updating MOSA-Core to source
echo.
echo This may be because you tried to install when already installed
echo.

if exist %app% rd /s /q %app%
if exist %VSDoc%\ProjectTemplates\"Mosa Project" rd /s /q %VSDoc%\ProjectTemplates\"Mosa Project"
if exist %VSDoc%\ProjectTemplates\"Mosa Project GUI" rd /s /q %VSDoc%\ProjectTemplates\"Mosa Project GUI"

if not exist %app%"\NUL" mkdir %app%
if not exist %app%\bin"\NUL" mkdir %app%\bin
xcopy bin\* %app%\bin /s /E
if not exist %app%\ASM"\NUL" mkdir %app%\ASM
xcopy ASM\*.o %app%\ASM /s /E
if not exist %app%\Tools\nasm"\NUL" mkdir %app%\Tools\nasm
xcopy Tools\nasm\* %app%\Tools\nasm /s /E
if not exist %app%\Tools\ndisasm"\NUL" mkdir %app%\Tools\ndisasm
xcopy Tools\ndisasm %app%\Tools\ndisasm /s /E
if not exist %app%\Tools\mkisofs"\NUL" mkdir %app%\Tools\mkisofs
xcopy Tools\mkisofs %app%\Tools\mkisofs /s /E
if not exist %app%\Tools\syslinux"\NUL" mkdir %app%\Tools\syslinux
xcopy Tools\syslinux %app%\Tools\syslinux /s /E
if not exist %app%\Tools\virtualbox"\NUL" mkdir %app%\Tools\virtualbox
xcopy Tools\virtualbox %app%\Tools\virtualbox /s /E
if not exist %app%\Tools\vmware"\NUL" mkdir %app%\Tools\vmware
xcopy Tools\vmware %app%\Tools\vmware /s /E
if not exist %app%\Tools\imdisk"\NUL" mkdir %app%\Tools\imdisk
xcopy Tools\imdisk %app%\Tools\imdisk /s /E
if not exist %app%\Tools\grub2"\NUL" mkdir %app%\Tools\grub2
xcopy Tools\grub2 %app%\Tools\grub2 /s /E

if "%errorlevel%" == "1" (
echo %ESC%[31mUpdate Failed!%ESC%[0m
echo.
pause
exit
) else (
echo %ESC%[32mSuccessful Update!%ESC%[0m
echo.
)

pause
exit
@end

var wshShell = WScript.CreateObject("WScript.Shell"),
    envVar = wshShell.Environment("Process"),
    numOpts = parseInt(envVar("numOpts"));

if ( WScript.Arguments.Length ) {
   // Enter menu options
   for ( var i=1; i <= numOpts; i++ ) {
      wshShell.SendKeys(envVar("option["+i+"]")+"{ENTER}");
   }
} else {
   // Enter a F7 to open the menu
   wshShell.SendKeys("{F7}{HOME}");
}