using System;
namespace TagLib.Mpeg4
{
	public static class BoxFactory
	{
		private static Box CreateBox(TagLib.File file,BoxHeader header,BoxHeader parent,IsoHandlerBox handler,int index)
		{
			if(parent.BoxType==BoxType.Stsd&&parent.Box is IsoSampleDescriptionBox&&index<(parent.Box as IsoSampleDescriptionBox).EntryCount)
			{
				if(handler!=null&&handler.HandlerType==BoxType.Soun)
				{
					return new IsoAudioSampleEntry(header,file,handler);
				}
				else if(handler!=null&&handler.HandlerType==BoxType.Vide)
				{
					return new IsoVisualSampleEntry(header,file,handler);
				}
				else if(handler!=null&&handler.HandlerType==BoxType.Alis)
				{
					if(header.BoxType==BoxType.Text)
					{
						return new TextBox(header,file,handler);
					}
					else if(header.BoxType==BoxType.Url)
					{
						return new UrlBox(header,file,handler);
					}
					return new UnknownBox(header,file,handler);
				}
				else
				{
					return new IsoSampleEntry(header,file,handler);
				}
			}
			ByteVector type=header.BoxType;
			if(type==BoxType.Mvhd)
			{
				return new IsoMovieHeaderBox(header,file,handler);
			}
			else if(type==BoxType.Stbl)
			{
				return new IsoSampleTableBox(header,file,handler);
			}
			else if(type==BoxType.Stsd)
			{
				return new IsoSampleDescriptionBox(header,file,handler);
			}
			else if(type==BoxType.Stco)
			{
				return new IsoChunkOffsetBox(header,file,handler);
			}
			else if(type==BoxType.Co64)
			{
				return new IsoChunkLargeOffsetBox(header,file,handler);
			}
			else if(type==BoxType.Hdlr)
			{
				return new IsoHandlerBox(header,file,handler);
			}
			else if(type==BoxType.Udta)
			{
				return new IsoUserDataBox(header,file,handler);
			}
			else if(type==BoxType.Meta)
			{
				return new IsoMetaBox(header,file,handler);
			}
			else if(type==BoxType.Ilst)
			{
				return new AppleItemListBox(header,file,handler);
			}
			else if(type==BoxType.Data)
			{
				return new AppleDataBox(header,file,handler);
			}
			else if(type==BoxType.Esds)
			{
				return new AppleElementaryStreamDescriptor(header,file,handler);
			}
			else if(type==BoxType.Free||type==BoxType.Skip)
			{
				return new IsoFreeSpaceBox(header,file,handler);
			}
			else if(type==BoxType.Mean||type==BoxType.Name)
			{
				return new AppleAdditionalInfoBox(header,file,handler);
			}
			if(parent.BoxType==BoxType.Ilst)
			{
				return new AppleAnnotationBox(header,file,handler);
			}
			return new UnknownBox(header,file,handler);
		}
		internal static Box CreateBox(TagLib.File file,long position,BoxHeader parent,IsoHandlerBox handler,int index)
		{
			BoxHeader header=new BoxHeader(file,position);
			return CreateBox(file,header,parent,handler,index);
		}
		public static Box CreateBox(TagLib.File file,long position,IsoHandlerBox handler)
		{
			return CreateBox(file,position,BoxHeader.Empty,handler,-1);
		}
		public static Box CreateBox(TagLib.File file,long position)
		{
			return CreateBox(file,position,null);
		}
		public static Box CreateBox(TagLib.File file,BoxHeader header,IsoHandlerBox handler)
		{
			return CreateBox(file,header,BoxHeader.Empty,handler,-1);
		}
		public static Box CreateBox(TagLib.File file,BoxHeader header)
		{
			return CreateBox(file,header,null);
		}
	}
}
