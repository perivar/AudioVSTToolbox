using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CommonUtils
{
	/// <summary>
	/// Utils for input output (IO).
	/// </summary>
	public static class IOUtils
	{
		public static FileInfo[] GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
		{
			if (extensions == null)
				throw new ArgumentNullException("extensions");
			IEnumerable<FileInfo> files = dir.EnumerateFiles();
			return files.Where(f => extensions.Contains(f.Extension)).ToArray<FileInfo>();
		}
		
		public static bool IsDirectory(string fileOrDirectoryPath) {
			// get the file attributes for file or directory
			FileAttributes attr = File.GetAttributes(fileOrDirectoryPath);

			//detect whether its a directory or file
			if((attr & FileAttributes.Directory) == FileAttributes.Directory) {
				return true;
			} else {
				return false;
			}
		}
		
		public static void LogMessageToFile(FileInfo file, string msg)
		{
			System.IO.StreamWriter sw = System.IO.File.AppendText(file.FullName);
			try
			{
				string logLine = System.String.Format(
					"{0:G}: {1}.", System.DateTime.Now, msg);
				sw.WriteLine(logLine);
			}
			finally
			{
				sw.Close();
			}
		}
	}
}

