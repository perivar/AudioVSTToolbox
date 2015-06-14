using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

using CommonUtils.FFT;

public enum SynthesisType {
	SYNTHESIS_SINE,
	SYNTHESIS_NOISE
}

public enum Window {
	WINDOW_HANN,
	WINDOW_BLACKMAN,
	WINDOW_TRIANGULAR,
	WINDOW_RECTANGULAR
}

public enum AxisScale {
	SCALE_LOGARITHMIC,
	SCALE_LINEAR
}

public enum BrightCorrection	{
	BRIGHT_NONE,
	BRIGHT_SQRT
}

public class Pair<T, U> {
	public Pair() {
	}

	public Pair(T first, U second) {
		this.First = first;
		this.Second = second;
	}

	public T First { get; set; }
	public U Second { get; set; }
};

// Converted from C++ to C#
// https://github.com/krajj7/spectrogram/blob/master/spectrogram.cpp
public class Spectrogram
{
	private int bandwidth;
	private int basefreq;
	private int maxfreq;
	private double overlap;
	private int pixpersec;
	private Window window;
	private AxisScale intensity_axis;
	private AxisScale frequency_axis;
	//private bool cancelled;
	private Palette palette;

	public Spectrogram()
	{
		bandwidth = 300; // 100
		basefreq = 55;
		maxfreq = 22050;
		overlap = 0.8;
		pixpersec = 150; // 100
		window = Window.WINDOW_HANN;
		intensity_axis = AxisScale.SCALE_LOGARITHMIC;
		frequency_axis = AxisScale.SCALE_LOGARITHMIC;
		//cancelled = false;
		palette = new Palette();
	}
	
	public Bitmap ToImage(ref double[] signal, int samplerate)
	{
		Console.Out.WriteLine("Transforming input");
		
		Complex[] spectrum = SpectrogramUtils.PaddedFFT(signal);

		//const size_t width = (spectrum.size()-1)*2*pixpersec/samplerate;
		double w1 = spectrum.Length - 1; // last spectrum index
		double w2 = (double) pixpersec/ (double) samplerate;
		double w3 = w1 * 2 * w2;
		int width = (int) w3;

		// transformation of frequency in hz to index in spectrum
		double filterscale = ((double)spectrum.Length*2) / samplerate;
		Console.Out.WriteLine("Filterscale: {0}", filterscale);

		Filterbank filterbank = Filterbank.GetFilterbank(frequency_axis, filterscale, basefreq, bandwidth, overlap);
		int bands = (int) filterbank.NumBandsEst(maxfreq);
		int top_index = (int) ((double)maxfreq * filterscale);
		// todo: use spectrum.length - 1
		
		// maxfreq has to be at most nyquist
		Debug.Assert(top_index <= spectrum.Length);

		//std::vector<real_vec> image_data;
		var image_data = new List<double[]>();
		
		for (int bandidx = 0;; ++bandidx) {
			// TODO: support cancelling this process
			
			// filtering
			Pair<int,int> range = filterbank.GetBand(bandidx);
			
			// Output progress
			//OutputBandProgress(bandidx, bands);
			Console.Out.WriteLine("Processing band {0} of {1} ({2:0.00}hz-{3:0.00}hz = {4:0.00}hz)", bandidx, bands, range.First / filterscale, range.Second/filterscale, (range.Second-range.First)/filterscale);

			/*
			Console.Out.WriteLine("-----");
			Console.Out.WriteLine("spectrum size: {0}", spectrum.Length);
			Console.Out.WriteLine("lowidx: {0:0.00} highidx: {1:0.00}", range.First, range.Second);
			Console.Out.WriteLine("(real)lowfreq: {0:0.00} (real)highfreq: {1:0.00}", range.First / filterscale, range.Second/filterscale);
			Console.Out.WriteLine("actual width: {0:0.00} hz", (range.Second-range.First)/filterscale);
			Console.Out.WriteLine("vertical values: {0:0.00}", (range.Second-range.First));
			Console.Out.WriteLine("crowd sample: {0:0.00}", (range.Second-range.First-1)*2);
			Console.Out.WriteLine("theoretically staci: {0:0.00} hz samplerate", 2*(range.Second-range.First)/filterscale);
			Console.Out.WriteLine("width: {0}", width);
			 */
			
			// take the complex samples	specified by the range and copy into filterband
			int filterbandLength = range.Second - range.First;
			var filterband = new Complex[filterbandLength];
			//std::copy(spectrum.begin()+range.first,
			//          spectrum.begin()+std::min(range.second, top_index),
			//          filterband.begin());
			int sourceIndexStart = range.First;
			int sourceIndexEnd = Math.Min(range.Second, top_index);
			int length = sourceIndexEnd - sourceIndexStart;
			if (sourceIndexStart < sourceIndexEnd) {
				Array.Copy(spectrum, sourceIndexStart, filterband, 0, length);
			} else {
				int stopme = 1; // test
			}
			
			// if the first range index is higher than the maximum index, we have reached the end
			if (range.First > top_index) {
				break;
			}
			// if the second range index is higher than the maximum index, we are at the last band
			if (range.Second > top_index) {
				//std::fill(filterband.begin()+top_index-range.first,
				//          filterband.end(), Complex(0,0));
				// not neccesary as c# already defaults the rest of the array to a Complex(0,0)
			}

			// windowing
			ApplyWindow(ref filterband, range.First, filterscale);

			// envelope detection
			double[] envelope = SpectrogramUtils.GetEnvelope(ref filterband);
			
			// resampling
			// todo: the widh cannot be right?
			//envelope = SpectrogramUtils.Resample(envelope, width);
			
			image_data.Add(envelope);
		}

		SpectrogramUtils.NormalizeImageCutoffNegative(ref image_data);

		return MakeImage(image_data);
	}
	
	public Bitmap MakeImage(List<double[]> data)
	{
		Console.Out.WriteLine("Generating image");
		
		int height = data.Count;
		int width = data[0].Length;
		
		Console.Write("Image size: ");
		Console.Write(width);
		Console.Write(" x ");
		Console.Write(height);
		Console.Write("\n");
		
		var @out = new Bitmap(width, height);
		
		BrightCorrection correction = BrightCorrection.BRIGHT_NONE;
		for (int y = 0; y < height; ++y) {
			Debug.Assert(data[y].Length == width);
			
			for (int x = 0; x < width; ++x) {
				double intensity = SpectrogramUtils.CalcIntensity(data[y][x], intensity_axis);
				intensity = SpectrogramUtils.BrightnessCorrection(intensity, correction);
				@out.SetPixel(x, (int)(height-1-y), palette.GetColor(intensity));
			}
		}
		
		//@out.SetText("Spectrogram", serialized()); // save parameters
		return @out;
	}
	
	public void ApplyWindow(ref Complex[] chunk, int lowidx, double filterscale)
	{
		int highidx = lowidx + chunk.Length;
		if (frequency_axis == AxisScale.SCALE_LINEAR) {
			for (int i = 0; i < chunk.Length; ++i) {
				chunk[i] *= SpectrogramUtils.WindowCoef((double)i/(chunk.Length-1), window);
			}
		} else {
			double rloglow = SpectrogramUtils.Freq2Cent(lowidx/filterscale); // after rounding
			double rloghigh = SpectrogramUtils.Freq2Cent((highidx-1)/filterscale);
			for (int i = 0; i < chunk.Length; ++i) {
				double logidx = SpectrogramUtils.Freq2Cent((lowidx+i)/filterscale);
				double winidx = (logidx - rloglow)/(rloghigh - rloglow);
				chunk[i] *= SpectrogramUtils.WindowCoef(winidx, window);
			}
		}
	}
	
	public double[] Synthetize(Bitmap image, int samplerate, SynthesisType type)
	{
		switch (type) {
			case SynthesisType.SYNTHESIS_SINE:
				return SynthetizeSine(image, samplerate);
			case SynthesisType.SYNTHESIS_NOISE:
				return SynthetizeNoise(image, samplerate);
		}
		Debug.Assert(false);
		return null;
	}
	
	public double[] SynthetizeSine(Bitmap image, int samplerate)
	{
		int samples = image.Width * samplerate/pixpersec;
		var spectrum = new Complex[samples/2+1];

		double filterscale = ((double)spectrum.Length*2)/samplerate;

		Filterbank filterbank = Filterbank.GetFilterbank(frequency_axis, filterscale, basefreq, bandwidth, overlap);

		for (int bandidx = 0; bandidx < image.Height; ++bandidx) {
			//if (cancelled())
			//	return List<int>();
			OutputBandProgress(bandidx, image.Height-1);

			double[] envelope = EnvelopeFromSpectrogram(image, bandidx);

			// random phase between +-pi
			double phase = (2 * SpectrogramUtils.RandomDouble() - 1) * Math.PI;

			var bandsignal = new double[envelope.Length*2];
			for (int j = 0; j < 4; ++j) {
				double sine = Math.Cos(j * Math.PI/2 + phase);
				for (int i = j; i < bandsignal.Length; i += 4)
					bandsignal[i] = envelope[i/2] * sine;
			}
			
			var filterband = SpectrogramUtils.PaddedFFT(bandsignal);

			for (int i = 0; i < filterband.Length; ++i) {
				double x = (double)i/filterband.Length;
				
				// normalized blackman window antiderivative
				filterband[i] *= x - ((0.5/(2.0 * Math.PI)) * Math.Sin(2.0 * Math.PI *x) + (0.08/(4.0 * Math.PI)) * Math.Sin(4.0 * Math.PI *x) / 0.42);
			}

			Console.Out.WriteLine("spectrum size: {0}", spectrum.Length);
			//std::cout << bandidx << ". filterband size: " << filterband.Length << "; start: " << filterbank->GetBand(bandidx).first <<"; end: " << filterbank->GetBand(bandidx).second << "\n";

			double center = filterbank.GetCenter(bandidx);
			double offset = Math.Max((uint)0, center - filterband.Length/2);
			
			Console.Out.WriteLine("offset: {0} = {1} hz", offset, offset/filterscale);
			
			for (uint i = 0; i < filterband.Length; ++i) {
				if (offset+i > 0 && offset+i < spectrum.Length) {
					spectrum[ (int) (offset+i) ] += filterband[i];
				}
			}
		}

		double[] @out = SpectrogramUtils.PaddedIFFT(spectrum);
		
		Console.Out.WriteLine("samples: {0} -> {1}", @out.Length, samples);
		
		SpectrogramUtils.NormalizeSignal(ref @out);
		return @out;
	}
	
	public double[] SynthetizeNoise(Bitmap image, int samplerate)
	{
		int samples = (int) image.Width * samplerate/pixpersec;

		Complex[] noise = SpectrogramUtils.GetPinkNoise(samplerate *10); // 10 sec loop

		double filterscale = ((double)noise.Length*2)/samplerate;
		Filterbank filterbank = Filterbank.GetFilterbank(frequency_axis, filterscale, basefreq, bandwidth, overlap);

		int top_index = (int) (maxfreq * filterscale);

		var @out = new double[samples];

		for (int bandidx = 0; bandidx < image.Height; ++bandidx) {
			//if (cancelled())
			//	return List<int>();
			OutputBandProgress(bandidx, image.Height-1);

			// filter noise
			Pair<int,int> range = filterbank.GetBand(bandidx);
			//std::cout << bandidx << "/"<<image.height()<<"\n";
			Console.Out.WriteLine("(noise) sample: {0}", range.Second-range.First);

			var filtered_noise = new double[noise.Length];
			//std.copy(noise.begin()+range.first, noise.begin()+Math.Min(range.second, top_index), filtered_noise.begin()+range.first);
			
			// ifft noise
			double[] noise_mod = SpectrogramUtils.PaddedIFFT(filtered_noise);
			
			// resample spectrogram band
			double[] envelope = SpectrogramUtils.Resample(EnvelopeFromSpectrogram(image, bandidx), samples);
			
			// modulate with looped noise
			for (uint i = 0; i < samples; ++i)
				@out[i] += envelope[i] * noise_mod[i % noise_mod.Length];
		}
		
		SpectrogramUtils.NormalizeSignal(ref @out);
		return @out;
	}
	
	public static void OutputBandProgress(int x, int of)
	{
		Console.Out.WriteLine("Processing band {0} of {1}", x, of);
	}
	
	public double[] EnvelopeFromSpectrogram(Bitmap image, int row)
	{
		var envelope = new double[image.Width];
		for (int x = 0; x < image.Width; ++x)
			envelope[x] = SpectrogramUtils.CalcIntensityInv(
				palette.GetIntensity(image.GetPixel(x, image.Height-row-1)), intensity_axis);
		return envelope;
	}
	
	public void Deserialize(String text)
	{
		/*
		char delimiter = ',';
		string[] tokens = text.Split(delimiter);
		bandwidth = tokens[1].toDouble();
		basefreq = tokens[2].toDouble();
		maxfreq = tokens[3].toDouble();
		overlap = tokens[4].toDouble()/100.0;
		pixpersec = tokens[5].toDouble();
		window = (Window)tokens[6].toInt();
		intensity_axis = (AxisScale)tokens[7].toInt();
		frequency_axis = (AxisScale)tokens[8].toInt();
		 */
	}
	
	public String Serialized()
	{
		/*
		String @out = new String();
		QTextStream desc = new QTextStream(@out);
		desc.setRealNumberPrecision(4);
		desc.setRealNumberNotation(QTextStream.FixedNotation);
		desc << "Spectrogram:" << delimiter << bandwidth << delimiter << basefreq << delimiter << maxfreq << delimiter << overlap *100 << delimiter << pixpersec << delimiter << (int)window << delimiter << (int)intensity_axis << delimiter << (int)frequency_axis << delimiter;
		Console.Out.WriteLine("serialized: " << out.toStdString() << "\n";
		return @out;
		 */
		return "Serialized string";
	}
}
