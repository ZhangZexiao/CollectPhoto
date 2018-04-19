namespace TagLib.IFD.Entries
{
	public class RationalIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public Rational Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public RationalIFDEntry(ushort tag,Rational value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Rational;
			count=1;
			ByteVector data=new ByteVector();
			data.Add(ByteVector.FromUInt(Value.Numerator,is_bigendian));
			data.Add(ByteVector.FromUInt(Value.Denominator,is_bigendian));
			return data;
		}
#endregion
	}
}
