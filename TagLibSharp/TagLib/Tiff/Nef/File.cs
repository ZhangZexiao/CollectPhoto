using System;
using System.Collections.Generic;
using TagLib;
using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Tags;
namespace TagLib.Tiff.Nef
{
	[SupportedMimeType("taglib/nef","nef")]
	[SupportedMimeType("image/nef")]
	[SupportedMimeType("image/x-nikon-nef")]
	public class File:TagLib.Tiff.File
	{
#region public Properties
		public override bool Writeable
		{
			get
			{
				return false;
			}
		}
#endregion
#region constructors
		public File(string path,ReadStyle propertiesStyle):
		this(new File.LocalFileAbstraction(path),propertiesStyle)
		{
		}
		public File(string path):
		this(path,ReadStyle.Average)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction,propertiesStyle)
		{
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
		protected override Codec CreateCodec(int width,int height)
		{
			return new Codec(width,height,"Nikon Raw File");
		}
	}
}
