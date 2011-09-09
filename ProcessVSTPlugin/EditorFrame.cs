using System;
using System.Drawing;
using System.Windows.Forms;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// The frame in which a custom plugin editor UI is displayed.
	/// </summary>
	public partial class EditorFrame : Form
	{
		/// <summary>
		/// Default ctor.
		/// </summary>
		public EditorFrame()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the Plugin Command Stub.
		/// </summary>
		//public Jacobi.Vst.Core.Host.IVstPluginCommandStub PluginCommandStub { get; set; }
		
		public VstPluginContext PluginContext { get; set; }
		
		/// <summary>
		/// Shows the custom plugin editor UI.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public new DialogResult ShowDialog(IWin32Window owner)
		{
			Rectangle wndRect = new Rectangle();

			this.Text = PluginContext.PluginCommandStub.GetEffectName();

			if (PluginContext.PluginCommandStub.EditorGetRect(out wndRect))
			{
				this.pluginPanel.Size = this.SizeFromClientSize(new Size(wndRect.Width, wndRect.Height));
				//PluginContext.PluginCommandStub.EditorOpen(this.Handle);
				PluginContext.PluginCommandStub.EditorOpen(this.pluginPanel.Handle);
			}

			FillProgram();
			return base.ShowDialog(owner);
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			if (e.Cancel == false)
			{
				PluginContext.PluginCommandStub.EditorClose();
			}
		}
		
		private void FillProgram()
		{
			presetComboBox.Items.Clear();
			string[] programs = new String[PluginContext.PluginInfo.ProgramCount];
			for (int i = 0; i < PluginContext.PluginInfo.ProgramCount; i++) {
				PluginContext.PluginCommandStub.SetProgram(i);
				//int count = PluginContext.PluginCommandStub.GetProgram();
				string name = PluginContext.PluginCommandStub.GetProgramName();
				programs[i] = name;
			}
			presetComboBox.Items.AddRange(programs);
			//presetComboBox.DataSource = programs;
			//presetComboBox.SelectedIndex = 0;
			
			/*
			 * comboBox1.DataSource = myArray;
			 * 
			 * For the first variant you can only use strings as items, while with data binding you can bind a collection of more complex objects. You can then specify what properties are displayed:
			 * comboBox1.DisplayMember = "Name";
			 * 
			 * and what are treated as value:
			 * comboBox1.ValueMember = "ID";
			 * 
			 * You can access the original object that is selected later with
			 * comboBox1.SelectedItem
			 * 
			 * or the value with
			 * comboBox1.SelectedValue
			 * 
			 * The value is the property you specified with ValueMember
			 */
		}

		void SaveBtnClick(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Effect Preset Files (.fxp)|*.fxp|All Files|*.*||";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				string fxpFilePath = dialog.FileName;
				VstHost host = VstHost.Instance;
				host.PluginContext = this.PluginContext;
				
				host.SaveFXP(fxpFilePath);
			}
		}
		
		void LoadBtnClick(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Effect Preset Files (.fxp)|*.fxp|All Files|*.*||";
			if (dialog.ShowDialog(this) == DialogResult.OK)
			{
				string fxpFilePath = dialog.FileName;
				VstHost host = VstHost.Instance;
				host.PluginContext = this.PluginContext;
				
				host.LoadFXP(fxpFilePath);
				//this.Refresh();
				FillProgram();
				PluginContext.PluginCommandStub.EditorIdle();
				//presetComboBox.SelectedIndex = 0;
			}
		}
		
		void PresetComboBoxSelectedValueChanged(object sender, EventArgs e)
		{
			int index = presetComboBox.SelectedIndex;
			PluginContext.PluginCommandStub.SetProgram(index);
			//FillParameterList();
		}
	}
}
