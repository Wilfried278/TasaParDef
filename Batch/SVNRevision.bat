REM 
REM 	Stapelverarbeirtungsdatei muss ich in einem Verzeichnis
REM 	uinterhalb des Projektverzeichnisses befinden. Diese
REM		liegt �berlicherweise in ..\Batch\SVNRevision.bat
REM 
		@echo off
cls
REM 
REM		Tortoise Programme und Pfad setzen
REM
		set exedir="C:\Program Files\TortoiseSVN\bin"
		set exefile= %exedir%"\subwcrev.exe"
REM
REM		In das �bergeordnete Verzeichnis wechseln, denn das ist der
REM 	Verzeichnnis mit dem Namen des Projekrarchivs
REM 
		cd ..
		set projekt=%CD%
		%exefile%  %projekt% %projekt%\CRevisionsInfo.tpl %projekt%\CRevisionsInfo.cs
		
pause
