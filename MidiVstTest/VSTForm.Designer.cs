namespace MidiVstTest
{
    partial class VSTForm
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
        	this.waveformPainter1 = new NAudio.Gui.WaveformPainter();
        	this.waveformPainter2 = new NAudio.Gui.WaveformPainter();
        	this.timer1 = new System.Windows.Forms.Timer(this.components);
        	this.SuspendLayout();
        	// 
        	// waveformPainter1
        	// 
        	this.waveformPainter1.BackColor = System.Drawing.Color.Black;
        	this.waveformPainter1.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.waveformPainter1.ForeColor = System.Drawing.Color.SteelBlue;
        	this.waveformPainter1.Location = new System.Drawing.Point(0, 226);
        	this.waveformPainter1.Name = "waveformPainter1";
        	this.waveformPainter1.Size = new System.Drawing.Size(336, 50);
        	this.waveformPainter1.TabIndex = 3;
        	this.waveformPainter1.Text = "waveformPainter1";
        	// 
        	// waveformPainter2
        	// 
        	this.waveformPainter2.BackColor = System.Drawing.Color.Black;
        	this.waveformPainter2.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.waveformPainter2.ForeColor = System.Drawing.Color.SteelBlue;
        	this.waveformPainter2.Location = new System.Drawing.Point(0, 176);
        	this.waveformPainter2.Name = "waveformPainter2";
        	this.waveformPainter2.Size = new System.Drawing.Size(336, 50);
        	this.waveformPainter2.TabIndex = 1;
        	this.waveformPainter2.Text = "waveformPainter2";
        	// 
        	// timer1
        	// 
        	this.timer1.Enabled = true;
        	this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
        	// 
        	// VSTForm
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(336, 276);
        	this.Controls.Add(this.waveformPainter2);
        	this.Controls.Add(this.waveformPainter1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "VSTForm";
        	this.ShowInTaskbar = false;
        	this.Text = "VSTForm";
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VSTForm_FormClosing);
        	this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VSTForm_FormClosed);
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.Timer timer1;

        #endregion

        private NAudio.Gui.WaveformPainter waveformPainter1;
        private NAudio.Gui.WaveformPainter waveformPainter2;
    }
}