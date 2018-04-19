using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace TagLib.Id3v2
{
	public class Tag:
	TagLib.Tag,IEnumerable<Frame>,ICloneable
	{
#region Private Static Fields
		private static string language=CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;
		private static byte default_version=3;
		private static bool force_default_version=false;
		private static StringType default_string_type=StringType.UTF8;
		private static bool force_default_string_type=false;
		private static bool use_numeric_genres=true;
#endregion
#region Private Fields
		private Header header=new Header();
		private ExtendedHeader extended_header=null;
		private List<Frame>frame_list=new List<Frame>();
#endregion
#region Constructors
		public Tag()
		{
		}
		public Tag(File file,long position)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			file.Mode=TagLib.File.AccessMode.Read;
			if(position<0||position>file.Length-Header.Size)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			Read(file,position);
		}
		public Tag(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<Header.Size)
			{
				throw new CorruptFileException("Does not contain enough header data.");
			}
			header=new Header(data);
			if(header.TagSize==0)
			{
				return;
			}
			if(data.Count-Header.Size<header.TagSize)
			{
				throw new CorruptFileException("Does not contain enough tag data.");
			}
			Parse(data.Mid((int)Header.Size,(int)header.TagSize));
		}
#endregion
#region Public Methods
		public string GetTextAsString(ByteVector ident)
		{
			Frame frame;
			if(ident[0]=='W')
			{
				frame=UrlLinkFrame.Get(this,ident,false);
			}
			else
			{
				frame=TextInformationFrame.Get(this,ident,false);
			}
			string result=frame==null?null:frame.ToString();
			return string.IsNullOrEmpty(result)?null:result;
		}
		public IEnumerable<Frame>GetFrames()
		{
			return frame_list;
		}
		public IEnumerable<Frame>GetFrames(ByteVector ident)
		{
			if(ident==null)
			{
				throw new ArgumentNullException("ident");
			}
			if(ident.Count!=4)
			{
				throw new ArgumentException("Identifier must be four bytes long.","ident");
			}
			foreach(Frame f in frame_list)
			if(f.FrameId.Equals(ident))
			{
				yield return f;
			}
		}
		public IEnumerable<T>GetFrames<T>()where T:
		Frame
		{
			foreach(Frame f in frame_list)
			{
				T tf=f as T;
				if(tf!=null)
				{
					yield return tf;
				}
			}
		}
		public IEnumerable<T>GetFrames<T>(ByteVector ident)where T:
		Frame
		{
			if(ident==null)
			{
				throw new ArgumentNullException("ident");
			}
			if(ident.Count!=4)
			{
				throw new ArgumentException("Identifier must be four bytes long.","ident");
			}
			foreach(Frame f in frame_list)
			{
				T tf=f as T;
				if(tf!=null&&f.FrameId.Equals(ident))
				{
					yield return tf;
				}
			}
		}
		public void AddFrame(Frame frame)
		{
			if(frame==null)
			{
				throw new ArgumentNullException("frame");
			}
			frame_list.Add(frame);
		}
		public void ReplaceFrame(Frame oldFrame,Frame newFrame)
		{
			if(oldFrame==null)
			{
				throw new ArgumentNullException("oldFrame");
			}
			if(newFrame==null)
			{
				throw new ArgumentNullException("newFrame");
			}
			if(oldFrame==newFrame)
			{
				return;
			}
			int i=frame_list.IndexOf(oldFrame);
			if(i>=0)
			{
				frame_list[i]=newFrame;
			}
			else
			{
				frame_list.Add(newFrame);
			}
		}
		public void RemoveFrame(Frame frame)
		{
			if(frame==null)
			{
				throw new ArgumentNullException("frame");
			}
			if(frame_list.Contains(frame))
			{
				frame_list.Remove(frame);
			}
		}
		public void RemoveFrames(ByteVector ident)
		{
			if(ident==null)
			{
				throw new ArgumentNullException("ident");
			}
			if(ident.Count!=4)
			{
				throw new ArgumentException("Identifier must be four bytes long.","ident");
			}
			for(int i=frame_list.Count-1;i>=0;i--)
			{
				if(frame_list[i].FrameId.Equals(ident))
				{
					frame_list.RemoveAt(i);
				}
			}
		}
		public void SetTextFrame(ByteVector ident,params string[]text)
		{
			if(ident==null)
			{
				throw new ArgumentNullException("ident");
			}
			if(ident.Count!=4)
			{
				throw new ArgumentException("Identifier must be four bytes long.","ident");
			}
			bool empty=true;
			if(text!=null)
			{
				for(int i=0;empty&&i<text.Length;i++)
				{
					if(!string.IsNullOrEmpty(text[i]))
					{
						empty=false;
					}
				}
			}
			if(empty)
			{
				RemoveFrames(ident);
				return;
			}
			if(ident[0]=='W')
			{
				UrlLinkFrame urlFrame=UrlLinkFrame.Get(this,ident,true);
				urlFrame.Text=text;
				urlFrame.TextEncoding=DefaultEncoding;
				return;
			}
			TextInformationFrame frame=TextInformationFrame.Get(this,ident,true);
			frame.Text=text;
			frame.TextEncoding=DefaultEncoding;
		}
		[Obsolete("Use SetTextFrame(ByteVector,String[])")]
		public void SetTextFrame(ByteVector ident,StringCollection text)
		{
			if(text==null||text.Count==0)
			{
				RemoveFrames(ident);
			}
			else
			{
				SetTextFrame(ident,text.ToArray());
			}
		}
		public void SetNumberFrame(ByteVector ident,uint number,uint count)
		{
			if(ident==null)
			{
				throw new ArgumentNullException("ident");
			}
			if(ident.Count!=4)
			{
				throw new ArgumentException("Identifier must be four bytes long.","ident");
			}
			if(number==0&&count==0)
			{
				RemoveFrames(ident);
			}
			else if(count!=0)
			{
				SetTextFrame(ident,string.Format(CultureInfo.InvariantCulture,"{0}/{1}",number,count));
			}
			else
			{
				SetTextFrame(ident,number.ToString(CultureInfo.InvariantCulture));
			}
		}
		public ByteVector Render()
		{
			bool has_footer=(header.Flags&HeaderFlags.FooterPresent)!=0;
			bool unsynchAtFrameLevel=(header.Flags&HeaderFlags.Unsynchronisation)!=0&&Version>=4;
			bool unsynchAtTagLevel=(header.Flags&HeaderFlags.Unsynchronisation)!=0&&Version<4;
			header.MajorVersion=has_footer?(byte)4:Version;
			ByteVector tag_data=new ByteVector();
			header.Flags&= ~HeaderFlags.ExtendedHeader;
			foreach(Frame frame in frame_list)
			{
				if(unsynchAtFrameLevel)
				{
					frame.Flags|=FrameFlags.Unsynchronisation;
				}
				if((frame.Flags&FrameFlags.TagAlterPreservation)!=0)
				{
					continue;
				}
				try
				{
					tag_data.Add(frame.Render(header.MajorVersion));
				}
				catch(NotImplementedException)
				{
				}
			}
			if(unsynchAtTagLevel)
			{
				SynchData.UnsynchByteVector(tag_data);
			}
			if(!has_footer)
			{
				tag_data.Add(new ByteVector((int)((tag_data.Count<header.TagSize)?(header.TagSize-tag_data.Count):1024)));
			}
			header.TagSize=(uint)tag_data.Count;
			tag_data.Insert(0,header.Render());
			if(has_footer)
			{
				tag_data.Add(new Footer(header).Render());
			}
			return tag_data;
		}
#endregion
#region Public Properties
		public HeaderFlags Flags
		{
			get
			{
				return header.Flags;
			}
			set
			{
				header.Flags=value;
			}
		}
		public byte Version
		{
			get
			{
				return ForceDefaultVersion?DefaultVersion:header.MajorVersion;
			}
			set
			{
				if(value<2||value>4)
				{
					throw new ArgumentOutOfRangeException("value","Version must be 2, 3, or 4");
				}
				header.MajorVersion=value;
			}
		}
#endregion
#region Public Static Properties
		public static string Language
		{
			get
			{
				return language;
			}
			set
			{
				language=(value==null||value.Length<3)?"   ":value.Substring(0,3);
			}
		}
		public static byte DefaultVersion
		{
			get
			{
				return default_version;
			}
			set
			{
				if(value<2||value>4)
				{
					throw new ArgumentOutOfRangeException("value","Version must be 2, 3, or 4");
				}
				default_version=value;
			}
		}
		public static bool ForceDefaultVersion
		{
			get
			{
				return force_default_version;
			}
			set
			{
				force_default_version=value;
			}
		}
		public static StringType DefaultEncoding
		{
			get
			{
				return default_string_type;
			}
			set
			{
				default_string_type=value;
			}
		}
		public static bool ForceDefaultEncoding
		{
			get
			{
				return force_default_string_type;
			}
			set
			{
				force_default_string_type=value;
			}
		}
		public static bool UseNumericGenres
		{
			get
			{
				return use_numeric_genres;
			}
			set
			{
				use_numeric_genres=value;
			}
		}
#endregion
#region Protected Methods
		protected void Read(File file,long position)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			file.Mode=File.AccessMode.Read;
			if(position<0||position>file.Length-Header.Size)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			file.Seek(position);
			header=new Header(file.ReadBlock((int)Header.Size));
			if(header.TagSize==0)
			{
				return;
			}
			Parse(file.ReadBlock((int)header.TagSize));
		}
		protected void Parse(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			bool fullTagUnsynch=(header.MajorVersion<4)&&((header.Flags&HeaderFlags.Unsynchronisation)!=0);
			if(fullTagUnsynch)
			{
				SynchData.ResynchByteVector(data);
			}
			int frame_data_position=0;
			int frame_data_length=data.Count;
			if((header.Flags&HeaderFlags.ExtendedHeader)!=0)
			{
				extended_header=new ExtendedHeader(data,header.MajorVersion);
				if(extended_header.Size<=data.Count)
				{
					frame_data_position+=(int)extended_header.Size;
					frame_data_length-=(int)extended_header.Size;
				}
			}
			TextInformationFrame tdrc=null;
			TextInformationFrame tyer=null;
			TextInformationFrame tdat=null;
			TextInformationFrame time=null;
			while(frame_data_position<frame_data_length-FrameHeader.Size(header.MajorVersion))
			{
				if(data[frame_data_position]==0)
				{
					break;
				}
				Frame frame=null;
				try
				{
					frame=FrameFactory.CreateFrame(data,ref frame_data_position,header.MajorVersion,fullTagUnsynch);
				}
				catch(NotImplementedException)
				{
					continue;
				}
				catch(CorruptFileException)
				{
					continue;
				}
				if(frame==null)
				{
					break;
				}
				if(frame.Size==0)
				{
					continue;
				}
				AddFrame(frame);
				if(header.MajorVersion==4)
				{
					continue;
				}
				if(tdrc==null&&frame.FrameId.Equals(FrameType.TDRC))
				{
					tdrc=frame as TextInformationFrame;
				}
				else if(tyer==null&&frame.FrameId.Equals(FrameType.TYER))
				{
					tyer=frame as TextInformationFrame;
				}
				else if(tdat==null&&frame.FrameId.Equals(FrameType.TDAT))
				{
					tdat=frame as TextInformationFrame;
				}
				else if(time==null&&frame.FrameId.Equals(FrameType.TIME))
				{
					time=frame as TextInformationFrame;
				}
			}
			if(tdrc==null||tdat==null||tdrc.ToString().Length>4)
			{
				return;
			}
			string year=tdrc.ToString();
			if(year.Length!=4)
			{
				return;
			}
			StringBuilder tdrc_text=new StringBuilder();
			tdrc_text.Append(year);
			if(tdat!=null)
			{
				string tdat_text=tdat.ToString();
				if(tdat_text.Length==4)
				{
					tdrc_text.Append("-").Append(tdat_text,0,2).Append("-").Append(tdat_text,2,2);
					if(time!=null)
					{
						string time_text=time.ToString();
						if(time_text.Length==4)
						{
							tdrc_text.Append("T").Append(time_text,0,2).Append(":").Append(time_text,2,2);
						}
						RemoveFrames(FrameType.TIME);
					}
				}
				RemoveFrames(FrameType.TDAT);
			}
			tdrc.Text=new string[]
			{
				tdrc_text.ToString()
			};
		}
#endregion
#region Private Methods
		private string[]GetTextAsArray(ByteVector ident)
		{
			TextInformationFrame frame=TextInformationFrame.Get(this,ident,false);
			return frame==null?new string[0]:frame.Text;
		}
		private uint GetTextAsUInt32(ByteVector ident,int index)
		{
			string text=GetTextAsString(ident);
			if(text==null)
			{
				return 0;
			}
			string[]values=text.Split(new char[]{'/'},index+2);
			if(values.Length<index+1)
			{
				return 0;
			}
			uint result;
			if(uint.TryParse(values[index],out result))
			{
				return result;
			}
			return 0;
		}
		private string GetUserTextAsString(string description,bool caseSensitive)
		{
			UserTextInformationFrame frame=UserTextInformationFrame.Get(this,description,Tag.DefaultEncoding,false,caseSensitive);
			string result=frame==null?null:string.Join(";",frame.Text);
			return string.IsNullOrEmpty(result)?null:result;
		}
		private string GetUserTextAsString(string description)
		{
			return GetUserTextAsString(description,true);
		}
		private void SetUserTextAsString(string description,string text,bool caseSensitive)
		{
			UserTextInformationFrame frame=UserTextInformationFrame.Get(this,description,Tag.DefaultEncoding,true,caseSensitive);
			if(!string.IsNullOrEmpty(text))
			{
				frame.Text=text.Split(';');
			}
			else
			{
				RemoveFrame(frame);
			}
		}
		private void SetUserTextAsString(string description,string text)
		{
			SetUserTextAsString(description,text,true);
		}
		private string GetUfidText(string owner)
		{
			UniqueFileIdentifierFrame frame=UniqueFileIdentifierFrame.Get(this,owner,false);
			string result=frame==null?null:frame.Identifier.ToString();
			return string.IsNullOrEmpty(result)?null:result;
		}
		private void SetUfidText(string owner,string text)
		{
			UniqueFileIdentifierFrame frame=UniqueFileIdentifierFrame.Get(this,owner,true);
			if(!string.IsNullOrEmpty(text))
			{
				ByteVector identifier=ByteVector.FromString(text,StringType.UTF8);
				frame.Identifier=identifier;
			}
			else
			{
				RemoveFrame(frame);
			}
		}
		private void MakeFirstOfType(Frame frame)
		{
			ByteVector type=frame.FrameId;
			Frame swapping=null;
			for(int i=0;i<frame_list.Count;i++)
			{
				if(swapping==null)
				{
					if(frame_list[i].FrameId.Equals(type))
					{
						swapping=frame;
					}
					else
					{
						continue;
					}
				}
				Frame tmp=frame_list[i];
				frame_list[i]=swapping;
				swapping=tmp;
				if(swapping==frame)
				{
					return;
				}
			}
			if(swapping!=null)
			{
				frame_list.Add(swapping);
			}
		}
#endregion
#region IEnumerable
		public IEnumerator<Frame>GetEnumerator()
		{
			return frame_list.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return frame_list.GetEnumerator();
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.Id3v2;
			}
		}
		public override string Title
		{
			get
			{
				return GetTextAsString(FrameType.TIT2);
			}
			set
			{
				SetTextFrame(FrameType.TIT2,value);
			}
		}
		public override string TitleSort
		{
			get
			{
				return GetTextAsString(FrameType.TSOT);
			}
			set
			{
				SetTextFrame(FrameType.TSOT,value);
			}
		}
		public override string[]Performers
		{
			get
			{
				return GetTextAsArray(FrameType.TPE1);
			}
			set
			{
				SetTextFrame(FrameType.TPE1,value);
			}
		}
		public override string[]PerformersSort
		{
			get
			{
				return GetTextAsArray(FrameType.TSOP);
			}
			set
			{
				SetTextFrame(FrameType.TSOP,value);
			}
		}
		public override string[]AlbumArtistsSort
		{
			get
			{
				return GetTextAsArray(FrameType.TSO2);
			}
			set
			{
				SetTextFrame(FrameType.TSO2,value);
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				return GetTextAsArray(FrameType.TPE2);
			}
			set
			{
				SetTextFrame(FrameType.TPE2,value);
			}
		}
		public override string[]Composers
		{
			get
			{
				return GetTextAsArray(FrameType.TCOM);
			}
			set
			{
				SetTextFrame(FrameType.TCOM,value);
			}
		}
		public override string[]ComposersSort
		{
			get
			{
				return GetTextAsArray(FrameType.TSOC);
			}
			set
			{
				SetTextFrame(FrameType.TSOC,value);
			}
		}
		public override string Album
		{
			get
			{
				return GetTextAsString(FrameType.TALB);
			}
			set
			{
				SetTextFrame(FrameType.TALB,value);
			}
		}
		public override string AlbumSort
		{
			get
			{
				return GetTextAsString(FrameType.TSOA);
			}
			set
			{
				SetTextFrame(FrameType.TSOA,value);
			}
		}
		public override string Comment
		{
			get
			{
				CommentsFrame f=CommentsFrame.GetPreferred(this,String.Empty,Language);
				return f!=null?f.ToString():null;
			}
			set
			{
				CommentsFrame frame;
				if(string.IsNullOrEmpty(value))
				{
					while((frame=CommentsFrame.GetPreferred(this,string.Empty,Language))!=null)RemoveFrame(frame);
					return;
				}
				frame=CommentsFrame.Get(this,String.Empty,Language,true);
				frame.Text=value;
				frame.TextEncoding=DefaultEncoding;
				MakeFirstOfType(frame);
			}
		}
		public override string[]Genres
		{
			get
			{
				string[]text=GetTextAsArray(FrameType.TCON);
				if(text.Length==0)
				{
					return text;
				}
				List<string>list=new List<string>();
				foreach(string genre in text)
				{
					if(string.IsNullOrEmpty(genre))
					{
						continue;
					}
					string genre_from_index=TagLib.Genres.IndexToAudio(genre);
					if(genre_from_index!=null)
					{
						list.Add(genre_from_index);
					}
					else
					{
						list.Add(genre);
					}
				}
				return list.ToArray();
			}
			set
			{
				if(value==null|| !use_numeric_genres)
				{
					SetTextFrame(FrameType.TCON,value);
					return;
				}
				value=(string[])value.Clone();
				for(int i=0;i<value.Length;i++)
				{
					int index=TagLib.Genres.AudioToIndex(value[i]);
					if(index!=255)
					{
						value[i]=index.ToString(CultureInfo.InvariantCulture);
					}
				}
				SetTextFrame(FrameType.TCON,value);
			}
		}
		public override uint Year
		{
			get
			{
				string text=GetTextAsString(FrameType.TDRC);
				if(text==null||text.Length<4)
				{
					return 0;
				}
				uint value;
				if(uint.TryParse(text.Substring(0,4),out value))
				{
					return value;
				}
				return 0;
			}
			set
			{
				if(value>9999)
				{
					value=0;
				}
				SetNumberFrame(FrameType.TDRC,value,0);
			}
		}
		public override uint Track
		{
			get
			{
				return GetTextAsUInt32(FrameType.TRCK,0);
			}
			set
			{
				SetNumberFrame(FrameType.TRCK,value,TrackCount);
			}
		}
		public override uint TrackCount
		{
			get
			{
				return GetTextAsUInt32(FrameType.TRCK,1);
			}
			set
			{
				SetNumberFrame(FrameType.TRCK,Track,value);
			}
		}
		public override uint Disc
		{
			get
			{
				return GetTextAsUInt32(FrameType.TPOS,0);
			}
			set
			{
				SetNumberFrame(FrameType.TPOS,value,DiscCount);
			}
		}
		public override uint DiscCount
		{
			get
			{
				return GetTextAsUInt32(FrameType.TPOS,1);
			}
			set
			{
				SetNumberFrame(FrameType.TPOS,Disc,value);
			}
		}
		public override string Lyrics
		{
			get
			{
				UnsynchronisedLyricsFrame f=UnsynchronisedLyricsFrame.GetPreferred(this,string.Empty,Language);
				return f!=null?f.ToString():null;
			}
			set
			{
				UnsynchronisedLyricsFrame frame;
				if(string.IsNullOrEmpty(value))
				{
					while((frame=UnsynchronisedLyricsFrame.GetPreferred(this,string.Empty,Language))!=null)RemoveFrame(frame);
					return;
				}
				frame=UnsynchronisedLyricsFrame.Get(this,String.Empty,Language,true);
				frame.Text=value;
				frame.TextEncoding=DefaultEncoding;
			}
		}
		public override string Grouping
		{
			get
			{
				return GetTextAsString(FrameType.TIT1);
			}
			set
			{
				SetTextFrame(FrameType.TIT1,value);
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				string text=GetTextAsString(FrameType.TBPM);
				if(text==null)
				{
					return 0;
				}
				double result;
				if(double.TryParse(text,out result)&&result>=0.0)
				{
					return(uint)Math.Round(result);
				}
				return 0;
			}
			set
			{
				SetNumberFrame(FrameType.TBPM,value,0);
			}
		}
		public override string Conductor
		{
			get
			{
				return GetTextAsString(FrameType.TPE3);
			}
			set
			{
				SetTextFrame(FrameType.TPE3,value);
			}
		}
		public override string Copyright
		{
			get
			{
				return GetTextAsString(FrameType.TCOP);
			}
			set
			{
				SetTextFrame(FrameType.TCOP,value);
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				return GetUserTextAsString("MusicBrainz Artist Id");
			}
			set
			{
				SetUserTextAsString("MusicBrainz Artist Id",value);
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				return GetUserTextAsString("MusicBrainz Album Id");
			}
			set
			{
				SetUserTextAsString("MusicBrainz Album Id",value);
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				return GetUserTextAsString("MusicBrainz Album Artist Id");
			}
			set
			{
				SetUserTextAsString("MusicBrainz Album Artist Id",value);
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				return GetUfidText("http://musicbrainz.org");
			}
			set
			{
				SetUfidText("http://musicbrainz.org",value);
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				return GetUserTextAsString("MusicBrainz Disc Id");
			}
			set
			{
				SetUserTextAsString("MusicBrainz Disc Id",value);
			}
		}
		public override string MusicIpId
		{
			get
			{
				return GetUserTextAsString("MusicIP PUID");
			}
			set
			{
				SetUserTextAsString("MusicIP PUID",value);
			}
		}
		public override string AmazonId
		{
			get
			{
				return GetUserTextAsString("ASIN");
			}
			set
			{
				SetUserTextAsString("ASIN",value);
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				return GetUserTextAsString("MusicBrainz Album Status");
			}
			set
			{
				SetUserTextAsString("MusicBrainz Album Status",value);
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				return GetUserTextAsString("MusicBrainz Album Type");
			}
			set
			{
				SetUserTextAsString("MusicBrainz Album Type",value);
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				return GetUserTextAsString("MusicBrainz Album Release Country");
			}
			set
			{
				SetUserTextAsString("MusicBrainz Album Release Country",value);
			}
		}
		public override double ReplayGainTrackGain
		{
			get
			{
				string text=GetUserTextAsString("REPLAYGAIN_TRACK_GAIN",false);
				double value;
				if(text==null)
				{
					return double.NaN;
				}
				if(text.ToLower(CultureInfo.InvariantCulture).EndsWith("db"))
				{
					text=text.Substring(0,text.Length-2).Trim();
				}
				if(double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					SetUserTextAsString("REPLAYGAIN_TRACK_GAIN",null,false);
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetUserTextAsString("REPLAYGAIN_TRACK_GAIN",text,false);
				}
			}
		}
		public override double ReplayGainTrackPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetUserTextAsString("REPLAYGAIN_TRACK_PEAK",false))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					SetUserTextAsString("REPLAYGAIN_TRACK_PEAK",null,false);
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetUserTextAsString("REPLAYGAIN_TRACK_PEAK",text,false);
				}
			}
		}
		public override double ReplayGainAlbumGain
		{
			get
			{
				string text=GetUserTextAsString("REPLAYGAIN_ALBUM_GAIN",false);
				double value;
				if(text==null)
				{
					return double.NaN;
				}
				if(text.ToLower(CultureInfo.InvariantCulture).EndsWith("db"))
				{
					text=text.Substring(0,text.Length-2).Trim();
				}
				if(double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					SetUserTextAsString("REPLAYGAIN_ALBUM_GAIN",null,false);
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetUserTextAsString("REPLAYGAIN_ALBUM_GAIN",text,false);
				}
			}
		}
		public override double ReplayGainAlbumPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetUserTextAsString("REPLAYGAIN_ALBUM_PEAK",false))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					SetUserTextAsString("REPLAYGAIN_ALBUM_PEAK",null,false);
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetUserTextAsString("REPLAYGAIN_ALBUM_PEAK",text,false);
				}
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				return new List<AttachedPictureFrame>(GetFrames<AttachedPictureFrame>(FrameType.APIC)).ToArray();
			}
			set
			{
				RemoveFrames(FrameType.APIC);
				if(value==null||value.Length==0)
				{
					return;
				}
				foreach(IPicture picture in value)
				{
					AttachedPictureFrame frame=picture as AttachedPictureFrame;
					if(frame==null)
					{
						frame=new AttachedPictureFrame(picture);
					}
					AddFrame(frame);
				}
			}
		}
		public override bool IsEmpty
		{
			get
			{
				return frame_list.Count==0;
			}
		}
		public override void Clear()
		{
			frame_list.Clear();
		}
		public bool IsCompilation
		{
			get
			{
				string val=GetTextAsString(FrameType.TCMP);
				return!string.IsNullOrEmpty(val)&&val!="0";
			}
			set
			{
				SetTextFrame(FrameType.TCMP,value?"1":null);
			}
		}
		public override void CopyTo(TagLib.Tag target,bool overwrite)
		{
			if(target==null)
			{
				throw new ArgumentNullException("target");
			}
			TagLib.Id3v2.Tag match=target as TagLib.Id3v2.Tag;
			if(match==null)
			{
				base.CopyTo(target,overwrite);
				return;
			}
			List<Frame>frames=new List<Frame>(frame_list);
			while(frames.Count>0)
			{
				ByteVector ident=frames[0].FrameId;
				bool copy=true;
				if(overwrite)
				{
					match.RemoveFrames(ident);
				}
				else
				{
					foreach(Frame f in match.frame_list)
					if(f.FrameId.Equals(ident))
					{
						copy=false;
						break;
					}
				}
				for(int i=0;i<frames.Count;)
				{
					if(frames[i].FrameId.Equals(ident))
					{
						if(copy)
						{
							match.frame_list.Add(frames[i].Clone());
						}
						frames.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}
#endregion
#region ICloneable
		public Tag Clone()
		{
			Tag tag=new Tag();
			tag.header=header;
			if(tag.extended_header!=null)
			{
				tag.extended_header=extended_header.Clone();
			}
			foreach(Frame frame in frame_list)
			tag.frame_list.Add(frame.Clone());
			return tag;
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
#endregion
	}
}
