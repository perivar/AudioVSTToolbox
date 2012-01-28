namespace ProcessVSTPlugin
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
			this.AmplitudeTrackBar = new System.Windows.Forms.TrackBar();
			this.StartPositionTrackBar = new System.Windows.Forms.TrackBar();
			this.CropBtn = new System.Windows.Forms.Button();
			this.MidiNoteCheckbox = new System.Windows.Forms.CheckBox();
			this.SaveWAVBtn = new System.Windows.Forms.Button();
			this.clearBtn = new System.Windows.Forms.Button();
			this.stopBtn = new System.Windows.Forms.Button();
			this.recordBtn = new System.Windows.Forms.Button();
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
			this.CropBtn.Location = new System.Drawing.Point(357, 8);
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
			this.SaveWAVBtn.Location = new System.Drawing.Point(414, 8);
			this.SaveWAVBtn.Name = "SaveWAVBtn";
			this.SaveWAVBtn.Size = new System.Drawing.Size(73, 23);
			this.SaveWAVBtn.TabIndex = 17;
			this.SaveWAVBtn.Text = "Save Wav";
			this.SaveWAVBtn.UseVisualStyleBackColor = true;
			this.SaveWAVBtn.Click += new System.EventHandler(this.SaveWAVBtnClick);
			// 
			// clearBtn
			// 
			this.clearBtn.Location = new System.Drawing.Point(300, 8);
			this.clearBtn.Name = "clearBtn";
			this.clearBtn.Size = new System.Drawing.Size(51, 23);
			this.clearBtn.TabIndex = 11;
			this.clearBtn.Text = "Clear";
			this.clearBtn.UseVisualStyleBackColor = true;
			this.clearBtn.Click += new System.EventHandler(this.ClearBtnClick);
			// 
			// stopBtn
			// 
			this.stopBtn.Location = new System.Drawing.Point(243, 8);
			this.stopBtn.Name = "stopBtn";
			this.stopBtn.Size = new System.Drawing.Size(51, 23);
			this.stopBtn.TabIndex = 10;
			this.stopBtn.Text = "Stop";
			this.stopBtn.UseVisualStyleBackColor = true;
			this.stopBtn.Click += new System.EventHandler(this.StopBtnClick);
			// 
			// recordBtn
			// 
			this.recordBtn.Location = new System.Drawing.Point(186, 8);
			this.recordBtn.Name = "recordBtn";
			this.recordBtn.Size = new System.Drawing.Size(51, 24);
			this.recordBtn.TabIndex = 9;
			this.recordBtn.Text = "Record";
			this.recordBtn.UseVisualStyleBackColor = true;
			this.recordBtn.Click += new System.EventHandler(this.RecordBtnClick);
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
			this.Text = "Waveform";
			((System.ComponentModel.ISupportInitialize)(this.MaxResolutionTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.AmplitudeTrackBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.StartPositionTrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Timer timer1;
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
