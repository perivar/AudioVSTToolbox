using System;
using System.Xml;
using System.IO;

public class FXP {

	private string m_chunkMagic;
	private int m_byteSize;
	private string m_fxMagic;
	private int m_version;
	private string m_fxID;
	private int m_fxVersion;
	private int m_numPrograms;
	private string m_name;
	private int m_chunkSize;
	private string m_chunkData;
	private XmlDocument m_xmlDocument;

	public string chunkMagic { get { return m_chunkMagic;  } set { m_chunkMagic = value; } }
	public int byteSize { get { return m_byteSize;  } set { m_byteSize = value; } }
	public string fxMagic { get { return m_fxMagic;  } set { m_fxMagic = value; } }
	public int version { get { return m_version;  } set { m_version = value; } }
	public string fxID { get { return m_fxID;  } set { m_fxID = value; } }
	public int fxVersion { get { return m_fxVersion;  } set { m_fxVersion = value; } }
	public int numPrograms { get { return m_numPrograms;  } set { m_numPrograms = value; } }
	public string name { get { return m_name;  } set { m_name = value; } }
	public int chunkSize { get { return m_chunkSize;  } set { m_chunkSize = value; } }
	public string chunkData { get { return m_chunkData;  } set { m_chunkData = value; } }
	public XmlDocument xmlDocument { get { return m_xmlDocument;  } set { m_xmlDocument = value; } }

	public FXP() {
		
	}
	
	public void WriteFile( string filePath ) {

		BinaryFile bf = new BinaryFile(filePath, BinaryFile.ByteOrder.BigEndian, true);

		string xmlChunkData = "";
		if (xmlDocument != null) {
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlDocument.WriteTo(xmlTextWriter);
			xmlTextWriter.Flush();
			xmlChunkData = stringWriter.ToString().Replace("'", "&apos;");
			chunkSize = xmlChunkData.Length;
		} else {
			xmlChunkData = this.chunkData;
		}
		byteSize = 52 + chunkSize;
		
		Console.Out.WriteLine("Writing FXP to {0} ...", filePath);
		Console.Out.WriteLine(">chunkMagic: {0}", chunkMagic);
		Console.Out.WriteLine(">byteSize: {0}", byteSize);
		Console.Out.WriteLine(">fxMagic: {0}", fxMagic);
		Console.Out.WriteLine(">version: {0}", version);
		Console.Out.WriteLine(">fxID: {0}", fxID);
		Console.Out.WriteLine(">fxVersion: {0}", fxVersion);
		Console.Out.WriteLine(">numPrograms: {0}", numPrograms);
		Console.Out.WriteLine(">name: {0}", name);
		Console.Out.WriteLine(">chunkSize: {0}", chunkSize);

		bf.Write( chunkMagic );							// chunkMagic, 4
		bf.Write( byteSize );							// byteSize = 4
		bf.Write( fxMagic );							// fxMagic, 4
		bf.Write( version );							// version, 4
		bf.Write( fxID );								// fxID, 4
		bf.Write( fxVersion );							// fxVersion, 4
		bf.Write( numPrograms );						// numPrograms, 4
		bf.Write( name, 28 );							// name, 28
		bf.Write( chunkSize );							// chunkSize, 4
		bf.Write( xmlChunkData );						// chunkData, <chunkSize>
		bf.Close();
	}

	public void ReadFile( string filePath ) {
		BinaryFile bf = new BinaryFile(filePath, BinaryFile.ByteOrder.BigEndian);
		
		chunkMagic = bf.ReadString(4);
		byteSize = bf.ReadInt32();
		fxMagic = bf.ReadString(4);
		version = bf.ReadInt32();
		fxID = bf.ReadString(4);
		fxVersion = bf.ReadInt32();
		numPrograms = bf.ReadInt32();
		name = bf.ReadString(28);
		chunkSize = bf.ReadInt32();
		chunkData = bf.ReadString(chunkSize);
		bf.Close();
		
		// read the xml chunk into memory
		xmlDocument = new XmlDocument();
		try {
			xmlDocument.LoadXml(chunkData);
		} catch (XmlException xe) {
			Console.Out.WriteLine("No XML found");
		}
	}
	
	public FXP(byte[] values) {

		BinaryFile bf = new BinaryFile(values, BinaryFile.ByteOrder.BigEndian);

		chunkMagic = bf.ReadString(4);
		byteSize = bf.ReadInt32();
		fxMagic = bf.ReadString(4);
		version = bf.ReadInt32();
		fxID = bf.ReadString(4);
		fxVersion = bf.ReadInt32();
		numPrograms = bf.ReadInt32();
		name = bf.ReadString(28);
		chunkSize = bf.ReadInt32();
		chunkData = bf.ReadString(chunkSize);
		
		// read the xml chunk into memory
		xmlDocument = new XmlDocument();
		try {
			xmlDocument.LoadXml(chunkData);
		} catch (XmlException xe) {
			Console.Out.WriteLine("No XML found");
		}
	}
}
