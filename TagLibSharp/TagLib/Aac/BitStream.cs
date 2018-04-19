using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
namespace TagLib.Aac
{
	public class BitStream
	{
#region Private Fields
		private BitArray bits;
		private int bitindex;
#endregion
#region Constructors
		public BitStream(byte[]buffer)
		{
			Debug.Assert(buffer.Length==7,"buffer.Length == 7","buffer size invalid");
			if(buffer.Length!=7)
			{
				throw new ArgumentException("Buffer size must be 7 bytes");
			}
			bits=new BitArray(buffer.Length*8);
			for(int i=0;i<buffer.Length;i++)
			{
				for(int y=0;y<8;y++)
				{
					bits[i*8+y]=((buffer[i]&(1<<(7-y)))>0);
				}
			}
			bitindex=0;
		}
#endregion
#region Public Methods
		public int ReadInt32(int numberOfBits)
		{
			Debug.Assert(numberOfBits>0,"numberOfBits < 1");
			Debug.Assert(numberOfBits<=32,"numberOfBits <= 32");
			if(numberOfBits<=0)
			{
				throw new ArgumentException("Number of bits to read must be >= 1");
			}
			if(numberOfBits>32)
			{
				throw new ArgumentException("Number of bits to read must be <= 32");
			}
			int value=0;
			int start=bitindex+numberOfBits-1;
			for(int i=0;i<numberOfBits;i++)
			{
				value+=bits[start]?(1<<i):0;
				bitindex++;
				start--;
			}
			return value;
		}
#endregion
	}
}
