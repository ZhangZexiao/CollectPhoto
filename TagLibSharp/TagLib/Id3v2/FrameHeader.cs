using System;
namespace TagLib.Id3v2
{
	[Flags]
	public enum FrameFlags:ushort
	{
		None=0,TagAlterPreservation=0x4000,FileAlterPreservation=0x2000,ReadOnly=0x1000,GroupingIdentity=0x0040,Compression=0x0008,Encryption=0x0004,Unsynchronisation=0x0002,DataLengthIndicator=0x0001
	}
	public struct FrameHeader
	{
#region Private Fields
		private ReadOnlyByteVector frame_id;
		private uint frame_size;
		private FrameFlags flags;
#endregion
#region Constructors
		public FrameHeader(ByteVector data,byte version)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			flags=0;
			frame_size=0;
			if(version<2||version>4)
			{
				throw new CorruptFileException("Unsupported tag version.");
			}
			if(data.Count<(version==2?3:4))
			{
				throw new CorruptFileException("Data must contain at least a frame ID.");
			}
			switch(version)
			{
			case 2:
				frame_id=ConvertId(data.Mid(0,3),version,false);
				if(data.Count<6)
				{
					return;
				}
				frame_size=data.Mid(3,3).ToUInt();
				return;
			case 3:
				frame_id=ConvertId(data.Mid(0,4),version,false);
				if(data.Count<10)
				{
					return;
				}
				frame_size=data.Mid(4,4).ToUInt();
				flags=(FrameFlags)(((data[8]<<7)&0x7000)|((data[9]>>4)&0x000C)|((data[9]<<1)&0x0040));
				return;
			case 4:
				frame_id=new ReadOnlyByteVector(data.Mid(0,4));
				if(data.Count<10)
				{
					return;
				}
				frame_size=SynchData.ToUInt(data.Mid(4,4));
				flags=(FrameFlags)data.Mid(8,2).ToUShort();
				return;
			default:
				throw new CorruptFileException("Unsupported tag version.");
			}
		}
#endregion
#region Public Properties
		public ReadOnlyByteVector FrameId
		{
			get
			{
				return frame_id;
			}
			set
			{
				if(value==null)
				{
					throw new ArgumentNullException("value");
				}
				frame_id=value.Count==4?value:new ReadOnlyByteVector(value.Mid(0,4));
			}
		}
		public uint FrameSize
		{
			get
			{
				return frame_size;
			}
			set
			{
				frame_size=value;
			}
		}
		public FrameFlags Flags
		{
			get
			{
				return flags;
			}
			set
			{
				if((value&(FrameFlags.Compression|FrameFlags.Encryption))!=0)
				{
					throw new ArgumentException("Encryption and compression are not supported.","value");
				}
				flags=value;
			}
		}
#endregion
#region Public Methods
		public ByteVector Render(byte version)
		{
			ByteVector data=new ByteVector();
			ByteVector id=ConvertId(frame_id,version,true);
			if(id==null)
			{
				throw new NotImplementedException();
			}
			switch(version)
			{
			case 2:
				data.Add(id);
				data.Add(ByteVector.FromUInt(frame_size).Mid(1,3));
				return data;
			case 3:
				ushort new_flags=(ushort)((((ushort)flags<<1)&0xE000)|(((ushort)flags<<4)&0x00C0)|(((ushort)flags>>1)&0x0020));
				data.Add(id);
				data.Add(ByteVector.FromUInt(frame_size));
				data.Add(ByteVector.FromUShort(new_flags));
				return data;
			case 4:
				data.Add(id);
				data.Add(SynchData.FromUInt(frame_size));
				data.Add(ByteVector.FromUShort((ushort)flags));
				return data;
			default:
				throw new NotImplementedException("Unsupported tag version.");
			}
		}
		public static uint Size(byte version)
		{
			return(uint)(version<3?6:10);
		}
#endregion
#region Private Methods
		private static ReadOnlyByteVector ConvertId(ByteVector id,byte version,bool toVersion)
		{
			if(version>=4)
			{
				ReadOnlyByteVector outid=id as ReadOnlyByteVector;
				return outid!=null?outid:new ReadOnlyByteVector(id);
			}
			if(id==null||version<2)
			{
				return null;
			}
			if(!toVersion&&(id==FrameType.EQUA||id==FrameType.RVAD||id==FrameType.TRDA||id==FrameType.TSIZ))
			{
				return null;
			}
			if(version==2)
			{
				for(int i=0;i<version2_frames.GetLength(0);i++)
				{
					if(!version2_frames[i,toVersion?1:0].Equals(id))
					{
						continue;
					}
					return version2_frames[i,toVersion?0:1];
				}
			}
			if(version==3)
			{
				for(int i=0;i<version3_frames.GetLength(0);i++)
				{
					if(!version3_frames[i,toVersion?1:0].Equals(id))
					{
						continue;
					}
					return version3_frames[i,toVersion?0:1];
				}
			}
			if((id.Count!=4&&version>2)||(id.Count!=3&&version==2))
			{
				return null;
			}
			return id is ReadOnlyByteVector?id as ReadOnlyByteVector:new ReadOnlyByteVector(id);
		}
		private static readonly ReadOnlyByteVector[,]version2_frames=new ReadOnlyByteVector[59,2]
		{
			{
				"BUF","RBUF"
			}
			,
			{
				"CNT","PCNT"
			}
			,
			{
				"COM","COMM"
			}
			,
			{
				"CRA","AENC"
			}
			,
			{
				"ETC","ETCO"
			}
			,
			{
				"GEO","GEOB"
			}
			,
			{
				"IPL","TIPL"
			}
			,
			{
				"MCI","MCDI"
			}
			,
			{
				"MLL","MLLT"
			}
			,
			{
				"PIC","APIC"
			}
			,
			{
				"POP","POPM"
			}
			,
			{
				"REV","RVRB"
			}
			,
			{
				"SLT","SYLT"
			}
			,
			{
				"STC","SYTC"
			}
			,
			{
				"TAL","TALB"
			}
			,
			{
				"TBP","TBPM"
			}
			,
			{
				"TCM","TCOM"
			}
			,
			{
				"TCO","TCON"
			}
			,
			{
				"TCP","TCMP"
			}
			,
			{
				"TCR","TCOP"
			}
			,
			{
				"TDA","TDAT"
			}
			,
			{
				"TIM","TIME"
			}
			,
			{
				"TDY","TDLY"
			}
			,
			{
				"TEN","TENC"
			}
			,
			{
				"TFT","TFLT"
			}
			,
			{
				"TKE","TKEY"
			}
			,
			{
				"TLA","TLAN"
			}
			,
			{
				"TLE","TLEN"
			}
			,
			{
				"TMT","TMED"
			}
			,
			{
				"TOA","TOAL"
			}
			,
			{
				"TOF","TOFN"
			}
			,
			{
				"TOL","TOLY"
			}
			,
			{
				"TOR","TDOR"
			}
			,
			{
				"TOT","TOAL"
			}
			,
			{
				"TP1","TPE1"
			}
			,
			{
				"TP2","TPE2"
			}
			,
			{
				"TP3","TPE3"
			}
			,
			{
				"TP4","TPE4"
			}
			,
			{
				"TPA","TPOS"
			}
			,
			{
				"TPB","TPUB"
			}
			,
			{
				"TRC","TSRC"
			}
			,
			{
				"TRK","TRCK"
			}
			,
			{
				"TSS","TSSE"
			}
			,
			{
				"TT1","TIT1"
			}
			,
			{
				"TT2","TIT2"
			}
			,
			{
				"TT3","TIT3"
			}
			,
			{
				"TXT","TOLY"
			}
			,
			{
				"TXX","TXXX"
			}
			,
			{
				"TYE","TDRC"
			}
			,
			{
				"UFI","UFID"
			}
			,
			{
				"ULT","USLT"
			}
			,
			{
				"WAF","WOAF"
			}
			,
			{
				"WAR","WOAR"
			}
			,
			{
				"WAS","WOAS"
			}
			,
			{
				"WCM","WCOM"
			}
			,
			{
				"WCP","WCOP"
			}
			,
			{
				"WPB","WPUB"
			}
			,
			{
				"WXX","WXXX"
			}
			,
			{
				"XRV","RVA2"
			}
		};
		private static readonly ReadOnlyByteVector[,]version3_frames=new ReadOnlyByteVector[3,2]
		{
			{
				"TORY","TDOR"
			}
			,
			{
				"TYER","TDRC"
			}
			,
			{
				"XRVA","RVA2"
			}
		};
#endregion
	}
}
