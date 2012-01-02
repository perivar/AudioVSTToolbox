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
        	this.MidiNoteCheckbox = new System.Windows.Forms.CheckBox();
        	this.AnalyseBtn = new System.Windows.Forms.Button();
        	this.SuspendLayout();
        	// 
        	// LoadBtn
        	// 
        	this.LoadBtn.Location = new System.Drawing.Point(331, 6);
        	this.LoadBtn.Name = "LoadBtn";
        	this.LoadBtn.Size = new System.Drawing.Size(46, 23);
        	this.LoadBtn.TabIndex = 0;
        	this.LoadBtn.Text = "Load";
        	this.LoadBtn.UseVisualStyleBackColor = true;
        	this.LoadBtn.Click += new System.EventHandler(this.LoadBtnClick);
        	// 
        	// SaveBtn
        	// 
        	this.SaveBtn.Location = new System.Drawing.Point(380, 6);
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
        	this.presetComboBox.Size = new System.Drawing.Size(283, 21);
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
        	this.pluginPanel.Size = new System.Drawing.Size(799, 231);
        	this.pluginPanel.TabIndex = 4;
        	// 
        	// InvestigatePluginPresetFileCheckbox
        	// 
        	this.InvestigatePluginPresetFileCheckbox.Location = new System.Drawing.Point(542, 6);
        	this.InvestigatePluginPresetFileCheckbox.Name = "InvestigatePluginPresetFileCheckbox";
        	this.InvestigatePluginPresetFileCheckbox.Size = new System.Drawing.Size(87, 24);
        	this.InvestigatePluginPresetFileCheckbox.TabIndex = 5;
        	this.InvestigatePluginPresetFileCheckbox.Text = "TrackChunk";
        	this.InvestigatePluginPresetFileCheckbox.UseVisualStyleBackColor = true;
        	this.InvestigatePluginPresetFileCheckbox.CheckedChanged += new System.EventHandler(this.InvestigatePluginPresetFileCheckboxCheckedChanged);
        	// 
        	// PresetContentBtn
        	// 
        	this.PresetContentBtn.Location = new System.Drawing.Point(445, 6);
        	this.PresetContentBtn.Name = "PresetContentBtn";
        	this.PresetContentBtn.Size = new System.Drawing.Size(93, 23);
        	this.PresetContentBtn.TabIndex = 6;
        	this.PresetContentBtn.Text = "Chunk Changes";
        	this.PresetContentBtn.UseVisualStyleBackColor = true;
        	this.PresetContentBtn.Click += new System.EventHandler(this.PresetContentBtnClick);
        	// 
        	// TextDiffCheckbox
        	// 
        	this.TextDiffCheckbox.Location = new System.Drawing.Point(622, 6);
        	this.TextDiffCheckbox.Name = "TextDiffCheckbox";
        	this.TextDiffCheckbox.Size = new System.Drawing.Size(63, 24);
        	this.TextDiffCheckbox.TabIndex = 7;
        	this.TextDiffCheckbox.Text = "TextDiff";
        	this.TextDiffCheckbox.UseVisualStyleBackColor = true;
        	this.TextDiffCheckbox.CheckedChanged += new System.EventHandler(this.TextDiffCheckboxCheckedChanged);
        	// 
        	// MidiNoteCheckbox
        	// 
        	this.MidiNoteCheckbox.Location = new System.Drawing.Point(682, 6);
        	this.MidiNoteCheckbox.Name = "MidiNoteCheckbox";
        	this.MidiNoteCheckbox.Size = new System.Drawing.Size(76, 24);
        	this.MidiNoteCheckbox.TabIndex = 8;
        	this.MidiNoteCheckbox.Text = "Midi Note";
        	this.MidiNoteCheckbox.UseVisualStyleBackColor = true;
        	this.MidiNoteCheckbox.CheckedChanged += new System.EventHandler(this.MidiNoteCheckboxCheckedChanged);
        	// 
        	// AnalyseBtn
        	// 
        	this.AnalyseBtn.Location = new System.Drawing.Point(747, 6);
        	this.AnalyseBtn.Name = "AnalyseBtn";
        	this.AnalyseBtn.Size = new System.Drawing.Size(53, 23);
        	this.AnalyseBtn.TabIndex = 9;
        	this.AnalyseBtn.Text = "Analyse";
        	this.AnalyseBtn.UseVisualStyleBackColor = true;
        	this.AnalyseBtn.Click += new System.EventHandler(this.AnalyseBtnClick);
        	// 
        	// EditorFrame
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.AutoSize = true;
        	this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        	this.ClientSize = new System.Drawing.Size(802, 266);
        	this.Controls.Add(this.AnalyseBtn);
        	this.Controls.Add(this.MidiNoteCheckbox);
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
        private System.Windows.Forms.Button AnalyseBtn;
        private System.Windows.Forms.CheckBox MidiNoteCheckbox;
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