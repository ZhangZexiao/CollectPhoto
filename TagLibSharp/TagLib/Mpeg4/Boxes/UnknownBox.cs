using System;
namespace TagLib.Mpeg4
{
	public class UnknownBox:
	Box
	{
#region Private Fields
		private ByteVector data;
#endregion
#region Constructors
		public UnknownBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,handler)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			this.data=LoadData(file);
		}
#endregion
#region Public Properties
		public override ByteVector Data
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
	}
}
