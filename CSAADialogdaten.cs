using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TasaParDef
{
  public class CSAADialogdaten
  {
    private string dbListeHeben;
    private string dbListesenken;
    private string abgleichslistenDateinamen;
    private string bestätigungslistenDateinamen;

    private double seildurchmesser;
    private double akzLagedifferenz;
    private double startRegBeiSeillänge;
    private double regKosntante;
    private double regOrdnung_1;
    private double regOrdnung_2;
    private double regOrdnung_3;
    private double regOrdnung_4;
    private double regOrdnung_5;
    private double regOrdnung_6;
    private double regOrdnung_7;
    private double regOrdnung_8;

    private  CDateinamen dateinamen;

    private readonly CEinstellungen prgEintellungen;

    public CSAADialogdaten( CEinstellungen  prg, CDateinamen  dateinamen)
    {
      this.dateinamen = dateinamen;
      this.DrahtbruchlisteHeben = "";
      this.DrahtbruchlisteSenken = "";
      this.Seildurchmesser = 0.0;
      this.Lagedifferenz = 0.10;
      this.StartRegression = 0.0;
      this.RegressionsKontante = 1.0;
      this.RegressionOrdnung1 = 0.0;
      this.RegressionOrdnung2 = 0.0;
      this.RegressionOrdnung3 = 0.0;
      this.RegressionOrdnung4 = 0.0;
      this.RegressionOrdnung5 = 0.0;
      this.RegressionOrdnung6 = 0.0;
      this.RegressionOrdnung7 = 0.0;
      this.RegressionOrdnung8 = 0.0;

      this.prgEintellungen = prg;

    }





    public string DrahtbruchlisteHeben
    {
      set
      {
        dbListeHeben = value;
        if (dbListeHeben.Length > 7)
          erzeugeListenDateinamen();
      }
      get { return dbListeHeben; }
    }






    private void erzeugeListenDateinamen()
    {
      string dateiname = Path.GetFileName(dbListeHeben);

      //  Die Ermittlung des Projektnamen erfolgt ab sofort ausschließlich über die Kennung
      //  der Seilposition und der Bewegungsrichtung. Die bisherige Ermittlung des Projektnamen
      //  in dem nach dem Vorkommen ".DDF" sucht wurde wird verlassen, da eine durchgängige Verarbeitung,
      //  Dateinamen auch ohne Kennung von ".DDF", gewährleistet werden muss.
      //
     
      string[] prüfvorgänge = { "_LH", "_LS", "_RH", "_RS", "_LAH", "_LAS", "_RAH", "_RAS" };

      int längeVorgang = 0;
      int vorhangPos = -1;
      foreach (string vorgang in prüfvorgänge)
      {
        längeVorgang = vorgang.Length;
        vorhangPos = dateiname.IndexOf(vorgang);
        if (!(vorhangPos < 0))
          break;
      }
       
      

      //  die Information über 2-Seil oder 4-Seilgräte nkann nur im Dateinamen
      //  gescucht wir ab der Position ".DDF" minus 5 Zeichen, sowird der zweite
      //  Untestrich im Dateinamen ermittelt. Die Gerätenummer bleibt unberücksichtigt
      //        V--4-V                      V-3-V        
      //  0123456789-0123456789-      0123456789-0123456789-
      //  Bg_290_LAH1.DDF_...         Bg_260_LS1.DDF_...
      //
//      int posUnterstrich = dateiname.IndexOf('_', posPunkt );
////      int posUnterstrich = dateiname.IndexOf('_', posPunkt - 5);
//      int anzZeichenSeilbezeichnung = posPunkt - posUnterstrich - 1;
      int posExtract = 0;

      if (längeVorgang == 4)          // -123456789-123456789-123456789
        posExtract = vorhangPos + 3;  // Bg_290_LAH1.DDF wird zu Bg_258_LA
      else
        posExtract = vorhangPos + 2;  // Bg_258_LS1.DDF wird zu Bg_258_L

      string teildateinamen = dateiname.Substring(0, posExtract);

      // abgleichslistenDateinamen = prgEintellungen.AusgabeverzeichnisListen + "\\" + teildateinamen + "Seil_Fehleranzeigen.txt";
      // bestätigungslistenDateinamen = prgEintellungen.Datenpfad + "\\" + teildateinamen + "Seil_Fehler.txt";
      //
      // --- 19.08.2011 08:08:25 --- 
      //  Änderung in der Klasse CEinstellunge (Programmeinstellungen) 
      //  ab sofort gibt es keine Variable mehr für das Listenverzeichnis
      //  Das Listenverzeichnis wurde zum  aktuellen Datepfad 
      // 
      

      abgleichslistenDateinamen = dateinamen.FehlerAnzeigenlistenDateiname;
      bestätigungslistenDateinamen = dateinamen.BestätigungslistennDateiname;
//      abgleichslistenDateinamen = prgEintellungen.AktuellerDatenPfad + "\\" + teildateinamen + "_Seil_Fehleranzeigen.txt";
//      bestätigungslistenDateinamen = prgEintellungen.AktuellerDatenPfad + "\\" + teildateinamen + "_Seil_Fehler.txt";

    }
 
    public string DrahtbruchlisteSenken
    {
      set { dbListesenken = value; }
      get { return dbListesenken; }
    }

    public double Seildurchmesser
    {
      set { seildurchmesser = value; }
      get { return seildurchmesser; }
    }

    public string AbgleichslistenDateiname
    {
      get { return abgleichslistenDateinamen; }
    }

    public string BestätigunglistenDateiname
    {
    
      get { return bestätigungslistenDateinamen; }
    }

    public double Lagedifferenz
    {
      set { akzLagedifferenz = value; }
      get { return akzLagedifferenz; }
    }

    public double StartRegression
    {
      set { startRegBeiSeillänge = value; }
      get { return startRegBeiSeillänge; }
    }

    public double RegressionsKontante
    {
      set { regKosntante = value; }
      get { return regKosntante; }
    }

    public double RegressionOrdnung1
    {
      set { regOrdnung_1 = value; }
      get { return regOrdnung_1; }
    }

    public double RegressionOrdnung2
    {
      set { regOrdnung_2 = value; }
      get { return regOrdnung_2; }
    }

    public double RegressionOrdnung3
    {
      set { regOrdnung_3 = value; }
      get { return regOrdnung_3; }
    }

    public double RegressionOrdnung4
    {
      set { regOrdnung_4 = value; }
      get { return regOrdnung_4; }
    }

    public double RegressionOrdnung5
    {
      set { regOrdnung_5 = value; }
      get { return regOrdnung_5; }
    }

    public double RegressionOrdnung6
    {
      set { regOrdnung_6 = value; }
      get { return regOrdnung_6; }
    }

    public double RegressionOrdnung7
    {
      set { regOrdnung_7 = value; }
      get { return regOrdnung_7; }
    }

    public double RegressionOrdnung8
    {
      set { regOrdnung_8 = value; }
      get { return regOrdnung_8; }
    }
 
    
  }
}
