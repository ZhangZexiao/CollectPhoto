using System.Collections;
using System;
using System.Text;
using System.Globalization;
namespace TagLib.Riff
{
	public class DivXTag:
	TagLib.Tag
	{
#region Private Fields
		private string title;
		private string artist;
		private string year;
		private string comment;
		private string genre;
		private ByteVector extra_data;
#endregion
#region Public Static Fields
		public const uint Size=128;
		public static readonly ReadOnlyByteVector FileIdentifier="DIVXTAG";
#endregion
#region Constructors
		public DivXTag()
		{
			Clear();
		}
		public DivXTag(File file,long position)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			file.Mode=TagLib.File.AccessMode.Read;
			if(position<0||position>file.Length-Size)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			file.Seek(position);
			ByteVector data=file.ReadBlock((int)Size);
			if(!data.EndsWith(FileIdentifier))
			{
				throw new CorruptFileException("DivX tag data does not end with identifier.");
			}
			Parse(data);
		}
		public DivXTag(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<Size)
			{
				throw new CorruptFileException("DivX tag data is less than 128 bytes long.");
			}
			if(!data.EndsWith(FileIdentifier))
			{
				throw new CorruptFileException("DivX tag data does not end with identifier.");
			}
			Parse(data);
		}
#endregion
#region Public Methods
		public ByteVector Render()
		{
			ByteVector data=new ByteVector();
			data.Add(ByteVector.FromString(title,StringType.Latin1).Resize(32,0x20));
			data.Add(ByteVector.FromString(artist,StringType.Latin1).Resize(28,0x20));
			data.Add(ByteVector.FromString(year,StringType.Latin1).Resize(4,0x20));
			data.Add(ByteVector.FromString(comment,StringType.Latin1).Resize(48,0x20));
			data.Add(ByteVector.FromString(genre,StringType.Latin1).Resize(3,0x20));
			data.Add(extra_data);
			data.Add(FileIdentifier);
			return data;
		}
#endregion
#region Private Methods
		private void Parse(ByteVector data)
		{
			title=data.ToString(StringType.Latin1,0,32).Trim();
			artist=data.ToString(StringType.Latin1,32,28).Trim();
			year=data.ToString(StringType.Latin1,60,4).Trim();
			comment=data.ToString(StringType.Latin1,64,48).Trim();
			genre=data.ToString(StringType.Latin1,112,3).Trim();
			extra_data=data.Mid(115,6);
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.DivX;
			}
		}
		public override string Title
		{
			get
			{
				return string.IsNullOrEmpty(title)?null:title;
			}
			set
			{
				title=value!=null?value.Trim():String.Empty;
			}
		}
		public override string[]Performers
		{
			get
			{
				return string.IsNullOrEmpty(artist)?new string[0]:artist.Split(';');
			}
			set
			{
				artist=value!=null?string.Join(";",value):string.Empty;
			}
		}
		public override string Comment
		{
			get
			{
				return string.IsNullOrEmpty(comment)?null:comment;
			}
			set
			{
				comment=value!=null?value.Trim():String.Empty;
			}
		}
		public override string[]Genres
		{
			get
			{
				string genre_name=TagLib.Genres.IndexToVideo(genre);
				return(genre_name!=null)?new string[]
				{
					genre_name
				}
				:
				new string[0];
			}
			set
			{
				genre=(value!=null&&value.Length>0)?TagLib.Genres.VideoToIndex(value[0].Trim()).ToString(CultureInfo.InvariantCulture):string.Empty;
			}
		}
		public override uint Year
		{
			get
			{
				uint value;
				return uint.TryParse(year,NumberStyles.Integer,CultureInfo.InvariantCulture,out value)?value:0;
			}
			set
			{
				year=(value>0&&value<10000)?value.ToString(CultureInfo.InvariantCulture):String.Empty;
			}
		}
		public override void Clear()
		{
			title=artist=genre=year=comment=String.Empty;
			extra_data=new ByteVector(6);
		}
#endregion
	}
}
