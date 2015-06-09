@echo off
SET CURRENT_DIRECTORY=%CD%
cd %CURRENT_DIRECTORY%
"c:\mingw\bin\gcc.exe" -D DEBUG -I"%CURRENT_DIRECTORY%" -L"%CURRENT_DIRECTORY%" -Wall *.c -o arss.exe -l libfftw3-3
