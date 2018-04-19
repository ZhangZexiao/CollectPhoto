using System;
using System.Collections.Generic;
namespace TagLib.Image
{
	public abstract class ImageBlockFile:
	TagLib.Image.File
	{
		private class MetadataBlock
		{
			public long Start
			{
				get;
				set;
			}
			public long Length
			{
				get;
				set;
			}
			public MetadataBlock(long start,long length)
			{
				if(start<0)
				{
					throw new ArgumentOutOfRangeException("start");
				}
				if(length<0)
				{
					throw new ArgumentOutOfRangeException("length");
				}
				Start=start;
				Length=length;
			}
			public MetadataBlock():
			this(0,0)
			{
			}
			public bool OverlapsWith(MetadataBlock block)
			{
				if(block.Start>=Start&&block.Start<=Start+Length)
				{
					return true;
				}
				if(Start>=block.Start&&Start<=block.Start+block.Length)
				{
					return true;
				}
				return false;
			}
			public void Add(MetadataBlock block)
			{
				if(block.Start>=Start&&block.Start<=Start+Length)
				{
					Length=Math.Max(Length,block.Start+block.Length-Start);
					return;
				}
				if(Start>=block.Start&&Start<=block.Start+block.Length)
				{
					Length=Math.Max(block.Length,Start+Length-block.Start);
					Start=block.Start;
					return;
				}
				throw new ArgumentException(String.Format("blocks do not overlap: {0} and {1}",this,block));
			}
			public bool Before(MetadataBlock block)
			{
				return(Start+Length<block.Start);
			}
			public override string ToString()
			{
				return string.Format("[MetadataBlock: Start={0}, Length={1}]",Start,Length);
			}
		}
		private List<MetadataBlock>metadata_blocks=new List<MetadataBlock>();
		protected void AddMetadataBlock(long start,long length)
		{
			MetadataBlock new_block=new MetadataBlock(start,length);
			for(int i=0;i<metadata_blocks.Count;i++)
			{
				var block=metadata_blocks[i];
				if(new_block.OverlapsWith(block))
				{
					block.Add(new_block);
					i++;
					while(i<metadata_blocks.Count)
					{
						var next_block=metadata_blocks[i];
						if(block.OverlapsWith(next_block))
						{
							block.Add(next_block);
							metadata_blocks.Remove(next_block);
						}
						else
						{
							return;
						}
					}
					return;
				}
				else if(new_block.Before(block))
				{
					metadata_blocks.Insert(i,new_block);
					return;
				}
			}
			metadata_blocks.Add(new_block);
		}
		protected void SaveMetadata(ByteVector data,long start)
		{
			long new_start=0;
			AddMetadataBlock(start,0);
			for(int i=metadata_blocks.Count-1;i>=0;i--)
			{
				var block=metadata_blocks[i];
				if(block.Start<=start&&block.Start+block.Length>=start)
				{
					Insert(data,block.Start,block.Length);
					new_start=block.Start;
				}
				else
				{
					Insert("",block.Start,block.Length);
					if(block.Start<start)
					{
						new_start-=block.Length;
					}
				}
			}
			metadata_blocks.Clear();
			AddMetadataBlock(new_start,data.Count);
		}
#region Constructors
		protected ImageBlockFile(string path):
		base(path)
		{
		}
		protected ImageBlockFile(IFileAbstraction abstraction):
		base(abstraction)
		{
		}
#endregion
	}
}
