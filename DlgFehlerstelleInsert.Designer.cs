namespace TasaParDef
{
  partial class DlgFehlerstelleInsert
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
      this.components = new System.ComponentModel.Container();
      this.BTN_Ok = new System.Windows.Forms.Button();
      this.BTN_abbruch = new System.Windows.Forms.Button();
      this.EDT_SeilfehlerPos1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.RBTN_AussenTyp1 = new System.Windows.Forms.RadioButton();
      this.RBTN_AussenTyp2 = new System.Windows.Forms.RadioButton();
      this.RBTN_AussenTyp3 = new System.Windows.Forms.RadioButton();
      this.RBTN_Innen = new System.Windows.Forms.RadioButton();
      this.RBTN_KerbeNarbe = new System.Windows.Forms.RadioButton();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // BTN_Ok
      // 
      this.BTN_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.BTN_Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.BTN_Ok.Location = new System.Drawing.Point(384, 12);
      this.BTN_Ok.Name = "BTN_Ok";
      this.BTN_Ok.Size = new System.Drawing.Size(75, 23);
      this.BTN_Ok.TabIndex = 1;
      this.BTN_Ok.Text = "Ok";
      this.BTN_Ok.UseVisualStyleBackColor = true;
      this.BTN_Ok.Click += new System.EventHandler(this.BTN_Ok_Click);
      // 
      // BTN_abbruch
      // 
      this.BTN_abbruch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.BTN_abbruch.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.BTN_abbruch.Location = new System.Drawing.Point(384, 39);
      this.BTN_abbruch.Name = "BTN_abbruch";
      this.BTN_abbruch.Size = new System.Drawing.Size(75, 23);
      this.BTN_abbruch.TabIndex = 2;
      this.BTN_abbruch.Text = "Abbuch";
      this.BTN_abbruch.UseVisualStyleBackColor = true;
      // 
      // EDT_SeilfehlerPos1
      // 
      this.errorProvider1.SetError(this.EDT_SeilfehlerPos1, "Der eigegebene Wert ist ungültig!");
      this.EDT_SeilfehlerPos1.Location = new System.Drawing.Point(70, 29);
      this.EDT_SeilfehlerPos1.Name = "EDT_SeilfehlerPos1";
      this.EDT_SeilfehlerPos1.Size = new System.Drawing.Size(100, 20);
      this.EDT_SeilfehlerPos1.TabIndex = 1;
      this.EDT_SeilfehlerPos1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.EDT_SeilfehlerPos1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EDT_SeilfehlerPos1_KeyUp);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(20, 32);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(44, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Position";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(176, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(21, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "[m]";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.groupBox2);
      this.groupBox1.Controls.Add(this.EDT_SeilfehlerPos1);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(366, 235);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Drahtbruch";
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // RBTN_AussenTyp1
      // 
      this.RBTN_AussenTyp1.AutoSize = true;
      this.RBTN_AussenTyp1.Location = new System.Drawing.Point(15, 19);
      this.RBTN_AussenTyp1.Name = "RBTN_AussenTyp1";
      this.RBTN_AussenTyp1.Size = new System.Drawing.Size(108, 17);
      this.RBTN_AussenTyp1.TabIndex = 3;
      this.RBTN_AussenTyp1.TabStop = true;
      this.RBTN_AussenTyp1.Text = "Aussen DB Typ 1";
      this.RBTN_AussenTyp1.UseVisualStyleBackColor = true;
      // 
      // RBTN_AussenTyp2
      // 
      this.RBTN_AussenTyp2.AutoSize = true;
      this.RBTN_AussenTyp2.Location = new System.Drawing.Point(15, 42);
      this.RBTN_AussenTyp2.Name = "RBTN_AussenTyp2";
      this.RBTN_AussenTyp2.Size = new System.Drawing.Size(154, 17);
      this.RBTN_AussenTyp2.TabIndex = 3;
      this.RBTN_AussenTyp2.TabStop = true;
      this.RBTN_AussenTyp2.Text = "Aussen DB Typ 2 (W-Form)";
      this.RBTN_AussenTyp2.UseVisualStyleBackColor = true;
      // 
      // RBTN_AussenTyp3
      // 
      this.RBTN_AussenTyp3.AutoSize = true;
      this.RBTN_AussenTyp3.Location = new System.Drawing.Point(15, 65);
      this.RBTN_AussenTyp3.Name = "RBTN_AussenTyp3";
      this.RBTN_AussenTyp3.Size = new System.Drawing.Size(299, 17);
      this.RBTN_AussenTyp3.TabIndex = 3;
      this.RBTN_AussenTyp3.TabStop = true;
      this.RBTN_AussenTyp3.Text = "Drahtbruch aussen 3 (ausgebrochene Draehte, Zahnform)";
      this.RBTN_AussenTyp3.UseVisualStyleBackColor = true;
      // 
      // RBTN_Innen
      // 
      this.RBTN_Innen.AutoSize = true;
      this.RBTN_Innen.Location = new System.Drawing.Point(15, 88);
      this.RBTN_Innen.Name = "RBTN_Innen";
      this.RBTN_Innen.Size = new System.Drawing.Size(209, 17);
      this.RBTN_Innen.TabIndex = 3;
      this.RBTN_Innen.TabStop = true;
      this.RBTN_Innen.Text = "Drahtbruch innen (verbreiterte W-Form)";
      this.RBTN_Innen.UseVisualStyleBackColor = true;
      // 
      // RBTN_KerbeNarbe
      // 
      this.RBTN_KerbeNarbe.AutoSize = true;
      this.RBTN_KerbeNarbe.Location = new System.Drawing.Point(15, 111);
      this.RBTN_KerbeNarbe.Name = "RBTN_KerbeNarbe";
      this.RBTN_KerbeNarbe.Size = new System.Drawing.Size(93, 17);
      this.RBTN_KerbeNarbe.TabIndex = 3;
      this.RBTN_KerbeNarbe.TabStop = true;
      this.RBTN_KerbeNarbe.Text = "Kerbe / Narbe";
      this.RBTN_KerbeNarbe.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.RBTN_AussenTyp1);
      this.groupBox2.Controls.Add(this.RBTN_AussenTyp3);
      this.groupBox2.Controls.Add(this.RBTN_Innen);
      this.groupBox2.Controls.Add(this.RBTN_AussenTyp2);
      this.groupBox2.Controls.Add(this.RBTN_KerbeNarbe);
      this.groupBox2.Location = new System.Drawing.Point(23, 66);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(324, 142);
      this.groupBox2.TabIndex = 4;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Drahtbruchtyp";
      // 
      // DlgFehlerstelleInsert
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.ClientSize = new System.Drawing.Size(471, 263);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.BTN_abbruch);
      this.Controls.Add(this.BTN_Ok);
      this.Name = "DlgFehlerstelleInsert";
      this.Text = "Fehlerstelle einfügen";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button BTN_Ok;
    private System.Windows.Forms.Button BTN_abbruch;
    private System.Windows.Forms.TextBox EDT_SeilfehlerPos1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.RadioButton RBTN_AussenTyp3;
    private System.Windows.Forms.RadioButton RBTN_AussenTyp2;
    private System.Windows.Forms.RadioButton RBTN_KerbeNarbe;
    private System.Windows.Forms.RadioButton RBTN_Innen;
    private System.Windows.Forms.RadioButton RBTN_AussenTyp1;
    private System.Windows.Forms.GroupBox groupBox2;
  }
}