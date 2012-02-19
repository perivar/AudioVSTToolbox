using System;
using System.IO;

namespace Wav2Zebra2Osc
{
	public class Conversions
	{
		public const double PI = Math.PI; // 3.141593F;
		public const double TWO_PI = 2 * Math.PI; //6.283186F;
		
		// TODO: see http://stackoverflow.com/questions/6876159/how-can-i-transfer-a-discrete-set-of-data-into-the-frequency-domain-and-back-pr
		// and http://technoburst.blogspot.com/2011/06/matlab-programs-07-finding-dft-and-idft.html
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