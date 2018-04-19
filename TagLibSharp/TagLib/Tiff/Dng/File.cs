using System;
using System.Collections.Generic;
using TagLib;
using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Tags;
using TagLib.IFD.Entries;
namespace TagLib.Tiff.Dng
{
	[SupportedMimeType("taglib/dng","dng")]
	[SupportedMimeType("image/dng")]
	[SupportedMimeType("image/x-adobe-dng")]
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
		protected override Properties ExtractProperties()
		{
			int width=0,height=0;
			IFDTag tag=GetTag(TagTypes.TiffIFD)
			as IFDTag;
			IFDStructure structure=tag.Structure;
			var sub_ifds=structure.GetEntry(0,(ushort)IFDEntryTag.SubIFDs)
			as SubIFDArrayEntry;
			if(sub_ifds==null)
			{
				return base.ExtractProperties();
			}
			foreach(var entry in sub_ifds.Entries)
			{
				var type=entry.GetLongValue(0,(ushort)IFDEntryTag.NewSubfileType);
				if(type==0)
				{
					width=(int)(entry.GetLongValue(0,(ushort)IFDEntryTag.ImageWidth)??0);
					height=(int)(entry.GetLongValue(0,(ushort)IFDEntryTag.ImageLength)??0);
					break;
				}
			}
			if(width>0&&height>0)
			{
				return new Properties(TimeSpan.Zero,CreateCodec(width,height));
			}
			return base.ExtractProperties();
		}
		protected override Codec CreateCodec(int width,int height)
		{
			return new Codec(width,height,"Adobe Digital Negative File");
		}
	}
}
