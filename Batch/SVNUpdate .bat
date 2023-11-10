REM 
REM 	Stapelverarbeirtungsdatei muss ich in einem Verzeichnis
REM 	uinterhalb des Projektverzeichnisses befinden. Diese
REM		liegt überlicherweise in ..\Batch\SVNUpdate.bat
REM 
		@echo off
		cls
REM 
REM		Tortoise Programme und Pfad setzen
REM
		set exedir="C:\Program Files\TortoiseSVN\bin"
		set exefile= %exedir%"\TortoiseProc.exe"
REM
REM		In das Übergeordnete Verzeichnis wechseln, denn das ist der
REM 	Verzeichnnis mit dem Namen des Projekrarchivs
REM 
		cd ..
		set projektdir=%CD%
		%exefile% /Command:commit /Path:%projektdir% /logmsg:"Compilerlauf"  /closeonend:1
