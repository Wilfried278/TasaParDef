using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TasaParDef
{
  public class CSeilFehlerstelle
  {

    private int fehlerNrInt;
    private string fehlerNr;
    private string fehlerPositionHeben;
    private string fehlerPositionSenken;
    private string fehlerTyp;
    private string fehlerUrsprung;
    private bool istAusgewählt;
    private string ident; 

    public CSeilFehlerstelle(string fehlernummer, string fehlerPositionHeben,string fehlerPositionSenken,string fehlerTyp, string fehlerHerkunft)
    {
      this.fehlerNrInt = Convert.ToInt32(fehlernummer);
      this.fehlerNr = fehlernummer;
      this.fehlerPositionHeben = fehlerPositionHeben;
      this.fehlerPositionSenken = fehlerPositionSenken;
      this.ident = FehlerpositionHeben + FehlerpositionSenken; // zur eindeutigen Identifikation in der Listview 
      this.fehlerTyp = fehlerTyp;
      this.fehlerUrsprung = fehlerHerkunft; 
    }


    public string Fehlernummer
    {
      get { return this.fehlerNr; }
      set { this.fehlerNr = value; }
    }
    public string FehlerpositionHeben
    {
      get { return this.fehlerPositionHeben; }
      //set { this.fehlerPositionHeben = value; }
    }
    public string FehlerpositionSenken
    {
      get { return this.fehlerPositionSenken; }
      //set { this.fehlerPositionSenken = value; }
    }
    public string FehlerTyp
    {
      get { return this.fehlerTyp; }
      set { this.fehlerTyp = value; }
    }
    
    public string FehlerHerkunft
    {
      get { return this.fehlerUrsprung; }
      //set { this.fehlerHerkunft = value; }
    }

    public bool FehlerAusgewählt
    {
      get { return this.istAusgewählt; }
      set { this.istAusgewählt = value; }
    }

    public string Indetität
    {
      get { return this.ident; }
    }

  }
}
