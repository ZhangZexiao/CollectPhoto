using System;
using System.Collections.Generic;
using System.Xml;
namespace TagLib.Xmp
{
	internal static class XmlNodeExtensions
	{
		public static bool In(this XmlNode node,string ns)
		{
			return node.NamespaceURI==ns;
		}
		public static bool Is(this XmlNode node,string ns,string name)
		{
			return node.In(ns)&&node.LocalName==name;
		}
		public static bool IsCoreSyntax(this XmlNode node)
		{
			return node.In(XmpTag.RDF_NS)&&(node.LocalName==XmpTag.RDF_URI||node.LocalName==XmpTag.ID_URI||node.LocalName==XmpTag.ABOUT_URI||node.LocalName==XmpTag.PARSE_TYPE_URI||node.LocalName==XmpTag.RESOURCE_URI||node.LocalName==XmpTag.NODE_ID_URI||node.LocalName==XmpTag.DATA_TYPE_URI);
		}
		public static bool IsOld(this XmlNode node)
		{
			return node.In(XmpTag.RDF_NS)&&(node.LocalName==XmpTag.ABOUT_EACH_URI||node.LocalName==XmpTag.ABOUT_EACH_PREFIX_URI||node.LocalName==XmpTag.BAG_ID_URI);
		}
		public static bool IsNodeElement(this XmlNode node)
		{
			return!node.IsCoreSyntax()&& !node.Is(XmpTag.RDF_NS,XmpTag.LI_URI)&& !node.IsOld();
		}
		public static bool IsPropertyElement(this XmlNode node)
		{
			return!node.IsCoreSyntax()&& !node.Is(XmpTag.RDF_NS,XmpTag.DESCRIPTION_URI)&& !node.IsOld();
		}
		public static bool IsPropertyAttribute(this XmlNode node)
		{
			return node is XmlAttribute&& !node.IsCoreSyntax()&& !node.Is(XmpTag.RDF_NS,XmpTag.DESCRIPTION_URI)&& !node.Is(XmpTag.RDF_NS,XmpTag.LI_URI)&& !node.IsOld();
		}
	}
}
