NET STOP "OJS Local Worker Service"
sc delete "OJS Local Worker Service"
timeout 5
CD %~dp0
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil "..\OJS.Workers.LocalWorker.exe"
sc failure "OJS Local Worker Service" actions= restart/60000/restart/60000/""/60000 reset= 7200
NET START "OJS Local Worker Service"
pause