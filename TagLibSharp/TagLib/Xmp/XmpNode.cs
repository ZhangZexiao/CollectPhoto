using System;
using System.Collections.Generic;
using System.Xml;
namespace TagLib.Xmp
{
	public class XmpNode
	{
#region Private Fields
		private List<XmpNode>children;
		private Dictionary<string,Dictionary<string,XmpNode>>qualifiers;
		private string name;
#endregion
#region Properties
		public string Namespace
		{
			get;
			private set;
		}
		public string Name
		{
			get
			{
				return name;
			}
			internal set
			{
				if(name!=null)
				{
					throw new Exception("Cannot change named node");
				}
				if(value==null)
				{
					throw new ArgumentException("value");
				}
				name=value;
			}
		}
		public string Value
		{
			get;
			set;
		}
		public XmpNodeType Type
		{
			get;
			internal set;
		}
		public int QualifierCount
		{
			get
			{
				if(qualifiers==null)
				{
					return 0;
				}
				int count=0;
				foreach(var collection in qualifiers.Values)
				{
					count+=collection==null?0:collection.Count;
				}
				return count;
			}
		}
		public List<XmpNode>Children
		{
			get
			{
				return children??new List<XmpNode>();
			}
		}
#endregion
#region Constructors
		public XmpNode(string ns,string name)
		{
			if(ns!=String.Empty&&ns!=XmpTag.XML_NS&& !ns.EndsWith("/")&& !ns.EndsWith("#"))
			{
				ns=String.Format("{0}/",ns);
			}
			Namespace=ns;
			Name=name;
			Type=XmpNodeType.Simple;
			Value=String.Empty;
		}
		public XmpNode(string ns,string name,string value):
		this(ns,name)
		{
			Value=value;
		}
#endregion
#region Public Methods
		public void AddChild(XmpNode node)
		{
			if(node==null||node==this)
			{
				throw new ArgumentException("node");
			}
			if(children==null)
			{
				children=new List<XmpNode>();
			}
			children.Add(node);
		}
		public void RemoveChild(XmpNode node)
		{
			if(children==null)
			{
				return;
			}
			children.Remove(node);
		}
		public XmpNode GetChild(string ns,string name)
		{
			foreach(var node in children)
			{
				if(node.Namespace.Equals(ns)&&node.Name.Equals(name))
				{
					return node;
				}
			}
			return null;
		}
		public void AddQualifier(XmpNode node)
		{
			if(node==null||node==this)
			{
				throw new ArgumentException("node");
			}
			if(qualifiers==null)
			{
				qualifiers=new Dictionary<string,Dictionary<string,XmpNode>>();
			}
			if(!qualifiers.ContainsKey(node.Namespace))
			{
				qualifiers[node.Namespace]=new Dictionary<string,XmpNode>();
			}
			qualifiers[node.Namespace][node.Name]=node;
		}
		public XmpNode GetQualifier(string ns,string name)
		{
			if(qualifiers==null)
			{
				return null;
			}
			if(!qualifiers.ContainsKey(ns))
			{
				return null;
			}
			if(!qualifiers[ns].ContainsKey(name))
			{
				return null;
			}
			return qualifiers[ns][name];
		}
		public void Dump()
		{
			Dump("");
		}
		public void Accept(XmpNodeVisitor visitor)
		{
			visitor.Visit(this);
			if(children!=null)
			{
				foreach(XmpNode child in children)
				{
					child.Accept(visitor);
				}
			}
		}
		public void RenderInto(XmlNode parent)
		{
			if(IsRootNode)
			{
				AddAllChildrenTo(parent);
			}
			else if(IsReallySimpleType&&parent.Attributes.GetNamedItem(XmpTag.PARSE_TYPE_URI,XmpTag.RDF_NS)==null)
			{
				XmlAttribute attr=XmpTag.CreateAttribute(parent.OwnerDocument,Name,Namespace);
				attr.Value=Value;
				parent.Attributes.Append(attr);
			}
			else if(Type==XmpNodeType.Simple||Type==XmpNodeType.Struct)
			{
				var node=XmpTag.CreateNode(parent.OwnerDocument,Name,Namespace);
				node.InnerText=Value;
				if(Type==XmpNodeType.Struct)
				{
					XmlAttribute attr=XmpTag.CreateAttribute(parent.OwnerDocument,XmpTag.PARSE_TYPE_URI,XmpTag.RDF_NS);
					attr.Value="Resource";
					node.Attributes.Append(attr);
				}
				AddAllQualifiersTo(node);
				AddAllChildrenTo(node);
				parent.AppendChild(node);
			}
			else if(Type==XmpNodeType.Bag)
			{
				var node=XmpTag.CreateNode(parent.OwnerDocument,Name,Namespace);
				if(QualifierCount>0)
				{
					throw new NotImplementedException();
				}
				var bag=XmpTag.CreateNode(parent.OwnerDocument,XmpTag.BAG_URI,XmpTag.RDF_NS);
				foreach(var child in Children)
				child.RenderInto(bag);
				node.AppendChild(bag);
				parent.AppendChild(node);
			}
			else if(Type==XmpNodeType.Alt)
			{
				var node=XmpTag.CreateNode(parent.OwnerDocument,Name,Namespace);
				if(QualifierCount>0)
				{
					throw new NotImplementedException();
				}
				var bag=XmpTag.CreateNode(parent.OwnerDocument,XmpTag.ALT_URI,XmpTag.RDF_NS);
				foreach(var child in Children)
				child.RenderInto(bag);
				node.AppendChild(bag);
				parent.AppendChild(node);
			}
			else if(Type==XmpNodeType.Seq)
			{
				var node=XmpTag.CreateNode(parent.OwnerDocument,Name,Namespace);
				if(QualifierCount>0)
				{
					throw new NotImplementedException();
				}
				var bag=XmpTag.CreateNode(parent.OwnerDocument,XmpTag.SEQ_URI,XmpTag.RDF_NS);
				foreach(var child in Children)
				child.RenderInto(bag);
				node.AppendChild(bag);
				parent.AppendChild(node);
			}
			else
			{
				Dump();
				throw new NotImplementedException();
			}
		}
#endregion
#region Internal Methods
		internal void Dump(string prefix)
		{
			Console.WriteLine("{0}{1}{2} ({4}) = \"{3}\"",prefix,Namespace,Name,Value,Type);
			if(qualifiers!=null)
			{
				Console.WriteLine("{0}Qualifiers:",prefix);
				foreach(string ns in qualifiers.Keys)
				{
					foreach(string name in qualifiers[ns].Keys)
					{
						qualifiers[ns][name].Dump(prefix+"  ->  ");
					}
				}
			}
			if(children!=null)
			{
				Console.WriteLine("{0}Children:",prefix);
				foreach(XmpNode child in children)
				{
					child.Dump(prefix+"  ->  ");
				}
			}
		}
#endregion
#region Private Methods
		private bool IsReallySimpleType
		{
			get
			{
				return Type==XmpNodeType.Simple&&(children==null||children.Count==0)&&QualifierCount==0&&(Name!=XmpTag.LI_URI||Namespace!=XmpTag.RDF_NS);
			}
		}
		private bool IsRootNode
		{
			get
			{
				return Name==String.Empty&&Namespace==String.Empty;
			}
		}
		private void AddAllQualifiersTo(XmlNode xml)
		{
			if(qualifiers==null)
			{
				return;
			}
			foreach(var collection in qualifiers.Values)
			{
				foreach(XmpNode node in collection.Values)
				{
					XmlAttribute attr=XmpTag.CreateAttribute(xml.OwnerDocument,node.Name,node.Namespace);
					attr.Value=node.Value;
					xml.Attributes.Append(attr);
				}
			}
		}
		private void AddAllChildrenTo(XmlNode parent)
		{
			if(children==null)
			{
				return;
			}
			foreach(var child in children)
			child.RenderInto(parent);
		}
#endregion
	}
}
