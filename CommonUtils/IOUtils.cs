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
		/// <summary>
		/// Return all files by their extension in ONE Directory (not recursive)
		/// </summary>
		/// <param name="dir">Directoy Path</param>
		/// <param name="extensions">extensions, e.g. ".jpg",".exe",".gif"</param>
		/// <returns></returns>
		/// <example>dInfo.GetFilesByExtensions(".jpg",".exe",".gif");</example>
		public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
		{
			if (extensions == null)
				throw new ArgumentNullException("extensions");
			
			IEnumerable<FileInfo> files = dir.EnumerateFiles();
			return files.Where(f => extensions.Contains(f.Extension));
		}
		
		/// <summary>
		/// Get Files using regexp pattern like \.mp3|\.mp4\.wav\.ogg
		/// By using SearchOption.AllDirectories, you can make it recursive
		/// </summary>
		/// <param name="path">Directoy Path</param>
		/// <param name="searchPatternExpression">Regexp pattern like \.mp3|\.mp4\.wav\.ogg</param>
		/// <param name="searchOption">SearchOption like SearchOption.AllDirectories</param>
		/// <returns>IEnumerable array of filenames</returns>
		/// <example>var files = IOUtils.GetFiles(path, "\\.mp3|\\.mp4\\.wav\\.ogg", SearchOption.AllDirectories);</example>
		public static IEnumerable<string> GetFiles(string path, string searchPatternExpression = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			Regex reSearchPattern = new Regex(searchPatternExpression);
			return Directory.EnumerateFiles(path, "*", searchOption).Where(file => reSearchPattern.IsMatch(Path.GetExtension(file)));
		}

		/// <summary>
		/// Get Files using regexp array of extensions and executes in parallel
		/// By using SearchOption.AllDirectories, you can make it recursive
		/// </summary>
		/// <param name="path">Directoy Path</param>
		/// <param name="searchPatterns">Array of extensions like: string[] extensions = { "*.mp3", "*.wav", "*.ogg" };</param>
		/// <param name="searchOption">SearchOption like SearchOption.AllDirectories</param>
		/// <returns>IEnumerable array of filenames</returns>
		/// <example>
		/// string[] extensions = { "*.mp3", "*.wma", "*.mp4", "*.wav", "*.ogg" };
		/// var files = IOUtils.GetFiles(path, extensions, SearchOption.AllDirectories);
		/// </example>
		public static IEnumerable<string> GetFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, searchPattern, searchOption));
		}
		
		/// <summary>
		/// Determine wheter a path is a file or a directory
		/// </summary>
		/// <param name="fileOrDirectoryPath">path</param>
		/// <returns>bool if the path is a directory</returns>
		public static bool IsDirectory(string fileOrDirectoryPath) {
			
			return Directory.Exists(fileOrDirectoryPath);
			/*
			// get the file attributes for file or directory
			FileAttributes attr = File.GetAttributes(fileOrDirectoryPath);

			//detect whether its a directory or file
			if((attr & FileAttributes.Directory) == FileAttributes.Directory) {
				return true;
			} else {
				return false;
			}
			 */
		}
		
		/// <summary>
		/// Log a message to file (e.g. a log file)
		/// </summary>
		/// <param name="file">filename to use</param>
		/// <param name="msg">message to log</param>
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

