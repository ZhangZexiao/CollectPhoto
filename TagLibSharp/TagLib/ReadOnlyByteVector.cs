namespace TagLib
{
	public sealed class ReadOnlyByteVector:
	ByteVector
	{
#region Constructors
		public ReadOnlyByteVector():
		base()
		{
		}
		public ReadOnlyByteVector(int size,byte value):
		base(size,value)
		{
		}
		public ReadOnlyByteVector(int size):
		this(size,0)
		{
		}
		public ReadOnlyByteVector(ByteVector vector):
		base(vector)
		{
		}
		public ReadOnlyByteVector(byte[]data,int length):
		base(data,length)
		{
		}
		public ReadOnlyByteVector(params byte[]data):
		base(data)
		{
		}
#endregion
#region Operators
		public static implicit operator ReadOnlyByteVector(byte value)
		{
			return new ReadOnlyByteVector(value);
		}
		public static implicit operator ReadOnlyByteVector(byte[]value)
		{
			return new ReadOnlyByteVector(value);
		}
		public static implicit operator ReadOnlyByteVector(string value)
		{
			return new ReadOnlyByteVector(ByteVector.FromString(value,StringType.UTF8));
		}
#endregion
#region IList<T>
		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}
		public override bool IsFixedSize
		{
			get
			{
				return true;
			}
		}
#endregion
	}
}
