using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TasaParDef
{
  public class CRevisionsInfo
  {
	// Achtung: auch in den Properties/Assemblyversion.template ist die 
  
	private string hauptversion  = "2";
    private string unterversion  = "0";
    private string svnRevision  = "165";
    private string svnDatum     = "2016/05/23 12:06:34";
    private string svnJetzt     = "2016/05/23 12:06:38";
    private string svnURL       = "file:///Q:/TASAParDef/TASAPardef_2.0/TasaParDef";

    public CRevisionsInfo()
    {

    }

    #region Eigenschaften

    public string Hautpversion
    {
      get { return hauptversion; }
    }


    public string Unterversion
    {
      get { return unterversion; }
    }


    public string SVNRevision
    {
      get { return svnRevision; }
    }


    public string SVNDatum
    {
      get { return svnDatum; }
    }


    public string SVNJetzt
    {
      get { return svnJetzt; }
    }


    public string SVNUrl
    {
      get { return svnURL; }
    }


    public string Versionsnummer
    {
      get 
      {
        string vn = Hautpversion + "." + Unterversion + "." + SVNRevision;
        return vn;
      }
    }
   
    
    
    #endregion // Eigenschaften 
  
  }

  
}
