using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TasaParDef
{
  /// <summary>
  /// Dialog zum manuellen einfügen einer Fehlerstelle
  /// </summary>
  public partial class DlgFehlerstelleInsert : Form
  {
    /// <summary>
    /// Die Position des Fehlers 
    /// </summary>
    private string fehlerPos;

    /// <summary>
    /// Den Fehlertyp (abgelegt in CBX_Drahtbruchtyp im Designer)
    /// </summary>
    private string fehlerTyp;

    /// <summary>
    /// Alle Angaben für den Fehler sind korrekt
    /// </summary>
    bool fehlerIstkorrekt;

    /// <summary>
    /// Den Dialog initialiseren
    /// </summary>
    public DlgFehlerstelleInsert()
    {
      InitializeComponent();
      initialisierung();

    }

    /// <summary>
    /// Die Steuerelemente initialisieren
    /// </summary>
    private void initialisierung()
    {
      errorProvider1.Clear();
      RBTN_AussenTyp1.Checked = true;

      this.StartPosition = FormStartPosition.CenterParent;
    }


    /// <summary>
    /// Beenden des Dialoges und zwischenspeichern des Objekts
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_Ok_Click(object sender, EventArgs e)
    {
      neuenFehlerEinfügen();

    }

    private void neuenFehlerEinfügen()
    {
      try
      {
        fehlerPos = EDT_SeilfehlerPos1.Text.Replace('.', ',');
        double zahlFehlerPos = Convert.ToDouble(fehlerPos);
        fehlerPos = zahlFehlerPos.ToString("F2");
        fehlerTyp = auswertungRadioButton();
        fehlerIstkorrekt = true;
        this.DialogResult = DialogResult.OK;
      }
      catch (Exception)
      {
        System.Media.SystemSounds.Asterisk.Play();
        errorProvider1.SetError(EDT_SeilfehlerPos1, "Die eingegebene Position ist ungültig!");
        fehlerIstkorrekt = false;
      }
    }

    private string auswertungRadioButton()
    {
      if (RBTN_AussenTyp1.Checked == true)
        return "Aussen DB Typ 1";
      else if (RBTN_AussenTyp2.Checked == true)
        return "Aussen DB Typ 2 (W-Form)";
      else if (RBTN_AussenTyp3.Checked == true)
        return "Drahtbruch aussen 3 (ausgebrochene Draehte, Zahnform)";
      else if (RBTN_Innen.Checked == true)
        return "Drahtbruch innen (verbreiterte W-Form)";
      else if (RBTN_KerbeNarbe.Checked == true)
        return "Kerbe / Narbe";

      return "undefiniert!";
    }


    /// <summary>
    /// Eine im Dialog eigegebene Fehlerstelle als Objekt zurückgeben
    /// </summary>
    /// <returns></returns>
    public CSeilFehlerstelle gibFehlerStelle()
    {
      CSeilFehlerstelle  fehler;
      if (fehlerIstkorrekt == true)
      {
        fehler = new CSeilFehlerstelle("0", fehlerPos, fehlerPos, fehlerTyp, "manuell");
        return fehler;
      }
      return null;
    }

    private void EDT_SeilfehlerPos1_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        neuenFehlerEinfügen();
        this.DialogResult = DialogResult.OK;
      }
    }
  }
}
