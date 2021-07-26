@echo off
echo --- Removing bin folder...
rd /s /q bin

echo --- Building MOSA...
cd Source

pushd "%~d0%~p0"
FOR /F "tokens=* USEBACKQ" %%F IN (
    `"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`
) DO (SET msbuild=%%F)
popd

"%msbuild%" Mosa.sln /t:Build /p:Configuration=Debug;Platform="Mixed Platforms"

echo --- Creating installer...
cd Inno-Setup-Script
create-installer.bat
pause