using System.Collections.Generic;
namespace TagLib.Id3v2
{
	public static class FrameFactory
	{
		public delegate Frame FrameCreator(ByteVector data,int offset,FrameHeader header,byte version);
		private static List<FrameCreator>frame_creators=new List<FrameCreator>();
		public static Frame CreateFrame(ByteVector data,ref int offset,byte version,bool alreadyUnsynched)
		{
			int position=offset;
			FrameHeader header=new FrameHeader(data.Mid(position,(int)FrameHeader.Size(version)),version);
			offset+=(int)(header.FrameSize+FrameHeader.Size(version));
			if(header.FrameId==null)
			{
				throw new System.NotImplementedException();
			}
			foreach(byte b in header.FrameId)
			{
				char c=(char)b;
				if((c<'A'||c>'Z')&&(c<'0'||c>'9'))
				{
					return null;
				}
			}
			if(alreadyUnsynched)
			{
				header.Flags&= ~FrameFlags.Unsynchronisation;
			}
			if(header.FrameSize==0)
			{
				header.Flags|=FrameFlags.TagAlterPreservation;
				return new UnknownFrame(data,position,header,version);
			}
			if((header.Flags&FrameFlags.Compression)!=0)
			{
				throw new System.NotImplementedException();
			}
			if((header.Flags&FrameFlags.Encryption)!=0)
			{
				throw new System.NotImplementedException();
			}
			foreach(FrameCreator creator in frame_creators)
			{
				Frame frame=creator(data,position,header,version);
				if(frame!=null)
				{
					return frame;
				}
			}
			if(header.FrameId==FrameType.TXXX)
			{
				return new UserTextInformationFrame(data,position,header,version);
			}
			if(header.FrameId[0]==(byte)'T')
			{
				return new TextInformationFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.UFID)
			{
				return new UniqueFileIdentifierFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.MCDI)
			{
				return new MusicCdIdentifierFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.USLT)
			{
				return new UnsynchronisedLyricsFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.SYLT)
			{
				return new SynchronisedLyricsFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.COMM)
			{
				return new CommentsFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.RVA2)
			{
				return new RelativeVolumeFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.APIC)
			{
				return new AttachedPictureFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.GEOB)
			{
				return new GeneralEncapsulatedObjectFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.PCNT)
			{
				return new PlayCountFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.POPM)
			{
				return new PopularimeterFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.USER)
			{
				return new TermsOfUseFrame(data,position,header,version);
			}
			if(header.FrameId==FrameType.PRIV)
			{
				return new PrivateFrame(data,position,header,version);
			}
			if(header.FrameId[0]==(byte)'W')
			{
				return new UrlLinkFrame(data,position,header,version);
			}
			return new UnknownFrame(data,position,header,version);
		}
		public static void AddFrameCreator(FrameCreator creator)
		{
			if(creator==null)
			{
				throw new System.ArgumentNullException("creator");
			}
			frame_creators.Insert(0,creator);
		}
	}
}
