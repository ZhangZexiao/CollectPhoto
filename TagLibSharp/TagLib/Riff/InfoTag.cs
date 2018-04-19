using System;
namespace TagLib.Riff
{
	public class InfoTag:
	ListTag
	{
#region Constructors
		public InfoTag():
		base()
		{
		}
		public InfoTag(ByteVector data):
		base(data)
		{
		}
		public InfoTag(TagLib.File file,long position,int length):
		base(file,position,length)
		{
		}
#endregion
#region Public Methods
		public override ByteVector RenderEnclosed()
		{
			return RenderEnclosed("INFO");
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.RiffInfo;
			}
		}
		public override string Title
		{
			get
			{
				foreach(string s in GetValuesAsStrings("INAM"))
				if(!string.IsNullOrEmpty(s))
				{
					return s;
				}
				return null;
			}
			set
			{
				SetValue("INAM",value);
			}
		}
		public override string[]Performers
		{
			get
			{
				return GetValuesAsStrings("ISTR");
			}
			set
			{
				SetValue("ISTR",value);
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				return GetValuesAsStrings("IART");
			}
			set
			{
				SetValue("IART",value);
			}
		}
		public override string[]Composers
		{
			get
			{
				return GetValuesAsStrings("IWRI");
			}
			set
			{
				SetValue("IWRI",value);
			}
		}
		public override string Comment
		{
			get
			{
				foreach(string s in GetValuesAsStrings("ICMT"))
				if(!string.IsNullOrEmpty(s))
				{
					return s;
				}
				return null;
			}
			set
			{
				SetValue("ICMT",value);
			}
		}
		public override string[]Genres
		{
			get
			{
				return GetValuesAsStrings("IGNR");
			}
			set
			{
				SetValue("IGNR",value);
			}
		}
		public override uint Year
		{
			get
			{
				return GetValueAsUInt("ICRD");
			}
			set
			{
				SetValue("ICRD",value);
			}
		}
		public override uint Track
		{
			get
			{
				return GetValueAsUInt("IPRT");
			}
			set
			{
				SetValue("IPRT",value);
			}
		}
		public override uint TrackCount
		{
			get
			{
				return GetValueAsUInt("IFRM");
			}
			set
			{
				SetValue("IFRM",value);
			}
		}
		public override string Copyright
		{
			get
			{
				foreach(string s in GetValuesAsStrings("ICOP"))
				if(!string.IsNullOrEmpty(s))
				{
					return s;
				}
				return null;
			}
			set
			{
				SetValue("ICOP",value);
			}
		}
#endregion
	}
}
