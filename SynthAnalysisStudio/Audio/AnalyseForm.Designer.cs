/*
 * Created by SharpDevelop.
 * User: perivar.nerseth
 * Date: 02.01.2012
 * Time: 00:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SynthAnalysisStudio
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
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.foundFreqTextBox = new System.Windows.Forms.TextBox();
			this.foundDBTextBox = new System.Windows.Forms.TextBox();
			this.freqSampleBtn = new System.Windows.Forms.Button();
			this.filterATextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.filterBTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.filterCtrlTextBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
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
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(670, 126);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Freq.";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(671, 167);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 15);
			this.label4.TabIndex = 7;
			this.label4.Text = "dB";
			// 
			// foundFreqTextBox
			// 
			this.foundFreqTextBox.Location = new System.Drawing.Point(670, 142);
			this.foundFreqTextBox.Name = "foundFreqTextBox";
			this.foundFreqTextBox.Size = new System.Drawing.Size(67, 20);
			this.foundFreqTextBox.TabIndex = 8;
			// 
			// foundDBTextBox
			// 
			this.foundDBTextBox.Location = new System.Drawing.Point(671, 181);
			this.foundDBTextBox.Name = "foundDBTextBox";
			this.foundDBTextBox.Size = new System.Drawing.Size(67, 20);
			this.foundDBTextBox.TabIndex = 9;
			// 
			// freqSampleBtn
			// 
			this.freqSampleBtn.Location = new System.Drawing.Point(671, 316);
			this.freqSampleBtn.Name = "freqSampleBtn";
			this.freqSampleBtn.Size = new System.Drawing.Size(67, 38);
			this.freqSampleBtn.TabIndex = 10;
			this.freqSampleBtn.Text = "Freq. Sample";
			this.freqSampleBtn.UseVisualStyleBackColor = true;
			this.freqSampleBtn.Click += new System.EventHandler(this.FreqSampleBtnClick);
			// 
			// filterATextBox
			// 
			this.filterATextBox.Location = new System.Drawing.Point(671, 218);
			this.filterATextBox.Name = "filterATextBox";
			this.filterATextBox.Size = new System.Drawing.Size(67, 20);
			this.filterATextBox.TabIndex = 12;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(671, 204);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 15);
			this.label5.TabIndex = 11;
			this.label5.Text = "FilterA";
			// 
			// filterBTextBox
			// 
			this.filterBTextBox.Location = new System.Drawing.Point(671, 256);
			this.filterBTextBox.Name = "filterBTextBox";
			this.filterBTextBox.Size = new System.Drawing.Size(67, 20);
			this.filterBTextBox.TabIndex = 14;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(671, 242);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(54, 15);
			this.label6.TabIndex = 13;
			this.label6.Text = "FilterB";
			// 
			// filterCtrlTextBox
			// 
			this.filterCtrlTextBox.Location = new System.Drawing.Point(671, 294);
			this.filterCtrlTextBox.Name = "filterCtrlTextBox";
			this.filterCtrlTextBox.Size = new System.Drawing.Size(67, 20);
			this.filterCtrlTextBox.TabIndex = 16;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(671, 280);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(54, 15);
			this.label7.TabIndex = 15;
			this.label7.Text = "FilterCtrl";
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
			this.Controls.Add(this.filterCtrlTextBox);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.filterBTextBox);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.filterATextBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.freqSampleBtn);
			this.Controls.Add(this.foundDBTextBox);
			this.Controls.Add(this.foundFreqTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.WindowsSizeComboBox);
			this.Controls.Add(this.OnOffCheckbox);
			this.Controls.Add(this.frequencyAnalyserUserControl1);
			this.Name = "AnalyseForm";
			this.Text = "Frequency - Analysis";
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox filterCtrlTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox filterBTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox filterATextBox;
		private System.Windows.Forms.Button freqSampleBtn;
		private System.Windows.Forms.TextBox foundDBTextBox;
		private System.Windows.Forms.TextBox foundFreqTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar trackBar1;
		private CommonUtils.GUI.FrequencyAnalyserUserControl frequencyAnalyserUserControl1;
		private System.Windows.Forms.CheckBox OnOffCheckbox;
		private System.Windows.Forms.ComboBox WindowsSizeComboBox;
		private System.Windows.Forms.Label label1;
	}
}
