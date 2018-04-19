using System.Collections.Generic;
using System;
namespace TagLib.Ogg
{
	public class Page
	{
#region Private Properties
		private PageHeader header;
		private ByteVectorCollection packets;
#endregion
#region Constructors
		protected Page(PageHeader header)
		{
			this.header=header;
			packets=new ByteVectorCollection();
		}
		public Page(File file,long position):
		this(new PageHeader(file,position))
		{
			file.Seek(position+header.Size);
			foreach(int packet_size in header.PacketSizes)
			packets.Add(file.ReadBlock(packet_size));
		}
		public Page(ByteVectorCollection packets,PageHeader header):
		this(header)
		{
			if(packets==null)
			{
				throw new ArgumentNullException("packets");
			}
			this.packets=new ByteVectorCollection(packets);
			List<int>packet_sizes=new List<int>();
			foreach(ByteVector v in packets)
			packet_sizes.Add(v.Count);
			header.PacketSizes=packet_sizes.ToArray();
		}
#endregion
#region Public Methods
		public ByteVector Render()
		{
			ByteVector data=header.Render();
			foreach(ByteVector v in packets)
			data.Add(v);
			ByteVector checksum=ByteVector.FromUInt(data.Checksum,false);
			for(int i=0;i<4;i++)
			{
				data[i+22]=checksum[i];
			}
			return data;
		}
#endregion
#region Public Properties
		public PageHeader Header
		{
			get
			{
				return header;
			}
		}
		public ByteVector[]Packets
		{
			get
			{
				return packets.ToArray();
			}
		}
		public uint Size
		{
			get
			{
				return header.Size+header.DataSize;
			}
		}
#endregion
#region Public Static Methods
		public static void OverwriteSequenceNumbers(File file,long position,IDictionary<uint,int>shiftTable)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			if(shiftTable==null)
			{
				throw new ArgumentNullException("shiftTable");
			}
			bool done=true;
			foreach(KeyValuePair<uint,int>pair in shiftTable)
			if(pair.Value!=0)
			{
				done=false;
				break;
			}
			if(done)
			{
				return;
			}
			while(position<file.Length-27)
			{
				PageHeader header=new PageHeader(file,position);
				int size=(int)(header.Size+header.DataSize);
				if(shiftTable.ContainsKey(header.StreamSerialNumber)&&shiftTable[header.StreamSerialNumber]!=0)
				{
					file.Seek(position);
					ByteVector page_data=file.ReadBlock(size);
					ByteVector new_data=ByteVector.FromUInt((uint)(header.PageSequenceNumber+shiftTable[header.StreamSerialNumber]),false);
					for(int i=18;i<22;i++)
					{
						page_data[i]=new_data[i-18];
					}
					for(int i=22;i<26;i++)
					{
						page_data[i]=0;
					}
					new_data.Add(ByteVector.FromUInt(page_data.Checksum,false));
					file.Seek(position+18);
					file.WriteBlock(new_data);
				}
				position+=size;
			}
		}
#endregion
	}
}
