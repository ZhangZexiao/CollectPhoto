using System.Collections;
using System;
namespace TagLib.Id3v1
{
	public class StringHandler
	{
		public virtual string Parse(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			string output=data.ToString(StringType.Latin1).Trim();
			int i=output.IndexOf('\0');
			return(i>=0)?output.Substring(0,i):output;
		}
		public virtual ByteVector Render(string text)
		{
			return ByteVector.FromString(text,StringType.Latin1);
		}
	}
}
