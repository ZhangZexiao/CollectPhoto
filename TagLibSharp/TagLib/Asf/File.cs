using System;
using System.Collections.Generic;
namespace TagLib.Asf
{
	[SupportedMimeType("taglib/wma","wma")]
	[SupportedMimeType("taglib/wmv","wmv")]
	[SupportedMimeType("taglib/asf","asf")]
	[SupportedMimeType("audio/x-ms-wma")]
	[SupportedMimeType("audio/x-ms-asf")]
	[SupportedMimeType("video/x-ms-asf")]
	public class File:TagLib.File
	{
#region Private Fields
		private Asf.Tag asf_tag=null;
		private Properties properties=null;
#endregion
#region Constructors
		public File(string path,ReadStyle propertiesStyle):
		base(path)
		{
			Read(propertiesStyle);
		}
		public File(string path):
		this(path,ReadStyle.Average)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction)
		{
			Read(propertiesStyle);
		}
		public File(File.IFileAbstraction abstraction):
		this(abstraction,ReadStyle.Average)
		{
		}
#endregion
#region Public Properties
		public override TagLib.Tag Tag
		{
			get
			{
				return asf_tag;
			}
		}
		public override TagLib.Properties Properties
		{
			get
			{
				return properties;
			}
		}
#endregion
#region Public Methods
		public override void Save()
		{
			Mode=AccessMode.Write;
			try
			{
				HeaderObject header=new HeaderObject(this,0);
				if(asf_tag==null)
				{
					header.RemoveContentDescriptors();
					TagTypesOnDisk&= ~TagTypes.Asf;
				}
				else
				{
					TagTypesOnDisk|=TagTypes.Asf;
					header.AddUniqueObject(asf_tag.ContentDescriptionObject);
					header.AddUniqueObject(asf_tag.ExtendedContentDescriptionObject);
					header.Extension.AddUniqueObject(asf_tag.MetadataLibraryObject);
				}
				ByteVector output=header.Render();
				long diff=output.Count-(long)header.OriginalSize;
				Insert(output,0,(long)header.OriginalSize);
				InvariantStartPosition+=diff;
				InvariantEndPosition+=diff;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		public override TagLib.Tag GetTag(TagTypes type,bool create)
		{
			if(type==TagTypes.Asf)
			{
				return asf_tag;
			}
			return null;
		}
		public override void RemoveTags(TagTypes types)
		{
			if((types&TagTypes.Asf)==TagTypes.Asf)
			{
				asf_tag.Clear();
			}
		}
		public ushort ReadWord()
		{
			return ReadBlock(2).ToUShort(false);
		}
		public uint ReadDWord()
		{
			return ReadBlock(4).ToUInt(false);
		}
		public ulong ReadQWord()
		{
			return ReadBlock(8).ToULong(false);
		}
		public System.Guid ReadGuid()
		{
			return new System.Guid(ReadBlock(16).Data);
		}
		public string ReadUnicode(int length)
		{
			ByteVector data=ReadBlock(length);
			string output=data.ToString(StringType.UTF16LE);
			int i=output.IndexOf('\0');
			return(i>=0)?output.Substring(0,i):output;
		}
		public IEnumerable<Object>ReadObjects(uint count,long position)
		{
			for(int i=0;i<(int)count;i++)
			{
				Object obj=ReadObject(position);
				position+=(long)obj.OriginalSize;
				yield return obj;
			}
		}
		public Object ReadObject(long position)
		{
			Seek(position);
			System.Guid id=ReadGuid();
			if(id.Equals(Guid.AsfFilePropertiesObject))
			{
				return new FilePropertiesObject(this,position);
			}
			if(id.Equals(Guid.AsfStreamPropertiesObject))
			{
				return new StreamPropertiesObject(this,position);
			}
			if(id.Equals(Guid.AsfContentDescriptionObject))
			{
				return new ContentDescriptionObject(this,position);
			}
			if(id.Equals(Guid.AsfExtendedContentDescriptionObject))
			{
				return new ExtendedContentDescriptionObject(this,position);
			}
			if(id.Equals(Guid.AsfPaddingObject))
			{
				return new PaddingObject(this,position);
			}
			if(id.Equals(Guid.AsfHeaderExtensionObject))
			{
				return new HeaderExtensionObject(this,position);
			}
			if(id.Equals(Guid.AsfMetadataLibraryObject))
			{
				return new MetadataLibraryObject(this,position);
			}
			return new UnknownObject(this,position);
		}
#endregion
#region Private Methods
		private void Read(ReadStyle propertiesStyle)
		{
			Mode=AccessMode.Read;
			try
			{
				HeaderObject header=new HeaderObject(this,0);
				if(header.HasContentDescriptors)
				{
					TagTypesOnDisk|=TagTypes.Asf;
				}
				asf_tag=new Asf.Tag(header);
				InvariantStartPosition=(long)header.OriginalSize;
				InvariantEndPosition=Length;
				if(propertiesStyle!=ReadStyle.None)
				{
					properties=header.Properties;
				}
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
#endregion
	}
}
