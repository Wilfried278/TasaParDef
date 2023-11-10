@echo on
cls
REM
REM ---------------------------------------------------------------------------
REM
REM	Revisionverwaltung für CRevisionsInfo.cs und AssmblyInfo.cs
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
		set ProjektPfad=%cd%
		
		set svnexepfad="C:\Program Files\TortoiseSVN\bin\"
REM
REM ---------------------------------------------------------------------------
REM	Update der Klasse CRevisionInfo.cs
REM ---------------------------------------------------------------------------
REM		
		%svnexepfad%subwcrev  %projektpfad% %projektpfad%\CRevisionsInfo.tpl %projektpfad%\CRevisionsInfo.cs

REM
REM ---------------------------------------------------------------------------
REM	Update der AssemblyInfomation.cs
REM ---------------------------------------------------------------------------
REM 

		%svnexepfad%subwcrev  %projektpfad% %projektpfad%\Properties\AssemblyInfo.tpl %projektpfad%\Properties\AssemblyInfo.cs


pause
