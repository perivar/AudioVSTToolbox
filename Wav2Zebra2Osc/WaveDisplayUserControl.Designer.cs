using System;
using System.Drawing;

namespace Wav2Zebra2Osc
{
	partial class WaveDisplayUserControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
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
			this.SuspendLayout();
			// 
			// WaveDisplayUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.Name = "WaveDisplayUserControl";
			this.Click += new System.EventHandler(this.WaveDisplayUserControlClick);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.WaveDisplayUserControl_Paint);
			this.DoubleClick += new System.EventHandler(this.WaveDisplayUserControlDoubleClick);
			this.ResumeLayout(false);
		}		
	}
}
