using System;
using System.Xml;
using System.IO;

public class FXP {
	/*
	 * Class for reading and writing Steinberg Preset files and Bank files (fxp and fxb files).
	 * Per Ivar Nerseth, 2011
	 */
	
	// Preset (Program) (.fxp) without chunk (magic = 'FxCk')
	// Preset (Program) (.fxp) with chunk (magic = 'FPCh')
	/*
    typedef struct
    {
        char chunkMagic[4];     // 'CcnK'
        long byteSize;          // of this chunk, excl. magic + byteSize
        char fxMagic[4];        // 'FPCh'
        long version;
        char fxID[4];           // fx unique id
        long fxVersion;
        long numPrograms;
        char name[28];
        long chunkSize;
        unsigned char chunkData[chunkSize];
    } fxProgramSet;
	 */
	// Bank (.fxb) without chunk (magic = 'FxBk')
	// Bank (.fxb) with chunk (magic = 'FBCh')
	/*
    typedef struct
    {
        char chunkMagic[4];     // 'CcnK'
        long byteSize;          // of this chunk, excl. magic + byteSize
        char fxMagic[4];        // 'FBCh'
        long version;
        char fxID[4];           // fx unique id
        long fxVersion;
        long numPrograms;
        char future[128];
        long chunkSize;
        unsigned char chunkData[chunkSize];
    } fxChunkSet;
	 */

	private string m_chunkMagic;    // 'CcnK'
	private int m_byteSize;         // of this chunk, excl. magic + byteSize
	private string m_fxMagic;       // 'FxCk', 'FxBk', 'FBCh' or 'FPCh'
	private int m_version;
	private string m_fxID;
	private int m_fxVersion;
	private int m_numPrograms;      // FPCh = numProgams, FxCk = numParams
	private string m_name;          // if FPCh
	private string m_future;        // if FBCh
	private int m_chunkSize;
	
	private string m_chunkData;
	private byte[] m_chunkDataByteArray;
	private XmlDocument m_xmlDocument;

	public string chunkMagic { get { return m_chunkMagic;  } set { m_chunkMagic = value; } }
	public int byteSize { get { return m_byteSize;  } set { m_byteSize = value; } }
	public string fxMagic { get { return m_fxMagic;  } set { m_fxMagic = value; } }
	public int version { get { return m_version;  } set { m_version = value; } }
	public string fxID { get { return m_fxID;  } set { m_fxID = value; } }
	public int fxVersion { get { return m_fxVersion;  } set { m_fxVersion = value; } }
	public int numPrograms { get { return m_numPrograms;  } set { m_numPrograms = value; } }
	public string name { get { return m_name;  } set { m_name = value; } }
	public string future { get { return m_future; } set { m_future = value; } }
	public int chunkSize { get { return m_chunkSize; } set { m_chunkSize = value; } }
	
	public string chunkData {
		get {
			return m_chunkData;
		} set {
			m_chunkData = value;
		}
	}
	
	public byte[] chunkDataByteArray {
		get {
			return m_chunkDataByteArray;
		}
		set {
			m_chunkDataByteArray = value;
		}
	}
	
	public XmlDocument xmlDocument {
		get {
			return m_xmlDocument;
		}
		set {
			m_xmlDocument = value;
		}
	}

	public FXP() {
		
	}
	
	public void WriteFile( string filePath ) {

		BinaryFile bf = new BinaryFile(filePath, BinaryFile.ByteOrder.BigEndian, true);

		// determine if the chunkdata is saved as XML
		bool writeXMLChunkData = false;
		string xmlChunkData = "";
		if (xmlDocument != null) {
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlDocument.WriteTo(xmlTextWriter);
			xmlTextWriter.Flush();
			xmlChunkData = stringWriter.ToString().Replace("'", "&apos;");
			chunkSize = xmlChunkData.Length;
			writeXMLChunkData = true;
		}

		if (chunkMagic != "CcnK")
		{
			Console.Out.WriteLine("Cannot save the preset file. Missing preset header information.");
			return;
		}

		Console.Out.WriteLine("Writing FXP to {0} ...", filePath);
		Console.Out.WriteLine(">chunkMagic: {0}", chunkMagic);
		Console.Out.WriteLine(">byteSize: {0}", byteSize);
		Console.Out.WriteLine(">fxMagic: {0}", fxMagic);
		Console.Out.WriteLine(">version: {0}", version);
		Console.Out.WriteLine(">fxID: {0}", fxID);
		Console.Out.WriteLine(">fxVersion: {0}", fxVersion);
		Console.Out.WriteLine(">numPrograms: {0}", numPrograms);
		Console.Out.WriteLine(">name: {0}", name);
		Console.Out.WriteLine(">future: {0}", future);
		Console.Out.WriteLine(">chunkSize: {0}", chunkSize);

		bf.Write(chunkMagic);							// chunkMagic, 4

		// check what preset type we are saving
		if (fxMagic == "FBCh")
		{
			// Bank with Chunk Data
			byteSize = 152 + chunkSize;

			bf.Write(byteSize);							// byteSize = 4
			bf.Write(fxMagic);							// fxMagic, 4
			bf.Write(version);							// version, 4
			bf.Write(fxID);								// fxID, 4
			bf.Write(fxVersion);						// fxVersion, 4
			bf.Write(numPrograms);						// numPrograms, 4
			bf.Write(future, 128);						// future, 128
			bf.Write(chunkSize);						// chunkSize, 4
			
			if (writeXMLChunkData) {
				bf.Write(xmlChunkData);						// chunkData, <chunkSize>
			} else {
				// Even though the main FXP is BigEndian format the preset chunk is saved in LittleEndian format
				bf.Write(m_chunkDataByteArray, BinaryFile.ByteOrder.LittleEndian);
			}
		}
		else if (fxMagic == "FPCh")
		{
			// Preset with Chunk Data
			byteSize = 52 + chunkSize;

			bf.Write(byteSize);							// byteSize = 4
			bf.Write(fxMagic);							// fxMagic, 4
			bf.Write(version);							// version, 4
			bf.Write(fxID);								// fxID, 4
			bf.Write(fxVersion);						// fxVersion, 4
			bf.Write(numPrograms);						// numPrograms, 4
			bf.Write(name, 28);							// name, 28
			bf.Write(chunkSize);						// chunkSize, 4

			if (writeXMLChunkData) {
				bf.Write(xmlChunkData);						// chunkData, <chunkSize>
			} else {
				// Even though the main FXP is BigEndian format the preset chunk is saved in LittleEndian format
				bf.Write(m_chunkDataByteArray, BinaryFile.ByteOrder.LittleEndian);
			}
		}
		
		bf.Close();
	}

	public void ReadFile( string filePath ) {
		BinaryFile bf = new BinaryFile(filePath, BinaryFile.ByteOrder.BigEndian);
		
		chunkMagic = bf.ReadString(4);
		if (chunkMagic != "CcnK")
		{
			Console.Out.WriteLine("Error reading file. Missing preset header information.");
		}

		byteSize = bf.ReadInt32();
		fxMagic = bf.ReadString(4);

		if (fxMagic == "FBCh")
		{
			// Bank with Chunk Data
			version = bf.ReadInt32();
			fxID = bf.ReadString(4);
			fxVersion = bf.ReadInt32();
			numPrograms = bf.ReadInt32();
			future = bf.ReadString(128);
			chunkSize = bf.ReadInt32();

			// Even though the main FXP is BigEndian format the preset chunk is saved in LittleEndian format
			m_chunkDataByteArray = new byte[chunkSize];
			m_chunkDataByteArray = bf.ReadBytes(0, chunkSize, BinaryFile.ByteOrder.LittleEndian);
			chunkData = BinaryFile.ByteArrayToString(m_chunkDataByteArray);
		}
		else if (fxMagic == "FPCh")
		{
			// Preset with Chunk Data
			version = bf.ReadInt32();
			fxID = bf.ReadString(4);
			fxVersion = bf.ReadInt32();
			numPrograms = bf.ReadInt32();
			name = bf.ReadString(28);
			chunkSize = bf.ReadInt32();

			// Even though the main FXP is BigEndian format the preset chunk is saved in LittleEndian format
			m_chunkDataByteArray = new byte[chunkSize];
			m_chunkDataByteArray = bf.ReadBytes(0, chunkSize, BinaryFile.ByteOrder.LittleEndian);
			chunkData = BinaryFile.ByteArrayToString(m_chunkDataByteArray);
		}
		
		bf.Close();

		Console.Out.WriteLine("Loading FXP from {0} ...", filePath);
		Console.Out.WriteLine(">chunkMagic: {0}", chunkMagic);
		Console.Out.WriteLine(">byteSize: {0}", byteSize);
		Console.Out.WriteLine(">fxMagic: {0}", fxMagic);
		Console.Out.WriteLine(">version: {0}", version);
		Console.Out.WriteLine(">fxID: {0}", fxID);
		Console.Out.WriteLine(">fxVersion: {0}", fxVersion);
		Console.Out.WriteLine(">numPrograms: {0}", numPrograms);
		Console.Out.WriteLine(">name: {0}", name);
		Console.Out.WriteLine(">future: {0}", future);
		Console.Out.WriteLine(">chunkSize: {0}", chunkSize);
		
		// read the xml chunk into memory
		xmlDocument = new XmlDocument();
		try {
			xmlDocument.LoadXml(chunkData);
		} catch (XmlException) {
			Console.Out.WriteLine("No XML found");
		}
	}
	
	public FXP(byte[] values) {

		BinaryFile bf = new BinaryFile(values, BinaryFile.ByteOrder.BigEndian);

		chunkMagic = bf.ReadString(4);
		if (chunkMagic != "CcnK")
		{
			Console.Out.WriteLine("Error reading file. Missing preset header information.");
		}

		byteSize = bf.ReadInt32();
		fxMagic = bf.ReadString(4);

		if (fxMagic == "FBCh")
		{
			// Bank with Chunk Data
			version = bf.ReadInt32();
			fxID = bf.ReadString(4);
			fxVersion = bf.ReadInt32();
			numPrograms = bf.ReadInt32();
			future = bf.ReadString(128);
			chunkSize = bf.ReadInt32();
			
			// Even though the main FXP is BigEndian format the preset chunk is saved in LittleEndian format
			m_chunkDataByteArray = new byte[chunkSize];
			m_chunkDataByteArray = bf.ReadBytes(0, chunkSize, BinaryFile.ByteOrder.LittleEndian);
			chunkData = BinaryFile.ByteArrayToString(m_chunkDataByteArray);
		}
		else if (fxMagic == "FPCh")
		{
			// Preset with Chunk Data
			version = bf.ReadInt32();
			fxID = bf.ReadString(4);
			fxVersion = bf.ReadInt32();
			numPrograms = bf.ReadInt32();
			name = bf.ReadString(28);
			chunkSize = bf.ReadInt32();
			
			// Even though the main FXP is BigEndian format the preset chunk is saved in LittleEndian format
			m_chunkDataByteArray = new byte[chunkSize];
			m_chunkDataByteArray = bf.ReadBytes(0, chunkSize, BinaryFile.ByteOrder.LittleEndian);
			chunkData = BinaryFile.ByteArrayToString(m_chunkDataByteArray);
		}

		bf.Close();
		
		// read the xml chunk into memory
		xmlDocument = new XmlDocument();
		try {
			xmlDocument.LoadXml(chunkData);
		} catch (XmlException) {
			Console.Out.WriteLine("No XML found");
		}
	}
}
