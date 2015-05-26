using System;

namespace Wav2Zebra2Osc
{
	// Originally developed by Jupiter8
	// https://sites.google.com/site/wav2zebra2/
	public static class Conversions
	{
		public const double PI = Math.PI;
		public const double TWO_PI = 2 * Math.PI;
		
		// TODO: see http://stackoverflow.com/questions/6876159/how-can-i-transfer-a-discrete-set-of-data-into-the-frequency-domain-and-back-pr
		// and http://technoburst.blogspot.com/2011/06/matlab-programs-07-finding-dft-and-idft.html
		public static float[] DFT(float[] waveForm)
		{
			const int MAX_PARTIALS = 128;
			const int partials = 128;
			const int framesize = 128;
			const int halfFrameSize = framesize / 2;
			const float pd = (float)PI / halfFrameSize;
			
			var cosineComponent = new float[MAX_PARTIALS];
			var sineComponent = new float[MAX_PARTIALS];
			var amp = new float[MAX_PARTIALS];
			
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
			var audioBuffer = new float[128];

			const float phaseInc = (float)(TWO_PI / 128);

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
			var normalizedAudio = new float[length];
			float maxAmplitude = 0.0F;
			for (int i = 0; i < length; i++)
			{
				if (Math.Abs(audio[i]) > maxAmplitude)
				{
					maxAmplitude = Math.Abs(audio[i]);
				}
			}
			if (maxAmplitude > 0.0F) // Don't normalize zero amplitude arrays.  Divide by zero is bad.
			{
				float gain = 1.0F / maxAmplitude;
				for (int i = 0; i < length; i++)
				{
					normalizedAudio[i] = audio[i] * gain;
				}
			}
			return normalizedAudio;
		}
	}
}