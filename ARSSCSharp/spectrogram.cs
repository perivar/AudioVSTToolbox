using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using CommonUtils;
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

public static class GlobalMembersSpectrogram
{
	public static double[] zeros(double[] signal, int length) {
		if (length > signal.Length) {
			Array.Resize<double>(ref signal, length);
		}
		return signal;
	}

	public static Complex[] zeros(Complex[] signal, int length) {
		if (length > signal.Length) {
			Array.Resize<Complex>(ref signal, length);
		}
		return signal;
	}

	public static double[] padded_IFFT(Complex[] complexSignal) {
		
		int N = MathUtils.NextPowerOfTwo(complexSignal.Length);
		if (N <= 4096) {
			complexSignal = zeros(complexSignal, N);
			Fourier.FFT(complexSignal, N, FourierDirection.Backward);
		} else {
			N = 4096;
			Array.Resize(ref complexSignal, N);
			Fourier.FFT(complexSignal, N, FourierDirection.Backward);
		}
		
		// get the result
		double[] fft_real = new double[N];
		for (int j = 0; j < N; j++) {
			fft_real[j] = complexSignal[j].Re;
		}
		return fft_real;
	}
	
	public static double[] padded_IFFT(double[] signal) {
		
		int N = signal.Length;
		Complex[] complexSignal = FFTUtils.DoubleToComplex(signal);
		Fourier.FFT(complexSignal, N, FourierDirection.Backward);
		
		// get the result
		double[] fft_real = new double[N];
		for (int j = 0; j < N; j++) {
			fft_real[j] = complexSignal[j].Re;
		}
		return fft_real;
	}
	
	public static double[] padded_FFT(Complex[] complexSignal) {

		int N = complexSignal.Length;
		Fourier.FFT(complexSignal, N, FourierDirection.Forward);

		// get the result
		double[] fft_real = new double[N];
		for (int j = 0; j < N; j++) {
			fft_real[j] = complexSignal[j].Re;
		}
		return fft_real;
	}
	
	public static Complex[] padded_FFT(double[] signal) {
		
		int N = MathUtils.NextPowerOfTwo(signal.Length);
		signal = zeros(signal, N);

		double[] signal_fft = FFTUtils.FFT(signal);
		Complex[] complexSignal = FFTUtils.DoubleToComplex(signal_fft);
		
		return complexSignal;
	}
	
	public static double log10scale(double val)
	{
		Debug.Assert(val >= 0 && val <= 1);
		return Math.Log10(1 + 9 * val);
	}

	public static double log10scale_inv(double val)
	{
		Debug.Assert(val >= 0 && val <= 1);
		return (Math.Pow(10, val) -1) / 9;
	}

	// cent = octave/1200
	public static double cent2freq(double cents)
	{
		return Math.Pow(2, cents / 1200);
	}
	
	public static double freq2cent(double freq)
	{
		return Math.Log(freq) / Math.Log(2) * 1200;
	}
	
	public static double cent2oct(double cents)
	{
		return cents / 1200;
	}
	
	public static double oct2cent(double oct)
	{
		return oct * 1200;
	}

	public static void shift90deg(ref Complex x)
	{
		x = x.GetConjugate();
		// x = std.conj(Complex(x.imag(), x.real()));
	}

	public static double[] resample(double[] @in, int len)
	{
		Debug.Assert(len > 0);
		Console.Out.WriteLine("resample(data size: {0}, len: {1}", @in.Length, len);
		if (@in.Length == len)
			return @in;

		return @in;
		
		/*
		double ratio = (double)len/@in.Length;
		if (ratio >= 256)
			return resample(resample(@in, @in.Length*50), len);
		else if (ratio <= 1.0/256)
			return resample(resample(@in, @in.Length/50), len);

		double[] @out = new double[len];
		
		// Using http://www.mega-nerd.com/SRC/api_simple.html
		//SRC_DATA parms = new SRC_DATA(const_cast<double*>(@in[0]), @out[0], @in.Length, @out.Length, 0, 0, 0, ratio);
		//src_simple(parms, SRC_SINC_FASTEST, 1);

		return @out;
		 */
	}

	public static double[] get_envelope(ref Complex[] band)
	{
		Debug.Assert(band.Length > 1);

		// copy + phase shift
		Complex[] shifted = new Complex[band.Length];
		for (int i = 0; i < band.Length; i++) {
			shift90deg(ref band[i]);
			shifted[i] = band[i];
		}
		
		double[] envelope = padded_IFFT(band);
		double[] shifted_signal = padded_IFFT(shifted);

		for (int i = 0; i < envelope.Length; ++i) {
			envelope[i] = envelope[i]*envelope[i] + shifted_signal[i]*shifted_signal[i];
		}

		return envelope;
	}

	public static double blackman_window(double x)
	{
		Debug.Assert(x >= 0 && x <= 1);
		return Math.Max(0.42 - 0.5 * Math.Cos(2 * Math.PI * x) + 0.08 * Math.Cos(4 * Math.PI * x), 0.0);
	}

	public static double hann_window(double x)
	{
		Debug.Assert(x >= 0 && x <= 1);
		return 0.5 * (1 - Math.Cos(x * 2 * Math.PI));
	}

	public static double triangular_window(double x)
	{
		Debug.Assert(x >= 0 && x <= 1);
		return 1 - Math.Abs( 2 * (x - 0.5));
	}

	public static double window_coef(double x, Window window)
	{
		Debug.Assert(x >= 0 && x <= 1);
		
		if (window == Window.WINDOW_RECTANGULAR)
			return 1.0;
		switch (window) {
			case Window.WINDOW_HANN:
				return GlobalMembersSpectrogram.hann_window(x);
			case Window.WINDOW_BLACKMAN:
				return GlobalMembersSpectrogram.blackman_window(x);
			case Window.WINDOW_TRIANGULAR:
				return GlobalMembersSpectrogram.triangular_window(x);
			default:
				Debug.Assert(false);
				break;
		}
		return 0.0;
	}

	public static double calc_intensity(double val, AxisScale intensity_axis)
	{
		Debug.Assert(val >= 0 && val <= 1);
		switch (intensity_axis) {
			case AxisScale.SCALE_LOGARITHMIC:
				return GlobalMembersSpectrogram.log10scale(val);
			case AxisScale.SCALE_LINEAR:
				return val;
			default:
				Debug.Assert(false);
				break;
		}
		return 0.0;
	}

	public static double calc_intensity_inv(double val, AxisScale intensity_axis)
	{
		Debug.Assert(val >= 0 && val <= 1);
		switch (intensity_axis) {
			case AxisScale.SCALE_LOGARITHMIC:
				return GlobalMembersSpectrogram.log10scale_inv(val);
			case AxisScale.SCALE_LINEAR:
				return val;
			default:
				Debug.Assert(false);
				break;
		}
		return 0.0;
	}

	// to <0,1> (cutoff negative)
	public static void normalize_image_cutoff_negative(ref List<double[]> data)
	{
		// Find maximum number when all numbers are made positive.
		double max = data.Max((b) => b.Max((v) => Math.Abs(v)));
		
		Debug.Assert(max > 0);
		if (max == 0.0f)
			return;

		// divide by max and return
		data = data.Select(i => i.Select(j => Math.Abs(j)/max).ToArray()).ToList();
	}

	// to <-1,1>
	public static void normalize_signal(ref double[] data)
	{
		// Find maximum number when all numbers are made positive.
		double max = data.Max((b) => Math.Abs(b));
		
		Debug.Assert(max > 0);
		if (max == 0.0f)
			return;

		// divide by max and return
		data = data.Select(i => i/max).ToArray();
	}

	// random number from <0,1>
	public static double random_double()
	{
		return RandomNumbers.NextNumber();
	}

	public static double brightness_correction(double intensity, BrightCorrection correction)
	{
		switch (correction) {
			case BrightCorrection.BRIGHT_NONE:
				return intensity;
			case BrightCorrection.BRIGHT_SQRT:
				return Math.Sqrt(intensity);
		}
		
		Debug.Assert(false);
		return 0.0;
	}

	public static Complex[] get_pink_noise(int size)
	{
		Complex[] res = new Complex[size];
		for (int i = 0; i < (size+1)/2; ++i)
		{
			double mag = Math.Pow((double) i, -0.5f);
			double phase = (2 *GlobalMembersSpectrogram.random_double()-1) * Math.PI; //+-pi random phase
			Complex complex = new Complex(mag * Math.Cos(phase), mag * Math.Sin(phase));
			res[i] = complex;
		}
		return res;
	}
}

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
		bandwidth = 100;
		basefreq = 55;
		maxfreq = 22050;
		overlap = 0.8;
		pixpersec = 100;
		window = Window.WINDOW_HANN;
		intensity_axis = AxisScale.SCALE_LOGARITHMIC;
		frequency_axis = AxisScale.SCALE_LOGARITHMIC;
		//cancelled = false;
		palette = new Palette();
	}
	
	public Bitmap to_image(ref double[] signal, int samplerate)
	{
		Console.Out.WriteLine("Transforming input");
		
		Complex[] spectrum = GlobalMembersSpectrogram.padded_FFT(signal);

		//const size_t width = (spectrum.size()-1)*2*pixpersec/samplerate;
		double w1 = spectrum.Length - 1;
		double w2 = (double) pixpersec/ (double) samplerate;
		double w3 = w1 * 2 * w2;
		int width = (int) w3;

		// transformation of frequency in hz to index in spectrum
		//const double filterscale = ((double)spectrum.size()*2)/samplerate;
		double filterscale = ((double)spectrum.Length*2) / samplerate;
		Console.Out.WriteLine("filterscale: {0}", filterscale);

		Filterbank filterbank = Filterbank.get_filterbank(frequency_axis, filterscale, basefreq, bandwidth, overlap);
		int bands = (int) filterbank.num_bands_est(maxfreq);
		int top_index = (int) (maxfreq * filterscale);
		
		// maxfreq has to be at most nyquist
		Debug.Assert(top_index <= spectrum.Length);

		//std::vector<real_vec> image_data;
		List<double[]> image_data = new List<double[]>();
		
		for (int bandidx = 0;; ++bandidx) {
			band_progress(bandidx, bands);

			// filtering
			Pair<int,int> range = filterbank.get_band(bandidx);
			
			Console.Out.WriteLine("-----");
			Console.Out.WriteLine("spectrum size: {0}", spectrum.Length);
			Console.Out.WriteLine("lowidx: {0:0.00} highidx: {1:0.00}", range.First, range.Second);
			Console.Out.WriteLine("(real)lowfreq: {0:0.00} (real)highfreq: {1:0.00}", range.First / filterscale, range.Second/filterscale);
			Console.Out.WriteLine("actual width: {0:0.00} hz", (range.Second-range.First)/filterscale);
			Console.Out.WriteLine("vertical values: {0:0.00}", (range.Second-range.First));
			Console.Out.WriteLine("crowd sample: {0:0.00}", (range.Second-range.First-1)*2);
			Console.Out.WriteLine("theoretically staci: {0:0.00} hz samplerate", 2*(range.Second-range.First)/filterscale);
			Console.Out.WriteLine("width: {0}", width);
			
			int filterbandLength = range.Second - range.First;
			Complex[] filterband = new Complex[filterbandLength];
			//std::copy(spectrum.begin()+range.first,
			//          spectrum.begin()+std::min(range.second, top_index),
			//          filterband.begin());
			int sourceIndexStart = range.First;
			int sourceIndexEnd = Math.Min(range.Second, top_index);
			int length = sourceIndexEnd - sourceIndexStart;
			Array.Copy(spectrum, sourceIndexStart, filterband, 0, length);
			
			if (range.First > top_index) {
				break;
			}
			if (range.Second > top_index) {
				int start = top_index-range.First;
				//std::fill(filterband.begin()+top_index-range.first,
				//          filterband.end(), Complex(0,0));
				break; // TODO: fix above
			}

			// windowing
			apply_window(ref filterband, range.First, filterscale);

			// envelope detection + resampling
			double[] envelope = GlobalMembersSpectrogram.resample(GlobalMembersSpectrogram.get_envelope(ref filterband), width);
			
			image_data.Add(envelope);
		}

		GlobalMembersSpectrogram.normalize_image_cutoff_negative(ref image_data);

		return make_image(image_data);
	}
	
	public Bitmap make_image(List<double[]> data)
	{
		Console.Out.WriteLine("Generating image");
		
		int height = data.Count;
		int width = data[0].Length;
		
		Console.Write("image size: ");
		Console.Write(width);
		Console.Write(" x ");
		Console.Write(height);
		Console.Write("\n");
		
		Bitmap @out = new Bitmap(width, height);
		
		BrightCorrection correction = BrightCorrection.BRIGHT_NONE;
		for (int y = 0; y < height; ++y) {
			Debug.Assert(data[y].Length == width);
			
			for (int x = 0; x < width; ++x) {
				double intensity = GlobalMembersSpectrogram.calc_intensity(data[y][x], intensity_axis);
				intensity = GlobalMembersSpectrogram.brightness_correction(intensity, correction);
				@out.SetPixel(x, (int)(height-1-y), palette.get_color(intensity));
			}
		}
		
		//@out.SetText("Spectrogram", serialized()); // save parameters
		return @out;
	}
	
	public void apply_window(ref Complex[] chunk, int lowidx, double filterscale)
	{
		int highidx = lowidx + chunk.Length;
		if (frequency_axis == AxisScale.SCALE_LINEAR) {
			for (int i = 0; i < chunk.Length; ++i) {
				chunk[i] *= GlobalMembersSpectrogram.window_coef((double)i/(chunk.Length-1), window);
			}
		} else {
			double rloglow = GlobalMembersSpectrogram.freq2cent(lowidx/filterscale); // after rounding
			double rloghigh = GlobalMembersSpectrogram.freq2cent((highidx-1)/filterscale);
			for (int i = 0; i < chunk.Length; ++i) {
				double logidx = GlobalMembersSpectrogram.freq2cent((lowidx+i)/filterscale);
				double winidx = (logidx - rloglow)/(rloghigh - rloglow);
				chunk[i] *= GlobalMembersSpectrogram.window_coef(winidx, window);
			}
		}
	}
	
	public double[] synthetize(Bitmap image, int samplerate, SynthesisType type)
	{
		switch (type) {
			case SynthesisType.SYNTHESIS_SINE:
				return synt_sine(image, samplerate);
			case SynthesisType.SYNTHESIS_NOISE:
				return synt_noise(image, samplerate);
		}
		Debug.Assert(false);
		return null;
	}
	
	public double[] synt_sine(Bitmap image, int samplerate)
	{
		int samples = image.Width * samplerate/pixpersec;
		Complex[] spectrum = new Complex[samples/2+1];

		double filterscale = ((double)spectrum.Length*2)/samplerate;

		Filterbank filterbank = Filterbank.get_filterbank(frequency_axis, filterscale, basefreq, bandwidth, overlap);

		for (int bandidx = 0; bandidx < image.Height; ++bandidx) {
			//if (cancelled())
			//	return List<int>();
			band_progress(bandidx, image.Height-1);

			double[] envelope = envelope_from_spectrogram(image, bandidx);

			// random phase between +-pi
			double phase = (2 * GlobalMembersSpectrogram.random_double() - 1) * Math.PI;

			double[] bandsignal = new double[envelope.Length*2];
			for (int j = 0; j < 4; ++j) {
				double sine = Math.Cos(j * Math.PI/2 + phase);
				for (int i = j; i < bandsignal.Length; i += 4)
					bandsignal[i] = envelope[i/2] * sine;
			}
			
			Complex[] filterband = GlobalMembersSpectrogram.padded_FFT(bandsignal);

			for (int i = 0; i < filterband.Length; ++i) {
				double x = (double)i/filterband.Length;
				
				// normalized blackman window antiderivative
				filterband[i] *= x - ((0.5/(2.0 * Math.PI)) * Math.Sin(2.0 * Math.PI *x) + (0.08/(4.0 * Math.PI)) * Math.Sin(4.0 * Math.PI *x) / 0.42);
			}

			Console.Out.WriteLine("spectrum size: {0}", spectrum.Length);
			//std::cout << bandidx << ". filterband size: " << filterband.Length << "; start: " << filterbank->get_band(bandidx).first <<"; end: " << filterbank->get_band(bandidx).second << "\n";

			double center = filterbank.get_center(bandidx);
			double offset = Math.Max((uint)0, center - filterband.Length/2);
			
			Console.Out.WriteLine("offset: {0} = {1} hz", offset, offset/filterscale);
			
			for (uint i = 0; i < filterband.Length; ++i) {
				if (offset+i > 0 && offset+i < spectrum.Length) {
					spectrum[ (int) (offset+i) ] += filterband[i];
				}
			}
		}

		double[] @out = GlobalMembersSpectrogram.padded_IFFT(spectrum);
		
		Console.Out.WriteLine("samples: {0} -> {1}", @out.Length, samples);
		
		GlobalMembersSpectrogram.normalize_signal(ref @out);
		return @out;
	}
	
	public double[] synt_noise(Bitmap image, int samplerate)
	{
		int samples = (int) image.Width * samplerate/pixpersec;

		Complex[] noise = GlobalMembersSpectrogram.get_pink_noise(samplerate *10); // 10 sec loop

		double filterscale = ((double)noise.Length*2)/samplerate;
		Filterbank filterbank = Filterbank.get_filterbank(frequency_axis, filterscale, basefreq, bandwidth, overlap);

		int top_index = (int) (maxfreq * filterscale);

		double[] @out = new double[samples];

		for (int bandidx = 0; bandidx < image.Height; ++bandidx) {
			//if (cancelled())
			//	return List<int>();
			band_progress(bandidx, image.Height-1);

			// filter noise
			Pair<int,int> range = filterbank.get_band(bandidx);
			//std::cout << bandidx << "/"<<image.height()<<"\n";
			Console.Out.WriteLine("(noise) sample: {0}", range.Second-range.First);

			double[] filtered_noise = new double[noise.Length];
			//std.copy(noise.begin()+range.first, noise.begin()+Math.Min(range.second, top_index), filtered_noise.begin()+range.first);
			
			// ifft noise
			double[] noise_mod = GlobalMembersSpectrogram.padded_IFFT(filtered_noise);
			
			// resample spectrogram band
			double[] envelope = GlobalMembersSpectrogram.resample(envelope_from_spectrogram(image, bandidx), samples);
			
			// modulate with looped noise
			for (uint i = 0; i < samples; ++i)
				@out[i] += envelope[i] * noise_mod[i % noise_mod.Length];
		}
		
		GlobalMembersSpectrogram.normalize_signal(ref @out);
		return @out;
	}
	
	public void band_progress(int x, int of)
	{
		Console.Out.WriteLine("Processing band {0} of {1}", x, of);
	}
	
	public double[] envelope_from_spectrogram(Bitmap image, int row)
	{
		double[] envelope = new double[image.Width];
		for (int x = 0; x < image.Width; ++x)
			envelope[x] = GlobalMembersSpectrogram.calc_intensity_inv(
				palette.get_intensity(image.GetPixel(x, image.Height-row-1)), intensity_axis);
		return envelope;
	}
	
	public void deserialize(String text)
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
	
	public String serialized()
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

public class Palette
{
	List<Color> colors_;
	
	public Palette(Bitmap img)
	{
		Debug.Assert(img != null);
		
		for (int x = 0; x < img.Width; ++x)
			colors_.Add(img.GetPixel(x, 0));
	}
	
	public Palette()
	{
		colors_ = new List<Color>();
		for (int i = 0; i < 256; ++i)
			colors_.Add(Color.FromArgb(i, i, i));
	}
	
	public Color get_color(double val)
	{
		Debug.Assert(val >= 0 && val <= 1);

		// returns the RGB value
		return colors_[(int) ((colors_.Count-1) * val)];
	}
	
	public bool has_color(Color color)
	{
		return colors_.IndexOf(color) != -1;
	}
	
	public double get_intensity(Color color)
	{
		int index = colors_.IndexOf(color);
		if (index == -1) // shouldn't happen
			return 0;
		return (double) index / (colors_.Count - 1);
	}
	
	public Bitmap makeCanvas(int width, int height)
	{
		Bitmap @out = new Bitmap( width, height, PixelFormat.Format32bppArgb );
		Graphics g = Graphics.FromImage(@out);
		g.FillRectangle(new SolidBrush(colors_[0]), 0, 0, width, height);
		return @out;
	}
	
	public int numColors()
	{
		return colors_.Count;
	}
}

public class Filterbank
{
	public static Filterbank get_filterbank(AxisScale type, double scale, double @base, double bandwidth, double overlap)
	{
		Filterbank filterbank;
		if (type == AxisScale.SCALE_LINEAR) {
			filterbank = new LinearFilterbank(scale, @base, bandwidth, overlap);
		} else {
			filterbank = new LogFilterbank(scale, @base, bandwidth, overlap);
		}
		return filterbank;
	}
	
	public virtual double num_bands_est(double maxfreq) { return 0; }
	
	public virtual double get_center(int i) { return 0; }
	
	public virtual Pair<int,int> get_band(int i) { return null; }
}

public class LinearFilterbank : Filterbank
{
	private double scale_;
	private double bandwidth_;
	private double startidx_;
	private double step_;

	public LinearFilterbank(double scale, double @base, double hzbandwidth, double overlap)
	{
		scale_ = scale;
		bandwidth_ = hzbandwidth * scale;
		startidx_ = Math.Max(scale_ * @base-bandwidth_/ 2, 0.0);
		step_ = (1-overlap)*bandwidth_;
		
		Console.Out.WriteLine("bandwidth: {0}", bandwidth_);
		Console.Out.WriteLine("step_: {0}", step_);

		Debug.Assert(step_ > 0);
	}
	
	public override double num_bands_est(double maxfreq)
	{
		return (maxfreq *scale_-startidx_) / step_;
	}
	
	public override Pair<int,int> get_band(int i)
	{
		Pair<int,int> @out = new Pair<int,int>();
		@out.First = (int) (startidx_ + i * step_);
		@out.Second = (int) (@out.First + bandwidth_);
		return @out;
	}
	
	public override double get_center(int i)
	{
		return startidx_ + i *step_ + bandwidth_ / 2.0;
	}
}

public class LogFilterbank : Filterbank
{

	private double scale_;
	private double centsperband_;
	private double logstart_;
	private double logstep_;

	public LogFilterbank(double scale, double @base, double centsperband, double overlap)
	{
		scale_ = scale;
		centsperband_ = centsperband;
		logstart_ = GlobalMembersSpectrogram.freq2cent(@base);
		logstep_ = (1-overlap)*centsperband_;

		Console.Out.WriteLine("centsperband_: {0}", centsperband_);
		Console.Out.WriteLine("logstep_: {0}", logstep_);
		Debug.Assert(logstep_ > 0);
	}
	
	public override double num_bands_est(double maxfreq)
	{
		return (GlobalMembersSpectrogram.freq2cent(maxfreq)-logstart_) / logstep_ + 4;
	}
	
	public override double get_center(int i)
	{
		double logcenter = logstart_ + i * logstep_;
		return GlobalMembersSpectrogram.cent2freq(logcenter) * scale_;
	}
	
	public override Pair<int,int> get_band(int i)
	{
		double logcenter = logstart_ + i * logstep_;
		double loglow = logcenter - centsperband_/ 2.0;
		double loghigh = loglow + centsperband_;
		Pair<int,int> @out = new Pair<int,int>();
		@out.First = (int) (GlobalMembersSpectrogram.cent2freq(loglow) * scale_);
		@out.Second = (int) (GlobalMembersSpectrogram.cent2freq(loghigh) * scale_);
		
		Console.Out.WriteLine("centerfreq: {0}", GlobalMembersSpectrogram.cent2freq(logcenter));
		Console.Out.WriteLine("lowfreq: {0}, highfreq: {1}", GlobalMembersSpectrogram.cent2freq(loglow), GlobalMembersSpectrogram.cent2freq(loghigh));
		return @out;
	}
}