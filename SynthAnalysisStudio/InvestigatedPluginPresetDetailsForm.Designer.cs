
namespace SynthAnalysisStudio
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
			this.XMLDumpBtn = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.TrackPositionTextBox = new System.Windows.Forms.TextBox();
			this.NumberOfBytesTextBox = new System.Windows.Forms.TextBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.TrackPresetTextBox = new System.Windows.Forms.TextBox();
			this.ValueTextBox = new System.Windows.Forms.TextBox();
			this.LittleEndianCheckBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(0, 33);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.Size = new System.Drawing.Size(774, 282);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DataGridView1RowsAdded);
			this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);			
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
			this.CSVDumpBtn.Location = new System.Drawing.Point(552, 6);
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
			// XMLDumpBtn
			// 
			this.XMLDumpBtn.Location = new System.Drawing.Point(664, 6);
			this.XMLDumpBtn.Name = "XMLDumpBtn";
			this.XMLDumpBtn.Size = new System.Drawing.Size(106, 23);
			this.XMLDumpBtn.TabIndex = 5;
			this.XMLDumpBtn.Text = "Dump to XML file";
			this.XMLDumpBtn.UseVisualStyleBackColor = true;
			this.XMLDumpBtn.Click += new System.EventHandler(this.XMLDumpBtnClick);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(242, 326);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 18);
			this.label2.TabIndex = 8;
			this.label2.Text = "Num. Bytes:";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(353, 325);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 18);
			this.label4.TabIndex = 10;
			this.label4.Text = " Bytes:";
			// 
			// TrackPositionTextBox
			// 
			this.TrackPositionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TrackPositionTextBox.Location = new System.Drawing.Point(185, 324);
			this.TrackPositionTextBox.MaxLength = 8;
			this.TrackPositionTextBox.Name = "TrackPositionTextBox";
			this.TrackPositionTextBox.Size = new System.Drawing.Size(54, 20);
			this.TrackPositionTextBox.TabIndex = 11;
			this.TrackPositionTextBox.TextChanged += new System.EventHandler(this.TrackPositionTextBoxTextChanged);
			// 
			// NumberOfBytesTextBox
			// 
			this.NumberOfBytesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.NumberOfBytesTextBox.Location = new System.Drawing.Point(319, 323);
			this.NumberOfBytesTextBox.MaxLength = 2;
			this.NumberOfBytesTextBox.Name = "NumberOfBytesTextBox";
			this.NumberOfBytesTextBox.Size = new System.Drawing.Size(28, 20);
			this.NumberOfBytesTextBox.TabIndex = 12;
			this.NumberOfBytesTextBox.TextChanged += new System.EventHandler(this.NumberOfBytesTextBoxTextChanged);
			// 
			// checkBox1
			// 
			this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox1.Location = new System.Drawing.Point(12, 324);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(94, 18);
			this.checkBox1.TabIndex = 13;
			this.checkBox1.Text = "Track Chunk?";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(104, 326);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(78, 18);
			this.label3.TabIndex = 9;
			this.label3.Text = "Track Position:";
			// 
			// TrackPresetTextBox
			// 
			this.TrackPresetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TrackPresetTextBox.Location = new System.Drawing.Point(399, 321);
			this.TrackPresetTextBox.Name = "TrackPresetTextBox";
			this.TrackPresetTextBox.ReadOnly = true;
			this.TrackPresetTextBox.Size = new System.Drawing.Size(106, 20);
			this.TrackPresetTextBox.TabIndex = 14;
			// 
			// ValueTextBox
			// 
			this.ValueTextBox.Location = new System.Drawing.Point(511, 321);
			this.ValueTextBox.Name = "ValueTextBox";
			this.ValueTextBox.ReadOnly = true;
			this.ValueTextBox.Size = new System.Drawing.Size(169, 20);
			this.ValueTextBox.TabIndex = 15;
			// 
			// LittleEndianCheckBox
			// 
			this.LittleEndianCheckBox.Checked = true;
			this.LittleEndianCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.LittleEndianCheckBox.Location = new System.Drawing.Point(686, 321);
			this.LittleEndianCheckBox.Name = "LittleEndianCheckBox";
			this.LittleEndianCheckBox.Size = new System.Drawing.Size(84, 24);
			this.LittleEndianCheckBox.TabIndex = 16;
			this.LittleEndianCheckBox.Text = "Little Endian";
			this.LittleEndianCheckBox.UseVisualStyleBackColor = true;
			// 
			// InvestigatedPluginPresetDetailsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(774, 348);
			this.Controls.Add(this.LittleEndianCheckBox);
			this.Controls.Add(this.ValueTextBox);
			this.Controls.Add(this.TrackPresetTextBox);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.NumberOfBytesTextBox);
			this.Controls.Add(this.TrackPositionTextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.XMLDumpBtn);
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
		private System.Windows.Forms.CheckBox LittleEndianCheckBox;
		private System.Windows.Forms.TextBox ValueTextBox;
		private System.Windows.Forms.TextBox TrackPresetTextBox;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.TextBox NumberOfBytesTextBox;
		private System.Windows.Forms.TextBox TrackPositionTextBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button XMLDumpBtn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox FilterTextBox;
		private System.Windows.Forms.Button CSVDumpBtn;
		private System.Windows.Forms.Button UpdateBtn;
		private System.Windows.Forms.DataGridView dataGridView1;
	}
}
