namespace TagLib.IFD.Entries
{
	public class LongArrayIFDEntry:
	ArrayIFDEntry<uint>
	{
#region Constructors
		public LongArrayIFDEntry(ushort tag,uint[]values):
		base(tag)
		{
			Values=values;
		}
#endregion
#region Public Methods
		public override ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Long;
			count=(uint)Values.Length;
			ByteVector data=new ByteVector();
			foreach(uint value in Values)
			data.Add(ByteVector.FromUInt(value,is_bigendian));
			return data;
		}
#endregion
	}
}
