using System.Collections.Generic;
namespace TagLib
{
	public class CombinedTag:
	Tag
	{
#region Private Fields
		private List<Tag>tags;
#endregion
#region Constructors
		public CombinedTag()
		{
			this.tags=new List<Tag>();
		}
		public CombinedTag(params Tag[]tags)
		{
			this.tags=new List<Tag>(tags);
		}
#endregion
#region Public Properties
		public virtual Tag[]Tags
		{
			get
			{
				return tags.ToArray();
			}
		}
#endregion
#region Public Methods
		public void SetTags(params Tag[]tags)
		{
			this.tags.Clear();
			this.tags.AddRange(tags);
		}
#endregion
#region Protected Methods
		protected void InsertTag(int index,Tag tag)
		{
			this.tags.Insert(index,tag);
		}
		protected void AddTag(Tag tag)
		{
			this.tags.Add(tag);
		}
		protected void RemoveTag(Tag tag)
		{
			this.tags.Remove(tag);
		}
		protected void ClearTags()
		{
			this.tags.Clear();
		}
#endregion
#region Overrides
		public override TagTypes TagTypes
		{
			get
			{
				TagTypes types=TagTypes.None;
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Title;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Title=value;
				}
			}
		}
		public override string[]Performers
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Performers=value;
				}
			}
		}
		public override string[]PerformersSort
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.PerformersSort=value;
				}
			}
		}
		public override string[]AlbumArtistsSort
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.AlbumArtistsSort=value;
				}
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.AlbumArtists=value;
				}
			}
		}
		public override string[]Composers
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Composers=value;
				}
			}
		}
		public override string[]ComposersSort
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.ComposersSort=value;
				}
			}
		}
		public override string TitleSort
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.TitleSort=value;
				}
			}
		}
		public override string AlbumSort
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.AlbumSort=value;
				}
			}
		}
		public override string Album
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Album;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Album=value;
				}
			}
		}
		public override string Comment
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Comment;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Comment=value;
				}
			}
		}
		public override string[]Genres
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Genres=value;
				}
			}
		}
		public override uint Year
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					uint value=tag.Year;
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Year=value;
				}
			}
		}
		public override uint Track
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					uint value=tag.Track;
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Track=value;
				}
			}
		}
		public override uint TrackCount
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					uint value=tag.TrackCount;
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.TrackCount=value;
				}
			}
		}
		public override uint Disc
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					uint value=tag.Disc;
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Disc=value;
				}
			}
		}
		public override uint DiscCount
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					uint value=tag.DiscCount;
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.DiscCount=value;
				}
			}
		}
		public override string Lyrics
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Lyrics;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Lyrics=value;
				}
			}
		}
		public override string Grouping
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Grouping;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Grouping=value;
				}
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					uint value=tag.BeatsPerMinute;
					if(value!=0)
					{
						return value;
					}
				}
				return 0;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.BeatsPerMinute=value;
				}
			}
		}
		public override string Conductor
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Conductor;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Conductor=value;
				}
			}
		}
		public override string Copyright
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.Copyright;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Copyright=value;
				}
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzArtistId;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzArtistId=value;
				}
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseId;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzReleaseId=value;
				}
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseArtistId;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzReleaseArtistId=value;
				}
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzTrackId;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzTrackId=value;
				}
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzDiscId;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzDiscId=value;
				}
			}
		}
		public override string MusicIpId
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicIpId;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicIpId=value;
				}
			}
		}
		public override string AmazonId
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.AmazonId;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.AmazonId=value;
				}
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseStatus;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzReleaseStatus=value;
				}
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseType;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzReleaseType=value;
				}
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					string value=tag.MusicBrainzReleaseCountry;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.MusicBrainzReleaseCountry=value;
				}
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				foreach(Tag tag in tags)
				{
					if(tag==null)
					{
						continue;
					}
					IPicture[]value=tag.Pictures;
					if(value!=null&&value.Length>0)
					{
						return value;
					}
				}
				return base.Pictures;
			}
			set
			{
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.Pictures=value;
				}
			}
		}
		public override double ReplayGainTrackGain
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.ReplayGainTrackGain=value;
				}
			}
		}
		public override double ReplayGainTrackPeak
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.ReplayGainTrackPeak=value;
				}
			}
		}
		public override double ReplayGainAlbumGain
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.ReplayGainAlbumGain=value;
				}
			}
		}
		public override double ReplayGainAlbumPeak
		{
			get
			{
				foreach(Tag tag in tags)
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
				foreach(Tag tag in tags)
				if(tag!=null)
				{
					tag.ReplayGainAlbumPeak=value;
				}
			}
		}
		public override bool IsEmpty
		{
			get
			{
				foreach(Tag tag in tags)
				if(tag.IsEmpty)
				{
					return true;
				}
				return false;
			}
		}
		public override void Clear()
		{
			foreach(Tag tag in tags)
			tag.Clear();
		}
#endregion
	}
}
