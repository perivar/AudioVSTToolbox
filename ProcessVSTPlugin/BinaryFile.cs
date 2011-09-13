using System;
using System.IO;
using System.Text;

public class BinaryFile {

    /*
     * Class for reading and writing binary files.
     * 
     * An alternative for the conversion in this file is to use:
     * (UInt32) IPAddress.HostToNetworkOrder(byteSize)
     * longValue = IPAddress.HostToNetworkOrder(longValue);
	 * 
     * It's overloaded to handle shorts, ints, and longs. Or use:
     * BitConverter.ToInt32(ReadBytes(reader, 4, ByteOrder.BigEndian), 0);
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
	
	public static short ReadInt16(BinaryReader reader, ByteOrder byteOrder)
	{
		if (byteOrder == ByteOrder.LittleEndian)
		{
			return (short)reader.ReadInt16();
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
			return (int)reader.ReadInt32();
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
			return (int)reader.ReadInt64();
		}
		else // Big-Endian
		{
			return BitConverter.ToInt64(ReadBytes(reader, 8, ByteOrder.BigEndian), 0);
		}
	}

	public short ReadInt16(ByteOrder byteOrder)
	{
		return ReadInt16(binaryReader, byteOrder);
	}
	
	public int ReadInt32(ByteOrder byteOrder)
	{
		return ReadInt32(binaryReader, byteOrder);
	}
	
	public long ReadInt64(ByteOrder byteOrder)
	{
		return ReadInt64(binaryReader, byteOrder);
	}
	
	public short ReadInt16()
	{
		return ReadInt16(byteOrder);
	}

	public int ReadInt32()
	{
		return ReadInt32(byteOrder);
	}

	public long ReadInt64()
	{
		return ReadInt64(byteOrder);
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
	
	/*
    int8/System.Int8      byte     Signed 8-bit integer
    int16/System.Int16    short    Signed 16-bit integer
    int32/System.Int32    int      Signed 32-bit integer
    int64/System.Int64    long     Signed 64-bit integer
	 */
	public static byte[] ShortToByteArray(short value, bool invert) {
		if (invert) {
			return new byte[] {
				(byte) value,
				(byte) (value >> 8),
			};
		} else {
			return new byte[] {
				(byte) (value >> 8),
				(byte) value };
		}
	}

	public static byte[] IntToByteArray(int value, bool invert) {
		if (invert) {
			return new byte[] {
				(byte) value,
				(byte) (value >> 8),
				(byte) (value >> 16),
				(byte) (value >> 24) };
		} else {
			return new byte[] {
				(byte) (value >> 24),
				(byte) (value >> 16),
				(byte) (value >> 8),
				(byte) value };
		}
	}

	public static byte[] LongToByteArray(long value, bool invert) {
		if (invert) {
			return new byte[] {
				(byte) value,
				(byte) (value >> 8),
				(byte) (value >> 16),
				(byte) (value >> 24),
				(byte) (value >> 32),
				(byte) (value >> 40),
				(byte) (value >> 48),
				(byte) (value >> 56)
			};
		} else {
			return new byte[] {
				(byte) (value >> 56),
				(byte) (value >> 48),
				(byte) (value >> 40),
				(byte) (value >> 32),
				(byte) (value >> 24),
				(byte) (value >> 16),
				(byte) (value >> 8),
				(byte) value };
		}
	}

	public static int TwoByteArrayToInt(byte [] b, bool invert) {
		int i = 0;
		if (invert) {
			i = ((b[0] & 0xFF)) + ((b[1] & 0xFF) << 8);
		} else {
			i = ((b[1] & 0xFF)) + ((b[0] & 0xFF) << 8);
		}
		return i;
	}
	
	public static int FourByteArrayToInt(byte [] b, bool invert) {
		int i = 0;
		if (invert) {
			i = ((b[0] & 0xFF)) + ((b[1] & 0xFF) << 8) + ((b[2] & 0xFF) << 16) + ((b[3] & 0xFF) << 24);
		} else {
			i = ((b[3] & 0xFF)) + ((b[2] & 0xFF) << 8) + ((b[1] & 0xFF) << 16) + ((b[0] & 0xFF) << 24);
		}
		return i;
	}

	public static long EightByteArrayToLong(byte [] b, bool invert) {
		long l = 0;
		if (invert) {
			l = ((b[0] & 0xFF)) + ((b[1] & 0xFF) << 8) + ((b[2] & 0xFF) << 16) + ((b[3] & 0xFF) << 24) + ((b[4] & 0xFF) << 32) + ((b[5] & 0xFF) << 40) + ((b[6] & 0xFF) << 48) + ((b[7] & 0xFF) << 56);
		} else {
			l = ((b[7] & 0xFF)) + ((b[6] & 0xFF) << 8) + ((b[5] & 0xFF) << 16) + ((b[4] & 0xFF) << 24) + ((b[3] & 0xFF) << 32) + ((b[2] & 0xFF) << 40) + ((b[1] & 0xFF) << 48) + ((b[0] & 0xFF) << 56);
		}
		return l;
	}
	
	/**
	 * Convert the byte array to an int.
	 *
	 * @param b The byte array
	 * @return The integer
	 */
	public static int FourByteArrayToInt(byte[] b) {
		return FourByteArrayToInt(b, 0);
	}

	/**
	 * Convert the byte array to an int starting from the given offset.
	 *
	 * @param b The byte array
	 * @param offset The array offset
	 * @return The integer
	 */
	public static int FourByteArrayToInt(byte[] b, int offset) {
		int value = 0;
		for (int i = 0; i < 4; i++) {
			int shift = (4 - 1 - i) * 8;
			value += (b[i + offset] & 0x000000FF) << shift;
		}
		return value;
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
}
