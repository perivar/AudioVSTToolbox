using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

using PresetConverter;
using SynthAnalysisStudio;

namespace DumpPreset
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			// Enable drag drop on form
			this.AllowDrop = true;
			this.DragEnter += new DragEventHandler(Form_DragEnter);
			this.DragDrop += new DragEventHandler(Form_DragDrop);
			
			// Enable drag drop on text field
			txtContent.AllowDrop = true;
			txtContent.DragEnter += new DragEventHandler(Form_DragEnter);
			txtContent.DragDrop += new DragEventHandler(Form_DragDrop);
		}
		
		void Form_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
		}
		
		void Form_DragDrop(object sender, DragEventArgs e)
		{
			var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string filePath in filePaths) {
				txtContent.AppendText("Dumping Contents from file: " + filePath + "\n");
				
				ProcessFile(filePath);
				
				txtContent.AppendText("\n");
			}
		}
		
		void ProcessFile(string filePath) {
			string[] validExtensions = { ".fxp", ".fxb", ".h2p", ".xps" };
			
			var fileInfo = new FileInfo(filePath);
			
			if (validExtensions.Contains(fileInfo.Extension)) {
				// read preset file
				var sylenth1 = new Sylenth1Preset();
				if (sylenth1.Read(fileInfo.FullName)) {
					txtContent.AppendText(sylenth1.ToString());
				}
				
				// read preset file
				var uadSSLChannel = new UADSSLChannel();
				if (uadSSLChannel.Read(fileInfo.FullName)) {
					txtContent.AppendText(uadSSLChannel.ToString());
				}
				
				#region Waves preset reading
				var ssl = new WavesSSLChannel();
				var sslcomp = new WavesSSLComp();
				using (var stringWriter = new StringWriter()) {
					if (ssl.ReadXps(fileInfo.FullName, stringWriter)) {
						txtContent.AppendText(stringWriter.ToString());
					}
					if (sslcomp.ReadXps(fileInfo.FullName, stringWriter)) {
						txtContent.AppendText(stringWriter.ToString());
					}
				}
				#endregion

				var zebra = new Zebra2Preset();
				if (zebra.Read(fileInfo.FullName)) {
					txtContent.AppendText(zebra.GetReadableOscillatorInfo());
					txtContent.AppendText(zebra.GetReadableEnvelopeInfo());
					txtContent.AppendText(zebra.GetReadableLFOInfo());
					txtContent.AppendText(zebra.GetReadableFilterInfo());

				}
			}
			
		}
	}
}