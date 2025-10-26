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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1265, 732);
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
    }
}
