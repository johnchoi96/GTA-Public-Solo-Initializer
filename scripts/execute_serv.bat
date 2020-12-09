:: Change the path ni[B][B]a
cd %USERPROFILE%
cd AppData
cd Local
cd Temp
cd file.zip.extracted

:: take a pause ni[B][B]a
.\pssuspend.exe gta5

:: pause for 10 seconds
PING localhost -n 11 > NUL
.\pssuspend.exe gta5 -r

PAUSE
EXIT

