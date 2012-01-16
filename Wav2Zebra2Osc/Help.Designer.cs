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
			this.helpTextBox.Location = new System.Drawing.Point(12, 11);
			this.helpTextBox.Multiline = true;
			this.helpTextBox.Name = "helpTextBox";
			this.helpTextBox.ReadOnly = true;
			this.helpTextBox.Size = new System.Drawing.Size(524, 388);
			this.helpTextBox.TabIndex = 1;
			this.helpTextBox.Text = resources.GetString("helpTextBox.Text");
			// 
			// Closebutton
			// 
			this.Closebutton.Location = new System.Drawing.Point(226, 405);
			this.Closebutton.Name = "Closebutton";
			this.Closebutton.Size = new System.Drawing.Size(75, 37);
			this.Closebutton.TabIndex = 2;
			this.Closebutton.Text = "Close";
			this.Closebutton.UseVisualStyleBackColor = true;
			this.Closebutton.Click += new System.EventHandler(this.ClosebuttonClick);
			// 
			// Help
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(548, 446);
			this.Controls.Add(this.Closebutton);
			this.Controls.Add(this.helpTextBox);
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
