using System;
namespace TagLib.Mpeg
{
	[SupportedMimeType("taglib/mp3","mp3")]
	[SupportedMimeType("audio/x-mp3")]
	[SupportedMimeType("application/x-id3")]
	[SupportedMimeType("audio/mpeg")]
	[SupportedMimeType("audio/x-mpeg")]
	[SupportedMimeType("audio/x-mpeg-3")]
	[SupportedMimeType("audio/mpeg3")]
	[SupportedMimeType("audio/mp3")]
	[SupportedMimeType("taglib/m2a","m2a")]
	[SupportedMimeType("taglib/mp2","mp2")]
	[SupportedMimeType("taglib/mp1","mp1")]
	[SupportedMimeType("audio/x-mp2")]
	[SupportedMimeType("audio/x-mp1")]
	public class AudioFile:TagLib.NonContainer.File
	{
#region Private Fields
		private AudioHeader first_header;
#endregion
#region Constructors
		public AudioFile(string path,ReadStyle propertiesStyle):
		base(path,propertiesStyle)
		{
		}
		public AudioFile(string path):
		base(path)
		{
		}
		public AudioFile(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction,propertiesStyle)
		{
		}
		public AudioFile(File.IFileAbstraction abstraction):
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
				throw new CorruptFileException("MPEG audio header not found.");
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
