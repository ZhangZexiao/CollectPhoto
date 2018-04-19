using System;
using System.Collections.Generic;
using System.Text;
namespace TagLib.Id3v2
{
	public enum TimestampFormat
	{
		Unknown=0x00,AbsoluteMpegFrames=0x01,AbsoluteMilliseconds=0x02
	}
	public enum SynchedTextType
	{
		Other=0x00,Lyrics=0x01,TextTranscription=0x02,Movement=0x03,Events=0x04,Chord=0x05,Trivia=0x06,WebpageUrls=0x07,ImageUrls=0x08
	}
	public class SynchronisedLyricsFrame:
	Frame
	{
#region Private Properties
		private StringType encoding=Tag.DefaultEncoding;
		private string language=null;
		private string description=null;
		private TimestampFormat timestamp_format=TimestampFormat.Unknown;
		private SynchedTextType lyrics_type=SynchedTextType.Other;
		private SynchedText[]text=new SynchedText[0];
#endregion
#region Constructors
		public SynchronisedLyricsFrame(string description,string language,SynchedTextType type,StringType encoding):
		base(FrameType.SYLT,4)
		{
			this.encoding=encoding;
			this.language=language;
			this.description=description;
			this.lyrics_type=type;
		}
		public SynchronisedLyricsFrame(string description,string language,SynchedTextType type):
		this(description,language,type,TagLib.Id3v2.Tag.DefaultEncoding)
		{
		}
		public SynchronisedLyricsFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal SynchronisedLyricsFrame(ByteVector data,int offset,FrameHeader header,byte version):
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
				return(language!=null&&language.Length>2)?language.Substring(0,3):"XXX";
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
				return description;
			}
			set
			{
				description=value;
			}
		}
		public TimestampFormat Format
		{
			get
			{
				return timestamp_format;
			}
			set
			{
				timestamp_format=value;
			}
		}
		public SynchedTextType Type
		{
			get
			{
				return lyrics_type;
			}
			set
			{
				lyrics_type=value;
			}
		}
		public SynchedText[]Text
		{
			get
			{
				return text;
			}
			set
			{
				text=value==null?new SynchedText[0]:value;
			}
		}
#endregion
#region Public Static Methods
		public static SynchronisedLyricsFrame Get(Tag tag,string description,string language,SynchedTextType type,bool create)
		{
			foreach(Frame f in tag)
			{
				SynchronisedLyricsFrame lyr=f as SynchronisedLyricsFrame;
				if(lyr==null)
				{
					continue;
				}
				if(lyr.Description==description&&(language==null||language==lyr.Language)&&type==lyr.Type)
				{
					return lyr;
				}
			}
			if(!create)
			{
				return null;
			}
			SynchronisedLyricsFrame frame=new SynchronisedLyricsFrame(description,language,type);
			tag.AddFrame(frame);
			return frame;
		}
		public static SynchronisedLyricsFrame GetPreferred(Tag tag,string description,string language,SynchedTextType type)
		{
			int best_value= -1;
			SynchronisedLyricsFrame best_frame=null;
			foreach(Frame f in tag)
			{
				SynchronisedLyricsFrame cf=f as SynchronisedLyricsFrame;
				if(cf==null)
				{
					continue;
				}
				int value=0;
				if(cf.Language==language)
				{
					value+=4;
				}
				if(cf.Description==description)
				{
					value+=2;
				}
				if(cf.Type==type)
				{
					value+=1;
				}
				if(value==7)
				{
					return cf;
				}
				if(value<=best_value)
				{
					continue;
				}
				best_value=value;
				best_frame=cf;
			}
			return best_frame;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			if(data.Count<6)
			{
				throw new CorruptFileException("Not enough bytes in field.");
			}
			encoding=(StringType)data[0];
			language=data.ToString(StringType.Latin1,1,3);
			timestamp_format=(TimestampFormat)data[4];
			lyrics_type=(SynchedTextType)data[5];
			ByteVector delim=ByteVector.TextDelimiter(encoding);
			int delim_index=data.Find(delim,6,delim.Count);
			if(delim_index<0)
			{
				throw new CorruptFileException("Text delimiter expected.");
			}
			description=data.ToString(encoding,6,delim_index-6);
			int offset=delim_index+delim.Count;
			List<SynchedText>l=new List<SynchedText>();
			while(offset+delim.Count+4<data.Count)
			{
				delim_index=data.Find(delim,offset,delim.Count);
				if(delim_index<offset)
				{
					throw new CorruptFileException("Text delimiter expected.");
				}
				string text=data.ToString(encoding,offset,delim_index-offset);
				offset=delim_index+delim.Count;
				if(offset+4>data.Count)
				{
					break;
				}
				l.Add(new SynchedText(data.Mid(offset,4).ToUInt(),text));
				offset+=4;
			}
			this.text=l.ToArray();
		}
		protected override ByteVector RenderFields(byte version)
		{
			StringType encoding=CorrectEncoding(TextEncoding,version);
			ByteVector delim=ByteVector.TextDelimiter(encoding);
			ByteVector v=new ByteVector();
			v.Add((byte)encoding);
			v.Add(ByteVector.FromString(Language,StringType.Latin1));
			v.Add((byte)timestamp_format);
			v.Add((byte)lyrics_type);
			v.Add(ByteVector.FromString(description,encoding));
			v.Add(delim);
			foreach(SynchedText t in text)
			{
				v.Add(ByteVector.FromString(t.Text,encoding));
				v.Add(delim);
				v.Add(ByteVector.FromUInt((uint)t.Time));
			}
			return v;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			SynchronisedLyricsFrame frame=new SynchronisedLyricsFrame(description,language,lyrics_type,encoding);
			frame.timestamp_format=timestamp_format;
			frame.text=(SynchedText[])text.Clone();
			return frame;
		}
#endregion
	}
	public struct SynchedText
	{
		private long time;
		private string text;
		public SynchedText(long time,string text)
		{
			this.time=time;
			this.text=text;
		}
		public long Time
		{
			get
			{
				return time;
			}
			set
			{
				time=value;
			}
		}
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text=value;
			}
		}
	}
}
