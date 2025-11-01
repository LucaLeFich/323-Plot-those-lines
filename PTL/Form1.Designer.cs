namespace PTL
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            trackBar1 = new TrackBar();
            trackBar2 = new TrackBar();
            comboBox1 = new ComboBox();
            btnExport = new Button();
            btnImport = new Button();
            BtnMin = new Button();
            BtnMax = new Button();
            BtnAvg = new Button();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).BeginInit();
            SuspendLayout();
            // 
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Location = new Point(158, 91);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(922, 476);
            formsPlot1.TabIndex = 0;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(727, 584);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(337, 45);
            trackBar1.TabIndex = 2;
            // 
            // trackBar2
            // 
            trackBar2.Location = new Point(222, 584);
            trackBar2.Name = "trackBar2";
            trackBar2.Size = new Size(337, 45);
            trackBar2.TabIndex = 3;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(12, 12);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(217, 23);
            comboBox1.TabIndex = 4;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(1119, 12);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(134, 23);
            btnExport.TabIndex = 5;
            btnExport.Text = "Exporter le graphique";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnImport
            // 
            btnImport.Location = new Point(1119, 41);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(134, 23);
            btnImport.TabIndex = 6;
            btnImport.Text = "Importer des données";
            btnImport.TextAlign = ContentAlignment.MiddleLeft;
            btnImport.UseVisualStyleBackColor = true;
            btnImport.Click += btnImport_Click;
            // 
            // BtnMin
            // 
            BtnMin.Location = new Point(1095, 114);
            BtnMin.Name = "BtnMin";
            BtnMin.Size = new Size(144, 23);
            BtnMin.TabIndex = 7;
            BtnMin.Text = "Valeure minimum";
            BtnMin.UseVisualStyleBackColor = true;
            BtnMin.Click += BtnMin_Click;
            // 
            // BtnMax
            // 
            BtnMax.Location = new Point(1095, 143);
            BtnMax.Name = "BtnMax";
            BtnMax.Size = new Size(144, 23);
            BtnMax.TabIndex = 8;
            BtnMax.Text = "Valeure maximum";
            BtnMax.UseVisualStyleBackColor = true;
            BtnMax.Click += BtnMax_Click;
            // 
            // BtnAvg
            // 
            BtnAvg.Location = new Point(1095, 172);
            BtnAvg.Name = "BtnAvg";
            BtnAvg.Size = new Size(144, 23);
            BtnAvg.TabIndex = 9;
            BtnAvg.Text = "Moyenne";
            BtnAvg.UseVisualStyleBackColor = true;
            BtnAvg.Click += BtnAvg_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(12, 44);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(230, 19);
            checkBox1.TabIndex = 10;
            checkBox1.Text = "Afficher km/h et mph en même temps";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(12, 69);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(195, 19);
            checkBox2.TabIndex = 11;
            checkBox2.Text = "Afficher distance en km et miles";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1265, 732);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(BtnAvg);
            Controls.Add(BtnMax);
            Controls.Add(BtnMin);
            Controls.Add(btnImport);
            Controls.Add(btnExport);
            Controls.Add(comboBox1);
            Controls.Add(trackBar2);
            Controls.Add(trackBar1);
            Controls.Add(formsPlot1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private TrackBar trackBar1;
        private TrackBar trackBar2;
        private ComboBox comboBox1;
        private Button btnExport;
        private Button btnImport;
        private Button BtnMin;
        private Button BtnMax;
        private Button BtnAvg;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
    }
}
