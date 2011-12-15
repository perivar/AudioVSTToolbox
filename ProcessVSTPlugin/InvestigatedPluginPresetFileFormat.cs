using System;
using System.ComponentModel;

namespace ProcessVSTPlugin
{
	/// <summary>
	/// InvestigatedPluginPresetFileFormat is used to store information about what binary content that changes in a preset file.
	/// http://www.switchonthecode.com/tutorials/csharp-tutorial-binding-a-datagridview-to-a-collection
	/// </summary>
	public class InvestigatedPluginPresetFileFormat : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		private int _indexInFile;
		private byte _byteValue;
		private string _byteValueHexString;
		private string _parameterName;
		private string _parameterNameFormatted;
		private string _parameterLabel;
		private string _parameterDisplay;

		private string _textChanges;
		
		public InvestigatedPluginPresetFileFormat(int indexInFile,
		                                          byte byteValue,
		                                          string parameterName,
		                                          string parameterLabel,
		                                          string parameterDisplay)
		{
			_indexInFile = indexInFile;
			_byteValue = byteValue;
			_byteValueHexString = StringUtils.ToHexString(byteValue);
			_parameterName = parameterName;
			_parameterNameFormatted = StringUtils.ConvertCaseString(_parameterName, StringUtils.Case.PascalCase);
			_parameterLabel = parameterLabel;
			_parameterDisplay = parameterDisplay;
		}
		
		public InvestigatedPluginPresetFileFormat(int indexInFile,
		                                          byte byteValue,
		                                          string parameterName,
		                                          string parameterLabel,
		                                          string parameterDisplay,
		                                          string textChanges)
			: this (indexInFile, byteValue, parameterName, parameterLabel, parameterDisplay)
		{
			_textChanges = textChanges;
		}
		
		public int IndexInFile
		{
			get { return _indexInFile; }
			set {
				_indexInFile = value;
				this.NotifyPropertyChanged("IndexInFile");
			}
		}

		public byte ByteValue
		{
			get { return _byteValue; }
			set {
				_byteValue = value;
				this.NotifyPropertyChanged("ByteValue");
			}
		}

		public string ByteValueHexString
		{
			get { return _byteValueHexString; }
			set {
				_byteValueHexString = value;
				this.NotifyPropertyChanged("ByteValueHexString");
			}
		}
		
		public string ParameterName
		{
			get { return _parameterName; }
			set {
				_parameterName = value;
				this.NotifyPropertyChanged("ParameterName");
			}
		}

		public string ParameterNameFormatted
		{
			get { return _parameterNameFormatted; }
			set {
				_parameterNameFormatted = value;
				this.NotifyPropertyChanged("ParameterNameFormatted");
			}
		}
		
		public string ParameterLabel
		{
			get { return _parameterLabel; }
			set {
				_parameterLabel = value;
				this.NotifyPropertyChanged("ParameterLabel");
			}
		}

		public string ParameterDisplay
		{
			get { return _parameterDisplay; }
			set {
				_parameterDisplay = value;
				this.NotifyPropertyChanged("ParameterDisplay");
			}
		}

		public string TextChanges
		{
			get { return _textChanges; }
			set {
				_textChanges = value;
				this.NotifyPropertyChanged("TextChanges");
			}
		}
		
		private void NotifyPropertyChanged(string name)
		{
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
