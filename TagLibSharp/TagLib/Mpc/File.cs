using System;
namespace TagLib.MusePack
{
	[SupportedMimeType("taglib/mpc","mpc")]
	[SupportedMimeType("taglib/mp+","mp+")]
	[SupportedMimeType("taglib/mpp","mpp")]
	[SupportedMimeType("audio/x-musepack")]
	public class File:TagLib.NonContainer.File
	{
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
		protected override void ReadEnd(long end,ReadStyle propertiesStyle)
		{
			GetTag(TagTypes.Ape,true);
		}
		protected override Properties ReadProperties(long start,long end,ReadStyle propertiesStyle)
		{
			StreamHeader header=new StreamHeader(this,end-start);
			return new Properties(TimeSpan.Zero,header);
		}
#endregion
	}
}
