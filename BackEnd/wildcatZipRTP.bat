Rem This script will create a zip folder for RTP
Rem Copy to [repo]\BackEnd and run from source location
Rem Update gameId value to your game name
Rem Compile your game before running (assumes net7.0)
Rem Search order for build is RTP, RTP_Debug (if you want to skip ahead you must delete in [repo]\BackEnd\[gameId]\bin)

echo off
set gameId=GAMEID
set netVersion=net7.0

set source1=%CD%\%gameId%\bin\RTP\%netVersion%
set source2=%CD%\%gameId%\bin\RTP_Debug\%netVersion%
set source="%source1%"

set msg=ERROR: 'RTP' or 'RTP_Debug' must exist in '%gameId%\bin\' and contain a '%netVersion%' folder.
if exist %source1% (
    set msg=RTP build exists at '%gameId%\bin\'. Press enter to generate 'RTP\%gameId%.zip.'
) else (
    if exist %source2% (
        set msg=RTP_Debug build exists at '%gameId%\bin\'. Press enter to generate 'RTP\%gameId%.zip'.
    )
    set source="%source2%"
)
set /p key="%msg%"

set zip=%CD%\RTP\%gameId%.zip
set temp=%CD%\RTP\%gameId%

IF EXIST "%temp%" (rmdir /S /Q "%temp%")
IF EXIST "%zip%" (del "%zip%")

xcopy /E /I "%source%" "%temp%" 

del "%temp%\Refit*"
del "%temp%\*.deps.json"
del "%temp%\Newtonsoft.Json.dll"
del "%temp%\Microsoft.*"
del "%temp%\Serilog.*"
del "%temp%\System.*"
del "%temp%\NLog.*"

rmdir /S /Q "%temp%\runtimes"

"C:\Program Files\7-Zip\7z.exe" a -tzip "%zip%" "%temp%"

rmdir /S /Q "%temp%"

set /p key="Done. Find zip file in RTP folder. Hit enter to exit."