/*
 * Created by SharpDevelop.
 * User: perivar.nerseth
 * Date: 11.01.2012
 * Time: 18:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace SynthAnalysisStudio
{
	partial class WaveDisplayForm
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
			this.MaxResolutionTrackBar = new System.Windows.Forms.TrackBar();
			this.waveDisplayUserControl1 = new CommonUtils.GUI.WaveDisplayUserControl();
			this.label2 = new System.Windows.Forms.Label();
			this.OnOffCheckbox = new System.Windows.Forms.CheckBox();
			this.recordBtn = new System.Windows.Forms.Button();
			this.stopBtn = new System.Windows.Forms.Button();
			this.clearBtn = new System.Windows.Forms.Button();
			this.AmplitudeTrackBar = new System.Windows.Forms.TrackBar();
			this.StartPositionTrackBar = new System.Windows.Forms.TrackBar();
			this.CropBtn = new System.Windows.Forms.Button();
			this.MidiNoteCheckbox = new System.Windows.Forms.CheckBox();
			this.SaveWAVBtn = new System.Windows.Forms.Button();
			this.adsrSampleBtn = new System.Windows.Forms.Button();
			this.durationSamplesTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.attackTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.decayTextBox = new System.Windows.Forms.TextBox();
			this.sustainTextBox = new System.Windows.Forms.TextBox();
			this.releaseTextBox = new System.Windows.Forms.TextBox();
			this.durationMsTextBox = new System.Windows.Forms.TextBox();
			this.playMidiC5100msBtn = new System.Windows.Forms.Button();
			this.measureDBtn = new System.Windows.Forms.Button();
			this.measureABtn = new System.Windows.Forms.Button();
			this.measureRBtn = new System.Windows.Forms.Button();
			this.measureLFOBtn = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.MaxResolutionTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.AmplitudeTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.StartPositionTrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// MaxResolutionTrackBar
			// 
			this.MaxResolutionTrackBar.LargeChange = 10;
			this.MaxResolutionTrackBar.Location = new System.Drawing.Point(674, 49);
			this.MaxResolutionTrackBar.Margin = new System.Windows.Forms.Padding(0);
			this.MaxResolutionTrackBar.Maximum = 1000;
			this.MaxResolutionTrackBar.Minimum = 1;
			this.MaxResolutionTrackBar.Name = "MaxResolutionTrackBar";
			this.MaxResolutionTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.MaxResolutionTrackBar.Size = new System.Drawing.Size(70, 45);
			this.MaxResolutionTrackBar.TabIndex = 5;
			this.MaxResolutionTrackBar.TickFrequency = 100;
			this.MaxResolutionTrackBar.Value = 1;
			this.MaxResolutionTrackBar.Scroll += new System.EventHandler(this.MaxResolutionTrackBarScroll);
			// 
			// waveDisplayUserControl1
			// 
			this.waveDisplayUserControl1.Amplitude = 1;
			this.waveDisplayUserControl1.Location = new System.Drawing.Point(51, 35);
			this.waveDisplayUserControl1.Name = "waveDisplayUserControl1";
			this.waveDisplayUserControl1.Resolution = 1;
			this.waveDisplayUserControl1.SampleRate = 44100D;
			this.waveDisplayUserControl1.Size = new System.Drawing.Size(620, 254);
			this.waveDisplayUserControl1.StartPosition = 0;
			this.waveDisplayUserControl1.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(678, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Resolution";
			// 
			// OnOffCheckbox
			// 
			this.OnOffCheckbox.Checked = true;
			this.OnOffCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.OnOffCheckbox.Location = new System.Drawing.Point(678, 6);
			this.OnOffCheckbox.Margin = new System.Windows.Forms.Padding(0);
			this.OnOffCheckbox.Name = "OnOffCheckbox";
			this.OnOffCheckbox.Size = new System.Drawing.Size(66, 24);
			this.OnOffCheckbox.TabIndex = 8;
			this.OnOffCheckbox.Text = "Turn On";
			this.OnOffCheckbox.UseVisualStyleBackColor = true;
			this.OnOffCheckbox.CheckedChanged += new System.EventHandler(this.OnOffCheckboxCheckedChanged);
			// 
			// recordBtn
			// 
			this.recordBtn.Location = new System.Drawing.Point(186, 6);
			this.recordBtn.Name = "recordBtn";
			this.recordBtn.Size = new System.Drawing.Size(51, 24);
			this.recordBtn.TabIndex = 9;
			this.recordBtn.Text = "Record";
			this.recordBtn.UseVisualStyleBackColor = true;
			this.recordBtn.Click += new System.EventHandler(this.RecordBtnClick);
			// 
			// stopBtn
			// 
			this.stopBtn.Location = new System.Drawing.Point(239, 7);
			this.stopBtn.Name = "stopBtn";
			this.stopBtn.Size = new System.Drawing.Size(51, 23);
			this.stopBtn.TabIndex = 10;
			this.stopBtn.Text = "Stop";
			this.stopBtn.UseVisualStyleBackColor = true;
			this.stopBtn.Click += new System.EventHandler(this.StopBtnClick);
			// 
			// clearBtn
			// 
			this.clearBtn.Location = new System.Drawing.Point(292, 7);
			this.clearBtn.Name = "clearBtn";
			this.clearBtn.Size = new System.Drawing.Size(51, 23);
			this.clearBtn.TabIndex = 11;
			this.clearBtn.Text = "Clear";
			this.clearBtn.UseVisualStyleBackColor = true;
			this.clearBtn.Click += new System.EventHandler(this.ClearBtnClick);
			// 
			// AmplitudeTrackBar
			// 
			this.AmplitudeTrackBar.LargeChange = 1;
			this.AmplitudeTrackBar.Location = new System.Drawing.Point(0, 96);
			this.AmplitudeTrackBar.Minimum = 1;
			this.AmplitudeTrackBar.Name = "AmplitudeTrackBar";
			this.AmplitudeTrackBar.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.AmplitudeTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.AmplitudeTrackBar.Size = new System.Drawing.Size(45, 70);
			this.AmplitudeTrackBar.TabIndex = 12;
			this.AmplitudeTrackBar.Value = 1;
			this.AmplitudeTrackBar.Scroll += new System.EventHandler(this.AmplitudeTrackBarScroll);
			// 
			// StartPositionTrackBar
			// 
			this.StartPositionTrackBar.LargeChange = 4410;
			this.StartPositionTrackBar.Location = new System.Drawing.Point(674, 76);
			this.StartPositionTrackBar.Margin = new System.Windows.Forms.Padding(0);
			this.StartPositionTrackBar.Maximum = 441000;
			this.StartPositionTrackBar.Name = "StartPositionTrackBar";
			this.StartPositionTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.StartPositionTrackBar.Size = new System.Drawing.Size(70, 45);
			this.StartPositionTrackBar.SmallChange = 441;
			this.StartPositionTrackBar.TabIndex = 13;
			this.StartPositionTrackBar.TickFrequency = 44100;
			this.StartPositionTrackBar.Scroll += new System.EventHandler(this.StartPositionTrackBarScroll);
			// 
			// CropBtn
			// 
			this.CropBtn.Location = new System.Drawing.Point(345, 7);
			this.CropBtn.Name = "CropBtn";
			this.CropBtn.Size = new System.Drawing.Size(51, 23);
			this.CropBtn.TabIndex = 14;
			this.CropBtn.Text = "Crop";
			this.CropBtn.UseVisualStyleBackColor = true;
			this.CropBtn.Click += new System.EventHandler(this.CropBtnClick);
			// 
			// MidiNoteCheckbox
			// 
			this.MidiNoteCheckbox.Location = new System.Drawing.Point(104, 8);
			this.MidiNoteCheckbox.Name = "MidiNoteCheckbox";
			this.MidiNoteCheckbox.Size = new System.Drawing.Size(76, 24);
			this.MidiNoteCheckbox.TabIndex = 16;
			this.MidiNoteCheckbox.Text = "Midi Note";
			this.MidiNoteCheckbox.UseVisualStyleBackColor = true;
			this.MidiNoteCheckbox.CheckedChanged += new System.EventHandler(this.MidiNoteCheckboxCheckedChanged);
			// 
			// SaveWAVBtn
			// 
			this.SaveWAVBtn.Location = new System.Drawing.Point(598, 8);
			this.SaveWAVBtn.Name = "SaveWAVBtn";
			this.SaveWAVBtn.Size = new System.Drawing.Size(73, 23);
			this.SaveWAVBtn.TabIndex = 17;
			this.SaveWAVBtn.Text = "Save Wav";
			this.SaveWAVBtn.UseVisualStyleBackColor = true;
			this.SaveWAVBtn.Click += new System.EventHandler(this.SaveWAVBtnClick);
			// 
			// adsrSampleBtn
			// 
			this.adsrSampleBtn.Location = new System.Drawing.Point(678, 269);
			this.adsrSampleBtn.Name = "adsrSampleBtn";
			this.adsrSampleBtn.Size = new System.Drawing.Size(67, 20);
			this.adsrSampleBtn.TabIndex = 20;
			this.adsrSampleBtn.Text = "Save";
			this.adsrSampleBtn.UseVisualStyleBackColor = true;
			this.adsrSampleBtn.Click += new System.EventHandler(this.AdsrSampleBtnClick);
			// 
			// durationSamplesTextBox
			// 
			this.durationSamplesTextBox.Location = new System.Drawing.Point(678, 119);
			this.durationSamplesTextBox.Name = "durationSamplesTextBox";
			this.durationSamplesTextBox.Size = new System.Drawing.Size(67, 20);
			this.durationSamplesTextBox.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(678, 106);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 16);
			this.label3.TabIndex = 18;
			this.label3.Text = "Duration (ms)";
			// 
			// attackTextBox
			// 
			this.attackTextBox.Location = new System.Drawing.Point(678, 180);
			this.attackTextBox.Name = "attackTextBox";
			this.attackTextBox.Size = new System.Drawing.Size(67, 20);
			this.attackTextBox.TabIndex = 22;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(678, 166);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 16);
			this.label1.TabIndex = 21;
			this.label1.Text = "A D S R";
			// 
			// decayTextBox
			// 
			this.decayTextBox.Location = new System.Drawing.Point(678, 202);
			this.decayTextBox.Name = "decayTextBox";
			this.decayTextBox.Size = new System.Drawing.Size(67, 20);
			this.decayTextBox.TabIndex = 23;
			// 
			// sustainTextBox
			// 
			this.sustainTextBox.Location = new System.Drawing.Point(678, 224);
			this.sustainTextBox.Name = "sustainTextBox";
			this.sustainTextBox.Size = new System.Drawing.Size(67, 20);
			this.sustainTextBox.TabIndex = 24;
			// 
			// releaseTextBox
			// 
			this.releaseTextBox.Location = new System.Drawing.Point(678, 246);
			this.releaseTextBox.Name = "releaseTextBox";
			this.releaseTextBox.Size = new System.Drawing.Size(67, 20);
			this.releaseTextBox.TabIndex = 25;
			// 
			// durationMsTextBox
			// 
			this.durationMsTextBox.Location = new System.Drawing.Point(678, 141);
			this.durationMsTextBox.Name = "durationMsTextBox";
			this.durationMsTextBox.Size = new System.Drawing.Size(67, 20);
			this.durationMsTextBox.TabIndex = 26;
			// 
			// playMidiC5100msBtn
			// 
			this.playMidiC5100msBtn.Location = new System.Drawing.Point(12, 7);
			this.playMidiC5100msBtn.Name = "playMidiC5100msBtn";
			this.playMidiC5100msBtn.Size = new System.Drawing.Size(87, 24);
			this.playMidiC5100msBtn.TabIndex = 27;
			this.playMidiC5100msBtn.Text = "Play C5 100ms";
			this.playMidiC5100msBtn.UseVisualStyleBackColor = true;
			this.playMidiC5100msBtn.Click += new System.EventHandler(this.PlayMidiC5100msBtnClick);
			// 
			// measureDBtn
			// 
			this.measureDBtn.Location = new System.Drawing.Point(456, 7);
			this.measureDBtn.Name = "measureDBtn";
			this.measureDBtn.Size = new System.Drawing.Size(19, 24);
			this.measureDBtn.TabIndex = 28;
			this.measureDBtn.Text = "D";
			this.measureDBtn.UseVisualStyleBackColor = true;
			this.measureDBtn.Click += new System.EventHandler(this.MeasureDBtnClick);
			// 
			// measureABtn
			// 
			this.measureABtn.Location = new System.Drawing.Point(434, 7);
			this.measureABtn.Name = "measureABtn";
			this.measureABtn.Size = new System.Drawing.Size(19, 24);
			this.measureABtn.TabIndex = 29;
			this.measureABtn.Text = "A";
			this.measureABtn.UseVisualStyleBackColor = true;
			this.measureABtn.Click += new System.EventHandler(this.MeasureABtnClick);
			// 
			// measureRBtn
			// 
			this.measureRBtn.Location = new System.Drawing.Point(478, 7);
			this.measureRBtn.Name = "measureRBtn";
			this.measureRBtn.Size = new System.Drawing.Size(19, 24);
			this.measureRBtn.TabIndex = 30;
			this.measureRBtn.Text = "R";
			this.measureRBtn.UseVisualStyleBackColor = true;
			this.measureRBtn.Click += new System.EventHandler(this.MeasureRBtnClick);
			// 
			// measureLFOBtn
			// 
			this.measureLFOBtn.Location = new System.Drawing.Point(523, 8);
			this.measureLFOBtn.Name = "measureLFOBtn";
			this.measureLFOBtn.Size = new System.Drawing.Size(35, 24);
			this.measureLFOBtn.TabIndex = 31;
			this.measureLFOBtn.Text = "LFO";
			this.measureLFOBtn.UseVisualStyleBackColor = true;
			this.measureLFOBtn.Click += new System.EventHandler(this.MeasureLFOBtnClick);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
			// 
			// WaveDisplayForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(755, 289);
			this.Controls.Add(this.measureLFOBtn);
			this.Controls.Add(this.measureRBtn);
			this.Controls.Add(this.measureABtn);
			this.Controls.Add(this.measureDBtn);
			this.Controls.Add(this.playMidiC5100msBtn);
			this.Controls.Add(this.durationMsTextBox);
			this.Controls.Add(this.releaseTextBox);
			this.Controls.Add(this.sustainTextBox);
			this.Controls.Add(this.decayTextBox);
			this.Controls.Add(this.attackTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.adsrSampleBtn);
			this.Controls.Add(this.durationSamplesTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.SaveWAVBtn);
			this.Controls.Add(this.MidiNoteCheckbox);
			this.Controls.Add(this.CropBtn);
			this.Controls.Add(this.StartPositionTrackBar);
			this.Controls.Add(this.AmplitudeTrackBar);
			this.Controls.Add(this.clearBtn);
			this.Controls.Add(this.stopBtn);
			this.Controls.Add(this.recordBtn);
			this.Controls.Add(this.OnOffCheckbox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.waveDisplayUserControl1);
			this.Controls.Add(this.MaxResolutionTrackBar);
			this.Name = "WaveDisplayForm";
			this.Text = "Waveform - Analysis";
			((System.ComponentModel.ISupportInitialize)(this.MaxResolutionTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.AmplitudeTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.StartPositionTrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button measureLFOBtn;
		private System.Windows.Forms.Button measureRBtn;
		private System.Windows.Forms.Button measureABtn;
		private System.Windows.Forms.Button measureDBtn;
		private System.Windows.Forms.Button playMidiC5100msBtn;
		private System.Windows.Forms.TextBox durationMsTextBox;
		private System.Windows.Forms.TextBox releaseTextBox;
		private System.Windows.Forms.TextBox sustainTextBox;
		private System.Windows.Forms.TextBox decayTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox attackTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox durationSamplesTextBox;
		private System.Windows.Forms.Button adsrSampleBtn;
		private System.Windows.Forms.Button SaveWAVBtn;
		private System.Windows.Forms.CheckBox MidiNoteCheckbox;
		private System.Windows.Forms.Button CropBtn;
		private System.Windows.Forms.TrackBar MaxResolutionTrackBar;
		private System.Windows.Forms.TrackBar StartPositionTrackBar;
		private System.Windows.Forms.TrackBar AmplitudeTrackBar;
		private System.Windows.Forms.Button clearBtn;
		private System.Windows.Forms.Button stopBtn;
		private System.Windows.Forms.Button recordBtn;
		private System.Windows.Forms.CheckBox OnOffCheckbox;
		private System.Windows.Forms.Label label2;
		private CommonUtils.GUI.WaveDisplayUserControl waveDisplayUserControl1;
		
	}
}
