using System;
using TagLib.Id3v2;
namespace TagLib.Aiff
{
	[SupportedMimeType("taglib/aif","aif")]
	[SupportedMimeType("audio/x-aiff")]
	[SupportedMimeType("audio/aiff")]
	[SupportedMimeType("sound/aiff")]
	[SupportedMimeType("application/x-aiff")]
	public class File:TagLib.File
	{
#region Private Fields
		private ByteVector header_block=null;
		private Id3v2.Tag tag=null;
		private Properties properties=null;
#endregion
#region Public Static Fields
		public static readonly ReadOnlyByteVector FileIdentifier="FORM";
		public static readonly ReadOnlyByteVector CommIdentifier="COMM";
		public static readonly ReadOnlyByteVector SoundIdentifier="SSND";
		public static readonly ReadOnlyByteVector ID3Identifier="ID3 ";
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
				uint aiff_size;
				long tag_start,tag_end;
				Read(true,propertiesStyle,out aiff_size,out tag_start,out tag_end);
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
				ByteVector data=new ByteVector();
				if(tag!=null)
				{
					ByteVector tag_data=tag.Render();
					if(tag_data.Count>10)
					{
						if(tag_data.Count%2==1)
						{
							tag_data.Add(0);
						}
						data.Add("ID3 ");
						data.Add(ByteVector.FromUInt((uint)tag_data.Count,true));
						data.Add(tag_data);
					}
				}
				uint aiff_size;
				long tag_start,tag_end;
				Read(false,ReadStyle.None,out aiff_size,out tag_start,out tag_end);
				if(tag_start<12||tag_end<tag_start)
				{
					tag_start=tag_end=Length;
				}
				int length=(int)(tag_end-tag_start+8);
				Insert(data,tag_start,length);
				if(data.Count-length!=0&&tag_start<=aiff_size)
				{
					if(tag==null)
					{
						length-=16;
					}
					else
					{
						length-=8;
					}
					Insert(ByteVector.FromUInt((uint)(aiff_size+data.Count-length),true),4,4);
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
		private void Read(bool read_tags,ReadStyle style,out uint aiff_size,out long tag_start,out long tag_end)
		{
			Seek(0);
			if(ReadBlock(4)!=FileIdentifier)
			{
				throw new CorruptFileException("File does not begin with AIFF identifier");
			}
			aiff_size=ReadBlock(4).ToUInt(true);
			tag_start= -1;
			tag_end= -1;
			if(header_block==null&&style!=ReadStyle.None)
			{
				long common_chunk_pos=Find(CommIdentifier,0);
				if(common_chunk_pos== -1)
				{
					throw new CorruptFileException("No Common chunk available in AIFF file.");
				}
				Seek(common_chunk_pos);
				header_block=ReadBlock((int)StreamHeader.Size);
				StreamHeader header=new StreamHeader(header_block,aiff_size);
				properties=new Properties(TimeSpan.Zero,header);
			}
			long id3_chunk_pos= -1;
			long sound_chunk_pos=Find(SoundIdentifier,0,ID3Identifier);
			if(sound_chunk_pos== -1)
			{
				id3_chunk_pos=Find(ID3Identifier,0);
			}
			sound_chunk_pos=Find(SoundIdentifier,0);
			if(sound_chunk_pos== -1)
			{
				throw new CorruptFileException("No Sound chunk available in AIFF file.");
			}
			Seek(sound_chunk_pos+4);
			ulong sound_chunk_length=ReadBlock(4).ToULong(true);
			long start_search_pos=(long)sound_chunk_length+sound_chunk_pos+4;
			if(id3_chunk_pos== -1)
			{
				id3_chunk_pos=Find(ID3Identifier,start_search_pos);
			}
			if(id3_chunk_pos> -1)
			{
				if(read_tags&&tag==null)
				{
					tag=new Id3v2.Tag(this,id3_chunk_pos+8);
				}
				Seek(id3_chunk_pos+4);
				uint tag_size=ReadBlock(4).ToUInt(true)+8;
				tag_start=InvariantStartPosition=id3_chunk_pos;
				tag_end=InvariantEndPosition=tag_start+tag_size;
			}
		}
#endregion
	}
}
