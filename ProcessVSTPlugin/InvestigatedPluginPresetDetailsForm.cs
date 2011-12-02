using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Equin.ApplicationFramework;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of InvestigatedPluginPresetDetailsForm.
	/// </summary>
	public partial class InvestigatedPluginPresetDetailsForm : Form
	{
		private List<InvestigatedPluginPresetFileFormat> investigatedPluginPresetFileFormatList;
		private BindingListView<InvestigatedPluginPresetFileFormat> investigatedPluginPresetFileFormatView;
		
		public VstPluginContext PluginContext { get; set; }
		
		public List<InvestigatedPluginPresetFileFormat> InvestigatedPluginPresetFileFormatList {
			set {
				this.investigatedPluginPresetFileFormatList = value;
				this.investigatedPluginPresetFileFormatView = new BindingListView<InvestigatedPluginPresetFileFormat>(this.investigatedPluginPresetFileFormatList);
				this.dataGridView1.DataSource = this.investigatedPluginPresetFileFormatView;
			}
			get {
				return investigatedPluginPresetFileFormatList;
			}
		}
		
		public InvestigatedPluginPresetDetailsForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			
			FilterTextBox.TextChanged += delegate {
				this.investigatedPluginPresetFileFormatView.ApplyFilter(
					delegate(InvestigatedPluginPresetFileFormat format) {
						return format.ParameterName.Contains(FilterTextBox.Text);
					});
			};
		}

		private void updateDataGridView()
		{
			this.dataGridView1.DataSource = null;
			this.dataGridView1.DataSource = investigatedPluginPresetFileFormatView;
		}
		
		void UpdateBtnClick(object sender, EventArgs e)
		{
			updateDataGridView();
		}
		
		void CSVDumpBtnClick(object sender, EventArgs e)
		{
			string strValue = string.Empty;
			for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
			{
				for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
				{
					if (!string.IsNullOrEmpty(dataGridView1[j, i].Value.ToString()))
					{
						if (j > 0)
							strValue = strValue + ";" + dataGridView1[j, i].Value.ToString();
						else
						{
							if (string.IsNullOrEmpty(strValue))
								strValue = dataGridView1[j, i].Value.ToString();
							else
								strValue = strValue + Environment.NewLine + dataGridView1[j, i].Value.ToString();
						}
					} else {
						if (j > 0)
							strValue = strValue + ";";
					}
				}
			}

			using (StreamWriter MyFile = new StreamWriter(Application.StartupPath+"\\output.csv"))
			{
				MyFile.Write(strValue);
				MessageBox.Show("Information in the the table is successfully saved in the following location: \n"+Application.StartupPath+"\\output.csv", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
	}
}




