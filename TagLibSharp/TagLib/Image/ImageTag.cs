using System;
namespace TagLib.Image
{
	public abstract class ImageTag:
	Tag
	{
#region Public Properties
		public virtual string[]Keywords
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
		public virtual uint?Rating
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual DateTime?DateTime
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual ImageOrientation Orientation
		{
			get
			{
				return ImageOrientation.None;
			}
			set
			{
			}
		}
		public virtual string Software
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual double?Latitude
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual double?Longitude
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual double?Altitude
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual double?ExposureTime
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual double?FNumber
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual uint?ISOSpeedRatings
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual double?FocalLength
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual uint?FocalLengthIn35mmFilm
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string Make
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string Model
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		public virtual string Creator
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
#endregion
	}
}
