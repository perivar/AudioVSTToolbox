namespace Wav2Zebra2Osc
{
	partial class Help
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Help));
			this.helpTextBox = new System.Windows.Forms.TextBox();
			this.Closebutton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// helpTextBox
			// 
			this.helpTextBox.BackColor = System.Drawing.SystemColors.Info;
			this.helpTextBox.Location = new System.Drawing.Point(9, 9);
			this.helpTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.helpTextBox.Multiline = true;
			this.helpTextBox.Name = "helpTextBox";
			this.helpTextBox.ReadOnly = true;
			this.helpTextBox.Size = new System.Drawing.Size(394, 316);
			this.helpTextBox.TabIndex = 1;
			this.helpTextBox.Text = resources.GetString("helpTextBox.Text");
			// 
			// Closebutton
			// 
			this.Closebutton.Location = new System.Drawing.Point(170, 329);
			this.Closebutton.Margin = new System.Windows.Forms.Padding(2);
			this.Closebutton.Name = "Closebutton";
			this.Closebutton.Size = new System.Drawing.Size(56, 30);
			this.Closebutton.TabIndex = 2;
			this.Closebutton.Text = "Close";
			this.Closebutton.UseVisualStyleBackColor = true;
			this.Closebutton.Click += new System.EventHandler(this.ClosebuttonClick);
			// 
			// Help
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(411, 362);
			this.Controls.Add(this.Closebutton);
			this.Controls.Add(this.helpTextBox);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "Help";
			this.Text = "Help";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.Button Closebutton;
		private System.Windows.Forms.TextBox helpTextBox;
		
		void ClosebuttonClick(object sender, System.EventArgs e)
		{
			this.Dispose();
		}
	}
}
