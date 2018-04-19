using System;
namespace TagLib
{
	public enum PictureType
	{
		Other=0x00,
		FileIcon=0x01,
		OtherFileIcon=0x02,
		FrontCover=0x03,
		BackCover=0x04,
		LeafletPage=0x05,
		Media=0x06,
		LeadArtist=0x07,
		Artist=0x08,
		Conductor=0x09,
		Band=0x0A,
		Composer=0x0B,
		Lyricist=0x0C,
		RecordingLocation=0x0D,
		DuringRecording=0x0E,
		DuringPerformance=0x0F,
		MovieScreenCapture=0x10,
		ColoredFish=0x11,
		Illustration=0x12,
		BandLogo=0x13,
		PublisherLogo=0x14
	}
	public interface IPicture
	{
		string MimeType
		{
			get;
			set;
		}
		PictureType Type
		{
			get;
			set;
		}
		string Description
		{
			get;
			set;
		}
		ByteVector Data
		{
			get;
			set;
		}
	}
	public class Picture:
	IPicture
	{
#region Private Fields
		private string mime_type;
		private PictureType type;
		private string description;
		private ByteVector data;
#endregion
#region Constructors
		public Picture()
		{
		}
		public Picture(string path)
		{
			if(path==null)
			{
				throw new ArgumentNullException("path");
			}
			Data=ByteVector.FromPath(path);
			FillInMimeFromData();
			Description=path;
		}
		public Picture(File.IFileAbstraction abstraction)
		{
			if(abstraction==null)
			{
				throw new ArgumentNullException("abstraction");
			}
			Data=ByteVector.FromFile(abstraction);
			FillInMimeFromData();
			Description=abstraction.Name;
		}
		public Picture(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			Data=new ByteVector(data);
			FillInMimeFromData();
		}
#endregion
#region Public Static Methods
		[Obsolete("Use Picture(string filename) constructor instead.")]
		public static Picture CreateFromPath(string filename)
		{
			return new Picture(filename);
		}
		[Obsolete("Use Picture(File.IFileAbstraction abstraction) constructor instead.")]
		public static Picture CreateFromFile(File.IFileAbstraction abstraction)
		{
			return new Picture(abstraction);
		}
#endregion
#region Public Properties
		public string MimeType
		{
			get
			{
				return mime_type;
			}
			set
			{
				mime_type=value;
			}
		}
		public PictureType Type
		{
			get
			{
				return type;
			}
			set
			{
				type=value;
			}
		}
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description=value;
			}
		}
		public ByteVector Data
		{
			get
			{
				return data;
			}
			set
			{
				data=value;
			}
		}
#endregion
#region Private Methods
		private void FillInMimeFromData()
		{
			string mimetype="image/jpeg";
			string ext="jpg";
			if(Data.Count>=4&&(Data[1]=='P'&&Data[2]=='N'&&Data[3]=='G'))
			{
				mimetype="image/png";
				ext="png";
			}
			else if(Data.Count>=3&&(Data[0]=='G'&&Data[1]=='I'&&Data[2]=='F'))
			{
				mimetype="image/gif";
				ext="gif";
			}
			else if(Data.Count>=2&&(Data[0]=='B'&&Data[1]=='M'))
			{
				mimetype="image/bmp";
				ext="bmp";
			}
			MimeType=mimetype;
			Type=PictureType.FrontCover;
			Description="cover."+ext;
		}
#endregion
	}
}
