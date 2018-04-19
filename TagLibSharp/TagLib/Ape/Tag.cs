using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
namespace TagLib.Ape
{
	public class Tag:
	TagLib.Tag,IEnumerable<string>
	{
#region Private Static Fields
		private static string[]picture_item_names=new string[]
		{
			"Cover Art (other)",
			"Cover Art (icon)",
			"Cover Art (other icon)",
			"Cover Art (front)",
			"Cover Art (back)",
			"Cover Art (leaflet)",
			"Cover Art (media)",
			"Cover Art (lead)",
			"Cover Art (artist)",
			"Cover Art (conductor)",
			"Cover Art (band)",
			"Cover Art (composer)",
			"Cover Art (lyricist)",
			"Cover Art (studio)",
			"Cover Art (recording)",
			"Cover Art (performance)",
			"Cover Art (movie scene)",
			"Cover Art (colored fish)",
			"Cover Art (illustration)",
			"Cover Art (band logo)",
			"Cover Art (publisher logo)"
		};
#endregion
#region Private Fields
		private Footer footer=new Footer();
		private List<Item>items=new List<Item>();
#endregion
#region Public Static Properties
		[Obsolete("Use Footer.FileIdentifer")]
		public static readonly ReadOnlyByteVector FileIdentifier=Footer.FileIdentifier;
#endregion
#region Constructors
		public Tag()
		{
		}
		public Tag(TagLib.File file,long position)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			if(position<0||position>file.Length-Footer.Size)
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
			if(data.Count<Footer.Size)
			{
				throw new CorruptFileException("Does not contain enough footer data.");
			}
			footer=new Footer(data.Mid((int)(data.Count-Footer.Size)));
			if(footer.TagSize==0)
			{
				throw new CorruptFileException("Tag size out of bounds.");
			}
			if((footer.Flags&FooterFlags.IsHeader)!=0)
			{
				throw new CorruptFileException("Footer was actually header.");
			}
			if(data.Count<footer.TagSize)
			{
				throw new CorruptFileException("Does not contain enough tag data.");
			}
			Parse(data.Mid((int)(data.Count-footer.TagSize),(int)(footer.TagSize-Footer.Size)));
		}
#endregion
#region Public Properties
		public bool HeaderPresent
		{
			get
			{
				return(footer.Flags&FooterFlags.HeaderPresent)!=0;
			}
			set
			{
				if(value)
				{
					footer.Flags|=FooterFlags.HeaderPresent;
				}
				else
				{
					footer.Flags&= ~FooterFlags.HeaderPresent;
				}
			}
		}
#endregion
#region Public Methods
		public void AddValue(string key,uint number,uint count)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(number==0&&count==0)
			{
				return;
			}
			else if(count!=0)
			{
				AddValue(key,string.Format(CultureInfo.InvariantCulture,"{0}/{1}",number,count));
			}
			else
			{
				AddValue(key,number.ToString(CultureInfo.InvariantCulture));
			}
		}
		public void SetValue(string key,uint number,uint count)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(number==0&&count==0)
			{
				RemoveItem(key);
			}
			else if(count!=0)
			{
				SetValue(key,string.Format(CultureInfo.InvariantCulture,"{0}/{1}",number,count));
			}
			else
			{
				SetValue(key,number.ToString(CultureInfo.InvariantCulture));
			}
		}
		public void AddValue(string key,string value)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(string.IsNullOrEmpty(value))
			{
				return;
			}
			AddValue(key,new string[]{value});
		}
		public void SetValue(string key,string value)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(string.IsNullOrEmpty(value))
			{
				RemoveItem(key);
			}
			else
			{
				SetValue(key,new string[]{value});
			}
		}
		public void AddValue(string key,string[]value)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(value==null||value.Length==0)
			{
				return;
			}
			int index=GetItemIndex(key);
			List<string>values=new List<string>();
			if(index>=0)
			{
				values.AddRange(items[index].ToStringArray());
			}
			values.AddRange(value);
			Item item=new Item(key,values.ToArray());
			if(index>=0)
			{
				items[index]=item;
			}
			else
			{
				items.Add(item);
			}
		}
		public void SetValue(string key,string[]value)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(value==null||value.Length==0)
			{
				RemoveItem(key);
				return;
			}
			Item item=new Item(key,value);
			int index=GetItemIndex(key);
			if(index>=0)
			{
				items[index]=item;
			}
			else
			{
				items.Add(item);
			}
		}
		public Item GetItem(string key)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			StringComparison comparison=StringComparison.InvariantCultureIgnoreCase;
			foreach(Item item in items)
			if(key.Equals(item.Key,comparison))
			{
				return item;
			}
			return null;
		}
		public void SetItem(Item item)
		{
			if(item==null)
			{
				throw new ArgumentNullException("item");
			}
			int index=GetItemIndex(item.Key);
			if(index>=0)
			{
				items[index]=item;
			}
			else
			{
				items.Add(item);
			}
		}
		public void RemoveItem(string key)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			StringComparison comparison=StringComparison.InvariantCultureIgnoreCase;
			for(int i=items.Count-1;i>=0;i--)
			{
				if(key.Equals(items[i].Key,comparison))
				{
					items.RemoveAt(i);
				}
			}
		}
		public bool HasItem(string key)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			return GetItemIndex(key)>=0;
		}
		public ByteVector Render()
		{
			ByteVector data=new ByteVector();
			uint item_count=0;
			foreach(Item item in items)
			{
				data.Add(item.Render());
				item_count++;
			}
			footer.ItemCount=item_count;
			footer.TagSize=(uint)(data.Count+Footer.Size);
			HeaderPresent=true;
			data.Insert(0,footer.RenderHeader());
			data.Add(footer.RenderFooter());
			return data;
		}
#endregion
#region Protected Methods
		protected void Read(TagLib.File file,long position)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			file.Mode=File.AccessMode.Read;
			if(position<0||position>file.Length-Footer.Size)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			file.Seek(position);
			footer=new Footer(file.ReadBlock((int)Footer.Size));
			if(footer.TagSize==0)
			{
				throw new CorruptFileException("Tag size out of bounds.");
			}
			if((footer.Flags&FooterFlags.IsHeader)==0)
			{
				file.Seek(position+Footer.Size-footer.TagSize);
			}
			Parse(file.ReadBlock((int)(footer.TagSize-Footer.Size)));
		}
		protected void Parse(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			int pos=0;
			try
			{
				for(uint i=0;i<footer.ItemCount&&pos<=data.Count-11;i++)
				{
					Item item=new Item(data,pos);
					SetItem(item);
					pos+=item.Size;
				}
			}
			catch(CorruptFileException)
			{
			}
		}
#endregion
#region Private Methods
		private int GetItemIndex(string key)
		{
			StringComparison comparison=StringComparison.InvariantCultureIgnoreCase;
			for(int i=0;i<items.Count;i++)
			{
				if(key.Equals(items[i].Key,comparison))
				{
					return i;
				}
			}
			return-1;
		}
		private string GetItemAsString(string key)
		{
			Item item=GetItem(key);
			return item!=null?item.ToString():null;
		}
		private string[]GetItemAsStrings(string key)
		{
			Item item=GetItem(key);
			return item!=null?item.ToStringArray():new string[0];
		}
		private uint GetItemAsUInt32(string key,int index)
		{
			string text=GetItemAsString(key);
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
#endregion
#region IEnumerable
		public IEnumerator<string>GetEnumerator()
		{
			foreach(Item item in items)
			yield return item.Key;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.Ape;
			}
		}
		public override string Title
		{
			get
			{
				return GetItemAsString("Title");
			}
			set
			{
				SetValue("Title",value);
			}
		}
		public override string TitleSort
		{
			get
			{
				return GetItemAsString("TitleSort");
			}
			set
			{
				SetValue("TitleSort",value);
			}
		}
		public override string[]Performers
		{
			get
			{
				return GetItemAsStrings("Artist");
			}
			set
			{
				SetValue("Artist",value);
			}
		}
		public override string[]PerformersSort
		{
			get
			{
				return GetItemAsStrings("ArtistSort");
			}
			set
			{
				SetValue("ArtistSort",value);
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				string[]list=GetItemAsStrings("Album Artist");
				if(list.Length==0)
				{
					list=GetItemAsStrings("AlbumArtist");
				}
				return list;
			}
			set
			{
				SetValue("Album Artist",value);
				if(HasItem("AlbumArtist"))
				{
					SetValue("AlbumArtist",value);
				}
			}
		}
		public override string[]AlbumArtistsSort
		{
			get
			{
				return GetItemAsStrings("AlbumArtistSort");
			}
			set
			{
				SetValue("AlbumArtistSort",value);
			}
		}
		public override string[]Composers
		{
			get
			{
				return GetItemAsStrings("Composer");
			}
			set
			{
				SetValue("Composer",value);
			}
		}
		public override string[]ComposersSort
		{
			get
			{
				return GetItemAsStrings("ComposerSort");
			}
			set
			{
				SetValue("ComposerSort",value);
			}
		}
		public override string Album
		{
			get
			{
				return GetItemAsString("Album");
			}
			set
			{
				SetValue("Album",value);
			}
		}
		public override string AlbumSort
		{
			get
			{
				return GetItemAsString("AlbumSort");
			}
			set
			{
				SetValue("AlbumSort",value);
			}
		}
		public override string Comment
		{
			get
			{
				return GetItemAsString("Comment");
			}
			set
			{
				SetValue("Comment",value);
			}
		}
		public override string[]Genres
		{
			get
			{
				return GetItemAsStrings("Genre");
			}
			set
			{
				SetValue("Genre",value);
			}
		}
		public override uint Year
		{
			get
			{
				string text=GetItemAsString("Year");
				if(text==null||text.Length==0)
				{
					return 0;
				}
				uint value;
				if(uint.TryParse(text,out value)||(text.Length>=4&&uint.TryParse(text.Substring(0,4),out value)))
				{
					return value;
				}
				return 0;
			}
			set
			{
				SetValue("Year",value,0);
			}
		}
		public override uint Track
		{
			get
			{
				return GetItemAsUInt32("Track",0);
			}
			set
			{
				SetValue("Track",value,TrackCount);
			}
		}
		public override uint TrackCount
		{
			get
			{
				return GetItemAsUInt32("Track",1);
			}
			set
			{
				SetValue("Track",Track,value);
			}
		}
		public override uint Disc
		{
			get
			{
				return GetItemAsUInt32("Disc",0);
			}
			set
			{
				SetValue("Disc",value,DiscCount);
			}
		}
		public override uint DiscCount
		{
			get
			{
				return GetItemAsUInt32("Disc",1);
			}
			set
			{
				SetValue("Disc",Disc,value);
			}
		}
		public override string Lyrics
		{
			get
			{
				return GetItemAsString("Lyrics");
			}
			set
			{
				SetValue("Lyrics",value);
			}
		}
		public override string Grouping
		{
			get
			{
				return GetItemAsString("Grouping");
			}
			set
			{
				SetValue("Grouping",value);
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				string text=GetItemAsString("BPM");
				if(text==null)
				{
					return 0;
				}
				double value;
				if(double.TryParse(text,out value))
				{
					return(uint)Math.Round(value);
				}
				return 0;
			}
			set
			{
				SetValue("BPM",value,0);
			}
		}
		public override string Conductor
		{
			get
			{
				return GetItemAsString("Conductor");
			}
			set
			{
				SetValue("Conductor",value);
			}
		}
		public override string Copyright
		{
			get
			{
				return GetItemAsString("Copyright");
			}
			set
			{
				SetValue("Copyright",value);
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				return GetItemAsString("MUSICBRAINZ_ARTISTID");
			}
			set
			{
				SetValue("MUSICBRAINZ_ARTISTID",value);
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				return GetItemAsString("MUSICBRAINZ_ALBUMID");
			}
			set
			{
				SetValue("MUSICBRAINZ_ALBUMID",value);
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				return GetItemAsString("MUSICBRAINZ_ALBUMARTISTID");
			}
			set
			{
				SetValue("MUSICBRAINZ_ALBUMARTISTID",value);
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				return GetItemAsString("MUSICBRAINZ_TRACKID");
			}
			set
			{
				SetValue("MUSICBRAINZ_TRACKID",value);
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				return GetItemAsString("MUSICBRAINZ_DISCID");
			}
			set
			{
				SetValue("MUSICBRAINZ_DISCID",value);
			}
		}
		public override string MusicIpId
		{
			get
			{
				return GetItemAsString("MUSICIP_PUID");
			}
			set
			{
				SetValue("MUSICIP_PUID",value);
			}
		}
		public override string AmazonId
		{
			get
			{
				return GetItemAsString("ASIN");
			}
			set
			{
				SetValue("ASIN",value);
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				return GetItemAsString("MUSICBRAINZ_ALBUMSTATUS");
			}
			set
			{
				SetValue("MUSICBRAINZ_ALBUMSTATUS",value);
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				return GetItemAsString("MUSICBRAINZ_ALBUMTYPE");
			}
			set
			{
				SetValue("MUSICBRAINZ_ALBUMTYPE",value);
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				return GetItemAsString("RELEASECOUNTRY");
			}
			set
			{
				SetValue("RELEASECOUNTRY",value);
			}
		}
		public override double ReplayGainTrackGain
		{
			get
			{
				string text=GetItemAsString("REPLAYGAIN_TRACK_GAIN");
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
					RemoveItem("REPLAYGAIN_TRACK_GAIN");
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetValue("REPLAYGAIN_TRACK_GAIN",text);
				}
			}
		}
		public override double ReplayGainTrackPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetItemAsString("REPLAYGAIN_TRACK_PEAK"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					RemoveItem("REPLAYGAIN_TRACK_PEAK");
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetValue("REPLAYGAIN_TRACK_PEAK",text);
				}
			}
		}
		public override double ReplayGainAlbumGain
		{
			get
			{
				string text=GetItemAsString("REPLAYGAIN_ALBUM_GAIN");
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
					RemoveItem("REPLAYGAIN_ALBUM_GAIN");
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetValue("REPLAYGAIN_ALBUM_GAIN",text);
				}
			}
		}
		public override double ReplayGainAlbumPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetItemAsString("REPLAYGAIN_ALBUM_PEAK"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					RemoveItem("REPLAYGAIN_ALBUM_PEAK");
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetValue("REPLAYGAIN_ALBUM_PEAK",text);
				}
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				List<IPicture>pictures=new List<IPicture>();
				for(int i=0;i<picture_item_names.Length;i++)
				{
					Item item=GetItem(picture_item_names[i]);
					if(item==null||item.Type!=ItemType.Binary)
					{
						continue;
					}
					int index=item.Value.Find(ByteVector.TextDelimiter(StringType.UTF8));
					if(index<0)
					{
						continue;
					}
					Picture pic=new Picture(item.Value.Mid(index+1));
					pic.Description=item.Value.ToString(StringType.UTF8,0,index);
					pic.Type=(PictureType)i;
					pictures.Add(pic);
				}
				return pictures.ToArray();
			}
			set
			{
				foreach(string item_name in picture_item_names)
				RemoveItem(item_name);
				if(value==null||value.Length==0)
				{
					return;
				}
				foreach(IPicture pic in value)
				{
					int type=(int)pic.Type;
					if(type>=picture_item_names.Length)
					{
						type=0;
					}
					string name=picture_item_names[type];
					if(GetItem(name)!=null)
					{
						continue;
					}
					ByteVector data=ByteVector.FromString(pic.Description,StringType.UTF8);
					data.Add(ByteVector.TextDelimiter(StringType.UTF8));
					data.Add(pic.Data);
					SetItem(new Item(name,data));
				}
			}
		}
		public override bool IsEmpty
		{
			get
			{
				return items.Count==0;
			}
		}
		public override void Clear()
		{
			items.Clear();
		}
		public override void CopyTo(TagLib.Tag target,bool overwrite)
		{
			if(target==null)
			{
				throw new ArgumentNullException("target");
			}
			TagLib.Ape.Tag match=target as TagLib.Ape.Tag;
			if(match==null)
			{
				base.CopyTo(target,overwrite);
				return;
			}
			foreach(Item item in items)
			{
				if(!overwrite&&match.GetItem(item.Key)!=null)
				{
					continue;
				}
				match.items.Add(item.Clone());
			}
		}
#endregion
	}
}
