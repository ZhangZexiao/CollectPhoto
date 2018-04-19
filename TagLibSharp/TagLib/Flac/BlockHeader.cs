using System.Collections.Generic;
using System;
namespace TagLib.Flac
{
	public enum BlockType
	{
		StreamInfo=0,Padding,Application,SeekTable,XiphComment,CueSheet,Picture
	}
	public struct BlockHeader
	{
		private BlockType block_type;
		private bool is_last_block;
		private uint block_size;
		public const uint Size=4;
		public BlockHeader(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<Size)
			{
				throw new CorruptFileException("Not enough data in Flac header.");
			}
			block_type=(BlockType)(data[0]&0x7f);
			is_last_block=(data[0]&0x80)!=0;
			block_size=data.Mid(1,3).ToUInt();
		}
		public BlockHeader(BlockType type,uint blockSize)
		{
			block_type=type;
			is_last_block=false;
			block_size=blockSize;
		}
		public ByteVector Render(bool isLastBlock)
		{
			ByteVector data=ByteVector.FromUInt(block_size);
			data[0]=(byte)(block_type+(isLastBlock?0x80:0));
			return data;
		}
		public BlockType BlockType
		{
			get
			{
				return block_type;
			}
		}
		public bool IsLastBlock
		{
			get
			{
				return is_last_block;
			}
		}
		public uint BlockSize
		{
			get
			{
				return block_size;
			}
		}
	}
}
