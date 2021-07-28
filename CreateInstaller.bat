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

echo Removing bin folder
rd /s /q bin

cd Source

echo Restoring packages
dotnet restore

echo Building MOSA

pushd "%~d0%~p0"
FOR /F "tokens=* USEBACKQ" %%F IN (
    `"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`
) DO (SET msbuild=%%F)
popd

"%msbuild%" Mosa.sln /t:Build /p:Configuration=Debug;Platform="Mixed Platforms" -m

echo Creating installer
cd Inno-Setup-Script
create-installer.bat
