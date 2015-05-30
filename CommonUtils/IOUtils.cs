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
			var reSearchPattern = new Regex(searchPatternExpression);
			return Directory.EnumerateFiles(path, "*", searchOption).Where(file => reSearchPattern.IsMatch(Path.GetExtension(file).ToLower()));
		}

		/// <summary>
		/// Get Files using array of extensions and executes in parallel
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
		/// Get Files recursively using a search pattern
		/// </summary>
		/// <param name="path">Directoy Path</param>
		/// <param name="searchPattern">Search pattern like: "*.mp3" or "one_specific_file.wav"</param>
		/// <returns>IEnumerable array of filenames</returns>
		public static IEnumerable<string> GetFilesRecursive(string path, string searchPattern) {
			return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
		}
		
		/// <summary>
		/// Backup a file to a filename.bak or filename.bak_number etc
		/// </summary>
		/// <param name="fileName">filename to backup</param>
		public static void MakeBackupOfFile(string fileName) {
			if (File.Exists(fileName)) {
				
				string destinationBackupFileName = fileName + ".bak";
				
				// make sure to create a new backup if the backup file already exist
				int backupFileCount = -1;
				do
				{
					backupFileCount++;
				}
				while (File.Exists(destinationBackupFileName + (backupFileCount > 0 ? "_" + backupFileCount.ToString() : "")));
				
				destinationBackupFileName = (destinationBackupFileName + (backupFileCount > 0 ? "_" + (backupFileCount).ToString() : ""));
				File.Copy(fileName, destinationBackupFileName);
			}
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
		
		/// <summary>
		/// Read everything from a file as text (string)
		/// </summary>
		/// <param name="filePath">file</param>
		/// <returns>string</returns>
		public static string ReadTextFromFile(string filePath) {
			return File.ReadAllText(filePath);
		}
		
		/// <summary>
		/// Write text to a file
		/// </summary>
		/// <param name="filePath">file</param>
		/// <param name="text">text to write</param>
		/// <returns>true if successful</returns>
		public static bool WriteTextToFile(string filePath, string text) {
			try {
				// create a writer and open the file
				TextWriter tw = new StreamWriter(filePath);
				
				// write the text
				tw.Write(text);
				
				// close the stream
				tw.Close();
			} catch (Exception) {
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Return a temporary file name
		/// </summary>
		/// <param name="extension">extension without the dot e.g. wav or csv</param>
		/// <returns>filepath to the temporary file</returns>
		public static string GetTempFilePathWithExtension(string extension) {
			var path = Path.GetTempPath();
			var fileName = Guid.NewGuid().ToString() + "." + extension;
			return Path.Combine(path, fileName);
		}

		/// <summary>
		/// Return the right part of the path after a given base path if found
		/// </summary>
		/// <param name="path">long path</param>
		/// <param name="startAfterPart">base path</param>
		/// <returns></returns>
		public static string GetRightPartOfPath(string path, string startAfterPart)
		{
			int startAfter = path.LastIndexOf(startAfterPart);

			if (startAfter == -1)
			{
				// path path not found
				return null;
			}

			return path.Substring(startAfterPart.Length);
		}
	}
}

