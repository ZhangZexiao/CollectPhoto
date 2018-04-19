del *.stderr *.orig
rd /Q /S obj
for /f "tokens=1-4 delims=/ " %%i in ("%date%") do (
     set year=%%i
     set month=%%j
     set day=%%k
     set week=%%l
)
for /f "tokens=1-4 delims=:. " %%i in ("%time%") do (
     set h=%%i
     set m=%%j
     set s=%%k
     set mili=%%l
)
set datestr=%year%_%month%_%day%
set timestr=%h%_%m%_%s%_%mili%
"C:\Program Files\7-Zip\7z.exe" a -r -i!".\bin\x64\Release\*.dll" ..\..\CollectPhotoRelease_x64_%datestr%_%timestr%.7z
"C:\Program Files\7-Zip\7z.exe" a -i!".\bin\x64\Release\CollectPhoto.exe" ..\..\CollectPhotoRelease_x64_%datestr%_%timestr%.7z
"C:\Program Files\7-Zip\7z.exe" a -r -i!".\bin\x86\Release\*.dll" ..\..\CollectPhotoRelease_x86_%datestr%_%timestr%.7z
"C:\Program Files\7-Zip\7z.exe" a -i!".\bin\x86\Release\CollectPhoto.exe" ..\..\CollectPhotoRelease_x86_%datestr%_%timestr%.7z
rd /Q /S ..\.klocwork
rd /Q /S ..\.vs
rd /Q /S ..\TagLibSharp\obj
"C:\Program Files\7-Zip\7z.exe" a -r -i!".\..\*" -x!"bin" ..\..\CollectPhotoSolution_%datestr%_%timestr%.7z