using System;
namespace TagLib.Asf
{
	public enum DataType
	{
		Unicode=0,Bytes=1,Bool=2,DWord=3,QWord=4,Word=5,Guid=6
	}
	public class ContentDescriptor
	{
#region Private Fields
		private DataType type=DataType.Unicode;
		private string name=null;
		private string strValue=null;
		private ByteVector byteValue=null;
		private ulong longValue=0;
#endregion
#region Constructors
		public ContentDescriptor(string name,string value)
		{
			this.name=name;
			this.strValue=value;
		}
		public ContentDescriptor(string name,ByteVector value)
		{
			this.name=name;
			this.type=DataType.Bytes;
			this.byteValue=new ByteVector(value);
		}
		public ContentDescriptor(string name,uint value)
		{
			this.name=name;
			this.type=DataType.DWord;
			this.longValue=value;
		}
		public ContentDescriptor(string name,ulong value)
		{
			this.name=name;
			this.type=DataType.QWord;
			this.longValue=value;
		}
		public ContentDescriptor(string name,ushort value)
		{
			this.name=name;
			this.type=DataType.Word;
			this.longValue=value;
		}
		public ContentDescriptor(string name,bool value)
		{
			this.name=name;
			this.type=DataType.Bool;
			this.longValue=value?1uL:0;
		}
		protected internal ContentDescriptor(Asf.File file)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			if(!Parse(file))
			{
				throw new CorruptFileException("Failed to parse content descriptor.");
			}
		}
#endregion
#region Public Properties
		public string Name
		{
			get
			{
				return name;
			}
		}
		public DataType Type
		{
			get
			{
				return type;
			}
		}
#endregion
#region Public Methods
		public override string ToString()
		{
			if(type==DataType.Unicode)
			{
				return strValue;
			}
			if(type==DataType.Bytes)
			{
				return byteValue.ToString(StringType.UTF16LE);
			}
			return longValue.ToString();
		}
		public ByteVector ToByteVector()
		{
			return byteValue;
		}
		public bool ToBool()
		{
			return longValue!=0;
		}
		public uint ToDWord()
		{
			uint value;
			if(type==DataType.Unicode&&strValue!=null&&uint.TryParse(strValue,out value))
			{
				return value;
			}
			return(uint)longValue;
		}
		public ulong ToQWord()
		{
			ulong value;
			if(type==DataType.Unicode&&strValue!=null&&ulong.TryParse(strValue,out value))
			{
				return value;
			}
			return longValue;
		}
		public ushort ToWord()
		{
			ushort value;
			if(type==DataType.Unicode&&strValue!=null&&ushort.TryParse(strValue,out value))
			{
				return value;
			}
			return(ushort)longValue;
		}
		public ByteVector Render()
		{
			ByteVector value=null;
			switch(type)
			{
			case DataType.Unicode:
				value=Object.RenderUnicode(strValue);
				break;
			case DataType.Bytes:
				value=byteValue;
				break;
			case DataType.Bool:
			case DataType.DWord:
				value=Object.RenderDWord((uint)longValue);
				break;
			case DataType.QWord:
				value=Object.RenderQWord(longValue);
				break;
			case DataType.Word:
				value=Object.RenderWord((ushort)longValue);
				break;
			default:
				return null;
			}
			ByteVector name=Object.RenderUnicode(this.name);
			ByteVector output=new ByteVector();
			output.Add(Object.RenderWord((ushort)name.Count));
			output.Add(name);
			output.Add(Object.RenderWord((ushort)type));
			output.Add(Object.RenderWord((ushort)value.Count));
			output.Add(value);
			return output;
		}
#endregion
#region Protected Methods
		protected bool Parse(Asf.File file)
		{
			int name_count=file.ReadWord();
			name=file.ReadUnicode(name_count);
			type=(DataType)file.ReadWord();
			int value_count=file.ReadWord();
			switch(type)
			{
			case DataType.Word:
				longValue=file.ReadWord();
				break;
			case DataType.Bool:
				longValue=file.ReadDWord();
				break;
			case DataType.DWord:
				longValue=file.ReadDWord();
				break;
			case DataType.QWord:
				longValue=file.ReadQWord();
				break;
			case DataType.Unicode:
				strValue=file.ReadUnicode(value_count);
				break;
			case DataType.Bytes:
				byteValue=file.ReadBlock(value_count);
				break;
			default:
				return false;
			}
			return true;
		}
#endregion
	}
}
