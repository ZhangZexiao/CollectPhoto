using System;
namespace TagLib.IIM
{
	public class IIMReader
	{
		private static readonly byte[]IPTC_IIM_SEGMENT=new byte[]
		{
			0x1C,0x02
		};
		private IIMTag Tag
		{
			get;
			set;
		}
		private ByteVector Data
		{
			get;
			set;
		}
		public IIMReader(ByteVector data)
		{
			Data=data;
			Tag=new IIMTag();
		}
		public IIMTag Process()
		{
			int findOffset=0;
			int count=0;
			for(int i=Data.Find(IPTC_IIM_SEGMENT,findOffset);i>=findOffset;i=Data.Find(IPTC_IIM_SEGMENT,findOffset))
			{
				count++;
				i+=IPTC_IIM_SEGMENT.Length;
				int len=Data.Mid(i+1).ToUShort();
				switch(Data[i])
				{
				case 5:
					Tag.Title=Data.ToString(StringType.Latin1,i+3,len);
					break;
				case 25:
					Tag.AddKeyword(Data.ToString(StringType.Latin1,i+3,len));
					break;
				case 80:
					Tag.Creator=Data.ToString(StringType.Latin1,i+3,len);
					break;
				case 116:
					Tag.Copyright=Data.ToString(StringType.Latin1,i+3,len);
					break;
				case 120:
					Tag.Comment=Data.ToString(StringType.Latin1,i+3,len);
					break;
				}
				findOffset=i+3+len;
			}
			if(count==0)
			{
				return null;
			}
			return Tag;
		}
	}
}
