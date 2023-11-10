//
//  Präprozessordefinitionen für bedingte Kompilierung
//  Dies muss(!) die erste Anweisung im Quelltext sein
// 
#define DEBUG_DirketzugriffAuf_BezugelängenFehlerStruktur
//#define DEBUG_AusgabeOhneAnzahlFehlerAufBezugslänge
//
//  Bibliotheken
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;

//
//  Zugriff auf Office-Anwendungen
//
using Microsoft.Office.Interop.Excel;
using ExcelApp = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;





namespace TasaParDef
{
  /// <summary>
  /// Ausgabeformatsteuerung für Drahbruchliste als Text- oder EXCEL-Grafikdatei
  /// </summary>
  enum AusgabeFormat { TEXTDATEI, WORDDATEI };

  enum AuswerteModus { VOLLSTÄNDIG, WORDIMPORTDATEI};

  public partial class Hautform : Form
  {

    #region Datenelemente

    /// <summary>
    /// Aufzählung der Reihenfolge der Registerkarte für den verbesserten und 
    /// lesbaren Zugriff auf die Registerkarten
    /// </summary>
    enum REGISTERKARTE { TASA = 0, SAA, FEHLERSTELLEN, EINSTELLUNGEN };

    
    /// <summary>
    /// Aufzählung der Dateikenner zum Generieren von Grafik-Exceldateinamem
    /// für 30xD ind 6xD
    /// </summary>
    enum GRAFIKDATEI { DATEINAME6xD = 0, DATEINAME30xD};

    
    /// <summary>
    /// Objekt für die Programmeinstellungen. Diese liegen in einer XML Datei
    /// </summary>
    private CEinstellungen prgEinstellungen;

    /// <summary>
    /// alle Daten des SAA-Dialogs liegen in dieser Klasse
    /// </summary>
    private CSAADialogdaten saaDialogdaten;

    /// <summary>
    /// Objekt für die Revisionsinformation
    /// </summary>
    private CRevisionsInfo revision;

    /// <summary>
    /// Objekt zur Ablage der Seilfehler
    /// </summary>
    private CSeilFehlerstellenListe fehlerliste;

    /// <summary>
    /// gibt an auf der Dialog SAA bereits einmal initialisiert wurde
    /// </summary>
    private bool saaDialogIstInitialisiert;
    
    /// <summary>
    /// gibt an, ob beim Start fetgestllt wurde, dass die XML-Datei fehlt
    /// Wenn diese Fehler auftritt, dann im Form_Load auf die Registerkarte
    /// 'Einstellungen' springen
    /// </summary>
    private bool xmlDateiFehlt;

    /// <summary>
    /// Name der Parameterdatei für TASA dieser muss konstant sein. Da das Pascal-Programm 
    /// von Udo Denzer diesen Dateinamen erwartet
    /// </summary>
    private const string tasaParDateiname = "Tasa_laser.par";

    /// <summary>
    /// Name der Parameterdatei für SAA dieser muss konstant sein. Da das Pascal-Programm 
    /// von Udo Denzer diesen Dateinamen erwartet
    /// </summary>
    private const string saaParDateiname = "saaproRegr.par";

    /// <summary>
    /// Unveränderlicher (unterer) Teil der TASA_Laser.par (wird nur angehangen) 
    /// </summary>
    private const string tasaParDateinameTeil2 = "TASA_Laser_par.Teil2";

    /// <summary>
    /// Batchdateiname zum Start von TASA
    /// </summary>
    private const string tasaBatchDateiname = "Start_tasapro_laser.bat";

    /// <summary>
    /// Batchdateiname zum Start von SAA
    /// </summary>
    private const string saaBatchDateiname = "Start_saaproRegr.bat";

    /// <summary>
    /// Erweiterung für den Ausgabedateinamen der bestätigten Fehlerliste
    /// </summary>
    private const string fehlerlistenName = "_Fehler.txt";

    /// <summary>
    /// Erweiterung für den Ausgabedateinamen der bestätigten Fehlerliste als Importdatei für Word
    /// </summary>
    private const string fehlerlistenNameWord = "_Fehler_WordImport.txt";

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
    private const string tasaInfoDateiname = ".TID";

    /// <summary>
    /// Zeigt an, ob die Fehlerliste bereits geladen wurden 
    /// </summary>
    private bool fehlerListeGeladen; 

    /// <summary>
    /// Datenpfad wird beim Start aus den Programeinstellungegeladen. Dieser wird
    /// jedoch geändert, wenn Dateien von einem anderen Ort geladen werden.
    /// </summary>
    private string aktuellerDatenpfad;

    /// <summary>
    /// Die geprüfte Seillänge aus der TID (Technische Information Datei, *.tid)
    /// </summary>
    private double geprüfteSeillänge;

    /// <summary>
    /// Instanz für die aus dem Projekt abgeleiteten übrigen Dateinamen
    /// </summary>
    private CDateinamen dateiNamen;

    /// <summary>
    /// der Auswertemodus steht standardmäßig auf AuswerteModus.VOLLSTÄNDIG. Wird im Formular
    /// Form1, Registerkarte Fehlerstellen der Button Worddatei betätigt, so wird der Modus
    /// auf AuswerteModus.WORDIMPORTDATEI gesetzt. Das hat zur Folge, dass keine Excelgrafiken
    /// mit grafioscher aufbereitumg der Seilfehler erzeugt werden.
    /// </summary>
    AuswerteModus auswerteModus = AuswerteModus.VOLLSTÄNDIG;

  
    
    #endregion // Datenelemente

    #region Konstruktion und Initialisierung

    /// <summary>
    /// Initialisierung der Hauptform und Starten der Anwendung
    /// </summary>
    public Hautform()
    {
      
      InitializeComponent();
      initialisierung();
      //MessageBox.Show("Ausgabe der Diagramme geändert! Funktion ?", "Achtung!", MessageBoxButtons.OK, MessageBoxIcon.Stop);

    }





    /// <summary>
    /// Definition der Voreinstellungen beim Start des Hauptformulars
    /// </summary>
    private void initialisierung()
    {
       AuswerteModus auswerteModus = AuswerteModus.VOLLSTÄNDIG;


      //MessageBox.Show("Ausgabe der Diagramme geändert! Funktion ?", "Achtung!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      //
      // Initialisierung der Registerkarte Einstellungen 03.08.2011  07:41:32
      // 
      initRegEinstellungen();
      //
      //  Information über die verwendete Version/Revision holen
      //
      revision = new CRevisionsInfo();
      //
      // Registerkarte TASA initialisieren 03.08.2011  07:35:15
      // 
      initRegTasa();
      //
      // Registerkarte SAA initialisieren 03.08.2011  07:35:37
      // 
      initRegSAA();//
      //
      // Registerkarte Fehlerstellen initialisieren 03.08.2011  07:35:37
      // 
      initRegkarteFehlerstellen();
      //
      //  Initialisierung der Dtaenelemente der Klasse
      initDatenlemente();
      //
      //  Startposition der Formzulars zentriert auf dem Bildschirm
      //
      this.StartPosition = FormStartPosition.CenterScreen;
    }

    private void initDatenlemente()
    {
      dateiNamen = new CDateinamen();

      //dateiNamen.TasaUserDatenPfad = System.Windows.Forms.Application.CommonAppDataPath + "\\" + "TASA";
      //dateiNamen.SaaUserDatenPfad = System.Windows.Forms.Application.CommonAppDataPath + "\\" + "SAA";
      //
      // QST: 2016052300
      //
      //  Ab 23.05.2016 auf C:\\ProgramData\\RWE Power\\TasaParDef\\SAA und C:\\ProgramData\\RWE Power\\TasaParDef\\TASA
      //  hart codiert, das sonst die Versionsnummer aus den Assemblyinformationen zur Installation mit verwendet würden.
      //  Das Zielverzeichnis hieße dann C:\\ProgramData\\RWE Power\\TasaParDef\\TasaParDef_2.0.0.165\\SAA.
      //  Dann müsste jedoch auch die Assemblyinformationen gepflegt werden und für SAA und TASA müsste bei jeder Änderung 
      //  von TASAPardef SAA und TASA neu installiert werden. Um das zu verhindern, wie der Pfad zwar im ProgrammData-Verzeichnis
      //  belassen, aber hier im Quelltext und im Quelltext von InnoSetup fest codiert. Somit wird TASA und SAA (Pascal) nur einmal
      //  installiert.
      //
      string commondatenpfad = "C:\\ProgramData\\RWE Power\\TasaParDef\\";
      dateiNamen.TasaUserDatenPfad = commondatenpfad + "TASA";
      dateiNamen.SaaUserDatenPfad = commondatenpfad + "SAA";
      
      saaDialogdaten = new CSAADialogdaten(prgEinstellungen, dateiNamen);
    }





    /// <summary>
    /// Registerkarte SAA initialisieren
    /// </summary>
    private void initRegSAA()
    {

      BTN_StartSAA.Enabled = true;
      //
      //  neues Objekt für die Daten des SAA-Dialoges 
      //
      //saaDialogdaten = new CSAADialogdaten(prgEinstellungen, dateiNamen);
      //
      //  gibt an ob der Dialog bereits einmal initialisiert wurde
      //
      //saaDialogIstInitialisiert = false;
      //
      //  Checkbox einmal umschalten um Ereignis auszulösen , damit die
      //  Groupbox "Regression" deaktiviert wird
      //
      CHK_BoxMitRegression.Checked = true;
      CHK_BoxMitRegression.Checked = false;
      //
      //  Dialogfelder mit Standarddaten aus der Dialogklasse füllen
      //
      // aktuellePosition = 0;
      //  EDT_SeildurchmesserTASA.Text = saaDialogdaten.Seildurchmesser.ToString();
      //  EDT_akzLagedifferenz.Text = saaDialogdaten.Lagedifferenz.ToString("F2");
      EDT_akzLagedifferenz.Text = "0,2";

      

    }





    /// <summary>
    /// Registerkarte Einstellungen initialisieren
    /// </summary>
    private void initRegEinstellungen()
    {
      //
      // aktuelle Programmeisntellungen laden 03.08.2011  07:48:41
      // 
      prgEinstellungen = new CEinstellungen();
      parametereinlesen();
      prgEinstellungen.AktuellerDatenPfad = prgEinstellungen.Datenpfad;
      EDT_SpeicherortEinstellungen.Text = System.Windows.Forms.Application.UserAppDataPath;

    }




    /// <summary>
    /// Registerkarte TASA initialisieren
    /// </summary>
    private void initRegTasa()
    {
      //BTN_DatenVerzeichnisÖffnen.Enabled = false;
      BTN_TASAStart.Enabled = true;
      //
      // Radiobutten vorwählen 03.08.2011  07:47:40
      // 
      RBTN_Heben.Checked = true;
      RBTN_ohneSigKorrektur.Checked = true;
      RBTN_NordpolSüdpol.Checked = true;
      //
      //  Radiobutton für Kurvendiskussion vorwählen
      //
      RBTN_KurvenDis_ein.Checked = true;
      //
      // Auswahl der Empfindlichkeit 03.08.2011  07:48:16
      // 
      CBX_Empfindlichkeit.SelectedIndex = 5;
      CHK_Empfindlichkeit.Checked = true;
      //
      //  Seilgeschwindigkeit mit 0,5 m/s vorwählen
      //
      CBX_Seilgeschwindigkeit.SelectedIndex = 1;

      EDT_SeildurchmesserTASA.Text = "0";
      EDT_SeilImpulsverhältnis.Text = "0";
    }





    /// <summary>
    /// Initialisierung der Registerkarte Fehlerstellen, die beim 
    /// Eintritt in die Registerkarte und nach dem Ausgeben der der 
    /// Drahtbruchliste ausgeführt wird.
    /// </summary>
    private void initRegkarteFehlerstellen()
    {
      LVW_Seilfehler.Items.Clear();
    
      if (fehlerliste != null)
        fehlerliste.Clear();

      fehlerListeGeladen = false;


      BTN_AlleAuswählen.Enabled = false;
      BTN_AlleAbwählen.Enabled = false;
      BTN_DbListeAusgeben.Enabled = false;
      BTN_FehlerEinfügen.Enabled = false;
      EDT_Fehlerlistendatei.Text = "";
      //
      // 03.08.2011  07:28:01
      // 
      EDT_SummeAllerDb.Text = "0";
      EDT_SummeAutoDb.Text = "0";
      EDT_SummeBestDb.Text = "0";
      EDT_SummeManuellDb.Text = "0";

    }





    /// <summary>
    /// Berechnung der aktuellen Fehlerstatistik für alle, ausgewählte und 
    /// manuell hizugefügte Fehlerstellen und Update auf der Registerkarte
    /// Fehlerstellen
    /// </summary>
    private void updateDrahtbruchstatistik()
    {
      int alleFehler = fehlerliste.gibAnzahlAllerFehler;
      int autoFehler = fehlerliste.gibAnzahlAutoFehler; 
      int manuelleFehler = alleFehler - autoFehler;


      EDT_SummeAllerDb.Text = alleFehler.ToString();
      EDT_SummeAutoDb.Text = autoFehler.ToString();
      EDT_SummeBestDb.Text    = fehlerliste.gibAnzahlBestätigterFehler.ToString();
      EDT_SummeManuellDb.Text = manuelleFehler.ToString();
    }

    #endregion // Konstruktion und Initialisierung

    #region Hauptformularfunktionen




    /// <summary>
    /// Schaltfläche "Beenden" mit Auslösung zur Speicherung der eingegebenen
    /// Parameter in der Registerkarte "Einstellungen"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_Beenden_Click(object sender, EventArgs e)
    {
      parameterSpeichern();
      this.Close();
    }




    /// <summary>
    /// Schaltfläche "Abbruch" und Verlassen des Programms ohne Sichern der Einstellung
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_Abbruch_Click(object sender, EventArgs e)
    {
      this.Close();

    }





    /// <summary>
    /// Titel des Hauptformulars mit Text und Revisionsnummer versehen und dem Dialog zuweisen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Hautform_Load(object sender, EventArgs e)
    {
      //MessageBox.Show("Ausgabe der Diagramme geändert! Funktion ?", "Achtung!", MessageBoxButtons.OK, MessageBoxIcon.Stop);


      string formTitel = "Seilprüfauswertung TASAPardef";
      formTitel += " (Version " + revision.Versionsnummer + ")";
      this.Text = formTitel;
      //
      //  beim Laden der XML-Datei ist ein Fehler aufegtreten, die datei war möglicherweise
      //  nicht vohanden, deshalb direkt zur Registerkarte "Einstellungen" springen
      //
      if (xmlDateiFehlt == true)
      {
        string txt = "Die Programmeinstellungen konnten nichtgeladen werden!";
        programmWarnung("Eingabedaten fehlen!", txt );
        REG_Auswerteparameter.SelectedIndex = (int)REGISTERKARTE.EINSTELLUNGEN;
      }
      //
      //  es wurde festgestellt dass der Pfad zur SAA.Exe nicht den Einstellungen entspricht
      //  
      string saaPfadAbsolut = Path.Combine(prgEinstellungen.SaaEXEPfad, prgEinstellungen.SaaEXEName);
      if (File.Exists(saaPfadAbsolut) == false)
      {
        string txt = "Die Programmeinstellungen sind nicht korrekt!\nDer Pfad- und/oder der Name der auführbaren Datei für SAA ist falsch!";
        txt += "\nBitte berabeiten Sie zuerst den Dialog 'Einstellungen'";
        programmWarnung("Eingabedaten fehlen!", txt);
        REG_Auswerteparameter.SelectedIndex = (int)REGISTERKARTE.EINSTELLUNGEN; 

      }
      //
      //  es wurde festgestellt dass der Pfad zur TASA.Exe nicht den Einstellungen entspricht
      //  
      string tasaPfadAbsolut = Path.Combine(prgEinstellungen.TasaEXEPfad, prgEinstellungen.TasaEXEName);
      if (File.Exists(tasaPfadAbsolut) == false)
      {
        string txt = "Die Programmeinstellungen sind nicht korrekt!\nDer Pfad- und/oder der Name der auführbaren Datei für TASA ist falsch!";
        txt += "\nBitte berabeiten Sie zuerst den Dialog 'Einstellungen'";
        programmWarnung("Eingabedaten fehlen!", txt);
        REG_Auswerteparameter.SelectedIndex = (int) REGISTERKARTE.EINSTELLUNGEN;

      }
    }

    
    
    
    
    /// <summary>
    /// Ausgabe einer Warnung als Messagesbox
    /// </summary>
    /// <param name="titel">Fenstertitel</param>
    /// <param name="nachricht">Nachricht an den Benutzer</param>
    private void programmWarnung(string titel, string nachricht)
    {
      //  "Einstellungen" gesprungen wird 

      // Mesagebox wz_msgBox mit Titel und Text parametriert
      //
      MessageBoxButtons button = MessageBoxButtons.OK;
      MessageBoxIcon icon = MessageBoxIcon.Warning;


      MessageBox.Show(nachricht, titel, button, icon);
    }

    /// <summary>
    /// Öffnet das Verzeichnis vom aktuellen Datenpfad im Explorer. Dieser Button kann nur angeklickt werden
    /// wenn eine eine Datei ausgewählt wurde
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_VerzeichnisÖffnen_Click(object sender, EventArgs e)
    {
      string verzeichnis = aktuellerDatenpfad;

      Process explorer = new Process();

      explorer.StartInfo.FileName = "Explorer";
      explorer.StartInfo.Arguments = prgEinstellungen.AktuellerDatenPfad;
      explorer.StartInfo.UseShellExecute = false;
      explorer.Start();

    }





    /// <summary>
    /// öffnet das Listenverzeichnis im Explorer, das auf der Registerkarte "Einstellungen" angegeben wurde 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_ListenVerzeichnisÖffnen_Click(object sender, EventArgs e)
    {
      string verzeichnis = prgEinstellungen.AktuellerDatenPfad;

      Process explorer = new Process();

      explorer.StartInfo.FileName = "Explorer";
      explorer.StartInfo.Arguments = verzeichnis;
      explorer.StartInfo.UseShellExecute = false;
      explorer.Start();

    }





    /// <summary>
    /// Versenden einer E-Mail an den Programmierer mit dem Hinweis auf Fehler bzw. Änderungen
    /// behoben bzw. geändert werden müssen.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
    private void linkLabel1_Zilger(object sender, LinkLabelLinkClickedEventArgs e)
    {
      string mailtext = "mailto: wilfried.zilger@rwe.com";
      mailtext += "?subject=Fehler in TASAPardef / Änderungen in TASAPardef";
      System.Diagnostics.Process.Start(mailtext);
    }

    private void linkLabel_Denzer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      string mailtext = "mailto: udo.denzer@rwe.com";
      mailtext += "?subject=Fehler in TASAPardef / Änderungen in TASAPardef";
      System.Diagnostics.Process.Start(mailtext);

    }

   

    #endregion // Hauptformularfunktionen

    #region Hilfsmethoden

    /// <summary>
    /// Konvertierung eines Eingegebenenwertes aus einem Eingabefeld in eine doppelt genaue Zahl und
    /// Überprüfung eines Eingegebenenwertes auf Gültigkeit. Gegebenenfalls wirdbei auftreten eines Fehlers
    /// der Ausgabeparameter "fehler" gesetzt und einen Fehlertext ausgegeben. Zur Steuerung wird eine allgemeine
    /// Ausnahme ausgelöst
    /// </summary>
    /// <param name="eingabefeld">Objekt der Textbox in der die Zahl eingegeben wurde</param>
    /// <param name="minWert">zulässiger Minimalwert</param>
    /// <param name="maxWert">zulässiger Maximalwert</param>
    /// <param name="fehler">wird true, wenn ein Fehler aufgetreten ist</param>
    /// <param name="fehlerText">Fehlertext der bei Auftreten eines Fehlers ausgegeben wird</param>
    /// <returns></returns>
    private double checkDouble(System.Windows.Forms.TextBox eingabefeld, double minWert, double maxWert, out bool fehler, string fehlerText)
    {
      fehler = false;

      try
      {
        double doubleZahl = double.Parse(eingabefeld.Text.Replace('.', ','));
        if (doubleZahl < minWert || doubleZahl > maxWert)
          throw new Exception(fehlerText);

        return doubleZahl;

      }
      catch (Exception ex1)
      {
        fehler = true;
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Stop;

        string überschrift = "Achtung: Fehler bei der Zahlenkonvertierung";
//        string meldung = "Die angegebene Zahl ist ungültig!";
        string meldung = ex1.Message;

        MessageBox.Show(meldung, überschrift, button, icon);
        errorProvider1.SetError(eingabefeld, fehlerText);

        return 0;
      }

    }


  

    #endregion // Hilfsmethoden

    #region Registerkarten
    

    #region Registerkarte TASA




    /// <summary>
    /// Auswahl der Diadem-Headerdatei im Dialog für die Eingabe der Daten zum Start
    /// des Programms TASA
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_AuswahlHeader_Click(object sender, EventArgs e)
    {
      auswählenHeaderDatei();
    }

    private void auswählenHeaderDatei()
    {
      bool umlauteImText = false;
      bool dateinameOk = false;

      DialogResult result;
      //
      //  Umlaute abfangen bzw. deren Eingabe nicht zulassen!
      //
      do
      {
        errorProvider1.Clear();
        OpenFileDialog file = new OpenFileDialog();

        file.Title = "Dateiauswahl der DIAdem-Headerdatei";
        file.Filter = "DIAdem Headerdatei (*.DAT)|*.dat|Alle Dateien (*.*)|*.*";
        //
        // 17.08.2011  09:37:22
        // 
        //        file.InitialDirectory = prgEinstellungen.Datenpfad;
        file.InitialDirectory = prgEinstellungen.AktuellerDatenPfad;

        result = file.ShowDialog();

        if (result == DialogResult.OK)
        {
          EDT_Headerdatei.Text = file.FileName;
          EDT_SeildurchmesserTASA.Focus();
          umlauteImText = checkAufUmlaute(EDT_Headerdatei);
          dateinameOk = checkDateiName(EDT_Headerdatei);

          if (dateinameOk == false)
          {
            EDT_Headerdatei.Text = "";
            EDT_Headerdatei.Focus();
            break;
          }

          if (umlauteImText == false)
          {
            prgEinstellungen.AktuellerDatenPfad = Path.GetDirectoryName(EDT_Headerdatei.Text);
            //
            //  auf Grund des einegebenen (header-)Dateinamens werden nun in 
            //  CDateiname.HeaderDateiname die Dateinamen aller Dateien erzeugt
            //
            dateiNamen.Headerdateiname = EDT_Headerdatei.Text;
            programmNamen2CDateinamen();
          }
        }

//      } while (result == DialogResult.OK && (umlauteImText == true || dateinameOk == false));
      } while (result == DialogResult.OK && umlauteImText == true);


    }



    /// <summary>
    /// Eruegz die Dateinamen für die Auswertung des ausgewählten Seiles 
    /// </summary>
    /// <seealso cref="BTN_AuswahlHeader_Click"/>
    /// <param name="ausgewählterDateinamen"></param>
    private void programmNamen2CDateinamen()
    {
      // --- 20.10.2011 14:18:56 --- 
      // 
      // 
      //dateiNamen = new CDateinamen(ausgewählterDateinamen);
      //
      // 31.08.2011  12:59:14
      // neu, für neue Verarbeitung der Dateinmamen in einer gesonderten Klasse
      // 
      dateiNamen.TasaProgrammname = prgEinstellungen.TasaEXEName;
      dateiNamen.TasaProgrammpfad = prgEinstellungen.TasaEXEPfad;
      
      dateiNamen.SaaProgrammname = prgEinstellungen.SaaEXEName;
      dateiNamen.SAAProgrammpfad = prgEinstellungen.SaaEXEPfad;
      //

    }



    /// <summary>
    /// Überprüfung des Dateinamens auf ein bestimmtes Format. In dieser Version,
    /// wird der Dateiname dahingehend überprüft, ob auf die letzten drei bzw. vier Stellen
    /// vor dem Punkt der Dateierweiterung die folgenden Zeichen vorhanden sind:
    /// für zweiseil Geräte: LH1, LS1, RH1, RS1
    /// für vierseil Geräte: LAH1, LAS2 ,LIH1, LIS2, RAH1, RAS2,RIH1, RIS2
    /// Die Struktur muss entweder zwei oder drei alphanumerischen Zeichen gefolgt von einem 
    /// numerischen Zeichen entsprechen. in der Variable Format wird die Struktur anhand von Zeichnungen
    /// festgestellt. Der Buchstabe A wir für ein alphanumerische Zeichen und der Buchstabe N
    /// für ein numerisches verwandt. Ein unbekanntes Zeichen wird mit einem ? gekennzeichnet
    /// somit sind gültige Formatszeichenketten für Vierseilgeräte AAAN und für Zwieseilgeräte AAN
    /// </summary>
    /// <param name="EDT_Headerdatei">Control</param>
    /// <returns>true wenn der Dateiname den Konventionen entspricht, false wenn er ungültig ist</returns>
    private bool checkDateiName(System.Windows.Forms.TextBox EDT_Headerdatei)
    {
      //
      //  Dateinamen isolieren 
      //
      string dateiname = EDT_Headerdatei.Text;
      //
      //  Zeichenkette die die Seilörtlichkeit bestimmt extrahieren
      //
      int punktPos = dateiname.IndexOf('.');
      int unterstrichPos = dateiname.IndexOf('_', punktPos-5, 5);
      int länge = punktPos - unterstrichPos -1;
      string teilzeichenkette = dateiname.Substring(unterstrichPos + 1, länge);
      //
      //  Zeichentypen ermitteln  und in String, dass die Zeichentypen repräsentiert
      //  ablegen (AAN = alphanum. Zeichen,alphanum. Zeichen, numerischen Zeichen
      //  AAAN = alphanum. Zeichen, alphanum. Zeichen, alphanum. Zeichen , numerischen Zeichen
      //
      bool[] zeichenOk = new bool[länge];

      string format = "";
      for (int i = länge; i > -0; i--)
      {
        if (char.IsLetter(teilzeichenkette[teilzeichenkette.Length - i]))
          format += "A"; // ein Alphanumerisches Zeichen
        else if (char.IsNumber(teilzeichenkette[teilzeichenkette.Length - i]))
          format += "N"; // ein Numerisches Zeichen
        else
          format += "?"; // ein unbekanntes Zeichen
       zeichenOk[0] = char.IsNumber(teilzeichenkette[teilzeichenkette.Length - 1]);
      }

      bool fehler = false;
      if (format.Length == 4 && format != "AAAN") //z.B. Bg290_LAH1....
        fehler = true;

      if (format.Length == 3 && format != "AAN") //z.B. Bg258_LS1....
        fehler = true;

      if (fehler == true)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Asterisk;

        string überschrift = "Achtung:Verletzung der Dateinamenkonvention!";
        string meldung = "Der Dateinaname sollte der Struktur Bg_290_aaa#.* oder Bg_258_aa#.* entsprechen!";
        meldung += "\nBeispiel: Bg_290_LAH1.DAT (Vierseilgerät) oder BG_258_LS1.DAT (Zweiseilgerät)";
        MessageBox.Show(meldung, überschrift, button, icon);
        return false;
      
      }

      return true;
    }

    /// <summary>
    /// Eine Eingabe in eine Textboc auf Umlaute prügfen
    /// </summary>
    /// <param name="tbox"></param>
    /// <returns></returns>
    private bool checkAufUmlaute(System.Windows.Forms.TextBox tbox)
    {
      string pat = "äöüßÄÖÜ";

      foreach (char ch in tbox.Text)
      {
        if (pat.IndexOf(ch) >= 0)
        {
	        // Mesagebox wz_msgBox mit Titel und Text parametriert
	        //
	        MessageBoxButtons button = MessageBoxButtons.OK;
	        MessageBoxIcon icon = MessageBoxIcon.Warning;

	        string überschrift = "Achtung: Ungültiges Zeichen im ";
	        string meldung = "Die verwendeten Programme können keine Zeichen (äöüßÄÖÜ) verarbeiten\n";
          meldung += "Bitte wählen sie eine anderes Verzeichnis und/oder einen anderen Dateinamen aus!";

	        MessageBox.Show(meldung, überschrift, button , icon );
	
          //  Textbox-Inhalt löschen
          tbox.Text = "";

          return true;
        }
      }
      return false;
    }





    /// <summary>
    /// Schaltfläche für den Start der Anwendung "TASA".Hier wird nach Plausibilitätsprüfung
    /// und den Test auf Vollständigkeit der Daten, die Parameterdatei geschrieben und die Anwendung
    /// TASA gestartet.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_TASAStart_Click(object sender, EventArgs e)
    {
      //
      //  Listbox für TasaInfo löschen
      //
      LBX_TasaInfo.Items.Clear();
      this.Update();

      if (testAufVollständigkeit() == false) return;


      try
      {
        schreibeTASAParDatei();

        starteTASAPro();

        zeigeTasaIndfomationsDatei();

      }
      catch (Exception ex1)
      {
        throw ex1;
      }

    }

    /// <summary>
    /// Anzeigen der Informationen aus der TASA-Informationsdatei in der Listbox der
    /// Registerkarte TASA
    /// </summary>
    private void zeigeTasaIndfomationsDatei()
    {
      try
      {
        string[] alleZeilen = ladeTasaIndfomationsDatei();

        foreach (string zeile in alleZeilen)
          LBX_TasaInfo.Items.Add(zeile);

      }
      catch (Exception ex1)
      {
        // weiter werfen, da die einen Prgrammabbruch zur Folge haben muss
        //  
        throw ex1;
      }


      }

    



   

    /// <summary>
    /// Hauptmethode zum schreiben der Parameterdatei für die TASA
    /// </summary>
    private void schreibeTASAParDatei()
    {
      //string ausgabedatei1 = prgEinstellungen.TasaEXEPfad + "\\" + tasaParDateiname;
      string ausgabedatei =  dateiNamen.TasaParameterateiname;
      FileStream fs = new FileStream(ausgabedatei, FileMode.Create, FileAccess.Write, FileShare.None);

      StreamWriter sw = new StreamWriter(fs,System.Text.Encoding.Default);

      schreibeEingabedaten(sw);

//      string laserParTeil2 = System.Windows.Forms.Application.StartupPath + "\\" + tasaParDateinameTeil2;
      
      // --- 17.08.2011 08:02:58 --- 
      // 
      // 
      // string laserParTeil2 = System.Windows.Forms.Application.StartupPath + "\\" + tasaParDateinameTeil2;
      string laserParTeil2 = dateiNamen.TasaPardateinameTeil2;

      StreamReader sr = new StreamReader(laserParTeil2);

      schreibeTeil2(sr, sw);

      sr.Close();
      sw.Close();

      sichereTASAParDatei();

    }



    /// <summary>
    /// Start des Prozesses TASA als eigenständiger Prozess im DOS-Fenster
    /// </summary>
    private void starteTASAPro()
    {

      //  Parameter für den Start des Prozesses
      ProcessStartInfo pInfo = new ProcessStartInfo();
      pInfo.FileName = dateiNamen.TasaProgramm;
      pInfo.WorkingDirectory =  dateiNamen.TasaProgrammpfad;
      // pInfo.WindowStyle = ProcessWindowStyle.Hidden;
      //
      //  Prozess anlegen
      //
      Process tasaPro = new Process();
      //
      //  ... mit den Daten für den Programmstart versorgen
      tasaPro.StartInfo = pInfo;
      //
      //  ... und starten
      //
      
      tasaPro.Start();
      //
      //  warten bis der extern gestartete Prozess beendet wurde 
      //
      //
      tasaPro.WaitForExit();
      //Thread.Sleep(2000);
      //MessageBox.Show("Tasa-Lauf wurde beendet!", "TASA");
      this.Update();
    }




    /// <summary>
    /// eine Kopie der Parameterdatei für den Start von TASA erzeugt wurde, als Kopie
    /// Datenpfad ablegen. Der Dateiname wird dazu neu erzeugt, so dass er den ersten Teil
    /// des Headerdateinamens (bis zum ersten Punkt) gefolgt vom eigentlichen Parameterdateinamen.
    /// </summary>
    /// <param name="quelldatei">Name der Parameterdatei für TASA</param>
    private void sichereTASAParDatei()
    {
      File.Copy(dateiNamen.TasaParameterateiname, dateiNamen.TasaParBackupdateiname, true); // <-- true überschreibt die ggfs. existierende
    }





    /// <summary>
    /// eine Kopie der Parameterdatei für den Start von TASA erzeugt wurde, als Kopie
    /// Datenpfad ablegen. Der Dateiname wird dazu neu erzeugt, so dass er den ersten Teil
    /// des Headerdateinamens (bis zum ersten Punkt) gefolgt vom eigentlichen Parameterdateinamen.
    /// </summary>
    /// <param name="quelldatei">Name der Parameterdatei für TASA</param>
    private void sichereSAAParDatei(string quelldatei)
    {
      string headerdatei = Path.GetFileName(EDT_DBListeHeben.Text);
      int posPunkt = headerdatei.IndexOf(".");
      headerdatei = headerdatei.Substring(0, posPunkt);

      //int posUnterstrich = headerdatei.LastIndexOf('_');
      //int differenz = posPunkt - posUnterstrich -1;

      //if (differenz == 4)
      //  headerdatei = headerdatei.Substring(0, headerdatei.Length - 2);
      //else
        headerdatei = headerdatei.Substring(0, headerdatei.Length - 2);
   
      //      string ziel = prgEinstellungen.AusgabeverzeichnisListen + "\\" + headerdatei + "_" + saaParDateiname;
      string ziel = aktuellerDatenpfad + "\\" + headerdatei + "_" + saaParDateiname;


      File.Copy(quelldatei, ziel, true); // <-- true überschreibt die ggfs. existierende

    }


    /// <summary>
    /// Erzeugung der Stapelverarbeitungsdatei für den Start von SAA. Diese Datei muss
    /// bei jedem Lauf neu erzeugt werden, weil sich der Speicherort und/oder der Dateiname
    /// der ausführbaren Datei geändert haben kann.
    /// </summary>
    private void schreibeStartSAABatch()
    {
      //string batchDatei = prgEinstellungen.SaaEXEPfad + "\\" + saaBatchDateiname;
      string batchDatei = dateiNamen.SaaProgramm;
      
      FileStream fs = new FileStream(batchDatei, FileMode.Create, FileAccess.Write, FileShare.None);

      StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
      sw.WriteLine("@ECHO OFF");
      sw.WriteLine("CLS");
      sw.WriteLine("REM");
      sw.WriteLine("REM automatisch generierte Stapelverarbeitungdatei: " + batchDatei);
      sw.WriteLine("REM erzeugt von TasaParDef.Exe am " + DateTime.Now.ToString("F"));
      sw.WriteLine("REM");
      //
      // 13.07.2012  11:56:34
      // 
      //  Der absolute Namen zur SAA-Exe muss in Hochkkomma stehen, wenn das Programm mittels
      //  Batchdatei gestartet wird und sich in einem Pfad befindet der Leerzeichenenthält wie 
      //  z.B.: C:\Program Files (x86)\RWEPower\SAA
      //
      string pfadZurExe = "\"" + Path.Combine(prgEinstellungen.SaaEXEPfad, prgEinstellungen.SaaEXEName) + "\"";
      sw.WriteLine(pfadZurExe);

      sw.WriteLine("pause");
      sw.WriteLine("REM");
      sw.WriteLine("REM Ende der automatisch generierten Stapelverarbeitungdatei");
      sw.WriteLine("REM");

      sw.Close();
      fs.Close();
    }




    ///// <summary>
    ///// Erzeugung der Stapelverarbeitungsdatei für den Start von TASA. Diese Datei muss
    ///// bei jedem Lauf neu erzeugt werden, weil sich der Speicherort und/oder der Dateiname
    ///// der ausführbaren Datei geändert haben kann.
    ///// </summary>
    //private void schreibeStartTASABatch()
    //{
    //  string batchDatei = prgEinstellungen.TasaEXEPfad + "\\Start_tasapro_laser.bat";
    //  FileStream fs = new FileStream(batchDatei, FileMode.Create, FileAccess.Write, FileShare.None);

    //  StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
    //  sw.WriteLine("@ECHo OFF");
    //  sw.WriteLine("CLS");
    //  sw.WriteLine("REM");
    //  sw.WriteLine("REM automatisch generierte Stapelverarbeitungdatei: " + batchDatei);
    //  sw.WriteLine("REM erzeugt von TasaParDef.Exe am " + DateTime.Now.ToString("F"));
    //  sw.WriteLine("REM");
    //  sw.WriteLine(Path.Combine(prgEinstellungen.TasaEXEPfad, prgEinstellungen.TasaEXEName));
    //  sw.WriteLine("pause");
    //  sw.WriteLine("REM");
    //  sw.WriteLine("REM Ende der automatisch generierten Stapelverarbeitungdatei");
    //  sw.WriteLine("REM");

    //  sw.Close();
    //  fs.Close();
    //}






    /// <summary>
    /// der erste Teil der Parameterdatei wird aus dem vorliegenden Programm heraus erzeugt.
    /// Der zweite Teil der Parameterdatei ist immer konstant und wird deshalb aus dem 
    /// Verzeichnis der Applikation gelesen und an Parameterdatei angehangen
    /// </summary>
    /// <param name="sr">Stream Reader zum Lesen des konstanten Teils der Parameterdatei</param>
    /// <param name="sw">Stream Writer zum schreiben der Parameterdatei</param>
    private void schreibeTeil2(StreamReader sr, StreamWriter sw)
    {
      string datenTeil2 = sr.ReadToEnd();
      sw.WriteLine(datenTeil2);
    }





    /// <summary>
    /// schreiben der Eingabedaten in die Parameterdateien und entsprechende Konvertierung
    /// in die vorgegebenen Format (hier: Zahlenformat mit "." als Dezimaltrenner
    /// </summary>
    /// <param name="sw">Stream Writer zur Ausgabedatei</param>
    private void schreibeEingabedaten(StreamWriter sw)
    {
      //
      //  Headerdatei
      //
      sw.WriteLine("{Headerdatei der Messdaten}");
//      string headerdatei = Path.GetFileName(EDT_Headerdatei.Text);
      string headerdatei = dateiNamen.Headerdateiname;
      sw.WriteLine(headerdatei);
      sw.WriteLine();
      //
      //  Seillängenimpulsverhältnis
      //
      sw.WriteLine("{Seillaengen Impulsverhaeltnis}");
      string seillängenImpVerh = EDT_SeilImpulsverhältnis.Text.Replace('.', ',');
      double seillängenImpVerhZahl = double.Parse(seillängenImpVerh);
      // Zahl mit Dezimalpunkt ausgeben
      sw.WriteLine(seillängenImpVerhZahl.ToString("F6", NumberFormatInfo.InvariantInfo));
      sw.WriteLine();
      //
      //  Programmpfad
      //
      sw.WriteLine("{Programmpfad}");
      string programmpfad = EDT_TASAExePfad.Text + "\\";
      sw.WriteLine(programmpfad);
      sw.WriteLine();
      //
      //  Datenpfad
      //
      sw.WriteLine("{Datenpfad}");
      string aktuellerDatenpfad = Path.GetDirectoryName(EDT_Headerdatei.Text);
      aktuellerDatenpfad += "\\";
      sw.WriteLine(aktuellerDatenpfad);
      sw.WriteLine();
      //
      //  Seillaufrichtung
      //
      sw.WriteLine("{Seillaufrichtung}");
      string seillaufrichtungBlau = "Suedpol (rot)  ==>  Nordpol (blau)";
      string seillaufrichtungRot = "Nordpol (blau)  ==>  Suedpol (rot)";

      if (RBTN_SüdpolNordpol.Checked == true)
        seillaufrichtungBlau += " x";
      else
        seillaufrichtungRot += " x";

      sw.WriteLine(seillaufrichtungBlau);
      sw.WriteLine(seillaufrichtungRot);
      sw.WriteLine();
      //
      //  Bewegungsrichtung
      //
      sw.WriteLine("{Bewegungsrichtung Ausleger}");
      string bewegungsRichtungHeben = "Heben";
      string bewegungsRichtungSenken = "Senken";

      if (RBTN_Heben.Checked == true)
        bewegungsRichtungHeben += " x";
      else
        bewegungsRichtungSenken += " x";

      sw.WriteLine(bewegungsRichtungHeben);
      sw.WriteLine(bewegungsRichtungSenken);
      sw.WriteLine();
      //
      //  voreingestellte Seilgeschwindigkeit
      //
      sw.WriteLine("{voreingestellte Seilgeschwindigkeit in m/s}");
      string seilgeschwindigkeit = CBX_Seilgeschwindigkeit.Text.Replace(',', '.');
      sw.WriteLine(seilgeschwindigkeit);
      sw.WriteLine();
      //
      //  Signifikanzkorrektur
      //
      sw.WriteLine("{integrierte Signifikanzkorrektur fuer verschleissarme Seilbereiche}");
      string mitSigifikanzKorrektur = "mit Signifikanzkorrektur";
      string ohneSigifikanzKorrektur = "ohne Signifikanzkorrektur";

      if (RBTN_mitSigKorrektur.Checked == true)
        mitSigifikanzKorrektur += " x";
      else
        ohneSigifikanzKorrektur += " x";

      sw.WriteLine(mitSigifikanzKorrektur);
      sw.WriteLine(ohneSigifikanzKorrektur);
      sw.WriteLine();
      //
      //  Fensterbreite für Vertrauenswert
      //
      sw.WriteLine("{Fensterbreite in m Seil für die automatische Ermittlung des Vertrauenswertes; kleine Fensterbreite > hohe Empfindlichkeit (MIDAN)}");
      string fensterbreiteEmpfidlichkeit = CBX_Empfindlichkeit.Text.Replace(',', '.');
      sw.WriteLine(fensterbreiteEmpfidlichkeit);
      sw.WriteLine();
      //
      //  Kurvendiskussion aktiv  / inaktiv
      //
      sw.WriteLine("{Kurvendikussion aktiv}");
      string mitKuvendiskussion = "ja";
      string ohneKuvendiskussion = "nein";

      if (RBTN_KurvenDis_ein.Checked == true)
        mitKuvendiskussion += " x";
      else
        ohneKuvendiskussion += " x";

      sw.WriteLine(mitKuvendiskussion);
      sw.WriteLine(ohneKuvendiskussion);

    }

    
    
    
    
    /// <summary>
    /// Beim Verlassen der Textbox zur Eingabe des Seillängen Impulsverhältnisses
    /// wird die eingegebene Zahl in eine doppelt genaue Zahl konvertiert.
    /// Gegebenenfalls wird dadurch eine Exception ausgelöst, die anzeigt
    /// das eingegebene Zahl (aus der Textbox) ungültig war.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EDT_SeilImpulsverhältnis_Leave(object sender, EventArgs e)
    {
      try
      {
        double impVerhältnis = double.Parse(EDT_SeilImpulsverhältnis.Text);
      }
      catch (Exception)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Stop;

        string überschrift = "Achtung: Fehler Seillängenimpulsverhältnis";
        string meldung = "Die angebene Zahl ist ungültig";

        MessageBox.Show(meldung, überschrift, button, icon);

        EDT_SeilImpulsverhältnis.Focus();
      }

      //  Button "TASA starten" nur freigeben, wenn alle Eingaben getätigt sind
      //
      testAufVollständigkeit();

    }




    /// <summary>
    /// Eingabedaten überprüfen, ob diese vorhanden sind und den Mindestanforderungen
    /// entsprechen
    /// </summary>
    /// <returns>true wenn die Daten o.k. waren</returns>
    private bool testAufVollständigkeit()
    {
      bool dateinameVorhanden = false;
      bool ImpulsverhältnisVorhanden = false;

      dateinameVorhanden = (EDT_Headerdatei.Text.Length > 0);
      ImpulsverhältnisVorhanden = (EDT_SeilImpulsverhältnis.Text.Length > 0);

      if (dateinameVorhanden == false)
      {
        string msgText = "Es wurde keine DIAdem-Hedaerdatei (*.dat) angegeben!";
        MessageboxWarnung(msgText);
        errorProvider1.SetError(EDT_Headerdatei, msgText);
      }
      else if (ImpulsverhältnisVorhanden == false)
      {
        string msgText = "Es wurde kein Seilängenimpulsverhältnis angegeben!";
        MessageboxWarnung(msgText);

        errorProvider1.SetError(EDT_SeilImpulsverhältnis, msgText);

      }
      bool datenOk = dateinameVorhanden && ImpulsverhältnisVorhanden;



      return datenOk;

    }

    private DialogResult MessageboxWarnung(string msgText)
    {
      // Mesagebox wz_msgBox mit Titel und Text parametriert
      //
      MessageBoxButtons button = MessageBoxButtons.OK;
      MessageBoxIcon icon = MessageBoxIcon.Warning;

      string überschrift = "Achtung: Erforderliche Werte fehlen!";
      string meldung = msgText;

      DialogResult antwort = MessageBox.Show(meldung, überschrift, button, icon);

      return antwort;

    }




    /// <summary>
    /// beim Eintritt in die Textbox des Seillängen Impulsverhältnisses muss
    /// der errorProvider1 gelöscht werden
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EDT_SeilImpulsverhältnis_Enter(object sender, EventArgs e)
    {
      errorProvider1.Clear();
    }

    /// <summary>
    /// beim eintreten in die Registerkarte des SAA- Dialoges wird ermittelt, ob in den 
    /// Eingabefeldern eine Veränderung stattgefunden hat. Dies ist insbesondere
    /// bei der Umschaltung der Checkbox für die Regression erforderlich.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void REG_SAA_Enter(object sender, EventArgs e)
    {

      bool geändert = EDT_RegKonstante.Modified ||
                      EDT_REG_1_Ordnung.Modified ||
                      EDT_REG_2_Ordnung.Modified ||
                      EDT_REG_3_Ordnung.Modified ||
                      EDT_REG_4_Ordnung.Modified ||
                      EDT_REG_5_Ordnung.Modified ||
                      EDT_REG_6_Ordnung.Modified ||
                      EDT_REG_7_Ordnung.Modified ||
                      EDT_REG_8_Ordnung.Modified ||
                      EDT_akzLagedifferenz.Modified ||
                      EDT_StartBeiSeillänge.Modified;


      //  hat eine Veränderung stattgefunden so gilt dieser Dialog als initialisiert und
      //  wird beim nächsten Eintritt in der Registerkarte nicht erneut initialisiert, damit nicht
      //  bereits eingegebenen Daten überschrieben werden.
      //
      if (geändert == true)
        saaDialogIstInitialisiert = true;

      //  bei erstmaliger Dialoginitialisierung, werden die Regressionsparameter auf Standardwerte
      //  gesetzt
      if (saaDialogIstInitialisiert == false)
      {
        initRegOrdnungen();
      }
    }





    /// <summary>
    /// Standardvorgaben für die Regressionsrechnung
    /// </summary>
    private void initRegOrdnungen()
    {
      EDT_StartBeiSeillänge.Text = "0";
      EDT_RegKonstante.Text = "1,0";
      EDT_REG_1_Ordnung.Text = "0,0";
      EDT_REG_2_Ordnung.Text = "0,0";
      EDT_REG_3_Ordnung.Text = "0,0";
      EDT_REG_4_Ordnung.Text = "0,0";
      EDT_REG_5_Ordnung.Text = "0,0";
      EDT_REG_6_Ordnung.Text = "0,0";
      EDT_REG_7_Ordnung.Text = "0,0";
      EDT_REG_8_Ordnung.Text = "0,0";
    }




    /// <summary>
    /// Schaltfläche zum Starten des SAA- Dialogs mit Plausibilitätsprüfung, anschließendem Schreiben der
    /// Parameterdatei für diesen Dialog und dem Start der eigentlichen Anwendung
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_SAAStart_Click(object sender, EventArgs e)
    {
      saaDialogIstInitialisiert = true;

      saaDialogdaten = new CSAADialogdaten(prgEinstellungen, dateiNamen);


      if (checkSeildurchmesser() < 0)
      {
        return;
      }

      if (plausibilitätsPrüfung() == false) return;

      aktuellerDatenpfad = Path.GetDirectoryName(EDT_DBListeHeben.Text);


      schreibeSAAPardatei();
      starteSAA();


    }





    /// <summary>
    /// Start der Anwendung SAA über eine Stapelverarbeitungsdatei, damit bei der Ausführung des eigentlichen
    /// Programms eventuell auftretende Fehler in der DOS-Box sichtbar bleiben (letzte Befehl in der Stapelverarbeitungsdatei
    /// ist der Befehl "Pause"
    /// </summary>
    private void starteSAA()
    {
      schreibeStartSAABatch();


      Process saaPro = new Process();
      string dateipfad = Path.Combine(System.Windows.Forms.Application.UserAppDataPath, saaParDateiname);

      string exeabsolut = prgEinstellungen.SaaEXEPfad + "\\Start_saaproRegr.bat";

      saaPro.StartInfo.FileName = exeabsolut;
      saaPro.StartInfo.WorkingDirectory = prgEinstellungen.SaaEXEPfad;
      saaPro.StartInfo.UseShellExecute = false;
      saaPro.Start();
    }





    /// <summary>
    /// Erzeugung der Parameterdatei für das Programm SAA. Diese Datei wird in dem angegebenen Pfad, in dem sich auch
    /// die ausführbare Datei befindet abgelegt.
    /// </summary>
    private void schreibeSAAPardatei()
    {
      //
      // QST: 2016052300
      //
      //string dateipfad = Path.Combine(EDT_SAAExePfad.Text, saaParDateiname);
      string dateipfad = Path.Combine(dateiNamen.SaaUserDatenPfad, saaParDateiname);

      FileStream fs = new FileStream(dateipfad, FileMode.Create, FileAccess.Write, FileShare.None);
      StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);


      schreibeDateinamen(sw);
      schreibeSeildurchmesser(sw);
      schreibeRegressionsparameter(sw);
      sw.Close();
      fs.Close();

      sichereSAAParDatei(dateipfad);
    }




    /// <summary>
    /// Schreiben des Seildurchmessers in die Parameterdatei
    /// </summary>
    /// <param name="tw"></param>
    private void schreibeSeildurchmesser(StreamWriter tw)
    {

      tw.WriteLine("{Seilnenndurchmesser in mm}");
      tw.WriteLine(saaDialogdaten.Seildurchmesser.ToString("F0", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

    }





    /// <summary>
    /// Schreiben der ausgewählten Dateinamen für die Drahtbruchlisten in die Parameterdatei
    /// </summary>
    /// <param name="tw"></param>
    private void schreibeDateinamen(StreamWriter tw)
    {
      tw.WriteLine("{unabgeglichene DB-Liste heben}");
      tw.WriteLine(saaDialogdaten.DrahtbruchlisteHeben);
      tw.WriteLine();

      tw.WriteLine("{unabgeglichene DB-Liste senken oder heben2}");
      tw.WriteLine(saaDialogdaten.DrahtbruchlisteSenken);
      tw.WriteLine();


      tw.WriteLine("{Abgleichsliste}");
      tw.WriteLine(saaDialogdaten.AbgleichslistenDateiname);
      tw.WriteLine();

      tw.WriteLine("{Bestaetigungsliste}");
      tw.WriteLine(saaDialogdaten.BestätigunglistenDateiname);
      tw.WriteLine();
    }




    /// <summary>
    /// Schreiben der Parameter für die Regression in die Parameterdatei
    /// </summary>
    /// <param name="tw"></param>
    private void schreibeRegressionsparameter(StreamWriter tw)
    {

      string text = "{Koeffizienten fuer Ausgleichsrechnung zum Eleminieren des Einflusses aus dem Reibradschlupf (polynomiale Regression 8. Ordnung}\n";
      text += "{durch Anwendung eines berechneten gleitenden Verhaeltnisses Seillaenge HSchrieb / SSchrieb oder HSchrieb / HSchrieb2 fuer Korrektur der LiS}\n";
      text += "{wenn keine Ausgleichsrechnung gewuenscht wird, wird nur das konstante Glied (Constant proportion) auf 1 und alle anderen Koeffizienten auf 0 gesetzt}\n";
      tw.WriteLine(text);

      tw.WriteLine("{Constant proportion}");
      tw.WriteLine(saaDialogdaten.RegressionsKontante.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{First order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung1.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Second order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung2.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Third order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung3.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Fourth order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung4.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Fifth order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung5.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Sixth order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung6.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Seventh order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung7.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Eighth order coefficient}");
      tw.WriteLine(saaDialogdaten.RegressionOrdnung8.ToString("E6", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{Start of the correction ab Seilmeter}");
      tw.WriteLine(saaDialogdaten.StartRegression.ToString("F2", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();

      tw.WriteLine("{akzeptierte Lagedifferenz nach Korrektur in m}");
      tw.WriteLine(saaDialogdaten.Lagedifferenz.ToString("F2", NumberFormatInfo.InvariantInfo));
      tw.WriteLine();
    }





    /// <summary>
    /// Prüfung der Dialogdaten auf ihre Gültigkeit
    /// </summary>
    /// <returns></returns>
    private bool plausibilitätsPrüfung()
    {
      bool fehlerAufgetreten = false;
      errorProvider1.Clear();
      string fehlerText = "kein Fehlertext";
      //
      //  Eingegebene Dateinamen auf existenz prüfen
      //
      if (File.Exists(EDT_DBListeHeben.Text) == false || File.Exists(EDT_DBListeSenken.Text) == false)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Stop;

        string überschrift = "Drahtbruchliste";
        string meldung = "Bitte wählen sie die Drahtbruchliste(n) aus!";

        MessageBox.Show(meldung, überschrift, button, icon);
        errorProvider1.SetError(EDT_DBListeHeben, "Drahtbruchliste auswählen!");
        errorProvider1.SetError(EDT_DBListeSenken, "Drahtbruchliste auswählen!");
        return false;
      }
      //
      //  Dialog sichern
      // 
      saaDialogdaten.DrahtbruchlisteHeben = EDT_DBListeHeben.Text;
      saaDialogdaten.DrahtbruchlisteSenken = EDT_DBListeSenken.Text;
      //
      //  Seildurchmesser auf Gültigkeit prüfen
      //
      fehlerText = "Seildurchmesser muss zwischen 25 und 75 mm liegen!";
      double seildurchmesser = checkDouble(EDT_SeildurchmesserTASA, 25, 75, out fehlerAufgetreten, fehlerText);
      EDT_SeildurchmesserTASA.Focus();
      if (fehlerAufgetreten == true) return false;
      //
      // Ich weiss, das ist kein guter Stil, 
      // der Seildurchmesser wurd bisher in SAA einegeben
      //
      saaDialogdaten.Seildurchmesser = seildurchmesser; 
      //
      //  Akzeptierte Lagedifferenz auf Gültigkeit prüfen
      //
      fehlerText = "akzeptierte Lagedifferenz muss zwischen 0 m und 5 m liegen!";
      double akzLagedifferenz = checkDouble(EDT_akzLagedifferenz, 0, 5, out fehlerAufgetreten, fehlerText);
      EDT_akzLagedifferenz.Focus();
      if (fehlerAufgetreten == true) return false;
      saaDialogdaten.Lagedifferenz = akzLagedifferenz;
      //
      //  bei eingeschalteter Regression die angebenenen Parameter prüfen
      //
      if (CHK_BoxMitRegression.Checked == true)
      {
        //
        //  "startBeiSeillänge" auf Gültigkeit prüfen
        //
        fehlerText = "Start der Regression muss zwischen 0 m und 25 m liegen !";
        double startRegBeiSeillänge = checkDouble(EDT_StartBeiSeillänge, 0, 25, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.StartRegression = startRegBeiSeillänge;
        //
        //  Konstante für die Regression auf Gültigkeit prüfen
        //
        fehlerText = "Konstante für die Regression mus zwischen -10  und 10  liegen !";
        double regKosntante = checkDouble(EDT_RegKonstante, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionsKontante = regKosntante;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_1 = checkDouble(EDT_REG_1_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung1 = regOrdnung_1;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_2 = checkDouble(EDT_REG_2_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung2 = regOrdnung_2;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_3 = checkDouble(EDT_REG_3_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung3 = regOrdnung_3;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_4 = checkDouble(EDT_REG_4_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung4 = regOrdnung_4;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_5 = checkDouble(EDT_REG_5_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung5 = regOrdnung_5;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_6 = checkDouble(EDT_REG_6_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung6 = regOrdnung_6;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_7 = checkDouble(EDT_REG_7_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung7 = regOrdnung_7;
        //
        //  Erste Ordnung für die Regression auf Gültigkeit prüfen
        //
        double regOrdnung_8 = checkDouble(EDT_REG_8_Ordnung, -10, 10, out fehlerAufgetreten, fehlerText);
        if (fehlerAufgetreten == true) return false;
        saaDialogdaten.RegressionOrdnung8 = regOrdnung_8;

      }
      //  es sind keine Fehler vorhanden besser wird der Dialog mit "true" verlassen
      //
      return true;

    }

    
    /// <summary>
    /// Checkbox für die auswahl der Empfindlichkeit behandeln
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CHK_Empfindlichkeit_CheckedChanged(object sender, EventArgs e)
    {
      if (CHK_Empfindlichkeit.Checked == true)
      {
        CBX_Empfindlichkeit.SelectedIndex = 5; // Standard 0.8
        CBX_Empfindlichkeit.Enabled = false;
      }
      else
      {
        CBX_Empfindlichkeit.Enabled = true;
      }

    }


    #endregion // Registerkarte TASA

    #region Registerkarte SAA
    /// <summary>
    /// Schaltfläche zur Auswahl des Speicherortes für die ausführbare Datei SAA____.EXE
    /// auszuwählen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_SAAExePfad_Click(object sender, EventArgs e)
    {
      OpenFileDialog fd = new OpenFileDialog();
      fd.Filter = "Ausführbare Dateien (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
      fd.Title = "Ordner in der sich SAA__.EXE befindet";
      fd.InitialDirectory = prgEinstellungen.SaaEXEPfad;

      if (fd.ShowDialog() == DialogResult.OK)
      {
        string filename = fd.FileName;
        string pfad = Path.GetDirectoryName(filename);
        prgEinstellungen.SaaEXEPfad = pfad;
        EDT_SAAExePfad.Text = prgEinstellungen.SaaEXEPfad;

        string exeName = Path.GetFileName(filename);
        prgEinstellungen.SaaEXEName = exeName;
        EDT_SAAExeName.Text = prgEinstellungen.SaaEXEName;

      }
    }


    /// <summary>
    /// Schaltfläche zur Auswahl der Drahtbruchlisten für den SAA- Dialog. Dieser Dialog wird zweimal,
    /// direkt hintereinander aufgerufen, um als erstes die Drahtbruchliste für den Hebevorgang und
    /// als zweites die Drahtbruchliste für den Senkorgang(oder das zweite Heben)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_DrahtbruchlistenAuswahl_Click(object sender, EventArgs e)
    {
      auswählenDrahtbruchlistendateinamen();
    }

    private void auswählenDrahtbruchlistendateinamen()
    {

      errorProvider1.Clear();
      bool umlauteImText = false;
      DialogResult result;

      do
      {

        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Title = "unabgeglichene Drahtbruchliste heben";
        ofd.Filter = "Db-Liste heben (*.txt)|*fehleranzeigenliste.txt|Alle Dateien (*.*)|*.*";

        ofd.InitialDirectory = prgEinstellungen.AktuellerDatenPfad;
        //ofd.InitialDirectory = dateiNamen.AktuellerDatenpfad;

        result = ofd.ShowDialog();

        if (result == DialogResult.OK)
        {
          EDT_DBListeHeben.Text = ofd.FileName;
          umlauteImText = checkAufUmlaute(EDT_DBListeHeben);
          //
          // 24.08.2011  10:37:02
          // 
          if (umlauteImText == false)
          {
            prgEinstellungen.AktuellerDatenPfad = Path.GetDirectoryName(EDT_DBListeHeben.Text);
            dateiNamen.DrahtbruchlistendateinameHeben = EDT_DBListeHeben.Text;
            //
            // 20.10.2011  14:21:57
            // 
            programmNamen2CDateinamen();

          }
        }
        else
        {
          //  wurde der Dialog (die Auswahl der ersten FehleranzeigenlistenDatei
          //  nicht mit DialogResult.OK beendet, wird nachfolgende Dialog nicht
          //  ausgeführt
          //
          return;
        }


      } while (umlauteImText == true && result == DialogResult.OK);

      do
      {
        OpenFileDialog ofd = new OpenFileDialog();

        ofd.Title = "unabgeglichene Drahtbruchliste senken oder heben 2";
        ofd.Filter = "Db-Liste senken oder heben 2 (*.txt)|*fehleranzeigenliste.txt|Alle Dateien (*.*)|*.*";

        ofd.InitialDirectory = prgEinstellungen.AktuellerDatenPfad;

        result = ofd.ShowDialog();

        if (result == DialogResult.OK)
        {
          EDT_DBListeSenken.Text = ofd.FileName;
          umlauteImText = checkAufUmlaute(EDT_DBListeSenken);
          dateiNamen.DrahtbruchlistendateinameSenken = EDT_DBListeSenken.Text;

        }
      } while (umlauteImText == true && result == DialogResult.OK);
    }


   



    /// <summary>
    /// Umschaltung der Checkbox und Umschaltung auf "mit Regression" oder auf "ohne Regression"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CHK_BoxMitRegression_CheckedChanged(object sender, EventArgs e)
    {
      initRegOrdnungen();

      if (CHK_BoxMitRegression.Checked == true)
      {
        GBX_Regression.Enabled = true;
        EDT_StartBeiSeillänge.Focus();
      }
      else
      {
        EDT_SeildurchmesserTASA.Focus();
        GBX_Regression.Enabled = false;
      }
    }



    #endregion // SAA-Registerkarte

    #region Fehlerstellen

    private void REG_Fehlerstellen_Enter(object sender, EventArgs e)
    {
      if (fehlerListeGeladen == false)
      {
        BTN_DbListeAusgeben.Enabled = false;
        BTN_FehlerEinfügen.Enabled = false;
      }
    }


    private string auswahlFehlerstellendatei()
    {
      string dbFehlerListenDatei = "";

      OpenFileDialog fd = new OpenFileDialog();
      fd.Filter = "Fehlerliste (*.txt)|*Seil_Fehleranzeigen.txt|Alle Dateien (*.*)|*.*";
      fd.Title = "Fehlerstellenliste aus SAA";
      fd.InitialDirectory = prgEinstellungen.AktuellerDatenPfad;

      if (fd.ShowDialog() == DialogResult.OK)
      {
        dbFehlerListenDatei = fd.FileName;
        EDT_Fehlerlistendatei.Text = dbFehlerListenDatei;
        prgEinstellungen.AktuellerDatenPfad = Path.GetDirectoryName(EDT_Fehlerlistendatei.Text);
      }

      return dbFehlerListenDatei;
    }


    /// <summary>
    /// Seilfehlerliste in der ListView anueigen
    /// </summary>
    /// <param name="listeReadonly"></param>
    //    private void anzeigenSeilfehler(IList<CSeilFehlerstelle> listeReadonly)
    private void anzeigenSeilfehler()
    {

      LVW_Seilfehler.Items.Clear();

      List<ListViewItem> lvItemsCollection = new List<ListViewItem>(100);

      foreach (CSeilFehlerstelle f in fehlerliste)
      {
        ListViewItem lvi = new ListViewItem(f.Fehlernummer);
        lvi.SubItems.Add(f.FehlerpositionHeben);
        lvi.SubItems.Add(f.FehlerTyp);
        lvi.SubItems.Add(f.FehlerpositionSenken);
        lvi.SubItems.Add(f.FehlerHerkunft);

        if (f.FehlerAusgewählt == true)
          lvi.Checked = true;

        lvItemsCollection.Add(lvi);
      }

      LVW_Seilfehler.BeginUpdate();
      LVW_Seilfehler.Items.AddRange(lvItemsCollection.ToArray());
      LVW_Seilfehler.EndUpdate();


      updateDrahtbruchstatistik();

      // auf optimale Spaltenbreite setzen
      //
      for (int i = 0; i < LVW_Seilfehler.Columns.Count; i++)
        LVW_Seilfehler.Columns[i].Width = -2;

    }



    private void BTN_FehlerlistenDatei_Click(object sender, EventArgs e)
    {
      //
      //  sind alles Daten für die Klasse CDateinamen gesetzt?
      //  Aus dieser Klasse heraus werden die Pfade und Namen für 
      //  die Ausgabe des Dialogs auf der Registerkarte "Fehlerstellen"
      //  ernmittelt!
      // Ist das nicht der Fall, diese noch nachträglich auswählen 
      //
      if (dateiNamen.ErforderlicheDatenOK == false)
      {
        userBenachrichtigen();
        auswählenHeaderDatei();
        auswählenDrahtbruchlistendateinamen();
      }


      string dbFehlerListenDatei = auswahlFehlerstellendatei();

      if (dbFehlerListenDatei.Length == 0)
        //
        // Es wurde kein Dateiname ausgewählt (der Dialog wurde mit "Abbruch" beendet
        return; // Abbruch im OpenFileDialog 

      try
      {
        //  einlesen der Fehler un bereitstellung der Daten als Readonly-Collection
        //
        fehlerliste = new CSeilFehlerstellenListe(dbFehlerListenDatei);
        IList<CSeilFehlerstelle> listeReadonly = fehlerliste.giblistenDaten();
        //
        //  Seilfehler in ListView anzeigen
        //
        anzeigenSeilfehler();
        //
        //  Zustand merken und Schaltföächen freigeben
        //
        fehlerListeGeladen = true;
        BTN_DbListeAusgeben.Enabled = true;
        BTN_FehlerEinfügen.Enabled = true;
        BTN_AlleAbwählen.Enabled = true;
        BTN_AlleAuswählen.Enabled = true;
        //
        //  Anzeige der Drahtbruchstatistik
        //
        updateDrahtbruchstatistik();

      }
      catch (Exception ex1)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Stop;

        string überschrift = "Achtung: Ungültige Daten ";
        string meldung = "Die Datei mit den SAA-Drahtbruchdaten kann nicht gelesen werden!\n";
        meldung += ex1.Message;

        MessageBox.Show(meldung, überschrift, button, icon);

      }
    }

    private void userBenachrichtigen()
    {
      //Mesagebox wz_msgBox mit Titel und Text parametriert
      //
      MessageBoxButtons button = MessageBoxButtons.OK;
      MessageBoxIcon icon = MessageBoxIcon.Asterisk;
      string programmName = System.Windows.Forms.Application.ProductName;
      string überschrift = programmName + ": Nicht alle Daten sind bekannt!";
      string meldung = "Sie sind in den Dialog eingestiegen, ohne in der gleichen Sitzung";
      meldung += "\ndie Registerkarte TASA und SAA abgearbeitet zu haben. Dadurch sind nicht";
      meldung += "\nalle Dateien und Pfade bekannt. Bitte wählen Sie nun die enstprechenden";
      meldung += "\nDateien aus. Beachten Sie dabei bitte die Fenstertitel der Dialoge!";
      

      MessageBox.Show(meldung, überschrift, button, icon);

    }


    /// <summary>
    /// Neuen Fehler einfügen 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_FehlerEinfügen_Click(object sender, EventArgs e)
    {
      DlgFehlerstelleInsert dlgFehler = new DlgFehlerstelleInsert();

      //  wird in LVW_Seilfehler_ItemChecked festgehalten und muss hier gesichert
      //  werden, das beim einfügen einenes neuen Otems dieses sofort wieder
      //  'gechecked' wird udn somit die aktuelle position überschreiben wird
      //
      //int letzeAktuelleGechekteItemNr = aktuellePosition;

      CSeilFehlerstelle fehlerNeu;

      if (dlgFehler.ShowDialog() == DialogResult.OK)
      {
        //  Dialogdaten aus CSeilFehlerstelle
        //
        fehlerNeu = dlgFehler.gibFehlerStelle();
        //
        if (fehlerNeu == null)
          return; // es ist etwas schief gegangen
        //
        //  ist der neu eigebene Fehler bereits vorhanden, kann der Anwender entscheiden, ob dieser
        //  ersetzt wird oder nicht
        //
        CSeilFehlerstelle fsOriginal;
        //
        //  zum Löschen wird der original im Container "fehlerliste" vohandene Fehler gebraucht
        //  Wird die Hebenposition im Container durch istDerFehlerVorhanden(..) gefunden  
        //  so wird das Original das dem neuen Fehler im Container enstspricht zurückgegeben. 
        //  Gibet es die angebene Position nicht, wird null zurückgegeben
        //  Container gelöscht werden.
        //
        if ((fsOriginal = istDerFehlerVorhanden(fehlerNeu)) != null)
        {
          //  den Anwender fragen ob der Fehler ersetzt werden soll (der Fehler war vorhanden)
          //
          DialogResult ersetzen = sollFehlerErsetztWerden(fehlerNeu);
          if (ersetzen == DialogResult.Yes)
          {
            //  der Anwender hat sich entschieden den Fehler zu ersetzen
            //  es wird nun versucht den originalen Fehler zu löschen
            //
            bool result = fehlerliste.löscheAnPosition(fsOriginal);
            if (result == false)
            {
              // Mesagebox wz_msgBox mit Titel und Text parametriert
              //
              MessageBoxButtons button = MessageBoxButtons.OK;
              MessageBoxIcon icon = MessageBoxIcon.Asterisk;

              string überschrift = "Achtung: Einfügen des Fehlers fehlgeschlagen";
              string meldung = "Der Usprungsfehler konnte an der angebenen Position nicht gelöscht werden!";

              MessageBox.Show(meldung, überschrift, button, icon);

              return;
            }

          }
          else // der Anwender hat die Frage zum Ersetzen mit Nein beantwortet
            return;
        }
        //
        //  der Fehler wird eingefügt
        //
        fehlerliste.einfügenFehler(fehlerNeu);

        fehlerliste.setzeFehlerAusgewählt(fehlerNeu.FehlerpositionHeben, fehlerNeu.FehlerpositionSenken);

        LVW_Seilfehler.Items.Clear();

        IList<CSeilFehlerstelle> listeReadonly = fehlerliste.giblistenDaten();

        //anzeigenSeilfehler(listeReadonly);
        anzeigenSeilfehler();

        positioniereAufNeueingegebenenFehler(fehlerNeu);

        //  Den Zähler für manuell hinzugefügte Drahtbrüche erhöhen und Felder auf
        //  Registerkarte Fehlerstellen updaten
        //
        updateDrahtbruchstatistik();

      }

    }

    /// <summary>
    /// Prüfung ob ein neu eingegebener Fehler im Datenbestand bereits vorhanden ist
    /// </summary>
    /// <param name="fehlerNeu">Objekt des neuen Fehlers</param>
    /// <returns>die originale Fehlerstelle, oder null wenn nicht vorhanden</returns>
    private CSeilFehlerstelle istDerFehlerVorhanden(CSeilFehlerstelle fehlerNeu)
    {

      //  Prüfen ob die Hebenpsoition in den Daten schon vorhanden ist
      //
      foreach (CSeilFehlerstelle fs in fehlerliste)
      {
        //  es wird nur die Position für das Heben verglichen
        //
        if (fs.FehlerpositionHeben == fehlerNeu.FehlerpositionHeben)
        {
          return fs;  // das Original vohandene Objekt zurückliefern, denn dieses
                      //  wird u.U. zum löschen des selben gebraucht
        }
      }

      return null;
    }


    /// <summary>
    /// der Anwender wird gefragt ob er den neu eingegebenen Fehler ersetzen möchten
    /// </summary>
    /// <param name="fehlerNeu"></param>
    /// <returns>DialogResult.Yes / DialogResult.No </returns>
    private DialogResult sollFehlerErsetztWerden(CSeilFehlerstelle fehlerNeu)
    {
      DialogResult result;
      // Mesagebox wz_msgBox mit Titel und Text parametriert
      //
      MessageBoxButtons button = MessageBoxButtons.YesNo;
      MessageBoxIcon icon = MessageBoxIcon.Stop;

      string überschrift = "Achtung:Doppelt eigegebener Fehler";
      string meldung = "Der Fehler an der Position " + fehlerNeu.FehlerpositionHeben + " ist bereits vorhanden!";
      meldung += "\n Soll der Fehler ersetzt werden ?";
      
      result = MessageBox.Show(meldung, überschrift, button, icon);
     

      return result;
    }


    /// <summary>
    /// Wurde ein Fehler eingefügt, so wurde auch die ListView neu gefüllt. Dadurch
    /// wird wieder der erste Eintrag der Liste angezeigt, auch wenn der Benutzer am bspw.
    /// 60 Eintrag arbeitete und den 61 Eintrag neu eingefügt hat. Durch das Merken
    /// des letzt eingetragenen Seilfehlers kann mit dieser Methode nach diesem Fehler
    /// in der Listview gesucht werden und den Index der Eintrages bestimmt werden.
    /// Anachliessend wird auf diese Zeile positioniert und diese in den sichbaren Bereich
    /// gebracht. Weiterhin wird geprüft ob noch weiter gescrollt werden kann, um 
    /// den aktuell eingefügten Fehler in die Mitte der Listview zu bringen
    /// </summary>
    /// <param name="fehlerNeu"></param>
    private void positioniereAufNeueingegebenenFehler(CSeilFehlerstelle fehlerNeu)
    {
      foreach (ListViewItem lvi1 in LVW_Seilfehler.Items)
      {
        // Sichtabren Eintga wenn möglich in die Mitte der ListView
        //
        const int scrollzugabe = 5;

        ListViewItem.ListViewSubItemCollection subItem = lvi1.SubItems;
        ListViewItem.ListViewSubItem posHeben = subItem[1];
        ListViewItem.ListViewSubItem posSenken = subItem[2];

        if (posHeben.Text == fehlerNeu.FehlerpositionHeben)
        {
          //  Wenn möglich Eintrag in die Mitte der ListView scrollen
          //
          int index = lvi1.Index;
          int mitteListview = index + scrollzugabe;

          if ((mitteListview) > fehlerliste.gibAnzahlAllerFehler - 1)
          {
            LVW_Seilfehler.Items[index].Selected = true;
            LVW_Seilfehler.EnsureVisible(index);
          }
          else
          {
            LVW_Seilfehler.Items[index].Selected = true;
            LVW_Seilfehler.EnsureVisible(mitteListview);
          }

        }
      }
    }



    /// <summary>
    /// Wurde eine Fehlerstelle 'gechecked', so nuss diese 'Auswahl' auch in der Klasse 
    /// durchgeführt werden
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LVW_Seilfehler_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
      ListViewItem item = e.Item;
      ListViewItem.ListViewSubItemCollection subItem = item.SubItems;
      ListViewItem.ListViewSubItem posHeben = subItem[1];
      ListViewItem.ListViewSubItem posSenken = subItem[3];

      if (item.Checked == true)
      {
        fehlerliste.setzeFehlerAusgewählt(posHeben.Text, posSenken.Text);
      }
      else
      {
        fehlerliste.löscheFehlerAusgewählt(posHeben.Text, posSenken.Text);
      }
      //
      //  Dathbruchstatistik neu berechnen und in Maske ausgeben
      //
      updateDrahtbruchstatistik();
    }


    /// <summary>
    /// Da der Seildurchmesser für mehrere Dialoge (SAA und Fehlerszellen) gebraucht wird
    /// wurde die Überprüfung von der übergeordneten Prüfung heausgenommen  
    /// </summary>
    /// <returns></returns>
    private double checkSeildurchmesser()
    {
      string text = "Fehlender bzw. ungültiger Seildurchmesser";
      bool fehler;
      double seildurchmesser;
      seildurchmesser = checkDouble(EDT_SeildurchmesserTASA, 25, 75, out fehler, text);
      if (fehler == true)
      {
      ungültigerSeildurchmesser();
      REG_Auswerteparameter.SelectedIndex = (int)REGISTERKARTE.TASA;
      EDT_SeildurchmesserTASA.Focus();
      //
      //  Durch einen seildurchmesser < 0 ekennt die übergeordnete Funktion 
      //  einen fehlhaften Seildurchmesser
      //
      seildurchmesser = -1.0;
      }

    return seildurchmesser;
 
    }


    
    private double gibGeprüfteSeillänge()
    {
      double geprSeillänge = 0.0;

      try
      {
        string []alleZeilen;
        //
        // Textdatei laden (zZeile wurden beil Laden bereits modifiziert (Return entfernt)
        //
        alleZeilen = ladeTasaIndfomationsDatei();
        //
        //  alle Zeile durchlaufen und Inder der Zeile mit dem Inhalt 
        //  "gepruefte Seillaenge" ermitteln
        string zeile = "";
        foreach (string s in alleZeilen)
        {
          if (s.IndexOf("gepruefte Seillaenge") > -1)
          {
            zeile = s;
            break;
          }
        }


        // Zeile enthält den ganzen Text der Zeile "gepruefte Seillaenge 193.87 m"
        //  diese Zeile in Einzelemenete aufsplitten
        //
        string[] alleWerteDerZeile = zeile.Split(' ');

        int zeileDieEinenPunktEnthält = 0;

        // Array nach einer Zeile durchsuchen,die einen Punkt enthält
        //  das müsset dann die gepr. Seillänge sein.

        for (int i = 0; i < alleWerteDerZeile.Length; i++)
          if (alleWerteDerZeile[i].IndexOf('.') > -1)
            zeileDieEinenPunktEnthält = i;

        // Punkt durch Komma ersetzen und in Doubel Konvertieren
        //
        string geprSeillängeText = alleWerteDerZeile[zeileDieEinenPunktEnthält].Replace('.', ',');
        geprSeillänge = Convert.ToDouble(geprSeillängeText);
      }
      catch (Exception)
      {
        MessageboxWarnung("Die geprüfte Seillänge konnte nicht ermittelt werden!");
      }

      return geprSeillänge;
    }


    private string[] ladeTasaIndfomationsDatei()
    {

      //  neue Verarbeitung des Dateinamens 
      //
      //  Udo löscht die Heraderextension aus dem Dateinamen 
      //  Beispiel: aus Bg_258_RH1.DDF.DAT wird Bg_258_RH1.DDF 
      //  An diesen Namen hängt Udo ".TID" an damit wird die
      //  Tasa-Informationsdatei zuB g_258_RH1.DDF.TID
      //
      //  Da für mich der ausgewählte Dateiname (TASA) Bg_258_RH1.DDF.DAT ist
      //  würde mein Programm nun für die Tasa-Informationsdatei den Namen
      //  Bg_258_RH1.DDF.DAT.TID erwarten
      //
      //  Udo liefert jedoch den Namen ohne die extension ".DAT" somit
      //  wird die Tasa-Informationsdatei Bg_258_RH1.DDF.TID geschrieben
      //

      string[] tasaInfoFiles = Directory.GetFiles(prgEinstellungen.AktuellerDatenPfad, "*.tid");
      

      // nur die Hebendatei auswählen
      //
      string infoDateiNameTASA="Tasa-InfoDateiname undefiniert";

 
      
      try
      {
        //
        //  ab sofort (das habe ich entschieden!) wird die TID-Datei des
        //  aktuellen TASA-Laufs in der Listbox gezeigt, es wird nicht wie
        //  bisher versucht, die *.TID des Hebenschriebes zu zeigen
        //
        string TidInfoDateiNameTASA = dateiNamen.TidDateiname;

        string[] alleZeilen;
        string alleInfos = File.ReadAllText(TidInfoDateiNameTASA);

        alleZeilen = alleInfos.Split('\n');

        // Return aus allen Zeilen entferenen
        //
        for (int i = 0; i< alleZeilen.Length; i++)
          alleZeilen[i] = alleZeilen[i].TrimEnd('\r');
 
  
        return alleZeilen;

      }
      catch (Exception)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Exclamation;

        string überschrift = "Achtung: Fehler beim lesen aus der TASA-Informationsdatei";
        string meldung = "Aus der Informationdatei zum TASA-Lauf (*.tid) konnte nicht gelesen werden!";

        MessageBox.Show(meldung, überschrift, button, icon);


      }

      return null;

    }

    /// <summary>
    /// Ausgabe der kompletten Drahtbruchliste (einzel festgestellte Drahtbrüche und die
    /// Ausgabe der Bewertung auf die Bezugslängen 6 x D und 30 x D als
    /// Textdatei und Importdatei für Word schreiben
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_DbListeAusgeben_Click(object sender, EventArgs e)
    {
     
      // --- 23.08.2011 11:17:47 --- 
      //  Prüfdatum und Diagramm überschrift von Anwender abfragen
      // 
      DlgDiagrammdaten dlg = new DlgDiagrammdaten();
      dlg.ShowDialog();
      //
      DateTime prüfdatum = dlg.Prüfdatum;
      string diagrammTitel = dlg.Diagrammüberschrift;
      //
      //  Seildurchmesser darf nicht <= 0 sein 
      //  in checkSeildurchmesser() wird eine Exception
      //  ausgegeben wenn dieser Fehlt oder ungültig ist
      //  Die Rückgabe von 0 aus checkSeildurchmesser()
      //  wird benutzt um den Rückssprung aus hier dieser
      //  Methode einzuleiten
      //
      double seildurchmesser = 0;
      if ((seildurchmesser = checkSeildurchmesser())  < 0 )
        return;

      //
      // 
      this.Enabled = false;
      this.Update();


      //  Bezugslängen für 6 und 30 mal Nennurchmesser auswerten
      //
      BezugsLängenListe fehler6xD = new BezugsLängenListe();
      fehler6xD = bewertungBezuglänge_Zil(seildurchmesser, 6);
  
      BezugsLängenListe fehler30xD = new BezugsLängenListe();
      fehler30xD = bewertungBezuglänge_Zil(seildurchmesser, 30);
      //
      //  unveränderte Ausgabe in Textform ausgabedateiName = "D:\\DIA\\Seilpruefung\\Daten\\FehlerKraus\\BG_284_L_Seil_Fehler.txt"
      //
      // --- 21.10.2011 09:47:56 --- 
      // 
      // string ausgabedateiName = fehlerAusgabeDateiname();
      // ToDo: 
      string ausgabedateiName = dateiNamen.BestätigungslistennDateiname;
      TextWriter tw = new StreamWriter(ausgabedateiName);

      schreibeKopfaten(tw);
      schreibeDrahtbruchListe(tw);
      schreibeKopfatenBezugsLänge(tw);
      schreibeBezugsLängen(tw, seildurchmesser, fehler6xD, 6,AusgabeFormat.TEXTDATEI);
      schreibeBezugsLängen(tw, seildurchmesser, fehler30xD, 30,AusgabeFormat.TEXTDATEI);
      tw.Close();
      //
      //  Importdatei für Word schreiben
      //
      string ausgabedateiWord;
      ausgabedateiWord = fehlerAusgabeDateinameWord();

      TextWriter twWord = new StreamWriter(ausgabedateiWord);
      //
      //  Hier wird der Usprungscontainer modifiziert
      //
      modifiziereDrahtbruchtyp(fehlerliste);
      schreibeDrahtbruchListeWordImport(twWord);
      schreibeKopfatenBezugsLänge(twWord);
      schreibeBezugsLängen(twWord, seildurchmesser, fehler6xD, 6,AusgabeFormat.WORDDATEI);
      schreibeBezugsLängen(twWord, seildurchmesser, fehler30xD, 30,AusgabeFormat.WORDDATEI);
 
      twWord.Close();

      //  Sonderfall: Es wurde nur gewünscht, die Wordimportdatei auzugeben!
      //
      if (auswerteModus == AuswerteModus.WORDIMPORTDATEI)
      {
        // Form wieder für Eingaben öffnen
        this.Enabled = true;
        this.Update();
        return;
      }

      //
      // 11.08.2011  06:44:01
      // 
      //  Exceldatei mit den Werten der Seilfehler- und Anzahl der Fehler auf eine
      //  bestimmet Bezugslänge füllen. Dabei werden natürlich die Seilfehlerstellen 
      //  in ein Array, das die Seillänge repräsentiert geschrieben, damit die Stellen
      //  ohne Fehler eine äquidistante X-Achse ausweisen
      //
      //  Diese Datei für 30 x D  schreiben
      //
      try
      {
        string excelFilename30D = fehlerGrafikDateinameExcel(GRAFIKDATEI.DATEINAME30xD);
        //      TextWriter twExcel30xD = new StreamWriter(excelFilename30D);
        schreibeExcelGrafik(excelFilename30D, fehler30xD, prüfdatum, diagrammTitel, GRAFIKDATEI.DATEINAME30xD);
        //
        //  das ganze auch für 6 x D 
        //
        string excelFilename6D = fehlerGrafikDateinameExcel(GRAFIKDATEI.DATEINAME6xD);
        schreibeExcelGrafik(excelFilename6D, fehler6xD, prüfdatum, diagrammTitel, GRAFIKDATEI.DATEINAME6xD);



        //  Dialog auf Ausgangszustand zurücksetzen (Liste löschen, Button deaktivieren etc.)
        //
        initRegkarteFehlerstellen();

        string text1 = "Das Ergebnis der Auswertung steht in den Dateien\n" + ausgabedateiName + " und\n" + ausgabedateiWord;
        MessageBox.Show(text1, "Ausgabedateien wurden erzeugt!", MessageBoxButtons.OK, MessageBoxIcon.Information);


      }
      catch (Exception)
      {
        //Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Asterisk;
        string programmName = System.Windows.Forms.Application.ProductName;
        string überschrift = programmName + ": Excel-Grafiken erzeugen";
        string meldung = "Die Excel-Dateien für die grafische Aufbereitung der Seilfehler konnten nicht erzeugt werden!";

        MessageBox.Show(meldung, überschrift, button, icon);
      }
      finally
      {
        this.Enabled = true;

      }
    }



    /// <summary>
    /// Excelgrafik mit Werten füllen
    /// </summary>
    /// <param name="zielExcelFilename"></param>
    /// <param name="fehlerStellen"></param>
    /// <param name="dateiTyp"></param>
    private void schreibeExcelGrafik(string zielExcelFilename, BezugsLängenListe fehlerStellen,DateTime prüfdatum, string diagrammtitel, GRAFIKDATEI dateiTyp)
    {
      if (istExcelInstalliert() == false)
      {
        //Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Asterisk;
        string programmName = System.Windows.Forms.Application.ProductName;
        string überschrift = programmName + ": Excel-Bezugslängendiagramme! ";
        string meldung = "Für diese Funktionalität muss Excel2003 installiert sein!";

        MessageBox.Show(meldung, überschrift, button, icon);

        return;
      }

      // Todo: 1. Umstellen auf neue Dateinamen aus CDateinamen

      string vorlagenDateiExcel = Path.Combine(System.Windows.Forms.Application.StartupPath, excelVorlagenSeilfehler);

      ExcelApp.ApplicationClass Excel = new ExcelApp.ApplicationClass();
      //
      // 30.01.2012  11:05:55
      // 
      ExcelApp.Workbook arbeitsMappe = null;
      ExcelApp.Worksheet arbeitsBlatt = null;

      try
      {
        // --- 14.08.2011 14:20:55 --- 
        // 
        // 

        if (Excel == null)
        {
          // Mesagebox wz_msgBox mit Titel und Text parametriert
          //
          MessageBoxButtons button = MessageBoxButtons.OK;
          MessageBoxIcon icon = MessageBoxIcon.Asterisk;

          string überschrift = "Achtung: Excel ist nicht installiert";
          string meldung = "Die Anwendung 'Microsoft-Excel' konnte nicht gestartet werden!";

          MessageBox.Show(meldung, überschrift, button, icon);
          return;
        }

        arbeitsMappe = null;
        arbeitsBlatt = null;

        if (File.Exists(vorlagenDateiExcel) == false)
        {
          // Mesagebox wz_msgBox mit Titel und Text parametriert
          //
          MessageBoxButtons button = MessageBoxButtons.OK;
          MessageBoxIcon icon = MessageBoxIcon.Asterisk;

          string überschrift = "Achtung: Excel Vorlagendatei nicht gefunden!";
          string meldung = "Die benötigte Vorlagendatei " + vorlagenDateiExcel + " konnte nicht geöffnet werden!";

          MessageBox.Show(meldung, überschrift, button, icon);
          return;

        }
        //
        // Kopie der Vorlagendatei in Zielordner erstellen 
        //
        if (File.Exists(zielExcelFilename) == true)
          File.Delete(zielExcelFilename);

        File.Copy(vorlagenDateiExcel, zielExcelFilename,true);
        //
        //  Arbeitsmappe öffnen
        //
        arbeitsMappe = Excel.Workbooks.Open(zielExcelFilename, 0, false, 5, "", "", false, ExcelApp.XlPlatform.xlWindows, "",
                      true, false, 0, true, false, false);

        // keine Anzeige von Excel
        Excel.Visible = false;

        // aktives Arbeitsblatt ist die ertste [1] Tabelle 
        arbeitsBlatt = (ExcelApp.Worksheet)arbeitsMappe.Worksheets[1];
        //
        //  Den Excelfilenamen in die Tabelle schreiben
        //  Dies ist auch zugleich die Überschrift des Diagramm und soll
        //  zeitnah durch einen sprechenden Namen erstezt werden
        //
        // geprüfte Seillänge ermitteln
        //
        geprüfteSeillänge = gibGeprüfteSeillänge();

        if (geprüfteSeillänge < 1)
        {
          //Mesagebox wz_msgBox mit Titel und Text parametriert
          //
          MessageBoxButtons button = MessageBoxButtons.OK;
          MessageBoxIcon icon = MessageBoxIcon.Asterisk;
          string programmName = System.Windows.Forms.Application.ProductName;
          string überschrift = programmName + ": geprüfte Seillänge konnte nich ermittelt werden";
          string meldung = "Die Verarbeitung kann nicht fortgesetzt werden, da die geprüfte Seillänge nicht ermittelt werden konnte!";

          MessageBox.Show(meldung, überschrift, button, icon);
          this.Enabled = true;
          this.Update();
          throw new Exception(überschrift);
        }


        arbeitsBlatt.Cells[1, 2] = prüfdatum;
        arbeitsBlatt.Cells[2, 2] = diagrammtitel;
        arbeitsBlatt.Cells[3, 2] = geprüfteSeillänge;
        //
        //  Spaltenübnerschrift je nach Bezugslänge setzen
        //
        if (dateiTyp == GRAFIKDATEI.DATEINAME30xD)
          arbeitsBlatt.Cells[4, 2] = "Anzahl Fehler auf 30 x D";
        else
          arbeitsBlatt.Cells[4, 2] = "Anzahl Fehler auf 6 x D";
        //
        //  höchste Fehlerstelleim Seil suchen (dieser Wert soll zeitnah durch
        //  die wirklich geprüfte Seillänge ersetzt werden
        //int maxXFehlerWert_cm = positionLetzterFehler(fehlerStellen);
        //
        // --- 23.08.2011 13:47:44 --- 
        // 
        // 
        int maxXFehlerWert_cm = (int) (geprüfteSeillänge * 100.0);
        //
        //  Array, dass al Index die Seillänge in cm hat, fü soviele Längenzentimeter
        //  anlegen wie vorhanden
        //
        int[] seillänge = new int[maxXFehlerWert_cm + 1];

        for (int kcm = 0; kcm < maxXFehlerWert_cm; kcm++)
        {
          seillänge[kcm] = 0;
        }
        //
        //  Umspeichern der Seilfehler in das Array, in dem der Index die 
        //  Seillänge in cm repräsentiert 
        //  (Beispiel: 3 Fehler ab 86,12 m -> Index 8612 -> seillänge[8612] = 3
        //  
        foreach (BezugelängenFehler bzl in fehlerStellen.bzlFehler)
        {
          int xWert = Convert.ToInt32(Convert.ToDouble(bzl.fehlerPosition) * 100);
     
          seillänge[xWert] = bzl.anzahlFehlerAufBezugslänge;
        }

       
        object[,] objData = new Object[maxXFehlerWert_cm + 2, 2];
        
        int arrayIndex = 0;

        for (int k = 1; k <= maxXFehlerWert_cm; k++)
        {
          string zeile;
          double seillänge_m = k / 100.0;
          if (seillänge[k] > 0)
          {
            arrayIndex++;
            zeile = seillänge_m.ToString("F2") + "\t" + seillänge[k];
            objData[arrayIndex, 0] = seillänge_m.ToString("F2");
            objData[arrayIndex, 1] = seillänge[k].ToString();
          }
        }

        // 
        //  Exceltábelle mit Werten füllen
        //
        ExcelApp.Range xlsBereich;

        int startZeile = maxXFehlerWert_cm + 5;


        xlsBereich = arbeitsBlatt.get_Range("A5", "B" + startZeile.ToString());
        xlsBereich = xlsBereich.get_Resize(arrayIndex, 2);
        xlsBereich.Value2 = objData;
        // 
        //  Tabellenwerte für den Bereich des Diagramms definieren
        //
        ExcelApp._Chart chart;
        chart = (ExcelApp._Chart)arbeitsMappe.Charts[1];
        chart.SetSourceData(xlsBereich, ExcelApp.XlRowCol.xlColumns);
        // --- 22.08.2011 13:50:45 --- 
        // Achsen formatieren
        // 
        //ExcelApp.Axis xlAxis = (ExcelApp.Axis)chart.Axes(ExcelApp.XlAxisType.xlValue, ExcelApp.XlAxisGroup.xlPrimary);
        const ExcelApp.XlAxisType XAchse = XlAxisType.xlCategory;
        const ExcelApp.XlAxisType YAchse = XlAxisType.xlValue;

        ExcelApp.Axis xl_X_Axis = (ExcelApp.Axis)chart.Axes(XAchse, ExcelApp.XlAxisGroup.xlPrimary);
        xl_X_Axis.MinimumScale = 0;
        xl_X_Axis.MajorUnit = 25;
        xl_X_Axis.MinorUnit = 5;
        xl_X_Axis.MaximumScale = geprüfteSeillänge;
        xl_X_Axis.MinorTickMark = XlTickMark.xlTickMarkOutside;

        ExcelApp.Axis xl_Y_Axis = (ExcelApp.Axis)chart.Axes(YAchse, ExcelApp.XlAxisGroup.xlPrimary);
        xl_Y_Axis.MinimumScale = 0;
        xl_Y_Axis.MaximumScale = fehlerStellen.maxAnzahlFehlerAufBezugslänge + 1;

    
        //
        //  Arbeitsmappe mitel "Speichern unter"  ablegen
        //
        //ExcelApp.XlSaveAsAccessMode modus;
        //modus = ExcelApp.XlSaveAsAccessMode.xlNoChange;
        object NoValue = Missing.Value;

        arbeitsMappe.Save();
        //
        //  Wichtig: Alle offenen Atrbeitsmappen schliessen!
        //
        Excel.Workbooks.Close();
        //
        //  Excel beenden
        //
        // Excel.Quit();
        Excel.Application.Quit();


      }
      catch (Exception ex1)
      {
        throw ex1;

        //// Mesagebox wz_msgBox mit Titel und Text parametriert
        ////
        //MessageBoxButtons button = MessageBoxButtons.OK;
        //MessageBoxIcon icon = MessageBoxIcon.Exclamation;

        //string überschrift = "Exceldiagramm ";
        //string meldung = "Die Erzeugung des Exceldigramms für die Darstellung der Seilfehler je Bezugslänge ist fehlgeschlagen!\n";
        //meldung += "\n";
        //meldung += ex1.Source;
        //MessageBox.Show(meldung, überschrift, button, icon);

        //// bei einer Ausnahme Arbeitsmappe schliessen
        //// und evtl Vorhanden Exceldatei löschen
        //arbeitsMappe.Close(false, Missing.Value, Missing.Value);
        //arbeitsMappe = null;

        //if (File.Exists(zielExcelFilename) == true)
        //  File.Delete(zielExcelFilename);

        throw ex1;

      }
      finally
      {
        //if (arbeitsMappe != null) 
        //  arbeitsMappe.Close(true, Missing.Value, Missing.Value);
        
        Excel.Workbooks.Close();
        Excel.Application.Quit();
        Excel.Quit();

        Marshal.ReleaseComObject(arbeitsBlatt);
        Marshal.ReleaseComObject(arbeitsMappe);
        Marshal.ReleaseComObject(Excel);
      }


    }

    private bool istExcelInstalliert()
    {
      RegistryKey OurKey = Registry.ClassesRoot;
      return (OurKey.OpenSubKey("Excel.Application", false) != null);
    }

    private void meldungKeineFehler()
    {
      	// Mesagebox wz_msgBox mit Titel und Text parametriert
	      //
	      MessageBoxButtons button = MessageBoxButtons.OK;
	      MessageBoxIcon icon = MessageBoxIcon.Asterisk;

	      string überschrift = "Achtung: Keine Fehler im Seil ausgewählt!";
        string meldung = "Es wurden keine Fehler ausgewählt!";

	      MessageBox.Show(meldung, überschrift, button , icon );
	

    }

    
    
    
    
    /// <summary>
    /// alle Seilfehler durchsehen und dadurch die höschste Position suchen 
    /// X-Achsen Maximalwert (normalerweise soll das die gepr. Seillänge sein
    /// </summary>
    /// <param name="fehlerStellen"></param>
    /// <returns></returns>
    private int positionLetzterFehler(BezugsLängenListe fehlerStellen)
    {
      int maxXFehlerWert_cm = 0;
      int i = 0;
      foreach (BezugelängenFehler bzl in fehlerStellen.bzlFehler)
      {
        int xWert = Convert.ToInt32(Convert.ToDouble(bzl.fehlerPosition) * 100);

        if (xWert > maxXFehlerWert_cm)
          maxXFehlerWert_cm = xWert;
        i++;

      }
      return maxXFehlerWert_cm;
    }





    /// <summary>
    /// Für 
    /// </summary>
    /// <param name="fehlerliste"></param>
    private void modifiziereDrahtbruchtyp(CSeilFehlerstellenListe fehlerliste)
    {
      //Aussen DB Typ 1
      //Aussen DB Typ 2 (W-Form)
      //Drahtbruch aussen 3 (ausgebrochene Draehte, Zahnform)
      //Drahtbruch innen (verbreiterte W-Form)
      //Kerbe / Narbe

      foreach(CSeilFehlerstelle f in fehlerliste)
      {
        if (f.FehlerTyp.IndexOf("Aussen DB") > -1 || 
            f.FehlerTyp.IndexOf("Kerbe") > -1 || 
            f.FehlerTyp.IndexOf("Drahtbruch aussen") > -1 ||
            f.FehlerTyp.IndexOf("Drahtbruch in Außenlitze") > -1 )
        
        f.FehlerTyp = "Drahtbruch in Außenlitze";

        else if (f.FehlerTyp.IndexOf("Innerer Fehler") > -1 ||
                 f.FehlerTyp.IndexOf("Drahtbruch innen") > -1)
          f.FehlerTyp = "Fehler im Seilinneren";
        else
          f.FehlerTyp = "unbekannter Fehler!";
       
      }
     }

    private void schreibeKopfatenBezugsLänge(TextWriter tw)
    {
      string text;
      //
      //  Überschrift für die entsprechende Bezugslänge ausgeben
      //
      text = "Auswertung der Drahtbrüche in Anlehnung an DIN 15020 (Bewertungshilfe)";
      tw.WriteLine();
      tw.WriteLine();
      tw.WriteLine(text);

      text = "Bezugslänge für Ablagekriterien";
      tw.WriteLine(text);
  
    }

    #region Strukturen für das einfachere Datenhandling
    
    struct FListe
    {
      public CSeilFehlerstelle fehlerstelle;
      public double fehler;
      public bool PosDbHaeuf01;
      public bool PosDbHaeuf;
      public bool fehlerGenannt;
      public string FehlerText;
      public int anzahl;
    };

    struct BezugelängenFehler
    {
      public bool fehlerGültig;
      public string fehlerPosition;
      public int anzahlFehlerAufBezugslänge;
    };

    struct BezugsLängenListe
    {
      public List<BezugelängenFehler> bzlFehler;
      public List<string> fehlerpositionen;
      public int maxAnzahlFehlerAufBezugslänge;
    };

    #endregion // Strukturen für das einfachere Datenhandling




    
    /// <summary>
    /// Bewertung der aufgetretenen Darhtbrüche auf eine bestimmte Bezugslänge. Ursprünglich stammt diese Methode
    /// aus dem Code von Udo Denzer (Pascal-Programm). Da es jedoch Ungereimtheiten in der Pascal Funktion kam,
    /// habe ich diese Methode neu geschrieben und getestet.zur besseren Handhabung, wird der vorhandene Container
    /// in eine Struktur um kopiert.
    /// Die Bewertung auf eine bestimmtes Bezugslänge ist nicht trivial!Fehler, die infolge auf eine bestimmte
    /// Bezugslänge auftreten müssen genannt werden.
    /// 
    /// </summary>
    /// <param name="seilDurchmesser"></param>
    /// <param name="bezuglängenFaktor"></param>
    /// <returns></returns>
    private BezugsLängenListe bewertungBezuglänge_Zil(double seilDurchmesser, int bezuglängenFaktor)
    {
      List<string> fehlerAufBezugslänge = new List<string>();
      List<BezugelängenFehler> fehlerStellenAufBezugslänge = new List<BezugelängenFehler>();
     
      

      //  Seilfehler in neue Struktur umkopieren
      //
      FListe[] FListe01 = new FListe[fehlerliste.gibAnzahlAllerFehler];
      //
      int anzahlAusgewählterFehler = 0;
      foreach (CSeilFehlerstelle s in fehlerliste)
      {
        if (s.FehlerAusgewählt == true) // nur ausgewählte Fehler übernehmen
        {
          FListe01[anzahlAusgewählterFehler].fehlerstelle = s;
          FListe01[anzahlAusgewählterFehler].fehler = Convert.ToDouble(s.FehlerpositionHeben);
          FListe01[anzahlAusgewählterFehler].PosDbHaeuf01 = false;
          FListe01[anzahlAusgewählterFehler].fehlerGenannt = false;
          FListe01[anzahlAusgewählterFehler].anzahl = 0;
          FListe01[anzahlAusgewählterFehler].FehlerText = "";
          anzahlAusgewählterFehler++;
        }
      }


      int anzahlFehler = anzahlAusgewählterFehler; // fehlerliste.gibAnzahlAllerFehler; --- 04.08.2011 10:53:47 --- 
      // 
      // 
      double bezugslänge = seilDurchmesser * bezuglängenFaktor / 1000;

      // fehlerAufBezugslänge.Add("

      int anzahlInBezugslänge = 0;
      int maxAnzahlInBezugslänge = 0;
      //int indexLetzterFehlerInBezugslänge = 0;
      bool fehlerHäufung = false;
      
      double fehlerPos1 = 0;
      double fehlerPos2 = 0;
      double fehlerDiff = 0;

      for (int i = 0; i < anzahlFehler; i++) //alle Fehler werden in dieser Hauptschleife bearbeitet
      {
        // zu überprüfende Fehlerposition sichern
        //
        fehlerPos1 = FListe01[i].fehler; // Positionen zwischenspeichern (Debug)
        //
        //  einen neue bezugslängen Fehler anlegen und den Status dieses Fehler auf
        //  ungültig setzen
        //
        BezugelängenFehler bzl = new BezugelängenFehler();
        bzl.fehlerGültig = false; 
        //
        // alle Fehler die nach dem Fehler der ersten Schleife liegen werden hier bearbeitet 03.08.2011  09:16:49
        // 
        for (int j = i + 1; j < anzahlFehler; j++)
        {
          //
          //  wurde dieser Fehler (j) bereits beim vorherigen Lauf genannt, soll auf den nächsten Fehler 
          //  positioniert werden. Dieser Effekt tritt dann auf, wenn bereits ein oder mehrere Fehler
          //  erkannt wurden und einer der beiden Fehler (i) und der erst nicht genannte Fehler (j)
          //  die Bezugslänge unterschreitet
          // 
          if (FListe01[j].PosDbHaeuf == true)
            continue;
          //
          // zur besseren Lesbarkeit den Abstand der beiden Fehler berechnen
          // 
          fehlerPos2 = FListe01[j].fehler;
          fehlerDiff = fehlerPos2 - fehlerPos1;
          //
          // liegt der Fehler in der Bezugslänge, so muss dieser Fehler genannt werden
          // 
          if ((Math.Abs(fehlerDiff) <= bezugslänge))
          {
            //
            //  merken, ob der Fehler zu einer mindesten 2 Fehler zählenden Häufung gehört
            //
            FListe01[j].PosDbHaeuf = true;
            //
            //  merken ob grundsätzlich eine Fehlerhäufung vorliegt
            //
            fehlerHäufung = true;
            //
            //  Anzahl der Fehler (ohne den Fehler(i)) hochzählen
            //
            anzahlInBezugslänge++;
            //
            //  bezugslängenfehler mit der aktuellen Anzahl (+1 fü den Fehler aus Schleife (i) updaten
            //
            bzl.anzahlFehlerAufBezugslänge = anzahlInBezugslänge +1;
            //
            //  wurde dieser Fehler (J) noch nicht genannt, diesen Fehler merken 
            //
            if (FListe01[i].fehlerGenannt == false)
            {
              FListe01[i].fehlerGenannt = true;

              fehlerAufBezugslänge.Add(fehlerPos1.ToString("F2"));
              
              // Beginn der Fehler ist an dieset Stelle schon bekannt
              //  die Anzahl der nachfolgenden in die Bezugelänge fallenden
              //  jedoch noch nicht !
              bzl.fehlerPosition = fehlerPos1.ToString("F2");
              bzl.fehlerGültig = true;
            }
            //
            //  die maximale Anzahl der Fehler in diesem Seil merken
            //
            if (maxAnzahlInBezugslänge < anzahlInBezugslänge)
              maxAnzahlInBezugslänge = anzahlInBezugslänge;
          }
          else
          {
            //  liegt die aktuelle Differenz außerhalb der Bezugslängeund wurde eine Fehlerhäufung
            //  also ein oder mehrere Fehler erkannt, diese Fehler ausgeben.
            //  
            if (fehlerHäufung == true)
            {
              //  wurde bereits eine Fehlerhäufung festgestellt und lag der nächste Fehler
              //  ausserhalb der Bezugslänge, Fehlerstelle speichern
              //
              fehlerStellenAufBezugslänge.Add(bzl);
              //
              //  die Anzahl der Fehler in der Bezugslänge ist für den nächsten Lauf wieder 0

              anzahlInBezugslänge = 0;
              //
              //  Fehler wurde genannt, damit ist die 'fehlerHäufung' nicht nehr gegeben
              //
              fehlerHäufung = false;
              break; // nächsten Seilfehler als Startwert 
            }
          }
        }
        // --- 10.08.2011 10:27:18 --- 
        // 
        //  liegt nach Abschluss inneren Schleife eine Fehlerhäufung vor und
        //  ist der Fehler gültig diesen ausgeben (sonst wird der letzte Fehler
        //  nicht ganannt
        //
        if (fehlerHäufung == true && bzl.fehlerGültig == true)
           fehlerStellenAufBezugslänge.Add(bzl);
      }
      //
      //  zur besseren Handhabung und zum binden der Informationen aus dem Auswertungsprozess
      //  die Informationen in der Struktur Bezugslängeliste zusammenfassen und an die aufrufende
      //  Methode zurückgeben
      //
      BezugsLängenListe fehlerBezug = new BezugsLängenListe();
      //
      // 
      fehlerBezug.bzlFehler = fehlerStellenAufBezugslänge;
      fehlerBezug.fehlerpositionen = fehlerAufBezugslänge;
      fehlerBezug.maxAnzahlFehlerAufBezugslänge = maxAnzahlInBezugslänge + 1;
      //
      return fehlerBezug;
    }





    /// <summary>
    /// eine Meldung in einem Fenster ausgeben, wenn auf der Registerkarte SAA die Angabe
    /// des Seildurchmessers fehlt. 
    /// </summary>
    private void ungültigerSeildurchmesser()
    {
      // Mesagebox wz_msgBox mit Titel und Text parametriert
      //
      MessageBoxButtons button = MessageBoxButtons.OK;
      MessageBoxIcon icon = MessageBoxIcon.Asterisk;

      string überschrift = "Achtung: Ungültiger Seildurchmesser!";
      string meldung = "Bitte geben Sie zuerst auf der Registerkarte TASA einen gültigen Seildurchmesser an!";

      MessageBox.Show(meldung, überschrift, button, icon);


    }





    /// <summary>
    /// Ausgabe der aktuell festgestellten Fehler in einer bestimmten Bezugslänge
    /// </summary>
    /// <param name="tw">TextWriter</param>
    /// <param name="seildurchmesser">Nenndurchmesser des Seiles</param>
    /// <param name="fehler6xD">Fehlerstellen auf 6 x D</param>
    /// <param name="fehler30xD">Fehlerstellen auf 30 x D</param>
    private void schreibeBezugsLängen(TextWriter tw, double seildurchmesser, BezugsLängenListe fehlerBezugslänge, int bezugsLängenFaktor, AusgabeFormat DATEIFORMAT)
    {
      //  Anzahl der Fehler und Bezugslänge ermitteln
      int anzFehlerAufBezugslänge = fehlerBezugslänge.fehlerpositionen.Count;
      double bezugslänge = seildurchmesser * bezugsLängenFaktor / 1000;



      string text;

      if(DATEIFORMAT == AusgabeFormat.TEXTDATEI)
        text = "Bezugslänge " + bezugsLängenFaktor.ToString() + " x Seilnenndurchmesser: " + bezugslänge.ToString("F3") + " m";
      else
        text = "Bezugslänge " + bezugsLängenFaktor.ToString() + " x Seilnenndurchmesser:\t" + bezugslänge.ToString("F3") + " m";

      tw.WriteLine();
      tw.WriteLine();
      tw.WriteLine(text);

      if (anzFehlerAufBezugslänge == 0)
        text = "\nAnzahl von Drahtbrüchen in der Bezugslänge <= 1";
      else
      {
        text = "Anzahl von Drahtbrüchen in der Bezugslänge mindestens 2\n jedoch höchstens " + fehlerBezugslänge.maxAnzahlFehlerAufBezugslänge.ToString();
        text += " ab Position:";
      }

      tw.WriteLine(text);
      tw.WriteLine();
      //
      //  Fehlerstellen für diese Bezugslänge ausgeben
      //
      int i = 0;


      if (DATEIFORMAT == AusgabeFormat.TEXTDATEI)
      {
        //
        // Hier wird die Textdatei geschrieben
        //
        foreach (BezugelängenFehler fehler in fehlerBezugslänge.bzlFehler)
        {
          i++;
          double fehlerPos = Convert.ToDouble(fehler.fehlerPosition);
          string fehlerPosRechtsbündig = string.Format("{0,7:F2}", fehlerPos);
          string iRechtsbündig = string.Format("{0,2:D}", i);
          text = "Pos. " + iRechtsbündig + "\t" + fehlerPosRechtsbündig + " m \t" + fehler.anzahlFehlerAufBezugslänge.ToString();


#if DEBUG_AusgabeOhneAnzahlFehlerAufBezugslänge

          text = "Pos. " + iRechtsbündig + "\t" + fehlerPosRechtsbündig + " m";
#else
          text = "Pos. " + iRechtsbündig + "\t" + fehlerPosRechtsbündig + " m \t" + fehler.anzahlFehlerAufBezugslänge.ToString();

#endif

          tw.WriteLine(text);

        }
      }
      else
      {
        //
        // Hier wird die Worddatei geschrieben
        //
        foreach (BezugelängenFehler fehler in fehlerBezugslänge.bzlFehler)
        {
          i++;
          double fehlerPos = Convert.ToDouble(fehler.fehlerPosition);
          string fehlerPosRechtsbündig = fehlerPos.ToString("F2");
          string iRechtsbündig =  i.ToString();
          text = "Pos. " + iRechtsbündig + "\t" + fehlerPosRechtsbündig + " m \t" + fehler.anzahlFehlerAufBezugslänge.ToString();


#if DEBUG_AusgabeOhneAnzahlFehlerAufBezugslänge

          text = "Pos. " + iRechtsbündig + "\t" + fehlerPosRechtsbündig + " m";
#else
          text = "Pos. " + iRechtsbündig + "\t" + fehlerPosRechtsbündig + " m \t" + fehler.anzahlFehlerAufBezugslänge.ToString();

#endif

          tw.WriteLine(text);

        }

      }

    }


    /// <summary>
    /// gesamte Drahtbruchliste in Ausgabedatei ausgeben
    /// </summary>
    /// <param name="tw"></param>
    private void schreibeDrahtbruchListe(TextWriter tw)
    {
      tw.Write("Anzahl Drahtbrüche gesamt:");
      tw.WriteLine(fehlerliste.gibAnzahlBestätigterFehler.ToString());

      tw.WriteLine();
      tw.WriteLine();
      tw.WriteLine();
      tw.WriteLine("Fehlerstellenliste");

      IList<CSeilFehlerstelle> listeReadonly = fehlerliste.giblistenDaten();

      const char separator = '\t';
      // const char separator = ';';

      int fNr = 0;
      foreach (CSeilFehlerstelle f in listeReadonly)
      {
        if (f.FehlerAusgewählt == true)
        {
          StringBuilder sb = new StringBuilder();
          sb.Append(string.Format("{0,2:D}", ++fNr));
          //sb.Append("\t###");
          sb.Append(separator);
          sb.Append(string.Format("{0,7:F2}", f.FehlerpositionHeben));
          sb.Append(" m");
          sb.Append(separator);
          sb.Append(string.Format("{0,-55:s}", f.FehlerTyp));
          sb.Append(separator);
          sb.Append(string.Format("{0,7:F2}", f.FehlerpositionSenken));
          sb.Append(" m");
          tw.WriteLine(sb.ToString());
        }
      }

    }   
    
    
    


    /// <summary>
    /// Gesamte Drahtbruchliste modifiziert für den Import in Word in Ausgabedatei ausgeben
    /// </summary>
    /// <param name="tw">Textwriter</param>
    private void schreibeDrahtbruchListeWordImport(TextWriter tw)
    {
      tw.Write("Anzahl Drahtbrüche gesamt:\t");
      tw.WriteLine(fehlerliste.gibAnzahlBestätigterFehler.ToString());

      tw.WriteLine();
      tw.WriteLine();
      tw.WriteLine();
      tw.WriteLine("Fehlerstellenliste");

      IList<CSeilFehlerstelle> listeReadonly = fehlerliste.giblistenDaten();

      const char separator = '\t';
      // const char separator = ';';

      int fNr = 0;
      foreach (CSeilFehlerstelle f in listeReadonly)
      {
        if (f.FehlerAusgewählt == true)
        {
          StringBuilder sb = new StringBuilder();
//          sb.Append(string.Format("{0,2:D}", ++fNr));
          sb.Append((++fNr).ToString());
          //sb.Append("\t###");
          sb.Append(separator);
          sb.Append( f.FehlerpositionHeben);
          sb.Append(" m");
          sb.Append(separator);
          sb.Append(f.FehlerTyp);
          tw.WriteLine(sb.ToString());
        }
      }

    }

    
    
    
    /// <summary>
    /// Kopfdaten für die Fehlerstellen ausgeben und Information über die Position der Fehler zu
    /// einem bestimmten Bezugspunkt ausgeben
    /// </summary>
    /// <param name="tw"></param>
    private void schreibeKopfaten(TextWriter tw)
    {

      tw.WriteLine("Die Angaben der Fehlerstellen (Lage im Seil in Meter) beziehen sich auf die Betriebsposition:");
      tw.WriteLine("Endschalter Hubwerk Hochstellung.");

      tw.WriteLine();
      tw.WriteLine();
      tw.WriteLine();

    }

    
    
    ///// <summary>
    ///// Erzeugung des Dateinamens für die Ausgabe der Fehlerliste
    ///// </summary>
    ///// <returns></returns>
    //private string fehlerAusgabeDateiname()
    //{

    //  string dirName = Path.GetDirectoryName(EDT_Fehlerlistendatei.Text);
    //  string fileName = Path.GetFileName(EDT_Fehlerlistendatei.Text);

    //  fileName = fileName.Substring(0, fileName.IndexOf("_Fehleranzeigen.txt"));

    //  fileName = prgEinstellungen.AktuellerDatenPfad + "\\" + fileName + fehlerlistenName;

    //  return fileName;
    //}



    /// <summary>
    /// Erzeugung des Dateinamens für die Ausgabe der Fehlerliste asl Word-Import
    /// </summary>
    /// <returns></returns>
    private string fehlerAusgabeDateinameWord()
    {

      string dirName = Path.GetDirectoryName(EDT_Fehlerlistendatei.Text);
      string fileName = Path.GetFileName(EDT_Fehlerlistendatei.Text);

      fileName = fileName.Substring(0, fileName.IndexOf("_Fehleranzeigen.txt"));

      fileName = prgEinstellungen.AktuellerDatenPfad + "\\" + fileName + fehlerlistenNameWord;

      return fileName;
    }





    /// <summary>
    /// Erzeugung des Dateinamens für die Ausgabe der Fehlerliste asl Word-Import
    /// </summary>
    /// <returns></returns>
    private string fehlerGrafikDateinameExcel(GRAFIKDATEI DateiKenner)
    {

      string diagrammFileExcel = "";

      string dirName =  Path.GetDirectoryName(EDT_Fehlerlistendatei.Text);
      string fileName = Path.GetFileName(EDT_Fehlerlistendatei.Text);

      switch ((int)DateiKenner)
      {
        case (int)GRAFIKDATEI.DATEINAME6xD:
          diagrammFileExcel = diagrammFileExcel6D;
          break;
        case (int)GRAFIKDATEI.DATEINAME30xD:
          diagrammFileExcel = diagrammFileExcel30D;
          break;
      }

      fileName = fileName.Substring(0, fileName.IndexOf("_Fehleranzeigen.txt"));

      fileName = prgEinstellungen.AktuellerDatenPfad + "\\" + fileName + diagrammFileExcel;

      return fileName;
    }





    /// <summary>
    /// Zur besseren Handhabung können mit dieser Schaltfläche können alle Fehler im Seil
    /// ausgewählt werden (Checkbox ist mit Haken gesetzt)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_AlleAuswählen_Click(object sender, EventArgs e)
    {

      foreach (CSeilFehlerstelle s in fehlerliste)
        s.FehlerAusgewählt = true;

      LVW_Seilfehler.BeginUpdate();

      foreach (ListViewItem lvi in LVW_Seilfehler.Items)
        lvi.Checked = true;

      LVW_Seilfehler.EndUpdate();

    }





    /// <summary>
    /// Zur besseren Handhabung können mit dieser Schaltfläche  alle Fehler im Seil
    /// abgewählt werden (Checkbox ist nicht mit Haken gesetzt)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_AlleAbwählen_Click(object sender, EventArgs e)
    {
      //  alle Objekte der Klasse auf false setzen
      //
      foreach (CSeilFehlerstelle s in fehlerliste)
        s.FehlerAusgewählt = false;

      //  und alle ListView-Einträge auf false setzen, somit braucht die
      //  ListView nicht neu gefüllt zu werden
      //
      LVW_Seilfehler.BeginUpdate();

      foreach (ListViewItem lvi in LVW_Seilfehler.Items)
        lvi.Checked = false;

      LVW_Seilfehler.EndUpdate();

    }

    #endregion // Fehlerstellen 

    #region Registerkarte Einstellungen




    /// <summary>
    /// Übertragen der Daten aus den Programmeinstellungen (XML-Datei)
    /// in die Registerkarte "Einstellungen"
    /// </summary>
    private void prgEinstellungenInMaskeZeigen()
    {
      EDT_SAAExePfad.Text = prgEinstellungen.SaaEXEPfad;
      EDT_SAAExeName.Text = prgEinstellungen.SaaEXEName;

      EDT_TASAExePfad.Text = prgEinstellungen.TasaEXEPfad;
      EDT_TasaExeName.Text = prgEinstellungen.TasaEXEName;
      
      EDT_Startverzeichnis.Text = prgEinstellungen.Datenpfad;

      //EDT_AusgabeListen.Text = prgEinstellungen.AusgabeverzeichnisListen;

    }





    /// <summary>
    /// Übertragen der Daten aus der Registerkarte "Einstellungen" in die Klasse
    /// für die Programmeinstellungen, die dann später, beim Verlassen des Programms
    /// in einer XML-Datei gespeichert werden.
    /// </summary>
    private void prgEinstellungenAusMaskeSichern()
    {
      prgEinstellungen.TasaEXEPfad = EDT_TASAExePfad.Text;
      prgEinstellungen.TasaEXEName = EDT_TasaExeName.Text;
      
      prgEinstellungen.SaaEXEPfad = EDT_SAAExePfad.Text;
      prgEinstellungen.SaaEXEName = EDT_SAAExeName.Text;

      prgEinstellungen.Datenpfad = EDT_Startverzeichnis.Text;

      // --- 19.08.2011 09:39:49 --- 
      // gibt es ab sofort nicht mehr! Wurde als aktuelleDatenpfad umfunktioniert 
      // 
      // prgEinstellungen.AusgabeverzeichnisListen = EDT_AusgabeListen.Text;
      

    }





    /// <summary>
    /// Serialisierung der Daten aus der Klasse CEinstellungen in die angegebene
    /// XML-Datei
    /// </summary>
    private void parameterSpeichern()
    {
      prgEinstellungenAusMaskeSichern();

      try
      {
        //string pfad = System.Windows.Forms.Application.StartupPath;
        string pfad = System.Windows.Forms.Application.UserAppDataPath; // WIndows 7
        string datei = "tasapardef.XML";
        string xmlFilename = Path.Combine(pfad, datei);

        XmlSerializer ser = new XmlSerializer(typeof(CEinstellungen));
        FileStream str = new FileStream(xmlFilename, FileMode.Create);
        ser.Serialize(str, prgEinstellungen);
        str.Close();

      }
      catch (Exception e)
      {
        string titel = "Fehler beim schreiben der Programmeinstellungen!";
        string nachricht = "Fehler: " + e.Message;
        MessageBoxButtons mbxButton = MessageBoxButtons.OK;
        MessageBoxIcon mbxicon = MessageBoxIcon.Warning;
        MessageBox.Show(nachricht, titel, mbxButton, mbxicon);
        throw;
      }

    }




    /// <summary>
    /// Serialisierung der Programmeinstellungen aus der XML-Datei in die Klasse, die diese
    /// verwaltet (CEinstellungen)
    /// </summary>
    /// <returns></returns>
    public CEinstellungen parametereinlesen()
    {
      xmlDateiFehlt = false; //   ich gehen zunächst davon aus, das diese Datei existiert

      //string pfad = System.Windows.Forms.Application.StartupPath;
      string pfad = System.Windows.Forms.Application.UserAppDataPath; // Windows 7
      string datei = "tasapardef.XML";
      string xmlFilename = Path.Combine(pfad, datei);
      try
      {
        XmlSerializer ser = new XmlSerializer(typeof(CEinstellungen));
        StreamReader sr = new StreamReader(xmlFilename);
        prgEinstellungen = (CEinstellungen)ser.Deserialize(sr);
        sr.Close();
        prgEinstellungenInMaskeZeigen();
        return prgEinstellungen;

      }
      catch (Exception)
      {
        xmlDateiFehlt = true; //  löst in Form_Load aus, dass auf die Registerkarte
 

        return null;
      }
    }


    /// <summary>
    /// Auswahl des Speicherortes und dadurch Feststellung des Verzeichnisses in dem
    /// die ausführbare Datei für das Pascal-Programm TASA liegt
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_TASAExePfad_Click(object sender, EventArgs e)
    {
      OpenFileDialog fd = new OpenFileDialog();
      fd.Filter = "Ausführbare Dateien (*.exe)|*.exe|Alle Dateien (*.*)|*.*";
      fd.Title = "Ordner in der sich TASAxxx.EXE befindet";
      fd.InitialDirectory = prgEinstellungen.TasaEXEPfad;

      if (fd.ShowDialog() == DialogResult.OK)
      {
        string filename = fd.FileName;
        string pfad = Path.GetDirectoryName(filename);
        prgEinstellungen.TasaEXEPfad = pfad;
        EDT_TASAExePfad.Text = prgEinstellungen.TasaEXEPfad;

        string exeName = Path.GetFileName(filename);
        prgEinstellungen.TasaEXEName = exeName;
        EDT_TasaExeName.Text = prgEinstellungen.TasaEXEName;

      }

    }




    /// <summary>
    /// Auswahl des Datenverzeichnisses, in dem sich die Datendateien zur Auswertung der Seilprüfungen
    /// befinden
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_Startverzeichnis_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog fd = new FolderBrowserDialog();
      fd.Description = "Auswahl des Verzeichnisses, in dem sich die Datendateien\nder Seilprüfung befinden";
      fd.SelectedPath = prgEinstellungen.Datenpfad;

      if (fd.ShowDialog() == DialogResult.OK)
      {
        string verzeichnis = fd.SelectedPath;
        EDT_Startverzeichnis.Text = verzeichnis;
        prgEinstellungen.Datenpfad = verzeichnis;

      }
    }




    /// <summary>
    /// beim Verlassen der Registerkarte "Einstellungen" werden die Daten aus dieser Registerkarte
    /// in der Klasse für die Programmeinstellungen gesichert
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void REG_Einstellungen_Leave(object sender, EventArgs e)
    {
      prgEinstellungenAusMaskeSichern();
      parameterSpeichern();
    }





    /// <summary>
    /// Auswahl der Speicherortes für die Ausgabelisten
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_AusgabeListenVerz_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog fd = new FolderBrowserDialog();
      fd.Description = "Auswahl des Verzeichnisses, in dem die Abgleichs- und Bestätigungslisten abgelegt werden sollen";
      fd.SelectedPath = prgEinstellungen.Datenpfad;

      if (fd.ShowDialog() == DialogResult.OK)
      {
        string verzeichnis = fd.SelectedPath;
        EDT_SpeicherortEinstellungen.Text = verzeichnis;
        prgEinstellungen.AktuellerDatenPfad = verzeichnis;

      }
    }

    #endregion // Programmeinstellungen verwalten

    private void RBTN_KurvenDis_ein_CheckedChanged(object sender, EventArgs e)
    {

    }

 
   

 
    #endregion // Registerkarten 

    private void cSeilFehlerstellenListeBindingSource_CurrentChanged(object sender, EventArgs e)
    {

    }

    private void BTN_WordImportdatei_Click(object sender, EventArgs e)
    {
      //Mesagebox wz_msgBox mit Titel und Text parametriert
      //
      MessageBoxButtons button = MessageBoxButtons.OK;
      MessageBoxIcon icon = MessageBoxIcon.Asterisk;
      string programmName = System.Windows.Forms.Application.ProductName;
      string überschrift = programmName + ": Sonderauswertung";
      string meldung = "Achtung: Es wird nur die Word-Importdatei ausgegeben!\nEs werden keine Excelgrafikdateien ausgegeben!";

      MessageBox.Show(meldung, überschrift, button, icon);

      auswerteModus = AuswerteModus.WORDIMPORTDATEI;
      BTN_DbListeAusgeben.PerformClick();
      


    }

  } // Klassenende Hauptform

} // Namespace-Ende TasaParDef
