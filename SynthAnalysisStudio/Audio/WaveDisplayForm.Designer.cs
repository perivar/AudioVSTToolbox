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
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.waveDisplayUserControl1 = new CommonUtils.GUI.WaveDisplayUserControl();
			this.label2 = new System.Windows.Forms.Label();
			this.OnOffCheckbox = new System.Windows.Forms.CheckBox();
			this.recordBtn = new System.Windows.Forms.Button();
			this.stopBtn = new System.Windows.Forms.Button();
			this.clearBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar1
			// 
			this.trackBar1.LargeChange = 50;
			this.trackBar1.Location = new System.Drawing.Point(653, 64);
			this.trackBar1.Maximum = 1000;
			this.trackBar1.Minimum = 1;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.trackBar1.Size = new System.Drawing.Size(70, 45);
			this.trackBar1.TabIndex = 5;
			this.trackBar1.TickFrequency = 100;
			this.trackBar1.Value = 1;
			this.trackBar1.Scroll += new System.EventHandler(this.TrackBar1Scroll);
			// 
			// waveDisplayUserControl1
			// 
			this.waveDisplayUserControl1.Location = new System.Drawing.Point(0, 0);
			this.waveDisplayUserControl1.Name = "waveDisplayUserControl1";
			this.waveDisplayUserControl1.Resolution = 1;
			this.waveDisplayUserControl1.Size = new System.Drawing.Size(647, 261);
			this.waveDisplayUserControl1.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(663, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Resolution";
			// 
			// OnOffCheckbox
			// 
			this.OnOffCheckbox.Checked = true;
			this.OnOffCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.OnOffCheckbox.Location = new System.Drawing.Point(663, 7);
			this.OnOffCheckbox.Name = "OnOffCheckbox";
			this.OnOffCheckbox.Size = new System.Drawing.Size(66, 24);
			this.OnOffCheckbox.TabIndex = 8;
			this.OnOffCheckbox.Text = "Turn On";
			this.OnOffCheckbox.UseVisualStyleBackColor = true;
			this.OnOffCheckbox.CheckedChanged += new System.EventHandler(this.OnOffCheckboxCheckedChanged);
			// 
			// recordBtn
			// 
			this.recordBtn.Location = new System.Drawing.Point(663, 105);
			this.recordBtn.Name = "recordBtn";
			this.recordBtn.Size = new System.Drawing.Size(60, 23);
			this.recordBtn.TabIndex = 9;
			this.recordBtn.Text = "Record";
			this.recordBtn.UseVisualStyleBackColor = true;
			this.recordBtn.Click += new System.EventHandler(this.RecordBtnClick);
			// 
			// stopBtn
			// 
			this.stopBtn.Location = new System.Drawing.Point(663, 134);
			this.stopBtn.Name = "stopBtn";
			this.stopBtn.Size = new System.Drawing.Size(60, 23);
			this.stopBtn.TabIndex = 10;
			this.stopBtn.Text = "Stop";
			this.stopBtn.UseVisualStyleBackColor = true;
			this.stopBtn.Click += new System.EventHandler(this.StopBtnClick);
			// 
			// clearBtn
			// 
			this.clearBtn.Location = new System.Drawing.Point(663, 163);
			this.clearBtn.Name = "clearBtn";
			this.clearBtn.Size = new System.Drawing.Size(60, 23);
			this.clearBtn.TabIndex = 11;
			this.clearBtn.Text = "Clear";
			this.clearBtn.UseVisualStyleBackColor = true;
			this.clearBtn.Click += new System.EventHandler(this.ClearBtnClick);
			// 
			// WaveDisplayForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(735, 262);
			this.Controls.Add(this.clearBtn);
			this.Controls.Add(this.stopBtn);
			this.Controls.Add(this.recordBtn);
			this.Controls.Add(this.OnOffCheckbox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.waveDisplayUserControl1);
			this.Controls.Add(this.trackBar1);
			this.Name = "WaveDisplayForm";
			this.Text = "WaveDisplayForm";
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Button clearBtn;
		private System.Windows.Forms.Button stopBtn;
		private System.Windows.Forms.Button recordBtn;
		private System.Windows.Forms.CheckBox OnOffCheckbox;
		private System.Windows.Forms.Label label2;
		private CommonUtils.GUI.WaveDisplayUserControl waveDisplayUserControl1;
		private System.Windows.Forms.TrackBar trackBar1;
	}
}
