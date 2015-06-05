
1. Install MinGW

Best way:
2. Notepad++ Script (NppExec) - F6
Add the following commands to the window:
cd $(CURRENT_DIRECTORY)
"c:\mingw\bin\gcc.exe" -I"$(CURRENT_DIRECTORY)" -L"$(CURRENT_DIRECTORY)" -Wall *.c -o arss.exe -l libfftw3-3

OLD way:

2. Copy the fftw3.h file into C:\MinGW\include

3. Copy the fftw dll into C:\MinGW\lib (libfftw3-3.dll)

4. Compile with
gcc -Wall *.c -o arss.exe -l libfftw3-3

5. Notepad++ Script (NppExec) - F6
cd $(CURRENT_DIRECTORY)
gcc -Wall *.c -o arss.exe -l libfftw3-3
