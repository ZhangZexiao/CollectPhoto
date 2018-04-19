using System;
namespace TagLib.Mpeg4
{
	public class AppleAdditionalInfoBox:
	Box
	{
#region Private Fields
		private ByteVector data;
#endregion
#region Constructors
		public AppleAdditionalInfoBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,handler)
		{
			Data=LoadData(file);
		}
		public AppleAdditionalInfoBox(ByteVector header):
		base(header)
		{
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
				data=value!=null?value:new ByteVector();
			}
		}
		public string Text
		{
			get
			{
				return Data.ToString(StringType.Latin1).TrimStart('\0');
			}
			set
			{
				Data=ByteVector.FromString(value,StringType.Latin1);
			}
		}
#endregion
	}
}
