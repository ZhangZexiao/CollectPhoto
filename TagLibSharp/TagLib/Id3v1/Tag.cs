using System.Collections;
using System;
using System.Globalization;
namespace TagLib.Id3v1
{
	public class Tag:
	TagLib.Tag
	{
#region Private Static Fields
		private static StringHandler string_handler=new StringHandler();
#endregion
#region Private Fields
		private string title;
		private string artist;
		private string album;
		private string year;
		private string comment;
		private byte track;
		private byte genre;
#endregion
#region Public Static Fields
		public const uint Size=128;
		public static readonly ReadOnlyByteVector FileIdentifier="TAG";
#endregion
#region Constructors
		public Tag()
		{
			Clear();
		}
		public Tag(File file,long position)
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
			if(!data.StartsWith(FileIdentifier))
			{
				throw new CorruptFileException("ID3v1 data does not start with identifier.");
			}
			Parse(data);
		}
		public Tag(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<Size)
			{
				throw new CorruptFileException("ID3v1 data is less than 128 bytes long.");
			}
			if(!data.StartsWith(FileIdentifier))
			{
				throw new CorruptFileException("ID3v1 data does not start with identifier.");
			}
			Parse(data);
		}
#endregion
#region Public Methods
		public ByteVector Render()
		{
			ByteVector data=new ByteVector();
			data.Add(FileIdentifier);
			data.Add(string_handler.Render(title).Resize(30));
			data.Add(string_handler.Render(artist).Resize(30));
			data.Add(string_handler.Render(album).Resize(30));
			data.Add(string_handler.Render(year).Resize(4));
			data.Add(string_handler.Render(comment).Resize(28));
			data.Add((byte)0);
			data.Add(track);
			data.Add(genre);
			return data;
		}
#endregion
#region Public Static Properties
		public static StringHandler DefaultStringHandler
		{
			get
			{
				return string_handler;
			}
			set
			{
				string_handler=value;
			}
		}
#endregion
#region Private Methods
		private void Parse(ByteVector data)
		{
			title=string_handler.Parse(data.Mid(3,30));
			artist=string_handler.Parse(data.Mid(33,30));
			album=string_handler.Parse(data.Mid(63,30));
			year=string_handler.Parse(data.Mid(93,4));
			if(data[125]==0&&data[126]!=0)
			{
				comment=string_handler.Parse(data.Mid(97,28));
				track=data[126];
			}
			else
			{
				comment=string_handler.Parse(data.Mid(97,30));
			}
			genre=data[127];
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.Id3v1;
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
		public override string Album
		{
			get
			{
				return string.IsNullOrEmpty(album)?null:album;
			}
			set
			{
				album=value!=null?value.Trim():String.Empty;
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
				string genre_name=TagLib.Genres.IndexToAudio(genre);
				return(genre_name!=null)?new string[]
				{
					genre_name
				}
				:
				new string[0];
			}
			set
			{
				genre=(value==null||value.Length==0)?(byte)255:TagLib.Genres.AudioToIndex(value[0].Trim());
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
		public override uint Track
		{
			get
			{
				return track;
			}
			set
			{
				track=(byte)(value<256?value:0);
			}
		}
		public override void Clear()
		{
			title=artist=album=year=comment=null;
			track=0;
			genre=255;
		}
#endregion
	}
}
