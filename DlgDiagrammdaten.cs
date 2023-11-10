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
  public partial class DlgDiagrammdaten : Form
  {
    private DateTime prüfDatum;
    private string diagrammÜberschrift;

    public DlgDiagrammdaten()
    {
      InitializeComponent();
      this.StartPosition = FormStartPosition.CenterParent;
      CBX_DiagrammÜberschrift.SelectedIndex = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BTN_DiagrammdatenOK_Click(object sender, EventArgs e)
    {
      if (datumistOk() == false)
        return;

      Prüfdatum = dateTimePicker1.Value;
      Diagrammüberschrift = CBX_DiagrammÜberschrift.Text;


      this.Close();
    }

    private bool datumistOk()
    {

      if (dateTimePicker1.Value.Date > DateTime.Now.Date)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.YesNo;
        MessageBoxIcon icon = MessageBoxIcon.Question;

        string überschrift = "Achtung: Die Seilprüfung findet in der Zukunft statt ?";
        string meldung = "Die Seilprüfungen, die in der Zukunft stattfinden, können erst mit der nächste Version ausgewertet werden!";

        MessageBox.Show(meldung, überschrift, button, icon);

        return false;
      }

      //  wurde vielleicht vergessen das Prüfdatum eizugeben (dann ist das 
      //  Tagesdatum  gleich)
      //
      if (dateTimePicker1.Value.Date == DateTime.Now.Date)
      {
        // Mesagebox wz_msgBox mit Titel und Text parametriert
        //
        MessageBoxButtons button = MessageBoxButtons.YesNo;
        MessageBoxIcon icon = MessageBoxIcon.Question;

        string überschrift = "Achtung: Seilprüfung und Auswertung heute?";
        string meldung = "Wurde die Seilprüfung und die Auswertung wirklich heute durchgeführt?";

        DialogResult result = MessageBox.Show(meldung, überschrift, button, icon);

        if (result == DialogResult.No)
          return false;

      }

      return true;

    }


    #region Eigenschaften

    public DateTime Prüfdatum
    {
      get { return prüfDatum; }
      set { prüfDatum = value; }
    }

    public string Diagrammüberschrift
    {
      get { return diagrammÜberschrift; }
      set { diagrammÜberschrift = value; }
    }


    #endregion // Eigenschaften 
  }
}
