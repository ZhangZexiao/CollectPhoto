using System;
namespace TagLib.Id3v2
{
	public struct Footer
	{
#region Private Fields
		private byte major_version;
		private byte revision_number;
		private HeaderFlags flags;
		private uint tag_size;
#endregion
#region Public Fields
		public const uint Size=10;
		public static readonly ReadOnlyByteVector FileIdentifier="3DI";
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
				throw new CorruptFileException("Provided data does not start with the file identifier");
			}
			major_version=data[3];
			revision_number=data[4];
			flags=(HeaderFlags)data[5];
			if(major_version==2&&((int)flags&127)!=0)
			{
				throw new CorruptFileException("Invalid flags set on version 2 tag.");
			}
			if(major_version==3&&((int)flags&15)!=0)
			{
				throw new CorruptFileException("Invalid flags set on version 3 tag.");
			}
			if(major_version==4&&((int)flags&7)!=0)
			{
				throw new CorruptFileException("Invalid flags set on version 4 tag.");
			}
			for(int i=6;i<10;i++)
			{
				if(data[i]>=128)
				{
					throw new CorruptFileException("One of the bytes in the header was greater than the allowed 128.");
				}
			}
			tag_size=SynchData.ToUInt(data.Mid(6,4));
		}
		public Footer(Header header)
		{
			major_version=header.MajorVersion;
			revision_number=header.RevisionNumber;
			flags=header.Flags|HeaderFlags.FooterPresent;
			tag_size=header.TagSize;
		}
#endregion
#region Public Properties
		public byte MajorVersion
		{
			get
			{
				return major_version==0?Tag.DefaultVersion:major_version;
			}
			set
			{
				if(value!=4)
				{
					throw new ArgumentException("Version unsupported.");
				}
				major_version=value;
			}
		}
		public byte RevisionNumber
		{
			get
			{
				return revision_number;
			}
			set
			{
				revision_number=value;
			}
		}
		public HeaderFlags Flags
		{
			get
			{
				return flags;
			}
			set
			{
				if(0!=(value&(HeaderFlags.ExtendedHeader|HeaderFlags.ExperimentalIndicator))&&MajorVersion<3)
				{
					throw new ArgumentException("Feature only supported in version 2.3+","value");
				}
				if(0!=(value&HeaderFlags.FooterPresent)&&MajorVersion<3)
				{
					throw new ArgumentException("Feature only supported in version 2.4+","value");
				}
				flags=value;
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
				return TagSize+Header.Size+Size;
			}
		}
#endregion
#region Public Methods
		public ByteVector Render()
		{
			ByteVector v=new ByteVector();
			v.Add(FileIdentifier);
			v.Add(MajorVersion);
			v.Add(RevisionNumber);
			v.Add((byte)flags);
			v.Add(SynchData.FromUInt(TagSize));
			return v;
		}
#endregion
	}
}
