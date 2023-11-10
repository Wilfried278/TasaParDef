using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TasaParDef
{
  public class CDateinamen
  {

    #region Datenelemente_Variablen

    // Pfade enden immer OHNE Backslash
    //
    private string headerDateinamePfad; // kompletter Pfad incl. Datei
    private string headerDateiname;     // nur Dateiname
    private string projektName;         // ist der Headerdateiname ohne Erweiterung
    private string tidDateiname;
    private string fehlerAnzeigenlistenDateiname;
    private string bestätigungslistennDateiname;
    
    private string tasaParDateiname;
    private string tasaParBackupDateiname;

    private string saaParDateiname;
    private string saaParBackupDateiname;

    private string aktuellerDatenpfad;
    private string programmPfad;
    private string saaApplikationsPfad;
    private string tasaApplikationsPfad;
    //
    // QST: 2016052300
    //
    //
    //  23.05.2016 neu eingeführt
    //
    private string saaUserDatenPfad;
    private string tasaUserDatenPfad;
    //
    //
    private string saaApplikationsName;
    private string tasaApplikationsName;

    private string saaProgramm;
    private string tasaProgramm;

    private string drahtbruchlisteHeben;
    private string drahtbruchlisteSenken;

    private bool zweiSeilgerät;
    private bool vierSeilgerät;

    private string projektnameNeu;
    private string projektNameOhneExtension;

    /// <summary>
    /// Wurden die Klasssendaten über den Headerdateinamen
    /// erzeugt, also sind die Daten alle gesetzt?
    /// 
    /// </summary>
    private bool erforderlicheDatenOK; // wurden die Daten über den
    #endregion // Datenelemente

    #region Datenelemente_Konstanten


    /// <summary>
    /// Erweiterung des Namen der Parameterdatei für TASA dieser muss konstant sein. Da das Pascal-Programm 
    /// von Udo Denzer diesen Dateinamen erwartet
    /// </summary>
    private const string erwTasaParDateiname = "Tasa_laser.par";

    /// <summary>
    /// Erweiterung des Namen der Parameterdatei für SAA dieser muss konstant sein. Da das Pascal-Programm 
    /// von Udo Denzer diesen Dateinamen erwartet
    /// </summary>
    private const string erwSaaBackupParDateiname = "_saaproRegr.par";

    /// <summary>
    /// Namen der Parameterdatei für SAA dieser muss konstant sein. Da das Pascal-Programm 
    /// von Udo Denzer diesen Dateinamen im Programmverezeichnis von SAA erwartet
    /// </summary>
    private const string erwSaaParDateiname = "saaproRegr.par";

    /// <summary>
    /// Unveränderlicher (unterer) Teil der TASA_Laser.par (wird nur angehangen) 
    /// </summary>
    private const string tasaParDateiTeil2 = "TASA_Laser_par.Teil2";
    
    private string tasaParDateiNameTeil2;

    /// <summary>
    /// Batchdateiname zum Start von TASA
    /// </summary>
    private const string erwTasaBatchDateiname = "Start_tasapro_laser.bat";

    /// <summary>
    /// Batchdateiname zum Start von SAA
    /// </summary>
    private const string erwSaaBatchDateiname = "Start_saaproRegr.bat";

    /// <summary>
    /// Erweiterung für den Ausgabedateinamen der bestätigten Fehlerliste
    /// </summary>
    private const string ergebnisFehlerlistenName = "_Fehler.txt";

    /// <summary>
    /// Erweiterung für den Ausgabedateinamen der bestätigten Fehlerliste als Importdatei für Word
    /// </summary>
    private const string fehlerlistenNameWord = "_Fehler_WordImport.txt";

    private const string fehlerlistenName = "_Seil_Fehleranzeigen.txt";

    /// <summary>
    /// Erweiterung für den Ausgabedateinamen der Fehleriste zur grafischen Darstellung
    /// </summary>
    private const string diagrammFileExcel6D = "_Grafik_6D.xls";
    private const string diagrammFileExcel30D = "_Grafik_30D.xls";

    /// <summary>
    /// Vorlage zur grafischen Darstellung der Seilfehler, diese liegt im 
    /// Startup-Verezichnis der Applikation (normalerweise in 
    /// C:\Programme\RWEPower\TasaParDef
    /// </summary>
    private const string excelVorlagenSeilfehler = "Seilfehler_Grafik_Vorlage.xls";

    /// <summary>
    /// In dieser Datei landen die Informationen aus dem 
    /// TASA-Lauf (TID=TasaInformationsDatei)
    /// </summary>
    private const string erwTasaInfoDateiname = ".TID";

  

    #endregion // Datenelemente_Konstanten

    #region Konstruktion und Initialisierung

    public CDateinamen()
    {
      initialisierung();
    }


    
    private void initialisierung()
    {
      headerDateinamePfad = "";
      headerDateiname = "";
      projektName = "";   // ist der Headerdateiname ohne Erweiterung
      tidDateiname = "";
      fehlerAnzeigenlistenDateiname = "";
      bestätigungslistennDateiname = "";

      tasaParDateiname = "";
      saaParDateiname = "";
      aktuellerDatenpfad = "";
      programmPfad = "";
      saaApplikationsPfad = "";
      tasaApplikationsPfad = "";

      saaApplikationsName = "";
      tasaApplikationsName = "";

      saaProgramm = "";
      tasaProgramm = "";

      zweiSeilgerät = false;
      vierSeilgerät = false;
      projektNameOhneExtension = "";

      erforderlicheDatenOK = false;
    }

    //public CDateinamen(string headerDateinamePfad)
    //{
    //  initialisierung();
    //  this.headerDateinamePfad = headerDateinamePfad;

    //  headerdateinamenSplitten();
    //  tasaInfoDateinamen();
    //}


    public string Headerdateiname
    {
      set
      {
        headerDateinamePfad = value;
        headerdateinamenSplitten();
        tasaInfoDateinamen();
        erforderlicheDatenOK = true;
      }
      get { return headerDateiname; }
    }
    
    private void headerdateinamenSplitten()
    {
      //
      // 21.10.2011  07:53:40
      // 
      // Prüfen !!
      projektnameNeu = gibProjektName(headerDateinamePfad);
      projektNameOhneExtension = Path.GetFileNameWithoutExtension(headerDateinamePfad);
      projektName = Path.GetFileNameWithoutExtension(projektnameNeu);
      aktuellerDatenpfad = Path.GetDirectoryName(headerDateinamePfad);
      headerDateiname = Path.GetFileName(headerDateinamePfad);
      programmPfad = System.Windows.Forms.Application.StartupPath;

    }




    /// <summary>
    /// Projektname ohne Extension
    /// </summary>
    /// <param name="headerDateinamePfad"></param>
    /// <returns></returns>
    private string gibProjektName(string headerDateinamePfad)
    {

      //  Die Ermittlung des Projektnamen erfolgt ab sofort ausschließlich über die Kennung
      //  der Seilposition und der Bewegungsrichtung. Die bisherige Ermittlung des Projektnamen
      //  in dem nach dem Vorkommen ".DDF" sucht  wird verlassen, da eine durchgängige Verarbeitung,
      //  Dateinamen auch ohne Kennung von ".DDF", gewährleistet werden muss.
      //

      string[] prüfvorgänge = { "_LH", "_LS", "_RH", "_RS", "_LAH", "_LAS", "_RAH", "_RAS", "_LIH", "_LIS", "_RIH", "_RIS" };

      zweiSeilgerät = false;
      vierSeilgerät = false;

      int längeVorgang = 0;
      int vorgangPos = -1;
      foreach (string vorgang in prüfvorgänge)
      {
        längeVorgang = vorgang.Length;
        vorgangPos = headerDateinamePfad.IndexOf(vorgang);
        if (!(vorgangPos < 0))
          break;
      }

      if (vorgangPos < 0)
      {
        throw new ArgumentException("Seilposition und Bewegungsrichtung sind aus dem Dateinamen nicht zu ermitteln! ", "_LS;_LH;_RS;_RH;");
      }

      // Bestimmung des Gerätetyps Zweiseiler/Vierseiler
      //
      if (längeVorgang == 3)
      {
        // "_LH", "_LS", "_RH", "_RS",
        zweiSeilgerät = true;
      }
      else if (längeVorgang == 4)
      {
        // "_LAH", "_LAS", "_RAH", "_RAS"
        vierSeilgerät = true;
      }

      
      // Projektname extrahieren 
      // aus projektName = "D:\\DIA\\Seilpruefung\\Daten\\FehlerKraus\\BG_284_LH1.DAT"
      // wird projektName = "D:\\DIA\\Seilpruefung\\Daten\\FehlerKraus\\BG_284_LH1"
      int pos = vorgangPos + längeVorgang + 1;
      string projektName = headerDateinamePfad.Substring(0, pos);

      return projektName;
    }






    public string AktuellerDatenpfad
    {
      get { return aktuellerDatenpfad; }
    }




    #endregion // Kosntruktion und Initialisierung

    #region Funktionen für TASA-Dialog

    
    /// <summary>
    /// Bestimmung des Dateidateinamen für die TASA-Infornmationsdattei
    /// </summary>
    private void tasaInfoDateinamen()
    {
      string dateiMitErweiterung;

      //dateiMitErweiterung = projektName + erwTasaInfoDateiname;
      dateiMitErweiterung = Path.Combine(aktuellerDatenpfad, projektNameOhneExtension);
      tidDateiname = dateiMitErweiterung + erwTasaInfoDateiname;
      

    }




    private void erzeugeSAADateiNamen()
    {
      string dateiMitErweiterung;


      string saadblistenProjektname = gibProjektName(drahtbruchlisteSenken);
      //
      //  für die Ausgabedatei muss aus dem ursprünglichen saadblistenProjektname 
      //  zur Erzeugung der Ausgabedatei, in der alle gültigen Fehler beider Seile
      //  aufgelistet sind, so modifiziert werden, dass aus 
      //  "D:\\DIA\\Seilpruefung\\Daten\\FehlerKraus\\BG_284_LS1" oder 
      //  ein Name für das linke oder rechte Seil wird 
      //  "D:\\DIA\\Seilpruefung\\Daten\\FehlerKraus\\BG_284_L" oder 
      //  denn in dieser Datei sind die gültigen Fehler (durch SAA ermittelt)
      //  die beim Heben und Senken übereinstimmend gefunden wurden und sich daher 
      //  nur auf das Seil und nicht den Vorgang beziehen.

      string seilfehlerAusgabeProjektname = "";

      if (zweiSeilgerät == true)
      {
        int länge = saadblistenProjektname.Length;
        int endPos = länge - 2;

        seilfehlerAusgabeProjektname = saadblistenProjektname.Substring(0, endPos);
      }
      else if (vierSeilgerät == true)
      {
        int länge = saadblistenProjektname.Length;
        int endPos = länge - 3;

        seilfehlerAusgabeProjektname = saadblistenProjektname.Substring(0, endPos);
      }


      // Fehleristenergebnisdatei
      //
      fehlerAnzeigenlistenDateiname = seilfehlerAusgabeProjektname + fehlerlistenName;
      // fehlerAnzeigenlistenDateiname = aktuellerDatenpfad + "\\" + dateiMitErweiterung;

      bestätigungslistennDateiname = seilfehlerAusgabeProjektname + ergebnisFehlerlistenName;
      //bestätigungslistennDateiname = aktuellerDatenpfad + "\\" + dateiMitErweiterung;

      //
      //  SAA-parameterdatei wird im Projektverzeichnis, dort wo die Datendateien
      //  liegen als kopie abgelegt
      //
      saaParBackupDateiname = seilfehlerAusgabeProjektname + erwSaaBackupParDateiname;
      //
      //  SAA-parameterdatei wird ins Programmverzeichnus von SAA geschrieben
      //
      saaParDateiname = Path.Combine(saaApplikationsPfad, erwSaaParDateiname);

    }



    /// <summary>
    /// Dateinamen erzeigen, die auf Grund der EIngaben des Projektnamens für die 
    /// Auswertung unverändelich sind
    /// </summary>
    private void erzeugeTasaParDateiNamen()
    {
      string dateiMitErweiterung;
      //
      // Tasa-Backupdateiname
      //
      dateiMitErweiterung = projektName + "_" + erwTasaParDateiname;
      //
      // QST: 2016052300
      //
      tasaParBackupDateiname = Path.Combine(tasaUserDatenPfad, dateiMitErweiterung);
      //
      // Tasa-Parameterdateiname
      //
      dateiMitErweiterung = erwTasaParDateiname;
      tasaParDateiname = Path.Combine(tasaUserDatenPfad, dateiMitErweiterung);

      tasaParDateiNameTeil2 = Path.Combine(tasaUserDatenPfad, tasaParDateiTeil2);
 
    }

    #region TASA-Eigenschaften

    /// <summary>
    /// Rückgabe des TASA-Dateinamens
    /// </summary>
    public string TasaParameterateiname
    {
      get 
      {
        erzeugeTasaParDateiNamen();
        return tasaParDateiname; 
      }
    }





    /// <summary>
    /// Rückgabe des TASA-Programmpfades
    /// </summary>
    public string TasaProgrammpfad
    {
      get { return tasaApplikationsPfad; }
      set
      {
        tasaApplikationsPfad = value;
        //
        //  nri ausführen, wenn tasaApplikationsPfad bereits gesetzt, sonst in  
        if (tasaApplikationsName.Length > 0)
        {
          string dateiMitErweiterung = tasaApplikationsName; // nicht übr Batch starten -> erwTasaBatchDateiname;
          tasaProgramm = Path.Combine(tasaApplikationsPfad, dateiMitErweiterung);

          erzeugeTasaParDateiNamen();
        }
      }
      //set { tasaApplikationsPfad = value; }
    }





    /// <summary>
    /// Rückgabe des TASA-Programmenamens
    /// </summary>
    public string TasaProgrammname
    {
      get { return tasaApplikationsName; }
      set
      {
        tasaApplikationsName = value;

        if (tasaApplikationsPfad.Length > 0)
        {
          string dateiMitErweiterung = tasaApplikationsName; // nicht übr Batch starten -> erwTasaBatchDateiname;
          tasaProgramm = Path.Combine(tasaApplikationsPfad, dateiMitErweiterung);

          // erzeugeTasaParDateiNamen(); //23.05.2016

        }
      }
    }





    /// <summary>
    /// Rückgabe des TASA-Backupdteinamens 
    /// </summary>
    public string TasaParBackupdateiname
    {
      get { return tasaParBackupDateiname; }
    }


    /// <summary>
    /// Benutzerdatenpfad für SAA-Batchdateien etc. die programmatisch
    /// geschrieben werden und die dann gestartet werden
    /// </summary>
    public string SaaUserDatenPfad
    {
      get { return saaUserDatenPfad; }
      set { saaUserDatenPfad = value; }
    }


    /// <summary>
    /// Benutzerdatenpfad für SAA-Batchdateien etc. die programmatisch
    /// geschrieben werden und die dann gestartet werden
    /// </summary>
    public string TasaUserDatenPfad
    {
      get { return tasaUserDatenPfad; }
      set { tasaUserDatenPfad = value; }
    }

    /// <summary>
    /// Rückgabe des unverändelichen Teils (Teil 2) des TASA-Parameterdatei
    /// </summary>
    public string TasaPardateinameTeil2
    {
      get { return tasaParDateiNameTeil2; }
    }

    #endregion // TASA-Eigenschaften

    #endregion // Funktionen für TASA-Dialog

    #region Funktionen für SAA_Dialog



    private string saaSpezialDateiname()
    {

      int posPunkt = headerDateiname.IndexOf(".");
      headerDateiname = headerDateiname.Substring(0, posPunkt);

      //int posUnterstrich = headerdatei.LastIndexOf('_');
      //int differenz = posPunkt - posUnterstrich -1;

      //if (differenz == 4)
      //  headerdatei = headerdatei.Substring(0, headerdatei.Length - 2);
      //else
      headerDateiname = headerDateiname.Substring(0, headerDateiname.Length - 2);

      //      string ziel = prgEinstellungen.AusgabeverzeichnisListen + "\\" + headerdatei + "_" + saaParDateiname;

      return headerDateiname;
    }





    public string SAAProgrammpfad
    {
      get { return saaApplikationsPfad; }
      set
      {
        saaApplikationsPfad = value;

        string dateiMitErweiterung = erwSaaBatchDateiname;
        saaProgramm = Path.Combine(saaApplikationsPfad, dateiMitErweiterung);
        ////
        //// völliger Quatsch !
        ////
        //dateiMitErweiterung = saaSpezialDateiname() + erwSaaParDateiname;
        //saaParDateiname = Path.Combine(saaApplikationsPfad, dateiMitErweiterung);

        //erzeugeSAADateiNamen();
      }
    }



    public string SAAParameterdateiname
    {
      get { return saaParDateiname; }
    }


    public string SAABatchdateiname
    {
      get { return saaApplikationsPfad; }
      set { saaApplikationsPfad = value; }
    }




    public string SaaProgrammname
    {
      get { return saaApplikationsName; }
      set
      {
        saaApplikationsName = value;
        // %% erzeugeSAADateiNamen();
      }
    }

    
    
    /// <summary>
    /// Rückgabe des SAA-Programmnamens
    /// </summary>
    public string SaaProgramm
    {
      get { return saaProgramm; }
    }





    /// <summary>
    /// Rückgabe des SAA-Parameterdateinamens
    /// </summary>
    public string SaaParBackupdateiname
    {
      get { return saaParBackupDateiname; }
    }

    /// <summary>
    /// Rückgabe des SAA-Programmnamens
    /// </summary>
    public string DrahtbruchlistendateinameHeben
    {
      get { return drahtbruchlisteHeben; }
      set 
      { 
        drahtbruchlisteHeben = value;
        // bei jeder Änderung der Headerdatei müssen alle Ausgabdateinamen
        // neu berstimmt werden: hier deaktiviert, siehe "DrahtbruchlistendateinameSenken"
        // erzeugeSAADateiNamen();

     }
    }

 



    /// <summary>
    /// wird automatisch im Dialog SAA in Folge der Eingabe der ersten (Heben)-Datei
    /// der Fehleranzeigenliste ausgeführt
    /// </summary>
    public string DrahtbruchlistendateinameSenken
    {
      get { return drahtbruchlisteSenken; }
      set 
      { 
        drahtbruchlisteSenken = value;
        // bei jeder Änderung der Headerdatei müssen alle Ausgabdateinamen
        //  neu berstimmt werden
        erzeugeSAADateiNamen();
      }
 
    }





    public string TasaProgramm
    {
      get { return tasaProgramm; }
      //      set { tasaApplikationsName = value; }
    }





    public string BestätigungslistennDateiname
    {
      get { return bestätigungslistennDateiname; }
    }





    public string FehlerAnzeigenlistenDateiname
    {
      get { return fehlerAnzeigenlistenDateiname; }
    }






    public string TidDateiname
    {
      get
      {
        return tidDateiname;
      }
    }

    /// <summary>
    /// Sind die e
    /// </summary>
    public bool ErforderlicheDatenOK
    {
      get
      {
        return erforderlicheDatenOK;
      }
    }

    #endregion // Funktionen für SAA_Dialog 


  }
}
