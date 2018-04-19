using System;
namespace TagLib.Audible
{
	[SupportedMimeType("audio/x-audible")]
	[SupportedMimeType("taglib/aa","aa")]
	[SupportedMimeType("taglib/aax","aax")]
	public class File:TagLib.File
	{
#region Private Fields
		private TagLib.Tag tag;
		private Properties properties=new Properties();
#endregion	
#region Public Static Fields
		public const short TagBlockOffset=0xBD;
		public const short OffsetToEndTagPointer=0x38;
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
			Mode=AccessMode.Read;
			try
			{
				Seek(OffsetToEndTagPointer);
				int tagLen=((int)ReadBlock(4).ToUInt(true))-TagBlockOffset;
				Seek(TagBlockOffset);
				ByteVector bv=ReadBlock(tagLen);
				tag=new TagLib.Audible.Tag(bv);
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
			TagTypesOnDisk=TagTypes;
		}
		public File(File.IFileAbstraction abstraction):
		this(abstraction,ReadStyle.Average)
		{
		}
#endregion
#region Public Methods
		public override void Save()
		{
		}
		public override void RemoveTags(TagLib.TagTypes types)
		{
		}
		public override TagLib.Tag GetTag(TagTypes type,bool create)
		{
			if(type==TagTypes.AudibleMetadata)
			{
				return tag;
			}
			return null;
		}
#endregion
#region Public Properties
		public override TagLib.Tag Tag
		{
			get
			{
				return tag;
			}
		}
		public override TagLib.Properties Properties
		{
			get
			{
				return properties;
			}
		}
#endregion
	}
}
