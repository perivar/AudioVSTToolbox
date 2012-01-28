using System;

namespace PresetConverter
{
	/// <summary>
	/// Description of Preset.
	/// </summary>
	public interface Preset
	{

		bool Read(string filePath);
		
		bool Write(string filePath);
		
	}
}
