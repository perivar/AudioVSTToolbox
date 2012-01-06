using System;
using System.Drawing;

namespace Wav2Zebra2CSharp
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

	    private void WaveDisplayUserControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
	        Graphics g = e.Graphics;
	        
			int height = Height;
			int width = Width;
			
			// set black background
			g.Clear(Color.Black);

			// if selected, highlight
			if (this.Selected)
			{
				g.Clear(Color.Gray);
			}

			// TODO: what does this do?
			//g.DrawRectangle(Pens.White, 0, height / 2, width, height);

			// TODO: and this?
			//g.FillRectangle(Brushes.Black, 2, 2, width - 2, height - 2);

			if (this.parentForm.DoShowRAWWaves)
			{
				this.interpolatedData = Conversions.ReSampleToArbitrary(this.waveData, width);
			}			   
			else
			{
				this.interpolatedData = Conversions.ReSampleToArbitrary(this.dftData, width);	
			}			   
		
			for (int x = 0; x < width; x++)	
			{				 
				float heightScaling = height * (height - 4.0F) / height / 2.0F;
				int y = (int)(height / 2 + this.interpolatedData[x] * heightScaling);
				g.FillRectangle(Brushes.LightYellow, x, y, 1, 1);
			}
			
			Font f = new Font(FontFamily.GenericSansSerif, 9.0f, FontStyle.Regular);
			g.DrawString(System.IO.Path.GetFileName(this.fileName), f, Brushes.Yellow, 1, 1);
	    }		
		
		void WaveDisplayUserControlDoubleClick(object sender, System.EventArgs e)
		{
			this.parentForm.LoadCell();
		}
		
		void WaveDisplayUserControlClick(object sender, System.EventArgs e)
		{
			for (int i = 0; i < 16; i++)
			{
				if (this != this.parentForm.waveDisplays[i])
				{
					this.parentForm.waveDisplays[i].Selected = false;
					this.parentForm.waveDisplays[i].Refresh();
				}
			}
			this.Selected = true;
			this.Refresh();
		}
	}
}
