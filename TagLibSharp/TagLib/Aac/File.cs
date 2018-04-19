using System;
using System.Collections.Generic;
using System.Text;
namespace TagLib.Aac
{
	[SupportedMimeType("taglib/aac","aac")]
	[SupportedMimeType("audio/aac")]
	public class File:TagLib.NonContainer.File
	{
#region Private Fields
		private AudioHeader first_header;
#endregion
#region Constructors
		public File(string path,ReadStyle propertiesStyle):
		base(path,propertiesStyle)
		{
		}
		public File(string path):
		base(path)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction,propertiesStyle)
		{
		}
		public File(File.IFileAbstraction abstraction):
		base(abstraction)
		{
		}
#endregion
#region Public Methods
		public override TagLib.Tag GetTag(TagTypes type,bool create)
		{
			Tag t=(Tag as TagLib.NonContainer.Tag).GetTag(type);
			if(t!=null|| !create)
			{
				return t;
			}
			switch(type)
			{
			case TagTypes.Id3v1:
				return EndTag.AddTag(type,Tag);
			case TagTypes.Id3v2:
				return StartTag.AddTag(type,Tag);
			case TagTypes.Ape:
				return EndTag.AddTag(type,Tag);
			default:
				return null;
			}
		}
#endregion
#region Protected Methods
		protected override void ReadStart(long start,ReadStyle propertiesStyle)
		{
			if(propertiesStyle!=ReadStyle.None&& !AudioHeader.Find(out first_header,this,start,0x4000))
			{
				throw new CorruptFileException("ADTS audio header not found.");
			}
		}
		protected override void ReadEnd(long end,ReadStyle propertiesStyle)
		{
			GetTag(TagTypes.Id3v1,true);
			GetTag(TagTypes.Id3v2,true);
		}
		protected override Properties ReadProperties(long start,long end,ReadStyle propertiesStyle)
		{
			first_header.SetStreamLength(end-start);
			return new Properties(TimeSpan.Zero,first_header);
		}
#endregion
	}
}
