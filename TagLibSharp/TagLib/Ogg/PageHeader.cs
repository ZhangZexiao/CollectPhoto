using System;
using System.Collections.Generic;
namespace TagLib.Ogg
{
	[Flags]
	public enum PageFlags:byte
	{
		None=0,FirstPacketContinued=1,FirstPageOfStream=2,LastPageOfStream=4
	}
	public struct PageHeader
	{
#region Private Propertis
		private List<int>packet_sizes;
		private byte version;
		private PageFlags flags;
		private ulong absolute_granular_position;
		private uint stream_serial_number;
		private uint page_sequence_number;
		private uint size;
		private uint data_size;
#endregion
#region Constructors
		public PageHeader(uint streamSerialNumber,uint pageNumber,PageFlags flags)
		{
			version=0;
			this.flags=flags;
			absolute_granular_position=0;
			stream_serial_number=streamSerialNumber;
			page_sequence_number=pageNumber;
			size=0;
			data_size=0;
			packet_sizes=new List<int>();
			if(pageNumber==0&&(flags&PageFlags.FirstPacketContinued)==0)
			{
				this.flags|=PageFlags.FirstPageOfStream;
			}
		}
		public PageHeader(File file,long position)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			if(position<0||position>file.Length-27)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			file.Seek(position);
			ByteVector data=file.ReadBlock(27);
			if(data.Count<27|| !data.StartsWith("OggS"))
			{
				throw new CorruptFileException("Error reading page header");
			}
			version=data[4];
			this.flags=(PageFlags)data[5];
			absolute_granular_position=data.Mid(6,8).ToULong(false);
			stream_serial_number=data.Mid(14,4).ToUInt(false);
			page_sequence_number=data.Mid(18,4).ToUInt(false);
			int page_segment_count=data[26];
			ByteVector page_segments=file.ReadBlock(page_segment_count);
			if(page_segment_count<1||page_segments.Count!=page_segment_count)
			{
				throw new CorruptFileException("Incorrect number of page segments");
			}
			size=(uint)(27+page_segment_count);
			packet_sizes=new List<int>();
			int packet_size=0;
			data_size=0;
			for(int i=0;i<page_segment_count;i++)
			{
				data_size+=page_segments[i];
				packet_size+=page_segments[i];
				if(page_segments[i]<255)
				{
					packet_sizes.Add(packet_size);
					packet_size=0;
				}
			}
			if(packet_size>0)
			{
				packet_sizes.Add(packet_size);
			}
		}
		public PageHeader(PageHeader original,uint offset,PageFlags flags)
		{
			version=original.version;
			this.flags=flags;
			absolute_granular_position=original.absolute_granular_position;
			stream_serial_number=original.stream_serial_number;
			page_sequence_number=original.page_sequence_number+offset;
			size=original.size;
			data_size=original.data_size;
			packet_sizes=new List<int>();
			if(page_sequence_number==0&&(flags&PageFlags.FirstPacketContinued)==0)
			{
				this.flags|=PageFlags.FirstPageOfStream;
			}
		}
#endregion
#region Public Properties
		public int[]PacketSizes
		{
			get
			{
				return packet_sizes.ToArray();
			}
			set
			{
				packet_sizes.Clear();
				packet_sizes.AddRange(value);
			}
		}
		public PageFlags Flags
		{
			get
			{
				return flags;
			}
		}
		public long AbsoluteGranularPosition
		{
			get
			{
				return(long)absolute_granular_position;
			}
		}
		public uint PageSequenceNumber
		{
			get
			{
				return page_sequence_number;
			}
		}
		public uint StreamSerialNumber
		{
			get
			{
				return stream_serial_number;
			}
		}
		public uint Size
		{
			get
			{
				return size;
			}
		}
		public uint DataSize
		{
			get
			{
				return data_size;
			}
		}
#endregion
#region Public Methods
		public ByteVector Render()
		{
			ByteVector data=new ByteVector();
			data.Add("OggS");
			data.Add(version);
			data.Add((byte)flags);
			data.Add(ByteVector.FromULong(absolute_granular_position,false));
			data.Add(ByteVector.FromUInt(stream_serial_number,false));
			data.Add(ByteVector.FromUInt((uint)page_sequence_number,false));
			data.Add(new ByteVector(4,0));
			ByteVector page_segments=LacingValues;
			data.Add((byte)page_segments.Count);
			data.Add(page_segments);
			return data;
		}
#endregion
#region Private Properties
		private ByteVector LacingValues
		{
			get
			{
				ByteVector data=new ByteVector();
				int[]sizes=PacketSizes;
				for(int i=0;i<sizes.Length;i++)
				{
					int quot=sizes[i]/255;
					int rem=sizes[i]%255;
					for(int j=0;j<quot;j++)
					{
						data.Add((byte)255);
					}
					if(i<sizes.Length-1||(packet_sizes[i]%255)!=0)
					{
						data.Add((byte)rem);
					}
				}
				return data;
			}
		}
#endregion
#region IEquatable
		public override int GetHashCode()
		{
			unchecked
			{
				return(int)(LacingValues.GetHashCode()^version^(int)flags^(int)absolute_granular_position^stream_serial_number^page_sequence_number^size^data_size);
			}
		}
		public override bool Equals(object other)
		{
			if(!(other is PageHeader))
			{
				return false;
			}
			return Equals((PageHeader)other);
		}
		public bool Equals(PageHeader other)
		{
			return packet_sizes==other.packet_sizes&&version==other.version&&flags==other.flags&&absolute_granular_position==other.absolute_granular_position&&stream_serial_number==other.stream_serial_number&&page_sequence_number==other.page_sequence_number&&size==other.size&&data_size==other.data_size;
		}
		public static bool operator==(PageHeader first,PageHeader second)
		{
			return first.Equals(second);
		}
		public static bool operator!=(PageHeader first,PageHeader second)
		{
			return!first.Equals(second);
		}
#endregion
	}
}
