using System;
namespace TagLib.Ogg
{
	public class Bitstream
	{
#region Private Fields
		private ByteVector previous_packet;
		private int packet_index;
		private Codec codec;
		private long first_absolute_granular_position;
#endregion
#region Constructors
		public Bitstream(Page page)
		{
			if(page==null)
			{
				throw new ArgumentNullException("page");
			}
			codec=Codec.GetCodec(page.Packets[0]);
			first_absolute_granular_position=page.Header.AbsoluteGranularPosition;
		}
#endregion
#region Public Methods
		public bool ReadPage(Page page)
		{
			if(page==null)
			{
				throw new ArgumentNullException("page");
			}
			ByteVector[]packets=page.Packets;
			for(int i=0;i<packets.Length;i++)
			{
				if((page.Header.Flags&PageFlags.FirstPacketContinued)==0&&previous_packet!=null)
				{
					if(ReadPacket(previous_packet))
					{
						return true;
					}
					previous_packet=null;
				}
				ByteVector packet=packets[i];
				if(i==0&&(page.Header.Flags&PageFlags.FirstPacketContinued)!=0&&previous_packet!=null)
				{
					previous_packet.Add(packet);
					packet=previous_packet;
				}
				previous_packet=null;
				if(i==packets.Length-1)
				{
					previous_packet=new ByteVector(packet);
				}
				else if(ReadPacket(packet))
				{
					return true;
				}
			}
			return false;
		}
		public TimeSpan GetDuration(long lastAbsoluteGranularPosition)
		{
			return codec.GetDuration(first_absolute_granular_position,lastAbsoluteGranularPosition);
		}
#endregion
#region Public Properties
		public Codec Codec
		{
			get
			{
				return codec;
			}
		}
#endregion
#region Public Properties
		private bool ReadPacket(ByteVector packet)
		{
			return codec.ReadPacket(packet,packet_index++);
		}
#endregion
	}
}
