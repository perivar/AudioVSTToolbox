using System;
using CommonUtils;

public static class GlobalMembersSound_io
{
	public static void in_8(BinaryFile wavfile, double[][] sound, Int32 samplecount, Int32 channels)
	{
		Int32 i = new Int32();
		Int32 ic = new Int32();
		UInt16 @byte = new UInt16();

		#if DEBUG
		Console.Write("in_8...\n");
		#endif

		for (i = 0; i<samplecount; i++) {
			for (ic = 0;ic<channels;ic++)
			{
				@byte = wavfile.ReadUInt16();
				//fread(@byte, 1, 1, wavfile);
				sound[ic][i] = (double) @byte/128.0 - 1.0;
			}
		}
	}
	
	public static void out_8(BinaryFile wavfile, double[][] sound, Int32 samplecount, Int32 channels)
	{
		Int32 i = new Int32();
		Int32 ic = new Int32();
		double val;
		UInt16 @byte = new UInt16();

		#if DEBUG
		Console.Write("out_8...\n");
		#endif

		for (i = 0; i<samplecount; i++) {
			for (ic = 0;ic<channels;ic++)
			{
				val = GlobalMembersUtil.roundoff((sound[ic][i]+1.0)*128.0);
				
				if (val>255)
					val = 255;
				if (val<0)
					val = 0;

				@byte = (UInt16) val;

				wavfile.Write(@byte);
				//fwrite(@byte, sizeof(UInt16), 1, wavfile);
			}
		}
	}
	
	public static void in_16(BinaryFile wavfile, double[][] sound, Int32 samplecount, Int32 channels)
	{
		Int32 i = new Int32();
		Int32 ic = new Int32();

		#if DEBUG
		Console.Write("in_16...\n");
		#endif

		for (i = 0; i<samplecount; i++) {
			for (ic = 0; ic<channels; ic++) {
				sound[ic][i] = (double)(GlobalMembersUtil.fread_le_short(wavfile))/32768.0;
			}
		}
	}
	
	public static void out_16(BinaryFile wavfile, double[][] sound, Int32 samplecount, Int32 channels)
	{
		Int32 i = new Int32();
		Int32 ic = new Int32();
		double val;

		#if DEBUG
		Console.Write("out_16...\n");
		#endif

		for (i = 0; i<samplecount; i++) {
			for (ic = 0;ic<channels;ic++)
			{
				val = GlobalMembersUtil.roundoff(sound[ic][i]*32768.0);
				
				if (val>32767.0)
					val = 32767.0;
				if (val<-32768.0)
					val = -32768.0;

				GlobalMembersUtil.fwrite_le_short((UInt16) val, wavfile);
			}
		}
	}
	
	public static void in_32(BinaryFile wavfile, double[][] sound, Int32 samplecount, Int32 channels)
	{
		Int32 i = new Int32();
		Int32 ic = new Int32();
		float val;

		#if DEBUG
		Console.Write("in_32...\n");
		#endif

		for (i = 0;i<samplecount;i++) {
			for (ic = 0;ic<channels;ic++)
			{
				val = GlobalMembersUtil.fread_le_word(wavfile);
				sound[ic][i] = (double) val;
			}
		}
	}
	
	public static void out_32(BinaryFile wavfile, double[][] sound, Int32 samplecount, Int32 channels)
	{
		Int32 i = new Int32();
		Int32 ic = new Int32();
		float val;

		#if DEBUG
		Console.Write("out_32...\n");
		#endif

		for (i = 0; i<samplecount; i++) {
			for (ic = 0;ic<channels;ic++)
			{
				val = (float) sound[ic][i];
				GlobalMembersUtil.fwrite_le_word((UInt32) val, wavfile);
			}
		}
	}
	
	public static double[][] wav_in(BinaryFile wavfile, out Int32 channels, out Int32 samplecount, out Int32 samplerate)
	{
		double[][] sound;

		Int32 i = new Int32();
		Int32 ic = new Int32();
		Int32[] tag = new Int32[13];

		#if DEBUG
		Console.Write("wav_in...\n");
		#endif

		for (i = 0; i<13; i++) // tag reading
		{
			tag[i] = 0;

			if ((i == 5) || (i == 6) || (i == 9) || (i == 10)) {
				tag[i] = GlobalMembersUtil.fread_le_short(wavfile);
			} else {
				tag[i] = (int) GlobalMembersUtil.fread_le_word(wavfile);
			}
		}

		//********File format checking********
		if (tag[0]!=1179011410 || tag[2]!=1163280727)
		{
			Console.Error.WriteLine("This file is not in WAVE format\n");
			GlobalMembersUtil.win_return();
			Environment.Exit(1);
		}

		if (tag[3]!=544501094 || tag[4]!=16 || tag[11]!=1635017060)
		{
			Console.Error.WriteLine("This WAVE file format is not currently supported\n");
			GlobalMembersUtil.win_return();
			Environment.Exit(1);
		}
		//--------File format checking--------

		channels = tag[6];

		samplecount = tag[12]/(tag[10]/8) / channels;
		samplerate = tag[7];

		sound = new double[channels][];
		
		for (ic = 0; ic < channels; ic++) {
			sound[ic] = new double[samplecount];
		}

		//********Data loading********
		if (tag[10] ==8)
			in_8(wavfile, sound, samplecount, channels);
		if (tag[10] ==16)
			in_16(wavfile, sound, samplecount, channels);
		if (tag[10] ==32)
			in_32(wavfile, sound, samplecount, channels);
		//--------Data loading--------

		wavfile.Close();
		return sound;
	}
	
	public static void wav_out(BinaryFile wavfile, double[][] sound, Int32 channels, Int32 samplecount, Int32 samplerate, Int32 format_param)
	{
		Int32 i = new Int32();
		Int32[] tag = {1179011410, 0, 1163280727, 544501094, 16, 1, 1, 0, 0, 0, 0, 1635017060, 0, 0};

		#if DEBUG
		Console.Write("wav_out...\n");
		#endif

		//********WAV tags generation********

		tag[12] = samplecount*(format_param/8)*channels;
		tag[1] = tag[12]+36;
		tag[7] = samplerate;
		tag[8] = samplerate *format_param/8;
		tag[9] = format_param/8;
		tag[6] = channels;
		tag[10] = format_param;

		if ((format_param == 8) || (format_param == 16))
			tag[5] = 1;
		if (format_param == 32)
			tag[5] = 3;
		//--------WAV tags generation--------

		// tag writing
		for (i = 0; i<13; i++) {
			if ((i == 5) || (i == 6) || (i == 9) || (i == 10)) {
				GlobalMembersUtil.fwrite_le_short((ushort)tag[i], wavfile);
			} else {
				GlobalMembersUtil.fwrite_le_word((uint)tag[i], wavfile);
			}
		}

		if (format_param == 8)
			out_8(wavfile, sound, samplecount, channels);
		if (format_param == 16)
			out_16(wavfile, sound, samplecount, channels);
		if (format_param == 32)
			out_32(wavfile, sound, samplecount, channels);

		wavfile.Close();
	}
	
	public static Int32 wav_out_param()
	{
		Int32 bps = new Int32();

		do
		{
			Console.Write("Bits per sample (8/16/32) [16] : ");
			bps = (Int32) GlobalMembersUtil.getfloat();
			if (bps == 0 || bps<-2147483647) // The -2147483647 check is used for the sake of compatibility with C90
				bps = 16;
		}
		while (bps!=8 && bps!=16 && bps!=32);

		return bps;
	}
}
