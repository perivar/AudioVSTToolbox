using System;
using System.IO;

using CommonUtils;

namespace PresetConverter
{
	/// <summary>
	/// Description of Logger.
	/// </summary>
	public static class Logger
	{
		public enum LogLevel {
			Debug,
			Normal
		}
		
		// define the log file
		static readonly FileInfo outputStatusLog = new FileInfo("preset_converter_log.txt");

		// define the error log file
		static readonly FileInfo outputErrorLog = new FileInfo("preset_converter_error_log.txt");
		
		// define the default log level
		public static LogLevel logLevel = LogLevel.Normal;

		public static void DoLog(string logMsg) {
			Console.Out.WriteLine(logMsg);
			IOUtils.LogMessageToFile(outputStatusLog, logMsg);
		}
		
		public static void DoDebug(string debugMsg) {
			if (logLevel == LogLevel.Debug) IOUtils.LogMessageToFile(outputStatusLog, debugMsg);
		}

		public static void DoError(string errorMsg) {
			Console.Out.WriteLine(errorMsg);
			IOUtils.LogMessageToFile(outputStatusLog, errorMsg);
			IOUtils.LogMessageToFile(outputErrorLog, errorMsg);
		}
	}
}
