using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of WavesSSLChannelToUADSSLChannelAdapter.
	/// </summary>
	public class WavesSSLChannelToUADSSLChannelAdapter
	{
		WavesSSLChannel wavesSSLChannel = null;
		
		public WavesSSLChannelToUADSSLChannelAdapter(WavesSSLChannel wavesSSLChannel)
		{
			this.wavesSSLChannel = wavesSSLChannel;
		}
		
		public UADSSLChannel DoConvert() {
			
			UADSSLChannel uadSSLChannel = new UADSSLChannel();
			uadSSLChannel.PresetName = wavesSSLChannel.PresetName;
			
			string xmlFileName = @"C:\Users\perivar.nerseth\Documents\My Projects\AudioVSTToolbox\InvestigatePresetFileDump\ParametersMap.xml";
			
			uadSSLChannel.CMPThresh = FindClosest(xmlFileName, "CMP Thresh", wavesSSLChannel.CompThreshold);
			uadSSLChannel.CMPRatio = FindClosest(xmlFileName, "CMP Ratio", wavesSSLChannel.CompRatio);
			uadSSLChannel.CMPAttack = Convert.ToSingle(wavesSSLChannel.CompFastAttack);
			uadSSLChannel.CMPRelease = FindClosest(xmlFileName, "CMP Release", wavesSSLChannel.CompRelease);
			
			uadSSLChannel.EXPThresh = FindClosest(xmlFileName, "EXP Thresh", wavesSSLChannel.ExpThreshold);
			uadSSLChannel.EXPRange = FindClosest(xmlFileName, "EXP Range", wavesSSLChannel.ExpRange);
			//wavesSSLChannel.ExpGate
			uadSSLChannel.EXPAttack = Convert.ToSingle(wavesSSLChannel.ExpFastAttack);
			uadSSLChannel.EXPRelease = FindClosest(xmlFileName, "EXP Release", wavesSSLChannel.ExpRelease);
			
			uadSSLChannel.CompIn = Convert.ToSingle(wavesSSLChannel.DynToByPass);
			uadSSLChannel.PreDyn = Convert.ToSingle(wavesSSLChannel.DynToChannelOut);

			uadSSLChannel.LFBell = Convert.ToSingle(wavesSSLChannel.LFTypeBell);
			uadSSLChannel.LFGain = FindClosest(xmlFileName, "LF Gain", wavesSSLChannel.LFGain);
			uadSSLChannel.LFFreq = FindClosest(xmlFileName, "LF Freq", wavesSSLChannel.LFFrq);
			/*
			
		wavesSSLChannel.LMFGain;
		wavesSSLChannel.LMFFrq;
		wavesSSLChannel.LMFQ;
		
		wavesSSLChannel.HMFGain;
		wavesSSLChannel.HMFFrq;
		wavesSSLChannel.HMFQ;
		
		wavesSSLChannel.HFTypeBell;
		wavesSSLChannel.HFGain;
		wavesSSLChannel.HFFrq;
		
		wavesSSLChannel.EQToBypass;
		wavesSSLChannel.EQToDynSC;

		wavesSSLChannel.HPFrq;
		wavesSSLChannel.LPFrq;
		wavesSSLChannel.FilterSplit;
		
		wavesSSLChannel.Gain;
		wavesSSLChannel.Analog;
		wavesSSLChannel.VUShowOutput;
		wavesSSLChannel.PhaseReverse;
		wavesSSLChannel.InputTrim;
			 */
			
			return uadSSLChannel;
		}
		
		private static float FindClosest(string xmlfilename, string name, float searchFor) {
			
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			
			var entries = (from entry in xmlDoc.Descendants("Entry")
			               where entry.Parent.Attribute("name").Value == name
			               select new
			               {
			               	DisplayText = (string) entry.Element("DisplayText").Value,
			               	DisplayNumber = (float) GetDouble(entry.Element("DisplayNumber").Value, 0),
			               	Value = float.Parse(entry.Element("Value").Value)
			               });
			
			// lists to store values
			List<string> displayText = new List<string>();
			List<float> displayNumbers = new List<float>();
			List<float> values = new List<float>();
			
			foreach (var entry in entries) {
				displayText.Add(entry.DisplayText);
				displayNumbers.Add(entry.DisplayNumber);
				values.Add(entry.Value);
			}
			
			// find closests float value
			float foundClosest = displayNumbers.Aggregate( (x,y) => Math.Abs(x - searchFor) < Math.Abs(y - searchFor) ? x : y);
			int foundIndex = displayNumbers.IndexOf(foundClosest);
			string foundClosestDisplayText = displayText[foundIndex];
			float foundParameterValue = values[foundIndex];
			
			Console.Out.WriteLine("Searching '{0}' for value {1}. Found {2} with text '{3}'. Value = {4}", name, searchFor, foundClosest, foundClosestDisplayText, foundParameterValue);
			
			return foundParameterValue;
			
			/*
			var numbers = new List<float> { 10f, 20f, 22f, 30f };
			var target = 21f;

			//gets single number which is closest
			var closest = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.OrderBy( p => p.distance )
				.First().n;

			//get two closest
			var take = 2;
			var closests = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.OrderBy( p => p.distance )
				.Select( p => p.n )
				.Take( take );

			//gets any that are within x of target
			var within = 1;
			var withins = numbers.Select( n => new { n, distance = Math.Abs( n - target ) } )
				.Where( p => p.distance <= within )
				.Select( p => p.n );
			 */
			
		}
		
		public static double GetDouble(string value, double defaultValue)
		{
			double result;

			//Try parsing in the current culture
			if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
			    //Then try in US english
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
			    //Then in neutral language
			    !double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				result = defaultValue;
			}
			
			// special case
			if (result == 3.911555E-07) result = 0;

			return result;
		}
	}
}
