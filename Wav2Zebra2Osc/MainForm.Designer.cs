using System.Windows.Forms;

namespace Wav2Zebra2CSharp
{
	partial class MainForm
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
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToZebra2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadCellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setExportPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.outputField = new System.Windows.Forms.ToolStripStatusLabel();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.exportDFTWavesCheckBox = new System.Windows.Forms.CheckBox();
			this.exportRAWWavesCheckBox = new System.Windows.Forms.CheckBox();
			this.radioButtonPanel = new System.Windows.Forms.Panel();
			this.showDFTRadioButton = new System.Windows.Forms.RadioButton();
			this.showRAWRadioButton = new System.Windows.Forms.RadioButton();
			this.exportFileName = new System.Windows.Forms.TextBox();
			this.labelFileName = new System.Windows.Forms.Label();
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.radioButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileToolStripMenuItem,
									this.preferencesToolStripMenuItem,
									this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(602, 28);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.exportToZebra2MenuItem,
									this.loadCellToolStripMenuItem,
									this.clearAllCellsToolStripMenuItem,
									this.toolStripSeparator1,
									this.quitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// exportToZebra2MenuItem
			// 
			this.exportToZebra2MenuItem.Name = "exportToZebra2MenuItem";
			this.exportToZebra2MenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.exportToZebra2MenuItem.Size = new System.Drawing.Size(240, 24);
			this.exportToZebra2MenuItem.Text = "Export to Zebra2";
			this.exportToZebra2MenuItem.Click += new System.EventHandler(this.ExportToZebra2MenuItemClick);
			// 
			// loadCellToolStripMenuItem
			// 
			this.loadCellToolStripMenuItem.Name = "loadCellToolStripMenuItem";
			this.loadCellToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.loadCellToolStripMenuItem.Size = new System.Drawing.Size(240, 24);
			this.loadCellToolStripMenuItem.Text = "Load cell(s)";
			this.loadCellToolStripMenuItem.Click += new System.EventHandler(this.LoadCellToolStripMenuItemClick);
			// 
			// clearAllCellsToolStripMenuItem
			// 
			this.clearAllCellsToolStripMenuItem.Name = "clearAllCellsToolStripMenuItem";
			this.clearAllCellsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.clearAllCellsToolStripMenuItem.Size = new System.Drawing.Size(240, 24);
			this.clearAllCellsToolStripMenuItem.Text = "Clear all cells";
			this.clearAllCellsToolStripMenuItem.Click += new System.EventHandler(this.ClearAllCellsToolStripMenuItemClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(237, 6);
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(240, 24);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItemClick);
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.setExportPathToolStripMenuItem});
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
			this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(97, 24);
			this.preferencesToolStripMenuItem.Text = "Preferences";
			// 
			// setExportPathToolStripMenuItem
			// 
			this.setExportPathToolStripMenuItem.Name = "setExportPathToolStripMenuItem";
			this.setExportPathToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
			this.setExportPathToolStripMenuItem.Text = "Set export path";
			this.setExportPathToolStripMenuItem.Click += new System.EventHandler(this.SetExportPathToolStripMenuItemClick);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.helpToolStripMenuItem1});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// helpToolStripMenuItem1
			// 
			this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
			this.helpToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.helpToolStripMenuItem1.Size = new System.Drawing.Size(152, 24);
			this.helpToolStripMenuItem1.Text = "Help";
			this.helpToolStripMenuItem1.Click += new System.EventHandler(this.HelpToolStripMenuItem1Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.outputField});
			this.statusStrip.Location = new System.Drawing.Point(0, 550);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(602, 25);
			this.statusStrip.TabIndex = 1;
			this.statusStrip.Text = "statusStrip1";
			// 
			// outputField
			// 
			this.outputField.Name = "outputField";
			this.outputField.Size = new System.Drawing.Size(103, 20);
			this.outputField.Text = "Export path is:";
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.tableLayoutPanel.ColumnCount = 4;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 68);
			this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel.MinimumSize = new System.Drawing.Size(600, 480);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 4;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(600, 480);
			this.tableLayoutPanel.TabIndex = 3;
			// 
			// exportDFTWavesCheckBox
			// 
			this.exportDFTWavesCheckBox.Checked = true;
			this.exportDFTWavesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.exportDFTWavesCheckBox.Location = new System.Drawing.Point(306, 35);
			this.exportDFTWavesCheckBox.Name = "exportDFTWavesCheckBox";
			this.exportDFTWavesCheckBox.Size = new System.Drawing.Size(153, 24);
			this.exportDFTWavesCheckBox.TabIndex = 5;
			this.exportDFTWavesCheckBox.Text = "Export DFT waves";
			this.exportDFTWavesCheckBox.UseVisualStyleBackColor = true;
			// 
			// exportRAWWavesCheckBox
			// 
			this.exportRAWWavesCheckBox.Location = new System.Drawing.Point(452, 35);
			this.exportRAWWavesCheckBox.Name = "exportRAWWavesCheckBox";
			this.exportRAWWavesCheckBox.Size = new System.Drawing.Size(150, 24);
			this.exportRAWWavesCheckBox.TabIndex = 6;
			this.exportRAWWavesCheckBox.Text = "Export RAW waves";
			this.exportRAWWavesCheckBox.UseVisualStyleBackColor = true;
			// 
			// radioButtonPanel
			// 
			this.radioButtonPanel.Controls.Add(this.showDFTRadioButton);
			this.radioButtonPanel.Controls.Add(this.showRAWRadioButton);
			this.radioButtonPanel.Location = new System.Drawing.Point(295, 0);
			this.radioButtonPanel.Name = "radioButtonPanel";
			this.radioButtonPanel.Size = new System.Drawing.Size(305, 29);
			this.radioButtonPanel.TabIndex = 7;
			// 
			// showDFTRadioButton
			// 
			this.showDFTRadioButton.Checked = true;
			this.showDFTRadioButton.Location = new System.Drawing.Point(154, 2);
			this.showDFTRadioButton.Name = "showDFTRadioButton";
			this.showDFTRadioButton.Size = new System.Drawing.Size(140, 24);
			this.showDFTRadioButton.TabIndex = 1;
			this.showDFTRadioButton.TabStop = true;
			this.showDFTRadioButton.Text = "Show DFT waves";
			this.showDFTRadioButton.UseVisualStyleBackColor = true;
			this.showDFTRadioButton.CheckedChanged += new System.EventHandler(this.ShowDFTRadioButtonCheckedChanged);
			// 
			// showRAWRadioButton
			// 
			this.showRAWRadioButton.Location = new System.Drawing.Point(3, 3);
			this.showRAWRadioButton.Name = "showRAWRadioButton";
			this.showRAWRadioButton.Size = new System.Drawing.Size(145, 24);
			this.showRAWRadioButton.TabIndex = 0;
			this.showRAWRadioButton.Text = "Show RAW waves";
			this.showRAWRadioButton.UseVisualStyleBackColor = true;
			// 
			// exportFileName
			// 
			this.exportFileName.Location = new System.Drawing.Point(97, 35);
			this.exportFileName.Name = "exportFileName";
			this.exportFileName.Size = new System.Drawing.Size(203, 22);
			this.exportFileName.TabIndex = 9;
			// 
			// labelFileName
			// 
			this.labelFileName.Location = new System.Drawing.Point(4, 36);
			this.labelFileName.Name = "labelFileName";
			this.labelFileName.Size = new System.Drawing.Size(96, 23);
			this.labelFileName.TabIndex = 10;
			this.labelFileName.Text = "Export file as:";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 575);
			this.Controls.Add(this.labelFileName);
			this.Controls.Add(this.exportFileName);
			this.Controls.Add(this.radioButtonPanel);
			this.Controls.Add(this.exportRAWWavesCheckBox);
			this.Controls.Add(this.exportDFTWavesCheckBox);
			this.Controls.Add(this.tableLayoutPanel);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.MinimumSize = new System.Drawing.Size(620, 620);
			this.Name = "MainForm";
			this.Text = "Wav2Zebra2CSharp";
			this.Resize += new System.EventHandler(this.MainFormResize);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.radioButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TextBox exportFileName;
		private System.Windows.Forms.Label labelFileName;
		private System.Windows.Forms.Panel radioButtonPanel;
		private System.Windows.Forms.RadioButton showRAWRadioButton;
		private System.Windows.Forms.RadioButton showDFTRadioButton;
		private System.Windows.Forms.CheckBox exportRAWWavesCheckBox;
		private System.Windows.Forms.CheckBox exportDFTWavesCheckBox;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel outputField;
		private System.Windows.Forms.ToolStripMenuItem setExportPathToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem exportToZebra2MenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem clearAllCellsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadCellToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		
		void ExportToZebra2MenuItemClick(object sender, System.EventArgs e)
		{
			ExportToZebra2(DoExportDFTWaves, DoExportRAWWaves);
		}
		
		void ShowDFTRadioButtonCheckedChanged(object sender, System.EventArgs e)
		{
			for (int i = 0; i < 16; i++)
			{
				this.waveDisplays[i].Refresh();				
			}				 
			
			if (DoShowRAWWaves)
			{
				this.outputField.Text = "Raw View";
			}				 
			else
			{
				this.outputField.Text = "Dft View";
			}			
		}
		
		void QuitToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			Application.Exit();
		}
		
		void SetExportPathToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			SetExportPath();	
		}
		
		void ClearAllCellsToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			ClearAllCells();	
		}
		
		void LoadCellToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			LoadCell();
		}
		
		void MainFormResize(object sender, System.EventArgs e)
		{
			if (this.WindowState == FormWindowState.Maximized)
			{
			    // Do your stuff
			}
			if (this.waveDisplays != null && this.waveDisplays[0] != null) {
				int margins = this.waveDisplays[0].Margin.All;
				int newCellWidth = this.tableLayoutPanel.Width / 4 - (margins * 2);
				int newCellHeight = this.tableLayoutPanel.Height / 4 - (margins * 2);
				
				for (int i = 0; i < 16; i++)
				{
					this.waveDisplays[i].Size = new System.Drawing.Size(newCellWidth, newCellHeight);			
					this.waveDisplays[i].ResumeLayout(false);
					this.waveDisplays[i].PerformLayout();					
				}
			}
		}
	}
}
