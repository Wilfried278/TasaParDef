using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TasaParDef
{
  public class CEinstellungen
  {
    private string saaEXEPfad;
    private string saaEXEName;
    private string tasaEXEPfad;
    private string tasaEXEName;
    private string datenPfad;
    private string aktuellerDatenPfad;

    public CEinstellungen()
    {
      SaaEXEPfad = "C:\\";
      TasaEXEPfad = "C:\\";
      SaaEXEName = "";
      TasaEXEName = "";

      Datenpfad = "C:\\";
      aktuellerDatenPfad = "C:\\";
    }

    public CEinstellungen(string saaEXEPfad, string tasaEXEPfad, string startverzeichnisDaten)
    {
      this.SaaEXEPfad = saaEXEPfad;
      this.TasaEXEPfad = tasaEXEPfad;
      this.Datenpfad = startverzeichnisDaten;
      AktuellerDatenPfad = Datenpfad;
    }

    public string TasaEXEPfad
    {
      get { return this.tasaEXEPfad; }
      set { this.tasaEXEPfad = value; }
    }
    
    public string TasaEXEName
    {
      get { return this.tasaEXEName; }
      set { this.tasaEXEName = value; }
    }

    public string SaaEXEPfad
    {
      get { return this.saaEXEPfad; }
      set { this.saaEXEPfad = value; }
    }
    
    public string SaaEXEName
    {
      get { return this.saaEXEName; }
      set { this.saaEXEName = value; }
    }

    public string Datenpfad
    {
      get { return this.datenPfad; }
      set { this.datenPfad = value; }
    }

    public string AktuellerDatenPfad
    {
      get { return this.aktuellerDatenPfad; }
      set { this.aktuellerDatenPfad = value; }
    }
    
  }
}
