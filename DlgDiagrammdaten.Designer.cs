namespace TasaParDef
{
  partial class DlgDiagrammdaten
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.BTN_DiagrammdatenOK = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
      this.CBX_DiagrammÜberschrift = new System.Windows.Forms.ComboBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // BTN_DiagrammdatenOK
      // 
      this.BTN_DiagrammdatenOK.Location = new System.Drawing.Point(408, 14);
      this.BTN_DiagrammdatenOK.Name = "BTN_DiagrammdatenOK";
      this.BTN_DiagrammdatenOK.Size = new System.Drawing.Size(75, 23);
      this.BTN_DiagrammdatenOK.TabIndex = 2;
      this.BTN_DiagrammdatenOK.Text = "Ok";
      this.BTN_DiagrammdatenOK.UseVisualStyleBackColor = true;
      this.BTN_DiagrammdatenOK.Click += new System.EventHandler(this.BTN_DiagrammdatenOK_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.CBX_DiagrammÜberschrift);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.dateTimePicker1);
      this.groupBox1.Location = new System.Drawing.Point(12, 14);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(387, 163);
      this.groupBox1.TabIndex = 4;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Benötigte Daten für die grafische Darstellung der Seilfehler (Excel-Diagramm)";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(27, 89);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(108, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Diagramm-Überschrift";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(27, 37);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(96, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Datum der Prüfung";
      // 
      // dateTimePicker1
      // 
      this.dateTimePicker1.Location = new System.Drawing.Point(30, 53);
      this.dateTimePicker1.Name = "dateTimePicker1";
      this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
      this.dateTimePicker1.TabIndex = 4;
      // 
      // CBX_DiagrammÜberschrift
      // 
      this.CBX_DiagrammÜberschrift.FormattingEnabled = true;
      this.CBX_DiagrammÜberschrift.Items.AddRange(new object[] {
            "Seilprüfung Bg. ",
            "Seilprüfung Abs. ",
            "Seilprüfung Ag. "});
      this.CBX_DiagrammÜberschrift.Location = new System.Drawing.Point(30, 105);
      this.CBX_DiagrammÜberschrift.Name = "CBX_DiagrammÜberschrift";
      this.CBX_DiagrammÜberschrift.Size = new System.Drawing.Size(326, 21);
      this.CBX_DiagrammÜberschrift.TabIndex = 8;
      // 
      // DlgDiagrammdaten
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(495, 195);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.BTN_DiagrammdatenOK);
      this.Name = "DlgDiagrammdaten";
      this.Text = "Daten für das Exceldiagramm";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button BTN_DiagrammdatenOK;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.ComboBox CBX_DiagrammÜberschrift;
  }
}