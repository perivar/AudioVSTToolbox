/*
 * Created by SharpDevelop.
 * User: periv
 * Date: 04.11.2017
 * Time: 15:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace DumpPreset
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.RichTextBox txtContent;
		private System.Windows.Forms.Label lblDragPreset;
		
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
			this.lblDragPreset = new System.Windows.Forms.Label();
			this.txtContent = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// lblDragPreset
			// 
			this.lblDragPreset.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDragPreset.Location = new System.Drawing.Point(12, 9);
			this.lblDragPreset.Name = "lblDragPreset";
			this.lblDragPreset.Size = new System.Drawing.Size(619, 38);
			this.lblDragPreset.TabIndex = 1;
			this.lblDragPreset.Text = "Drag Preset Here!";
			// 
			// txtContent
			// 
			this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.txtContent.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtContent.Location = new System.Drawing.Point(12, 69);
			this.txtContent.Name = "txtContent";
			this.txtContent.Size = new System.Drawing.Size(900, 322);
			this.txtContent.TabIndex = 2;
			this.txtContent.Text = "";
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(924, 403);
			this.Controls.Add(this.txtContent);
			this.Controls.Add(this.lblDragPreset);
			this.Name = "MainForm";
			this.Text = "DumpPreset";
			this.ResumeLayout(false);

		}
	}
}
