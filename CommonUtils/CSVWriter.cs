using System.IO;
using System.Text;
using System.Globalization;

namespace CommonUtils
{
	/// <summary>
	///   Class for writing any object values in comma separated file
	/// </summary>
	public class CSVWriter
	{
		/// <summary>
		///   Separator used while writing to CVS
		/// </summary>
		private const char SEPARATOR = ',';

		/// <summary>
		///   Carriage return line feed
		/// </summary>
		private const string CRLF = "\r\n";

		/// <summary>
		///   Path to file
		/// </summary>
		private readonly string _pathToFile;

		/// <summary>
		///   Writer
		/// </summary>
		private StreamWriter _writer;
		
		/// <summary>
		/// Separator Character
		/// </summary>
		private char _separatorChar = SEPARATOR;
		
		/// <summary>
		/// Output encoding format
		/// </summary>
		private Encoding _encoding = Encoding.Default;

		/// <summary>
		/// Create a CSV Writer and specify where to store the file, using semicolon as the separator character
		/// </summary>
		/// <param name="pathToFile">Path to filename</param>
		public CSVWriter(string pathToFile)
		{
			_pathToFile = pathToFile;
		}

		/// <summary>
		/// Create a CSV Writer and specify where to store the file
		/// </summary>
		/// <param name="pathToFile">Path to filename</param>
		/// <param name="separatorChar">Separator character</param>
		public CSVWriter(string pathToFile, char separatorChar)
		{
			_pathToFile = pathToFile;
			_separatorChar = separatorChar;
		}

		public CSVWriter(string pathToFile, char separatorChar, Encoding encoding)
		{
			_pathToFile = pathToFile;
			_separatorChar = separatorChar;
			_encoding = encoding;
		}
		
		/// <summary>
		/// Write the data into CSV
		/// </summary>
		/// <param name = "data">Data to be written</param>
		public void Write(object[][] data)
		{
			using (_writer = new StreamWriter(_pathToFile, false, _encoding))
			{
				int cols = data[0].Length;
				var builder = new StringBuilder();
				for (int i = 0, n = data.GetLength(0); i < n; i++)
				{
					for (int j = 0; j < cols; j++)
					{
						object elem = data[i][j];
						
						// if the element is floating point, use 9 decimal places
						if (elem is float || elem is double) {
							builder.Append(string.Format(CultureInfo.InvariantCulture, "{0:0.000000000}", elem));
						} else {
							builder.Append(elem);
						}
						
						if (j != cols - 1)
							builder.Append(_separatorChar);
					}
					builder.Append(CRLF);
				}
				_writer.Write(builder.ToString());
				_writer.Close();
			}
		}
	}
}