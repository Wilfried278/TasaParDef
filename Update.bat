@echo on
cls
REM
REM ---------------------------------------------------------------------------
REM
REM	Revisionverwaltung f�r CRevisionsInfo.cs und AssmblyInfo.cs
REM	Mittels SVN werden die beiden Templatefiles (CRevisionsInfo.cs im Projekt-
REM	Verzeichnis und AssemblyInformtion.cs im Properties-Verzeichnis) mit
REM	der aktuellen Revision aktualisiert. 
REM
REM	Achtung: 	Es ist darauf zu achten, dass die Haupt- und Unterversion von
REM						Hand in den beiden Templatefiles gepflegt werden!
REM 
REM ---------------------------------------------------------------------------
REM
REM	
set exedir="C:\Program Files\TortoiseSVN\bin\"
REM ---Windows XP auf VMZilger --- set exedir=C:\Programme\TortoiseSVN\bin\
set ProjektPfad=%cd%

%exedir%TortoiseProc /Command:commit /Path:%ProjektPfad% /logmsg:"Compilerlauf" /notempfile /closeonend
pause