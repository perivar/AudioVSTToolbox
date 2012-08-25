using System;
using System.Windows.Forms;

namespace CommonUtils.GUI
{
	/// <summary>
	/// Description of GUIUtils.
	/// </summary>
	public static class GUIUtils
	{
		/// <summary>
		/// Prompt for a directory path and return a string
		/// </summary>
		/// <param name="selectedPath">selected path</param>
		/// <param name="description">description on the dialog box</param>
		/// <returns></returns>
		public static string PromptForPath(string selectedPath, string description) {

			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (null != selectedPath && "" != selectedPath) {
				dlg.SelectedPath = selectedPath;
			} else {
				dlg.SelectedPath = @"C:\";
			}

			if (null != description && "" != description) {
				dlg.Description = description;
			} else {
				dlg.Description = "Select a folder";
			}
			
			dlg.ShowNewFolderButton = true;
			DialogResult res = dlg.ShowDialog();
			if (res == DialogResult.OK) {
				return dlg.SelectedPath;
			}
			
			return null;
		}

		/// <summary>
		/// Prompt for a file path and return a string
		/// </summary>
		/// <param name="initialDirectory">initial directory path</param>
		/// <param name="title">title on the dialog box</param>
		/// <param name="filter">can be like this: "txt files (*.txt)|*.txt|All files (*.*)|*.*"</param>
		/// <returns></returns>
		public static string PromptForFile(string initialDirectory, string title, string filter) {

			OpenFileDialog dlg = new OpenFileDialog();
			if (null != initialDirectory && "" != initialDirectory) {
				dlg.InitialDirectory = initialDirectory;
			} else {
				dlg.InitialDirectory = @"C:\";
			}

			if (null != title && "" != title) {
				dlg.Title = title;
			} else {
				dlg.Title = "Select a file";
			}

			if (null != filter && "" != filter) {
				dlg.Filter = filter;
			}

			DialogResult res = dlg.ShowDialog();
			if (res == DialogResult.OK) {
				return dlg.FileName;
			}
			
			return null;
		}
	}
}
