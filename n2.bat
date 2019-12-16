
Rem  *** publish all ***
rem dotnet nuget push *.nupkg -k oy2bbli2ofzblzukwr4afcrcttpd2t7evkanccpnkawltu -s https://api.nuget.org/v3/index.json
dotnet nuget push *.nupkg -k MossIsTheBoss -s http://nugets.azurewebsites.net/nuget 
del *.nupkg
pause
