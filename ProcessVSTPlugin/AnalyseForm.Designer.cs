/*
 * Created by SharpDevelop.
 * User: perivar.nerseth
 * Date: 02.01.2012
 * Time: 00:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			this.frequencyAnalyserUserControl1 = new ProcessVSTPlugin.FrequencyAnalyserUserControl();
			this.SuspendLayout();
			// 
			// frequencyAnalyserUserControl1
			// 
			this.frequencyAnalyserUserControl1.FFTOverlap = 1;
			this.frequencyAnalyserUserControl1.FFTWindowsSize = 256;
			this.frequencyAnalyserUserControl1.Location = new System.Drawing.Point(12, 12);
			this.frequencyAnalyserUserControl1.Name = "frequencyAnalyserUserControl1";
			this.frequencyAnalyserUserControl1.SampleRate = 44100D;
			this.frequencyAnalyserUserControl1.Size = new System.Drawing.Size(713, 331);
			this.frequencyAnalyserUserControl1.TabIndex = 0;
			// 
			// AnalyseForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(737, 355);
			this.Controls.Add(this.frequencyAnalyserUserControl1);
			this.Name = "AnalyseForm";
			this.Text = "AnalyseForm";
			this.ResumeLayout(false);
		}
		private ProcessVSTPlugin.FrequencyAnalyserUserControl frequencyAnalyserUserControl1;
		
	}
}
