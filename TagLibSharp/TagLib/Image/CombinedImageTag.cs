using System;
using System.Collections.Generic;
using TagLib.IFD;
using TagLib.Xmp;
namespace TagLib.Image
{
	public class CombinedImageTag:
	ImageTag
	{
#region Private Fields
		public IFDTag Exif
		{
			get;
			private set;
		}
		public XmpTag Xmp
		{
			get;
			private set;
		}
		public List<ImageTag>OtherTags
		{
			get;
			private set;
		}
		internal TagTypes AllowedTypes
		{
			get;
			private set;
		}
		public List<ImageTag>AllTags
		{
			get
			{
				if(all_tags==null)
				{
					all_tags=new List<ImageTag>();
					if(Xmp!=null)
					{
						all_tags.Add(Xmp);
					}
					if(Exif!=null)
					{
						all_tags.Add(Exif);
					}
					all_tags.AddRange(OtherTags);
				}
				return all_tags;
			}
		}
		private List<ImageTag>all_tags=null;
#endregion
#region Constructors
		public CombinedImageTag(TagTypes allowed_types)
		{
			AllowedTypes=allowed_types;
			OtherTags=new List<ImageTag>();
		}
#endregion
#region Protected Methods
		internal void AddTag(ImageTag tag)
		{
			if((tag.TagTypes&AllowedTypes)!=tag.TagTypes)
			{
				throw new Exception(String.Format("Attempted to add {0} to an image, but the only allowed types are {1}",tag.TagTypes,AllowedTypes));
			}
			if(tag is IFDTag)
			{
				Exif=tag as IFDTag;
			}
			else if(tag is XmpTag)
			{
				if(Xmp!=null&&(tag is IIM.IIMTag||Xmp is IIM.IIMTag))
				{
					var iimTag=tag as IIM.IIMTag;
					if(iimTag==null)
					{
						iimTag=Xmp as IIM.IIMTag;
						Xmp=tag as XmpTag;
					}
					if(string.IsNullOrEmpty(Xmp.Title))
					{
						Xmp.Title=iimTag.Title;
					}
					if(string.IsNullOrEmpty(Xmp.Creator))
					{
						Xmp.Creator=iimTag.Creator;
					}
					if(string.IsNullOrEmpty(Xmp.Copyright))
					{
						Xmp.Copyright=iimTag.Copyright;
					}
					if(string.IsNullOrEmpty(Xmp.Comment))
					{
						Xmp.Comment=iimTag.Comment;
					}
					if(Xmp.Keywords==null)
					{
						Xmp.Keywords=iimTag.Keywords;
					}
				}
				else
				{
					Xmp=tag as XmpTag;
				}
			}
			else
			{
				OtherTags.Add(tag);
			}
			all_tags=null;
		}
		internal void RemoveTag(ImageTag tag)
		{
			if(tag is IFDTag)
			{
				Exif=null;
			}
			else if(tag is XmpTag)
			{
				Xmp=null;
			}
			else
			{
				OtherTags.Remove(tag);
			}
			all_tags=null;
		}
#endregion
#region Public Methods (Tag)
		public override TagTypes TagTypes
		{
			get
			{
				TagTypes types=TagTypes.None;
				foreach(ImageTag tag in AllTags)
				types|=tag.TagTypes;
				return types;
			}
		}
		public override void Clear()
		{
			foreach(ImageTag tag in AllTags)
			tag.Clear();
		}
#endregion
#region Public Properties (ImageTag)
		public override string[]Keywords
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string[]value=tag.Keywords;
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
				foreach(ImageTag tag in AllTags)
				tag.Keywords=value;
			}
		}
		public override uint?Rating
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					uint?value=tag.Rating;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Rating=value;
			}
		}
		public override DateTime?DateTime
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					DateTime?value=tag.DateTime;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.DateTime=value;
			}
		}
		public override ImageOrientation Orientation
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					ImageOrientation value=tag.Orientation;
					if((uint)value>=1U&&(uint)value<=8U)
					{
						return value;
					}
				}
				return ImageOrientation.None;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Orientation=value;
			}
		}
		public override string Software
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string value=tag.Software;
					if(!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Software=value;
			}
		}
		public override double?Latitude
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					double?value=tag.Latitude;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Latitude=value;
			}
		}
		public override double?Longitude
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					double?value=tag.Longitude;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Longitude=value;
			}
		}
		public override double?Altitude
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					double?value=tag.Altitude;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Altitude=value;
			}
		}
		public override double?ExposureTime
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					double?value=tag.ExposureTime;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.ExposureTime=value;
			}
		}
		public override double?FNumber
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					double?value=tag.FNumber;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.FNumber=value;
			}
		}
		public override uint?ISOSpeedRatings
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					uint?value=tag.ISOSpeedRatings;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.ISOSpeedRatings=value;
			}
		}
		public override double?FocalLength
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					double?value=tag.FocalLength;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.FocalLength=value;
			}
		}
		public override uint?FocalLengthIn35mmFilm
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					uint?value=tag.FocalLengthIn35mmFilm;
					if(value!=null)
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.FocalLengthIn35mmFilm=value;
			}
		}
		public override string Make
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string value=tag.Make;
					if(!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Make=value;
			}
		}
		public override string Model
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string value=tag.Model;
					if(!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Model=value;
			}
		}
		public override string Creator
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string value=tag.Creator;
					if(!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Creator=value;
			}
		}
#endregion
#region Public Properties (Tag)
		public override string Title
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string value=tag.Title;
					if(!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Title=value;
			}
		}
		public override string Comment
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string value=tag.Comment;
					if(!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
				return String.Empty;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Comment=value;
			}
		}
		public override string Copyright
		{
			get
			{
				foreach(ImageTag tag in AllTags)
				{
					string value=tag.Copyright;
					if(!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
				return null;
			}
			set
			{
				foreach(ImageTag tag in AllTags)
				tag.Copyright=value;
			}
		}
#endregion
	}
}
