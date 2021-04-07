start "valuator1" /d ..\Valuator\ dotnet run --no-build --urls http://*:5001
start "valuator2" /d ..\Valuator\ dotnet run --no-build --urls http://*:5002

start "nginx" /d ..\nginx\ nginx.exe

start "rankcalculator1" /d ..\RankCalculator\ dotnet run --no-build
start "rankcalculator2" /d ..\RankCalculator\ dotnet run --no-build

start "eventslogger1" /d ..\EventsLogger\ dotnet run --no-build
start "eventslogger2" /d ..\EventsLogger\ dotnet run --no-build