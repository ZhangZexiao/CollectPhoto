using System;
using System.Collections.Generic;
namespace TagLib.Flac
{
	[SupportedMimeType("taglib/flac","flac")]
	[SupportedMimeType("audio/x-flac")]
	[SupportedMimeType("application/x-flac")]
	[SupportedMimeType("audio/flac")]
	public class File:TagLib.NonContainer.File
	{
#region Private Fields
		private Metadata metadata=null;
		private CombinedTag tag=null;
		private ByteVector header_block=null;
		private long stream_start=0;
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
#region Public Properties
		public override TagLib.Tag Tag
		{
			get
			{
				return tag;
			}
		}
#endregion
#region Public Methods
		public override void Save()
		{
			Mode=AccessMode.Write;
			try
			{
				long metadata_start=StartTag.Write();
				long metadata_end;
				IList<Block>old_blocks=ReadBlocks(ref metadata_start,out metadata_end,BlockMode.Blacklist,BlockType.XiphComment,BlockType.Picture);
				GetTag(TagTypes.Xiph,true);
				List<Block>new_blocks=new List<Block>();
				new_blocks.Add(old_blocks[0]);
				foreach(Block block in old_blocks)
				if(block.Type!=BlockType.StreamInfo&&block.Type!=BlockType.XiphComment&&block.Type!=BlockType.Picture&&block.Type!=BlockType.Padding)
				{
					new_blocks.Add(block);
				}
				new_blocks.Add(new Block(BlockType.XiphComment,(GetTag(TagTypes.Xiph,true)as Ogg.XiphComment).Render(false)));
				foreach(IPicture picture in metadata.Pictures)
				{
					if(picture==null)
					{
						continue;
					}
					new_blocks.Add(new Block(BlockType.Picture,new Picture(picture).Render()));
				}
				long length=0;
				foreach(Block block in new_blocks)
				length+=block.TotalSize;
				long padding_size=metadata_end-metadata_start-BlockHeader.Size-length;
				if(padding_size<0)
				{
					padding_size=1024*4;
				}
				if(padding_size!=0)
				{
					new_blocks.Add(new Block(BlockType.Padding,new ByteVector((int)padding_size)));
				}
				ByteVector block_data=new ByteVector();
				for(int i=0;i<new_blocks.Count;i++)
				{
					block_data.Add(new_blocks[i].Render(i==new_blocks.Count-1));
				}
				Insert(block_data,metadata_start,metadata_end-metadata_start);
				EndTag.Write();
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		public override TagLib.Tag GetTag(TagTypes type,bool create)
		{
			switch(type)
			{
			case TagTypes.Xiph:
				return metadata.GetComment(create,tag);
			case TagTypes.FlacMetadata:
				return metadata;
			}
			Tag t=(base.Tag as TagLib.NonContainer.Tag).GetTag(type);
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
		public override void RemoveTags(TagTypes types)
		{
			if((types&TagTypes.Xiph)!=0)
			{
				metadata.RemoveComment();
			}
			if((types&TagTypes.FlacMetadata)!=0)
			{
				metadata.Clear();
			}
			base.RemoveTags(types);
		}
#endregion
#region Protected Methods
		protected override void ReadStart(long start,ReadStyle propertiesStyle)
		{
			long end;
			IList<Block>blocks=ReadBlocks(ref start,out end,BlockMode.Whitelist,BlockType.StreamInfo,BlockType.XiphComment,BlockType.Picture);
			metadata=new Metadata(blocks);
			TagTypesOnDisk|=metadata.TagTypes;
			if(propertiesStyle!=ReadStyle.None)
			{
				if(blocks.Count==0||blocks[0].Type!=BlockType.StreamInfo)
				{
					throw new CorruptFileException("FLAC stream does not begin with StreamInfo.");
				}
				stream_start=end;
				header_block=blocks[0].Data;
			}
		}
		protected override void ReadEnd(long end,ReadStyle propertiesStyle)
		{
			tag=new CombinedTag(metadata,base.Tag);
			GetTag(TagTypes.Xiph,true);
		}
		protected override Properties ReadProperties(long start,long end,ReadStyle propertiesStyle)
		{
			StreamHeader header=new StreamHeader(header_block,end-stream_start);
			return new Properties(TimeSpan.Zero,header);
		}
#endregion
#region Private Methods
		private enum BlockMode
		{
			Blacklist,Whitelist
		}
		private IList<Block>ReadBlocks(ref long start,out long end,BlockMode mode,params BlockType[]types)
		{
			List<Block>blocks=new List<Block>();
			long start_position=Find("fLaC",start);
			if(start_position<0)
			{
				throw new CorruptFileException("FLAC stream not found at starting position.");
			}
			end=start=start_position+4;
			Seek(start);
			BlockHeader header;
			do
			{
				header=new BlockHeader(ReadBlock((int)BlockHeader.Size));
				bool found=false;
				foreach(BlockType type in types)
				if(header.BlockType==type)
				{
					found=true;
					break;
				}
				if((mode==BlockMode.Whitelist&&found)||(mode==BlockMode.Blacklist&& !found))
				{
					blocks.Add(new Block(header,ReadBlock((int)header.BlockSize)));
				}
				else
				{
					Seek(header.BlockSize,System.IO.SeekOrigin.Current);
				}
				end+=header.BlockSize+BlockHeader.Size;
			}
			while(!header.IsLastBlock);
			return blocks;
		}
#endregion
	}
	public class Metadata:
	CombinedTag
	{
		private List<IPicture>pictures=new List<IPicture>();
		[Obsolete("Use Metadata(IEnumerable<Block>)")]
		public Metadata(List<Block>blocks):this(blocks as IEnumerable<Block>)
		{
		}
		public Metadata(IEnumerable<Block>blocks)
		{
			if(blocks==null)
			{
				throw new ArgumentNullException("blocks");
			}
			foreach(Block block in blocks)
			{
				if(block.Data.Count==0)
				{
					continue;
				}
				if(block.Type==BlockType.XiphComment)
				{
					AddTag(new Ogg.XiphComment(block.Data));
				}
				else if(block.Type==BlockType.Picture)
				{
					pictures.Add(new Picture(block.Data));
				}
			}
		}
		public Ogg.XiphComment GetComment(bool create,Tag copy)
		{
			foreach(Tag t in Tags)
			if(t is Ogg.XiphComment)
			{
				return t as Ogg.XiphComment;
			}
			if(!create)
			{
				return null;
			}
			Ogg.XiphComment c=new Ogg.XiphComment();
			if(copy!=null)
			{
				copy.CopyTo(c,true);
			}
			AddTag(c);
			return c;
		}
		public void RemoveComment()
		{
			Ogg.XiphComment c;
			while((c=GetComment(false,null))!=null)RemoveTag(c);
		}
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.FlacMetadata|base.TagTypes;
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				return pictures.ToArray();
			}
			set
			{
				pictures.Clear();
				if(value!=null)
				{
					pictures.AddRange(value);
				}
			}
		}
		public override void Clear()
		{
			pictures.Clear();
		}
	}
}
