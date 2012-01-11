using System;
using System.IO;
using System.Text;

namespace CommonUtils
{
	/// <summary>
	/// Description of BinaryFile
	/// </summary>
	
	public class BinaryFile {
		/*
		 * Class for reading and writing binary files.
		 * 
		 * Per Ivar Nerseth, 2011 - 2012
		 */
		public enum ByteOrder : int
		{
			LittleEndian,
			BigEndian
		}

		private ByteOrder byteOrder = ByteOrder.LittleEndian;
		
		private BinaryReader binaryReader = null;
		private BinaryWriter binaryWriter = null;
		private FileStream fs = null;
		private MemoryStream memStream = null;
		
		public BinaryFile(string filePath) : this(filePath, ByteOrder.LittleEndian, false) {}

		public BinaryFile(string filePath, ByteOrder byteOrder) : this(filePath, byteOrder, false) {}
		
		public BinaryFile(string filePath, ByteOrder byteOrder, bool createFile) {
			if (createFile) {
				fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
			} else {
				fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
			}
			binaryWriter = new BinaryWriter(fs, Encoding.Default);
			binaryReader = new BinaryReader(fs, Encoding.Default);
			this.byteOrder = byteOrder;
		}

		public BinaryFile(byte[] byteArray) : this(byteArray, ByteOrder.LittleEndian) {	}
		
		public BinaryFile(byte[] byteArray, ByteOrder byteOrder) {
			memStream = new MemoryStream(byteArray);
			binaryReader = new BinaryReader(memStream);

			// Set position to the beginning of the stream.
			memStream.Position = 0;

			this.byteOrder = byteOrder;
		}
		
		public long GetPosition() {
			long position = 0;
			if (null != fs) {
				position = fs.Position;
			} else if (null != memStream) {
				position = memStream.Position;
			}
			return position;
		}

		public void SetPosition(long position) {
			if (null != fs) {
				fs.Position = position;
			} else if (null != memStream) {
				memStream.Position = position;
			}
		}
		
		public byte ReadByte()
		{
			return binaryReader.ReadByte();
		}

		public byte[] ReadBytes(int size)
		{
			return ReadBytes(0, size);
		}

		public byte[] ReadBytes(int offset, int size)
		{
			if (size <= 0) return new byte[0];
			
			byte[] bytes = new byte[size];
			if (byteOrder == ByteOrder.LittleEndian) {
				int numBytesRead = binaryReader.Read(bytes, offset, size);
				return bytes;
			} else {
				int numBytesRead = binaryReader.Read(bytes, offset, size);
				Array.Reverse(bytes);
				return bytes;
			}
		}

		public byte[] ReadBytes(int offset, int size, ByteOrder byteOrder)
		{
			if (size <= 0) return new byte[0];
			
			byte[] bytes = new byte[size];
			if (byteOrder == ByteOrder.LittleEndian) {
				int numBytesRead = binaryReader.Read(bytes, offset, size);
				return bytes;
			} else {
				int numBytesRead = binaryReader.Read(bytes, offset, size);
				Array.Reverse(bytes);
				return bytes;
			}
		}

		public UInt16 ReadUInt16()
		{
			return ReadUInt16(byteOrder);
		}

		public UInt16 ReadUInt16(ByteOrder byteOrder)
		{
			return ReadUInt16(binaryReader, byteOrder);
		}
		
		public UInt32 ReadUInt32()
		{
			return ReadUInt32(byteOrder);
		}

		public UInt32 ReadUInt32(ByteOrder byteOrder)
		{
			return ReadUInt32(binaryReader, byteOrder);
		}
		
		public UInt64 ReadUInt64()
		{
			return ReadUInt64(byteOrder);
		}

		public UInt64 ReadUInt64(ByteOrder byteOrder)
		{
			return ReadUInt64(binaryReader, byteOrder);
		}
		
		public short ReadInt16()
		{
			return ReadInt16(byteOrder);
		}

		public short ReadInt16(ByteOrder byteOrder)
		{
			return ReadInt16(binaryReader, byteOrder);
		}
		
		public int ReadInt32()
		{
			return ReadInt32(byteOrder);
		}

		public int ReadInt32(ByteOrder byteOrder)
		{
			return ReadInt32(binaryReader, byteOrder);
		}
		
		public long ReadInt64()
		{
			return ReadInt64(byteOrder);
		}

		public long ReadInt64(ByteOrder byteOrder)
		{
			return ReadInt64(binaryReader, byteOrder);
		}
		
		public float ReadSingle()
		{
			return ReadSingle(byteOrder);
		}

		public float ReadSingle(ByteOrder byteOrder)
		{
			return ReadSingle(binaryReader, byteOrder);
		}

		public double ReadDouble()
		{
			return ReadDouble(byteOrder);
		}

		public double ReadDouble(ByteOrder byteOrder)
		{
			return ReadDouble(binaryReader, byteOrder);
		}
		
		public char[] ReadChars(int size) {
			return binaryReader.ReadChars(size);
		}
		
		public string ReadString(int size)
		{
			return new string(binaryReader.ReadChars(size));
		}

		public long Seek(long offset) {
			return SeekTo(offset);
		}
		
		public long SeekTo(long offset) {
			return binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
		}

		public bool Write(Int16 value) {
			if (byteOrder == ByteOrder.LittleEndian)
			{
				binaryWriter.Write(value);
			}
			else // Big-Endian
			{
				byte[] bytes = BitConverter.GetBytes(value);
				Array.Reverse(bytes);
				binaryWriter.Write( bytes );
			}
			return true;
		}

		public bool Write(Int32 value) {
			if (byteOrder == ByteOrder.LittleEndian)
			{
				binaryWriter.Write(value);
			}
			else // Big-Endian
			{
				byte[] bytes = BitConverter.GetBytes(value);
				Array.Reverse(bytes);
				binaryWriter.Write( bytes );
			}
			return true;
		}

		public bool Write(Int64 value) {
			if (byteOrder == ByteOrder.LittleEndian)
			{
				binaryWriter.Write(value);
			}
			else // Big-Endian
			{
				byte[] bytes = BitConverter.GetBytes(value);
				Array.Reverse(bytes);
				binaryWriter.Write( bytes );
			}
			return true;
		}

		public bool Write(string value, int length) {
			binaryWriter.Write(StringToByteArray(value, length) );
			return true;
		}
		
		public bool Write(string value) {
			binaryWriter.Write(StringToByteArray(value, value.Length) );
			return true;
		}

		public bool Write(byte value) {
			binaryWriter.Write(value);
			return true;
		}

		public bool Write(byte[] value) {
			if (byteOrder == ByteOrder.LittleEndian)
			{
				binaryWriter.Write(value);
			}
			else // Big-Endian
			{
				Array.Reverse( value );
				binaryWriter.Write( value );
			}
			return true;
		}

		public bool Write(byte[] value, ByteOrder byteOrder) {
			if (byteOrder == ByteOrder.LittleEndian)
			{
				binaryWriter.Write(value);
			}
			else // Big-Endian
			{
				Array.Reverse( value );
				binaryWriter.Write( value );
			}
			return true;
		}
		
		public bool Write(char value) {
			binaryWriter.Write(value);
			return true;
		}

		public bool Write(char[] value) {
			binaryWriter.Write(value);
			return true;
		}

		public bool Close()
		{
			binaryReader.Close();
			binaryWriter.Close();
			fs.Close();
			return true;
		}

		/********************************
		 *
		 * 		STATIC READ METHODS
		 * 
		 ********************************/

		public static byte[] ReadBytes(BinaryReader reader, int fieldSize, ByteOrder byteOrder)
		{
			byte[] bytes = new byte[fieldSize];
			if (byteOrder == ByteOrder.LittleEndian) {
				return reader.ReadBytes(fieldSize);
			} else {
				for (int i = fieldSize - 1; i > -1; i--)
					bytes[i] = reader.ReadByte();
				return bytes;
			}
		}
		
		public static short ReadInt16(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadInt16();
			}
			else // Big-Endian
			{
				return BitConverter.ToInt16(ReadBytes(reader, 2, ByteOrder.BigEndian), 0);
			}
		}
		
		public static int ReadInt32(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadInt32();
			}
			else // Big-Endian
			{
				return BitConverter.ToInt32(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
			}
		}
		
		public static long ReadInt64(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadInt64();
			}
			else // Big-Endian
			{
				return BitConverter.ToInt64(ReadBytes(reader, 8, ByteOrder.BigEndian), 0);
			}
		}

		public static UInt16 ReadUInt16(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadUInt16();
			}
			else // Big-Endian
			{
				return BitConverter.ToUInt16(ReadBytes(reader, 2, ByteOrder.BigEndian), 0);
			}
		}

		public static UInt32 ReadUInt32(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadUInt32();
			}
			else // Big-Endian
			{
				return BitConverter.ToUInt32(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
			}
		}
		
		public static UInt64 ReadUInt64(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadUInt64();
			}
			else // Big-Endian
			{
				return BitConverter.ToUInt64(ReadBytes(reader, 8, ByteOrder.BigEndian), 0);
			}
		}
		
		public static float ReadSingle(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadSingle();
			}
			else // Big-Endian
			{
				return BitConverter.ToSingle(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
			}
		}

		public static double ReadDouble(BinaryReader reader, ByteOrder byteOrder)
		{
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return reader.ReadDouble();
			}
			else // Big-Endian
			{
				return BitConverter.ToDouble(ReadBytes(reader, 8, ByteOrder.BigEndian), 0);
			}
		}
		
		
		/********************************
		 *
		 * 		UTILITY METHODS
		 * 
		 ********************************/

		public static byte[] HexStringToByteArray(string s)
		{
			var r = new byte[s.Length / 2];
			for (int i = 0; i < s.Length; i += 2)
			{
				r[i / 2] = (byte)Convert.ToInt32(s.Substring(i, 2), 16);
			}
			return r;
		}
		
		public static float ByteArrayToSingle(byte [] b, ByteOrder byteOrder) {
			if (b == null || b.Length < 4) return -1;
			
			if (byteOrder == ByteOrder.LittleEndian)
			{
				return BitConverter.ToSingle(b, 0);
			}
			else // Big-Endian
			{
				byte [] bClone = (byte[])b.Clone();
				Array.Reverse(bClone);
				return BitConverter.ToSingle(bClone, 0);
			}
		}

		public static Int16 ByteArrayToInt16(byte [] b, ByteOrder byteOrder) {
			if (b == null || b.Length < 2) return -1;

			if (byteOrder == ByteOrder.LittleEndian)
			{
				return BitConverter.ToInt16(b, 0);
			}
			else // Big-Endian
			{
				byte [] bClone = (byte[])b.Clone();
				Array.Reverse(bClone);
				return BitConverter.ToInt16(bClone, 0);
			}
		}

		public static Int32 ByteArrayToInt32(byte [] b, ByteOrder byteOrder) {
			if (b == null || b.Length < 4) return -1;

			if (byteOrder == ByteOrder.LittleEndian)
			{
				return BitConverter.ToInt32(b, 0);
			}
			else // Big-Endian
			{
				byte [] bClone = (byte[])b.Clone();
				Array.Reverse(bClone);
				return BitConverter.ToInt32(bClone, 0);
			}
		}
		
		public static Int64 ByteArrayToInt64(byte [] b, ByteOrder byteOrder) {
			if (b == null || b.Length < 8) return -1;

			if (byteOrder == ByteOrder.LittleEndian)
			{
				return BitConverter.ToInt64(b, 0);
			}
			else // Big-Endian
			{
				byte [] bClone = (byte[])b.Clone();
				Array.Reverse(bClone);
				return BitConverter.ToInt64(bClone, 0);
			}
		}

		
		public static int HexToInt(string sHexString) {
			return Convert.ToInt32(sHexString, 16);
		}
		
		public static byte[] CharArrayToByteArray(char[] charArray)
		{
			//System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			return Encoding.Default.GetBytes(charArray);
		}

		public static byte[] StringToByteArray(string StringToConvert, int length)
		{
			char[] CharArray = StringToConvert.ToCharArray();
			byte[] ByteArray = new byte[length];
			for (int i = 0; i < length; i++)
			{
				if (i < CharArray.Length) ByteArray[i] = Convert.ToByte(CharArray[i]);
			}
			return ByteArray;
		}

		public static byte[] StringToByteArray(string str)
		{
			//System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			return Encoding.Default.GetBytes(str);
		}

		public static string ByteArrayToString(byte[] byteArray)
		{
			//System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
			return Encoding.Default.GetString(byteArray);
		}
		
		public static byte[] AppendArrays(byte[] a, byte[] b)
		{
			byte[] c = new byte[a.Length + b.Length]; // just one array allocation
			Buffer.BlockCopy(a, 0, c, 0, a.Length);
			Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
			return c;
		}
		
		/// <summary>
		/// Function to save byte array to a file
		/// </summary>
		/// <param name="_FileName">File name to save byte array</param>
		/// <param name="_ByteArray">Byte array to save to external file</param>
		/// <returns>Return true if byte array save successfully, if not return false</returns>
		public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
		{
			try
			{
				// Open file for reading
				System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

				// Writes a block of bytes to this stream using data from a byte array.
				_FileStream.Write(_ByteArray, 0, _ByteArray.Length);

				// close file stream
				_FileStream.Close();

				return true;
			}
			catch (Exception _Exception)
			{
				// Error
				Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
			}

			// error occured, return false
			return false;
		}
		
		public static float[] FloatArrayFromStream(System.IO.MemoryStream stream)
		{
			return FloatArrayFromByteArray(stream.GetBuffer());
		}

		public static float[] FloatArrayFromByteArray(byte[] input)
		{
			float[] output = new float[input.Length / 4];
			Buffer.BlockCopy(input, 0, output, 0, input.Length);
			return output;
		}
	}
}