namespace TagLib.IFD.Entries
{
	public class RationalArrayIFDEntry:
	ArrayIFDEntry<Rational>
	{
#region Constructors
		public RationalArrayIFDEntry(ushort tag,Rational[]entries):
		base(tag)
		{
			Values=entries;
		}
#endregion
#region Public Methods
		public override ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Rational;
			count=(uint)Values.Length;
			ByteVector data=new ByteVector();
			foreach(Rational rational in Values)
			{
				data.Add(ByteVector.FromUInt(rational.Numerator,is_bigendian));
				data.Add(ByteVector.FromUInt(rational.Denominator,is_bigendian));
			}
			return data;
		}
#endregion
	}
}
