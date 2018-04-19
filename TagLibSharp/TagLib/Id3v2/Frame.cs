using System;
namespace TagLib.Id3v2
{
	public abstract class Frame:
	ICloneable
	{
#region Private Fields
		private FrameHeader header;
		private byte group_id;
		private byte encryption_id;
#endregion
#region Constructors
		protected Frame(ByteVector data,byte version)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<((version<3)?3:4))
			{
				throw new ArgumentException("Data contains an incomplete identifier.","data");
			}
			header=new FrameHeader(data,version);
		}
		protected Frame(FrameHeader header)
		{
			this.header=header;
		}
#endregion
#region Public Properties
		public ReadOnlyByteVector FrameId
		{
			get
			{
				return header.FrameId;
			}
		}
		public uint Size
		{
			get
			{
				return header.FrameSize;
			}
		}
		public FrameFlags Flags
		{
			get
			{
				return header.Flags;
			}
			set
			{
				header.Flags=value;
			}
		}
		public short GroupId
		{
			get
			{
				return(Flags&FrameFlags.GroupingIdentity)!=0?group_id:(short)-1;
			}
			set
			{
				if(value>=0x00&&value<=0xFF)
				{
					group_id=(byte)value;
					Flags|=FrameFlags.GroupingIdentity;
				}
				else
				{
					Flags&= ~FrameFlags.GroupingIdentity;
				}
			}
		}
		public short EncryptionId
		{
			get
			{
				return(Flags&FrameFlags.Encryption)!=0?encryption_id:(short)-1;
			}
			set
			{
				if(value>=0x00&&value<=0xFF)
				{
					encryption_id=(byte)value;
					Flags|=FrameFlags.Encryption;
				}
				else
				{
					Flags&= ~FrameFlags.Encryption;
				}
			}
		}
#endregion
#region Public Methods
		public virtual ByteVector Render(byte version)
		{
			if(version<4)
			{
				Flags&= ~(FrameFlags.DataLengthIndicator|FrameFlags.Unsynchronisation);
			}
			if(version<3)
			{
				Flags&= ~(FrameFlags.Compression|FrameFlags.Encryption|FrameFlags.FileAlterPreservation|FrameFlags.GroupingIdentity|FrameFlags.ReadOnly|FrameFlags.TagAlterPreservation);
			}
			ByteVector field_data=RenderFields(version);
			if(field_data.Count==0)
			{
				return new ByteVector();
			}
			ByteVector front_data=new ByteVector();
			if((Flags&(FrameFlags.Compression|FrameFlags.DataLengthIndicator))!=0)
			{
				front_data.Add(ByteVector.FromUInt((uint)field_data.Count));
			}
			if((Flags&FrameFlags.GroupingIdentity)!=0)
			{
				front_data.Add(group_id);
			}
			if((Flags&FrameFlags.Encryption)!=0)
			{
				front_data.Add(encryption_id);
			}
			if((Flags&FrameFlags.Compression)!=0)
			{
				throw new NotImplementedException("Compression not yet supported");
			}
			if((Flags&FrameFlags.Encryption)!=0)
			{
				throw new NotImplementedException("Encryption not yet supported");
			}
			if((Flags&FrameFlags.Unsynchronisation)!=0)
			{
				SynchData.UnsynchByteVector(field_data);
			}
			if(front_data.Count>0)
			{
				field_data.Insert(0,front_data);
			}
			header.FrameSize=(uint)field_data.Count;
			ByteVector header_data=header.Render(version);
			header_data.Add(field_data);
			return header_data;
		}
#endregion
#region Public Static Methods
		[Obsolete("Use ByteVector.TextDelimiter.")]
		public static ByteVector TextDelimiter(StringType type)
		{
			return ByteVector.TextDelimiter(type);
		}
#endregion
#region Protected Methods
		protected static StringType CorrectEncoding(StringType type,byte version)
		{
			if(Tag.ForceDefaultEncoding)
			{
				type=Tag.DefaultEncoding;
			}
			return(version<4&&type==StringType.UTF8)?StringType.UTF16:type;
		}
		protected void SetData(ByteVector data,int offset,byte version,bool readHeader)
		{
			if(readHeader)
			{
				header=new FrameHeader(data,version);
			}
			ParseFields(FieldData(data,offset,version),version);
		}
		protected abstract void ParseFields(ByteVector data,byte version);
		protected abstract ByteVector RenderFields(byte version);
		protected ByteVector FieldData(ByteVector frameData,int offset,byte version)
		{
			if(frameData==null)
			{
				throw new ArgumentNullException("frameData");
			}
			int data_offset=offset+(int)FrameHeader.Size(version);
			int data_length=(int)Size;
			if((Flags&(FrameFlags.Compression|FrameFlags.DataLengthIndicator))!=0)
			{
				data_offset+=4;
				data_length-=4;
			}
			if((Flags&FrameFlags.GroupingIdentity)!=0)
			{
				if(frameData.Count>=data_offset)
				{
					throw new TagLib.CorruptFileException("Frame data incomplete.");
				}
				group_id=frameData[data_offset++];
				data_length--;
			}
			if((Flags&FrameFlags.Encryption)!=0)
			{
				if(frameData.Count>=data_offset)
				{
					throw new TagLib.CorruptFileException("Frame data incomplete.");
				}
				encryption_id=frameData[data_offset++];
				data_length--;
			}
			data_length=Math.Min(data_length,frameData.Count-data_offset);
			if(data_length<0)
			{
				throw new CorruptFileException("Frame size less than zero.");
			}
			ByteVector data=frameData.Mid(data_offset,data_length);
			if((Flags&FrameFlags.Unsynchronisation)!=0)
			{
				int before_length=data.Count;
				SynchData.ResynchByteVector(data);
				data_length-=(data.Count-before_length);
			}
			if((Flags&FrameFlags.Encryption)!=0)
			{
				throw new NotImplementedException();
			}
			if((Flags&FrameFlags.Compression)!=0)
			{
				throw new NotImplementedException();
			}
			return data;
		}
#endregion
#region ICloneable
		public virtual Frame Clone()
		{
			int index=0;
			return FrameFactory.CreateFrame(Render(4),ref index,4,false);
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
#endregion
	}
}
