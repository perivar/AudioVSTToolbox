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
			this.adsrSaveBtn = new System.Windows.Forms.Button();
			this.durationSamplesTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.ampAttackTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.ampDecayTextBox = new System.Windows.Forms.TextBox();
			this.ampSustainTextBox = new System.Windows.Forms.TextBox();
			this.ampReleaseTextBox = new System.Windows.Forms.TextBox();
			this.durationMsTextBox = new System.Windows.Forms.TextBox();
			this.playMidiC5100msBtn = new System.Windows.Forms.Button();
			this.measureAmpDBtn = new System.Windows.Forms.Button();
			this.measureAmpABtn = new System.Windows.Forms.Button();
			this.measureAmpRBtn = new System.Windows.Forms.Button();
			this.measureLFOBtn = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.measureModDBtn = new System.Windows.Forms.Button();
			this.measureModABtn = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.modAttackTextBox = new System.Windows.Forms.TextBox();
			this.modDecayTextBox = new System.Windows.Forms.TextBox();
			this.modReleaseTextBox = new System.Windows.Forms.TextBox();
			this.modSustainTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.MaxResolutionTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.AmplitudeTrackBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.StartPositionTrackBar)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// MaxResolutionTrackBar
			// 
			this.MaxResolutionTrackBar.LargeChange = 10;
			this.MaxResolutionTrackBar.Location = new System.Drawing.Point(688, 45);
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
			this.waveDisplayUserControl1.Location = new System.Drawing.Point(51, 87);
			this.waveDisplayUserControl1.Name = "waveDisplayUserControl1";
			this.waveDisplayUserControl1.Resolution = 1;
			this.waveDisplayUserControl1.SampleRate = 44100D;
			this.waveDisplayUserControl1.Size = new System.Drawing.Size(620, 278);
			this.waveDisplayUserControl1.StartPosition = 0;
			this.waveDisplayUserControl1.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(688, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Resolution";
			// 
			// OnOffCheckbox
			// 
			this.OnOffCheckbox.Checked = true;
			this.OnOffCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.OnOffCheckbox.Location = new System.Drawing.Point(692, 2);
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
			this.AmplitudeTrackBar.Location = new System.Drawing.Point(3, 174);
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
			this.StartPositionTrackBar.Location = new System.Drawing.Point(688, 72);
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
			this.SaveWAVBtn.Location = new System.Drawing.Point(598, 7);
			this.SaveWAVBtn.Name = "SaveWAVBtn";
			this.SaveWAVBtn.Size = new System.Drawing.Size(73, 23);
			this.SaveWAVBtn.TabIndex = 17;
			this.SaveWAVBtn.Text = "Save Wav";
			this.SaveWAVBtn.UseVisualStyleBackColor = true;
			this.SaveWAVBtn.Click += new System.EventHandler(this.SaveWAVBtnClick);
			// 
			// adsrSaveBtn
			// 
			this.adsrSaveBtn.Location = new System.Drawing.Point(17, 198);
			this.adsrSaveBtn.Name = "adsrSaveBtn";
			this.adsrSaveBtn.Size = new System.Drawing.Size(67, 49);
			this.adsrSaveBtn.TabIndex = 20;
			this.adsrSaveBtn.Text = "Save ADSR Values";
			this.adsrSaveBtn.UseVisualStyleBackColor = true;
			this.adsrSaveBtn.Click += new System.EventHandler(this.AdsrSaveBtnClick);
			// 
			// durationSamplesTextBox
			// 
			this.durationSamplesTextBox.Location = new System.Drawing.Point(18, 29);
			this.durationSamplesTextBox.Name = "durationSamplesTextBox";
			this.durationSamplesTextBox.Size = new System.Drawing.Size(67, 20);
			this.durationSamplesTextBox.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(17, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 16);
			this.label3.TabIndex = 18;
			this.label3.Text = "Duration (ms)";
			// 
			// ampAttackTextBox
			// 
			this.ampAttackTextBox.Location = new System.Drawing.Point(21, 99);
			this.ampAttackTextBox.Name = "ampAttackTextBox";
			this.ampAttackTextBox.Size = new System.Drawing.Size(34, 20);
			this.ampAttackTextBox.TabIndex = 22;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 82);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 16);
			this.label1.TabIndex = 21;
			this.label1.Text = "Amp";
			// 
			// ampDecayTextBox
			// 
			this.ampDecayTextBox.Location = new System.Drawing.Point(21, 123);
			this.ampDecayTextBox.Name = "ampDecayTextBox";
			this.ampDecayTextBox.Size = new System.Drawing.Size(34, 20);
			this.ampDecayTextBox.TabIndex = 23;
			// 
			// ampSustainTextBox
			// 
			this.ampSustainTextBox.Location = new System.Drawing.Point(21, 147);
			this.ampSustainTextBox.Name = "ampSustainTextBox";
			this.ampSustainTextBox.Size = new System.Drawing.Size(34, 20);
			this.ampSustainTextBox.TabIndex = 24;
			// 
			// ampReleaseTextBox
			// 
			this.ampReleaseTextBox.Location = new System.Drawing.Point(22, 171);
			this.ampReleaseTextBox.Name = "ampReleaseTextBox";
			this.ampReleaseTextBox.Size = new System.Drawing.Size(34, 20);
			this.ampReleaseTextBox.TabIndex = 25;
			// 
			// durationMsTextBox
			// 
			this.durationMsTextBox.Location = new System.Drawing.Point(18, 55);
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
			// measureAmpDBtn
			// 
			this.measureAmpDBtn.Location = new System.Drawing.Point(81, 14);
			this.measureAmpDBtn.Name = "measureAmpDBtn";
			this.measureAmpDBtn.Size = new System.Drawing.Size(19, 24);
			this.measureAmpDBtn.TabIndex = 28;
			this.measureAmpDBtn.Text = "D";
			this.measureAmpDBtn.UseVisualStyleBackColor = true;
			this.measureAmpDBtn.Click += new System.EventHandler(this.MeasureAmpDBtnClick);
			// 
			// measureAmpABtn
			// 
			this.measureAmpABtn.Location = new System.Drawing.Point(56, 14);
			this.measureAmpABtn.Name = "measureAmpABtn";
			this.measureAmpABtn.Size = new System.Drawing.Size(19, 24);
			this.measureAmpABtn.TabIndex = 29;
			this.measureAmpABtn.Text = "A";
			this.measureAmpABtn.UseVisualStyleBackColor = true;
			this.measureAmpABtn.Click += new System.EventHandler(this.MeasureAmpABtnClick);
			// 
			// measureAmpRBtn
			// 
			this.measureAmpRBtn.Location = new System.Drawing.Point(106, 14);
			this.measureAmpRBtn.Name = "measureAmpRBtn";
			this.measureAmpRBtn.Size = new System.Drawing.Size(19, 24);
			this.measureAmpRBtn.TabIndex = 30;
			this.measureAmpRBtn.Text = "R";
			this.measureAmpRBtn.UseVisualStyleBackColor = true;
			this.measureAmpRBtn.Click += new System.EventHandler(this.MeasureAmpRBtnClick);
			// 
			// measureLFOBtn
			// 
			this.measureLFOBtn.Location = new System.Drawing.Point(149, 14);
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
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.measureModDBtn);
			this.groupBox1.Controls.Add(this.measureModABtn);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.measureAmpRBtn);
			this.groupBox1.Controls.Add(this.measureAmpABtn);
			this.groupBox1.Controls.Add(this.measureAmpDBtn);
			this.groupBox1.Controls.Add(this.measureLFOBtn);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(403, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(190, 79);
			this.groupBox1.TabIndex = 33;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sylenth Auto Measure";
			// 
			// measureModDBtn
			// 
			this.measureModDBtn.Location = new System.Drawing.Point(81, 44);
			this.measureModDBtn.Name = "measureModDBtn";
			this.measureModDBtn.Size = new System.Drawing.Size(19, 24);
			this.measureModDBtn.TabIndex = 35;
			this.measureModDBtn.Text = "D";
			this.measureModDBtn.UseVisualStyleBackColor = true;
			this.measureModDBtn.Click += new System.EventHandler(this.MeasureModDBtnClick);
			// 
			// measureModABtn
			// 
			this.measureModABtn.Location = new System.Drawing.Point(56, 44);
			this.measureModABtn.Name = "measureModABtn";
			this.measureModABtn.Size = new System.Drawing.Size(19, 24);
			this.measureModABtn.TabIndex = 34;
			this.measureModABtn.Text = "A";
			this.measureModABtn.UseVisualStyleBackColor = true;
			this.measureModABtn.Click += new System.EventHandler(this.MeasureModABtnClick);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 47);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 20);
			this.label5.TabIndex = 33;
			this.label5.Text = "ModEnv:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 20);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(58, 20);
			this.label4.TabIndex = 32;
			this.label4.Text = "AmpEnv:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.modAttackTextBox);
			this.groupBox2.Controls.Add(this.modDecayTextBox);
			this.groupBox2.Controls.Add(this.modReleaseTextBox);
			this.groupBox2.Controls.Add(this.modSustainTextBox);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.adsrSaveBtn);
			this.groupBox2.Controls.Add(this.ampAttackTextBox);
			this.groupBox2.Controls.Add(this.durationMsTextBox);
			this.groupBox2.Controls.Add(this.ampDecayTextBox);
			this.groupBox2.Controls.Add(this.durationSamplesTextBox);
			this.groupBox2.Controls.Add(this.ampReleaseTextBox);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.ampSustainTextBox);
			this.groupBox2.Location = new System.Drawing.Point(674, 112);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(96, 253);
			this.groupBox2.TabIndex = 34;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Measurment";
			// 
			// modAttackTextBox
			// 
			this.modAttackTextBox.Location = new System.Drawing.Point(59, 99);
			this.modAttackTextBox.Name = "modAttackTextBox";
			this.modAttackTextBox.Size = new System.Drawing.Size(34, 20);
			this.modAttackTextBox.TabIndex = 27;
			// 
			// modDecayTextBox
			// 
			this.modDecayTextBox.Location = new System.Drawing.Point(59, 123);
			this.modDecayTextBox.Name = "modDecayTextBox";
			this.modDecayTextBox.Size = new System.Drawing.Size(34, 20);
			this.modDecayTextBox.TabIndex = 28;
			// 
			// modReleaseTextBox
			// 
			this.modReleaseTextBox.Location = new System.Drawing.Point(60, 171);
			this.modReleaseTextBox.Name = "modReleaseTextBox";
			this.modReleaseTextBox.Size = new System.Drawing.Size(34, 20);
			this.modReleaseTextBox.TabIndex = 30;
			// 
			// modSustainTextBox
			// 
			this.modSustainTextBox.Location = new System.Drawing.Point(59, 147);
			this.modSustainTextBox.Name = "modSustainTextBox";
			this.modSustainTextBox.Size = new System.Drawing.Size(34, 20);
			this.modSustainTextBox.TabIndex = 29;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(50, 83);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(31, 16);
			this.label6.TabIndex = 31;
			this.label6.Text = "Mod";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(4, 102);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(17, 13);
			this.label7.TabIndex = 32;
			this.label7.Text = "A";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(4, 127);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(17, 13);
			this.label8.TabIndex = 33;
			this.label8.Text = "D";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(4, 150);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(17, 13);
			this.label9.TabIndex = 34;
			this.label9.Text = "S";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(4, 173);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(17, 13);
			this.label10.TabIndex = 35;
			this.label10.Text = "R";
			// 
			// WaveDisplayForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(772, 366);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.playMidiC5100msBtn);
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
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox modSustainTextBox;
		private System.Windows.Forms.TextBox modReleaseTextBox;
		private System.Windows.Forms.TextBox modDecayTextBox;
		private System.Windows.Forms.TextBox modAttackTextBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button measureModABtn;
		private System.Windows.Forms.Button measureModDBtn;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button measureLFOBtn;
		private System.Windows.Forms.Button measureAmpRBtn;
		private System.Windows.Forms.Button measureAmpABtn;
		private System.Windows.Forms.Button measureAmpDBtn;
		private System.Windows.Forms.Button playMidiC5100msBtn;
		private System.Windows.Forms.TextBox durationMsTextBox;
		private System.Windows.Forms.TextBox ampReleaseTextBox;
		private System.Windows.Forms.TextBox ampSustainTextBox;
		private System.Windows.Forms.TextBox ampDecayTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox ampAttackTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox durationSamplesTextBox;
		private System.Windows.Forms.Button adsrSaveBtn;
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
