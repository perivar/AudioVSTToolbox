using System;
using System.IO;

//using AudioFormat = javax.sound.sampled.AudioFormat;
//using Encoding = javax.sound.sampled.AudioFormat.Encoding;
//using AudioInputStream = javax.sound.sampled.AudioInputStream;

namespace Wav2Zebra2CSharp
{
	public class Conversions
	{
		public const double PI = Math.PI; // 3.141593F;
		public const double TWO_PI = 2 * Math.PI; //6.283186F;
		
		public static float[] AudioInputStreamToFloat(Stream ais)
		{
			float[] floatArray = new float[1];
			/*
			int length = (int)ais.FrameLength;
			float[] floatArray = new float[length];
			int numChannels = ais.Format.Channels;
			int numBytes = ais.Format.FrameSize / numChannels;
			bool bigEndian = ais.Format.BigEndian;
			bool signed = ais.Format.Encoding == AudioFormat.Encoding.PCM_SIGNED;
			sbyte[] byteArray = new sbyte[length * numBytes * numChannels];
			try
			{
				ais.read(byteArray);
			} catch (IOException e) {
			}
			
			for (int i = 0; i < length; i++)
			{
				floatArray[i] = 0.0F;
				int j = 0;
				int step = i * numChannels;
				
				float bits = (float)Math.Pow(2.0D, numBytes * 8.0D);
				do
				{
					if ((!bigEndian) && (signed))
					{
						float floatValue = byteArray[(step * numBytes + j)];
						float multiplier = (float)Math.Pow(2.0D, j * 8.0D);
						
						floatArray[i] += floatValue * multiplier / bits;
						
					} else if ((bigEndian) && (signed))
					{
						float floatValue = byteArray[(step * numBytes + (numBytes - 1 - j))];
						float multiplier = (float)Math.Pow(2.0D, j * 8.0D);
						floatArray[i] += floatValue * multiplier / bits;
					} else if (!signed)
					{
						int intval = byteArray[(step * numBytes + j)] & 0x7F;
						intval += (byteArray[(step * numBytes + j)] & 0x80);
						float floatValue = intval / bits - 0.5F;
						floatArray[i] += floatValue;
					}
					j++;
				} while (j < numBytes);
			}
			*/
			return floatArray;
		}		

		public static float[] ReSampleToArbitrary(float[] input, int size)
		{
			float[] returnArray = new float[size];
			int length = input.Length;
			float phaseInc = (float) length / size;
			float phase = 0.0F;
			float phaseMant = 0.0F;
			
			for (int i = 0; i < size; i++)
			{
				int intPhase = (int) phase;
				int intPhasePlusOne = intPhase + 1;
				if (intPhasePlusOne >= length)
				{
					intPhasePlusOne -= length;
				}			   
				phaseMant = (float) phase - intPhase;
				returnArray[i] = (input[intPhase] * (1.0F - phaseMant) + input[intPhasePlusOne] * phaseMant);
				phase += phaseInc;
			}			 
			return returnArray;
		}
		
		public static float[] DFT(float[] waveForm)
		{
			int MaxPartials = 128;
			int partials = 128;
			int framesize = 128;
			float[] cosineComponent = new float[MaxPartials];
			float[] sineComponent = new float[MaxPartials];
			float[] amp = new float[MaxPartials];
			for (int i = 0; i < partials; i++)
			{			   
				cosineComponent[i] = 0.0F;
				sineComponent[i] = 0.0F;
			}
			
			int halfFrameSize = framesize / 2;
			float pd = (float) PI / halfFrameSize;
			for (int i = 0; i < framesize; i++)	
			{			   
				float w = waveForm[i];
				int im = i - halfFrameSize;
				for (int h = 0; h < partials; h++)	
				{				 
					float theta = pd * (h + 1) * im;
					cosineComponent[h] = (float)(cosineComponent[h] + w * Math.Cos(theta));
					sineComponent[h] = (float)(sineComponent[h] + w * Math.Sin(theta));
				}
			}			 
			
			// power spectrum
			for (int h = 0; h < partials; h++)	
			{			   
				amp[h] = ((float) Math.Sqrt(cosineComponent[h] * cosineComponent[h] + sineComponent[h] * sineComponent[h]) / halfFrameSize);
			}			 
			return amp;
		}		   
		
		public static float[] iDFT(float[] harmLevel)
		{
			float[] audioBuffer = new float[128];
			
			for (int i = 0; i < 128; i++)
			{
				audioBuffer[i] = 0.0F;		
			}
			float phaseInc = (float) (TWO_PI / 128); // 0.04908739F;
			for (int harmonic = 0; harmonic < 64; harmonic++)
			{
				float phase = 0.0F;
				
				for (int phaseStep = 0; phaseStep < 128; phaseStep++)
				{
					float test = (float) Math.Sin(phase) * harmLevel[harmonic];
					audioBuffer[phaseStep] += test;
					phase += phaseInc * (harmonic + 1);
					if (phase > TWO_PI)
					{
						phase -= (float) TWO_PI;
					}
				}
			}			 
			return audioBuffer;
		}		   
		
		public static float[] Normalize(float[] audio)
		{
			int length = audio.Length;
			float[] NormalizedAudio = new float[length];
			float maxAmplitude = 0.0F;
			for (int i = 0; i < length; i++)
			{
				if (Math.Abs(audio[i]) > maxAmplitude)
				{
					maxAmplitude = Math.Abs(audio[i]);			
				}
			}			 
			if (maxAmplitude > 0.0F)
			{
				float gain = 1.0F / maxAmplitude;
				for (int i = 0; i < length; i++)
				{
					audio[i] *= gain;		
				}
			}			 
			return NormalizedAudio;
		}
	}
}