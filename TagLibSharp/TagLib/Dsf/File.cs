using System;
using TagLib.Id3v2;
namespace TagLib.Dsf
{
	[SupportedMimeType("taglib/dsf","dsf")]
	[SupportedMimeType("audio/x-dsf")]
	[SupportedMimeType("audio/dsf")]
	[SupportedMimeType("sound/dsf")]
	[SupportedMimeType("application/x-dsf")]
	public class File:TagLib.File
	{
#region Private Fields
		private ByteVector header_block=null;
		private Id3v2.Tag tag=null;
		private Properties properties=null;
		private uint dsf_size=0;
		private long tag_start;
		private long tag_end;
#endregion
#region Public Static Fields
		public static readonly ReadOnlyByteVector FileIdentifier="DSD ";
		public static readonly ReadOnlyByteVector FormatIdentifier="fmt ";
		public static readonly ReadOnlyByteVector ID3Identifier="ID3";
#endregion
#region Public Constructors
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
				Read(true,propertiesStyle,out dsf_size,out tag_start,out tag_end);
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
			TagTypesOnDisk=TagTypes;
			GetTag(TagTypes.Id3v2,true);
		}
		public File(File.IFileAbstraction abstraction):
		this(abstraction,ReadStyle.Average)
		{
		}
#endregion
#region Public Properties
		public override Tag Tag
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
#region Public Methods
		public override void Save()
		{
			Mode=AccessMode.Write;
			try
			{
				long original_tag_length=tag_end-tag_start;
				ByteVector data=new ByteVector();
				if(tag==null)
				{
					RemoveBlock(tag_start,original_tag_length);
					Insert(ByteVector.FromULong((ulong)(0),false),20,8);
				}
				else
				{
					data=tag.Render();
					if(tag_start==0||tag_end<tag_start)
					{
						tag_start=tag_end=Length;
						Insert(ByteVector.FromULong((ulong)(tag_start),false),20,8);
					}
					Insert(data,tag_start,data.Count);
				}
				long length=dsf_size+data.Count-original_tag_length;
				if(data.Count-original_tag_length!=0&&tag_start<=dsf_size)
				{
					Insert(ByteVector.FromULong((ulong)(length),false),12,8);
				}
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		public override void RemoveTags(TagTypes types)
		{
			if(types==TagLib.TagTypes.Id3v2||types==TagLib.TagTypes.AllTags)
			{
				tag=null;
			}
		}
		public override TagLib.Tag GetTag(TagTypes type,bool create)
		{
			TagLib.Tag id32_tag=null;
			switch(type)
			{
			case TagTypes.Id3v2:
				if(tag==null&&create)
				{
					tag=new Id3v2.Tag();
					tag.Version=2;
				}
				id32_tag=tag;
				break;
			}
			return id32_tag;
		}
#endregion
#region Private Methods
		private void Read(bool read_tags,ReadStyle style,out uint dsf_size,out long tag_start,out long tag_end)
		{
			Seek(0);
			if(ReadBlock(4)!=FileIdentifier)
			{
				throw new CorruptFileException("File does not begin with DSF identifier");
			}
			Seek(12);
			dsf_size=ReadBlock(8).ToUInt(false);
			tag_start=(long)ReadBlock(8).ToULong(false);
			tag_end= -1;
			if(header_block==null&&style!=ReadStyle.None)
			{
				long fmt_chunk_pos=Find(FormatIdentifier,0);
				if(fmt_chunk_pos== -1)
				{
					throw new CorruptFileException("No Format chunk available in DSF file.");
				}
				Seek(fmt_chunk_pos);
				header_block=ReadBlock((int)StreamHeader.Size);
				StreamHeader header=new StreamHeader(header_block,dsf_size);
				properties=new Properties(TimeSpan.Zero,header);
			}
			if(tag_start>0)
			{
				Seek(tag_start);
				if(ReadBlock(3)==ID3Identifier)
				{
					if(read_tags&&tag==null)
					{
						tag=new Id3v2.Tag(this,tag_start);
					}
					Seek(tag_start+6);
					uint tag_size=SynchData.ToUInt(ReadBlock(4))+10;
					InvariantStartPosition=tag_start;
					tag_end=InvariantEndPosition=tag_start+tag_size;
				}
			}
		}
#endregion
	}
}
