using System;
using System.IO;
using System.Linq;
using System.Drawing.Imaging; // for Bitmap Save

using CommonUtils;

namespace ARSS
{
	/// <summary>
	/// The Analysis & Resynthesis Sound Spectrograph
	/// Copyright (C) 2005-2008 Michel Rouzic
	/// 
	/// This program is free software; you can redistribute it and/or
	/// modify it under the terms of the GNU General Public License
	/// as published by the Free Software Foundation; either version 2
	/// of the License, or (at your option) any later version.
	///
	/// This program is distributed in the hope that it will be useful,
	/// but WITHOUT ANY WARRANTY; without even the implied warranty of
	/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	/// GNU General Public License for more details.
	/// 
	/// You should have received a copy of the GNU General Public License
	/// along with this program; if not, write to the Free Software
	/// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
	/// </summary>
	public static class Arss
	{
		public static string version = "0.2.3 - ported to C# by perivar@nerseth.com";
		public static string date = "May 29th, 2008 - ported to C# - 2012-2015";
		
		public const string MSG_NUMBER_EXPECTED = "A number is expected after {0}\nExiting with error.\n";
		
		public enum Mode : int {
			None = 0,
			Analysis = 1,
			Synthesis_Sine = 2,
			Synthesis_Noise = 3
		}
		
		/// <summary>
		/// Get Settings from User Input or Config file
		/// </summary>
		/// <param name="bands">Specifies the desired height of the spectrogram (bands)</param>
		/// <param name="samplecount">Number of samples</param>
		/// <param name="samplerate">Sample rate</param>
		/// <param name="basefreq">Base frequency in Hertz</param>
		/// <param name="maxfreq">Maximum frequency in Hertz</param>
		/// <param name="pixpersec">Time resolution in Pixels Per Second</param>
		/// <param name="bandsperoctave">Frequency resolution in Bands Per Octave</param>
		/// <param name="Xsize">Specifies the desired width of the spectrogram</param>
		/// <param name="mode">0 = Analysis mode, 1 = Synthesis mode</param>
		public static void SettingsInput(ref int bands, ref int samplecount, ref int samplerate, ref double basefreq, ref double maxfreq, ref double pixpersec, ref double bandsperoctave, ref int Xsize, Mode mode)
		{
			double floatUserInput;
			double trash; // used to ignore file read lines
			double maxAllowedFreq; // maximum allowed frequency
			int unset = 0; // count of unset interdependant settings
			int set_min = 0;
			int set_max = 0;
			int set_bpo = 0;
			int set_y = 0;
			int set_pps = 0;
			int set_x = 0;

			#if DEBUG
			Console.Write("SettingsInput...\n");
			#endif

			// Path to the configuration file
			string configFileName = "arss.conf";
			TextReader freqcfg = null;
			if (File.Exists(configFileName)) {
				freqcfg = new StreamReader(configFileName);
			}

			// if we're in synthesis mode and that no samplerate has been defined yet
			if (samplerate == 0)
			{
				if (Util.quiet)
				{
					Console.Error.WriteLine("Please provide a sample rate for your output sound.\nUse --sample-rate (-r).\nExiting with error.\n");
					Environment.Exit(1);
				}
				
				#region Output settings querying
				Console.Write("Sample rate [44100] : "); // Query for a samplerate
				samplerate = (int) Util.ReadUserInputFloat();
				
				if (samplerate == 0) {
					samplerate = 44100; // Default value
				}
				#endregion
			}

			// count unset interdependant frequency-domain settings
			if (basefreq !=0 ) {
				set_min = 1;
			}
			
			if (maxfreq !=0 ) {
				set_max = 1;
			}
			
			if (bandsperoctave !=0 ) {
				set_bpo = 1;
			}
			
			if (bands != 0) {
				set_y = 1;
			}
			
			unset = set_min + set_max + set_bpo + set_y;

			// if too many settings are set
			if (unset == 4)
			{
				if (mode == Mode.Analysis) {
					Console.Error.WriteLine("You have set one parameter too many.\nUnset either --min-freq (-min), --max-freq (-max), --bpo (-b)\nExiting with error.\n");
				}
				if (mode == Mode.Synthesis_Sine || mode == Mode.Synthesis_Noise) {
					Console.Error.WriteLine("You have set one parameter too many.\nUnset either --min-freq (-min), --max-freq (-max), --bpo (-b) or --height (-y)\nExiting with error.\n");
				}
				Environment.Exit(1);
			}

			if (pixpersec != 0) {
				set_pps = 1;
			}
			
			if (Xsize != 0) {
				set_x = 1;
			}

			if (set_x+set_pps == 2 && mode == Mode.Analysis)
			{
				Console.Error.WriteLine("You cannot define both the image width and the horizontal resolution.\nUnset either --pps (-p) or --width (-x)\nExiting with error.\n");
				Environment.Exit(1);
			}

			if (freqcfg != null) // load settings from file if it exists
			{
				// load values from it if they haven't been set yet
				if (basefreq == 0) {
					basefreq = double.Parse(freqcfg.ReadLine());
				} else{
					trash = double.Parse(freqcfg.ReadLine());
				}
				
				if (maxfreq == 0) {
					maxfreq = double.Parse(freqcfg.ReadLine());
				} else {
					trash = double.Parse(freqcfg.ReadLine());
				}
				
				if (bandsperoctave == 0) {
					bandsperoctave = double.Parse(freqcfg.ReadLine());
				} else {
					trash = double.Parse(freqcfg.ReadLine());
				}
				
				if (pixpersec == 0) {
					pixpersec = double.Parse(freqcfg.ReadLine());
				} else {
					trash = double.Parse(freqcfg.ReadLine());
				}
			}
			else
			{
				// otherwise load default values
				if (basefreq == 0) {
					basefreq = 27.5;
				}
				if (maxfreq == 0) {
					maxfreq = 20000;
				}
				if (bandsperoctave == 0) {
					bandsperoctave = 12;
				}
				if (pixpersec == 0) {
					pixpersec = 150;
				}
			}
			if (freqcfg != null) {
				freqcfg.Close();
			}

			if (unset<3 && set_min == 0)
			{
				if (Util.quiet)
				{
					Console.Error.WriteLine("Please define a minimum frequency.\nUse --min-freq (-min).\nExiting with error.\n");
					Environment.Exit(1);
				}
				Console.Write("Min. frequency (Hz) [{0:f3}]: ", basefreq);
				floatUserInput = Util.ReadUserInputFloat();
				if (floatUserInput != 0) {
					basefreq = floatUserInput;
				}
				unset++;
				set_min = 1;
			}
			basefreq /= samplerate; // turn basefreq from Hz to fractions of samplerate

			if (unset<3 && set_bpo == 0)
			{
				if (Util.quiet)
				{
					Console.Error.WriteLine("Please define a bands per octave setting.\nUse --bpo (-b).\nExiting with error.\n");
					Environment.Exit(1);
				}
				Console.Write("Bands per octave [{0:f3}]: ", bandsperoctave);
				floatUserInput = Util.ReadUserInputFloat();
				if (floatUserInput != 0) {
					bandsperoctave = floatUserInput;
				}
				unset++;
				set_bpo = 1;
			}

			if (unset<3 && set_max == 0)
			{
				double f = 0;
				int i = 0;
				do
				{
					i++;
					f = basefreq * Math.Pow(DSP.LOGBASE, (i / bandsperoctave));
				}
				while (f < 0.5);
				
				maxAllowedFreq = basefreq * Math.Pow(DSP.LOGBASE, ((i-2) / bandsperoctave)) * samplerate; // max allowed frequency

				if (maxfreq > maxAllowedFreq) {
					if (Util.FMod(maxAllowedFreq, 1.0) == 0.0) {
						// replaces the "Upper frequency limit above Nyquist frequency" warning
						maxfreq = maxAllowedFreq;
					} else {
						maxfreq = maxAllowedFreq - Util.FMod(maxAllowedFreq, 1.0);
					}
				}

				if (mode == Mode.Analysis) // if we're in Analysis mode
				{
					if (Util.quiet)
					{
						Console.Error.WriteLine("Please define a maximum frequency.\nUse --max-freq (-max).\nExiting with error.\n");
						Environment.Exit(1);
					}
					Console.Write("Max. frequency (Hz) (up to {0:f3}) [{1:f3}]: ", maxAllowedFreq, maxfreq);
					floatUserInput = Util.ReadUserInputFloat();
					if (floatUserInput != 0) {
						maxfreq = floatUserInput;
					}

					if (maxfreq > maxAllowedFreq) {
						// replaces the "Upper frequency limit above Nyquist frequency" warning
						if (Util.FMod(maxAllowedFreq, 1.0) == 0.0) {
							maxfreq = maxAllowedFreq;
						} else {
							maxfreq = maxAllowedFreq - Util.FMod(maxAllowedFreq, 1.0);
						}
					}
				}

				unset++;
				set_max = 1;
			}

			if (set_min == 0)
			{
				basefreq = Math.Pow(DSP.LOGBASE, (bands-1) / bandsperoctave) * maxfreq; // calculate the lower frequency in Hz
				Console.Write("Min. frequency : {0:f3} Hz\n", basefreq);
				basefreq /= samplerate;
			}

			if (set_max == 0)
			{
				maxfreq = Math.Pow(DSP.LOGBASE, (bands-1) / bandsperoctave) * (basefreq * samplerate); // calculate the upper frequency in Hz
				Console.Write("Max. frequency : {0:f3} Hz\n", maxfreq);
			}

			if (set_y == 0)
			{
				bands = 1 + Util.RoundToClosestInt(bandsperoctave * (Util.Log(maxfreq) - Util.Log(basefreq * samplerate)));
				Console.Write("Bands : {0:D}\n", bands);
			}

			if (set_bpo == 0)
			{
				if (DSP.LOGBASE == 1.0) {
					bandsperoctave = maxfreq / samplerate;
				} else {
					bandsperoctave = (bands-1) / (Util.Log(maxfreq) - Util.Log(basefreq * samplerate));
				}
				Console.Write("Bands per octave : {0:f3}\n", bandsperoctave);
			}

			if (set_x == 1 && mode == Mode.Analysis) // If we're in Analysis mode and that X is set (by the user)
			{
				pixpersec = (double) Xsize * (double) samplerate / (double) samplecount; // calculate pixpersec
				Console.Write("Pixels per second : {0:f3}\n", pixpersec);
			}

			// If in Analysis mode none are set or pixpersec isn't set in Synthesis mode
			if ((mode == Mode.Analysis && set_x == 0 && set_pps == 0)
			    || (set_pps == 0 && (mode == Mode.Synthesis_Sine || mode == Mode.Synthesis_Noise)))
			{
				if (Util.quiet)
				{
					Console.Error.WriteLine("Please define a pixels per second setting.\nUse --pps (-p).\nExiting with error.\n");
					Environment.Exit(1);
				}
				Console.Write("Pixels per second [{0:f3}]: ", pixpersec);
				floatUserInput = Util.ReadUserInputFloat();
				if (floatUserInput != 0) {
					pixpersec = floatUserInput;
				}
			}

			basefreq *= samplerate; // turn back to Hz just for the sake of writing to the file

			TextWriter freqcfgOut = new StreamWriter(configFileName); // saving settings to a file

			if (freqcfgOut == null)
			{
				Console.Error.WriteLine("Cannot write to configuration file");
				Environment.Exit(1);
			}
			freqcfgOut.WriteLine(basefreq);
			freqcfgOut.WriteLine(maxfreq);
			freqcfgOut.WriteLine(bandsperoctave);
			freqcfgOut.WriteLine(pixpersec);
			freqcfgOut.Close();

			basefreq /= samplerate; // basefreq is now in fraction of the sampling rate instead of Hz
			pixpersec /= samplerate; // pixpersec is now in fraction of the sampling rate instead of Hz
		}

		#region Print Help
		public static void PrintHelp()
		{
			Console.Write("Usage: arss [options] input_file output_file [options]. Example:\n"
			              + "\n"
			              + "arss -q in.bmp out.wav --noise --min-freq 55 -max 16000 --pps 100 -r 44100 -f 16\n"
			              + "\n"
			              + "--help, -h, /?                Display this help\n"
			              + "--adv-help                    Display more advanced options\n"
			              + "--version, -v                 Display the version of this program\n"
			              + "--quiet, -q                   No-prompt mode. Useful for scripting\n"
			              + "--analysis, -a                Analysis mode. Not req. if input file is a .wav\n"
			              + "--sine, -s                    Sine synthesis mode\n"
			              + "--noise, -n                   Noise synthesis mode\n"
			              + "--min-freq, -min [real]       Minimum frequency in Hertz\n"
			              + "--max-freq, -max [real]       Maximum frequency in Hertz\n"
			              + "--bpo, -b [real]              Frequency resolution in Bands Per Octave\n"
			              + "--pps, -p [real]              Time resolution in Pixels Per Second\n"
			              + "--height, -y [integer]        Specifies the desired height of the spectrogram\n"
			              + "--width, -x [integer]         Specifies the desired width of the spectrogram\n"
			              + "--sample-rate, -r [integer]   Sample rate of the output sound\n"
			              + "--brightness, -g [real]       Almost like gamma : f(x) = x^1/brightness\n"
			              + "--format-param, -f [integer]  Output format option. This is bit-depth for WAV files (8/16/32, default: 32). No option for BMP files.\n");
		}

		public static void PrintAdvancedHelp()
		{
			Console.Write("More advanced options :\n"
			              + "\n"
			              + "--log-base [real]          Frequency scale's logarithmic base (default: 2)\n"
			              + "--linear, -l               Linear frequency scale. Same as --log-base 1\n"
			              + "--loop-size [real]         Noise look-up table size in seconds (default: 10)\n"
			              + "--bmsq-lut-size [integer]  Blackman Square kernel LUT size (default: 16000)\n"
			              + "--pi [real]                pi (default: 3.1415926535897932)\n");
		}
		#endregion
		
		public static void Main(string[] args)
		{
			int argc = args.Length;
			
			BinaryFile fin;
			BinaryFile fout;
			
			double[][] sound;
			double[][] image;
			double basefreq = 0;
			double maxfreq = 0;
			double pixpersec = 0;
			double bpo = 0;
			double brightness = 1;
			int channels = 0;
			int samplecount = 0;
			int samplerate = 0;
			int Xsize = 0;
			int Ysize = 0;
			int format_param = 0;
			long clockb = 0;
			Mode mode = Mode.None;
			string in_name = null;
			string out_name = null;
			
			#if QUIET
			Util.quiet = true;
			#else
			Util.quiet = false;
			#endif

			Console.Write("The Analysis & Resynthesis Sound Spectrograph {0}\n", version);

			// initialize the random class
			RandomUtils.Seed(Guid.NewGuid().GetHashCode());

			bool doHelp = false;
			for (int i = 0; i < argc; i++) {
				
				// DOS friendly help
				if (string.Compare(args[i], "/?")==0) {
					doHelp = true;
				}

				// if the argument is not a function
				if (args[i][0] != '-') {
					if (in_name == null) {
						// if the input file name hasn't been filled in yet
						in_name = args[i];
					} else if (out_name == null) {
						// if the input name has been filled but not the output name yet
						out_name = args[i];
					} else {
						// if both names have already been filled in
						Console.Error.WriteLine("You can only have two file names as parameters.\nRemove parameter \"%s\".\nExiting with error.\n", args[i]);
						Environment.Exit(1);
					}
				} else {
					// if the argument is a parameter
					
					if (string.Compare(args[i], "--analysis")==0 || string.Compare(args[i], "-a")==0) {
						mode = Mode.Analysis;
					}

					if (string.Compare(args[i], "--sine")==0 || string.Compare(args[i], "-s")==0) {
						mode = Mode.Synthesis_Sine;
					}

					if (string.Compare(args[i], "--noise")==0 || string.Compare(args[i], "-n")==0) {
						mode = Mode.Synthesis_Noise;
					}

					if (string.Compare(args[i], "--quiet")==0 || string.Compare(args[i], "-q")==0) {
						Util.quiet = true;
					}

					if (string.Compare(args[i], "--linear")==0 || string.Compare(args[i], "-l")==0) {
						DSP.LOGBASE = 1.0;
					}

					if (string.Compare(args[i], "--sample-rate")==0 || string.Compare(args[i], "-r")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							samplerate = Convert.ToInt32(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--min-freq")==0 || string.Compare(args[i], "-min")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							basefreq = Convert.ToDouble(args[i]);
							if (basefreq == 0) {
								// Set it to this extremely close-to-zero number so that it's considered set
								basefreq = Double.MinValue;
							}
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--max-freq")==0 || string.Compare(args[i], "-max")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							maxfreq = Convert.ToDouble(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--bpo")==0 || string.Compare(args[i], "-b")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							bpo = Convert.ToDouble(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--pps")==0 || string.Compare(args[i], "-p")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							pixpersec = Convert.ToDouble(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--height")==0 || string.Compare(args[i], "-y")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							Ysize = Convert.ToInt32(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--width")==0 || string.Compare(args[i], "-x")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							Xsize = Convert.ToInt32(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--loop-size")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							DSP.LOOP_SIZE_SEC = Convert.ToInt32(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--log-base")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							DSP.LOGBASE = Convert.ToDouble(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--bmsq-lut-size")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							DSP.BMSQ_LUT_SIZE = Convert.ToInt32(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--pi")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							DSP.PI = Convert.ToDouble(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--format-param")==0 || string.Compare(args[i], "-f")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							format_param = Convert.ToInt32(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					if (string.Compare(args[i], "--brightness")==0 || string.Compare(args[i], "-g")==0) {
						if (StringUtils.IsNumeric(args[++i])) {
							brightness = Convert.ToDouble(args[i]);
						}
					} else {
						Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
						Environment.Exit(1);
					}

					// TODO implement --duration, -d

					if (string.Compare(args[i], "--version")==0 || string.Compare(args[i], "-v")==0) {
						Console.Write("Copyright (C) 2005-2008 Michel Rouzic\nProgram last modified by its author on {0}\n", date);
						Environment.Exit(0);
					}

					if (doHelp || string.Compare(args[i], "--help")==0 || string.Compare(args[i], "-h")==0) {
						PrintHelp();
						Environment.Exit(0);
					}

					if (string.Compare(args[i], "--adv-help")==0) {
						PrintAdvancedHelp();
						Environment.Exit(0);
					}
				}
			}

			// if in_name has already been filled in
			if (in_name != null) {
				// try to open it
				fin = new BinaryFile(in_name);
				if (fin == null) {
					Console.Error.WriteLine("The input file {0} could not be found\nExiting with error.\n", in_name);
					Environment.Exit(1);
				}
				Console.Write("Input file : {0}\n", in_name);
			} else {
				if (Util.quiet) {
					Console.Error.WriteLine("Please specify an input file.\nExiting with error.\n");
					Environment.Exit(1);
				}

				Console.Write("Type 'help' to read the manual page\n");

				do {
					fin = null;
					Console.Write("Input file : ");
					in_name = Util.ReadUserInputString();

					// if 'help' has been typed
					if (string.Compare(in_name, "help", StringComparison.Ordinal) == 0) {
						fin = null;
						PrintHelp(); // print the help
					} else {
						if (File.Exists(in_name)) {
							fin = new BinaryFile(in_name);
						}
					}
				}
				while (fin == null);
			}

			// if out_name has already been filled in
			if (out_name != null) {
				fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);
				if (fout == null) {
					Console.Error.WriteLine("The output file {0} could not be opened.\nPlease make sure it isn't opened by any other program and press Return.\nExiting with error.\n", out_name);
					Environment.Exit(1);
				}
				Console.Write("Output file : {0}\n", out_name);
			}
			else {
				if (Util.quiet) {
					Console.Error.WriteLine("Please specify an output file.\nExiting with error.\n");
					Environment.Exit(1);
				}
				Console.Write("Output file : ");
				out_name = Util.ReadUserInputString();

				fout = null;
				if (!string.IsNullOrEmpty(out_name)) {
					fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);
				}

				while (fout == null) {
					Console.Write("Output file : ");
					out_name = Util.ReadUserInputString();

					if (!string.IsNullOrEmpty(out_name)) {
						fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);
					}
				}

				// we will never get here cause BinaryFile does not return a null
				while (fout == null) {
					Console.Error.WriteLine("The output file {0} could not be opened.\nPlease make sure it isn't opened by any other program and press Return.\n", out_name);
					Console.Read();
					
					fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);
				}
			}

			// make the string lowercase
			in_name = in_name.ToLower();
			if (in_name.EndsWith(".wav") &&
			    (mode == Mode.None || mode == Mode.Synthesis_Sine || mode == Mode.Synthesis_Noise)) {
				mode = Mode.Analysis; // Automatic switch to the Analysis mode
			}

			if (mode == Mode.None) {
				do {
					if (Util.quiet) {
						Console.Error.WriteLine("Please specify an operation mode.\nUse either --analysis (-a), --sine (-s) or --noise (-n).\nExiting with error.\n");
						Environment.Exit(1);
					}
					Console.Write("Choose the mode (Press 1, 2 or 3) :\n\t1. Analysis\n\t2. Sine synthesis\n\t3. Noise synthesis\n> ");
					mode = (Mode) Util.ReadUserInputFloat();
				}
				while (mode != Mode.Analysis && mode != Mode.Synthesis_Sine && mode != Mode.Synthesis_Noise);
			}

			if (mode == Mode.Analysis) {
				sound = SoundIO.ReadWaveFile(fin, ref channels, ref samplecount, ref samplerate); // Sound input
				fin.Close();

				#if DEBUG
				Console.Write("Samplecount : {0:D}\nChannels : {1:D}\n", samplecount, channels);
				#endif
				
				SettingsInput(ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref maxfreq, ref pixpersec, ref bpo, ref Xsize, Mode.Analysis); // User settings input
				
				image = DSP.Analyze(ref sound[0], ref samplecount, ref samplerate, ref Xsize, ref Ysize, ref bpo, ref pixpersec, ref basefreq); // Analysis
				if (brightness != 1.0) {
					DSP.BrightnessControl(ref image, ref Ysize, ref Xsize, 1.0/brightness);
				}
				
				ImageIO.BMPWrite(fout, image, Ysize, Xsize); // Image output
				
				
				/*
				// Testing using the Spectrogram methods to synthesize and resynthesize
				var spectrogram = new ArssSpectrogram.Spectrogram();
				var spectrogramImage = spectrogram.ToImage(ref sound[0], samplerate);
				spectrogramImage.Save("test2.bmp", ImageFormat.Bmp);
				return;
				//double[] soundSynth = spectrogram.SynthetizeSine(new System.Drawing.Bitmap("test2.bmp"), samplerate);
				var testImage = new BinaryFile("test2.bmp");
				int testImageX = 0;
				int testImageY = 0;
				var testImageArray = ImageIO.BMPRead(testImage, ref testImageY, ref testImageX); // Image input
				double[] soundSynth2 = spectrogram.SynthetizeSine(testImageArray, samplerate);
				CommonUtils.Audio.NAudio.AudioUtilsNAudio.WriteIEEE32WaveFileMono("test2.wav", samplerate, MathUtils.DoubleToFloat(soundSynth2));
				 */
			}
			
			if (mode == Mode.Synthesis_Sine || mode == Mode.Synthesis_Noise) {
				sound = new double[1][];
				
				image = ImageIO.BMPRead(fin, ref Ysize, ref Xsize); // Image input

				// if the output format parameter is undefined
				if (format_param == 0) {
					if (!Util.quiet) {
						// if prompting is allowed
						format_param = SoundIO.ReadUserWaveOutParameters();
					} else {
						format_param = 32; // default is 32
					}
				}

				SettingsInput(ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref maxfreq, ref pixpersec, ref bpo, ref Xsize, Mode.Synthesis_Sine); // User settings input

				if (brightness!=1.0) {
					DSP.BrightnessControl(ref image, ref Ysize, ref Xsize, brightness);
				}

				if (mode == Mode.Synthesis_Sine) {
					sound[0] = DSP.SynthesizeSine(ref image, ref Xsize, ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref pixpersec, ref bpo); // Sine synthesis
				} else {
					sound[0] = DSP.SynthesizeNoise(ref image, ref Xsize, ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref pixpersec, ref bpo); // Noise synthesis
				}

				SoundIO.WriteWaveFile(fout, sound, 1, samplecount, samplerate, format_param);
			}

			clockb = Util.GetTimeTicks();
			TimeSpan duration = TimeSpan.FromMilliseconds(clockb-DSP.clockA);
			Console.Write("Processing time : {0:D2} m  {1:D2} s  {2:D2} ms\n", duration.Minutes, duration.Seconds, duration.Milliseconds);

			Util.ReadUserReturn();
		}
	}
}