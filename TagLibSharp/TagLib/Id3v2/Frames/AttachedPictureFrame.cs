using System.Collections;
using System;
using TagLib;
namespace TagLib.Id3v2
{
	public class AttachedPictureFrame:
	Frame,IPicture
	{
#region Private Properties
		private StringType text_encoding=Tag.DefaultEncoding;
		private string mime_type=null;
		private PictureType type=PictureType.Other;
		private string description=null;
		private ByteVector data=null;
		private ByteVector raw_data=null;
		private byte raw_version=0;
#endregion
#region Constructors
		public AttachedPictureFrame():
		base(FrameType.APIC,4)
		{
		}
		public AttachedPictureFrame(IPicture picture):
		base(FrameType.APIC,4)
		{
			if(picture==null)
			{
				throw new ArgumentNullException("picture");
			}
			mime_type=picture.MimeType;
			type=picture.Type;
			description=picture.Description;
			data=picture.Data;
		}
		public AttachedPictureFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal AttachedPictureFrame(ByteVector data,int offset,FrameHeader header,byte version):
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
				ParseRawData();
				return text_encoding;
			}
			set
			{
				text_encoding=value;
			}
		}
		public string MimeType
		{
			get
			{
				ParseRawData();
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
		public PictureType Type
		{
			get
			{
				ParseRawData();
				return type;
			}
			set
			{
				type=value;
			}
		}
		public string Description
		{
			get
			{
				ParseRawData();
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
		public ByteVector Data
		{
			get
			{
				ParseRawData();
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
			if(string.IsNullOrEmpty(Description))
			{
				builder.Append(Description);
				builder.Append(" ");
			}
			builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,"[{0}] {1} bytes",MimeType,Data.Count);
			return builder.ToString();
		}
#endregion
#region Public Static Methods
		public static AttachedPictureFrame Get(Tag tag,string description,bool create)
		{
			return Get(tag,description,PictureType.Other,create);
		}
		public static AttachedPictureFrame Get(Tag tag,PictureType type,bool create)
		{
			return Get(tag,null,type,create);
		}
		public static AttachedPictureFrame Get(Tag tag,string description,PictureType type,bool create)
		{
			AttachedPictureFrame apic;
			foreach(Frame frame in tag.GetFrames(FrameType.APIC))
			{
				apic=frame as AttachedPictureFrame;
				if(apic==null)
				{
					continue;
				}
				if(description!=null&&apic.Description!=description)
				{
					continue;
				}
				if(type!=PictureType.Other&&apic.Type!=type)
				{
					continue;
				}
				return apic;
			}
			if(!create)
			{
				return null;
			}
			apic=new AttachedPictureFrame();
			apic.Description=description;
			apic.Type=type;
			tag.AddFrame(apic);
			return apic;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			if(data.Count<5)
			{
				throw new CorruptFileException("A picture frame must contain at least 5 bytes.");
			}
			raw_data=data;
			raw_version=version;
		}
		protected void ParseRawData()
		{
			if(raw_data==null)
			{
				return;
			}
			int pos=0;
			int offset;
			text_encoding=(StringType)raw_data[pos++];
			if(raw_version>2)
			{
				offset=raw_data.Find(ByteVector.TextDelimiter(StringType.Latin1),pos);
				if(offset<pos)
				{
					return;
				}
				mime_type=raw_data.ToString(StringType.Latin1,pos,offset-pos);
				pos=offset+1;
			}
			else
			{
				ByteVector ext=raw_data.Mid(pos,3);
				if(ext=="JPG")
				{
					mime_type="image/jpeg";
				}
				else if(ext=="PNG")
				{
					mime_type="image/png";
				}
				else
				{
					mime_type="image/unknown";
				}
				pos+=3;
			}
			ByteVector delim=ByteVector.TextDelimiter(text_encoding);
			type=(PictureType)raw_data[pos++];
			offset=raw_data.Find(delim,pos,delim.Count);
			if(offset<pos)
			{
				return;
			}
			description=raw_data.ToString(text_encoding,pos,offset-pos);
			pos=offset+delim.Count;
			raw_data.RemoveRange(0,pos);
			this.data=raw_data;
			this.raw_data=null;
		}
		protected override ByteVector RenderFields(byte version)
		{
			if(raw_data!=null&&raw_version==version)
			{
				return raw_data;
			}
			StringType encoding=CorrectEncoding(TextEncoding,version);
			ByteVector data=new ByteVector();
			data.Add((byte)encoding);
			if(version==2)
			{
				switch(MimeType)
				{
				case"image/png":
					data.Add("PNG");
					break;
				case"image/jpeg":
					data.Add("JPG");
					break;
				default:
					data.Add("XXX");
					break;
				}
			}
			else
			{
				data.Add(ByteVector.FromString(MimeType,StringType.Latin1));
				data.Add(ByteVector.TextDelimiter(StringType.Latin1));
			}
			data.Add((byte)type);
			data.Add(ByteVector.FromString(Description,encoding));
			data.Add(ByteVector.TextDelimiter(encoding));
			data.Add(this.data);
			return data;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			AttachedPictureFrame frame=new AttachedPictureFrame();
			frame.text_encoding=text_encoding;
			frame.mime_type=mime_type;
			frame.type=type;
			frame.description=description;
			if(data!=null)
			{
				frame.data=new ByteVector(data);
			}
			if(raw_data!=null)
			{
				frame.data=new ByteVector(raw_data);
			}
			frame.raw_version=raw_version;
			return frame;
		}
#endregion
	}
}
