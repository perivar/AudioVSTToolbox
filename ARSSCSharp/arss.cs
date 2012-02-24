#define DEBUG

using System;
using System.IO;
using CommonUtils;

public static class GlobalMembersArss
{
	/* The Analysis & Resynthesis Sound Spectrograph
	Copyright (C) 2005-2008 Michel Rouzic
	
	This program is free software; you can redistribute it and/or
	modify it under the terms of the GNU General Public License
	as published by the Free Software Foundation; either version 2
	of the License, or (at your option) any later version.
	
	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.
	
	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.*/

	public static string version = "0.2.3";
	public static string date = "May 29th, 2008";
	
	public const int FILENAME_MAX = 260;
	public const string MSG_NUMBER_EXPECTED = "A number is expected after {0}\nExiting with error.\n";
	
	public const double PI_D = Math.PI;
	public const int LOGBASE_D = 2;
	public const double LOOP_SIZE_SEC_D = 10.0;
	public const int BMSQ_LUT_SIZE_D = 16000;
	public const double TRANSITION_BW_SYNT = 16.0;
	
	public static void settingsinput(ref Int32 bands, ref Int32 samplecount, ref Int32 samplerate, ref double basefreq, ref double maxfreq, ref double pixpersec, ref double bandsperoctave, ref Int32 Xsize, Int32 mode)
	{
		/* mode :
		 * 0 = Analysis mode
		 * 1 = Synthesis mode
		 */
		Int32 i = new Int32();
		double gf;
		double f;
		double trash;
		double ma; // maximum allowed frequency
		//sbyte @byte;
		Int32 unset = 0; // count of unset interdependant settings
		Int32 set_min = 0;
		Int32 set_max = 0;
		Int32 set_bpo = 0;
		Int32 set_y = 0;
		Int32 set_pps = 0;
		Int32 set_x = 0;
		//uint filesize; // boolean indicating if the configuration file's last expected byte is there (to verify the file's integrity)
		//string conf_path = new string(new char[FILENAME_MAX]); // Path to the configuration file (only used on non-Windows platforms)

		#if DEBUG
		Console.Write("settingsinput...\n");
		#endif

		//freqcfg = fopen("arss.conf", "rb"); // open saved settings file
		string configFileName = "arss.conf";
		TextReader freqcfg = null;
		if (File.Exists(configFileName)) {
			freqcfg = new StreamReader(configFileName);
		}

		if (samplerate == 0) // if we're in synthesis mode and that no samplerate has been defined yet
		{
			if (GlobalMembersUtil.quiet == 1)
			{
				Console.Error.WriteLine("Please provide a sample rate for your output sound.\nUse --sample-rate (-r).\nExiting with error.\n");
				Environment.Exit(1);
			}
			//********Output settings querying********

			Console.Write("Sample rate [44100] : "); // Query for a samplerate
			samplerate = (int) GlobalMembersUtil.getfloat();
			if (samplerate == 0 || samplerate<-2147483647) // The -2147483647 check is used for the sake of compatibility with C90
				samplerate = 44100; // Default value
			//--------Output settings querying--------
		}

		if (basefreq !=0 ) // count unset interdependant frequency-domain settings
			set_min = 1;
		
		if (maxfreq !=0 )
			set_max = 1;
		
		if (bandsperoctave !=0 )
			set_bpo = 1;
		
		if (bands != 0)
			set_y = 1;
		
		unset = set_min + set_max + set_bpo + set_y;

		if (unset == 4) // if too many settings are set
		{
			if (mode == 0)
				Console.Error.WriteLine("You have set one parameter too many.\nUnset either --min-freq (-min), --max-freq (-max), --bpo (-b)\nExiting with error.\n");
			if (mode == 1)
				Console.Error.WriteLine("You have set one parameter too many.\nUnset either --min-freq (-min), --max-freq (-max), --bpo (-b) or --height (-y)\nExiting with error.\n");
			Environment.Exit(1);
		}

		if (pixpersec != 0)
			set_pps = 1;
		
		if (Xsize != 0)
			set_x = 1;

		if (set_x+set_pps == 2 && mode == 0)
		{
			Console.Error.WriteLine("You cannot define both the image width and the horizontal resolution.\nUnset either --pps (-p) or --width (-x)\nExiting with error.\n");
			Environment.Exit(1);
		}

		if (freqcfg != null) // load settings from file if it exists
		{
			if (basefreq == 0) // load values from it if they haven't been set yet
				basefreq = double.Parse(freqcfg.ReadLine());
			else
				trash = double.Parse(freqcfg.ReadLine());
			
			if (maxfreq == 0)
				maxfreq = double.Parse(freqcfg.ReadLine());
			else
				trash = double.Parse(freqcfg.ReadLine());
			
			if (bandsperoctave == 0)
				bandsperoctave = double.Parse(freqcfg.ReadLine());
			else
				trash = double.Parse(freqcfg.ReadLine());
			
			if (pixpersec == 0)
				pixpersec = double.Parse(freqcfg.ReadLine());
			else
				trash = double.Parse(freqcfg.ReadLine());
		}
		else
		{
			if (basefreq == 0) // otherwise load default values
				basefreq = 27.5;
			if (maxfreq == 0)
				maxfreq = 20000;
			if (bandsperoctave == 0)
				bandsperoctave = 12;
			if (pixpersec == 0)
				pixpersec = 150;
		}
		if (freqcfg != null)
			freqcfg.Close();

		if (unset<3 && set_min == 0)
		{
			if (GlobalMembersUtil.quiet == 1)
			{
				Console.Error.WriteLine("Please define a minimum frequency.\nUse --min-freq (-min).\nExiting with error.\n");
				Environment.Exit(1);
			}
			Console.Write("Min. frequency (Hz) [{0:f3}]: ", basefreq);
			gf = GlobalMembersUtil.getfloat();
			if (gf != 0)
				basefreq = gf;
			unset++;
			set_min = 1;
		}
		basefreq /= samplerate; // turn basefreq from Hz to fractions of samplerate

		if (unset<3 && set_bpo == 0)
		{
			if (GlobalMembersUtil.quiet == 1)
			{
				Console.Error.WriteLine("Please define a bands per octave setting.\nUse --bpo (-b).\nExiting with error.\n");
				Environment.Exit(1);
			}
			Console.Write("Bands per octave [{0:f3}]: ", bandsperoctave);
			gf = GlobalMembersUtil.getfloat();
			if (gf != 0)
				bandsperoctave = gf;
			unset++;
			set_bpo = 1;
		}

		if (unset<3 && set_max == 0)
		{
			i = 0;
			do
			{
				i++;
				f = basefreq * Math.Pow(GlobalMembersDsp.LOGBASE, (i / bandsperoctave));
			}
			while (f<0.5);
			
			ma = basefreq * Math.Pow(GlobalMembersDsp.LOGBASE, ((i-2) / bandsperoctave)) * samplerate; // max allowed frequency

			if (maxfreq > ma)
				if (GlobalMembersUtil.fmod(ma, 1.0) == 0.0)
					maxfreq = ma; // replaces the "Upper frequency limit above Nyquist frequency" warning
				else
					maxfreq = ma - GlobalMembersUtil.fmod(ma, 1.0);

			if (mode == 0) // if we're in Analysis mode
			{
				if (GlobalMembersUtil.quiet == 1)
				{
					Console.Error.WriteLine("Please define a maximum frequency.\nUse --max-freq (-max).\nExiting with error.\n");
					Environment.Exit(1);
				}
				Console.Write("Max. frequency (Hz) (up to {0:f3}) [{1:f3}]: ", ma, maxfreq);
				gf = GlobalMembersUtil.getfloat();
				if (gf != 0)
					maxfreq = gf;

				if (maxfreq > ma)
					if (GlobalMembersUtil.fmod(ma, 1.0) == 0.0)
						maxfreq = ma; // replaces the "Upper frequency limit above Nyquist frequency" warning
					else
						maxfreq = ma - GlobalMembersUtil.fmod(ma, 1.0);
			}

			unset++;
			set_max = 1;
		}

		if (set_min == 0)
		{
			basefreq = Math.Pow(GlobalMembersDsp.LOGBASE, (bands-1) / bandsperoctave) * maxfreq; // calculate the lower frequency in Hz
			Console.Write("Min. frequency : {0:f3} Hz\n", basefreq);
			basefreq /= samplerate;
		}

		if (set_max == 0)
		{
			maxfreq = Math.Pow(GlobalMembersDsp.LOGBASE, (bands-1) / bandsperoctave) * (basefreq * samplerate); // calculate the upper frequency in Hz
			Console.Write("Max. frequency : {0:f3} Hz\n", maxfreq);
		}

		if (set_y == 0)
		{
			bands = 1 + (int) GlobalMembersUtil.roundoff(bandsperoctave * (GlobalMembersUtil.log_b(maxfreq) - GlobalMembersUtil.log_b(basefreq * samplerate)));
			Console.Write("Bands : {0:D}\n", bands);
		}

		if (set_bpo == 0)
		{
			if (GlobalMembersDsp.LOGBASE == 1.0)
				bandsperoctave = maxfreq / samplerate;
			else
				bandsperoctave = (bands-1) / (GlobalMembersUtil.log_b(maxfreq) - GlobalMembersUtil.log_b(basefreq * samplerate));
			Console.Write("Bands per octave : {0:f3}\n", bandsperoctave);
		}

		if (set_x == 1 && mode == 0) // If we're in Analysis mode and that X is set (by the user)
		{
			pixpersec = (double) Xsize * (double) samplerate / (double) samplecount; // calculate pixpersec
			Console.Write("Pixels per second : {0:f3}\n", pixpersec);
		}

		if ((mode == 0 && set_x == 0 && set_pps == 0) || (mode == 1 && set_pps == 0)) // If in Analysis mode none are set or pixpersec isn't set in Synthesis mode
		{
			if (GlobalMembersUtil.quiet == 1)
			{
				Console.Error.WriteLine("Please define a pixels per second setting.\nUse --pps (-p).\nExiting with error.\n");
				Environment.Exit(1);
			}
			Console.Write("Pixels per second [{0:f3}]: ", pixpersec);
			gf = GlobalMembersUtil.getfloat();
			if (gf != 0)
				pixpersec = gf;
		}

		basefreq *= samplerate; // turn back to Hz just for the sake of writing to the file

		//freqcfg = fopen(, "wb"); // saving settings to a file
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

	public static void print_help()
	{
		Console.Write("Usage: arss [options] input_file output_file [options]. Example:\n" + "\n" + "arss -q in.bmp out.wav --noise --min-freq 55 -max 16000 --pps 100 -r 44100 -f 16\n" + "\n" + "--help, -h, /?                Display this help\n" + "--adv-help                    Display more advanced options\n" + "--version, -v                 Display the version of this program\n" + "--quiet, -q                   No-prompt mode. Useful for scripting\n" + "--analysis, -a                Analysis mode. Not req. if input file is a .wav\n" + "--sine, -s                    Sine synthesis mode\n" + "--noise, -n                   Noise synthesis mode\n" + "--min-freq, -min [real]       Minimum frequency in Hertz\n" + "--max-freq, -max [real]       Maximum frequency in Hertz\n" + "--bpo, -b [real]              Frequency resolution in Bands Per Octave\n" + "--pps, -p [real]              Time resolution in Pixels Per Second\n" + "--height, -y [integer]        Specifies the desired height of the spectrogram\n" + "--width, -x [integer]         Specifies the desired width of the spectrogram\n" + "--sample-rate, -r [integer]   Sample rate of the output sound\n" + "--brightness, -g [real]       Almost like gamma : f(x) = x^1/brightness\n" + "--format-param, -f [integer]  Output format option. This is bit-depth for WAV files (8/16/32, default: 32). No option for BMP files.\n");
	}

	public static void print_adv_help()
	{
		Console.Write("More advanced options :\n" + "\n" + "--log-base [real]          Frequency scale's logarithmic base (default: 2)\n" + "--linear, -l               Linear frequency scale. Same as --log-base 1\n" + "--loop-size [real]         Noise look-up table size in seconds (default: 10)\n" + "--bmsq-lut-size [integer]  Blackman Square kernel LUT size (default: 16000)\n" + "--pi [real]                pi (default: 3.1415926535897932)\n");
	}

	public static void Main(string[] args)
	{
		/*
		CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R_INPLACE("fftwr2r_inplace.csv", null);
		CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIBR2R("fftwr2r.csv", null);
		CommonUtils.FFT.FFTTesting.FFTWTestUsingDouble("fftw.csv", null);
		CommonUtils.FFT.FFTTesting.FFTWTestUsingDoubleFFTWLIB("fftwlib.csv", null);
		CommonUtils.FFT.FFTTesting.LomontFFTTestUsingDouble("lomont.csv", null);
		CommonUtils.FFT.FFTTesting.ExocortexFFTTestUsingComplex("exocortex.csv", null);
		CommonUtils.FFT.FFTTesting.OctaveFFTWOuput("octave.csv");
		return;
		 */
		
		int argc = args.Length;
		
		BinaryFile fin;
		BinaryFile fout;
		
		Int32 i = new Int32();
		double[][] sound;
		double[][] image;
		double basefreq = 0;
		double maxfreq = 0;
		double pixpersec = 0;
		double bpo = 0;
		double brightness = 1;
		Int32 channels = new Int32();
		Int32 samplecount = 0;
		Int32 samplerate = 0;
		Int32 Xsize = 0;
		Int32 Ysize = 0;
		Int32 format_param = 0;
		Int32 clockb = new Int32();
		byte mode = 0;
		string in_name = null;
		string out_name = null;

		// initialisation of global using defaults defined in dsp.h
		GlobalMembersDsp.pi 			= PI_D;
		GlobalMembersDsp.LOGBASE 		= LOGBASE_D;
		GlobalMembersDsp.LOOP_SIZE_SEC 	= LOOP_SIZE_SEC_D;
		GlobalMembersDsp.BMSQ_LUT_SIZE 	= BMSQ_LUT_SIZE_D;
		
		#if QUIET
		GlobalMembersUtil.quiet = 1;
		#else
		GlobalMembersUtil.quiet = 0;
		#endif

		Console.Write("The Analysis & Resynthesis Sound Spectrograph {0}\n", version);

		RandomNumbers.Seed((int)DateTime.Now.Millisecond);

		bool doHelp = false;
		for (i = 1; i<argc; i++)
		{
			if (string.Compare(args[i], "/?")==0) // DOS friendly help
			{
				doHelp = true;
			}

			if (args[i][0] != '-') // if the argument is not a function
			{
				if (in_name == null) // if the input file name hasn't been filled in yet
					in_name = args[i];
				else
					if (out_name == null) // if the input name has been filled but not the output name yet
						out_name = args[i];
					else // if both names have already been filled in
				{
					Console.Error.WriteLine("You can only have two file names as parameters.\nRemove parameter \"%s\".\nExiting with error.\n", args[i]);
					Environment.Exit(1);
				}
			}
			else // if the argument is a parameter
			{
				if (string.Compare(args[i], "--analysis")==0 || string.Compare(args[i], "-a")==0)
					mode = 1;

				if (string.Compare(args[i], "--sine")==0 || string.Compare(args[i], "-s")==0)
					mode = 2;

				if (string.Compare(args[i], "--noise")==0 || string.Compare(args[i], "-n")==0)
					mode = 3;

				if (string.Compare(args[i], "--quiet")==0 || string.Compare(args[i], "-q")==0)
					GlobalMembersUtil.quiet = 1;

				if (string.Compare(args[i], "--linear")==0 || string.Compare(args[i], "-l")==0)
					GlobalMembersDsp.LOGBASE = 1.0;

				if (string.Compare(args[i], "--sample-rate")==0 || string.Compare(args[i], "-r")==0)
					if (StringUtils.IsNumeric(args[++i]))
						samplerate = Convert.ToInt32(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--min-freq")==0 || string.Compare(args[i], "-min")==0)
					if (StringUtils.IsNumeric(args[++i]))
				{
					basefreq = Convert.ToDouble(args[i]);
					if (basefreq == 0)
						basefreq = Double.MinValue; // Set it to this extremely close-to-zero number so that it's considered set
				}
				else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--max-freq")==0 || string.Compare(args[i], "-max")==0)
					if (StringUtils.IsNumeric(args[++i]))
						maxfreq = Convert.ToDouble(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--bpo")==0 || string.Compare(args[i], "-b")==0)
					if (StringUtils.IsNumeric(args[++i]))
						bpo = Convert.ToDouble(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--pps")==0 || string.Compare(args[i], "-p")==0)
					if (StringUtils.IsNumeric(args[++i]))
						pixpersec = Convert.ToDouble(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--height")==0 || string.Compare(args[i], "-y")==0)
					if (StringUtils.IsNumeric(args[++i]))
						Ysize = Convert.ToInt32(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--width")==0 || string.Compare(args[i], "-x")==0)
					if (StringUtils.IsNumeric(args[++i]))
						Xsize = Convert.ToInt32(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--loop-size")==0)
					if (StringUtils.IsNumeric(args[++i]))
						GlobalMembersDsp.LOOP_SIZE_SEC = Convert.ToInt32(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--log-base")==0)
					if (StringUtils.IsNumeric(args[++i]))
						GlobalMembersDsp.LOGBASE = Convert.ToDouble(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--bmsq-lut-size")==0)
					if (StringUtils.IsNumeric(args[++i]))
						GlobalMembersDsp.BMSQ_LUT_SIZE = Convert.ToInt32(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--pi")==0) // lol
					if (StringUtils.IsNumeric(args[++i]))
						GlobalMembersDsp.pi = Convert.ToDouble(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--format-param")==0 || string.Compare(args[i], "-f")==0)
					if (StringUtils.IsNumeric(args[++i]))
						format_param = Convert.ToInt32(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				if (string.Compare(args[i], "--brightness")==0 || string.Compare(args[i], "-g")==0)
					if (StringUtils.IsNumeric(args[++i]))
						brightness = Convert.ToDouble(args[i]);
					else
				{
					Console.Error.WriteLine(MSG_NUMBER_EXPECTED, args[i-1]);
					Environment.Exit(1);
				}

				// TODO implement --duration, -d

				if (string.Compare(args[i], "--version")==0 || string.Compare(args[i], "-v")==0)
				{
					Console.Write("Copyright (C) 2005-2008 Michel Rouzic\nProgram last modified by its author on {0}\n", date);
					Environment.Exit(0);
				}

				if (string.Compare(args[i], "--help")==0 || string.Compare(args[i], "-h")==0)
				{
					GlobalMembersArss.print_help();
					Environment.Exit(0);
				}

				if (string.Compare(args[i], "--adv-help")==0)
				{
					GlobalMembersArss.print_adv_help();
					Environment.Exit(0);
				}
			}
		}

		if (in_name!=null) // if in_name has already been filled in
		{
			//fin = fopen(in_name, "rb"); // try to open it
			fin = new BinaryFile(in_name); // try to open it

			if (fin == null)
			{
				Console.Error.WriteLine("The input file {0} could not be found\nExiting with error.\n", in_name);
				Environment.Exit(1);
			}
			Console.Write("Input file : {0}\n", in_name);
		}
		else
		{
			if (GlobalMembersUtil.quiet == 1)
			{
				Console.Error.WriteLine("Please specify an input file.\nExiting with error.\n");
				Environment.Exit(1);
			}

			Console.Write("Type 'help' to read the manual page\n");

			do
			{
				fin = null;
				Console.Write("Input file : ");
				in_name = GlobalMembersUtil.getstring();

				if (string.Compare(in_name, "help") == 0) // if 'help' has been typed
				{
					fin = null;
					GlobalMembersArss.print_help(); // print the help
				}
				else {
					if (File.Exists(in_name)) {
						fin = new BinaryFile(in_name);
						//fin = fopen(in_name, "rb");
					}
				}
			}
			while (fin == null);
		}

		if (out_name != null) // if out_name has already been filled in
		{
			fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);
			//fout = fopen(out_name, "wb");

			if (fout == null)
			{
				Console.Error.WriteLine("The output file {0} could not be opened.\nPlease make sure it isn't opened by any other program and press Return.\nExiting with error.\n", out_name);
				Environment.Exit(1);
			}
			Console.Write("Output file : {0}\n", out_name);
		}
		else
		{
			if (GlobalMembersUtil.quiet == 1)
			{
				Console.Error.WriteLine("Please specify an output file.\nExiting with error.\n");
				Environment.Exit(1);
			}
			Console.Write("Output file : ");
			out_name = GlobalMembersUtil.getstring();

			fout = null;
			if (out_name != null && out_name != "") fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);

			while (fout == null)
			{
				Console.Write("Output file : ");
				out_name = GlobalMembersUtil.getstring();

				if (out_name != null && out_name != "") fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);
				//fout = fopen(out_name, "wb");
			}

			// we will never get here cause BinaryFile does not return a null
			while (fout == null)
			{
				Console.Error.WriteLine("The output file {0} could not be opened.\nPlease make sure it isn't opened by any other program and press Return.\n", out_name);
				Console.Read();

				fout = new BinaryFile(out_name, BinaryFile.ByteOrder.LittleEndian, true);
				//fout = fopen(out_name, "wb");
			}
		}

		// make the string lowercase
		in_name = in_name.ToLower();
		if (mode == 0 && in_name.EndsWith(".wav"))
			mode = 1; // Automatic switch to the Analysis mode

		if (mode == 0) {
			do
			{
				if (GlobalMembersUtil.quiet == 1)
				{
					Console.Error.WriteLine("Please specify an operation mode.\nUse either --analysis (-a), --sine (-s) or --noise (-n).\nExiting with error.\n");
					Environment.Exit(1);
				}
				Console.Write("Choose the mode (Press 1, 2 or 3) :\n\t1. Analysis\n\t2. Sine synthesis\n\t3. Noise synthesis\n> ");
				mode = (byte) GlobalMembersUtil.getfloat();
			}
			while (mode!=1 && mode!=2 && mode!=3);
		}


		if (mode == 1)
		{
			sound = GlobalMembersSound_io.wav_in(fin, ref channels, ref samplecount, ref samplerate); // Sound input

			#if DEBUG
			Console.Write("samplecount : {0:D}\nchannels : {1:D}\n", samplecount, channels);
			#endif

			GlobalMembersArss.settingsinput(ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref maxfreq, ref pixpersec, ref bpo, ref Xsize, 0); // User settings input
			image = GlobalMembersDsp.anal(ref sound[0], ref samplecount, ref samplerate, ref Xsize, ref Ysize, ref bpo, ref pixpersec, ref basefreq); // Analysis
			if (brightness != 1.0)
				GlobalMembersDsp.brightness_control(ref image, ref Ysize, ref Xsize, 1.0/brightness);
			
			GlobalMembersImage_io.bmp_out(fout, image, Ysize, Xsize); // Image output
		}
		
		if (mode == 2 || mode == 3)
		{
			//C++ TO C# CONVERTER TODO TASK: The memory management function 'calloc' has no equivalent in C#:
			//sound = calloc (1, sizeof(double));
			sound = new double[1][];
			
			image = GlobalMembersImage_io.bmp_in(fin, ref Ysize, ref Xsize); // Image input

			// if the output format parameter is undefined
			if (format_param == 0) {
				if (GlobalMembersUtil.quiet == 0) // if prompting is allowed
					format_param = GlobalMembersSound_io.wav_out_param();
				else
					format_param = 32; // default is 32
			}

			GlobalMembersArss.settingsinput(ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref maxfreq, ref pixpersec, ref bpo, ref Xsize, 1); // User settings input

			if (brightness!=1.0)
				GlobalMembersDsp.brightness_control(ref image, ref Ysize, ref Xsize, brightness);

			if (mode == 2) {
				sound[0] = GlobalMembersDsp.synt_sine(ref image, ref Xsize, ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref pixpersec, ref bpo); // Sine synthesis
			} else {
				sound[0] = GlobalMembersDsp.synt_noise(ref image, ref Xsize, ref Ysize, ref samplecount, ref samplerate, ref basefreq, ref pixpersec, ref bpo); // Noise synthesis
			}

			GlobalMembersSound_io.wav_out(fout, sound, 1, samplecount, samplerate, format_param);
		}

		clockb = GlobalMembersUtil.gettime();
		Console.Write("Processing time : {0:f3} s\n", (double)(clockb-GlobalMembersDsp.clocka)/1000.0);

		GlobalMembersUtil.win_return();

	}
}
