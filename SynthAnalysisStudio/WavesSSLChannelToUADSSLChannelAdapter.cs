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
		
		// lists to store values
		Dictionary<string, List<string>> displayTextDict = new Dictionary<string, List<string>>();
		Dictionary<string, List<float>> displayNumbersDict = new Dictionary<string, List<float>>();
		Dictionary<string, List<float>> valuesDict = new Dictionary<string, List<float>>();

		public WavesSSLChannelToUADSSLChannelAdapter(WavesSSLChannel wavesSSLChannel)
		{
			this.wavesSSLChannel = wavesSSLChannel;
			
			InitializeMappingTables(@"C:\Users\perivar.nerseth\Documents\My Projects\AudioVSTToolbox\InvestigatePresetFileDump\ParametersMap.xml");
		}
		
		private void InitializeMappingTables(string xmlfilename) {
			XDocument xmlDoc = XDocument.Load(xmlfilename);
			
			var entries = (from entry in xmlDoc.Descendants("Entry")
			               group entry by (string) entry.Parent.Attribute("name").Value into g
			               select new {
			               	g.Key,
			               	Value = g.ToList()
			               });

			displayTextDict = entries.ToDictionary(o => o.Key, o => o.Value.Elements("DisplayText").Select(p => p.Value).ToList());
			displayNumbersDict = entries.ToDictionary(o => o.Key, o => o.Value.Elements("DisplayNumber").Select(p => (float)GetDouble(p.Value, 0)).ToList());
			valuesDict = entries.ToDictionary(o => o.Key, o => o.Value.Elements("Value").Select(p => float.Parse(p.Value)).ToList());
		}
		
		public UADSSLChannel DoConvert() {
			
			UADSSLChannel uadSSLChannel = new UADSSLChannel();
			uadSSLChannel.PresetName = wavesSSLChannel.PresetName;
			
			uadSSLChannel.CMPThresh = FindClosest("CMP Thresh", wavesSSLChannel.CompThreshold);
			uadSSLChannel.CMPRatio = FindClosest("CMP Ratio", wavesSSLChannel.CompRatio);
			uadSSLChannel.CMPAttack = Convert.ToSingle(wavesSSLChannel.CompFastAttack);
			uadSSLChannel.CMPRelease = FindClosest("CMP Release", wavesSSLChannel.CompRelease);
			
			uadSSLChannel.EXPThresh = FindClosest("EXP Thresh", wavesSSLChannel.ExpThreshold);
			uadSSLChannel.EXPRange = FindClosest("EXP Range", wavesSSLChannel.ExpRange);
			if (wavesSSLChannel.ExpGate) {
				uadSSLChannel.Select = FindClosest("Select", 0.5f);
			} else {
				uadSSLChannel.Select = FindClosest("Select", 0.0f);
			}
			uadSSLChannel.EXPAttack = Convert.ToSingle(wavesSSLChannel.ExpFastAttack);
			uadSSLChannel.EXPRelease = FindClosest("EXP Release", wavesSSLChannel.ExpRelease);
			
			uadSSLChannel.CompIn = Convert.ToSingle(!wavesSSLChannel.DynToByPass);
			uadSSLChannel.DYNIn = Convert.ToSingle(!wavesSSLChannel.DynToByPass);
			uadSSLChannel.PreDyn = Convert.ToSingle(wavesSSLChannel.DynToChannelOut);

			uadSSLChannel.LFBell = Convert.ToSingle(wavesSSLChannel.LFTypeBell);
			uadSSLChannel.LFGain = FindClosest("LF Gain", wavesSSLChannel.LFGain);
			uadSSLChannel.LFFreq = FindClosest("LF Freq", wavesSSLChannel.LFFrq);
			
			uadSSLChannel.LMFGain = FindClosest("LMF Gain", wavesSSLChannel.LMFGain);
			uadSSLChannel.LMFFreq = FindClosest("LMF Freq", wavesSSLChannel.LMFFrq*1000);
			uadSSLChannel.LMFQ = FindClosest("LMF Q", wavesSSLChannel.LMFQ);
			
			uadSSLChannel.HMFGain = FindClosest("HMF Gain", wavesSSLChannel.HMFGain);
			uadSSLChannel.HMFFreq = FindClosest("HMF Freq", wavesSSLChannel.HMFFrq*1000);
			uadSSLChannel.HMFQ = FindClosest("HMF Q", wavesSSLChannel.HMFQ);
			
			uadSSLChannel.HFBell = Convert.ToSingle(wavesSSLChannel.HFTypeBell);
			uadSSLChannel.HFGain = FindClosest("HF Gain", wavesSSLChannel.HFGain);
			uadSSLChannel.HFFreq = FindClosest("HF Freq", wavesSSLChannel.HFFrq*1000);

			uadSSLChannel.EQIn = Convert.ToSingle(!wavesSSLChannel.EQToBypass);
			uadSSLChannel.EQDynSC = Convert.ToSingle(wavesSSLChannel.EQToDynSC);
			
			uadSSLChannel.HPFreq = FindClosest("HP Freq", wavesSSLChannel.HPFrq);
			uadSSLChannel.LPFreq = FindClosest("LP Freq", wavesSSLChannel.LPFrq*1000);
			//wavesSSLChannel.FilterSplit;
			
			uadSSLChannel.Output = FindClosest("Output", wavesSSLChannel.Gain);
			//wavesSSLChannel.Analog;
			//wavesSSLChannel.VUShowOutput;
			uadSSLChannel.Phase = Convert.ToSingle(wavesSSLChannel.PhaseReverse);
			uadSSLChannel.Input = FindClosest("Input", wavesSSLChannel.InputTrim);
			
			return uadSSLChannel;
		}
		
		private float FindClosest(string name, float searchFor) {
			
			// find closest float value
			float foundClosest = displayNumbersDict[name].Aggregate( (x,y) => Math.Abs(x - searchFor) < Math.Abs(y - searchFor) ? x : y);
			int foundIndex = displayNumbersDict[name].IndexOf(foundClosest);
			string foundClosestDisplayText = displayTextDict[name][foundIndex];
			float foundParameterValue = valuesDict[name][foundIndex];
			
			Console.Out.WriteLine("Searching '{0}' for value {1}. Found {2} with text '{3}'. Value = {4}", name, searchFor, foundClosest, foundClosestDisplayText, foundParameterValue);
			
			return foundParameterValue;
			
			#region FindClosest Methods
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
			#endregion
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
