using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace TasaParDef
{

  /// <summary>
  /// Veraltung und Bereitsstellung der Fehlerstellenliste   
  /// </summary>
  public class CSeilFehlerstellenListe : IEnumerable<CSeilFehlerstelle>
  {
    /// <summary>
    /// string zur Aufnahme alles Fehler als Textstring die aus SAA kommen
    /// </summary>
    private string alleFehlerDaten;

    /// <summary>
    /// des Collection für die aus SAA gewonnenen Fehler 
    /// </summary>
    private List<CSeilFehlerstelle> fehlerListe;


    /// <summary>
    /// Statistik der Seilfehlerstellen
    /// </summary>
    private int summeAllerFehler;
    private int summeBestätigterFehler;
    private int summeAutoFehler;
    private int summeManFehler;






    /// <summary>
    /// Konstruktion mit Übergabe der SAA-Datei
    /// </summary>
    /// <param name="dateiname"></param>
    public CSeilFehlerstellenListe(string dateiname)
    {
      initialisierung();
      einlesenFehler(dateiname);


    }

    private void initialisierung()
    {
      fehlerListe = new List<CSeilFehlerstelle>(100);

      summeAllerFehler = 0;
      summeBestätigterFehler = 0;
      summeAutoFehler = 0;
      summeManFehler = 0;
    }


    public int Clear()
    {
      int anzahl = fehlerListe.Count;
      fehlerListe.Clear();

      return anzahl;
    }

    /// <summary>
    /// nur lesenden Zugriff von aussen erlauben
    /// </summary>
    /// <returns></returns>
    public IList<CSeilFehlerstelle> giblistenDaten()
    {
      // fehlerListe.Sort(sortfehlerPosition);
      IList<CSeilFehlerstelle> copy = fehlerListe.AsReadOnly();

      return copy;
    }


    /// <summary>
    /// Vergleich zweier Fehlerstellen zur Sortierung in der Liste
    /// </summary>
    /// <param name="a">erster Fehler</param>
    /// <param name="b">zweiter Fehler</param>
    /// <returns>Vergleichswert</returns>
    int sortfehlerPosition(CSeilFehlerstelle a, CSeilFehlerstelle b)
    {
      double aPos = Convert.ToDouble(a.FehlerpositionHeben);
      double bPos = Convert.ToDouble(b.FehlerpositionHeben);


      if (aPos > bPos)
        return 1;
      if (aPos < bPos)
        return -1;

      return 0;


    }



    /// <summary>
    /// Die als Textstring eingelesenen Fehler aus SAA (alles Fehller in einem String)
    /// splitten in einzelne Fehler und ablegen diesen in fehlerListe
    /// </summary>
    private void splittenFehler()
    {
//      int neueFehlernummer = 1;

      string[] fehlerDaten = alleFehlerDaten.Split('\n');

      string hebenPosAlt = "";

      foreach (string s in fehlerDaten)
      {
        if (s.Length == 0) continue; // leere Zeile ignorieren
    
      
        
        string temp = s.TrimEnd('\r'); // Return entfernen
        const string dbFehlerHerkunft = "automatisch";
        temp.Substring(0, temp.Length - 1);
        string[] fehlerArray = temp.Split('\t');

        for (int i = 0; i < fehlerArray.Length; i++)
          fehlerArray[i] = fehlerArray[i].Trim();

        // -- doppelte Zeilen ignorieren
        //
        if (fehlerArray[2] == hebenPosAlt)
          continue;
        else
          hebenPosAlt = fehlerArray[2];

        CSeilFehlerstelle fs = new CSeilFehlerstelle(fehlerArray[0], fehlerArray[2], fehlerArray[4], fehlerArray[3], dbFehlerHerkunft);
        fehlerListe.Add(fs);
        summeAllerFehler++;
        summeAutoFehler++;

      }



    }

    public bool löscheAnPosition ( CSeilFehlerstelle sfs)
    {
      bool result = fehlerListe.Remove(sfs);

      return result;
    }


    /// <summary>
    /// Einlesen aller fehler in einen Textstring
    /// </summary>
    /// <param name="dateiname"></param>
    private void einlesenFehler(string dateiname)
    {
      alleFehlerDaten = File.ReadAllText(dateiname);
      splittenFehler();
      fehlerListe.Sort(sortfehlerPosition); // --- 04.08.2011 11:02:36 --- 
      
      // --- 10.08.2011 08:24:38 --- 
      // 
      //  Die aus dern Ursprungsdaten kommende Fehlernummer muss verworfen werden,
      //  da Fehlerstellen aus Udo's Programm doppelt vorkommen können und daher
      //  die Ursprünglichen Fehlernummern lücken aufweisen können, hier neu 
      //  durchnummeriern um die Lücken (in der LietView zu schliessen.
      //
      neueNummerierung();
      // 

    }



    /// <summary>
    /// Einen Fehler einfügen
    /// </summary>
    /// <param name="fs">Fehlerstelle</param>
    public void einfügenFehler(CSeilFehlerstelle fs)
    {
      fehlerListe.Add(fs);
      summeAllerFehler++;
      fehlerListe.Sort(sortfehlerPosition);
      neueNummerierung();

    }


    /// <summary>
    /// Alle Fehler neu durchmummerieren
    /// </summary>
    private void neueNummerierung()
    {
      int i = 0;
      foreach (CSeilFehlerstelle f in fehlerListe)
      {
        // i++;
        f.Fehlernummer = (++i).ToString();
      }
    }



    /// <summary>
    /// Checkbox für eine fehler aktivieren
    /// </summary>
    /// <param name="posHeben"></param>
    /// <param name="posSenken"></param>
    public void setzeFehlerAusgewählt(string posHeben, string posSenken)
    {
      string indetNeu = posHeben + posSenken;

      foreach (CSeilFehlerstelle f in fehlerListe)
      {
        if (f.Indetität == indetNeu)
        {
          f.FehlerAusgewählt = true;
          summeBestätigterFehler++;
          break;
        }
      }
    }

    /// <summary>
    /// Checkbox für eine Fehler deaktivieren
    /// </summary>
    /// <param name="posHeben"></param>
    /// <param name="posSenken"></param>
    public void löscheFehlerAusgewählt(string posHeben, string posSenken)
    {
      string indetNeu = posHeben + posSenken;

      foreach (CSeilFehlerstelle f in fehlerListe)
      {
        if (f.Indetität == indetNeu)
        {
          f.FehlerAusgewählt = false;
          summeBestätigterFehler--;
          break;
        }
      }
    }

    public int gibAnzahlAllerFehler
    {
      get { return fehlerListe.Count; }
    }

    //  zum lesenden Zugriff von außen auf die Fehlerliste
    //  somit wird die Rückgabe des Readonly
    public IEnumerator<CSeilFehlerstelle> GetEnumerator()
    {
      return this.fehlerListe.GetEnumerator();
    }


    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public string Fehlerposition
    {
      get { return this.Fehlerposition; }
    }


    public int gibAnzahlBestätigterFehler
    {
      get { return gibAnzahlCheckedFehler; }
    }

    public int gibAnzahlManuellerFehler
    {
      get { return summeManFehler; }
    }

    public int gibAnzahlAutoFehler
    {
      get { return summeAutoFehler; }
    }

    private int gibAnzahlCheckedFehler
    {
      get
      {
        int fehlerChecked = 0;
        foreach (CSeilFehlerstelle s in fehlerListe)
        {
          if (s.FehlerAusgewählt == true)
            fehlerChecked++;
        }
        return fehlerChecked;
      }
    }
  }
}

