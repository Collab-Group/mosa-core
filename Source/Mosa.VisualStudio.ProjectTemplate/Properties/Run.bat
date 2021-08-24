@echo off
start "" "C:\Program Files (x86)\MOSA-Project\bin\Mosa.Launcher.Console.exe" "%cd%\%1%" "" False
::Add Your Commands Here
::Add JustBuild at the end can makes the compiler not launch virtualbox after compiling
::For more https://github.com/nifanfa/MOSA-Core/wiki
::The work dir is /bin so you want to add your bats here you have to use cd ..\Properties
::To enable VBE use start "" "C:\Program Files (x86)\MOSA-Project\bin\Mosa.Launcher.Console.exe" "%cd%\%1%" "" True
exit