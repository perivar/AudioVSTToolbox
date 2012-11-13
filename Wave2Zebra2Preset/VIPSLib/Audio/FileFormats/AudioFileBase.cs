
using System;

namespace VIPSLib.Audio
{
	public abstract class AudioFileBase
	{
		protected Byte[] _Data8;
		public byte[] Data8 {
			get { return _Data8; }
		}
		protected Int16[] _Data16;
		public short[] Data16 {
			get { return _Data16; }
		}
		protected Int32[] _Data32;
		public int[] Data32 {
			get { return _Data32; }
		}
        protected Double[] _Data;
        public double[] Data
        {
            get { return _Data; }
        }
		
		public AudioFileBase()
		{
		}
		
		public abstract void ReadFromFile(String filename);

        public abstract void ReadFromFileToDouble(String filename);
	}
}
