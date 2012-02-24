
1. Install MinGW

2. Copy the fftw.h file into C:\MinGW\include

3. Copy the fftw dll into C:\MinGW\lib (libfftw3-3.dll ++)

4. Compile with
gcc -Wall *.c -o arss.exe -l libfftw3-3

5. Notepad++ Script (NppExec) - F6
cd $(CURRENT_DIRECTORY)
gcc -Wall *.c -o arss.exe -l libfftw3-3