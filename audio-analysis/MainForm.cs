using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace audio_analysis_CSharp
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		
		void MainFormPaint(object sender, PaintEventArgs e)
		{
			System.Drawing.Graphics graphicsObj;

			graphicsObj = this.CreateGraphics();

			Pen myPen = new Pen(System.Drawing.Color.Red, 5);

			graphicsObj.DrawLine(myPen, 20, 20, 200, 210);
		}
	}
}
