@echo off
echo --- Removing previous installer...
del bin\MOSA-Installer.exe /q

echo --- Building MOSA...
cd Source
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe" Mosa.sln /t:Build /p:Configuration=Debug;Platform="Mixed Platforms"

echo --- Creating installer...
cd Inno-Setup-Script
create-installer.bat
pause
