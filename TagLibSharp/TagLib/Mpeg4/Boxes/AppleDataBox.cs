using System;
namespace TagLib.Mpeg4
{
	public class AppleDataBox:
	FullBox
	{
#region Enums
		public enum FlagType
		{
			ContainsText=0x01,ContainsData=0x00,ForTempo=0x15,ContainsJpegData=0x0D,ContainsPngData=0x0E
		}
#endregion
#region Private Fields
		private ByteVector data;
#endregion
#region Constructors
		public AppleDataBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,file,handler)
		{
			Data=LoadData(file);
		}
		public AppleDataBox(ByteVector data,uint flags):
		base("data",0,flags)
		{
			Data=data;
		}
#endregion
#region Public Properties
		protected override long DataPosition
		{
			get
			{
				return base.DataPosition+4;
			}
		}
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
				return((Flags&(int)FlagType.ContainsText)!=0)?Data.ToString(StringType.UTF8):null;
			}
			set
			{
				Flags=(int)FlagType.ContainsText;
				Data=ByteVector.FromString(value,StringType.UTF8);
			}
		}
#endregion
#region Protected Methods
		protected override ByteVector Render(ByteVector topData)
		{
			ByteVector output=new ByteVector(4);
			output.Add(topData);
			return base.Render(output);
		}
#endregion
	}
}
