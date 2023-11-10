using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;


namespace TasaParDef
{
  static class Program
  {
    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Hautform());
        try
      {
        //Application.Run(new Hautform());
      }
      catch (Exception ex1)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.OK;
        MessageBoxIcon icon = MessageBoxIcon.Warning;

        string überschrift = "Achtung: Programm wird beendet!";
        string meldung = "Durch Fehler bei der Ausführung musste das Programm beendet werden!\n";
        meldung += "Bitte teilen Sie dem Programmierer die nachfolgend aufgeführte Fehlermeldung mit!";

        MessageBox.Show(meldung, überschrift, button, icon);


        //StackTrace st = new StackTrace();
        //StackFrame sf = st.GetFrame(0);
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        button = MessageBoxButtons.OK;
        icon = MessageBoxIcon.Stop;

        überschrift = "Achtung: Programm wird beendet!";
        meldung = "Fehlemeldung:\n";
        meldung += ex1.StackTrace;
        MessageBox.Show(meldung, überschrift, button, icon);
      }
 
    }
  }
}
