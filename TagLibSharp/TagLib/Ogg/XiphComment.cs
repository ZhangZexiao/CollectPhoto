using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
namespace TagLib.Ogg
{
	public class XiphComment:
	TagLib.Tag,IEnumerable<string>
	{
#region Private Fields
		private Dictionary<string,string[]>field_list=new Dictionary<string,string[]>();
		private string vendor_id;
		private string comment_field="DESCRIPTION";
#endregion
#region Constructors
		public XiphComment()
		{
		}
		public XiphComment(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			Parse(data);
		}
#endregion
#region Public Methods
		public string[]GetField(string key)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			key=key.ToUpper(CultureInfo.InvariantCulture);
			if(!field_list.ContainsKey(key))
			{
				return new string[0];
			}
			return(string[])field_list[key].Clone();
		}
		public string GetFirstField(string key)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			string[]values=GetField(key);
			return(values.Length>0)?values[0]:null;
		}
		public void SetField(string key,uint number)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(number==0)
			{
				RemoveField(key);
			}
			else
			{
				SetField(key,number.ToString(CultureInfo.InvariantCulture));
			}
		}
		public void SetField(string key,params string[]values)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			key=key.ToUpper(CultureInfo.InvariantCulture);
			if(values==null||values.Length==0)
			{
				RemoveField(key);
				return;
			}
			List<string>result=new List<string>();
			foreach(string text in values)
			if(text!=null&&text.Trim().Length!=0)
			{
				result.Add(text);
			}
			if(result.Count==0)
			{
				RemoveField(key);
			}
			else if(field_list.ContainsKey(key))
			{
				field_list[key]=result.ToArray();
			}
			else
			{
				field_list.Add(key,result.ToArray());
			}
		}
		public void RemoveField(string key)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			key=key.ToUpper(CultureInfo.InvariantCulture);
			field_list.Remove(key);
		}
		public ByteVector Render(bool addFramingBit)
		{
			ByteVector data=new ByteVector();
			ByteVector vendor_data=ByteVector.FromString(vendor_id,StringType.UTF8);
			data.Add(ByteVector.FromUInt((uint)vendor_data.Count,false));
			data.Add(vendor_data);
			data.Add(ByteVector.FromUInt(FieldCount,false));
			foreach(KeyValuePair<string,string[]>entry in field_list)
			{
				foreach(string value in entry.Value)
				{
					ByteVector field_data=ByteVector.FromString(entry.Key,StringType.UTF8);
					field_data.Add((byte)'=');
					field_data.Add(ByteVector.FromString(value,StringType.UTF8));
					data.Add(ByteVector.FromUInt((uint)field_data.Count,false));
					data.Add(field_data);
				}
			}
			if(addFramingBit)
			{
				data.Add((byte)1);
			}
			return data;
		}
#endregion
#region Public Properties
		public uint FieldCount
		{
			get
			{
				uint count=0;
				foreach(string[]values in field_list.Values)
				count+=(uint)values.Length;
				return count;
			}
		}
		public string VendorId
		{
			get
			{
				return vendor_id;
			}
		}
#endregion
#region Protected Methods
		protected void Parse(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			int pos=0;
			int vendor_length=(int)data.Mid(pos,4).ToUInt(false);
			pos+=4;
			vendor_id=data.ToString(StringType.UTF8,pos,vendor_length);
			pos+=vendor_length;
			int comment_fields=(int)data.Mid(pos,4).ToUInt(false);
			pos+=4;
			for(int i=0;i<comment_fields;i++)
			{
				int comment_length=(int)data.Mid(pos,4).ToUInt(false);
				pos+=4;
				string comment=data.ToString(StringType.UTF8,pos,comment_length);
				pos+=comment_length;
				int comment_separator_position=comment.IndexOf('=');
				if(comment_separator_position<0)
				{
					continue;
				}
				string key=comment.Substring(0,comment_separator_position).ToUpper(CultureInfo.InvariantCulture);
				string value=comment.Substring(comment_separator_position+1);
				string[]values;
				if(field_list.TryGetValue(key,out values))
				{
					Array.Resize<string>(ref values,values.Length+1);
					values[values.Length-1]=value;
					field_list[key]=values;
				}
				else
				{
					SetField(key,value);
				}
			}
		}
#endregion
#region IEnumerable
		public IEnumerator<string>GetEnumerator()
		{
			return field_list.Keys.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return field_list.Keys.GetEnumerator();
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.Xiph;
			}
		}
		public override string Title
		{
			get
			{
				return GetFirstField("TITLE");
			}
			set
			{
				SetField("TITLE",value);
			}
		}
		public override string TitleSort
		{
			get
			{
				return GetFirstField("TITLESORT");
			}
			set
			{
				SetField("TITLESORT",value);
			}
		}
		public override string[]Performers
		{
			get
			{
				return GetField("ARTIST");
			}
			set
			{
				SetField("ARTIST",value);
			}
		}
		public override string[]PerformersSort
		{
			get
			{
				return GetField("ARTISTSORT");
			}
			set
			{
				SetField("ARTISTSORT",value);
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				string[]value=GetField("ALBUMARTIST");
				if(value!=null&&value.Length>0)
				{
					return value;
				}
				value=GetField("ALBUM ARTIST");
				if(value!=null&&value.Length>0)
				{
					return value;
				}
				return GetField("ENSEMBLE");
			}
			set
			{
				SetField("ALBUMARTIST",value);
			}
		}
		public override string[]AlbumArtistsSort
		{
			get
			{
				return GetField("ALBUMARTISTSORT");
			}
			set
			{
				SetField("ALBUMARTISTSORT",value);
			}
		}
		public override string[]Composers
		{
			get
			{
				return GetField("COMPOSER");
			}
			set
			{
				SetField("COMPOSER",value);
			}
		}
		public override string[]ComposersSort
		{
			get
			{
				return GetField("COMPOSERSORT");
			}
			set
			{
				SetField("COMPOSERSORT",value);
			}
		}
		public override string Album
		{
			get
			{
				return GetFirstField("ALBUM");
			}
			set
			{
				SetField("ALBUM",value);
			}
		}
		public override string AlbumSort
		{
			get
			{
				return GetFirstField("ALBUMSORT");
			}
			set
			{
				SetField("ALBUMSORT",value);
			}
		}
		public override string Comment
		{
			get
			{
				string value=GetFirstField(comment_field);
				if(value!=null||comment_field=="COMMENT")
				{
					return value;
				}
				comment_field="COMMENT";
				return GetFirstField(comment_field);
			}
			set
			{
				SetField(comment_field,value);
			}
		}
		public override string[]Genres
		{
			get
			{
				return GetField("GENRE");
			}
			set
			{
				SetField("GENRE",value);
			}
		}
		public override uint Year
		{
			get
			{
				string text=GetFirstField("DATE");
				uint value;
				return(text!=null&&uint.TryParse(text.Length>4?text.Substring(0,4):text,out value))?value:0;
			}
			set
			{
				SetField("DATE",value);
			}
		}
		public override uint Track
		{
			get
			{
				string text=GetFirstField("TRACKNUMBER");
				string[]values;
				uint value;
				if(text!=null&&(values=text.Split('/')).Length>0&&uint.TryParse(values[0],out value))
				{
					return value;
				}
				return 0;
			}
			set
			{
				SetField("TRACKTOTAL",TrackCount);
				SetField("TRACKNUMBER",value);
			}
		}
		public override uint TrackCount
		{
			get
			{
				string text;
				string[]values;
				uint value;
				if((text=GetFirstField("TRACKTOTAL"))!=null&&uint.TryParse(text,out value))
				{
					return value;
				}
				if((text=GetFirstField("TRACKNUMBER"))!=null&&(values=text.Split('/')).Length>1&&uint.TryParse(values[1],out value))
				{
					return value;
				}
				return 0;
			}
			set
			{
				SetField("TRACKTOTAL",value);
			}
		}
		public override uint Disc
		{
			get
			{
				string text=GetFirstField("DISCNUMBER");
				string[]values;
				uint value;
				if(text!=null&&(values=text.Split('/')).Length>0&&uint.TryParse(values[0],out value))
				{
					return value;
				}
				return 0;
			}
			set
			{
				SetField("DISCTOTAL",DiscCount);
				SetField("DISCNUMBER",value);
			}
		}
		public override uint DiscCount
		{
			get
			{
				string text;
				string[]values;
				uint value;
				if((text=GetFirstField("DISCTOTAL"))!=null&&uint.TryParse(text,out value))
				{
					return value;
				}
				if((text=GetFirstField("DISCNUMBER"))!=null&&(values=text.Split('/')).Length>1&&uint.TryParse(values[1],out value))
				{
					return value;
				}
				return 0;
			}
			set
			{
				SetField("DISCTOTAL",value);
			}
		}
		public override string Lyrics
		{
			get
			{
				return GetFirstField("LYRICS");
			}
			set
			{
				SetField("LYRICS",value);
			}
		}
		public override string Grouping
		{
			get
			{
				return GetFirstField("GROUPING");
			}
			set
			{
				SetField("GROUPING",value);
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				string text=GetFirstField("TEMPO");
				double value;
				return(text!=null&&double.TryParse(text,out value)&&value>0)?(uint)Math.Round(value):0;
			}
			set
			{
				SetField("TEMPO",value);
			}
		}
		public override string Conductor
		{
			get
			{
				return GetFirstField("CONDUCTOR");
			}
			set
			{
				SetField("CONDUCTOR",value);
			}
		}
		public override string Copyright
		{
			get
			{
				return GetFirstField("COPYRIGHT");
			}
			set
			{
				SetField("COPYRIGHT",value);
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				return GetFirstField("MUSICBRAINZ_ARTISTID");
			}
			set
			{
				SetField("MUSICBRAINZ_ARTISTID",value);
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				return GetFirstField("MUSICBRAINZ_ALBUMID");
			}
			set
			{
				SetField("MUSICBRAINZ_ALBUMID",value);
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				return GetFirstField("MUSICBRAINZ_ALBUMARTISTID");
			}
			set
			{
				SetField("MUSICBRAINZ_ALBUMARTISTID",value);
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				return GetFirstField("MUSICBRAINZ_TRACKID");
			}
			set
			{
				SetField("MUSICBRAINZ_TRACKID",value);
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				return GetFirstField("MUSICBRAINZ_DISCID");
			}
			set
			{
				SetField("MUSICBRAINZ_DISCID",value);
			}
		}
		public override string MusicIpId
		{
			get
			{
				return GetFirstField("MUSICIP_PUID");
			}
			set
			{
				SetField("MUSICIP_PUID",value);
			}
		}
		public override string AmazonId
		{
			get
			{
				return GetFirstField("ASIN");
			}
			set
			{
				SetField("ASIN",value);
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				return GetFirstField("MUSICBRAINZ_ALBUMSTATUS");
			}
			set
			{
				SetField("MUSICBRAINZ_ALBUMSTATUS",value);
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				return GetFirstField("MUSICBRAINZ_ALBUMTYPE");
			}
			set
			{
				SetField("MUSICBRAINZ_ALBUMTYPE",value);
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				return GetFirstField("RELEASECOUNTRY");
			}
			set
			{
				SetField("RELEASECOUNTRY",value);
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				string[]covers=GetField("COVERART");
				IPicture[]pictures=new Picture[covers.Length];
				for(int ii=0;ii<covers.Length;ii++)
				{
					ByteVector data=new ByteVector(Convert.FromBase64String(covers[ii]));
					pictures[ii]=new Picture(data);
				}
				return pictures;
			}
			set
			{
				string[]covers=new string[value.Length];
				for(int ii=0;ii<value.Length;ii++)
				{
					IPicture old=value[ii];
					covers[ii]=Convert.ToBase64String(old.Data.Data);
				}
				SetField("COVERART",covers);
			}
		}
		public bool IsCompilation
		{
			get
			{
				string text;
				int value;
				if((text=GetFirstField("COMPILATION"))!=null&&int.TryParse(text,out value))
				{
					return value==1;
				}
				return false;
			}
			set
			{
				if(value)
				{
					SetField("COMPILATION","1");
				}
				else
				{
					RemoveField("COMPILATION");
				}
			}
		}
		public override double ReplayGainTrackGain
		{
			get
			{
				string text=GetFirstField("REPLAYGAIN_TRACK_GAIN");
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
					RemoveField("REPLAYGAIN_TRACK_GAIN");
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetField("REPLAYGAIN_TRACK_GAIN",text);
				}
			}
		}
		public override double ReplayGainTrackPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetFirstField("REPLAYGAIN_TRACK_PEAK"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					RemoveField("REPLAYGAIN_TRACK_PEAK");
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetField("REPLAYGAIN_TRACK_PEAK",text);
				}
			}
		}
		public override double ReplayGainAlbumGain
		{
			get
			{
				string text=GetFirstField("REPLAYGAIN_ALBUM_GAIN");
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
					RemoveField("REPLAYGAIN_ALBUM_GAIN");
				}
				else
				{
					string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
					SetField("REPLAYGAIN_ALBUM_GAIN",text);
				}
			}
		}
		public override double ReplayGainAlbumPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetFirstField("REPLAYGAIN_ALBUM_PEAK"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				if(double.IsNaN(value))
				{
					RemoveField("REPLAYGAIN_ALBUM_PEAK");
				}
				else
				{
					string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
					SetField("REPLAYGAIN_ALBUM_PEAK",text);
				}
			}
		}
		public override bool IsEmpty
		{
			get
			{
				foreach(string[]values in field_list.Values)
				if(values.Length!=0)
				{
					return false;
				}
				return true;
			}
		}
		public override void Clear()
		{
			field_list.Clear();
		}
#endregion
	}
}
