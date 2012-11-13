using System;
using System.IO;

namespace VIPSLib.Audio
{
	public class WAVFile : AudioFileBase
	{
		#region Properties
		
		private string _Filepath;
		public string Filepath {
			get { return _Filepath; }
		}
		
		private byte[] _ID;
		public byte[] ID {
			get { return _ID; }
		}
		
		private Int16 _AudioFormat;
		public short AudioFormat {
			get { return _AudioFormat; }
		}
		private Int16 _NumChannels;
		public short NumChannels {
			get { return _NumChannels; }
		}
		private Int16 _BlockAlign;
		public short BlockAlign {
			get { return _BlockAlign; }
		}
		private Int16 _BitsPerSample;
		public short BitsPerSample {
			get { return _BitsPerSample; }
		}
		
		private byte[] _Format;
		public byte[] Format {
			get { return _Format; }
		}
		
		private byte[] _Subchunk1ID;
		public byte[] Subchunk1ID {
			get { return _Subchunk1ID; }
		}
		
		private byte[] _Subchunk2ID;
		public byte[] Subchunk2ID {
			get { return _Subchunk2ID; }
		}
		
		private UInt32 _ChunkSize;
		public uint ChunkSize {
			get { return _ChunkSize; }
		}
		private UInt32 _Subchunk1Size;
		public uint Subchunk1Size {
			get { return _Subchunk1Size; }
		}
		private UInt32 _SampleRate;
		public uint SampleRate {
			get { return _SampleRate; }
		}
		private UInt32 _ByteRate;
		public uint ByteRate {
			get { return _ByteRate; }
		}
		private UInt32 _Subchunk2Size;
		public uint Subchunk2Size {
			get { return _Subchunk2Size; }
		}
		private Int16 _ExtraParamSize;
		public short ExtraParamSize {
			get { return _ExtraParamSize; }
		}
		private byte[] _ExtraParam;
		public byte[] ExtraParam {
			get { return _ExtraParam; }
		}
		
		
		public int BytePerSec 
		{
			get 
			{ 
				return (int)SampleRate * NumChannels * BytePerSample;
			}
		}
		public int BytePerSample 
		{
			get 
			{ 
				return BitsPerSample / 8; 
			}
		}
		public int MiliSeconds
			
		{
			get 
			{ 
				int len = 0;
				if (Data != null)
					len = Data.Length;
				switch (BitsPerSample) 
				{
					case 8:
						len = Data8.Length;
						break;
					case 16:
						len = Data16.Length;
						break;
					case 32:
						len = Data32.Length;
						break;
				}
				return len * 1000 / BytePerSec;
			}
		}
		
		#endregion
		
		public WAVFile()
		{
		}

        public override void ReadFromFileToDouble(String filename)
        {
            this._Filepath = filename;
            try
            {
                BinaryReader binReader = new BinaryReader(File.OpenRead(filename));

                this._ID = binReader.ReadBytes(4);
                this._ChunkSize = binReader.ReadUInt32();
                this._Format = binReader.ReadBytes(4);

                this._Subchunk1ID = binReader.ReadBytes(4);
                this._Subchunk1Size = binReader.ReadUInt32();
                this._AudioFormat = binReader.ReadInt16();
                this._NumChannels = binReader.ReadInt16();
                this._SampleRate = binReader.ReadUInt32();
                this._ByteRate = binReader.ReadUInt32();

                this._BlockAlign = binReader.ReadInt16();
                this._BitsPerSample = binReader.ReadInt16();

                //binReader.BaseStream.Seek(_Subchunk1Size + 20, SeekOrigin.Begin);
                
                if (_Subchunk1Size != 16)
                {
                	this._ExtraParamSize = binReader.ReadInt16();
                	this._ExtraParam = binReader.ReadBytes(this._ExtraParamSize);
                }
                
                this._Subchunk2ID = binReader.ReadBytes(4);
                this._Subchunk2Size = binReader.ReadUInt32();

                this._BitsPerSample = (short)Math.Max((short)8, this._BitsPerSample);
                int len = (int)_Subchunk2Size;
                switch (this._BitsPerSample)
                {
                    case 8:
                        _Data8 = binReader.ReadBytes((int)len);
                        _Data = new double[len];
                        for (int i = 0; i < len; i++)
                            _Data[i] = (double)_Data8[i];
                        _Data8 = null;
                        break;
                    case 16:
                        len = (int)_Subchunk2Size / 2;
                        _Data = new double[len];
                        for (int i = 0; i < len; i++)
                            _Data[i] = (double)binReader.ReadInt16();
                        break;
                    case 32:
                        len = (int)_Subchunk2Size / 4;
                        _Data = new double[len];
                        for (int i = 0; i < len; i++)
                            _Data[i] = (double)binReader.ReadInt32();
                        break;
                }

                binReader.Close();
                binReader.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

		public override void ReadFromFile(String filename)
		{
			this._Filepath = filename;
            try
            {
                BinaryReader binReader = new BinaryReader(File.OpenRead(filename));

                this._ID = binReader.ReadBytes(4);
                this._ChunkSize = binReader.ReadUInt32();
                this._Format = binReader.ReadBytes(4);

                this._Subchunk1ID = binReader.ReadBytes(4);
                this._Subchunk1Size = binReader.ReadUInt32();
                this._AudioFormat = binReader.ReadInt16();
                this._NumChannels = binReader.ReadInt16();
                this._SampleRate = binReader.ReadUInt32();
                this._ByteRate = binReader.ReadUInt32();

                this._BlockAlign = binReader.ReadInt16();
                this._BitsPerSample = binReader.ReadInt16();

                binReader.BaseStream.Seek(_Subchunk1Size + 20, SeekOrigin.Begin);
                this._Subchunk2ID = binReader.ReadBytes(4);
                this._Subchunk2Size = binReader.ReadUInt32();

                this._BitsPerSample = (short)Math.Max((short)8, this._BitsPerSample);
                int len = (int)_Subchunk2Size;
                switch (this._BitsPerSample)
                {
                    case 8:
                        _Data8 = binReader.ReadBytes((int)len);
                        break;
                    case 16:
                        len = (int)_Subchunk2Size / 2;
                        _Data16 = new short[len];
                        _Data32 = new int[len];
                        for (int i = 0; i < len; i++)
                            _Data32[i] = _Data16[i] = binReader.ReadInt16();
                        break;
                    case 32:
                        len = (int)_Subchunk2Size / 4;
                        _Data32 = new int[len];
                        for (int i = 0; i < len; i++)
                            _Data32[i] = binReader.ReadInt32();
                        break;
                }

                binReader.Close();
                binReader.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
		}
	}
}
