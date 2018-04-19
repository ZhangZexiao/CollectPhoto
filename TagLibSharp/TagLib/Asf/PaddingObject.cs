using System;
namespace TagLib.Asf
{
	public class PaddingObject:
	Object
	{
#region Private Fields
		private ulong size;
#endregion
#region Constructors
		public PaddingObject(Asf.File file,long position):
		base(file,position)
		{
			if(!Guid.Equals(Asf.Guid.AsfPaddingObject))
			{
				throw new CorruptFileException("Object GUID incorrect.");
			}
			if(OriginalSize<24)
			{
				throw new CorruptFileException("Object size too small.");
			}
			size=OriginalSize;
		}
		public PaddingObject(uint size):
		base(Asf.Guid.AsfPaddingObject)
		{
			this.size=size;
		}
#endregion
#region Prublic Properties
		public ulong Size
		{
			get
			{
				return size;
			}
			set
			{
				size=value;
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render()
		{
			return Render(new ByteVector((int)(size-24)));
		}
#endregion
	}
}
