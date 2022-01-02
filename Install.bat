@echo off
set INNOSETUP="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if exist %INNOSETUP% (
echo Inno Setup Found
) else (
echo Inno Setup Not Found
echo Here To Download Inno Setup
echo https://jrsoftware.org/download.php/is.exe
pause
exit
)

cd src

echo Removing bin folder
rd /s /q bin

echo Locating MSBuild

pushd "%~d0%~p0"
FOR /F "tokens=* USEBACKQ" %%F IN (
    `"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`
) DO (SET msbuild=%%F)
popd

echo Building MOSA

cd Mosa

echo Restoring packages
dotnet restore

"%msbuild%" Mosa.sln /t:Build /p:Configuration=Debug;Platform="Mixed Platforms" -m
cd ..

echo Building Compiler

cd Compiler

echo Restoring packages
dotnet restore

"%msbuild%" Compiler.sln /t:Build /p:Configuration=Debug;Platform="Mixed Platforms" -m
cd ..

echo.
if "%errorlevel%" == "1" (
echo [31mCompilation Failed![0m
echo.
pause
exit
) else (
echo [32mSuccessful Compilation![0m
echo.
)

echo Creating installer
cd Inno-Setup-Script
create-installer.bat