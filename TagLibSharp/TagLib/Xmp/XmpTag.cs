using System;
using System.Collections.Generic;
using System.Xml;
using TagLib.Image;
using TagLib.IFD.Entries;
namespace TagLib.Xmp
{
	public class XmpTag:
	ImageTag
	{
		static XmpTag()
		{
			Initialize();
		}
#region Parsing speedup
		private Dictionary<string,Dictionary<string,XmpNode>>nodes;
		public static readonly string ADOBE_X_NS="adobe:ns:meta/";
		public static readonly string CRS_NS="http://ns.adobe.com/camera-raw-settings/1.0/";
		public static readonly string DC_NS="http://purl.org/dc/elements/1.1/";
		public static readonly string EXIF_NS="http://ns.adobe.com/exif/1.0/";
		public static readonly string EXIF_AUX_NS="http://ns.adobe.com/exif/1.0/aux/";
		public static readonly string JOB_NS="http://ns.adobe.com/xap/1.0/sType/Job#";
		public static readonly string MS_PHOTO_NS="http://ns.microsoft.com/photo/1.0/";
		public static readonly string PHOTOSHOP_NS="http://ns.adobe.com/photoshop/1.0/";
		public static readonly string PRISM_NS="http://prismstandard.org/namespaces/basic/2.1/";
		public static readonly string RDF_NS="http://www.w3.org/1999/02/22-rdf-syntax-ns#";
		public static readonly string STDIM_NS="http://ns.adobe.com/xap/1.0/sType/Dimensions#";
		public static readonly string TIFF_NS="http://ns.adobe.com/tiff/1.0/";
		public static readonly string XAP_NS="http://ns.adobe.com/xap/1.0/";
		public static readonly string XAP_BJ_NS="http://ns.adobe.com/xap/1.0/bj/";
		public static readonly string XAP_MM_NS="http://ns.adobe.com/xap/1.0/mm/";
		public static readonly string XAP_RIGHTS_NS="http://ns.adobe.com/xap/1.0/rights/";
		public static readonly string XML_NS="http://www.w3.org/XML/1998/namespace";
		public static readonly string XMLNS_NS="http://www.w3.org/2000/xmlns/";
		public static readonly string XMPTG_NS="http://ns.adobe.com/xap/1.0/t/pg/";
		internal static readonly string ABOUT_URI="about";
		internal static readonly string ABOUT_EACH_URI="aboutEach";
		internal static readonly string ABOUT_EACH_PREFIX_URI="aboutEachPrefix";
		internal static readonly string ALT_URI="Alt";
		internal static readonly string BAG_URI="Bag";
		internal static readonly string BAG_ID_URI="bagID";
		internal static readonly string DATA_TYPE_URI="datatype";
		internal static readonly string DESCRIPTION_URI="Description";
		internal static readonly string ID_URI="ID";
		internal static readonly string LANG_URI="lang";
		internal static readonly string LI_URI="li";
		internal static readonly string NODE_ID_URI="nodeID";
		internal static readonly string PARSE_TYPE_URI="parseType";
		internal static readonly string RDF_URI="RDF";
		internal static readonly string RESOURCE_URI="resource";
		internal static readonly string SEQ_URI="Seq";
		internal static readonly string VALUE_URI="value";
		static readonly NameTable NameTable=new NameTable();
		static bool initialized=false;
		static void Initialize()
		{
			if(initialized)
			{
				return;
			}
			lock(NameTable)
			{
				if(initialized)
				{
					return;
				}
				PrepareNamespaces();
				initialized=true;
			}
		}
		static void PrepareNamespaces()
		{
			AddNamespacePrefix("","");
			AddNamespacePrefix("x",ADOBE_X_NS);
			AddNamespacePrefix("crs",CRS_NS);
			AddNamespacePrefix("dc",DC_NS);
			AddNamespacePrefix("exif",EXIF_NS);
			AddNamespacePrefix("aux",EXIF_AUX_NS);
			AddNamespacePrefix("stJob",JOB_NS);
			AddNamespacePrefix("MicrosoftPhoto",MS_PHOTO_NS);
			AddNamespacePrefix("photoshop",PHOTOSHOP_NS);
			AddNamespacePrefix("prism",PRISM_NS);
			AddNamespacePrefix("rdf",RDF_NS);
			AddNamespacePrefix("stDim",STDIM_NS);
			AddNamespacePrefix("tiff",TIFF_NS);
			AddNamespacePrefix("xmp",XAP_NS);
			AddNamespacePrefix("xapBJ",XAP_BJ_NS);
			AddNamespacePrefix("xapMM",XAP_MM_NS);
			AddNamespacePrefix("xapRights",XAP_RIGHTS_NS);
			AddNamespacePrefix("xml",XML_NS);
			AddNamespacePrefix("xmlns",XMLNS_NS);
			AddNamespacePrefix("xmpTPg",XMPTG_NS);
			NameTable.Add(ABOUT_URI);
			NameTable.Add(ABOUT_EACH_URI);
			NameTable.Add(ABOUT_EACH_PREFIX_URI);
			NameTable.Add(ALT_URI);
			NameTable.Add(BAG_URI);
			NameTable.Add(BAG_ID_URI);
			NameTable.Add(DATA_TYPE_URI);
			NameTable.Add(DESCRIPTION_URI);
			NameTable.Add(ID_URI);
			NameTable.Add(LANG_URI);
			NameTable.Add(LI_URI);
			NameTable.Add(NODE_ID_URI);
			NameTable.Add(PARSE_TYPE_URI);
			NameTable.Add(RDF_URI);
			NameTable.Add(RESOURCE_URI);
			NameTable.Add(SEQ_URI);
			NameTable.Add(VALUE_URI);
		}
		public static Dictionary<string,string>NamespacePrefixes=new Dictionary<string,string>();
		static int anon_ns_count=0;
		static void AddNamespacePrefix(string prefix,string ns)
		{
			NameTable.Add(ns);
			NamespacePrefixes.Add(ns,prefix);
		}
#endregion
#region Constructors
		public XmpTag()
		{
			NodeTree=new XmpNode(String.Empty,String.Empty);
			nodes=new Dictionary<string,Dictionary<string,XmpNode>>();
		}
		public XmpTag(string data,TagLib.File file)
		{
			if(data[data.Length-1]=='\0')
			{
				data=data.Substring(0,data.Length-1);
			}
			XmlDocument doc=new XmlDocument(NameTable);
			doc.LoadXml(data);
			XmlNamespaceManager nsmgr=new XmlNamespaceManager(doc.NameTable);
			nsmgr.AddNamespace("x",ADOBE_X_NS);
			nsmgr.AddNamespace("rdf",RDF_NS);
			XmlNode node=doc.SelectSingleNode("/x:xmpmeta/rdf:RDF",nsmgr);
			node=node??doc.SelectSingleNode("/x:xapmeta/rdf:RDF",nsmgr);
			if(node==null)
			{
				throw new CorruptFileException();
			}
			NodeTree=ParseRDF(node,file);
			AcceptVisitors();
		}
#endregion
#region Private Methods
		private XmpNode ParseRDF(XmlNode rdf_node,TagLib.File file)
		{
			XmpNode top=new XmpNode(String.Empty,String.Empty);
			foreach(XmlNode node in rdf_node.ChildNodes)
			{
				if(node is XmlWhitespace)
				{
					continue;
				}
				if(node.Is(RDF_NS,DESCRIPTION_URI))
				{
					var attr=node.Attributes.GetNamedItem(RDF_NS,ABOUT_URI)
					as XmlAttribute;
					if(attr!=null)
					{
						if(top.Name!=String.Empty&&top.Name!=attr.InnerText)
						{
							throw new CorruptFileException("Multiple inconsistent rdf:about values!");
						}
						top.Name=attr.InnerText;
					}
					continue;
				}
				file.MarkAsCorrupt("Cannot have anything other than rdf:Description at the top level");
				return top;
			}
			ParseNodeElementList(top,rdf_node);
			return top;
		}
		private void ParseNodeElementList(XmpNode parent,XmlNode xml_parent)
		{
			foreach(XmlNode node in xml_parent.ChildNodes)
			{
				if(node is XmlWhitespace)
				{
					continue;
				}
				ParseNodeElement(parent,node);
			}
		}
		private void ParseNodeElement(XmpNode parent,XmlNode node)
		{
			if(!node.IsNodeElement())
			{
				throw new CorruptFileException("Unexpected node found, invalid RDF?");
			}
			if(node.Is(RDF_NS,SEQ_URI))
			{
				parent.Type=XmpNodeType.Seq;
			}
			else if(node.Is(RDF_NS,ALT_URI))
			{
				parent.Type=XmpNodeType.Alt;
			}
			else if(node.Is(RDF_NS,BAG_URI))
			{
				parent.Type=XmpNodeType.Bag;
			}
			else if(node.Is(RDF_NS,DESCRIPTION_URI))
			{
				parent.Type=XmpNodeType.Struct;
			}
			else
			{
				throw new Exception("Unknown nodeelement found! Perhaps an unimplemented collection?");
			}
			foreach(XmlAttribute attr in node.Attributes)
			{
				if(attr.In(XMLNS_NS))
				{
					continue;
				}
				if(attr.Is(RDF_NS,ID_URI)||attr.Is(RDF_NS,NODE_ID_URI)||attr.Is(RDF_NS,ABOUT_URI))
				{
					continue;
				}
				if(attr.Is(XML_NS,LANG_URI))
				{
					throw new CorruptFileException("xml:lang is not allowed here!");
				}
				parent.AddChild(new XmpNode(attr.NamespaceURI,attr.LocalName,attr.InnerText));
			}
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child is XmlWhitespace||child is XmlComment)
				{
					continue;
				}
				ParsePropertyElement(parent,child);
			}
		}
		private void ParsePropertyElement(XmpNode parent,XmlNode node)
		{
			int count=0;
			bool has_other=false;
			foreach(XmlAttribute attr in node.Attributes)
			{
				if(!attr.In(XMLNS_NS))
				{
					count++;
				}
				if(!attr.Is(XML_NS,LANG_URI)&& !attr.Is(RDF_NS,ID_URI)&& !attr.In(XMLNS_NS))
				{
					has_other=true;
				}
			}
			if(count>3)
			{
				ParseEmptyPropertyElement(parent,node);
			}
			else
			{
				if(!has_other)
				{
					if(!node.HasChildNodes)
					{
						ParseEmptyPropertyElement(parent,node);
					}
					else
					{
						bool only_text=true;
						foreach(XmlNode child in node.ChildNodes)
						{
							if(!(child is XmlText))
							{
								only_text=false;
							}
						}
						if(only_text)
						{
							ParseLiteralPropertyElement(parent,node);
						}
						else
						{
							ParseResourcePropertyElement(parent,node);
						}
					}
				}
				else
				{
					foreach(XmlAttribute attr in node.Attributes)
					{
						if(attr.Is(XML_NS,LANG_URI)||attr.Is(RDF_NS,ID_URI)||attr.In(XMLNS_NS))
						{
							continue;
						}
						if(attr.Is(RDF_NS,DATA_TYPE_URI))
						{
							ParseLiteralPropertyElement(parent,node);
						}
						else if(!attr.Is(RDF_NS,PARSE_TYPE_URI))
						{
							ParseEmptyPropertyElement(parent,node);
						}
						else if(attr.InnerText.Equals("Resource"))
						{
							ParseTypeResourcePropertyElement(parent,node);
						}
						else
						{
							throw new CorruptFileException(String.Format("This is not allowed in XMP! Bad XMP: {0}",node.OuterXml));
						}
					}
				}
			}
		}
		private void ParseResourcePropertyElement(XmpNode parent,XmlNode node)
		{
			if(!node.IsPropertyElement())
			{
				throw new CorruptFileException("Invalid property");
			}
			XmpNode new_node=new XmpNode(node.NamespaceURI,node.LocalName);
			foreach(XmlAttribute attr in node.Attributes)
			{
				if(attr.Is(XML_NS,LANG_URI))
				{
					new_node.AddQualifier(new XmpNode(XML_NS,LANG_URI,attr.InnerText));
				}
				else if(attr.Is(RDF_NS,ID_URI)||attr.In(XMLNS_NS))
				{
					continue;
				}
				throw new CorruptFileException(String.Format("Invalid attribute: {0}",attr.OuterXml));
			}
			bool has_xml_children=false;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child is XmlWhitespace)
				{
					continue;
				}
				if(child is XmlText)
				{
					throw new CorruptFileException("Can't have text here!");
				}
				has_xml_children=true;
				ParseNodeElement(new_node,child);
			}
			if(!has_xml_children)
			{
				throw new CorruptFileException("Missing children for resource property element");
			}
			parent.AddChild(new_node);
		}
		private void ParseLiteralPropertyElement(XmpNode parent,XmlNode node)
		{
			if(!node.IsPropertyElement())
			{
				throw new CorruptFileException("Invalid property");
			}
			parent.AddChild(CreateTextPropertyWithQualifiers(node,node.InnerText));
		}
		private void ParseTypeResourcePropertyElement(XmpNode parent,XmlNode node)
		{
			if(!node.IsPropertyElement())
			{
				throw new CorruptFileException("Invalid property");
			}
			XmpNode new_node=new XmpNode(node.NamespaceURI,node.LocalName);
			new_node.Type=XmpNodeType.Struct;
			foreach(XmlNode attr in node.Attributes)
			{
				if(attr.Is(XML_NS,LANG_URI))
				{
					new_node.AddQualifier(new XmpNode(XML_NS,LANG_URI,attr.InnerText));
				}
			}
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child is XmlWhitespace||child is XmlComment)
				{
					continue;
				}
				ParsePropertyElement(new_node,child);
			}
			parent.AddChild(new_node);
		}
		private void ParseEmptyPropertyElement(XmpNode parent,XmlNode node)
		{
			if(!node.IsPropertyElement())
			{
				throw new CorruptFileException("Invalid property");
			}
			if(node.HasChildNodes)
			{
				throw new CorruptFileException(String.Format("Can't have content in this node! Node: {0}",node.OuterXml));
			}
			var rdf_value=node.Attributes.GetNamedItem(VALUE_URI,RDF_NS)
			as XmlAttribute;
			var rdf_resource=node.Attributes.GetNamedItem(RESOURCE_URI,RDF_NS)
			as XmlAttribute;
			var simple_prop_val=rdf_value??rdf_resource??null;
			if(simple_prop_val!=null)
			{
				string value=simple_prop_val.InnerText;
				parent.AddChild(CreateTextPropertyWithQualifiers(node,value));
				return;
			}
			var new_node=new XmpNode(node.NamespaceURI,node.LocalName);
			foreach(XmlAttribute a in node.Attributes)
			{
				if(a.Is(RDF_NS,ID_URI)||a.Is(RDF_NS,NODE_ID_URI))
				{
					continue;
				}
				else if(a.In(XMLNS_NS))
				{
					continue;
				}
				else if(a.Is(XML_NS,LANG_URI))
				{
					new_node.AddQualifier(new XmpNode(XML_NS,LANG_URI,a.InnerText));
				}
				new_node.AddChild(new XmpNode(a.NamespaceURI,a.LocalName,a.InnerText));
			}
			parent.AddChild(new_node);
		}
		private XmpNode CreateTextPropertyWithQualifiers(XmlNode node,string value)
		{
			XmpNode t=new XmpNode(node.NamespaceURI,node.LocalName,value);
			foreach(XmlAttribute attr in node.Attributes)
			{
				if(attr.In(XMLNS_NS))
				{
					continue;
				}
				if(attr.Is(RDF_NS,VALUE_URI)||attr.Is(RDF_NS,RESOURCE_URI))
				{
					continue;
				}
				t.AddQualifier(new XmpNode(attr.NamespaceURI,attr.LocalName,attr.InnerText));
			}
			return t;
		}
		private XmpNode NewNode(string ns,string name)
		{
			Dictionary<string,XmpNode>ns_nodes=null;
			if(!nodes.ContainsKey(ns))
			{
				ns_nodes=new Dictionary<string,XmpNode>();
				nodes.Add(ns,ns_nodes);
			}
			else
			{
				ns_nodes=nodes[ns];
			}
			if(ns_nodes.ContainsKey(name))
			{
				foreach(XmpNode child_node in NodeTree.Children)
				{
					if(child_node.Namespace==ns&&child_node.Name==name)
					{
						NodeTree.RemoveChild(child_node);
						break;
					}
				}
				ns_nodes.Remove(name);
			}
			XmpNode node=new XmpNode(ns,name);
			ns_nodes.Add(name,node);
			NodeTree.AddChild(node);
			return node;
		}
		private XmpNode NewNode(string ns,string name,XmpNodeType type)
		{
			XmpNode node=NewNode(ns,name);
			node.Type=type;
			return node;
		}
		private void RemoveNode(string ns,string name)
		{
			if(!nodes.ContainsKey(ns))
			{
				return;
			}
			foreach(XmpNode node in NodeTree.Children)
			{
				if(node.Namespace==ns&&node.Name==name)
				{
					NodeTree.RemoveChild(node);
					break;
				}
			}
			nodes[ns].Remove(name);
		}
		private void AcceptVisitors()
		{
			NodeTree.Accept(new NodeIndexVisitor(this));
		}
#endregion
#region Public Properties
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.XMP;
			}
		}
		public XmpNode NodeTree
		{
			get;
			private set;
		}
#endregion
#region Public Methods
		public void ReplaceFrom(XmpTag tag)
		{
			NodeTree=tag.NodeTree;
			nodes=new Dictionary<string,Dictionary<string,XmpNode>>();
			AcceptVisitors();
		}
		public override void Clear()
		{
			throw new NotImplementedException();
		}
		public XmpNode FindNode(string ns,string name)
		{
			if(!nodes.ContainsKey(ns))
			{
				return null;
			}
			if(!nodes[ns].ContainsKey(name))
			{
				return null;
			}
			return nodes[ns][name];
		}
		public string GetTextNode(string ns,string name)
		{
			var node=FindNode(ns,name);
			if(node==null||node.Type!=XmpNodeType.Simple)
			{
				return null;
			}
			return node.Value;
		}
		public void SetTextNode(string ns,string name,string value)
		{
			if(value==null)
			{
				RemoveNode(ns,name);
				return;
			}
			var node=NewNode(ns,name);
			node.Value=value;
		}
		public string GetLangAltNode(string ns,string name)
		{
			var node=FindNode(ns,name);
			if(node==null)
			{
				return null;
			}
			if(node.Type==XmpNodeType.Simple)
			{
				return node.Value;
			}
			if(node.Type!=XmpNodeType.Alt)
			{
				return null;
			}
			var children=node.Children;
			foreach(XmpNode child_node in children)
			{
				var qualifier=child_node.GetQualifier(XML_NS,"lang");
				if(qualifier!=null&&qualifier.Value=="x-default")
				{
					return child_node.Value;
				}
			}
			if(children.Count>0&&children[0].Type==XmpNodeType.Simple)
			{
				return children[0].Value;
			}
			return null;
		}
		public void SetLangAltNode(string ns,string name,string value)
		{
			if(value==null)
			{
				RemoveNode(ns,name);
				return;
			}
			var node=NewNode(ns,name,XmpNodeType.Alt);
			var child_node=new XmpNode(RDF_NS,LI_URI,value);
			child_node.AddQualifier(new XmpNode(XML_NS,"lang","x-default"));
			node.AddChild(child_node);
		}
		public string[]GetCollectionNode(string ns,string name)
		{
			var node=FindNode(ns,name);
			if(node==null)
			{
				return null;
			}
			List<string>items=new List<string>();
			foreach(XmpNode child in node.Children)
			{
				string item=child.Value;
				if(item!=null)
				{
					items.Add(item);
				}
			}
			return items.ToArray();
		}
		public void SetCollectionNode(string ns,string name,string[]values,XmpNodeType type)
		{
			if(type==XmpNodeType.Simple||type==XmpNodeType.Alt)
			{
				throw new ArgumentException("type");
			}
			if(values==null)
			{
				RemoveNode(ns,name);
				return;
			}
			var node=NewNode(ns,name,type);
			foreach(string value in values)
			node.AddChild(new XmpNode(RDF_NS,LI_URI,value));
		}
		public double?GetRationalNode(string ns,string name)
		{
			var text=GetTextNode(ns,name);
			if(text==null)
			{
				return null;
			}
			string[]values=text.Split('/');
			if(values.Length!=2)
			{
				double result;
				if(Double.TryParse(text,out result))
				{
					return result;
				}
				return null;
			}
			double nom,den;
			if(Double.TryParse(values[0],out nom)&&Double.TryParse(values[1],out den))
			{
				if(den!=0.0)
				{
					return((double)nom)/((double)den);
				}
			}
			return null;
		}
		public void SetRationalNode(string ns,string name,double value)
		{
			string fraction=DecimalToFraction(value,(long)Math.Pow(10,10));
			SetTextNode(ns,name,fraction);
		}
		private string DecimalToFraction(double value,long max_denominator)
		{
			var m=new long[2,2];
			m[0,0]=m[1,1]=1;
			m[1,0]=m[0,1]=0;
			double x=value;
			long ai;
			while(m[1,0]*(ai=(long)x)+m[1,1]<=max_denominator)
			{
				long t=m[0,0]*ai+m[0,1];
				m[0,1]=m[0,0];
				m[0,0]=t;
				t=m[1,0]*ai+m[1,1];
				m[1,1]=m[1,0];
				m[1,0]=t;
				if(x==(double)ai)
				{
					break;
				}
				x=1/(x-(double)ai);
				if(x>(double)0x7FFFFFFF)
				{
					break;
				}
			}
			return String.Format("{0}/{1}",m[0,0],m[1,0]);
		}
		public uint?GetUIntNode(string ns,string name)
		{
			var text=GetTextNode(ns,name);
			if(text==null)
			{
				return null;
			}
			uint result;
			if(UInt32.TryParse(text,out result))
			{
				return result;
			}
			return null;
		}
		public string Render()
		{
			XmlDocument doc=new XmlDocument(NameTable);
			var meta=CreateNode(doc,"xmpmeta",ADOBE_X_NS);
			var rdf=CreateNode(doc,"RDF",RDF_NS);
			var description=CreateNode(doc,"Description",RDF_NS);
			NodeTree.RenderInto(description);
			doc.AppendChild(meta);
			meta.AppendChild(rdf);
			rdf.AppendChild(description);
			return doc.OuterXml;
		}
		static void EnsureNamespacePrefix(string ns)
		{
			if(!NamespacePrefixes.ContainsKey(ns))
			{
				NamespacePrefixes.Add(ns,String.Format("ns{0}",++anon_ns_count));
				Console.WriteLine("TAGLIB# DEBUG: Added {0} prefix for {1} namespace (XMP)",NamespacePrefixes[ns],ns);
			}
		}
		internal static XmlNode CreateNode(XmlDocument doc,string name,string ns)
		{
			EnsureNamespacePrefix(ns);
			return doc.CreateElement(NamespacePrefixes[ns],name,ns);
		}
		internal static XmlAttribute CreateAttribute(XmlDocument doc,string name,string ns)
		{
			EnsureNamespacePrefix(ns);
			return doc.CreateAttribute(NamespacePrefixes[ns],name,ns);
		}
#endregion
		private class NodeIndexVisitor:
		XmpNodeVisitor
		{
			private XmpTag tag;
			public NodeIndexVisitor(XmpTag tag)
			{
				this.tag=tag;
			}
			public void Visit(XmpNode node)
			{
				if(node.Namespace==XmpTag.RDF_NS&&node.Name==XmpTag.LI_URI)
				{
					return;
				}
				AddNode(node);
			}
			void AddNode(XmpNode node)
			{
				if(tag.nodes==null)
				{
					tag.nodes=new Dictionary<string,Dictionary<string,XmpNode>>();
				}
				if(!tag.nodes.ContainsKey(node.Namespace))
				{
					tag.nodes[node.Namespace]=new Dictionary<string,XmpNode>();
				}
				tag.nodes[node.Namespace][node.Name]=node;
			}
		}
#region Metadata fields
		public override string Comment
		{
			get
			{
				string comment=GetLangAltNode(DC_NS,"description");
				if(comment!=null)
				{
					return comment;
				}
				comment=GetLangAltNode(EXIF_NS,"UserComment");
				return comment;
			}
			set
			{
				SetLangAltNode(DC_NS,"description",value);
				SetLangAltNode(EXIF_NS,"UserComment",value);
			}
		}
		public override string[]Keywords
		{
			get
			{
				return GetCollectionNode(DC_NS,"subject")??new string[]
				{
				};
			}
			set
			{
				SetCollectionNode(DC_NS,"subject",value,XmpNodeType.Bag);
			}
		}
		public override uint?Rating
		{
			get
			{
				return GetUIntNode(XAP_NS,"Rating");
			}
			set
			{
				SetTextNode(XAP_NS,"Rating",value!=null?value.ToString():null);
			}
		}
		public override DateTime?DateTime
		{
			get
			{
				try
				{
					return System.DateTime.Parse(GetTextNode(XAP_NS,"CreateDate"));
				}
				catch
				{
				}
				return null;
			}
			set
			{
				SetTextNode(XAP_NS,"CreateDate",value!=null?value.ToString():null);
			}
		}
		public override ImageOrientation Orientation
		{
			get
			{
				var orientation=GetUIntNode(TIFF_NS,"Orientation");
				if(orientation.HasValue)
				{
					return(ImageOrientation)orientation;
				}
				return ImageOrientation.None;
			}
			set
			{
				if((uint)value<1U||(uint)value>8U)
				{
					RemoveNode(TIFF_NS,"Orientation");
					return;
				}
				SetTextNode(TIFF_NS,"Orientation",String.Format("{0}",(ushort)value));
			}
		}
		public override string Software
		{
			get
			{
				return GetTextNode(XAP_NS,"CreatorTool");
			}
			set
			{
				SetTextNode(XAP_NS,"CreatorTool",value);
			}
		}
		public override double?Latitude
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override double?Longitude
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override double?Altitude
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override double?ExposureTime
		{
			get
			{
				return GetRationalNode(EXIF_NS,"ExposureTime");
			}
			set
			{
				SetRationalNode(EXIF_NS,"ExposureTime",value.HasValue?(double)value:0);
			}
		}
		public override double?FNumber
		{
			get
			{
				return GetRationalNode(EXIF_NS,"FNumber")??GetRationalNode(TIFF_NS,"FNumber");
			}
			set
			{
				SetTextNode(TIFF_NS,"FNumber",null);
				SetRationalNode(EXIF_NS,"FNumber",value.HasValue?(double)value:0);
			}
		}
		public override uint?ISOSpeedRatings
		{
			get
			{
				string[]values=GetCollectionNode(EXIF_NS,"ISOSpeedRatings");
				if(values!=null&&values.Length>0)
				{
					uint result;
					if(UInt32.TryParse(values[0],out result))
					{
						return result;
					}
				}
				return GetUIntNode(EXIF_NS,"ISOSpeedRating");
			}
			set
			{
				SetCollectionNode(EXIF_NS,"ISOSpeedRating",null,XmpNodeType.Seq);
				SetCollectionNode(EXIF_NS,"ISOSpeedRatings",new string[]{value.ToString()},XmpNodeType.Seq);
			}
		}
		public override double?FocalLength
		{
			get
			{
				return GetRationalNode(EXIF_NS,"FocalLength");
			}
			set
			{
				SetRationalNode(EXIF_NS,"FocalLength",value.HasValue?(double)value:0);
			}
		}
		public override uint?FocalLengthIn35mmFilm
		{
			get
			{
				return GetUIntNode(EXIF_NS,"FocalLengthIn35mmFilm");
			}
			set
			{
				SetTextNode(EXIF_NS,"FocalLengthIn35mmFilm",value.HasValue?value.Value.ToString():String.Empty);
			}
		}
		public override string Make
		{
			get
			{
				return GetTextNode(TIFF_NS,"Make");
			}
			set
			{
				SetTextNode(TIFF_NS,"Make",value);
			}
		}
		public override string Model
		{
			get
			{
				return GetTextNode(TIFF_NS,"Model");
			}
			set
			{
				SetTextNode(TIFF_NS,"Model",value);
			}
		}
		public override string Creator
		{
			get
			{
				string[]values=GetCollectionNode(DC_NS,"creator");
				if(values!=null&&values.Length>0)
				{
					return values[0];
				}
				return null;
			}
			set
			{
				if(value==null)
				{
					RemoveNode(DC_NS,"creator");
				}
				SetCollectionNode(DC_NS,"creator",new string[]{value},XmpNodeType.Seq);
			}
		}
		public override string Title
		{
			get
			{
				return GetLangAltNode(DC_NS,"title");
			}
			set
			{
				SetLangAltNode(DC_NS,"title",value);
			}
		}
		public override string Copyright
		{
			get
			{
				return GetLangAltNode(DC_NS,"rights");
			}
			set
			{
				SetLangAltNode(DC_NS,"rights",value);
			}
		}
#endregion
	}
}
