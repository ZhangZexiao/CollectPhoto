using System.Collections;
using System;
namespace TagLib.Id3v2
{
	public class CommentsFrame:
	Frame
	{
#region Private Fields
		private StringType encoding=Tag.DefaultEncoding;
		private string language=null;
		private string description=null;
		private string text=null;
#endregion
#region Constructors
		public CommentsFrame(string description,string language,StringType encoding):
		base(FrameType.COMM,4)
		{
			this.encoding=encoding;
			this.language=language;
			this.description=description;
		}
		public CommentsFrame(string description,string language):
		this(description,language,TagLib.Id3v2.Tag.DefaultEncoding)
		{
		}
		public CommentsFrame(string description):
		this(description,null)
		{
		}
		public CommentsFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal CommentsFrame(ByteVector data,int offset,FrameHeader header,byte version):
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
		public static CommentsFrame Get(Tag tag,string description,string language,bool create)
		{
			CommentsFrame comm;
			foreach(Frame frame in tag.GetFrames(FrameType.COMM))
			{
				comm=frame as CommentsFrame;
				if(comm==null)
				{
					continue;
				}
				if(comm.Description!=description)
				{
					continue;
				}
				if(language!=null&&language!=comm.Language)
				{
					continue;
				}
				return comm;
			}
			if(!create)
			{
				return null;
			}
			comm=new CommentsFrame(description,language);
			tag.AddFrame(comm);
			return comm;
		}
		public static CommentsFrame GetPreferred(Tag tag,string description,string language)
		{
			bool skip_itunes=description==null|| !description.StartsWith("iTun");
			int best_value= -1;
			CommentsFrame best_frame=null;
			foreach(Frame frame in tag.GetFrames(FrameType.COMM))
			{
				CommentsFrame comm=frame as CommentsFrame;
				if(comm==null)
				{
					continue;
				}
				if(skip_itunes&&comm.Description.StartsWith("iTun"))
				{
					continue;
				}
				bool same_name=comm.Description==description;
				bool same_lang=comm.Language==language;
				if(same_name&&same_lang)
				{
					return comm;
				}
				int value=same_lang?2:same_name?1:0;
				if(value<=best_value)
				{
					continue;
				}
				best_value=value;
				best_frame=comm;
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
			string[]split=data.ToStrings(encoding,4,3);
			if(split.Length==0)
			{
				description=String.Empty;
				text=String.Empty;
			}
			else if(split.Length==1)
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
			CommentsFrame frame=new CommentsFrame(description,language,encoding);
			frame.text=text;
			return frame;
		}
#endregion
	}
}
