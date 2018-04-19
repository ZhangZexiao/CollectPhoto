namespace TagLib.IFD.Entries
{
	public class SRationalArrayIFDEntry:
	ArrayIFDEntry<SRational>
	{
#region Constructors
		public SRationalArrayIFDEntry(ushort tag,SRational[]entries):
		base(tag)
		{
			Values=entries;
		}
#endregion
#region Public Methods
		public override ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.SRational;
			count=(uint)Values.Length;
			ByteVector data=new ByteVector();
			foreach(SRational rational in Values)
			{
				data.Add(ByteVector.FromInt(rational.Numerator,is_bigendian));
				data.Add(ByteVector.FromInt(rational.Denominator,is_bigendian));
			}
			return data;
		}
#endregion
	}
}
