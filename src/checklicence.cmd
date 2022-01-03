@echo off

echo.
echo CHECKING FOR UNLICENCED FILES

for /f %%f in ('dir /b /s .\*.cs') do (
    @REM if not "%result%"=="// Copyright (c) MOSA Project. Licensed under the New BSD License." (
        @REM echo %%a
    @REM )
)

pause
exit