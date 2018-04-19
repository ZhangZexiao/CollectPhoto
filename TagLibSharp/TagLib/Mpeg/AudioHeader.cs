using System;
namespace TagLib.Mpeg
{
#region Enums
	public enum Version
	{
		Unknown= -1,Version1=0,Version2=1,Version25=2
	}
	public enum ChannelMode
	{
		Stereo=0,JointStereo=1,DualChannel=2,SingleChannel=3
	}
#endregion
	public struct AudioHeader:
	IAudioCodec
	{
#region Private Static Value Arrays
		private static readonly int[,]sample_rates=new int[3,4]
		{
			{
				44100,48000,32000,0
			}
			,
			{
				22050,24000,16000,0
			}
			,
			{
				11025,12000,8000,0
			}
		};
		private static readonly int[,]block_size=new int[3,4]
		{
			{
				0,384,1152,1152
			}
			,
			{
				0,384,1152,576
			}
			,
			{
				0,384,1152,576
			}
		};
		private static readonly int[,,]bitrates=new int[2,3,16]
		{
			{
				{
					0,
					32,
					64,
					96,
					128,
					160,
					192,
					224,
					256,
					288,
					320,
					352,
					384,
					416,
					448,
					-1
				}
				,
				{
					0,
					32,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					160,
					192,
					224,
					256,
					320,
					384,
					-1
				}
				,
				{
					0,
					32,
					40,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					160,
					192,
					224,
					256,
					320,
					-1
				}
			}
			,
			{
				{
					0,
					32,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					144,
					160,
					176,
					192,
					224,
					256,
					-1
				}
				,
				{
					0,
					8,
					16,
					24,
					32,
					40,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					144,
					160,
					-1
				}
				,
				{
					0,
					8,
					16,
					24,
					32,
					40,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					144,
					160,
					-1
				}
			}
		};
#endregion
#region Private Properties
		private uint flags;
		private long stream_length;
		private XingHeader xing_header;
		private VBRIHeader vbri_header;
		private TimeSpan duration;
#endregion
#region Public Fields
		public static readonly AudioHeader Unknown=new AudioHeader(0,0,XingHeader.Unknown,VBRIHeader.Unknown);
#endregion
#region Constructors
		private AudioHeader(uint flags,long streamLength,XingHeader xingHeader,VBRIHeader vbriHeader)
		{
			this.flags=flags;
			this.stream_length=streamLength;
			this.xing_header=xingHeader;
			this.vbri_header=vbriHeader;
			this.duration=TimeSpan.Zero;
		}
		private AudioHeader(ByteVector data,TagLib.File file,long position)
		{
			this.duration=TimeSpan.Zero;
			stream_length=0;
			string error=GetHeaderError(data);
			if(error!=null)
			{
				throw new CorruptFileException(error);
			}
			flags=data.ToUInt();
			xing_header=XingHeader.Unknown;
			vbri_header=VBRIHeader.Unknown;
			file.Seek(position+XingHeader.XingHeaderOffset(Version,ChannelMode));
			ByteVector xing_data=file.ReadBlock(16);
			if(xing_data.Count==16&&xing_data.StartsWith(XingHeader.FileIdentifier))
			{
				xing_header=new XingHeader(xing_data);
			}
			if(xing_header.Present)
			{
				return;
			}
			file.Seek(position+VBRIHeader.VBRIHeaderOffset());
			ByteVector vbri_data=file.ReadBlock(24);
			if(vbri_data.Count==24&&vbri_data.StartsWith(VBRIHeader.FileIdentifier))
			{
				vbri_header=new VBRIHeader(vbri_data);
			}
		}
#endregion
#region Public Properties
		public Version Version
		{
			get
			{
				switch((flags>>19)&0x03)
				{
				case 0:
					return Version.Version25;
				case 2:
					return Version.Version2;
				default:
					return Version.Version1;
				}
			}
		}
		public int AudioLayer
		{
			get
			{
				switch((flags>>17)&0x03)
				{
				case 1:
					return 3;
				case 2:
					return 2;
				default:
					return 1;
				}
			}
		}
		public int AudioBitrate
		{
			get
			{
				if(xing_header.TotalSize>0&&duration>TimeSpan.Zero)
				{
					return(int)Math.Round((((XingHeader.TotalSize*8L)/duration.TotalSeconds)/1000.0));
				}
				if(vbri_header.TotalSize>0&&duration>TimeSpan.Zero)
				{
					return(int)Math.Round((((VBRIHeader.TotalSize*8L)/duration.TotalSeconds)/1000.0));
				}
				return bitrates[Version==Version.Version1?0:1,AudioLayer>0?AudioLayer-1:0,(int)(flags>>12)&0x0F];
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return sample_rates[(int)Version,(int)(flags>>10)&0x03];
			}
		}
		public int AudioChannels
		{
			get
			{
				return ChannelMode==ChannelMode.SingleChannel?1:2;
			}
		}
		public int AudioFrameLength
		{
			get
			{
				switch(AudioLayer)
				{
					case 1:return 48000*AudioBitrate/AudioSampleRate+(IsPadded?4:0);
					case 2:return 144000*AudioBitrate/AudioSampleRate+(IsPadded?1:0);
				case 3:
					if(Version==Version.Version1)
					{
						goto case 2;
					}
					return 72000*AudioBitrate/AudioSampleRate+(IsPadded?1:0);
				default:
					return 0;
				}
			}
		}
		public TimeSpan Duration
		{
			get
			{
				if(duration>TimeSpan.Zero)
				{
					return duration;
				}
				if(xing_header.TotalFrames>0)
				{
					double time_per_frame=(double)block_size[(int)Version,AudioLayer]/(double)AudioSampleRate;
					duration=TimeSpan.FromSeconds(time_per_frame*XingHeader.TotalFrames);
				}
				else if(vbri_header.TotalFrames>0)
				{
					double time_per_frame=(double)block_size[(int)Version,AudioLayer]/(double)AudioSampleRate;
					duration=TimeSpan.FromSeconds(Math.Round(time_per_frame*VBRIHeader.TotalFrames));
				}
				else if(AudioFrameLength>0&&AudioBitrate>0)
				{
					int frames=(int)(stream_length/AudioFrameLength+1);
					duration=TimeSpan.FromSeconds((double)(AudioFrameLength*frames)/(double)(AudioBitrate*125)+0.5);
				}
				return duration;
			}
		}
		public string Description
		{
			get
			{
				System.Text.StringBuilder builder=new System.Text.StringBuilder();
				builder.Append("MPEG Version ");
				switch(Version)
				{
				case Version.Version1:
					builder.Append("1");
					break;
				case Version.Version2:
					builder.Append("2");
					break;
				case Version.Version25:
					builder.Append("2.5");
					break;
				}
				builder.Append(" Audio, Layer ");
				builder.Append(AudioLayer);
				if(xing_header.Present||vbri_header.Present)
				{
					builder.Append(" VBR");
				}
				return builder.ToString();
			}
		}
		public MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.Audio;
			}
		}
		public bool IsProtected
		{
			get
			{
				return((flags>>16)&1)==0;
			}
		}
		public bool IsPadded
		{
			get
			{
				return((flags>>9)&1)==1;
			}
		}
		public bool IsCopyrighted
		{
			get
			{
				return((flags>>3)&1)==1;
			}
		}
		public bool IsOriginal
		{
			get
			{
				return((flags>>2)&1)==1;
			}
		}
		public ChannelMode ChannelMode
		{
			get
			{
				return(ChannelMode)((flags>>6)&0x03);
			}
		}
		public XingHeader XingHeader
		{
			get
			{
				return xing_header;
			}
		}
		public VBRIHeader VBRIHeader
		{
			get
			{
				return vbri_header;
			}
		}
#endregion
#region Public Methods
		public void SetStreamLength(long streamLength)
		{
			this.stream_length=streamLength;
			if(xing_header.TotalFrames==0||vbri_header.TotalFrames==0)
			{
				duration=TimeSpan.Zero;
			}
		}
#endregion
#region Public Static Methods
		public static bool Find(out AudioHeader header,TagLib.File file,long position,int length)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			long end=position+length;
			header=AudioHeader.Unknown;
			file.Seek(position);
			ByteVector buffer=file.ReadBlock(3);
			if(buffer.Count<3)
			{
				return false;
			}
			do
			{
				file.Seek(position+3);
				buffer=buffer.Mid(buffer.Count-3);
				buffer.Add(file.ReadBlock((int)File.BufferSize));
				for(int i=0;i<buffer.Count-3&&(length<0||position+i<end);i++)
				{
					if(buffer[i]==0xFF&&buffer[i+1]>0xE0)
					{
						ByteVector data=buffer.Mid(i,4);
						if(GetHeaderError(data)==null)
						{
							try
							{
								header=new AudioHeader(data,file,position+i);
								return true;
							}
							catch(CorruptFileException)
							{
							}
						}
					}
				}
				position+=File.BufferSize;
			}
			while(buffer.Count>3&&(length<0||position<end));
			return false;
		}
		public static bool Find(out AudioHeader header,TagLib.File file,long position)
		{
			return Find(out header,file,position,-1);
		}
		private static string GetHeaderError(ByteVector data)
		{
			if(data.Count<4)
			{
				return"Insufficient header length.";
			}
			if(data[0]!=0xFF)
			{
				return"First byte did not match MPEG synch.";
			}
			if((data[1]&0xE6)<=0xE0||(data[1]&0x18)==0x08)
			{
				return"Second byte did not match MPEG synch.";
			}
			uint flags=data.ToUInt();
			if(((flags>>12)&0x0F)==0x0F)
			{
				return"Header uses invalid bitrate index.";
			}
			if(((flags>>10)&0x03)==0x03)
			{
				return"Invalid sample rate.";
			}
			return null;
		}
#endregion
	}
}
