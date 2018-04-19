using System;
using System.Collections.Generic;
using System.Text;
namespace TagLib.Matroska
{
	public class Tag:
	TagLib.Tag
	{
#region Private fields
		private string title;
		private string author;
		private string album;
		private string comments;
		private string genres;
		private string copyright;
#endregion
#region Constructors
#endregion
#region Taglib.Tag
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
				return title;
			}
			set
			{
				title=value;
			}
		}
		public override string TitleSort
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string[]Performers
		{
			get
			{
				return new string[]
				{
					author
				};
			}
			set
			{
				author=string.Join("; ",value);
			}
		}
		public override string[]PerformersSort
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
		public override string[]AlbumArtists
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
		public override string[]AlbumArtistsSort
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
		public override string[]Composers
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
		public override string Album
		{
			get
			{
				return album;
			}
			set
			{
				album=value;
			}
		}
		public override string AlbumSort
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string Comment
		{
			get
			{
				return comments;
			}
			set
			{
				comments=value;
			}
		}
		public override string[]Genres
		{
			get
			{
				string value=genres;
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
				genres=String.Join("; ",value);
			}
		}
		public override uint Year
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public override uint Track
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public override uint TrackCount
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public override uint Disc
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public override uint DiscCount
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public override string Lyrics
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string Grouping
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		public override string Conductor
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string Copyright
		{
			get
			{
				return copyright;
			}
			set
			{
				copyright=value;
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicIpId
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				List<IPicture>l=new List<IPicture>();
				return l.ToArray();
			}
			set
			{
			}
		}
		public override bool IsEmpty
		{
			get
			{
				return false;
			}
		}
		public override void Clear()
		{
		}
#endregion
	}
}
