namespace TagLib.IFD.Entries
{
	public class SShortArrayIFDEntry:
	ArrayIFDEntry<short>
	{
#region Constructors
		public SShortArrayIFDEntry(ushort tag,short[]values):
		base(tag)
		{
			Values=values;
		}
#endregion
#region Public Methods
		public override ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.SShort;
			count=(uint)Values.Length;
			ByteVector data=new ByteVector();
			foreach(var value in Values)
			data.Add(ByteVector.FromShort(value,is_bigendian));
			return data;
		}
#endregion
	}
}
