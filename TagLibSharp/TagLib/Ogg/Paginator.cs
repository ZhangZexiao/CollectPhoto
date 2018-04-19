using System;
using System.Collections.Generic;
namespace TagLib.Ogg
{
	public class Paginator
	{
#region Private Fields
		private ByteVectorCollection packets=new ByteVectorCollection();
		private PageHeader?first_page_header=null;
		private Codec codec;
		private int pages_read=0;
#endregion
#region Constructors
		public Paginator(Codec codec)
		{
			this.codec=codec;
		}
#endregion
#region Public Methods
		public void AddPage(Page page)
		{
			pages_read++;
			if(first_page_header==null)
			{
				first_page_header=page.Header;
			}
			if(page.Packets.Length==0)
			{
				return;
			}
			ByteVector[]page_packets=page.Packets;
			for(int i=0;i<page_packets.Length;i++)
			{
				if((page.Header.Flags&PageFlags.FirstPacketContinued)!=0&&i==0&&packets.Count>0)
				{
					packets[packets.Count-1].Add(page_packets[0]);
				}
				else
				{
					packets.Add(page_packets[i]);
				}
			}
		}
		public void SetComment(XiphComment comment)
		{
			codec.SetCommentPacket(packets,comment);
		}
		[Obsolete("Use Paginator.Paginate(out int)")]
		public Page[]Paginate()
		{
			int dummy;
			return Paginate(out dummy);
		}
		public Page[]Paginate(out int change)
		{
			if(pages_read==0)
			{
				change=0;
				return new Page[0];
			}
			int count=pages_read;
			ByteVectorCollection packets=new ByteVectorCollection(this.packets);
			PageHeader first_header=(PageHeader)first_page_header;
			List<Page>pages=new List<Page>();
			uint index=0;
			bool bos=first_header.PageSequenceNumber==0;
			if(bos)
			{
				pages.Add(new Page(new ByteVectorCollection(packets[0]),first_header));
				index++;
				packets.RemoveAt(0);
				count--;
			}
			int lacing_per_page=0xfc;
			if(count>0)
			{
				int total_lacing_bytes=0;
				for(int i=0;i<packets.Count;i++)
				{
					total_lacing_bytes+=GetLacingValueLength(packets,i);
				}
				lacing_per_page=Math.Min(total_lacing_bytes/count+1,lacing_per_page);
			}
			int lacing_bytes_used=0;
			ByteVectorCollection page_packets=new ByteVectorCollection();
			bool first_packet_continued=false;
			while(packets.Count>0)
			{
				int packet_bytes=GetLacingValueLength(packets,0);
				int remaining=lacing_per_page-lacing_bytes_used;
				bool whole_packet=packet_bytes<=remaining;
				if(whole_packet)
				{
					page_packets.Add(packets[0]);
					lacing_bytes_used+=packet_bytes;
					packets.RemoveAt(0);
				}
				else
				{
					page_packets.Add(packets[0].Mid(0,remaining*0xff));
					packets[0]=packets[0].Mid(remaining*0xff);
					lacing_bytes_used+=remaining;
				}
				if(lacing_bytes_used==lacing_per_page)
				{
					pages.Add(new Page(page_packets,new PageHeader(first_header,index,first_packet_continued?PageFlags.FirstPacketContinued:PageFlags.None)));
					page_packets=new ByteVectorCollection();
					lacing_bytes_used=0;
					index++;
					count--;
					first_packet_continued= !whole_packet;
				}
			}
			if(page_packets.Count>0)
			{
				pages.Add(new Page(page_packets,new PageHeader(first_header.StreamSerialNumber,index,first_packet_continued?PageFlags.FirstPacketContinued:PageFlags.None)));
				index++;
				count--;
			}
			change= -count;
			return pages.ToArray();
		}
#endregion
#region Private Methods
		private static int GetLacingValueLength(ByteVectorCollection packets,int index)
		{
			int size=packets[index].Count;
			return size/0xff+((index+1<packets.Count||size%0xff>0)?1:0);
		}
#endregion
	}
}
