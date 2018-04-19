using System;
using System.Collections.Generic;
using System.IO;
using TagLib.Image;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
namespace TagLib.IFD
{
	public class IFDTag:
	ImageTag
	{
#region Private Fields
		private IFDStructure exif_ifd=null;
		private IFDStructure gps_ifd=null;
#endregion
#region Public Properties
		public IFDStructure Structure
		{
			get;
			private set;
		}
		public IFDStructure ExifIFD
		{
			get
			{
				if(exif_ifd==null)
				{
					var entry=Structure.GetEntry(0,IFDEntryTag.ExifIFD)
					as SubIFDEntry;
					if(entry==null)
					{
						exif_ifd=new IFDStructure();
						entry=new SubIFDEntry((ushort)IFDEntryTag.ExifIFD,(ushort)IFDEntryType.Long,1,exif_ifd);
						Structure.SetEntry(0,entry);
					}
					exif_ifd=entry.Structure;
				}
				return exif_ifd;
			}
		}
		public IFDStructure GPSIFD
		{
			get
			{
				if(gps_ifd==null)
				{
					var entry=Structure.GetEntry(0,IFDEntryTag.GPSIFD)
					as SubIFDEntry;
					if(entry==null)
					{
						gps_ifd=new IFDStructure();
						entry=new SubIFDEntry((ushort)IFDEntryTag.GPSIFD,(ushort)IFDEntryType.Long,1,gps_ifd);
						Structure.SetEntry(0,entry);
					}
					gps_ifd=entry.Structure;
				}
				return gps_ifd;
			}
		}
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.TiffIFD;
			}
		}
#endregion
#region Constructors
		public IFDTag()
		{
			Structure=new IFDStructure();
		}
#endregion
#region Public Methods
		public override void Clear()
		{
			throw new NotImplementedException();
		}
#endregion
#region Metadata fields
		public override string Comment
		{
			get
			{
				var comment_entry=ExifIFD.GetEntry(0,(ushort)ExifEntryTag.UserComment)
				as UserCommentIFDEntry;
				if(comment_entry==null)
				{
					var description=Structure.GetEntry(0,IFDEntryTag.ImageDescription)
					as StringIFDEntry;
					return description==null?null:description.Value;
				}
				return comment_entry.Value;
			}
			set
			{
				if(value==null)
				{
					ExifIFD.RemoveTag(0,(ushort)ExifEntryTag.UserComment);
					Structure.RemoveTag(0,(ushort)IFDEntryTag.ImageDescription);
					return;
				}
				ExifIFD.SetEntry(0,new UserCommentIFDEntry((ushort)ExifEntryTag.UserComment,value));
				Structure.SetEntry(0,new StringIFDEntry((ushort)IFDEntryTag.ImageDescription,value));
			}
		}
		public override string Copyright
		{
			get
			{
				return Structure.GetStringValue(0,(ushort)IFDEntryTag.Copyright);
			}
			set
			{
				if(value==null)
				{
					Structure.RemoveTag(0,(ushort)IFDEntryTag.Copyright);
					return;
				}
				Structure.SetEntry(0,new StringIFDEntry((ushort)IFDEntryTag.Copyright,value));
			}
		}
		public override string Creator
		{
			get
			{
				return Structure.GetStringValue(0,(ushort)IFDEntryTag.Artist);
			}
			set
			{
				Structure.SetStringValue(0,(ushort)IFDEntryTag.Artist,value);
			}
		}
		public override string Software
		{
			get
			{
				return Structure.GetStringValue(0,(ushort)IFDEntryTag.Software);
			}
			set
			{
				Structure.SetStringValue(0,(ushort)IFDEntryTag.Software,value);
			}
		}
		public override DateTime?DateTime
		{
			get
			{
				return DateTimeOriginal;
			}
			set
			{
				DateTimeOriginal=value;
			}
		}
		public DateTime?DateTimeOriginal
		{
			get
			{
				return ExifIFD.GetDateTimeValue(0,(ushort)ExifEntryTag.DateTimeOriginal);
			}
			set
			{
				if(value==null)
				{
					ExifIFD.RemoveTag(0,(ushort)ExifEntryTag.DateTimeOriginal);
					return;
				}
				ExifIFD.SetDateTimeValue(0,(ushort)ExifEntryTag.DateTimeOriginal,value.Value);
			}
		}
		public DateTime?DateTimeDigitized
		{
			get
			{
				return ExifIFD.GetDateTimeValue(0,(ushort)ExifEntryTag.DateTimeDigitized);
			}
			set
			{
				if(value==null)
				{
					ExifIFD.RemoveTag(0,(ushort)ExifEntryTag.DateTimeDigitized);
					return;
				}
				ExifIFD.SetDateTimeValue(0,(ushort)ExifEntryTag.DateTimeDigitized,value.Value);
			}
		}
		public override double?Latitude
		{
			get
			{
				var gps_ifd=GPSIFD;
				var degree_entry=gps_ifd.GetEntry(0,(ushort)GPSEntryTag.GPSLatitude)
				as RationalArrayIFDEntry;
				var degree_ref=gps_ifd.GetStringValue(0,(ushort)GPSEntryTag.GPSLatitudeRef);
				if(degree_entry==null||degree_ref==null)
				{
					return null;
				}
				Rational[]values=degree_entry.Values;
				if(values.Length!=3)
				{
					return null;
				}
				double deg=values[0]+values[1]/60.0d+values[2]/3600.0d;
				if(degree_ref=="S")
				{
					deg*= -1.0d;
				}
				return Math.Max(Math.Min(deg,90.0d),-90.0d);
			}
			set
			{
				var gps_ifd=GPSIFD;
				if(value==null)
				{
					gps_ifd.RemoveTag(0,(ushort)GPSEntryTag.GPSLatitudeRef);
					gps_ifd.RemoveTag(0,(ushort)GPSEntryTag.GPSLatitude);
					return;
				}
				double angle=value.Value;
				if(angle< -90.0d||angle>90.0d)
				{
					throw new ArgumentException("value");
				}
				InitGpsDirectory();
				gps_ifd.SetStringValue(0,(ushort)GPSEntryTag.GPSLatitudeRef,angle<0?"S":"N");
				var entry=new RationalArrayIFDEntry((ushort)GPSEntryTag.GPSLatitude,DegreeToRationals(Math.Abs(angle)));
				gps_ifd.SetEntry(0,entry);
			}
		}
		public override double?Longitude
		{
			get
			{
				var gps_ifd=GPSIFD;
				var degree_entry=gps_ifd.GetEntry(0,(ushort)GPSEntryTag.GPSLongitude)
				as RationalArrayIFDEntry;
				var degree_ref=gps_ifd.GetStringValue(0,(ushort)GPSEntryTag.GPSLongitudeRef);
				if(degree_entry==null||degree_ref==null)
				{
					return null;
				}
				Rational[]values=degree_entry.Values;
				if(values.Length!=3)
				{
					return null;
				}
				double deg=values[0]+values[1]/60.0d+values[2]/3600.0d;
				if(degree_ref=="W")
				{
					deg*= -1.0d;
				}
				return Math.Max(Math.Min(deg,180.0d),-180.0d);
			}
			set
			{
				var gps_ifd=GPSIFD;
				if(value==null)
				{
					gps_ifd.RemoveTag(0,(ushort)GPSEntryTag.GPSLongitudeRef);
					gps_ifd.RemoveTag(0,(ushort)GPSEntryTag.GPSLongitude);
					return;
				}
				double angle=value.Value;
				if(angle< -180.0d||angle>180.0d)
				{
					throw new ArgumentException("value");
				}
				InitGpsDirectory();
				gps_ifd.SetStringValue(0,(ushort)GPSEntryTag.GPSLongitudeRef,angle<0?"W":"E");
				var entry=new RationalArrayIFDEntry((ushort)GPSEntryTag.GPSLongitude,DegreeToRationals(Math.Abs(angle)));
				gps_ifd.SetEntry(0,entry);
			}
		}
		public override double?Altitude
		{
			get
			{
				var gps_ifd=GPSIFD;
				var altitude=gps_ifd.GetRationalValue(0,(ushort)GPSEntryTag.GPSAltitude);
				var ref_entry=gps_ifd.GetByteValue(0,(ushort)GPSEntryTag.GPSAltitudeRef);
				if(altitude==null)
				{
					return null;
				}
				if(ref_entry!=null&&ref_entry.Value==1)
				{
					altitude*= -1.0d;
				}
				return altitude;
			}
			set
			{
				var gps_ifd=GPSIFD;
				if(value==null)
				{
					gps_ifd.RemoveTag(0,(ushort)GPSEntryTag.GPSAltitudeRef);
					gps_ifd.RemoveTag(0,(ushort)GPSEntryTag.GPSAltitude);
					return;
				}
				double altitude=value.Value;
				InitGpsDirectory();
				gps_ifd.SetByteValue(0,(ushort)GPSEntryTag.GPSAltitudeRef,(byte)(altitude<0?1:0));
				gps_ifd.SetRationalValue(0,(ushort)GPSEntryTag.GPSAltitude,Math.Abs(altitude));
			}
		}
		public override double?ExposureTime
		{
			get
			{
				return ExifIFD.GetRationalValue(0,(ushort)ExifEntryTag.ExposureTime);
			}
			set
			{
				ExifIFD.SetRationalValue(0,(ushort)ExifEntryTag.ExposureTime,value.HasValue?(double)value:0);
			}
		}
		public override double?FNumber
		{
			get
			{
				return ExifIFD.GetRationalValue(0,(ushort)ExifEntryTag.FNumber);
			}
			set
			{
				ExifIFD.SetRationalValue(0,(ushort)ExifEntryTag.FNumber,value.HasValue?(double)value:0);
			}
		}
		public override uint?ISOSpeedRatings
		{
			get
			{
				return ExifIFD.GetLongValue(0,(ushort)ExifEntryTag.ISOSpeedRatings);
			}
			set
			{
				ExifIFD.SetLongValue(0,(ushort)ExifEntryTag.ISOSpeedRatings,value.HasValue?(uint)value:0);
			}
		}
		public override double?FocalLength
		{
			get
			{
				return ExifIFD.GetRationalValue(0,(ushort)ExifEntryTag.FocalLength);
			}
			set
			{
				ExifIFD.SetRationalValue(0,(ushort)ExifEntryTag.FocalLength,value.HasValue?(double)value:0);
			}
		}
		public override uint?FocalLengthIn35mmFilm
		{
			get
			{
				return ExifIFD.GetLongValue(0,(ushort)ExifEntryTag.FocalLengthIn35mmFilm);
			}
			set
			{
				if(value.HasValue)
				{
					ExifIFD.SetLongValue(0,(ushort)ExifEntryTag.FocalLengthIn35mmFilm,(uint)value);
				}
				else
				{
					ExifIFD.RemoveTag(0,(ushort)ExifEntryTag.FocalLengthIn35mmFilm);
				}
			}
		}
		public override ImageOrientation Orientation
		{
			get
			{
				var orientation=Structure.GetLongValue(0,(ushort)IFDEntryTag.Orientation);
				if(orientation.HasValue)
				{
					return(ImageOrientation)orientation;
				}
				return ImageOrientation.None;
			}
			set
			{
				if((uint)value<1U||(uint)value>8U)
				{
					Structure.RemoveTag(0,(ushort)IFDEntryTag.Orientation);
					return;
				}
				Structure.SetLongValue(0,(ushort)IFDEntryTag.Orientation,(uint)value);
			}
		}
		public override string Make
		{
			get
			{
				return Structure.GetStringValue(0,(ushort)IFDEntryTag.Make);
			}
			set
			{
				Structure.SetStringValue(0,(ushort)IFDEntryTag.Make,value);
			}
		}
		public override string Model
		{
			get
			{
				return Structure.GetStringValue(0,(ushort)IFDEntryTag.Model);
			}
			set
			{
				Structure.SetStringValue(0,(ushort)IFDEntryTag.Model,value);
			}
		}
#endregion
#region Private Methods
		private void InitGpsDirectory()
		{
			GPSIFD.SetStringValue(0,(ushort)GPSEntryTag.GPSVersionID,"2 0 0 0");
			GPSIFD.SetStringValue(0,(ushort)GPSEntryTag.GPSMapDatum,"WGS-84");
		}
		private Rational[]DegreeToRationals(double angle)
		{
			if(angle<0.0||angle>180.0)
			{
				throw new ArgumentException("angle");
			}
			uint deg=(uint)Math.Floor(angle);
			uint min=(uint)((angle-Math.Floor(angle))*60.0);
			uint sec=(uint)((angle-Math.Floor(angle)-(min/60.0))*360000000.0);
			Rational[]rationals=new Rational[]
			{
				new Rational(deg,1),new Rational(min,1),new Rational(sec,100000)
			};
			return rationals;
		}
#endregion
	}
}
