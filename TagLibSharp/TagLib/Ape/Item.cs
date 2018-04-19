using System;
namespace TagLib.Ape
{
	public enum ItemType
	{
		Text=0,Binary=1,Locator=2
	}
	public class Item:
	ICloneable
	{
#region Private Fields
		private ItemType type=ItemType.Text;
		private string key=null;
		private ReadOnlyByteVector data=null;
		private string[]text=null;
		private bool read_only=false;
		private int size_on_disk;
#endregion
#region Constructors
		public Item(ByteVector data,int offset)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			Parse(data,offset);
		}
		public Item(string key,string value)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(value==null)
			{
				throw new ArgumentNullException("value");
			}
			this.key=key;
			this.text=new string[]
			{
				value
			};
		}
		public Item(string key,params string[]value)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(value==null)
			{
				throw new ArgumentNullException("value");
			}
			this.key=key;
			this.text=(string[])value.Clone();
		}
		[Obsolete("Use Item(string,string[])")]
		public Item(string key,StringCollection value)
		{
			if(key==null)
			{
				throw new ArgumentNullException("key");
			}
			if(value==null)
			{
				throw new ArgumentNullException("value");
			}
			this.key=key;
			this.text=value.ToArray();
		}
		public Item(string key,ByteVector value)
		{
			this.key=key;
			this.type=ItemType.Binary;
			data=value as ReadOnlyByteVector;
			if(data==null)
			{
				data=new ReadOnlyByteVector(value);
			}
		}
		private Item(Item item)
		{
			type=item.type;
			key=item.key;
			if(item.data!=null)
			{
				data=new ReadOnlyByteVector(item.data);
			}
			if(item.text!=null)
			{
				text=(string[])item.text.Clone();
			}
			read_only=item.read_only;
			size_on_disk=item.size_on_disk;
		}
#endregion
#region Public Properties
		public string Key
		{
			get
			{
				return key;
			}
		}
		public ByteVector Value
		{
			get
			{
				return(type==ItemType.Binary)?data:null;
			}
		}
		public int Size
		{
			get
			{
				return size_on_disk;
			}
		}
		public ItemType Type
		{
			get
			{
				return type;
			}
			set
			{
				type=value;
			}
		}
		public bool ReadOnly
		{
			get
			{
				return read_only;
			}
			set
			{
				read_only=value;
			}
		}
		public bool IsEmpty
		{
			get
			{
				if(type!=ItemType.Binary)
				{
					return text==null||text.Length==0;
				}
				else
				{
					return data==null||data.IsEmpty;
				}
			}
		}
#endregion
#region Public Methods
		public override string ToString()
		{
			if(type==ItemType.Binary||text==null)
			{
				return null;
			}
			else
			{
				return string.Join(", ",text);
			}
		}
		public string[]ToStringArray()
		{
			if(type==ItemType.Binary||text==null)
			{
				return new string[0];
			}
			return text;
		}
		public ByteVector Render()
		{
			uint flags=(uint)((ReadOnly)?1:0)|((uint)Type<<1);
			if(IsEmpty)
			{
				return new ByteVector();
			}
			ByteVector result=null;
			if(type==ItemType.Binary)
			{
				if(text==null&&data!=null)
				{
					result=data;
				}
			}
			if(result==null&&text!=null)
			{
				result=new ByteVector();
				for(int i=0;i<text.Length;i++)
				{
					if(i!=0)
					{
						result.Add((byte)0);
					}
					result.Add(ByteVector.FromString(text[i],StringType.UTF8));
				}
			}
			if(result==null||result.Count==0)
			{
				return new ByteVector();
			}
			ByteVector output=new ByteVector();
			output.Add(ByteVector.FromUInt((uint)result.Count,false));
			output.Add(ByteVector.FromUInt(flags,false));
			output.Add(ByteVector.FromString(key,StringType.UTF8));
			output.Add((byte)0);
			output.Add(result);
			size_on_disk=output.Count;
			return output;
		}
#endregion
#region Protected Methods
		protected void Parse(ByteVector data,int offset)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(offset<0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if(data.Count<offset+11)
			{
				throw new CorruptFileException("Not enough data for APE Item");
			}
			uint value_length=data.Mid(offset,4).ToUInt(false);
			uint flags=data.Mid(offset+4,4).ToUInt(false);
			ReadOnly=(flags&1)==1;
			Type=(ItemType)((flags>>1)&3);
			int pos=data.Find(ByteVector.TextDelimiter(StringType.UTF8),offset+8);
			key=data.ToString(StringType.UTF8,offset+8,pos-offset-8);
			if(value_length>data.Count-pos-1)
			{
				throw new CorruptFileException("Invalid data length.");
			}
			size_on_disk=pos+1+(int)value_length-offset;
			if(Type==ItemType.Binary)
			{
				this.data=new ReadOnlyByteVector(data.Mid(pos+1,(int)value_length));
			}
			else
			{
				this.text=data.Mid(pos+1,(int)value_length).ToStrings(StringType.UTF8,0);
			}
		}
#endregion
#region ICloneable
		public Item Clone()
		{
			return new Item(this);
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
#endregion
	}
}
