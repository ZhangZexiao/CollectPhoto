using System.Collections;
using System;
namespace TagLib.Mpeg
{
	public struct VBRIHeader
	{
#region Private Fields
		private uint frames;
		private uint size;
		private bool present;
#endregion
#region Public Fields
		public static readonly ReadOnlyByteVector FileIdentifier="VBRI";
		public static readonly VBRIHeader Unknown=new VBRIHeader(0,0);
#endregion
#region Constructors
		private VBRIHeader(uint frame,uint size)
		{
			this.frames=frame;
			this.size=size;
			this.present=false;
		}
		public VBRIHeader(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(!data.StartsWith(FileIdentifier))
			{
				throw new CorruptFileException("Not a valid VBRI header");
			}
			int position=10;
			size=data.Mid(position,4).ToUInt();
			position+=4;
			frames=data.Mid(position,4).ToUInt();
			position+=4;
			present=true;
		}
#endregion
#region Public Properties
		public uint TotalFrames
		{
			get
			{
				return frames;
			}
		}
		public uint TotalSize
		{
			get
			{
				return size;
			}
		}
		public bool Present
		{
			get
			{
				return present;
			}
		}
#endregion
#region Public Static Methods
		public static int VBRIHeaderOffset()
		{
			return 0x24;
		}
#endregion
	}
}
