using System.Collections;
using System;
namespace TagLib.Id3v2
{
	public class GeneralEncapsulatedObjectFrame:
	Frame
	{
#region Private Fields
		private StringType encoding=Tag.DefaultEncoding;
		private string mime_type=null;
		string file_name=null;
		private string description=null;
		private ByteVector data=null;
#endregion
#region Constructors
		public GeneralEncapsulatedObjectFrame():
		base(FrameType.GEOB,4)
		{
		}
		public GeneralEncapsulatedObjectFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal GeneralEncapsulatedObjectFrame(ByteVector data,int offset,FrameHeader header,byte version):
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
		public string MimeType
		{
			get
			{
				if(mime_type!=null)
				{
					return mime_type;
				}
				return string.Empty;
			}
			set
			{
				mime_type=value;
			}
		}
		public string FileName
		{
			get
			{
				if(file_name!=null)
				{
					return file_name;
				}
				return string.Empty;
			}
			set
			{
				file_name=value;
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
		public ByteVector Object
		{
			get
			{
				return data!=null?data:new ByteVector();
			}
			set
			{
				data=value;
			}
		}
#endregion
#region Public Methods
		public override string ToString()
		{
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			if(Description.Length==0)
			{
				builder.Append(Description);
				builder.Append(" ");
			}
			builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,"[{0}] {1} bytes",MimeType,Object.Count);
			return builder.ToString();
		}
#endregion
#region Public Static Methods
		public static GeneralEncapsulatedObjectFrame Get(Tag tag,string description,bool create)
		{
			GeneralEncapsulatedObjectFrame geob;
			foreach(Frame frame in tag.GetFrames(FrameType.GEOB))
			{
				geob=frame as GeneralEncapsulatedObjectFrame;
				if(geob==null)
				{
					continue;
				}
				if(geob.Description!=description)
				{
					continue;
				}
				return geob;
			}
			if(!create)
			{
				return null;
			}
			geob=new GeneralEncapsulatedObjectFrame();
			geob.Description=description;
			tag.AddFrame(geob);
			return geob;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			if(data.Count<4)
			{
				throw new CorruptFileException("An object frame must contain at least 4 bytes.");
			}
			int start=0;
			encoding=(StringType)data[start++];
			int end=data.Find(ByteVector.TextDelimiter(StringType.Latin1),start);
			if(end<start)
			{
				return;
			}
			mime_type=data.ToString(StringType.Latin1,start,end-start);
			ByteVector delim=ByteVector.TextDelimiter(encoding);
			start=end+1;
			end=data.Find(delim,start,delim.Count);
			if(end<start)
			{
				return;
			}
			file_name=data.ToString(encoding,start,end-start);
			start=end+delim.Count;
			end=data.Find(delim,start,delim.Count);
			if(end<start)
			{
				return;
			}
			description=data.ToString(encoding,start,end-start);
			start=end+delim.Count;
			data.RemoveRange(0,start);
			this.data=data;
		}
		protected override ByteVector RenderFields(byte version)
		{
			StringType encoding=CorrectEncoding(this.encoding,version);
			ByteVector v=new ByteVector();
			v.Add((byte)encoding);
			if(MimeType!=null)
			{
				v.Add(ByteVector.FromString(MimeType,StringType.Latin1));
			}
			v.Add(ByteVector.TextDelimiter(StringType.Latin1));
			if(FileName!=null)
			{
				v.Add(ByteVector.FromString(FileName,encoding));
			}
			v.Add(ByteVector.TextDelimiter(encoding));
			if(Description!=null)
			{
				v.Add(ByteVector.FromString(Description,encoding));
			}
			v.Add(ByteVector.TextDelimiter(encoding));
			v.Add(data);
			return v;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			GeneralEncapsulatedObjectFrame frame=new GeneralEncapsulatedObjectFrame();
			frame.encoding=encoding;
			frame.mime_type=mime_type;
			frame.file_name=file_name;
			frame.description=description;
			if(data!=null)
			{
				frame.data=new ByteVector(data);
			}
			return frame;
		}
#endregion
	}
}
