using System;
using System.Collections.Generic;
using System.Text;
using TagLib.Id3v2;
namespace TagLib.Id3v2
{
	public class UnsynchronisedLyricsFrame:
	Frame
	{
#region Private Properties
		private StringType encoding=Tag.DefaultEncoding;
		private string language=null;
		private string description=null;
		private string text=null;
#endregion
#region Constructors
		public UnsynchronisedLyricsFrame(string description,string language,StringType encoding):
		base(FrameType.USLT,4)
		{
			this.encoding=encoding;
			this.language=language;
			this.description=description;
		}
		public UnsynchronisedLyricsFrame(string description,string language):
		this(description,language,TagLib.Id3v2.Tag.DefaultEncoding)
		{
		}
		public UnsynchronisedLyricsFrame(string description):
		this(description,null)
		{
		}
		public UnsynchronisedLyricsFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal UnsynchronisedLyricsFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(header)
		{
			SetData(data,offset,version,false);
		}
#endregion
#region Public Properties
		public StringType TextEncoding
		{
			get
			{
				return encoding;
			}
			set
			{
				encoding=value;
			}
		}
		public string Language
		{
			get
			{
				if(language!=null&&language.Length>2)
				{
					return language.Substring(0,3);
				}
				return"XXX";
			}
			set
			{
				language=value;
			}
		}
		public string Description
		{
			get
			{
				if(description!=null)
				{
					return description;
				}
				return string.Empty;
			}
			set
			{
				description=value;
			}
		}
		public string Text
		{
			get
			{
				if(text!=null)
				{
					return text;
				}
				return string.Empty;
			}
			set
			{
				text=value;
			}
		}
#endregion
#region Public Methods
		public override string ToString()
		{
			return Text;
		}
#endregion
#region Public Static Methods
		public static UnsynchronisedLyricsFrame Get(Tag tag,string description,string language,bool create)
		{
			UnsynchronisedLyricsFrame uslt;
			foreach(Frame frame in tag.GetFrames(FrameType.USLT))
			{
				uslt=frame as UnsynchronisedLyricsFrame;
				if(uslt==null)
				{
					continue;
				}
				if(uslt.Description!=description)
				{
					continue;
				}
				if(language!=null&&language!=uslt.Language)
				{
					continue;
				}
				return uslt;
			}
			if(!create)
			{
				return null;
			}
			uslt=new UnsynchronisedLyricsFrame(description,language);
			tag.AddFrame(uslt);
			return uslt;
		}
		public static UnsynchronisedLyricsFrame GetPreferred(Tag tag,string description,string language)
		{
			int best_value= -1;
			UnsynchronisedLyricsFrame best_frame=null;
			foreach(Frame frame in tag.GetFrames(FrameType.USLT))
			{
				UnsynchronisedLyricsFrame uslt=frame as UnsynchronisedLyricsFrame;
				if(uslt==null)
				{
					continue;
				}
				bool same_name=uslt.Description==description;
				bool same_lang=uslt.Language==language;
				if(same_name&&same_lang)
				{
					return uslt;
				}
				int value=same_lang?2:same_name?1:0;
				if(value<=best_value)
				{
					continue;
				}
				best_value=value;
				best_frame=uslt;
			}
			return best_frame;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			if(data.Count<4)
			{
				throw new CorruptFileException("Not enough bytes in field.");
			}
			encoding=(StringType)data[0];
			language=data.ToString(StringType.Latin1,1,3);
			string[]split=data.ToStrings(encoding,4,2);
			if(split.Length==1)
			{
				description=String.Empty;
				text=split[0];
			}
			else
			{
				description=split[0];
				text=split[1];
			}
		}
		protected override ByteVector RenderFields(byte version)
		{
			StringType encoding=CorrectEncoding(TextEncoding,version);
			ByteVector v=new ByteVector();
			v.Add((byte)encoding);
			v.Add(ByteVector.FromString(Language,StringType.Latin1));
			v.Add(ByteVector.FromString(description,encoding));
			v.Add(ByteVector.TextDelimiter(encoding));
			v.Add(ByteVector.FromString(text,encoding));
			return v;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			UnsynchronisedLyricsFrame frame=new UnsynchronisedLyricsFrame(description,language,encoding);
			frame.text=text;
			return frame;
		}
#endregion
	}
}
