using System;
namespace TagLib.Image.NoMetadata
{
	[SupportedMimeType("taglib/bmp","bmp")]
	[SupportedMimeType("image/x-MS-bmp")]
	[SupportedMimeType("image/x-bmp")]
	[SupportedMimeType("taglib/ppm","ppm")]
	[SupportedMimeType("taglib/pgm","pgm")]
	[SupportedMimeType("taglib/pbm","pbm")]
	[SupportedMimeType("taglib/pnm","pnm")]
	[SupportedMimeType("image/x-portable-pixmap")]
	[SupportedMimeType("image/x-portable-graymap")]
	[SupportedMimeType("image/x-portable-bitmap")]
	[SupportedMimeType("image/x-portable-anymap")]
	[SupportedMimeType("taglib/pcx","pcx")]
	[SupportedMimeType("image/x-pcx")]
	[SupportedMimeType("taglib/svg","svg")]
	[SupportedMimeType("taglib/svgz","svgz")]
	[SupportedMimeType("image/svg+xml")]
	[SupportedMimeType("taglib/kdc","kdc")]
	[SupportedMimeType("taglib/orf","orf")]
	[SupportedMimeType("taglib/srf","srf")]
	[SupportedMimeType("taglib/crw","crw")]
	[SupportedMimeType("taglib/mrw","mrw")]
	[SupportedMimeType("taglib/raf","raf")]
	[SupportedMimeType("taglib/x3f","x3f")]
	public class File:TagLib.Image.File
	{
#region public Properties
		public override TagLib.Properties Properties
		{
			get
			{
				return null;
			}
		}
		public override bool Writeable
		{
			get
			{
				return false;
			}
		}
#endregion
#region Constructors
		public File(string path,ReadStyle propertiesStyle):
		this(new File.LocalFileAbstraction(path),propertiesStyle)
		{
		}
		public File(string path):
		this(path,ReadStyle.Average)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction)
		{
			ImageTag=new CombinedImageTag(TagTypes.XMP);
		}
		protected File(IFileAbstraction abstraction):
		this(abstraction,ReadStyle.Average)
		{
		}
#endregion
#region Public Methods
		public override void Save()
		{
			throw new NotSupportedException();
		}
#endregion
	}
}
