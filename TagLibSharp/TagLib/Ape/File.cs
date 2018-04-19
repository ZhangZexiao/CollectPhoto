using System;
namespace TagLib.Ape
{
	[SupportedMimeType("taglib/ape","ape")]
	[SupportedMimeType("audio/x-ape")]
	[SupportedMimeType("audio/ape")]
	[SupportedMimeType("application/x-ape")]
	public class File:TagLib.NonContainer.File
	{
#region Private Fields
		private ByteVector header_block=null;
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
			TagLib.Tag t=(Tag as TagLib.NonContainer.Tag).GetTag(type);
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
			if(header_block!=null&&propertiesStyle==ReadStyle.None)
			{
				return;
			}
			Seek(start);
			header_block=ReadBlock((int)StreamHeader.Size);
		}
		protected override void ReadEnd(long end,ReadStyle propertiesStyle)
		{
			GetTag(TagTypes.Ape,true);
		}
		protected override Properties ReadProperties(long start,long end,ReadStyle propertiesStyle)
		{
			StreamHeader header=new StreamHeader(header_block,end-start);
			return new Properties(TimeSpan.Zero,header);
		}
#endregion
	}
}
