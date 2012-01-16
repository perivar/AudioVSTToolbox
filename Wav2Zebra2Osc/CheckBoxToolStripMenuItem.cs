using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wav2Zebra2Osc
{
	public class CheckBoxToolStripMenuItem : System.Windows.Forms.ToolStripControlHost
	{
		private System.Windows.Forms.FlowLayoutPanel controlPanel;
		private System.Windows.Forms.CheckBox chk = new System.Windows.Forms.CheckBox();
		
		public CheckBoxToolStripMenuItem()
			: base(new System.Windows.Forms.FlowLayoutPanel())
		{
			controlPanel = (System.Windows.Forms.FlowLayoutPanel) base.Control;
			controlPanel.BackColor = System.Drawing.Color.Transparent;
			chk.AutoSize = true;
			controlPanel.Controls.Add(chk);
		}
		
		public bool Checked
		{
			get { return chk.Checked; }
			set { chk.Checked = value; }
		}
		
		public new string Text
		{
			get { return chk.Text; }
			set { chk.Text = value; }
		}
		
		protected override void OnSubscribeControlEvents(System.Windows.Forms.Control control)
		{
			base.OnSubscribeControlEvents(control);
			chk.CheckedChanged += new System.EventHandler(CheckedChanged);
			chk.TextChanged += new System.EventHandler(TextChanged);
		}
		
		protected override void OnUnsubscribeControlEvents(System.Windows.Forms.Control control)
		{
			base.OnUnsubscribeControlEvents(control);
			chk.CheckStateChanged -= new System.EventHandler(CheckedChanged);
			chk.TextChanged -= new System.EventHandler(TextChanged);
		}
		
		private void CheckedChanged(object sender,  System.EventArgs e)
		{
			// Do stuff here if required
		}
		
		private new void TextChanged(object sender, System.EventArgs e)
		{
			// Do stuff here if required
		}
	}
}