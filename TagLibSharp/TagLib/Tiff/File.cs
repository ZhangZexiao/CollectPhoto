using System;
using System.Collections.Generic;
using System.IO;
using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.Xmp;
namespace TagLib.Tiff
{
	[SupportedMimeType("taglib/tiff","tiff")]
	[SupportedMimeType("taglib/tif","tif")]
	[SupportedMimeType("image/tiff")]
	public class File:BaseTiffFile
	{
#region Private Fields
		private Properties properties;
#endregion
#region Constructors
		public File(string path,ReadStyle propertiesStyle):
		this(new File.LocalFileAbstraction(path),propertiesStyle)
		{
		}
		public File(string path):
		base(path)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction)
		{
			ImageTag=new CombinedImageTag(TagTypes.TiffIFD|TagTypes.XMP);
			Mode=AccessMode.Read;
			try
			{
				Read(propertiesStyle);
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		protected File(IFileAbstraction abstraction):
		base(abstraction)
		{
		}
#endregion
#region Public Properties
		public override TagLib.Properties Properties
		{
			get
			{
				return properties;
			}
		}
#endregion
#region Public Methods
		public override void Save()
		{
			Mode=AccessMode.Write;
			try
			{
				WriteFile();
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
#endregion
#region Private Methods
		private void WriteFile()
		{
			IFDTag exif=ImageTag.Exif;
			if(exif==null)
			{
				throw new Exception("Tiff file without tags");
			}
			UpdateTags(exif);
			uint first_ifd_offset=8;
			ByteVector data=RenderHeader(first_ifd_offset);
			var renderer=new IFDRenderer(IsBigEndian,exif.Structure,first_ifd_offset);
			data.Add(renderer.Render());
			Insert(data,0,Length);
		}
		private void UpdateTags(IFDTag exif)
		{
			exif.Structure.RemoveTag(0,(ushort)IFDEntryTag.XMP);
			XmpTag xmp=ImageTag.Xmp;
			if(xmp!=null)
			{
				exif.Structure.AddEntry(0,new ByteVectorIFDEntry((ushort)IFDEntryTag.XMP,xmp.Render()));
			}
		}
		protected void Read(ReadStyle propertiesStyle)
		{
			Mode=AccessMode.Read;
			try
			{
				uint first_ifd_offset=ReadHeader();
				ReadIFD(first_ifd_offset);
				var xmp_entry=ImageTag.Exif.Structure.GetEntry(0,(ushort)IFDEntryTag.XMP)
				as ByteVectorIFDEntry;
				if(xmp_entry!=null)
				{
					ImageTag.AddTag(new XmpTag(xmp_entry.Data.ToString(),this));
				}
				if(propertiesStyle==ReadStyle.None)
				{
					return;
				}
				properties=ExtractProperties();
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		protected virtual Properties ExtractProperties()
		{
			int width=0,height=0;
			IFDTag tag=GetTag(TagTypes.TiffIFD)
			as IFDTag;
			IFDStructure structure=tag.Structure;
			width=(int)(structure.GetLongValue(0,(ushort)IFDEntryTag.ImageWidth)??0);
			height=(int)(structure.GetLongValue(0,(ushort)IFDEntryTag.ImageLength)??0);
			if(width>0&&height>0)
			{
				return new Properties(TimeSpan.Zero,CreateCodec(width,height));
			}
			return null;
		}
		protected virtual Codec CreateCodec(int width,int height)
		{
			return new Codec(width,height);
		}
#endregion
	}
}
