using System;
using System.Diagnostics;

// Converted from C++ to C#
// https://github.com/krajj7/spectrogram/blob/master/spectrogram.cpp
// see Spectrogram.cs for enums

public class Filterbank
{
	public static Filterbank GetFilterbank(AxisScale type, double scale, double @base, double bandwidth, double overlap)
	{
		Filterbank filterbank;
		if (type == AxisScale.SCALE_LINEAR) {
			filterbank = new LinearFilterbank(scale, @base, bandwidth, overlap);
		} else {
			filterbank = new LogFilterbank(scale, @base, bandwidth, overlap);
		}
		return filterbank;
	}
	
	public virtual double NumBandsEst(double maxfreq) { return 0; }
	
	public virtual double GetCenter(int i) { return 0; }
	
	public virtual Pair<int,int> GetBand(int i) { return null; }
}

public class LinearFilterbank : Filterbank
{
	double scale_;
	double bandwidth_;
	double startidx_;
	double step_;

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
	
	public override double NumBandsEst(double maxfreq)
	{
		return (maxfreq *scale_-startidx_) / step_;
	}
	
	public override Pair<int,int> GetBand(int i)
	{
		var @out = new Pair<int,int>();
		@out.First = (int) (startidx_ + i * step_);
		@out.Second = (int) (@out.First + bandwidth_);
		return @out;
	}
	
	public override double GetCenter(int i)
	{
		return startidx_ + i *step_ + bandwidth_ / 2.0;
	}
}

public class LogFilterbank : Filterbank
{
	double scale_;
	double centsperband_;
	double logstart_;
	double logstep_;

	public LogFilterbank(double scale, double @base, double centsperband, double overlap)
	{
		scale_ = scale;
		centsperband_ = centsperband;
		logstart_ = SpectrogramUtils.Freq2Cent(@base);
		logstep_ = (1-overlap)*centsperband_;

		Console.Out.WriteLine("centsperband_: {0}", centsperband_);
		Console.Out.WriteLine("logstep_: {0}", logstep_);
		Debug.Assert(logstep_ > 0);
	}
	
	public override double NumBandsEst(double maxfreq)
	{
		return (SpectrogramUtils.Freq2Cent(maxfreq)-logstart_) / logstep_ + 4;
	}
	
	public override double GetCenter(int i)
	{
		double logcenter = logstart_ + i * logstep_;
		return SpectrogramUtils.Cent2Freq(logcenter) * scale_;
	}
	
	public override Pair<int,int> GetBand(int i)
	{
		double logcenter = logstart_ + i * logstep_;
		double loglow = logcenter - centsperband_/ 2.0;
		double loghigh = loglow + centsperband_;
		var @out = new Pair<int,int>();
		@out.First = (int) (SpectrogramUtils.Cent2Freq(loglow) * scale_);
		@out.Second = (int) (SpectrogramUtils.Cent2Freq(loghigh) * scale_);
		
		Console.Out.WriteLine("centerfreq: {0}", SpectrogramUtils.Cent2Freq(logcenter));
		Console.Out.WriteLine("lowfreq: {0}, highfreq: {1}", SpectrogramUtils.Cent2Freq(loglow), SpectrogramUtils.Cent2Freq(loghigh));
		return @out;
	}
}