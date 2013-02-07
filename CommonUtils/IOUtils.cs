using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
		
		/// <summary>
		/// Get Files using regexp pattern like \.mp3|\.mp4\.wav\.ogg
		/// </summary>
		/// <param name="path">Directoy Path</param>
		/// <param name="searchPatternExpression">Regexp pattern like \.mp3|\.mp4\.wav\.ogg</param>
		/// <param name="searchOption">SearchOption like SearchOption.AllDirectories</param>
		/// <returns>IEnumerable array of filenames</returns>
		/// <example>IOUtils.GetFiles(path, "\\.mp3|\\.mp4\\.wav\\.ogg", SearchOption.AllDirectories);</example>
		public static IEnumerable<string> GetFiles(string path, string searchPatternExpression = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			Regex reSearchPattern = new Regex(searchPatternExpression);
			return Directory.EnumerateFiles(path, "*", searchOption).Where(file => reSearchPattern.IsMatch(Path.GetExtension(file)));
		}

		/// <summary>
		/// Get Files using regexp array of extensions and executes in parallel
		/// </summary>
		/// <param name="path">Directoy Path</param>
		/// <param name="searchPatterns">Array of extensions like: string[] extensions = { "*.mp3", "*.wav", "*.ogg" };</param>
		/// <param name="searchOption">SearchOption like SearchOption.AllDirectories</param>
		/// <returns>IEnumerable array of filenames</returns>
		public static IEnumerable<string> GetFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption));
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
					"{0:G}: {1}", System.DateTime.Now, msg);
				sw.WriteLine(logLine);
			}
			finally
			{
				sw.Close();
			}
		}
	}
}

