using System;
namespace TagLib.Asf
{
	public class UnknownObject:
	Object
	{
#region Private Fields
		private ByteVector data;
#endregion
#region Constructors
		public UnknownObject(Asf.File file,long position):
		base(file,position)
		{
			data=file.ReadBlock((int)(OriginalSize-24));
		}
#endregion
#region Public Properties
		public ByteVector Data
		{
			get
			{
				return data;
			}
			set
			{
				data=value;
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render()
		{
			return Render(data);
		}
#endregion
	}
}
