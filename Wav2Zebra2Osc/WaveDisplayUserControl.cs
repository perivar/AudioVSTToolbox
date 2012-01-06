using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Wav2Zebra2CSharp
{
	/// <summary>
	/// Wave Cell Slot
	/// </summary>
	public partial class WaveDisplayUserControl : UserControl
	{
		
		private const long serialVersionUID = 1L;
		private float[] waveData;
		private float[] dftData;
		private float[] harmonicsData;
		private float[] interpolatedData;
		private float[] emptyData;
		private bool selected;
		private bool loaded;
		private string fileName;
		private MainForm parentForm;

		public WaveDisplayUserControl(MainForm parentForm)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
						
			//
			// Constructor code after the InitializeComponent() call.
			//
			this.parentForm = parentForm;			
			this.waveData = new float[128];
			this.harmonicsData = new float[128];
			this.emptyData = new float[128];
			this.dftData = new float[128];
			this.fileName = "";
			for (int i = 0; i < 128; i++)
			{
				this.waveData[i] = 0.0F;
				this.harmonicsData[i] = 0.0F;
				this.emptyData[i] = 0.0F;
				this.dftData[i] = 0.0F;	
			}
			
			this.selected = false;
			this.loaded = false;			
		}
		
		public virtual string FileName
		{
			set
			{
				this.fileName = value;
			}			 
			get
			{
				return this.fileName;		
			}
		}
		
		public virtual float[] WaveData
		{
			set
			{
				Array.Copy(value, 0, this.waveData, 0, 128);		
			}
			get
			{
				return this.waveData;	
			}			
		}
		
		public virtual float[] DftData
		{
			set
			{
				Array.Copy(value, 0, this.dftData, 0, 128);
			}
			get
			{
				return this.dftData;	
			}			
		}
		
		public virtual float[] HarmonicsData
		{
			set
			{
				int length = value.Length;
				for (int i = 0; i < length; i++)			
				{				 
					this.harmonicsData[i] = value[i];
				}
			}
			get
			{
				return this.harmonicsData;	
			}			
		}

		public virtual float[] InterpolatedData
		{
			get
			{
				return this.interpolatedData;	
			}			
		}
		
		public virtual void ClearWaveData()
		{
			Array.Copy(this.emptyData, 0, this.waveData, 0, 128);	
			this.loaded = false;
		}			 
		
		public virtual void ClearDftData()
		{
			Array.Copy(this.emptyData, 0, this.dftData, 0, 128);
		}			 
		
		public virtual void ClearHarmonics()
		{
			Array.Copy(this.emptyData, 0, this.harmonicsData, 0, 128);
			this.loaded = false;
		}
		
		public virtual bool Selected
		{
			set
			{
				this.selected = value;		
			}
			get
			{
				return this.selected;	
			}
		}
		
		public virtual bool Loaded
		{
			set
			{
				this.loaded = value;		
			}			 
			get
			{
				return this.loaded;	
			}
		}		
	}
}
