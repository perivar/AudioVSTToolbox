using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace CommonUtils
{
	public static class XmlUtils
	{
		
		/// <summary>
		/// Save XML and optionally disable the UTF-8 BOM bytes at the top of the Xml document (EF BB BF)
		/// which is actually discouraged by the Unicode standard
		/// </summary>
		/// <param name="xdoc">XDocument</param>
		/// <param name="fileName">filename</param>
		/// <param name="disableUTF8BOMBytes">whether to disable the UTF8 BOM bytes</param>
		/// <param name="disableFormatting">whether to disable indentation when serializing</param>
		public static void SaveXDocument(XDocument xdoc, string fileName, bool disableUTF8BOMBytes = true, bool disableFormatting = false) {

			if (disableUTF8BOMBytes) {
				// Save XML and disable the UTF-8 BOM bytes at the top of the Xml document (EF BB BF)
				// which is actually discouraged by the Unicode standard:
				using (TextWriter writer = new StreamWriter(fileName, false, new UTF8Encoding(false)))
				{
					if (disableFormatting) {
						xdoc.Save(writer, SaveOptions.DisableFormatting);
					} else {
						xdoc.Save(writer);
					}
				}
			} else {
				if (disableFormatting) {
					xdoc.Save(fileName, SaveOptions.DisableFormatting);
				} else {
					xdoc.Save(fileName);
				}
			}
		}
		
		public static XElement GetXElement(this XmlNode node)
		{
			XDocument xDoc = new XDocument();
			using (XmlWriter xmlWriter = xDoc.CreateWriter()) {
				node.WriteTo(xmlWriter);
			}
			return xDoc.Root;
		}

		public static XmlNode GetXmlNode(this XElement element)
		{
			using (XmlReader xmlReader = element.CreateReader())
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlReader);
				return xmlDoc;
			}
		}
		
		public static XmlElement GetXmlElement(this XElement element, XmlDocument xmlDoc)
		{
			return xmlDoc.ReadNode(element.CreateReader()) as XmlElement;
		}
	}
}