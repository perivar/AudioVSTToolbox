using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

// see Spectrogram.cs for enums

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
	
	public Color GetColor(double val)
	{
		Debug.Assert(val >= 0 && val <= 1);

		// returns the RGB value
		return colors_[(int) ((colors_.Count-1) * val)];
	}
	
	public bool HasColor(Color color)
	{
		return colors_.IndexOf(color) != -1;
	}
	
	public double GetIntensity(Color color)
	{
		int index = colors_.IndexOf(color);
		if (index == -1) // shouldn't happen
			return 0;
		return (double) index / (colors_.Count - 1);
	}
	
	public Bitmap MakeCanvas(int width, int height)
	{
		var @out = new Bitmap( width, height, PixelFormat.Format32bppArgb );
		Graphics g = Graphics.FromImage(@out);
		g.FillRectangle(new SolidBrush(colors_[0]), 0, 0, width, height);
		return @out;
	}
	
	public int NumColors()
	{
		return colors_.Count;
	}
}
