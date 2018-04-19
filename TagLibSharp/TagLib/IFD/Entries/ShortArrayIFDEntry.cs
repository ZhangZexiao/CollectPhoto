namespace TagLib.IFD.Entries
{
	public class ShortArrayIFDEntry:
	ArrayIFDEntry<ushort>
	{
#region Constructors
		public ShortArrayIFDEntry(ushort tag,ushort[]values):
		base(tag)
		{
			Values=values;
		}
#endregion
#region Public Methods
		public override ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Short;
			count=(uint)Values.Length;
			ByteVector data=new ByteVector();
			foreach(ushort value in Values)
			data.Add(ByteVector.FromUShort(value,is_bigendian));
			return data;
		}
#endregion
	}
}
