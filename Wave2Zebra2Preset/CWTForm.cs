//  Continuous Wavelet Transform Spectrogram program
//  By Chris Lang and Kyle Forinash
//  1997 - 2004

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Globalization;

namespace CWT
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new CWTForm());
		}	
	}

	//*********************************************************
	//   class for dialog box for wavelet transform parameters
	//*********************************************************
	public class Wavelet_Dialog : Form
	{
		private Button OKButton;
		private Button Cancel_Button;
		private Label sigmaLabel;
		private Label lowerfreqLabel;
		private Label upperfreqLabel;
		private TextBox sigmaBox;
		private TextBox lowerfreqBox;
		private TextBox upperfreqBox;

		public double sigma;
		public double upperfreq;
		public double lowerfreq;
		
		public Wavelet_Dialog()
		{
			Text = "Wavelet Parameters";
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Size = new System.Drawing.Size(240,240);

			OKButton = new Button();
			OKButton.Text = "OK";
			OKButton.Size = new System.Drawing.Size(40,25);
			OKButton.Location = new Point(100,160);
			OKButton.Click += new EventHandler(OKButton_Clicked);
			Controls.Add(OKButton);

			Cancel_Button = new Button();
			Cancel_Button.Text = "Cancel";
			Cancel_Button.Size = new System.Drawing.Size(60,25);
			Cancel_Button.Location = new Point(150,160);
			Cancel_Button.Click += new EventHandler(Cancel_Button_Clicked);
			Controls.Add(Cancel_Button);

			sigmaLabel = new Label();
			sigmaBox = new TextBox();
			sigmaLabel.Location = new Point(20,20);
			sigmaLabel.Size = new System.Drawing.Size(70,16);
			sigmaLabel.Text = "sigma";
			sigmaLabel.TextAlign = ContentAlignment.MiddleRight;
			sigmaBox.Location = new Point(90,20);
			sigmaBox.Size = new System.Drawing.Size(60,20);
			sigmaBox.Text = System.Convert.ToString(sigma);
			Controls.Add(sigmaLabel);
			Controls.Add(sigmaBox);

			lowerfreqLabel = new Label();
			lowerfreqBox = new TextBox();
			lowerfreqLabel.Location = new Point(20,50);
			lowerfreqLabel.Size = new System.Drawing.Size(70,16);
			lowerfreqLabel.Text = "lower freq";
			lowerfreqLabel.TextAlign = ContentAlignment.MiddleRight;
			lowerfreqBox.Location = new Point(90,50);
			lowerfreqBox.Size = new System.Drawing.Size(60,20);
			lowerfreqBox.Text = System.Convert.ToString(lowerfreq);
			Controls.Add(lowerfreqLabel);
			Controls.Add(lowerfreqBox);

			upperfreqLabel = new Label();
			upperfreqBox = new TextBox();
			upperfreqLabel.Location = new Point(20,80);
			upperfreqLabel.Size = new System.Drawing.Size(70,16);
			upperfreqLabel.Text = "upper freq";
			upperfreqLabel.TextAlign = ContentAlignment.MiddleRight;
			upperfreqBox.Location = new Point(90,80);
			upperfreqBox.Size = new System.Drawing.Size(60,20);
			upperfreqBox.Text = System.Convert.ToString(upperfreq);
			Controls.Add(upperfreqLabel);
			Controls.Add(upperfreqBox);
		}

		public void setSigma(double sigmaval)
		{
			sigmaBox.Text = System.Convert.ToString(sigmaval);
		}

		public void setUpperFreq(double upperfreqval)
		{
			upperfreqBox.Text = System.Convert.ToString(upperfreqval);
		}

		public void setLowerFreq(double lowerfreqval)
		{
			lowerfreqBox.Text = System.Convert.ToString(lowerfreqval);
		}

		public void OKButton_Clicked(object Sender, EventArgs pArgs)
		{
			string sigmabox_string;
			string lowerfreqbox_string;
			string upperfreqbox_string;
			sigmabox_string = sigmaBox.Text;
			sigma = System.Convert.ToDouble(sigmabox_string);
			lowerfreqbox_string = lowerfreqBox.Text;
			lowerfreq = System.Convert.ToDouble(lowerfreqbox_string);
			upperfreqbox_string = upperfreqBox.Text;
			upperfreq = System.Convert.ToDouble(upperfreqbox_string);
			Close();
		}

		public void Cancel_Button_Clicked(object Sender, EventArgs pArgs)
		{
			Close();
		}

	}

	//*********************************************************
	//  class CWTForm.  variables; 'Setup_Menu' function;
	//  'MenuItem_Clicked' function that handles menu events.
	//*********************************************************
	public partial class CWTForm : Form
	{
		private MainMenu menuBar;
		private MenuItem fileMenu;
		private MenuItem fileopenMenuItem;
		private MenuItem printMenuItem;
		private MenuItem exportEPSMenuItem;
		private MenuItem exitMenuItem;
		private MenuItem mathMenu;
		private MenuItem spectrogramMenuItem;
		private MenuItem waveletMenuItem;
		private MenuItem helpMenu;

		private int image_width;
		private int image_height;
		private double max;
		private Double[] signal_data;
		private Double[] image_data;
		private double signal_samplerate;
		private double signal_length;
		private double dt;
		private double sigma;
		private double upper_freq;
		private double lower_freq;
		private Int32 signal_size;
		private Bitmap bmp;
		private FileStream sf;
		private string filename_text;

		private void Setup_Menu()
		{
			menuBar = new MainMenu();

			fileMenu = new MenuItem("&File");
			menuBar.MenuItems.Add(fileMenu);
			fileopenMenuItem = new MenuItem("Open Signal File");
			fileMenu.MenuItems.Add(fileopenMenuItem);
			printMenuItem = new MenuItem("Print");
			fileMenu.MenuItems.Add(printMenuItem);
			exportEPSMenuItem = new MenuItem("Export EPS image ...");
			fileMenu.MenuItems.Add(exportEPSMenuItem);
			exitMenuItem = new MenuItem("Exit");
			fileMenu.MenuItems.Add(exitMenuItem);

			mathMenu = new MenuItem("&Math");
			menuBar.MenuItems.Add(mathMenu);
			spectrogramMenuItem = new MenuItem("&Compute Spectrogram");
			mathMenu.MenuItems.Add(spectrogramMenuItem);
			waveletMenuItem = new MenuItem("Wavelet Parameters ...");
			mathMenu.MenuItems.Add(waveletMenuItem);

			helpMenu = new MenuItem("&Help");
			menuBar.MenuItems.Add(helpMenu);

			Menu = menuBar;
		}

		private void MenuItem_Clicked(object pSender, EventArgs pArgs)
		{
			int i;
			string signal_file_line;

			if (pSender == printMenuItem)
			{
				PrintDocument pdoc = new PrintDocument();
				PrintDialog pd = new PrintDialog();
				pd.Document = pdoc;
				if (pd.ShowDialog() == DialogResult.OK)
				{
					pdoc.PrintPage += new PrintPageEventHandler(PrintCWT);
					pdoc.Print();
				}
				else
					MessageBox.Show("print cancelled", "information");
			}

			if (pSender == fileopenMenuItem)
			{
				OpenFileDialog ofd = new OpenFileDialog();
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					sf = new FileStream(ofd.FileName,FileMode.Open);
					filename_text = ofd.FileName;
					StreamReader sr = new StreamReader(sf);
					signal_file_line = sr.ReadLine();
					MessageBox.Show(signal_file_line, "File Name");
					signal_size = System.Convert.ToInt32(signal_file_line);
					signal_file_line = sr.ReadLine();
					signal_samplerate = Double.Parse(signal_file_line, CultureInfo.InvariantCulture);
					//signal_samplerate = System.Convert.ToDouble(signal_file_line);
					dt = 1.0/signal_samplerate;
					signal_length = System.Convert.ToDouble(signal_size)/signal_samplerate; // seconds
					for (i = 0;i<signal_size;i++)
					{
						try {
						signal_file_line = sr.ReadLine();
						//signal_data[i] = System.Convert.ToDouble(signal_file_line);
						signal_data[i] = Double.Parse(signal_file_line, CultureInfo.InvariantCulture);
						} catch (Exception e) {
							System.Diagnostics.Debug.WriteLine(e);
						}
					}
					sf.Close();
					this.Invalidate();
				}
			}

			if (pSender == exportEPSMenuItem)
			{
				SaveFileDialog sfd = new SaveFileDialog();
				if (sfd.ShowDialog() == DialogResult.OK)
				{
					writeepsfile(sfd.FileName);
				}
			}

			if (pSender == exitMenuItem)
				Application.Exit();

			if (pSender == spectrogramMenuItem)
			{
				ComputeSpectrogram();
			}

			if (pSender == waveletMenuItem)
			{
				Wavelet_Dialog wdlg = new Wavelet_Dialog();
				wdlg.setSigma(sigma);
				wdlg.setUpperFreq(upper_freq);
				wdlg.setLowerFreq(lower_freq);
				wdlg.ShowDialog();
				sigma = wdlg.sigma;
				upper_freq = wdlg.upperfreq;
				lower_freq = wdlg.lowerfreq;
				this.Invalidate();
			}


			if (pSender == helpMenu)
				MessageBox.Show("help menu item", "Menu");
		}


		//*******************************************************
		//   Mathematics Functions
		//*******************************************************
		public static double RealWavelet(double t, double sigma = 4)
		{
			//return Math.Cos(6.283185 *x)* Math.Exp(-1.0 *x *x/(2.0 *sigma *sigma))/(sigma *2.5066);
			return Math.Cos(2.0*Math.PI*t)* Math.Exp(-1.0*t*t/(2.0*sigma*sigma)) / (sigma*Math.Sqrt(2.0*Math.PI));
		}

		public static double ImagWavelet(double t, double sigma = 4)
		{
			//return Math.Sin(6.283185 *x)* Math.Exp(-1.0 *x *x/(2.0 *sigma *sigma))/(sigma *2.5066);
			return Math.Sin(2.0*Math.PI*t)* Math.Exp(-1.0*t*t/(2.0*sigma*sigma)) / (sigma*Math.Sqrt(2.0*Math.PI));
		}

		public void ComputeSpectrogram()
		{
			double t;
			double f;
			double df;
			double x;
			double realval;
			double imagval;
			int i;
			int j;
			int c;
			int row;
			int col;
			int k;
			int loc;
			double[] realkernel = new double[signal_size*2];//16384];
			double[] imagkernel = new double[signal_size*2];//16384];

			df = Math.Pow(upper_freq/lower_freq, 1.0/256.0);
			max = 0.0001;
			f = lower_freq;
			for (row = 0; row < 256 ; row++)
			{
				// compute new kernels for current frequency
				//t = -8192.0 * dt;
				t = -1.0 * signal_size * dt;
				for (i = 0; i < signal_size*2; i++) //16384
				{
					realkernel[i] = RealWavelet(t * f);
					imagkernel[i] = ImagWavelet(t * f);
					t = t + dt;
				}
				// compute values of CWT across row
				for (col = 0; col < 512; col++)
				{
					realval = 0.0;
					imagval = 0.0;
					loc = (col * signal_size) / 512;
					for (i = 0; i < signal_size; i++)
					{
						realval = realval + signal_data[i]*realkernel[signal_size-loc+i]; // 8192-loc+i
						imagval = imagval + signal_data[i]*imagkernel[signal_size-loc+i]; // 8192-loc+i
					}
					x = Math.Sqrt(realval * realval + imagval * imagval);
					k = 512 * row + col;
					image_data[k] = x;

					// store max value					
					if (max<x)
						max = x;
				}
				f = f * df;
			}

			// put computed wavelet data, scaled, into bitmap image
			for (i = 0;i<512;i++)
			{
				for (j = 0;j<256;j++)
				{
					k = 512 *j+i;
					c = 255 - (int)(255.0 *image_data[k]/max);
					if (c<0)
						c = 0;
					if (c>255)
						c = 255;
					bmp.SetPixel(i,255-j,System.Drawing.Color.FromArgb(255,c,c,c));
				}
			}
			this.Invalidate();
		}

		//*********************************************************
		//   function 'writeepsfile' to write
		//   encapsulated postscript image of spectrogram
		//*********************************************************

		public void writeepsfile(string filename)
		{
			int lowbyte;
			int hibyte;
			int height;
			int width;
			int i;
			int j;
			int k;
			int red;
			int blue;
			int green;
			FileStream sf = new FileStream(filename,FileMode.Create);
			StreamWriter sw = new StreamWriter(sf);

			k = 0;
			height = 256;
			width = 512;

			// write header
			sw.WriteLine("%!PS-Adobe-3.0 EPSF-3.0");
			sw.WriteLine("%%Creator: CWT Application (Chris Lang)");
			sw.WriteLine("%%BoundingBox: 0 0 512 256");
			sw.WriteLine("%%LanguageLevel: 2");
			sw.WriteLine("%%Pages: 1");
			sw.WriteLine("%%DocumentData: Clean7Bit");
			sw.WriteLine("512 256 scale");
			sw.WriteLine("512 256 8 [512 0 0 -256 0 256]");
			sw.WriteLine("{currentfile 3 512 mul string readhexstring pop} bind");
			sw.WriteLine("false 3 colorimage");

			// write data
			for (j = 0;j<height;j++)
			{
				for (i = 0;i<width;i++)
				{
					k++;
					red = bmp.GetPixel(i,j).R;
					hibyte = red/16;
					lowbyte = red - hibyte *16;
					switch (hibyte)
					{
						case 0:
							sw.Write("0");
							break;
						case 1:
							sw.Write("1");
							break;
						case 2:
							sw.Write("2");
							break;
						case 3:
							sw.Write("3");
							break;
						case 4:
							sw.Write("4");
							break;
						case 5:
							sw.Write("5");
							break;
						case 6:
							sw.Write("6");
							break;
						case 7:
							sw.Write("7");
							break;
						case 8:
							sw.Write("8");
							break;
						case 9:
							sw.Write("9");
							break;
						case 10:
							sw.Write("a");
							break;
						case 11:
							sw.Write("b");
							break;
						case 12:
							sw.Write("c");
							break;
						case 13:
							sw.Write("d");
							break;
						case 14:
							sw.Write("e");
							break;
						case 15:
							sw.Write("f");
							break;
					}
					switch (lowbyte)
					{
						case 0:
							sw.Write("0");
							break;
						case 1:
							sw.Write("1");
							break;
						case 2:
							sw.Write("2");
							break;
						case 3:
							sw.Write("3");
							break;
						case 4:
							sw.Write("4");
							break;
						case 5:
							sw.Write("5");
							break;
						case 6:
							sw.Write("6");
							break;
						case 7:
							sw.Write("7");
							break;
						case 8:
							sw.Write("8");
							break;
						case 9:
							sw.Write("9");
							break;
						case 10:
							sw.Write("a");
							break;
						case 11:
							sw.Write("b");
							break;
						case 12:
							sw.Write("c");
							break;
						case 13:
							sw.Write("d");
							break;
						case 14:
							sw.Write("e");
							break;
						case 15:
							sw.Write("f");
							break;
					}

					green = bmp.GetPixel(i,j).G;
					hibyte = green/16;
					lowbyte = green - hibyte *16;
					switch (hibyte)
					{
						case 0:
							sw.Write("0");
							break;
						case 1:
							sw.Write("1");
							break;
						case 2:
							sw.Write("2");
							break;
						case 3:
							sw.Write("3");
							break;
						case 4:
							sw.Write("4");
							break;
						case 5:
							sw.Write("5");
							break;
						case 6:
							sw.Write("6");
							break;
						case 7:
							sw.Write("7");
							break;
						case 8:
							sw.Write("8");
							break;
						case 9:
							sw.Write("9");
							break;
						case 10:
							sw.Write("a");
							break;
						case 11:
							sw.Write("b");
							break;
						case 12:
							sw.Write("c");
							break;
						case 13:
							sw.Write("d");
							break;
						case 14:
							sw.Write("e");
							break;
						case 15:
							sw.Write("f");
							break;
					}
					switch (lowbyte)
					{
						case 0:
							sw.Write("0");
							break;
						case 1:
							sw.Write("1");
							break;
						case 2:
							sw.Write("2");
							break;
						case 3:
							sw.Write("3");
							break;
						case 4:
							sw.Write("4");
							break;
						case 5:
							sw.Write("5");
							break;
						case 6:
							sw.Write("6");
							break;
						case 7:
							sw.Write("7");
							break;
						case 8:
							sw.Write("8");
							break;
						case 9:
							sw.Write("9");
							break;
						case 10:
							sw.Write("a");
							break;
						case 11:
							sw.Write("b");
							break;
						case 12:
							sw.Write("c");
							break;
						case 13:
							sw.Write("d");
							break;
						case 14:
							sw.Write("e");
							break;
						case 15:
							sw.Write("f");
							break;
					}

					blue = bmp.GetPixel(i,j).B;
					hibyte = blue/16;
					lowbyte = blue - hibyte *16;
					switch (hibyte)
					{
						case 0:
							sw.Write("0");
							break;
						case 1:
							sw.Write("1");
							break;
						case 2:
							sw.Write("2");
							break;
						case 3:
							sw.Write("3");
							break;
						case 4:
							sw.Write("4");
							break;
						case 5:
							sw.Write("5");
							break;
						case 6:
							sw.Write("6");
							break;
						case 7:
							sw.Write("7");
							break;
						case 8:
							sw.Write("8");
							break;
						case 9:
							sw.Write("9");
							break;
						case 10:
							sw.Write("a");
							break;
						case 11:
							sw.Write("b");
							break;
						case 12:
							sw.Write("c");
							break;
						case 13:
							sw.Write("d");
							break;
						case 14:
							sw.Write("e");
							break;
						case 15:
							sw.Write("f");
							break;
					}
					switch (lowbyte)
					{
						case 0:
							sw.Write("0");
							break;
						case 1:
							sw.Write("1");
							break;
						case 2:
							sw.Write("2");
							break;
						case 3:
							sw.Write("3");
							break;
						case 4:
							sw.Write("4");
							break;
						case 5:
							sw.Write("5");
							break;
						case 6:
							sw.Write("6");
							break;
						case 7:
							sw.Write("7");
							break;
						case 8:
							sw.Write("8");
							break;
						case 9:
							sw.Write("9");
							break;
						case 10:
							sw.Write("a");
							break;
						case 11:
							sw.Write("b");
							break;
						case 12:
							sw.Write("c");
							break;
						case 13:
							sw.Write("d");
							break;
						case 14:
							sw.Write("e");
							break;
						case 15:
							sw.Write("f");
							break;
					}

					if (k%12 == 0)
						sw.WriteLine();
				}
			}

			// write end-of-file tag
			sw.Write("%%EOF");

			sw.Flush();
			sw.Close();
		}



		//*********************************************************
		//   constructor class for main program.  Event handlers
		//   are actually linked to menu items here.
		//   Also: paint and print functions display and print
		//   results.
		//*********************************************************
		public CWTForm()
		{
			Text = "CWT";
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Size = new System.Drawing.Size(600, 600);

			image_width = 512;
			image_height = 256;
			sigma = 4.0;
			upper_freq = 5000;//128.0;
			lower_freq = 20;//4.0;
			//signal_data = new Double[16384]; 
			signal_data = new Double[55120]; 
			image_data = new Double[image_width*image_height]; //131072
			bmp = new Bitmap(image_width,image_height,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

			Setup_Menu();

			Paint += new PaintEventHandler(Form_Paint);
			printMenuItem.Click += new EventHandler(MenuItem_Clicked);
			fileopenMenuItem.Click += new EventHandler(MenuItem_Clicked);
			exportEPSMenuItem.Click += new EventHandler(MenuItem_Clicked);
			exitMenuItem.Click += new EventHandler(MenuItem_Clicked);
			spectrogramMenuItem.Click += new EventHandler(MenuItem_Clicked);
			waveletMenuItem.Click += new EventHandler(MenuItem_Clicked);
			helpMenu.Click += new EventHandler(MenuItem_Clicked);
		}

		public void Form_Paint(object pSender, PaintEventArgs pe)
		{
			string test_string = new string(new char[255]);
			int ix;
			int iy;
			int idx;
			int idy;
			string lowerfreq_text;
			string upperfreq_text;
			string sigma_text;
			string samplerate_text;
			string timelength_text;

			Graphics gr = pe.Graphics;
			System.Drawing.Font font1;
			System.Drawing.Brush sb = new SolidBrush(System.Drawing.Color.FromArgb(240,240,240));
			gr.FillRectangle(sb, 0,0,600,600);

			gr.DrawImage(bmp, 20, 20);

			font1 = new System.Drawing.Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
			ix = 20;
			iy = 300;
			idx = 125;
			idy = 25;

			upperfreq_text = System.Convert.ToString(upper_freq);
			gr.DrawString("upper frequency: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(upperfreq_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			lowerfreq_text = System.Convert.ToString(lower_freq);
			gr.DrawString("lower frequency: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(lowerfreq_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			sigma_text = System.Convert.ToString(sigma);
			gr.DrawString("sigma: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(sigma_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			samplerate_text = System.Convert.ToString(signal_samplerate);
			gr.DrawString("sample rate: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(samplerate_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			timelength_text = System.Convert.ToString(signal_size/signal_samplerate);
			gr.DrawString("signal duration: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(timelength_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			gr.DrawString("data source file: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(filename_text, font1, Brushes.Black, ix+idx, iy);

			gr.Dispose();
		}


		public void PrintCWT(object pSender, PrintPageEventArgs pe)
		{
			string test_string = new string(new char[255]);
			int ix;
			int iy;
			int idx;
			int idy;
			string lowerfreq_text;
			string upperfreq_text;
			string sigma_text;
			string samplerate_text;
			string timelength_text;

			Graphics gr = pe.Graphics;
			System.Drawing.Font font1;

			gr.DrawImage(bmp, 20, 20);

			font1 = new System.Drawing.Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
			ix = 20;
			iy = 300;
			idx = 125;
			idy = 25;

			upperfreq_text = System.Convert.ToString(upper_freq);
			gr.DrawString("upper frequency: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(upperfreq_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			lowerfreq_text = System.Convert.ToString(lower_freq);
			gr.DrawString("lower frequency: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(lowerfreq_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			sigma_text = System.Convert.ToString(sigma);
			gr.DrawString("sigma: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(sigma_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			samplerate_text = System.Convert.ToString(signal_samplerate);
			gr.DrawString("sample rate: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(samplerate_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			timelength_text = System.Convert.ToString(signal_size/signal_samplerate);
			gr.DrawString("signal duration: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(timelength_text, font1, Brushes.Black, ix+idx, iy);

			iy = iy+idy;
			gr.DrawString("data source file: ", font1, Brushes.Black, ix, iy);
			gr.DrawString(filename_text, font1, Brushes.Black, ix+idx, iy);

			gr.Dispose();
		}
	}
}