@echo off
set zielpfad=D:\Tagebaue\PCZ-DM\VCS2010\Auslieferungen\Tasapardef\HotUpdate
set quellpfad=%1
if not "%2" == "Release" goto labelEnde
copy %quellpfad%bin\Release\TasaParDef.exe %zielpfad%\*.*
if %errorlevel%==0 goto labelEnde
pause
:labelEnde

