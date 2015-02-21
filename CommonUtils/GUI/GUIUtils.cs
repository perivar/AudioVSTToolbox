using System;
using System.Windows.Forms;

namespace CommonUtils.GUI
{
	/// <summary>
	/// Assorted GUI related Utils
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

			var dlg = new FolderBrowserDialog();
			if (!string.IsNullOrEmpty(selectedPath)) {
				dlg.SelectedPath = selectedPath;
			} else {
				dlg.SelectedPath = @"C:\";
			}

			if (!string.IsNullOrEmpty(description)) {
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

			var dlg = new OpenFileDialog();
			if (!string.IsNullOrEmpty(initialDirectory)) {
				dlg.InitialDirectory = initialDirectory;
			} else {
				dlg.InitialDirectory = @"C:\";
			}

			if (!string.IsNullOrEmpty(title)) {
				dlg.Title = title;
			} else {
				dlg.Title = "Select a file";
			}

			if (!string.IsNullOrEmpty(filter)) {
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
