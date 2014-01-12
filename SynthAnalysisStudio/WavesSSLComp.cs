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
		#region Public Fields
		public float Threshold;
		public string Ratio;
		public float Attack;
		public string Release;
		public float MakeupGain;
		public float RateS;
		public bool In;
		public bool Analog;
		public string Fade;
		#endregion
		
		public WavesSSLComp()
		{
		}
		
		protected override void ReadRealWorldParameters()
		{
			if (PluginName == "SSLComp") {
				
				// <Parameters Type="RealWorld">8 1 * 3 4 3 * 1 1 1
				// 0 0 0.95000000000000006661 1 0.95000000000000006661 </Parameters>
				
				// split the parameters text into sections
				string[] splittedPhrase = RealWorldParameters.Split(' ', '\n');

				//Threshold (-15 - +15)
				Threshold = float.Parse(splittedPhrase[0], CultureInfo.InvariantCulture); // compression threshold in dB

				//Ratio (2:1=0, 4:1=1, 10:1=2)
				int ratio = int.Parse(splittedPhrase[1]);
				switch(ratio) {
					case 0:
						Ratio = "2:1";
						break;
					case 1:
						Ratio = "4:1";
						break;
					case 2:
						Ratio = "10:1";
						break;
				}
				
				// Fade [Off=0,Out=1,In=2]
				int fade = int.Parse(splittedPhrase[2]);
				switch(fade) {
					case 0:
						Fade = "Off";
						break;
					case 1:
						Fade = "Out";
						break;
					case 2:
						Fade = "In";
						break;
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
				
				// Release [0 - 4, .1 s, .3 s, .6 s, 1.2 s, Auto)
				int release = int.Parse(splittedPhrase[4]);
				switch(release) {
					case 0:
						Release = "0.1 s";
						break;
					case 1:
						Release = "0.3 s";
						break;
					case 2:
						Release = "0.6 s";
						break;
					case 3:
						Release = "1.2 s";
						break;
					case 4:
						Release = "Auto";
						break;
				}

				//Make-Up Gain (-5 - +15)
				MakeupGain = float.Parse(splittedPhrase[5], CultureInfo.InvariantCulture); // dB
				
				//*
				string Delimiter1 = splittedPhrase[6];
				
				//Rate-S (1 - +60)
				RateS = float.Parse(splittedPhrase[7], CultureInfo.InvariantCulture);
				
				//In
				In = (splittedPhrase[8] == "1");

				//Analog
				Analog = (splittedPhrase[9] == "1");
			}
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine(String.Format("PresetName: {0}", PresetName));
			sb.AppendLine();
			
			sb.AppendLine("Compression:");
			sb.AppendLine(String.Format("\tThreshold: {0:0.##} dB", Threshold));
			sb.AppendLine(String.Format("\tMake-up Gain: {0:0.##} dB", MakeupGain));
			sb.AppendLine(String.Format("\tAttack: {0:0.##} ms", Attack));
			sb.AppendLine(String.Format("\tRelease: {0}", Release));
			sb.AppendLine(String.Format("\tRatio: {0}", Ratio));
			sb.AppendLine(String.Format("\tRate-S: {0} s", RateS));
			sb.AppendLine(String.Format("\tIn: {0}", In));
			sb.AppendLine(String.Format("\tAnalog: {0}", Analog));
			sb.AppendLine(String.Format("\tFade: {0}", Fade));
			sb.AppendLine();
			
			return sb.ToString();
		}
		
	}
}
