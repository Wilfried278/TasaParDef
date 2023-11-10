@echo on
cls
set quellPfad=%1
set konfiguration=%2


set startverzeichnis=%~dp0
call %startverzeichnis%kopiere2HotUpdate.bat %quellpfad% %konfiguration%
call %startverzeichnis%copy2NewVersion.bat %quellpfad% %konfiguration%
