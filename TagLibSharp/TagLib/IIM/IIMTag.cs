using System;
using System.Collections.Generic;
using TagLib.Image;
using TagLib.IFD.Entries;
namespace TagLib.IIM
{
	public class IIMTag:
	Xmp.XmpTag
	{
		private List<string>m_Keywords;
		public IIMTag()
		{
		}
		public override TagLib.TagTypes TagTypes
		{
			get
			{
				return TagLib.TagTypes.IPTCIIM;
			}
		}
		public override void Clear()
		{
			Title=null;
			m_Keywords=null;
			Creator=null;
			Copyright=null;
			Comment=null;
		}
		public override string Title
		{
			get;
			set;
		}
		public override string Creator
		{
			get;
			set;
		}
		public override string Copyright
		{
			get;
			set;
		}
		public override string Comment
		{
			get;
			set;
		}
		public override string[]Keywords
		{
			get
			{
				if(m_Keywords==null)
				{
					return null;
				}
				return m_Keywords.ToArray();
			}
		}
		internal void AddKeyword(string keyword)
		{
			if(m_Keywords==null)
			{
				m_Keywords=new List<string>();
			}
			m_Keywords.Add(keyword);
		}
	}
}
