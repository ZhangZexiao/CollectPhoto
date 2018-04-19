using System;
namespace TagLib.Flac
{
	public class Picture:
	IPicture
	{
#region Private Fields
		private PictureType type;
		private string mime_type;
		private string description;
		private int width=0;
		private int height=0;
		private int color_depth=0;
		private int indexed_colors=0;
		private ByteVector picture_data;
#endregion
#region Constructors
		public Picture(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<32)
			{
				throw new CorruptFileException("Data must be at least 32 bytes long");
			}
			int pos=0;
			type=(PictureType)data.Mid(pos,4).ToUInt();
			pos+=4;
			int mimetype_length=(int)data.Mid(pos,4).ToUInt();
			pos+=4;
			mime_type=data.ToString(StringType.Latin1,pos,mimetype_length);
			pos+=mimetype_length;
			int description_length=(int)data.Mid(pos,4).ToUInt();
			pos+=4;
			description=data.ToString(StringType.UTF8,pos,description_length);
			pos+=description_length;
			width=(int)data.Mid(pos,4).ToUInt();
			pos+=4;
			height=(int)data.Mid(pos,4).ToUInt();
			pos+=4;
			color_depth=(int)data.Mid(pos,4).ToUInt();
			pos+=4;
			indexed_colors=(int)data.Mid(pos,4).ToUInt();
			pos+=4;
			int data_length=(int)data.Mid(pos,4).ToUInt();
			pos+=4;
			picture_data=data.Mid(pos,data_length);
		}
		public Picture(IPicture picture)
		{
			if(picture==null)
			{
				throw new ArgumentNullException("picture");
			}
			type=picture.Type;
			mime_type=picture.MimeType;
			description=picture.Description;
			picture_data=picture.Data;
			TagLib.Flac.Picture flac_picture=picture as TagLib.Flac.Picture;
			if(flac_picture==null)
			{
				return;
			}
			width=flac_picture.Width;
			height=flac_picture.Height;
			color_depth=flac_picture.ColorDepth;
			indexed_colors=flac_picture.IndexedColors;
		}
#endregion
#region Public Methods
		public ByteVector Render()
		{
			ByteVector data=new ByteVector();
			data.Add(ByteVector.FromUInt((uint)Type));
			ByteVector mime_data=ByteVector.FromString(MimeType,StringType.Latin1);
			data.Add(ByteVector.FromUInt((uint)mime_data.Count));
			data.Add(mime_data);
			ByteVector decription_data=ByteVector.FromString(Description,StringType.UTF8);
			data.Add(ByteVector.FromUInt((uint)decription_data.Count));
			data.Add(decription_data);
			data.Add(ByteVector.FromUInt((uint)Width));
			data.Add(ByteVector.FromUInt((uint)Height));
			data.Add(ByteVector.FromUInt((uint)ColorDepth));
			data.Add(ByteVector.FromUInt((uint)IndexedColors));
			data.Add(ByteVector.FromUInt((uint)Data.Count));
			data.Add(Data);
			return data;
		}
#endregion
#region Public Properties
		public string MimeType
		{
			get
			{
				return mime_type;
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
				return description;
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
				return picture_data;
			}
			set
			{
				picture_data=value;
			}
		}
		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width=value;
			}
		}
		public int Height
		{
			get
			{
				return height;
			}
			set
			{
				height=value;
			}
		}
		public int ColorDepth
		{
			get
			{
				return color_depth;
			}
			set
			{
				color_depth=value;
			}
		}
		public int IndexedColors
		{
			get
			{
				return indexed_colors;
			}
			set
			{
				indexed_colors=value;
			}
		}
#endregion
	}
}
