using System;
using System.Collections.Generic;
using System.Globalization;
namespace TagLib.Asf
{
	public class Tag:
	TagLib.Tag,IEnumerable<ContentDescriptor>
	{
#region Private Fields
		private ContentDescriptionObject description=new ContentDescriptionObject();
		private ExtendedContentDescriptionObject ext_description=new ExtendedContentDescriptionObject();
		private MetadataLibraryObject metadata_library=new MetadataLibraryObject();
#endregion
#region Constructors
		public Tag()
		{
		}
		public Tag(HeaderObject header)
		{
			if(header==null)
			{
				throw new ArgumentNullException("header");
			}
			foreach(Object child in header.Children)
			{
				if(child is ContentDescriptionObject)
				{
					description=child as ContentDescriptionObject;
				}
				if(child is ExtendedContentDescriptionObject)
				{
					ext_description=child as ExtendedContentDescriptionObject;
				}
			}
			foreach(Object child in header.Extension.Children)
			if(child is MetadataLibraryObject)
			{
				metadata_library=child as MetadataLibraryObject;
			}
		}
#endregion
#region Public Properties
		public ContentDescriptionObject ContentDescriptionObject
		{
			get
			{
				return description;
			}
		}
		public ExtendedContentDescriptionObject ExtendedContentDescriptionObject
		{
			get
			{
				return ext_description;
			}
		}
		public MetadataLibraryObject MetadataLibraryObject
		{
			get
			{
				return metadata_library;
			}
		}
#endregion
#region Public Methods
		public string GetDescriptorString(params string[]names)
		{
			if(names==null)
			{
				throw new ArgumentNullException("names");
			}
			foreach(ContentDescriptor desc in GetDescriptors(names))
			{
				if(desc==null||desc.Type!=DataType.Unicode)
				{
					continue;
				}
				string value=desc.ToString();
				if(value!=null)
				{
					return value;
				}
			}
			return null;
		}
		public string[]GetDescriptorStrings(params string[]names)
		{
			if(names==null)
			{
				throw new ArgumentNullException("names");
			}
			return SplitAndClean(GetDescriptorString(names));
		}
		public void SetDescriptorString(string value,params string[]names)
		{
			if(names==null)
			{
				throw new ArgumentNullException("names");
			}
			int index=0;
			if(value!=null&&value.Trim().Length!=0)
			{
				SetDescriptors(names[0],new ContentDescriptor(names[0],value));
				index++;
			}
			for(;index<names.Length;index++)
			{
				RemoveDescriptors(names[index]);
			}
		}
		public void SetDescriptorStrings(string[]value,params string[]names)
		{
			if(names==null)
			{
				throw new ArgumentNullException("names");
			}
			SetDescriptorString(String.Join("; ",value),names);
		}
		public void RemoveDescriptors(string name)
		{
			if(name==null)
			{
				throw new ArgumentNullException("name");
			}
			ext_description.RemoveDescriptors(name);
		}
		public IEnumerable<ContentDescriptor>GetDescriptors(params string[]names)
		{
			if(names==null)
			{
				throw new ArgumentNullException("names");
			}
			return ext_description.GetDescriptors(names);
		}
		public void SetDescriptors(string name,params ContentDescriptor[]descriptors)
		{
			if(name==null)
			{
				throw new ArgumentNullException("name");
			}
			ext_description.SetDescriptors(name,descriptors);
		}
		public void AddDescriptor(ContentDescriptor descriptor)
		{
			if(descriptor==null)
			{
				throw new ArgumentNullException("descriptor");
			}
			ext_description.AddDescriptor(descriptor);
		}
#endregion
#region Private Static Methods
		private static IPicture PictureFromData(ByteVector data)
		{
			if(data.Count<9)
			{
				return null;
			}
			int offset=0;
			Picture p=new Picture();
			p.Type=(PictureType)data[offset];
			offset+=1;
			int size=(int)data.Mid(offset,4).ToUInt(false);
			offset+=4;
			int found=data.Find(ByteVector.TextDelimiter(StringType.UTF16LE),offset,2);
			if(found<0)
			{
				return null;
			}
			p.MimeType=data.ToString(StringType.UTF16LE,offset,found-offset);
			offset=found+2;
			found=data.Find(ByteVector.TextDelimiter(StringType.UTF16LE),offset,2);
			if(found<0)
			{
				return null;
			}
			p.Description=data.ToString(StringType.UTF16LE,offset,found-offset);
			offset=found+2;
			p.Data=data.Mid(offset,size);
			return p;
		}
		private static ByteVector PictureToData(IPicture picture)
		{
			ByteVector v=new ByteVector((byte)picture.Type);
			v.Add(Object.RenderDWord((uint)picture.Data.Count));
			v.Add(Object.RenderUnicode(picture.MimeType));
			v.Add(Object.RenderUnicode(picture.Description));
			v.Add(picture.Data);
			return v;
		}
		private static string[]SplitAndClean(string s)
		{
			if(s==null||s.Trim().Length==0)
			{
				return new string[0];
			}
			string[]result=s.Split(';');
			for(int i=0;i<result.Length;i++)
			{
				result[i]=result[i].Trim();
			}
			return result;
		}
#endregion
#region IEnumerable
		public IEnumerator<ContentDescriptor>GetEnumerator()
		{
			return ext_description.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ext_description.GetEnumerator();
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.Asf;
			}
		}
		public override string Title
		{
			get
			{
				return description.Title;
			}
			set
			{
				description.Title=value;
			}
		}
		public override string TitleSort
		{
			get
			{
				return GetDescriptorString("WM/TitleSortOrder");
			}
			set
			{
				SetDescriptorString(value,"WM/TitleSortOrder");
			}
		}
		public override string[]Performers
		{
			get
			{
				return SplitAndClean(description.Author);
			}
			set
			{
				description.Author=string.Join("; ",value);
			}
		}
		public override string[]PerformersSort
		{
			get
			{
				return GetDescriptorStrings("WM/ArtistSortOrder");
			}
			set
			{
				SetDescriptorStrings(value,"WM/ArtistSortOrder");
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				return GetDescriptorStrings("WM/AlbumArtist","AlbumArtist");
			}
			set
			{
				SetDescriptorStrings(value,"WM/AlbumArtist","AlbumArtist");
			}
		}
		public override string[]AlbumArtistsSort
		{
			get
			{
				return GetDescriptorStrings("WM/AlbumArtistSortOrder");
			}
			set
			{
				SetDescriptorStrings(value,"WM/AlbumArtistSortOrder");
			}
		}
		public override string[]Composers
		{
			get
			{
				return GetDescriptorStrings("WM/Composer","Composer");
			}
			set
			{
				SetDescriptorStrings(value,"WM/Composer","Composer");
			}
		}
		public override string Album
		{
			get
			{
				return GetDescriptorString("WM/AlbumTitle","Album");
			}
			set
			{
				SetDescriptorString(value,"WM/AlbumTitle","Album");
			}
		}
		public override string AlbumSort
		{
			get
			{
				return GetDescriptorString("WM/AlbumSortOrder");
			}
			set
			{
				SetDescriptorString(value,"WM/AlbumSortOrder");
			}
		}
		public override string Comment
		{
			get
			{
				return description.Description;
			}
			set
			{
				description.Description=value;
			}
		}
		public override string[]Genres
		{
			get
			{
				string value=GetDescriptorString("WM/Genre","WM/GenreID","Genre");
				if(value==null||value.Trim().Length==0)
				{
					return new string[]
					{
					};
				}
				string[]result=value.Split(';');
				for(int i=0;i<result.Length;i++)
				{
					string genre=result[i].Trim();
					byte genre_id;
					int closing=genre.IndexOf(')');
					if(closing>0&&genre[0]=='('&&byte.TryParse(genre.Substring(1,closing-1),out genre_id))
					{
						genre=TagLib.Genres.IndexToAudio(genre_id);
					}
					result[i]=genre;
				}
				return result;
			}
			set
			{
				SetDescriptorString(String.Join("; ",value),"WM/Genre","Genre","WM/GenreID");
			}
		}
		public override uint Year
		{
			get
			{
				string text=GetDescriptorString("WM/Year");
				if(text==null||text.Length<4)
				{
					return 0;
				}
				uint value;
				if(uint.TryParse(text.Substring(0,4),NumberStyles.Integer,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return 0;
			}
			set
			{
				if(value==0)
				{
					RemoveDescriptors("WM/Year");
					return;
				}
				SetDescriptorString(value.ToString(CultureInfo.InvariantCulture),"WM/Year");
			}
		}
		public override uint Track
		{
			get
			{
				foreach(ContentDescriptor desc in GetDescriptors("WM/TrackNumber"))
				{
					uint value=desc.ToDWord();
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				if(value==0)
				{
					RemoveDescriptors("WM/TrackNumber");
				}
				else
				{
					SetDescriptors("WM/TrackNumber",new ContentDescriptor("WM/TrackNumber",value));
				}
			}
		}
		public override uint TrackCount
		{
			get
			{
				foreach(ContentDescriptor desc in GetDescriptors("TrackTotal"))
				{
					uint value=desc.ToDWord();
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				if(value==0)
				{
					RemoveDescriptors("TrackTotal");
				}
				else
				{
					SetDescriptors("TrackTotal",new ContentDescriptor("TrackTotal",value));
				}
			}
		}
		public override uint Disc
		{
			get
			{
				string text=GetDescriptorString("WM/PartOfSet");
				if(text==null)
				{
					return 0;
				}
				string[]texts=text.Split('/');
				uint value;
				if(texts.Length<1)
				{
					return 0;
				}
				return uint.TryParse(texts[0],NumberStyles.Integer,CultureInfo.InvariantCulture,out value)?value:0;
			}
			set
			{
				uint count=DiscCount;
				if(value==0&&count==0)
				{
					RemoveDescriptors("WM/PartOfSet");
					return;
				}
				if(count!=0)
				{
					SetDescriptorString(string.Format(CultureInfo.InvariantCulture,"{0}/{1}",value,count),"WM/PartOfSet");
					return;
				}
				SetDescriptorString(value.ToString(CultureInfo.InvariantCulture),"WM/PartOfSet");
			}
		}
		public override uint DiscCount
		{
			get
			{
				string text=GetDescriptorString("WM/PartOfSet");
				if(text==null)
				{
					return 0;
				}
				string[]texts=text.Split('/');
				uint value;
				if(texts.Length<2)
				{
					return 0;
				}
				return uint.TryParse(texts[1],NumberStyles.Integer,CultureInfo.InvariantCulture,out value)?value:0;
			}
			set
			{
				uint disc=Disc;
				if(disc==0&&value==0)
				{
					RemoveDescriptors("WM/PartOfSet");
					return;
				}
				if(value!=0)
				{
					SetDescriptorString(string.Format(CultureInfo.InvariantCulture,"{0}/{1}",disc,value),"WM/PartOfSet");
					return;
				}
				SetDescriptorString(disc.ToString(CultureInfo.InvariantCulture),"WM/PartOfSet");
			}
		}
		public override string Lyrics
		{
			get
			{
				return GetDescriptorString("WM/Lyrics");
			}
			set
			{
				SetDescriptorString(value,"WM/Lyrics");
			}
		}
		public override string Grouping
		{
			get
			{
				return GetDescriptorString("WM/ContentGroupDescription");
			}
			set
			{
				SetDescriptorString(value,"WM/ContentGroupDescription");
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				foreach(ContentDescriptor desc in GetDescriptors("WM/BeatsPerMinute"))
				{
					uint value=desc.ToDWord();
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				if(value==0)
				{
					RemoveDescriptors("WM/BeatsPerMinute");
					return;
				}
				SetDescriptors("WM/BeatsPerMinute",new ContentDescriptor("WM/BeatsPerMinute",value));
			}
		}
		public override string Conductor
		{
			get
			{
				return GetDescriptorString("WM/Conductor");
			}
			set
			{
				SetDescriptorString(value,"WM/Conductor");
			}
		}
		public override string Copyright
		{
			get
			{
				return description.Copyright;
			}
			set
			{
				description.Copyright=value;
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Artist Id");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Artist Id");
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Album Id");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Album Id");
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Album Artist Id");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Album Artist Id");
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Track Id");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Track Id");
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Disc Id");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Disc Id");
			}
		}
		public override string MusicIpId
		{
			get
			{
				return GetDescriptorString("MusicIP/PUID");
			}
			set
			{
				SetDescriptorString(value,"MusicIP/PUID");
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Album Status");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Album Status");
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Album Type");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Album Type");
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				return GetDescriptorString("MusicBrainz/Album Release Country");
			}
			set
			{
				SetDescriptorString(value,"MusicBrainz/Album Release Country");
			}
		}
		public override double ReplayGainTrackGain
		{
			get
			{
				string text=GetDescriptorString("ReplayGain/Track");
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
					RemoveDescriptors("ReplayGain/Track");
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetDescriptorString(text,"ReplayGain/Track");
				}
			}
		}
		public override double ReplayGainTrackPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetDescriptorString("ReplayGain/Track Peak"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					RemoveDescriptors("ReplayGain/Track Peak");
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetDescriptorString(text,"ReplayGain/Track Peak");
				}
			}
		}
		public override double ReplayGainAlbumGain
		{
			get
			{
				string text=GetDescriptorString("ReplayGain/Album");
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
					RemoveDescriptors("ReplayGain/Album");
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetDescriptorString(text,"ReplayGain/Album");
				}
			}
		}
		public override double ReplayGainAlbumPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetDescriptorString("ReplayGain/Album Peak"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					RemoveDescriptors("ReplayGain/Album Peak");
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetDescriptorString(text,"ReplayGain/Album Peak");
				}
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				List<IPicture>l=new List<IPicture>();
				foreach(ContentDescriptor descriptor in GetDescriptors("WM/Picture"))
				{
					IPicture p=PictureFromData(descriptor.ToByteVector());
					if(p!=null)
					{
						l.Add(p);
					}
				}
				foreach(DescriptionRecord record in metadata_library.GetRecords(0,0,"WM/Picture"))
				{
					IPicture p=PictureFromData(record.ToByteVector());
					if(p!=null)
					{
						l.Add(p);
					}
				}
				return l.ToArray();
			}
			set
			{
				if(value==null||value.Length==0)
				{
					RemoveDescriptors("WM/Picture");
					metadata_library.RemoveRecords(0,0,"WM/Picture");
					return;
				}
				List<ByteVector>pics=new List<ByteVector>();
				bool big_pics=false;
				foreach(IPicture pic in value)
				{
					ByteVector data=PictureToData(pic);
					pics.Add(data);
					if(data.Count>0xFFFF)
					{
						big_pics=true;
					}
				}
				if(big_pics)
				{
					DescriptionRecord[]records=new DescriptionRecord[pics.Count];
					for(int i=0;i<pics.Count;i++)
					{
						records[i]=new DescriptionRecord(0,0,"WM/Picture",pics[i]);
					}
					RemoveDescriptors("WM/Picture");
					metadata_library.SetRecords(0,0,"WM/Picture",records);
				}
				else
				{
					ContentDescriptor[]descs=new ContentDescriptor[pics.Count];
					for(int i=0;i<pics.Count;i++)
					{
						descs[i]=new ContentDescriptor("WM/Picture",pics[i]);
					}
					metadata_library.RemoveRecords(0,0,"WM/Picture");
					SetDescriptors("WM/Picture",descs);
				}
			}
		}
		public override bool IsEmpty
		{
			get
			{
				return description.IsEmpty&&ext_description.IsEmpty;
			}
		}
		public override void Clear()
		{
			description=new ContentDescriptionObject();
			ext_description=new ExtendedContentDescriptionObject();
			metadata_library.RemoveRecords(0,0,"WM/Picture");
		}
#endregion
	}
}
