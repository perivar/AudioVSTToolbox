using System.Windows.Forms;

namespace Wav2Zebra2Osc
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
			this.components = new System.ComponentModel.Container();
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
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.convertMassiveOscsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.outputField = new System.Windows.Forms.ToolStripStatusLabel();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sawRisingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sawFallingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.triangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.squareHighLowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pulseHighLowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pulseHighLowIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.triangleSawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportMorphedWavesCheckBox = new System.Windows.Forms.CheckBox();
			this.exportRAWWavesCheckBox = new System.Windows.Forms.CheckBox();
			this.radioButtonPanel = new System.Windows.Forms.Panel();
			this.showMorphedRadioButton = new System.Windows.Forms.RadioButton();
			this.showRAWRadioButton = new System.Windows.Forms.RadioButton();
			this.exportFileName = new System.Windows.Forms.TextBox();
			this.labelFileName = new System.Windows.Forms.Label();
			this.btnPlay = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.radioButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.preferencesToolStripMenuItem,
			this.toolsToolStripMenuItem,
			this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
			this.menuStrip.Size = new System.Drawing.Size(481, 24);
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
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// exportToZebra2MenuItem
			// 
			this.exportToZebra2MenuItem.Name = "exportToZebra2MenuItem";
			this.exportToZebra2MenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.exportToZebra2MenuItem.Size = new System.Drawing.Size(200, 22);
			this.exportToZebra2MenuItem.Text = "Export to Zebra2";
			this.exportToZebra2MenuItem.Click += new System.EventHandler(this.ExportToZebra2MenuItemClick);
			// 
			// loadCellToolStripMenuItem
			// 
			this.loadCellToolStripMenuItem.Name = "loadCellToolStripMenuItem";
			this.loadCellToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.loadCellToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.loadCellToolStripMenuItem.Text = "Load cell(s)";
			this.loadCellToolStripMenuItem.Click += new System.EventHandler(this.LoadCellToolStripMenuItemClick);
			// 
			// clearAllCellsToolStripMenuItem
			// 
			this.clearAllCellsToolStripMenuItem.Name = "clearAllCellsToolStripMenuItem";
			this.clearAllCellsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.clearAllCellsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.clearAllCellsToolStripMenuItem.Text = "Clear all cells";
			this.clearAllCellsToolStripMenuItem.Click += new System.EventHandler(this.ClearAllCellsToolStripMenuItemClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItemClick);
			// 
			// preferencesToolStripMenuItem
			// 
			this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.setExportPathToolStripMenuItem});
			this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
			this.preferencesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
			this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
			this.preferencesToolStripMenuItem.Text = "Preferences";
			// 
			// setExportPathToolStripMenuItem
			// 
			this.setExportPathToolStripMenuItem.Name = "setExportPathToolStripMenuItem";
			this.setExportPathToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
			this.setExportPathToolStripMenuItem.Text = "Set export path";
			this.setExportPathToolStripMenuItem.Click += new System.EventHandler(this.SetExportPathToolStripMenuItemClick);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.helpToolStripMenuItem1});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// helpToolStripMenuItem1
			// 
			this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
			this.helpToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.helpToolStripMenuItem1.Size = new System.Drawing.Size(118, 22);
			this.helpToolStripMenuItem1.Text = "Help";
			this.helpToolStripMenuItem1.Click += new System.EventHandler(this.HelpToolStripMenuItem1Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.convertMassiveOscsToolStripMenuItem});
			this.toolsToolStripMenuItem.Enabled = false;
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// convertMassiveOscsToolStripMenuItem
			// 
			this.convertMassiveOscsToolStripMenuItem.Name = "convertMassiveOscsToolStripMenuItem";
			this.convertMassiveOscsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.convertMassiveOscsToolStripMenuItem.Text = "Convert Massive Oscs";
			this.convertMassiveOscsToolStripMenuItem.Click += new System.EventHandler(this.ConvertMassiveOscsToolStripMenuItemClick);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.outputField});
			this.statusStrip.Location = new System.Drawing.Point(0, 450);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
			this.statusStrip.Size = new System.Drawing.Size(481, 22);
			this.statusStrip.TabIndex = 1;
			this.statusStrip.Text = "statusStrip1";
			// 
			// outputField
			// 
			this.outputField.Name = "outputField";
			this.outputField.Size = new System.Drawing.Size(81, 17);
			this.outputField.Text = "Export path is:";
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
			this.tableLayoutPanel.ColumnCount = 4;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.ContextMenuStrip = this.contextMenuStrip1;
			this.tableLayoutPanel.Location = new System.Drawing.Point(2, 74);
			this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel.MinimumSize = new System.Drawing.Size(472, 372);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 4;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(472, 372);
			this.tableLayoutPanel.TabIndex = 3;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.sineToolStripMenuItem,
			this.sawRisingToolStripMenuItem,
			this.sawFallingToolStripMenuItem,
			this.triangleToolStripMenuItem,
			this.squareHighLowToolStripMenuItem,
			this.pulseHighLowToolStripMenuItem,
			this.pulseHighLowIIToolStripMenuItem,
			this.triangleSawToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(161, 180);
			this.contextMenuStrip1.Text = "Square high low";
			// 
			// sineToolStripMenuItem
			// 
			this.sineToolStripMenuItem.Name = "sineToolStripMenuItem";
			this.sineToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.sineToolStripMenuItem.Text = "Sine";
			this.sineToolStripMenuItem.Click += new System.EventHandler(this.SineToolStripMenuItemClick);
			// 
			// sawRisingToolStripMenuItem
			// 
			this.sawRisingToolStripMenuItem.Name = "sawRisingToolStripMenuItem";
			this.sawRisingToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.sawRisingToolStripMenuItem.Text = "Saw Rising";
			this.sawRisingToolStripMenuItem.Click += new System.EventHandler(this.SawRisingToolStripMenuItemClick);
			// 
			// sawFallingToolStripMenuItem
			// 
			this.sawFallingToolStripMenuItem.Name = "sawFallingToolStripMenuItem";
			this.sawFallingToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.sawFallingToolStripMenuItem.Text = "Saw Falling";
			this.sawFallingToolStripMenuItem.Click += new System.EventHandler(this.SawFallingToolStripMenuItemClick);
			// 
			// triangleToolStripMenuItem
			// 
			this.triangleToolStripMenuItem.Name = "triangleToolStripMenuItem";
			this.triangleToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.triangleToolStripMenuItem.Text = "Triangle";
			this.triangleToolStripMenuItem.Click += new System.EventHandler(this.TriangleToolStripMenuItemClick);
			// 
			// squareHighLowToolStripMenuItem
			// 
			this.squareHighLowToolStripMenuItem.Name = "squareHighLowToolStripMenuItem";
			this.squareHighLowToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.squareHighLowToolStripMenuItem.Text = "Square high low";
			this.squareHighLowToolStripMenuItem.Click += new System.EventHandler(this.SquareHighLowToolStripMenuItemClick);
			// 
			// pulseHighLowToolStripMenuItem
			// 
			this.pulseHighLowToolStripMenuItem.Name = "pulseHighLowToolStripMenuItem";
			this.pulseHighLowToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.pulseHighLowToolStripMenuItem.Text = "Pulse high low I";
			this.pulseHighLowToolStripMenuItem.Click += new System.EventHandler(this.PulseHighLowToolStripMenuItemClick);
			// 
			// pulseHighLowIIToolStripMenuItem
			// 
			this.pulseHighLowIIToolStripMenuItem.Name = "pulseHighLowIIToolStripMenuItem";
			this.pulseHighLowIIToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.pulseHighLowIIToolStripMenuItem.Text = "Pulse high low II";
			this.pulseHighLowIIToolStripMenuItem.Click += new System.EventHandler(this.PulseHighLowIIToolStripMenuItemClick);
			// 
			// triangleSawToolStripMenuItem
			// 
			this.triangleSawToolStripMenuItem.Name = "triangleSawToolStripMenuItem";
			this.triangleSawToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
			this.triangleSawToolStripMenuItem.Text = "Triangle Saw";
			this.triangleSawToolStripMenuItem.Click += new System.EventHandler(this.TriangleSawToolStripMenuItemClick);
			// 
			// exportMorphedWavesCheckBox
			// 
			this.exportMorphedWavesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.exportMorphedWavesCheckBox.Location = new System.Drawing.Point(253, 29);
			this.exportMorphedWavesCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.exportMorphedWavesCheckBox.Name = "exportMorphedWavesCheckBox";
			this.exportMorphedWavesCheckBox.Size = new System.Drawing.Size(115, 20);
			this.exportMorphedWavesCheckBox.TabIndex = 5;
			this.exportMorphedWavesCheckBox.Text = "Export Morphed";
			this.exportMorphedWavesCheckBox.UseVisualStyleBackColor = true;
			// 
			// exportRAWWavesCheckBox
			// 
			this.exportRAWWavesCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.exportRAWWavesCheckBox.Checked = true;
			this.exportRAWWavesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.exportRAWWavesCheckBox.Location = new System.Drawing.Point(362, 29);
			this.exportRAWWavesCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.exportRAWWavesCheckBox.Name = "exportRAWWavesCheckBox";
			this.exportRAWWavesCheckBox.Size = new System.Drawing.Size(112, 20);
			this.exportRAWWavesCheckBox.TabIndex = 6;
			this.exportRAWWavesCheckBox.Text = "Export Raw";
			this.exportRAWWavesCheckBox.UseVisualStyleBackColor = true;
			// 
			// radioButtonPanel
			// 
			this.radioButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioButtonPanel.Controls.Add(this.showMorphedRadioButton);
			this.radioButtonPanel.Controls.Add(this.showRAWRadioButton);
			this.radioButtonPanel.Location = new System.Drawing.Point(251, 0);
			this.radioButtonPanel.Margin = new System.Windows.Forms.Padding(2);
			this.radioButtonPanel.Name = "radioButtonPanel";
			this.radioButtonPanel.Size = new System.Drawing.Size(229, 24);
			this.radioButtonPanel.TabIndex = 7;
			// 
			// showMorphedRadioButton
			// 
			this.showMorphedRadioButton.Location = new System.Drawing.Point(116, 2);
			this.showMorphedRadioButton.Margin = new System.Windows.Forms.Padding(2);
			this.showMorphedRadioButton.Name = "showMorphedRadioButton";
			this.showMorphedRadioButton.Size = new System.Drawing.Size(105, 20);
			this.showMorphedRadioButton.TabIndex = 1;
			this.showMorphedRadioButton.Text = "Show Morphed";
			this.showMorphedRadioButton.UseVisualStyleBackColor = true;
			this.showMorphedRadioButton.CheckedChanged += new System.EventHandler(this.ShowMorphedRadioButtonCheckedChanged);
			// 
			// showRAWRadioButton
			// 
			this.showRAWRadioButton.Checked = true;
			this.showRAWRadioButton.Location = new System.Drawing.Point(2, 2);
			this.showRAWRadioButton.Margin = new System.Windows.Forms.Padding(2);
			this.showRAWRadioButton.Name = "showRAWRadioButton";
			this.showRAWRadioButton.Size = new System.Drawing.Size(109, 20);
			this.showRAWRadioButton.TabIndex = 0;
			this.showRAWRadioButton.TabStop = true;
			this.showRAWRadioButton.Text = "Show Raw";
			this.showRAWRadioButton.UseVisualStyleBackColor = true;
			// 
			// exportFileName
			// 
			this.exportFileName.Location = new System.Drawing.Point(79, 28);
			this.exportFileName.Margin = new System.Windows.Forms.Padding(2);
			this.exportFileName.Name = "exportFileName";
			this.exportFileName.Size = new System.Drawing.Size(170, 20);
			this.exportFileName.TabIndex = 9;
			// 
			// labelFileName
			// 
			this.labelFileName.Location = new System.Drawing.Point(3, 29);
			this.labelFileName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelFileName.Name = "labelFileName";
			this.labelFileName.Size = new System.Drawing.Size(72, 19);
			this.labelFileName.TabIndex = 10;
			this.labelFileName.Text = "Export file as:";
			// 
			// btnPlay
			// 
			this.btnPlay.Location = new System.Drawing.Point(6, 51);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(45, 20);
			this.btnPlay.TabIndex = 11;
			this.btnPlay.TabStop = false;
			this.btnPlay.Text = "Play";
			this.btnPlay.UseVisualStyleBackColor = true;
			this.btnPlay.Click += new System.EventHandler(this.BtnPlayClick);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(57, 51);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(45, 20);
			this.btnStop.TabIndex = 12;
			this.btnStop.TabStop = false;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.BtnStopClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(481, 472);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnPlay);
			this.Controls.Add(this.labelFileName);
			this.Controls.Add(this.exportFileName);
			this.Controls.Add(this.radioButtonPanel);
			this.Controls.Add(this.exportRAWWavesCheckBox);
			this.Controls.Add(this.exportMorphedWavesCheckBox);
			this.Controls.Add(this.tableLayoutPanel);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MinimumSize = new System.Drawing.Size(497, 511);
			this.Name = "MainForm";
			this.Text = "Wav2Zebra2Osc";
			this.Resize += new System.EventHandler(this.MainFormResize);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.radioButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.TextBox exportFileName;
		private System.Windows.Forms.Label labelFileName;
		private System.Windows.Forms.Panel radioButtonPanel;
		private System.Windows.Forms.RadioButton showRAWRadioButton;
		private System.Windows.Forms.RadioButton showMorphedRadioButton;
		private System.Windows.Forms.CheckBox exportRAWWavesCheckBox;
		private System.Windows.Forms.CheckBox exportMorphedWavesCheckBox;
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
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem sineToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sawRisingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sawFallingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem triangleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem squareHighLowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pulseHighLowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pulseHighLowIIToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem triangleSawToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem convertMassiveOscsToolStripMenuItem;
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Button btnStop;
	}
}
