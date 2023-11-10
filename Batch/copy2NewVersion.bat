REM
REM ---------------------------------------------------------------------------
REM Kopieren der Releaseversion in ein Verzeichnis unterhalb des 
REM Auslieferungsverzeichnisses D:\Tagebaue\PBZ\PBZ-VM\VCS2008\Auslieferungen
REM \PBZ\<projekt>\NewVersion\files. 
REM 
REM ---------------------------------------------------------------------------
REM  

@echo on
cls

REM -----------------------------------------------------------
REM Pfad und Konfiguration (DEBUG/RELEASE) aus den Aufruf-
REM argumenten umkopieren
REM -----------------------------------------------------------

    REM Der Quellpfad endet mit einem \ (aus ProjektDir VS2008)
    REM 
		set quellpfad=%1
		set zielpfad=D:\Tagebaue\PCZ-DM\VCS2010\Auslieferungen\Tasapardef\NewVersion\files

REM -----------------------------------------------------------
REM Nur bei Release wird eine neue Version in das NewVersion
REM	Verzeichnis erstellt
REM -----------------------------------------------------------

		if not "%2" == "Release" goto labelEnde
		
		IF EXIST %zielpfad% GOTO DIREXIST

REM -----------------------------------------------------------
REM Verzeichnis erstellen falls existiert
REM -----------------------------------------------------------

		md ..\%zielpfad%

:DIREXIST

REM -----------------------------------------------------------
REM evtl. vorhandene Dateien zuerst löschen
REM -----------------------------------------------------------

		echo J | del /s /q %zielpfad%\*.*

REM -----------------------------------------------------------
REM Diese Dateien kopieren
REM -----------------------------------------------------------
echo on
		xcopy 	/Y %quellpfad%bin\Release\*.exe %zielpfad%\*.*
		xcopy 	/Y %quellpfad%bin\Release\*.xml %zielpfad%\*.*
		
		xcopy 	/Y %quellpfad%*.ico %zielpfad%\*.*

echo 		xcopy 	/Y %quellpfad%Dokumentation\ReleaseNotes.txt %zielpfad%\*.* >> 12345.lst
		xcopy 	/Y %quellpfad%Dokumentation\ReleaseNotes.txt %zielpfad%\*.*
echo off
REM -----------------------------------------------------------
REM spezielle Dateien kopieren
REM -----------------------------------------------------------
		
REM 		xcopy 	/Y %quellpfad%batch\hotupdate.bat %zielpfad%\*.*
REM 		xcopy 	/Y %quellpfad%bin\Release\*.teil2 %zielpfad%\*.*
		del %zielpfad%*.vshost.exe
:labelEnde


REM -----------------------------------------------------------
REM Ende von copy2NewVersion.bat
REM -----------------------------------------------------------
pause