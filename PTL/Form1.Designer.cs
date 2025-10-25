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
            buttonToggleUnit = new Button();
            trackBar1 = new TrackBar();
            trackBar2 = new TrackBar();
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
            formsPlot1.Load += formsPlot1_Load;
            // 
            // buttonToggleUnit
            // 
            buttonToggleUnit.Location = new Point(12, 12);
            buttonToggleUnit.Name = "buttonToggleUnit";
            buttonToggleUnit.Size = new Size(150, 23);
            buttonToggleUnit.TabIndex = 1;
            buttonToggleUnit.Text = "Afficher en mph";
            buttonToggleUnit.UseVisualStyleBackColor = true;
            buttonToggleUnit.Click += ToggleSpeedButton_Click;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1265, 732);
            Controls.Add(trackBar2);
            Controls.Add(trackBar1);
            Controls.Add(buttonToggleUnit);
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
        private System.Windows.Forms.Button buttonToggleUnit;
        private TrackBar trackBar1;
        private TrackBar trackBar2;
    }
}
