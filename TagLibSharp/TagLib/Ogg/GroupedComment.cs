using System.Collections.Generic;
namespace TagLib.Ogg
{
	public class GroupedComment:
	Tag
	{
#region Private Fields
		private Dictionary<uint,XiphComment>comment_hash;
		private List<XiphComment>tags;
#endregion
#region Constructors
		public GroupedComment():
		base()
		{
			comment_hash=new Dictionary<uint,XiphComment>();
			tags=new List<XiphComment>();
		}
		public IEnumerable<XiphComment>Comments
		{
			get
			{
				return tags;
			}
		}
		public XiphComment GetComment(uint streamSerialNumber)
		{
			return comment_hash[streamSerialNumber];
		}
		public void AddComment(uint streamSerialNumber,XiphComment comment)
		{
			comment_hash.Add(streamSerialNumber,comment);
			tags.Add(comment);
		}
		public void AddComment(uint streamSerialNumber,ByteVector data)
		{
			AddComment(streamSerialNumber,new XiphComment(data));
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				TagTypes types=TagTypes.None;
				foreach(XiphComment tag in tags)
				if(tag!=null)
				{
					types|=tag.TagTypes;
				}
				return types;
			}
		}
		public override string Title
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Title;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Title=value;
				}
			}
		}
		public override string TitleSort
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.TitleSort;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].TitleSort=value;
				}
			}
		}
		public override string[]Performers
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string[]value=tag.Performers;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return new string[]
				{
				};
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Performers=value;
				}
			}
		}
		public override string[]PerformersSort
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string[]value=tag.PerformersSort;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return new string[]
				{
				};
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].PerformersSort=value;
				}
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string[]value=tag.AlbumArtists;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return new string[]
				{
				};
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].AlbumArtists=value;
				}
			}
		}
		public override string[]AlbumArtistsSort
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string[]value=tag.AlbumArtistsSort;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return new string[]
				{
				};
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].AlbumArtistsSort=value;
				}
			}
		}
		public override string[]Composers
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string[]value=tag.Composers;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return new string[]
				{
				};
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Composers=value;
				}
			}
		}
		public override string[]ComposersSort
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string[]value=tag.ComposersSort;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return new string[]
				{
				};
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].ComposersSort=value;
				}
			}
		}
		public override string Album
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Album;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Album=value;
				}
			}
		}
		public override string AlbumSort
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.AlbumSort;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].AlbumSort=value;
				}
			}
		}
		public override string Comment
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Comment;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Comment=value;
				}
			}
		}
		public override string[]Genres
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string[]value=tag.Genres;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return new string[]
				{
				};
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Genres=value;
				}
			}
		}
		public override uint Year
		{
			get
			{
				foreach(XiphComment tag in tags)
				if(tag!=null&&tag.Year!=0)
				{
					return tag.Year;
				}
				return 0;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Year=value;
				}
			}
		}
		public override uint Track
		{
			get
			{
				foreach(XiphComment tag in tags)
				if(tag!=null&&tag.Track!=0)
				{
					return tag.Track;
				}
				return 0;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Track=value;
				}
			}
		}
		public override uint TrackCount
		{
			get
			{
				foreach(XiphComment tag in tags)
				if(tag!=null&&tag.TrackCount!=0)
				{
					return tag.TrackCount;
				}
				return 0;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].TrackCount=value;
				}
			}
		}
		public override uint Disc
		{
			get
			{
				foreach(XiphComment tag in tags)
				if(tag!=null&&tag.Disc!=0)
				{
					return tag.Disc;
				}
				return 0;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Disc=value;
				}
			}
		}
		public override uint DiscCount
		{
			get
			{
				foreach(XiphComment tag in tags)
				if(tag!=null&&tag.DiscCount!=0)
				{
					return tag.DiscCount;
				}
				return 0;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].DiscCount=value;
				}
			}
		}
		public override string Lyrics
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Lyrics;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Lyrics=value;
				}
			}
		}
		public override string Grouping
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Grouping;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Grouping=value;
				}
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				foreach(XiphComment tag in tags)
				if(tag!=null&&tag.BeatsPerMinute!=0)
				{
					return tag.BeatsPerMinute;
				}
				return 0;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].BeatsPerMinute=value;
				}
			}
		}
		public override string Conductor
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Conductor;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Conductor=value;
				}
			}
		}
		public override string Copyright
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Copyright;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Copyright=value;
				}
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzArtistId;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzArtistId=value;
				}
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseId;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzReleaseId=value;
				}
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseArtistId;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzReleaseArtistId=value;
				}
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzTrackId;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzTrackId=value;
				}
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzDiscId;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzDiscId=value;
				}
			}
		}
		public override string MusicIpId
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicIpId;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicIpId=value;
				}
			}
		}
		public override string AmazonId
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.AmazonId;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].AmazonId=value;
				}
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseStatus;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzReleaseStatus=value;
				}
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseType;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzReleaseType=value;
				}
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseCountry;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].MusicBrainzReleaseCountry=value;
				}
			}
		}
		public override double ReplayGainTrackGain
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					double value=tag.ReplayGainTrackGain;
					if(!double.IsNaN(value))
					{
						return value;
					}
				}
				return double.NaN;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].ReplayGainTrackGain=value;
				}
			}
		}
		public override double ReplayGainTrackPeak
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					double value=tag.ReplayGainTrackPeak;
					if(!double.IsNaN(value))
					{
						return value;
					}
				}
				return double.NaN;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].ReplayGainTrackPeak=value;
				}
			}
		}
		public override double ReplayGainAlbumGain
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					double value=tag.ReplayGainAlbumGain;
					if(!double.IsNaN(value))
					{
						return value;
					}
				}
				return double.NaN;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].ReplayGainAlbumGain=value;
				}
			}
		}
		public override double ReplayGainAlbumPeak
		{
			get
			{
				foreach(XiphComment tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					double value=tag.ReplayGainAlbumPeak;
					if(!double.IsNaN(value))
					{
						return value;
					}
				}
				return double.NaN;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].ReplayGainAlbumPeak=value;
				}
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				IPicture[]output=new IPicture[0];
				foreach(XiphComment tag in tags)
				if(tag!=null&&output.Length==0)
				{
					output=tag.Pictures;
				}
				return output;
			}
			set
			{
				if(tags.Count>0)
				{
					tags[0].Pictures=value;
				}
			}
		}
		public override bool IsEmpty
		{
			get
			{
				foreach(XiphComment tag in tags)
				if(!tag.IsEmpty)
				{
					return false;
				}
				return true;
			}
		}
		public override void Clear()
		{
			foreach(XiphComment tag in tags)
			tag.Clear();
		}
#endregion
	}
}
