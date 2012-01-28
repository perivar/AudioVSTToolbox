
namespace ProcessVSTPlugin
{
	partial class AnalyseForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.frequencyAnalyserUserControl1 = new CommonUtils.GUI.FrequencyAnalyserUserControl();
			this.OnOffCheckbox = new System.Windows.Forms.CheckBox();
			this.WindowsSizeComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SuspendLayout();
			// 
			// frequencyAnalyserUserControl1
			// 
			this.frequencyAnalyserUserControl1.FFTOverlap = 1;
			this.frequencyAnalyserUserControl1.FFTWindowsSize = 2048;
			this.frequencyAnalyserUserControl1.FoundMaxDecibel = 0F;
			this.frequencyAnalyserUserControl1.FoundMaxFrequency = 0F;
			this.frequencyAnalyserUserControl1.Location = new System.Drawing.Point(0, 1);
			this.frequencyAnalyserUserControl1.MaximumFrequency = 20000F;
			this.frequencyAnalyserUserControl1.MinimumFrequency = 0F;
			this.frequencyAnalyserUserControl1.Name = "frequencyAnalyserUserControl1";
			this.frequencyAnalyserUserControl1.SampleRate = 44100D;
			this.frequencyAnalyserUserControl1.Size = new System.Drawing.Size(665, 353);
			this.frequencyAnalyserUserControl1.TabIndex = 0;
			// 
			// OnOffCheckbox
			// 
			this.OnOffCheckbox.Checked = true;
			this.OnOffCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.OnOffCheckbox.Location = new System.Drawing.Point(671, 3);
			this.OnOffCheckbox.Name = "OnOffCheckbox";
			this.OnOffCheckbox.Size = new System.Drawing.Size(66, 24);
			this.OnOffCheckbox.TabIndex = 1;
			this.OnOffCheckbox.Text = "Turn On";
			this.OnOffCheckbox.UseVisualStyleBackColor = true;
			this.OnOffCheckbox.CheckedChanged += new System.EventHandler(this.OnOffCheckboxCheckedChanged);
			// 
			// WindowsSizeComboBox
			// 
			this.WindowsSizeComboBox.FormattingEnabled = true;
			this.WindowsSizeComboBox.Items.AddRange(new object[] {
									"256",
									"512",
									"1024",
									"2048",
									"4096",
									"8192"});
			this.WindowsSizeComboBox.Location = new System.Drawing.Point(671, 48);
			this.WindowsSizeComboBox.Name = "WindowsSizeComboBox";
			this.WindowsSizeComboBox.Size = new System.Drawing.Size(66, 21);
			this.WindowsSizeComboBox.TabIndex = 2;
			this.WindowsSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(671, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Window Size";
			// 
			// trackBar1
			// 
			this.trackBar1.LargeChange = 1000;
			this.trackBar1.Location = new System.Drawing.Point(671, 92);
			this.trackBar1.Maximum = 20000;
			this.trackBar1.Minimum = 500;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.trackBar1.Size = new System.Drawing.Size(70, 45);
			this.trackBar1.SmallChange = 100;
			this.trackBar1.TabIndex = 4;
			this.trackBar1.TickFrequency = 2000;
			this.trackBar1.Value = 20000;
			this.trackBar1.Scroll += new System.EventHandler(this.TrackBar1Scroll);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(677, 74);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Max Freq.";
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
			// 
			// AnalyseForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(743, 360);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.WindowsSizeComboBox);
			this.Controls.Add(this.OnOffCheckbox);
			this.Controls.Add(this.frequencyAnalyserUserControl1);
			this.Name = "AnalyseForm";
			this.Text = "Frequency Analyser";
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar trackBar1;
		private CommonUtils.GUI.FrequencyAnalyserUserControl frequencyAnalyserUserControl1;
		private System.Windows.Forms.CheckBox OnOffCheckbox;
		private System.Windows.Forms.ComboBox WindowsSizeComboBox;
		private System.Windows.Forms.Label label1;
	}
}
