using System;
namespace TagLib.Id3v2
{
	public class PlayCountFrame:
	Frame
	{
#region Private Properties
		private ulong play_count=0;
#endregion
#region Constructors
		public PlayCountFrame():
		base(FrameType.PCNT,4)
		{
		}
		public PlayCountFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal PlayCountFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(header)
		{
			SetData(data,offset,version,false);
		}
#endregion
#region Public Properties
		public ulong PlayCount
		{
			get
			{
				return play_count;
			}
			set
			{
				play_count=value;
			}
		}
#endregion
#region Public Static Methods
		public static PlayCountFrame Get(Tag tag,bool create)
		{
			PlayCountFrame pcnt;
			foreach(Frame frame in tag)
			{
				pcnt=frame as PlayCountFrame;
				if(pcnt!=null)
				{
					return pcnt;
				}
			}
			if(!create)
			{
				return null;
			}
			pcnt=new PlayCountFrame();
			tag.AddFrame(pcnt);
			return pcnt;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			play_count=data.ToULong();
		}
		protected override ByteVector RenderFields(byte version)
		{
			ByteVector data=ByteVector.FromULong(play_count);
			while(data.Count>4&&data[0]==0)data.RemoveAt(0);
			return data;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			PlayCountFrame frame=new PlayCountFrame();
			frame.play_count=play_count;
			return frame;
		}
#endregion
	}
}
