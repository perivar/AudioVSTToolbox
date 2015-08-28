using System;
using System.Text;
using System.Globalization;

namespace SynthAnalysisStudio
{
	/// <summary>
	/// Description of WavesSSLComp.
	/// </summary>
	public class WavesSSLComp : WavesPreset
	{
		// Ratio [2:1=0, 4:1=1, 10:1=2]
		public enum RatioType {
			Ratio_2_1 = 0,
			Ratio_4_1 = 1,
			Ratio_10_1 = 2
		}

		// Fade [Off=0 or *, Out=1, In=2]
		public enum FadeType {
			Off = 0,
			Out = 1,
			In = 2
		}
		
		#region Public Fields
		public float Threshold;
		public RatioType Ratio;
		public float Attack;
		public float Release;
		public float MakeupGain;
		public float RateS;
		public bool In;
		public bool Analog;
		public FadeType Fade;
		#endregion
		
		public WavesSSLComp()
		{
		}
		
		protected override bool ReadRealWorldParameters()
		{
			if (PluginName == "SSLComp") {
				
				// <Parameters Type="RealWorld">8 1 * 3 4 3 * 1 1 1
				// 0 0 0.95000000000000006661 1 0.95000000000000006661 </Parameters>
				
				// split the parameters text into sections
				string[] splittedPhrase = RealWorldParameters.Split(' ', '\n');

				//Threshold (-15 - +15)
				Threshold = float.Parse(splittedPhrase[0], CultureInfo.InvariantCulture); // compression threshold in dB

				//Ratio (2:1=0, 4:1=1, 10:1=2)
				Ratio = (RatioType) Enum.Parse(typeof(RatioType), splittedPhrase[1]);
				
				// Fade [Off=0 or *, Out=1, In=2]
				if (splittedPhrase[2] != "*") {
					Fade = (FadeType) Enum.Parse(typeof(FadeType), splittedPhrase[2]);
				} else {
					Fade = FadeType.Off;
				}
				
				// Attack [0 - 5, .1 ms, .3 ms, 1 ms, 3 ms, 10 ms, 30 ms)
				int attack = int.Parse(splittedPhrase[3]);
				switch(attack) {
					case 0:
						Attack = 0.1f;
						break;
					case 1:
						Attack = 0.3f;
						break;
					case 2:
						Attack = 1.0f;
						break;
					case 3:
						Attack = 3.0f;
						break;
					case 4:
						Attack = 10.0f;
						break;
					case 5:
						Attack = 30.0f;
						break;
				}
				
				// Release: 0 - 4, .1 s, .3 s, .6 s, 1.2 s, Auto (-1)
				int release = int.Parse(splittedPhrase[4]);
				switch(release) {
					case 0:
						Release = 0.1f;
						break;
					case 1:
						Release = 0.3f;
						break;
					case 2:
						Release = 0.6f;
						break;
					case 3:
						Release = 1.2f;
						break;
					case 4:
						Release = -1.0f;
						break;
				}

				//Make-Up Gain (-5 - +15) dB
				MakeupGain = float.Parse(splittedPhrase[5], CultureInfo.InvariantCulture);
				
				//*
				string Delimiter1 = splittedPhrase[6];
				
				//Rate-S (1 - +60) seconds
				// Autofade duration. Variable from 1 to 60 seconds
				RateS = float.Parse(splittedPhrase[7], CultureInfo.InvariantCulture);
				
				//In
				In = (splittedPhrase[8] == "1");

				//Analog
				Analog = (splittedPhrase[9] == "1");
				
				return true;
			} else {
				return false;
			}
		}

		public override string ToString() {
			var sb = new StringBuilder();
			
			sb.AppendLine(String.Format("PresetName: {0}", PresetName));
			if (PresetGroup != null) {
				sb.AppendLine(String.Format("Group: {0}", PresetGroup));
			}
			sb.AppendLine();
			
			sb.AppendLine("Compression:");
			sb.AppendLine(String.Format("\tThreshold: {0:0.##} dB", Threshold));
			sb.AppendLine(String.Format("\tMake-up Gain: {0:0.##} dB", MakeupGain));
			sb.AppendLine(String.Format("\tAttack: {0:0.##} ms", Attack));
			if (Release == -1.0f) {
				sb.AppendLine("\tRelease: Auto");
			} else {
				sb.AppendLine(String.Format("\tRelease: {0} s", Release));
			}
			sb.AppendLine(String.Format("\tRatio: {0}", Ratio));
			sb.AppendLine(String.Format("\tRate-S (Autofade duration): {0} s", RateS));
			sb.AppendLine(String.Format("\tIn: {0}", In));
			sb.AppendLine(String.Format("\tAnalog: {0}", Analog));
			sb.AppendLine(String.Format("\tFade: {0}", Fade));
			sb.AppendLine();
			
			return sb.ToString();
		}
		
	}
}
