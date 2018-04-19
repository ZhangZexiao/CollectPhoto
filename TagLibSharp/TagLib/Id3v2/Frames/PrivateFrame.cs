using System.Collections;
using System;
namespace TagLib.Id3v2
{
	public class PrivateFrame:
	Frame
	{
#region Private Properties
		private string owner=null;
		private ByteVector data=null;
#endregion
#region Constructors
		public PrivateFrame(string owner,ByteVector data):
		base(FrameType.PRIV,4)
		{
			this.owner=owner;
			this.data=data;
		}
		public PrivateFrame(string owner):
		this(owner,null)
		{
		}
		public PrivateFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal PrivateFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(header)
		{
			SetData(data,offset,version,false);
		}
#endregion
#region Public Properties
		public string Owner
		{
			get
			{
				return owner;
			}
		}
		public ByteVector PrivateData
		{
			get
			{
				return data;
			}
			set
			{
				data=value;
			}
		}
#endregion
#region Public Static Methods
		public static PrivateFrame Get(Tag tag,string owner,bool create)
		{
			PrivateFrame priv;
			foreach(Frame frame in tag.GetFrames(FrameType.PRIV))
			{
				priv=frame as PrivateFrame;
				if(priv!=null&&priv.Owner==owner)
				{
					return priv;
				}
			}
			if(!create)
			{
				return null;
			}
			priv=new PrivateFrame(owner);
			tag.AddFrame(priv);
			return priv;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			if(data.Count<1)
			{
				throw new CorruptFileException("A private frame must contain at least 1 byte.");
			}
			ByteVectorCollection l=ByteVectorCollection.Split(data,ByteVector.TextDelimiter(StringType.Latin1),1,2);
			if(l.Count==2)
			{
				this.owner=l[0].ToString(StringType.Latin1);
				this.data=l[1];
			}
		}
		protected override ByteVector RenderFields(byte version)
		{
			if(version<3)
			{
				throw new NotImplementedException();
			}
			ByteVector v=new ByteVector();
			v.Add(ByteVector.FromString(owner,StringType.Latin1));
			v.Add(ByteVector.TextDelimiter(StringType.Latin1));
			v.Add(data);
			return v;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			PrivateFrame frame=new PrivateFrame(owner);
			if(data!=null)
			{
				frame.data=new ByteVector(data);
			}
			return frame;
		}
#endregion
	}
}
