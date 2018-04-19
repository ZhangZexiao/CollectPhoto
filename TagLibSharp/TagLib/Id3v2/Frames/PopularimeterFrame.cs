using System;
namespace TagLib.Id3v2
{
	public class PopularimeterFrame:
	Frame
	{
#region Private Properties
		private string user=string.Empty;
		private byte rating=0;
		private ulong play_count=0;
#endregion
#region Constructors
		public PopularimeterFrame(string user):
		base(FrameType.POPM,4)
		{
			User=user;
		}
		public PopularimeterFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal PopularimeterFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(header)
		{
			SetData(data,offset,version,false);
		}
#endregion
#region Public Properties
		public string User
		{
			get
			{
				return user;
			}
			set
			{
				user=value!=null?value:string.Empty;
			}
		}
		public byte Rating
		{
			get
			{
				return rating;
			}
			set
			{
				rating=value;
			}
		}
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
		public static PopularimeterFrame Get(Tag tag,string user,bool create)
		{
			PopularimeterFrame popm;
			foreach(Frame frame in tag)
			{
				popm=frame as PopularimeterFrame;
				if(popm!=null&&popm.user.Equals(user))
				{
					return popm;
				}
			}
			if(!create)
			{
				return null;
			}
			popm=new PopularimeterFrame(user);
			tag.AddFrame(popm);
			return popm;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			ByteVector delim=ByteVector.TextDelimiter(StringType.Latin1);
			int index=data.Find(delim);
			if(index<0)
			{
				throw new CorruptFileException("Popularimeter frame does not contain a text delimiter");
			}
			if(index+2>data.Count)
			{
				throw new CorruptFileException("Popularimeter is too short");
			}
			user=data.ToString(StringType.Latin1,0,index);
			rating=data[index+1];
			play_count=data.Mid(index+2).ToULong();
		}
		protected override ByteVector RenderFields(byte version)
		{
			ByteVector data=ByteVector.FromULong(play_count);
			while(data.Count>0&&data[0]==0)data.RemoveAt(0);
			data.Insert(0,rating);
			data.Insert(0,0);
			data.Insert(0,ByteVector.FromString(user,StringType.Latin1));
			return data;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			PopularimeterFrame frame=new PopularimeterFrame(user);
			frame.play_count=play_count;
			frame.rating=rating;
			return frame;
		}
#endregion
	}
}
