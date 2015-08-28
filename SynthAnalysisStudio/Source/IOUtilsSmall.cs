using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CommonUtils
{
	/// <summary>
	/// Utils for input output (IO).
	/// </summary>
	public static class IOUtils
	{
		
		/// <summary>
		/// Log a message to file (e.g. a log file)
		/// </summary>
		/// <param name="file">filename to use</param>
		/// <param name="msg">message to log</param>
		public static void LogMessageToFile(FileInfo file, string msg)
		{
			// Make sure to support Multithreaded write access
			using (var fs = new FileStream(file.FullName, FileMode.Append, FileAccess.Write, FileShare.Write))
			{
				using (var sw = new StreamWriter(fs))
				{
					string logLine = String.Format(
						"{0:G}: {1}", DateTime.Now, msg);
					sw.WriteLine(logLine);
				}
			}
		}
		
	}
}

