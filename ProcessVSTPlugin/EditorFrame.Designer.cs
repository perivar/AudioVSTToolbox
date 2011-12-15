namespace ProcessVSTPlugin
{
    partial class EditorFrame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.LoadBtn = new System.Windows.Forms.Button();
        	this.SaveBtn = new System.Windows.Forms.Button();
        	this.presetComboBox = new System.Windows.Forms.ComboBox();
        	this.presetLabel = new System.Windows.Forms.Label();
        	this.pluginPanel = new System.Windows.Forms.Panel();
        	this.InvestigatePluginPresetFileCheckbox = new System.Windows.Forms.CheckBox();
        	this.PresetContentBtn = new System.Windows.Forms.Button();
        	this.TextDiffCheckbox = new System.Windows.Forms.CheckBox();
        	this.SuspendLayout();
        	// 
        	// LoadBtn
        	// 
        	this.LoadBtn.Location = new System.Drawing.Point(364, 4);
        	this.LoadBtn.Name = "LoadBtn";
        	this.LoadBtn.Size = new System.Drawing.Size(46, 23);
        	this.LoadBtn.TabIndex = 0;
        	this.LoadBtn.Text = "Load";
        	this.LoadBtn.UseVisualStyleBackColor = true;
        	this.LoadBtn.Click += new System.EventHandler(this.LoadBtnClick);
        	// 
        	// SaveBtn
        	// 
        	this.SaveBtn.Location = new System.Drawing.Point(413, 4);
        	this.SaveBtn.Name = "SaveBtn";
        	this.SaveBtn.Size = new System.Drawing.Size(46, 23);
        	this.SaveBtn.TabIndex = 1;
        	this.SaveBtn.Text = "Save";
        	this.SaveBtn.UseVisualStyleBackColor = true;
        	this.SaveBtn.Click += new System.EventHandler(this.SaveBtnClick);
        	// 
        	// presetComboBox
        	// 
        	this.presetComboBox.FormattingEnabled = true;
        	this.presetComboBox.Location = new System.Drawing.Point(42, 6);
        	this.presetComboBox.Name = "presetComboBox";
        	this.presetComboBox.Size = new System.Drawing.Size(316, 21);
        	this.presetComboBox.TabIndex = 2;
        	this.presetComboBox.SelectedValueChanged += new System.EventHandler(this.PresetComboBoxSelectedValueChanged);
        	// 
        	// presetLabel
        	// 
        	this.presetLabel.Location = new System.Drawing.Point(1, 9);
        	this.presetLabel.Name = "presetLabel";
        	this.presetLabel.Size = new System.Drawing.Size(43, 24);
        	this.presetLabel.TabIndex = 3;
        	this.presetLabel.Text = "Preset:";
        	// 
        	// pluginPanel
        	// 
        	this.pluginPanel.Location = new System.Drawing.Point(1, 36);
        	this.pluginPanel.Name = "pluginPanel";
        	this.pluginPanel.Size = new System.Drawing.Size(742, 231);
        	this.pluginPanel.TabIndex = 4;
        	// 
        	// InvestigatePluginPresetFileCheckbox
        	// 
        	this.InvestigatePluginPresetFileCheckbox.Location = new System.Drawing.Point(593, 4);
        	this.InvestigatePluginPresetFileCheckbox.Name = "InvestigatePluginPresetFileCheckbox";
        	this.InvestigatePluginPresetFileCheckbox.Size = new System.Drawing.Size(96, 24);
        	this.InvestigatePluginPresetFileCheckbox.TabIndex = 5;
        	this.InvestigatePluginPresetFileCheckbox.Text = "Track Preset";
        	this.InvestigatePluginPresetFileCheckbox.UseVisualStyleBackColor = true;
        	this.InvestigatePluginPresetFileCheckbox.CheckedChanged += new System.EventHandler(this.InvestigatePluginPresetFileCheckboxCheckedChanged);
        	// 
        	// PresetContentBtn
        	// 
        	this.PresetContentBtn.Location = new System.Drawing.Point(476, 4);
        	this.PresetContentBtn.Name = "PresetContentBtn";
        	this.PresetContentBtn.Size = new System.Drawing.Size(111, 23);
        	this.PresetContentBtn.TabIndex = 6;
        	this.PresetContentBtn.Text = "PresetFile Changes";
        	this.PresetContentBtn.UseVisualStyleBackColor = true;
        	this.PresetContentBtn.Click += new System.EventHandler(this.PresetContentBtnClick);
        	// 
        	// TextDiffCheckbox
        	// 
        	this.TextDiffCheckbox.Location = new System.Drawing.Point(680, 4);
        	this.TextDiffCheckbox.Name = "TextDiffCheckbox";
        	this.TextDiffCheckbox.Size = new System.Drawing.Size(63, 24);
        	this.TextDiffCheckbox.TabIndex = 7;
        	this.TextDiffCheckbox.Text = "TextDiff";
        	this.TextDiffCheckbox.UseVisualStyleBackColor = true;
        	this.TextDiffCheckbox.CheckedChanged += new System.EventHandler(this.TextDiffCheckboxCheckedChanged);
        	// 
        	// EditorFrame
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.AutoSize = true;
        	this.ClientSize = new System.Drawing.Size(745, 266);
        	this.Controls.Add(this.TextDiffCheckbox);
        	this.Controls.Add(this.PresetContentBtn);
        	this.Controls.Add(this.InvestigatePluginPresetFileCheckbox);
        	this.Controls.Add(this.pluginPanel);
        	this.Controls.Add(this.presetLabel);
        	this.Controls.Add(this.presetComboBox);
        	this.Controls.Add(this.SaveBtn);
        	this.Controls.Add(this.LoadBtn);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        	this.MaximizeBox = false;
        	this.Name = "EditorFrame";
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "EditorFrame";
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorFrameFormClosing);
        	this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditorFrameKeyDown);
        	this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EditorFrameKeyUp);
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.CheckBox TextDiffCheckbox;
        private System.Windows.Forms.CheckBox InvestigatePluginPresetFileCheckbox;
        private System.Windows.Forms.Button PresetContentBtn;
        private System.Windows.Forms.Panel pluginPanel;
        private System.Windows.Forms.Label presetLabel;
        private System.Windows.Forms.ComboBox presetComboBox;
        private System.Windows.Forms.Button LoadBtn;
        private System.Windows.Forms.Button SaveBtn;

        #endregion
        
    }
}