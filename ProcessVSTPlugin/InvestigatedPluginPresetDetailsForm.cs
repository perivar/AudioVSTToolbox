using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using System.Globalization;

using Equin.ApplicationFramework; // for BindingListView
using System.ComponentModel; // for BindingList
using System.Data; // for DataTable

using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core.Host;

using CommonUtils;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// Description of InvestigatedPluginPresetDetailsForm.
	/// </summary>
	public partial class InvestigatedPluginPresetDetailsForm : Form
	{
		public VstPluginContext PluginContext { get; set; }

		private BindingList<InvestigatedPluginPresetFileFormat> investigatedPluginPresetFileFormatList;
		private BindingListView<InvestigatedPluginPresetFileFormat> investigatedPluginPresetFileFormatView;
		
		public BindingList<InvestigatedPluginPresetFileFormat> InvestigatedPluginPresetFileFormatList {
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
			
			// add filter clause
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
		
		/*
		 * Dumps the current datagridview to a csv file.
		 * Note! Some numeric will be interpreted by Excel wrong ("1/8" gives 01.08.2011 which is wrong)
		 * */
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
		
		/*
		 * Dumps the current datagridview to a XML file.
		 * */
		void XMLDumpBtnClick(object sender, EventArgs e)
		{
			//set formatting options
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";

			using (XmlWriter writer = XmlWriter.Create(Application.StartupPath+"\\output.xml", settings))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("TrackedPresetFileChanges");

				for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
				{
					writer.WriteStartElement("Row");
					for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
					{
						if (!string.IsNullOrEmpty(dataGridView1[j, i].Value.ToString()))
						{
							writer.WriteElementString(dataGridView1.Columns[j].Name, dataGridView1[j, i].Value.ToString());
						} else {
							writer.WriteElementString(dataGridView1.Columns[j].Name, "");
						}
					}
					writer.WriteEndElement();
				}
				
				writer.WriteEndElement();
				writer.WriteEndDocument();
				MessageBox.Show("Information in the the table is successfully saved in the following location: \n"+Application.StartupPath+"\\output.xml", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		
		void DataGridView1RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			dataGridView1.ClearSelection();

			int nRowIndex = dataGridView1.Rows.Count - 1;
			if (nRowIndex > 0) {
				dataGridView1.Rows[nRowIndex].Selected = true;
				dataGridView1.FirstDisplayedScrollingRowIndex = nRowIndex;
			}
			
			// print the label
			if (checkBox1.Checked) {
				if (((HostCommandStub) PluginContext.HostCommandStub).DoTrackPluginPresetFileFormat) {
					TrackPresetTextBox.Text = "";
					byte[] trackedBytes = ((HostCommandStub) PluginContext.HostCommandStub).TrackPluginPresetFileBytes;
					
					BinaryFile.ByteOrder bOrder = BinaryFile.ByteOrder.BigEndian;
					bool invert = false;
					if (LittleEndianCheckBox.Checked) {
						bOrder = BinaryFile.ByteOrder.LittleEndian;
						invert = true;
					}
					TrackPresetTextBox.Text = StringUtils.ToHexAndAsciiString(trackedBytes, invert);
					
					int i = BinaryFile.ByteArrayToInt32(trackedBytes, bOrder);
					float f = BinaryFile.ByteArrayToSingle(trackedBytes, bOrder);
					ValueTextBox.Text = String.Format("int: {0} float: {1:00.0000}", i, f);
					
					// store these values in a tracking list together with the ParameterDisplay Value
					string paramName = dataGridView1["ParameterName", nRowIndex].Value.ToString();
					string paramValue = dataGridView1["ParameterDisplay", nRowIndex].Value.ToString().Trim();
					InvestigatedPluginPresetFileFormat row = new InvestigatedPluginPresetFileFormat(0, 0, paramName, "", paramValue, String.Format("{0:00.0000}",f));
					
					// store this in a xml ouput file.
					string xmlFilePath = "track-chunk-dump.xml";
					if (File.Exists(xmlFilePath)) {
						// add to existing xml document if the entry does not exist from before.
						XDocument xmlDoc = XDocument.Load(xmlFilePath);
						
						var query = from r in xmlDoc.Element("TrackedPresetFileChanges")
							.Elements("Row")
							where r.Element("ParameterName").Value == paramName
							&& r.Element("ParameterDisplay").Value == paramValue
							&& Convert.ToSingle(r.Element("FloatValue").Value, CultureInfo.InvariantCulture) == f
							&& Convert.ToInt32(r.Element("IntValue").Value) == i
							select r;
						
						if (query.Count() == 0) {
							xmlDoc.Element("TrackedPresetFileChanges").Add(
								new XElement("Row",
								             new XElement("ParameterName", paramName),
								             new XElement("ParameterDisplay", paramValue),
								             new XElement("FloatValue", f),
								             new XElement("IntValue", i)));

							xmlDoc.Save(xmlFilePath);
						}
					} else {
						// create xml document first
						XDocument xmlDoc =
							new XDocument(
								new XElement("TrackedPresetFileChanges",
								             new XElement("Row",
								                          new XElement("ParameterName", paramName),
								                          new XElement("ParameterDisplay", paramValue),
								                          new XElement("FloatValue", f),
								                          new XElement("IntValue", i))
								            )
							);
						xmlDoc.Save(xmlFilePath);
					}
				}
			}
		}
		
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			CheckBox check = (CheckBox) sender;
			if(check.Checked)
			{
				((HostCommandStub) PluginContext.HostCommandStub).DoTrackPluginPresetFileFormat = true;
			} else {
				((HostCommandStub) PluginContext.HostCommandStub).DoTrackPluginPresetFileFormat = false;
			}
		}
		
		void TrackPositionTextBoxTextChanged(object sender, EventArgs e)
		{
			int pos = 0;
			int.TryParse(this.TrackPositionTextBox.Text, out pos);
			((HostCommandStub) PluginContext.HostCommandStub).TrackPluginPresetFilePosition = pos;
		}
		
		void NumberOfBytesTextBoxTextChanged(object sender, EventArgs e)
		{
			int numBytes = 0;
			int.TryParse(this.NumberOfBytesTextBox.Text, out numBytes);
			((HostCommandStub) PluginContext.HostCommandStub).TrackPluginPresetFileNumberOfBytes = numBytes;
		}

		// set the indexinfile property for chunk tracking when double clicking on a cell
		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				checkBox1.Checked = true;
				((HostCommandStub) PluginContext.HostCommandStub).DoTrackPluginPresetFileFormat = true;
				
				if (NumberOfBytesTextBox.Text == "") {
					NumberOfBytesTextBox.Text = "4";
					int numBytes = 0;
					int.TryParse(this.NumberOfBytesTextBox.Text, out numBytes);
					((HostCommandStub) PluginContext.HostCommandStub).TrackPluginPresetFileNumberOfBytes = numBytes;
				}
				
				string sindex = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["IndexInFile"].Value.ToString();
				this.TrackPositionTextBox.Text = sindex;
				
				int pos = 0;
				int.TryParse(this.TrackPositionTextBox.Text, out pos);
				((HostCommandStub) PluginContext.HostCommandStub).TrackPluginPresetFilePosition = pos;
			}
			catch (Exception)
			{
			}
			
		}
	}
}


