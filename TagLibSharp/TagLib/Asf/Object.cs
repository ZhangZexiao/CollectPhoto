using System;
namespace TagLib.Asf
{
	public abstract class Object
	{
#region Private Fields
		private System.Guid id;
		private ulong size;
#endregion
#region Constructors
		protected Object(Asf.File file,long position)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			if(position<0||position>file.Length-24)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			file.Seek(position);
			id=file.ReadGuid();
			size=file.ReadQWord();
		}
		protected Object(System.Guid guid)
		{
			id=guid;
		}
#endregion
#region Public Properties
		public System.Guid Guid
		{
			get
			{
				return id;
			}
		}
		public ulong OriginalSize
		{
			get
			{
				return size;
			}
		}
#endregion
#region Public Methods
		public abstract ByteVector Render();
#endregion
#region Public Static Methods
		public static ByteVector RenderUnicode(string value)
		{
			ByteVector v=ByteVector.FromString(value,StringType.UTF16LE);
			v.Add(RenderWord(0));
			return v;
		}
		public static ByteVector RenderDWord(uint value)
		{
			return ByteVector.FromUInt(value,false);
		}
		public static ByteVector RenderQWord(ulong value)
		{
			return ByteVector.FromULong(value,false);
		}
		public static ByteVector RenderWord(ushort value)
		{
			return ByteVector.FromUShort(value,false);
		}
#endregion
#region Protected Methods
		protected ByteVector Render(ByteVector data)
		{
			ulong length=(ulong)((data!=null?data.Count:0)+24);
			ByteVector v=id.ToByteArray();
			v.Add(RenderQWord(length));
			v.Add(data);
			return v;
		}
#endregion
	}
}
