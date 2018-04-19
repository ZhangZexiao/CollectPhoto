using System;
namespace TagLib.Asf
{
	public class FilePropertiesObject:
	Object
	{
#region Private Fields
		private System.Guid file_id;
		private ulong file_size;
		private ulong creation_date;
		private ulong data_packets_count;
		private ulong play_duration;
		private ulong send_duration;
		private ulong preroll;
		private uint flags;
		private uint minimum_data_packet_size;
		private uint maximum_data_packet_size;
		private uint maximum_bitrate;
#endregion
#region Constructors
		public FilePropertiesObject(Asf.File file,long position):
		base(file,position)
		{
			if(!Guid.Equals(Asf.Guid.AsfFilePropertiesObject))
			{
				throw new CorruptFileException("Object GUID incorrect.");
			}
			if(OriginalSize<104)
			{
				throw new CorruptFileException("Object size too small.");
			}
			file_id=file.ReadGuid();
			file_size=file.ReadQWord();
			creation_date=file.ReadQWord();
			data_packets_count=file.ReadQWord();
			send_duration=file.ReadQWord();
			play_duration=file.ReadQWord();
			preroll=file.ReadQWord();
			flags=file.ReadDWord();
			minimum_data_packet_size=file.ReadDWord();
			maximum_data_packet_size=file.ReadDWord();
			maximum_bitrate=file.ReadDWord();
		}
#endregion
#region Public Properties
		public System.Guid FileId
		{
			get
			{
				return file_id;
			}
		}
		public ulong FileSize
		{
			get
			{
				return file_size;
			}
		}
		public DateTime CreationDate
		{
			get
			{
				return new DateTime((long)creation_date);
			}
		}
		public ulong DataPacketsCount
		{
			get
			{
				return data_packets_count;
			}
		}
		public TimeSpan PlayDuration
		{
			get
			{
				return new TimeSpan((long)play_duration);
			}
		}
		public TimeSpan SendDuration
		{
			get
			{
				return new TimeSpan((long)send_duration);
			}
		}
		public ulong Preroll
		{
			get
			{
				return preroll;
			}
		}
		public uint Flags
		{
			get
			{
				return flags;
			}
		}
		public uint MinimumDataPacketSize
		{
			get
			{
				return minimum_data_packet_size;
			}
		}
		public uint MaximumDataPacketSize
		{
			get
			{
				return maximum_data_packet_size;
			}
		}
		public uint MaximumBitrate
		{
			get
			{
				return maximum_bitrate;
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render()
		{
			ByteVector output=file_id.ToByteArray();
			output.Add(RenderQWord(file_size));
			output.Add(RenderQWord(creation_date));
			output.Add(RenderQWord(data_packets_count));
			output.Add(RenderQWord(send_duration));
			output.Add(RenderQWord(play_duration));
			output.Add(RenderQWord(preroll));
			output.Add(RenderDWord(flags));
			output.Add(RenderDWord(minimum_data_packet_size));
			output.Add(RenderDWord(maximum_data_packet_size));
			output.Add(RenderDWord(maximum_bitrate));
			return Render(output);
		}
#endregion
	}
}
