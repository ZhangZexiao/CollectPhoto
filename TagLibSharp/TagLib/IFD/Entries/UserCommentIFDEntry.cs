using System;
namespace TagLib.IFD.Entries
{
	public class UserCommentIFDEntry:
	IFDEntry
	{
#region Constant Values
		public static readonly ByteVector COMMENT_ASCII_CODE=new byte[]
		{
			0x41,0x53,0x43,0x49,0x49,0x00,0x00,0x00
		};
		public static readonly ByteVector COMMENT_JIS_CODE=new byte[]
		{
			0x4A,0x49,0x53,0x00,0x00,0x00,0x00,0x00
		};
		public static readonly ByteVector COMMENT_UNICODE_CODE=new byte[]
		{
			0x55,0x4E,0x49,0x43,0x4F,0x44,0x45,0x00
		};
		public static readonly ByteVector COMMENT_BAD_UNICODE_CODE=new byte[]
		{
			0x55,0x6E,0x69,0x63,0x6F,0x64,0x65,0x00
		};
		public static readonly ByteVector COMMENT_UNDEFINED_CODE=new byte[]
		{
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
		};
#endregion
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public string Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public UserCommentIFDEntry(ushort tag,string value)
		{
			Tag=tag;
			Value=value;
		}
		public UserCommentIFDEntry(ushort tag,ByteVector data,TagLib.File file)
		{
			Tag=tag;
			if(data.StartsWith(COMMENT_ASCII_CODE))
			{
				Value=TrimNull(data.ToString(StringType.Latin1,COMMENT_ASCII_CODE.Count,data.Count-COMMENT_ASCII_CODE.Count));
				return;
			}
			if(data.StartsWith(COMMENT_UNICODE_CODE))
			{
				Value=TrimNull(data.ToString(StringType.UTF8,COMMENT_UNICODE_CODE.Count,data.Count-COMMENT_UNICODE_CODE.Count));
				return;
			}
			var trimmed=data.ToString().Trim();
			if(trimmed.Length==0||trimmed=="\0")
			{
				Value=String.Empty;
				return;
			}
			if(data.StartsWith((byte)0x00)&&data.Count>=8)
			{
				int term=data.Find("\0",8);
				if(term!= -1)
				{
					Value=data.ToString(StringType.Latin1,8,term-8);
				}
				else
				{
					Value=data.ToString(StringType.Latin1,8,data.Count-8);
				}
				return;
			}
			if(data.Data.Length==0)
			{
				Value=String.Empty;
				return;
			}
			int offset=0;
			int length=data.Count-offset;
			if(data.StartsWith(COMMENT_BAD_UNICODE_CODE))
			{
				offset=COMMENT_BAD_UNICODE_CODE.Count;
				length=data.Count-offset;
			}
			file.MarkAsCorrupt("UserComment with other encoding than Latin1 or Unicode");
			Value=TrimNull(data.ToString(StringType.UTF8,offset,length));
		}
		private string TrimNull(string value)
		{
			int term=value.IndexOf('\0');
			if(term> -1)
			{
				value=value.Substring(0,term);
			}
			return value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Undefined;
			ByteVector data=new ByteVector();
			data.Add(COMMENT_UNICODE_CODE);
			data.Add(ByteVector.FromString(Value,StringType.UTF8));
			count=(uint)data.Count;
			return data;
		}
#endregion
	}
}
