using System;
using System.Windows.Forms;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core;

namespace MidiVstTest
{
	public partial class EditParametersForm : Form
	{
		public EditParametersForm()
		{
			InitializeComponent();
		}

		private void EditParametersForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Visible = false;
		}

		public void AddParameters(VstPluginContext pluginContext)
		{
			lbParameters.Items.Clear();

			for (int i = 0; i < pluginContext.PluginInfo.ParameterCount; i++)
			{
				var param = new VSTParameter();
				param.Properties = pluginContext.PluginCommandStub.GetParameterProperties(i);
				param.Name = pluginContext.PluginCommandStub.GetParameterName(i);
				param.Label = pluginContext.PluginCommandStub.GetParameterLabel(i);
				param.Display = pluginContext.PluginCommandStub.GetParameterDisplay(i);

				lbParameters.Items.Add(param);
			}
		}
	}
	
	public struct VSTParameter
	{
		public int Index;

		public string Name;
		public string Label;
		public string Display;

		public VstParameterProperties Properties;

		public override string ToString()
		{
			return Name+" = "+Display+Label;
		}
	}
}
