using System;
namespace TagLib.Ape
{
#region Enums
	[Flags]
	public enum FooterFlags:uint
	{
		FooterAbsent=0x40000000,IsHeader=0x20000000,HeaderPresent=0x80000000
	}
#endregion
	public struct Footer:
	IEquatable<Footer>
	{
#region Private Properties
		private uint version;
		private FooterFlags flags;
		private uint item_count;
		private uint tag_size;
#endregion
#region Public Static Fields
		public const uint Size=32;
		public static readonly ReadOnlyByteVector FileIdentifier="APETAGEX";
#endregion
#region Constructors
		public Footer(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<Size)
			{
				throw new CorruptFileException("Provided data is smaller than object size.");
			}
			if(!data.StartsWith(FileIdentifier))
			{
				throw new CorruptFileException("Provided data does not start with File Identifier");
			}
			version=data.Mid(8,4).ToUInt(false);
			tag_size=data.Mid(12,4).ToUInt(false);
			item_count=data.Mid(16,4).ToUInt(false);
			flags=(FooterFlags)data.Mid(20,4).ToUInt(false);
		}
#endregion
#region Public Properties
		public uint Version
		{
			get
			{
				return version==0?2000:version;
			}
		}
		public FooterFlags Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags=value;
			}
		}
		public uint ItemCount
		{
			get
			{
				return item_count;
			}
			set
			{
				item_count=value;
			}
		}
		public uint TagSize
		{
			get
			{
				return tag_size;
			}
			set
			{
				tag_size=value;
			}
		}
		public uint CompleteTagSize
		{
			get
			{
				return TagSize+((Flags&FooterFlags.HeaderPresent)!=0?Size:0);
			}
		}
#endregion
#region Public Methods
		public ByteVector RenderFooter()
		{
			return Render(false);
		}
		public ByteVector RenderHeader()
		{
			return(Flags&FooterFlags.HeaderPresent)!=0?Render(true):new ByteVector();
		}
#endregion
#region Private Methods
		private ByteVector Render(bool isHeader)
		{
			ByteVector v=new ByteVector();
			v.Add(FileIdentifier);
			v.Add(ByteVector.FromUInt(2000,false));
			v.Add(ByteVector.FromUInt(tag_size,false));
			v.Add(ByteVector.FromUInt(item_count,false));
			uint flags=0;
			if((Flags&FooterFlags.HeaderPresent)!=0)
			{
				flags|=(uint)FooterFlags.HeaderPresent;
			}
			if(isHeader)
			{
				flags|=(uint)FooterFlags.IsHeader;
			}
			else
			{
				flags&=(uint)~FooterFlags.IsHeader;
			}
			v.Add(ByteVector.FromUInt(flags,false));
			v.Add(ByteVector.FromULong(0));
			return v;
		}
#endregion
#region IEquatable
		public override int GetHashCode()
		{
			unchecked
			{
				return(int)((uint)flags^tag_size^item_count^version);
			}
		}
		public override bool Equals(object other)
		{
			if(!(other is Footer))
			{
				return false;
			}
			return Equals((Footer)other);
		}
		public bool Equals(Footer other)
		{
			return flags==other.flags&&tag_size==other.tag_size&&item_count==other.item_count&&version==other.version;
		}
		public static bool operator==(Footer first,Footer second)
		{
			return first.Equals(second);
		}
		public static bool operator!=(Footer first,Footer second)
		{
			return!first.Equals(second);
		}
#endregion
	}
}
