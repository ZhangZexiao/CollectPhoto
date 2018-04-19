using System;
namespace TagLib.Riff
{
	public class MovieIdTag:
	ListTag
	{
#region Constructors
		public MovieIdTag():
		base()
		{
		}
		public MovieIdTag(ByteVector data):
		base(data)
		{
		}
		public MovieIdTag(TagLib.File file,long position,int length):
		base(file,position,length)
		{
		}
#endregion
#region Public Methods
		public override ByteVector RenderEnclosed()
		{
			return RenderEnclosed("MID ");
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.MovieId;
			}
		}
		public override string Title
		{
			get
			{
				foreach(string s in GetValuesAsStrings("TITL"))
				if(!string.IsNullOrEmpty(s))
				{
					return s;
				}
				return null;
			}
			set
			{
				SetValue("TITL",value);
			}
		}
		public override string[]Performers
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
		public override string Comment
		{
			get
			{
				foreach(string s in GetValuesAsStrings("COMM"))
				if(!string.IsNullOrEmpty(s))
				{
					return s;
				}
				return null;
			}
			set
			{
				SetValue("COMM",value);
			}
		}
		public override string[]Genres
		{
			get
			{
				return GetValuesAsStrings("GENR");
			}
			set
			{
				SetValue("GENR",value);
			}
		}
		public override uint Track
		{
			get
			{
				return GetValueAsUInt("PRT1");
			}
			set
			{
				SetValue("PRT1",value);
			}
		}
		public override uint TrackCount
		{
			get
			{
				return GetValueAsUInt("PRT2");
			}
			set
			{
				SetValue("PRT2",value);
			}
		}
#endregion
	}
}
