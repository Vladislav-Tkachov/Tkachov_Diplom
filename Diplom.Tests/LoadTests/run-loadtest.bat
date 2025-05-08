@echo off
rem === go to the folder that contains this .bat ===
pushd "%~dp0"
rem === CONFIG ===
set "JMX=SignalR_Load.jmx"
set "OUT=results.jtl"
set "REP=report"
set "HEAP=-Xms2g -Xmx4g"

rem === JAVA HEAP for 10k users (4 GB) ===
set "HEAP=-Xms2g -Xmx4g"

rem === RUN ===
echo Running JMeter...
"D:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -f ^ -t "%JMX%" -l "%OUT%" -q jmeter.properties -e -o "%REP%"
if errorlevel 1 (
  echo JMeter exited with an error. See console for details.
  pause
  exit /b 1
)

echo.
echo HTML-report ➜ "%REP%\index.html"
pause
