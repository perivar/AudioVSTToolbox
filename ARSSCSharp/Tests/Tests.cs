using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using CommonUtils;
using CommonUtils.CommonMath.FFT;

using ArssSpectrogram;

namespace ARSS
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void TestSpectrogramFFT() {
			// Testing Forward and Backwards Padded FFT and IFFT
			List<object> objects = IOUtils.ReadCSV(@"Tests\fft_input.csv", false, SpectrogramUtils.CsvDoubleParser);
			IOUtils.WriteCSV(@"Tests\arss_input.csv", objects, SpectrogramUtils.CvsDoubleFormatter);
			
			var doubles = objects.Cast<double>().ToArray();
			Complex[] spectrum = SpectrogramUtils.padded_FFT(ref doubles);
			
			List<object> lines = spectrum.Cast<object>().ToList();
			IOUtils.WriteCSV(@"Tests\arss_fft.csv", lines, SpectrogramUtils.CvsComplexFormatter);

			var ifft = SpectrogramUtils.padded_IFFT(ref spectrum, true);
			List<object> ifft_lines = ifft.Cast<object>().ToList();
			IOUtils.WriteCSV(@"Tests\arss_ifft.csv", ifft_lines, SpectrogramUtils.CvsDoubleFormatter);
			
			// except for the padding, test that the original array and the end result is equal
			Array.Resize<double>(ref doubles, ifft.Length);
			Assert.That(doubles, Is.EqualTo(ifft).AsCollection.Within(0.001), "fail at [0]");
		}
	}
}
