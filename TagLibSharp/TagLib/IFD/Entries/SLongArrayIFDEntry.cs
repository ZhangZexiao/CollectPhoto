namespace TagLib.IFD.Entries
{
	public class SLongArrayIFDEntry:
	ArrayIFDEntry<int>
	{
#region Constructors
		public SLongArrayIFDEntry(ushort tag,int[]values):
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
			foreach(int value in Values)
			data.Add(ByteVector.FromInt(value,is_bigendian));
			return data;
		}
#endregion
	}
}
