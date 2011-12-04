
namespace ProcessVSTPlugin
{
	partial class InvestigatedPluginPresetDetailsForm
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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.UpdateBtn = new System.Windows.Forms.Button();
			this.CSVDumpBtn = new System.Windows.Forms.Button();
			this.FilterTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(12, 36);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.Size = new System.Drawing.Size(648, 294);
			this.dataGridView1.TabIndex = 0;
			// 
			// UpdateBtn
			// 
			this.UpdateBtn.Location = new System.Drawing.Point(12, 7);
			this.UpdateBtn.Name = "UpdateBtn";
			this.UpdateBtn.Size = new System.Drawing.Size(75, 23);
			this.UpdateBtn.TabIndex = 1;
			this.UpdateBtn.Text = "Update";
			this.UpdateBtn.UseVisualStyleBackColor = true;
			this.UpdateBtn.Click += new System.EventHandler(this.UpdateBtnClick);
			// 
			// CSVDumpBtn
			// 
			this.CSVDumpBtn.Location = new System.Drawing.Point(554, 7);
			this.CSVDumpBtn.Name = "CSVDumpBtn";
			this.CSVDumpBtn.Size = new System.Drawing.Size(106, 23);
			this.CSVDumpBtn.TabIndex = 2;
			this.CSVDumpBtn.Text = "Dump to CSV file";
			this.CSVDumpBtn.UseVisualStyleBackColor = true;
			this.CSVDumpBtn.Click += new System.EventHandler(this.CSVDumpBtnClick);
			// 
			// FilterTextBox
			// 
			this.FilterTextBox.Location = new System.Drawing.Point(231, 9);
			this.FilterTextBox.Name = "FilterTextBox";
			this.FilterTextBox.Size = new System.Drawing.Size(100, 20);
			this.FilterTextBox.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(106, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(119, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "Filter Parameter Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// InvestigatedPluginPresetDetailsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(672, 342);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.FilterTextBox);
			this.Controls.Add(this.CSVDumpBtn);
			this.Controls.Add(this.UpdateBtn);
			this.Controls.Add(this.dataGridView1);
			this.Name = "InvestigatedPluginPresetDetailsForm";
			this.Text = "Idenfied changes to  the preset file data chunks";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox FilterTextBox;
		private System.Windows.Forms.Button CSVDumpBtn;
		private System.Windows.Forms.Button UpdateBtn;
		private System.Windows.Forms.DataGridView dataGridView1;
	}
}
