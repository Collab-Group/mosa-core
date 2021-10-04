@echo off
"C:\Program Files (x86)\MOSA-Project\bin\Mosa.Launcher.Console.exe" "%cd%\%1%" -VBE
::Add Your Commands Here
::For more https://github.com/nifanfa/MOSA-Core/wiki
::The work dir is /bin so you want to add your bats here you have to use cd ..\Properties
::Arguments:
::Arguments 1 Is The Input File
::-VBE (Enable VBE)
::-JUSTBUILD (Tell Compiler Do Not Launch VirtualBox After Compiling)
exit