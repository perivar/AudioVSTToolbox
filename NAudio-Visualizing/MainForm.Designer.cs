/*
 * Created by SharpDevelop.
 * User: perivar.nerseth
 * Date: 12.12.2012
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace NAudio_Visualizing
{
	partial class MainForm
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
			this.btnBrowse = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.txtFilePath = new System.Windows.Forms.TextBox();
			this.btnPlay = new System.Windows.Forms.Button();
			this.btnPause = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.txtTime = new System.Windows.Forms.TextBox();
			this.customWaveViewer1 = new CommonUtils.GUI.CustomWaveViewer();
			this.SuspendLayout();
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(282, 262);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 0;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.BtnBrowseClick);
			// 
			// txtFilePath
			// 
			this.txtFilePath.Location = new System.Drawing.Point(12, 264);
			this.txtFilePath.Name = "txtFilePath";
			this.txtFilePath.Size = new System.Drawing.Size(264, 20);
			this.txtFilePath.TabIndex = 1;
			// 
			// btnPlay
			// 
			this.btnPlay.Location = new System.Drawing.Point(363, 261);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(75, 23);
			this.btnPlay.TabIndex = 2;
			this.btnPlay.Text = "Play";
			this.btnPlay.UseVisualStyleBackColor = true;
			this.btnPlay.Click += new System.EventHandler(this.BtnPlayClick);
			// 
			// btnPause
			// 
			this.btnPause.Location = new System.Drawing.Point(444, 262);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(75, 23);
			this.btnPause.TabIndex = 3;
			this.btnPause.Text = "Pause";
			this.btnPause.UseVisualStyleBackColor = true;
			this.btnPause.Click += new System.EventHandler(this.BtnPauseClick);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(525, 261);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 4;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.BtnStopClick);
			// 
			// txtTime
			// 
			this.txtTime.Location = new System.Drawing.Point(12, 12);
			this.txtTime.Name = "txtTime";
			this.txtTime.Size = new System.Drawing.Size(100, 20);
			this.txtTime.TabIndex = 5;
			// 
			// customWaveViewer1
			// 
			this.customWaveViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.customWaveViewer1.AutoSize = true;
			this.customWaveViewer1.Location = new System.Drawing.Point(12, 38);
			this.customWaveViewer1.Name = "customWaveViewer1";
			this.customWaveViewer1.PenColor = System.Drawing.Color.DodgerBlue;
			this.customWaveViewer1.PenWidth = 1F;
			this.customWaveViewer1.Size = new System.Drawing.Size(757, 217);
			this.customWaveViewer1.TabIndex = 6;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(781, 296);
			this.Controls.Add(this.customWaveViewer1);
			this.Controls.Add(this.txtTime);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnPause);
			this.Controls.Add(this.btnPlay);
			this.Controls.Add(this.txtFilePath);
			this.Controls.Add(this.btnBrowse);
			this.Name = "MainForm";
			this.Text = "NAudio-Visualizing";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private CommonUtils.GUI.CustomWaveViewer customWaveViewer1;
		private System.Windows.Forms.TextBox txtTime;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnPause;
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.TextBox txtFilePath;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button btnBrowse;
	}
}
