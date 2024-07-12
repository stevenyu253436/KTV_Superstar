@echo off
setlocal enabledelayedexpansion

:: Initialize the references part of the command
set refs=

:: Read each line from the references file and append it to the refs variable
for /F "tokens=*" %%a in (references.txt) do (
    set refs=!refs! /reference:%%a
)

:: Initialize the sources part of the command
set sources=

:: Recursively read each C# source file in the current directory and subdirectories
for /R %%f in (*.cs) do (
    set sources=!sources! "%%f"
)

:: Now call the csc.exe compiler with the constructed command
csc.exe /out:D:\KTV_SuperStar\DualScreenKTVPlayStation.exe /win32icon:ksonglover.ico %refs% %sources%

endlocal