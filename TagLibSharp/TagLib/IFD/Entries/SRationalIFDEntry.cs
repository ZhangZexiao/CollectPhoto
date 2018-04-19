namespace TagLib.IFD.Entries
{
	public class SRationalIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public SRational Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public SRationalIFDEntry(ushort tag,SRational value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.SRational;
			count=1;
			ByteVector data=new ByteVector();
			data.Add(ByteVector.FromInt(Value.Numerator,is_bigendian));
			data.Add(ByteVector.FromInt(Value.Denominator,is_bigendian));
			return data;
		}
#endregion
	}
}
