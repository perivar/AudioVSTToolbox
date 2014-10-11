using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CommonUtils
{
	public static class XmlUtils
	{
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