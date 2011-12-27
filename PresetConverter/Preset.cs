using System;

namespace PresetConverter
{
	/// <summary>
	/// Description of Preset.
	/// </summary>
	public interface Preset
	{

		void Read(string filePath);
		
		void Write(string filePath);
		
	}
}
