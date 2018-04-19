using System;
namespace TagLib
{
	[Flags]
	public enum TagTypes:uint
	{
		None=0x00000000,
		Xiph=0x00000001,
		Id3v1=0x00000002,
		Id3v2=0x00000004,
		Ape=0x00000008,
		Apple=0x00000010,
		Asf=0x00000020,
		RiffInfo=0x00000040,
		MovieId=0x00000080,
		DivX=0x00000100,
		FlacMetadata=0x00000200,
		TiffIFD=0x00000400,
		XMP=0x00000800,
		JpegComment=0x00001000,
		GifComment=0x00002000,
		Png=0x00004000,
		IPTCIIM=0x00008000,
		AudibleMetadata=0x00000400,
		AllTags=0xFFFFFFFF
	}
	public abstract class Tag
	{
		public abstract TagTypes TagTypes
		{
			get;
		}
		public virtual string Title
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string TitleSort
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string[]Performers
		{
			get
			{
				return new string[]
				{
				};
			}
			set
			{
			}
		}
		public virtual string[]PerformersSort
		{
			get
			{
				return new string[]
				{
				};
			}
			set
			{
			}
		}
		public virtual string[]AlbumArtists
		{
			get
			{
				return new string[]
				{
				};
			}
			set
			{
			}
		}
		public virtual string[]AlbumArtistsSort
		{
			get
			{
				return new string[]
				{
				};
			}
			set
			{
			}
		}
		public virtual string[]Composers
		{
			get
			{
				return new string[]
				{
				};
			}
			set
			{
			}
		}
		public virtual string[]ComposersSort
		{
			get
			{
				return new string[]
				{
				};
			}
			set
			{
			}
		}
		public virtual string Album
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string AlbumSort
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string Comment
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string[]Genres
		{
			get
			{
				return new string[]
				{
				};
			}
			set
			{
			}
		}
		public virtual uint Year
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public virtual uint Track
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public virtual uint TrackCount
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public virtual uint Disc
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public virtual uint DiscCount
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public virtual string Lyrics
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string Grouping
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual uint BeatsPerMinute
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public virtual string Conductor
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string Copyright
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzArtistId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzReleaseId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzReleaseArtistId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzTrackId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzDiscId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicIpId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string AmazonId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzReleaseStatus
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzReleaseType
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string MusicBrainzReleaseCountry
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual double ReplayGainTrackGain
		{
			get
			{
				return double.NaN;
			}
			set
			{
			}
		}
		public virtual double ReplayGainTrackPeak
		{
			get
			{
				return double.NaN;
			}
			set
			{
			}
		}
		public virtual double ReplayGainAlbumGain
		{
			get
			{
				return double.NaN;
			}
			set
			{
			}
		}
		public virtual double ReplayGainAlbumPeak
		{
			get
			{
				return double.NaN;
			}
			set
			{
			}
		}
		public virtual IPicture[]Pictures
		{
			get
			{
				return new Picture[]
				{
				};
			}
			set
			{
			}
		}
		[Obsolete("For album artists use AlbumArtists. For track artists, use Performers")]
		public virtual string[]Artists
		{
			get
			{
				return Performers;
			}
			set
			{
				Performers=value;
			}
		}
		[Obsolete("For album artists use FirstAlbumArtist. For track artists, use FirstPerformer")]
		public string FirstArtist
		{
			get
			{
				return FirstPerformer;
			}
		}
		public string FirstAlbumArtist
		{
			get
			{
				return FirstInGroup(AlbumArtists);
			}
		}
		public string FirstAlbumArtistSort
		{
			get
			{
				return FirstInGroup(AlbumArtistsSort);
			}
		}
		public string FirstPerformer
		{
			get
			{
				return FirstInGroup(Performers);
			}
		}
		public string FirstPerformerSort
		{
			get
			{
				return FirstInGroup(PerformersSort);
			}
		}
		public string FirstComposerSort
		{
			get
			{
				return FirstInGroup(ComposersSort);
			}
		}
		public string FirstComposer
		{
			get
			{
				return FirstInGroup(Composers);
			}
		}
		public string FirstGenre
		{
			get
			{
				return FirstInGroup(Genres);
			}
		}
		[Obsolete("For album artists use JoinedAlbumArtists. For track artists, use JoinedPerformers")]
		public string JoinedArtists
		{
			get
			{
				return JoinedPerformers;
			}
		}
		public string JoinedAlbumArtists
		{
			get
			{
				return JoinGroup(AlbumArtists);
			}
		}
		public string JoinedPerformers
		{
			get
			{
				return JoinGroup(Performers);
			}
		}
		public string JoinedPerformersSort
		{
			get
			{
				return JoinGroup(PerformersSort);
			}
		}
		public string JoinedComposers
		{
			get
			{
				return JoinGroup(Composers);
			}
		}
		public string JoinedGenres
		{
			get
			{
				return JoinGroup(Genres);
			}
		}
		private static string FirstInGroup(string[]group)
		{
			return group==null||group.Length==0?null:group[0];
		}
		private static string JoinGroup(string[]group)
		{
			if(group==null)
			{
				return null;
			}
			return string.Join("; ",group);
		}
		public virtual bool IsEmpty
		{
			get
			{
				return IsNullOrLikeEmpty(Title)&&IsNullOrLikeEmpty(Grouping)&&IsNullOrLikeEmpty(AlbumArtists)&&IsNullOrLikeEmpty(Performers)&&IsNullOrLikeEmpty(Composers)&&IsNullOrLikeEmpty(Conductor)&&IsNullOrLikeEmpty(Copyright)&&IsNullOrLikeEmpty(Album)&&IsNullOrLikeEmpty(Comment)&&IsNullOrLikeEmpty(Genres)&&Year==0&&BeatsPerMinute==0&&Track==0&&TrackCount==0&&Disc==0&&DiscCount==0;
			}
		}
		public abstract void Clear();
		[Obsolete("Use Tag.CopyTo(Tag,bool)")]
		public static void Duplicate(Tag source,Tag target,bool overwrite)
		{
			if(source==null)
			{
				throw new ArgumentNullException("source");
			}
			if(target==null)
			{
				throw new ArgumentNullException("target");
			}
			source.CopyTo(target,overwrite);
		}
		public virtual void CopyTo(Tag target,bool overwrite)
		{
			if(target==null)
			{
				throw new ArgumentNullException("target");
			}
			if(overwrite||IsNullOrLikeEmpty(target.Title))
			{
				target.Title=Title;
			}
			if(overwrite||IsNullOrLikeEmpty(target.AlbumArtists))
			{
				target.AlbumArtists=AlbumArtists;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Performers))
			{
				target.Performers=Performers;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Composers))
			{
				target.Composers=Composers;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Album))
			{
				target.Album=Album;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Comment))
			{
				target.Comment=Comment;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Genres))
			{
				target.Genres=Genres;
			}
			if(overwrite||target.Year==0)
			{
				target.Year=Year;
			}
			if(overwrite||target.Track==0)
			{
				target.Track=Track;
			}
			if(overwrite||target.TrackCount==0)
			{
				target.TrackCount=TrackCount;
			}
			if(overwrite||target.Disc==0)
			{
				target.Disc=Disc;
			}
			if(overwrite||target.DiscCount==0)
			{
				target.DiscCount=DiscCount;
			}
			if(overwrite||target.BeatsPerMinute==0)
			{
				target.BeatsPerMinute=BeatsPerMinute;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Grouping))
			{
				target.Grouping=Grouping;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Conductor))
			{
				target.Conductor=Conductor;
			}
			if(overwrite||IsNullOrLikeEmpty(target.Copyright))
			{
				target.Copyright=Copyright;
			}
		}
		private static bool IsNullOrLikeEmpty(string value)
		{
			return value==null||value.Trim().Length==0;
		}
		private static bool IsNullOrLikeEmpty(string[]value)
		{
			if(value==null)
			{
				return true;
			}
			foreach(string s in value)
			if(!IsNullOrLikeEmpty(s))
			{
				return false;
			}
			return true;
		}
	}
}
