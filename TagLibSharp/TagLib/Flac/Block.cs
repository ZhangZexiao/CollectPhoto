using System;
using System.Collections.Generic;
namespace TagLib.Flac
{
	public class Block
	{
#region Private Fields
		private BlockHeader header;
		private ByteVector data;
#endregion
#region Constructors
		public Block(BlockHeader header,ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(header.BlockSize!=data.Count)
			{
				throw new CorruptFileException("Data count not equal to block size.");
			}
			this.header=header;
			this.data=data;
		}
		public Block(BlockType type,ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			header=new BlockHeader(type,(uint)data.Count);
			this.data=data;
		}
#endregion
#region Public Properties
		public BlockType Type
		{
			get
			{
				return header.BlockType;
			}
		}
		public bool IsLastBlock
		{
			get
			{
				return header.IsLastBlock;
			}
		}
		public uint DataSize
		{
			get
			{
				return header.BlockSize;
			}
		}
		public uint TotalSize
		{
			get
			{
				return DataSize+BlockHeader.Size;
			}
		}
		public ByteVector Data
		{
			get
			{
				return data;
			}
		}
#endregion
#region Public Methods
		public ByteVector Render(bool isLastBlock)
		{
			if(this.data==null)
			{
				throw new InvalidOperationException("Cannot render empty blocks.");
			}
			ByteVector data=header.Render(isLastBlock);
			data.Add(this.data);
			return data;
		}
#endregion
	}
}
