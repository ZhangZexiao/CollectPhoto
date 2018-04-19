namespace TagLib.IFD
{
	public interface IFDEntry
	{
#region Properties
		ushort Tag
		{
			get;
		}
#endregion
#region Methods
		ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count);
#endregion
	}
	public abstract class ArrayIFDEntry<T> :
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public T[]Values
		{
			get;
			protected set;
		}
#endregion
#region Constructors
		public ArrayIFDEntry(ushort tag)
		{
			Tag=tag;
		}
#endregion
#region Public Methods
		public abstract ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count);
#endregion
	}
}
