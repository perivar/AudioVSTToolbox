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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VSTForm));
        	this.waveformPainter1 = new NAudio.Gui.WaveformPainter();
        	this.waveformPainter2 = new NAudio.Gui.WaveformPainter();
        	this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        	this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
        	this.tsbLoad = new System.Windows.Forms.ToolStripButton();
        	this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        	this.tsbPlay = new System.Windows.Forms.ToolStripButton();
        	this.tsbStop = new System.Windows.Forms.ToolStripButton();
        	this.tslNowTime = new System.Windows.Forms.ToolStripLabel();
        	this.tslTotalTime = new System.Windows.Forms.ToolStripLabel();
        	this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        	this.tsbSave = new System.Windows.Forms.ToolStripButton();
        	this.tsbRec = new System.Windows.Forms.ToolStripButton();
        	this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
        	this.tsbMixer = new System.Windows.Forms.ToolStripButton();
        	this.timer1 = new System.Windows.Forms.Timer(this.components);
        	this.toolStrip1.SuspendLayout();
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
        	// toolStrip1
        	// 
        	this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
        	this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
        	this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.toolStripLabel1,
        	        	        	this.tsbLoad,
        	        	        	this.toolStripSeparator1,
        	        	        	this.tsbPlay,
        	        	        	this.tsbStop,
        	        	        	this.tslNowTime,
        	        	        	this.tslTotalTime,
        	        	        	this.toolStripSeparator2,
        	        	        	this.tsbSave,
        	        	        	this.tsbRec,
        	        	        	this.toolStripSeparator3,
        	        	        	this.tsbMixer});
        	this.toolStrip1.Location = new System.Drawing.Point(0, 151);
        	this.toolStrip1.Name = "toolStrip1";
        	this.toolStrip1.Size = new System.Drawing.Size(336, 25);
        	this.toolStrip1.TabIndex = 2;
        	this.toolStrip1.Text = "toolStrip1";
        	// 
        	// toolStripLabel1
        	// 
        	this.toolStripLabel1.Name = "toolStripLabel1";
        	this.toolStripLabel1.Size = new System.Drawing.Size(69, 22);
        	this.toolStripLabel1.Text = "Mp3 Player:";
        	// 
        	// tsbLoad
        	// 
        	this.tsbLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbLoad.Image = ((System.Drawing.Image)(resources.GetObject("tsbLoad.Image")));
        	this.tsbLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbLoad.Name = "tsbLoad";
        	this.tsbLoad.Size = new System.Drawing.Size(23, 22);
        	this.tsbLoad.Text = "Load";
        	this.tsbLoad.Click += new System.EventHandler(this.tsbLoad_Click);
        	// 
        	// toolStripSeparator1
        	// 
        	this.toolStripSeparator1.Name = "toolStripSeparator1";
        	this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
        	// 
        	// tsbPlay
        	// 
        	this.tsbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbPlay.Image = ((System.Drawing.Image)(resources.GetObject("tsbPlay.Image")));
        	this.tsbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbPlay.Name = "tsbPlay";
        	this.tsbPlay.Size = new System.Drawing.Size(23, 22);
        	this.tsbPlay.Text = "Play";
        	this.tsbPlay.Click += new System.EventHandler(this.tsbPlay_Click);
        	// 
        	// tsbStop
        	// 
        	this.tsbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbStop.Image = ((System.Drawing.Image)(resources.GetObject("tsbStop.Image")));
        	this.tsbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbStop.Name = "tsbStop";
        	this.tsbStop.Size = new System.Drawing.Size(23, 22);
        	this.tsbStop.Text = "Pause/Stop";
        	this.tsbStop.Click += new System.EventHandler(this.tsbStop_Click);
        	// 
        	// tslNowTime
        	// 
        	this.tslNowTime.Name = "tslNowTime";
        	this.tslNowTime.Size = new System.Drawing.Size(22, 22);
        	this.tslNowTime.Text = "---";
        	// 
        	// tslTotalTime
        	// 
        	this.tslTotalTime.Name = "tslTotalTime";
        	this.tslTotalTime.Size = new System.Drawing.Size(30, 22);
        	this.tslTotalTime.Text = "/ ---";
        	// 
        	// toolStripSeparator2
        	// 
        	this.toolStripSeparator2.Name = "toolStripSeparator2";
        	this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
        	// 
        	// tsbSave
        	// 
        	this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
        	this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbSave.Name = "tsbSave";
        	this.tsbSave.Size = new System.Drawing.Size(23, 22);
        	this.tsbSave.Text = "Save";
        	this.tsbSave.Click += new System.EventHandler(this.tsbSave_Click);
        	// 
        	// tsbRec
        	// 
        	this.tsbRec.BackColor = System.Drawing.Color.Transparent;
        	this.tsbRec.CheckOnClick = true;
        	this.tsbRec.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbRec.Image = ((System.Drawing.Image)(resources.GetObject("tsbRec.Image")));
        	this.tsbRec.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbRec.Name = "tsbRec";
        	this.tsbRec.Size = new System.Drawing.Size(23, 22);
        	this.tsbRec.Text = "Rec";
        	this.tsbRec.CheckedChanged += new System.EventHandler(this.tsbRec_CheckedChanged);
        	// 
        	// toolStripSeparator3
        	// 
        	this.toolStripSeparator3.Name = "toolStripSeparator3";
        	this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
        	// 
        	// tsbMixer
        	// 
        	this.tsbMixer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        	this.tsbMixer.Image = ((System.Drawing.Image)(resources.GetObject("tsbMixer.Image")));
        	this.tsbMixer.ImageTransparentColor = System.Drawing.Color.Magenta;
        	this.tsbMixer.Name = "tsbMixer";
        	this.tsbMixer.Size = new System.Drawing.Size(23, 22);
        	this.tsbMixer.Text = "Mixer";
        	this.tsbMixer.Click += new System.EventHandler(this.tsbMixer_Click);
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
        	this.Controls.Add(this.toolStrip1);
        	this.Controls.Add(this.waveformPainter2);
        	this.Controls.Add(this.waveformPainter1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        	this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "VSTForm";
        	this.ShowInTaskbar = false;
        	this.Text = "VSTForm";
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VSTForm_FormClosing);
        	this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VSTForm_FormClosed);
        	this.toolStrip1.ResumeLayout(false);
        	this.toolStrip1.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }

        #endregion

        private NAudio.Gui.WaveformPainter waveformPainter1;
        private NAudio.Gui.WaveformPainter waveformPainter2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbLoad;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbPlay;
        private System.Windows.Forms.ToolStripButton tsbStop;
        private System.Windows.Forms.ToolStripLabel tslNowTime;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbRec;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsbMixer;
        private System.Windows.Forms.ToolStripLabel tslTotalTime;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;




    }
}