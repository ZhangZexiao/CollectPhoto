using System;
using System.IO;
namespace TagLib.IFD.Entries
{
	public class StripOffsetsIFDEntry:
	ArrayIFDEntry<uint>
	{
#region Private Fields
		private uint[]byte_counts;
		private File file;
#endregion
#region Constructors
		public StripOffsetsIFDEntry(ushort tag,uint[]values,uint[]byte_counts,File file):
		base(tag)
		{
			Values=values;
			this.byte_counts=byte_counts;
			this.file=file;
			if(values.Length!=byte_counts.Length)
			{
				throw new Exception("strip offsets and strip byte counts do not have the same length");
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			ByteVector data=new ByteVector();
			ByteVector offset_data=new ByteVector();
			uint data_offset=offset+(uint)(4*Values.Length);
			for(int i=0;i<Values.Length;i++)
			{
				uint new_offset=(uint)(data_offset+data.Count);
				file.Seek(Values[i],SeekOrigin.Begin);
				data.Add(file.ReadBlock((int)byte_counts[i]));
				Values[i]=new_offset;
				offset_data.Add(ByteVector.FromUInt(new_offset,is_bigendian));
			}
			if(Values.Length>1)
			{
				data.Insert(0,offset_data);
			}
			else
			{
				Values[0]=offset;
			}
			while(data.Count<4)data.Add(0x00);
			type=(ushort)IFDEntryType.Long;
			count=(uint)Values.Length;
			return data;
		}
#endregion
	}
}
