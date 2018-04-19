using System.Collections;
using System;
namespace TagLib.Id3v2
{
	public class UniqueFileIdentifierFrame:
	Frame
	{
#region Private Fields
		private string owner=null;
		private ByteVector identifier=null;
#endregion
#region Constructors
		public UniqueFileIdentifierFrame(string owner,ByteVector identifier):
		base(FrameType.UFID,4)
		{
			if(owner==null)
			{
				throw new ArgumentNullException("owner");
			}
			this.owner=owner;
			this.identifier=identifier;
		}
		public UniqueFileIdentifierFrame(string owner):
		this(owner,null)
		{
		}
		public UniqueFileIdentifierFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal UniqueFileIdentifierFrame(ByteVector data,int offset,FrameHeader header,byte version):
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
		public ByteVector Identifier
		{
			get
			{
				return identifier;
			}
			set
			{
				identifier=value;
			}
		}
#endregion
#region Public Static Methods
		public static UniqueFileIdentifierFrame Get(Tag tag,string owner,bool create)
		{
			UniqueFileIdentifierFrame ufid;
			foreach(Frame frame in tag.GetFrames(FrameType.UFID))
			{
				ufid=frame as UniqueFileIdentifierFrame;
				if(ufid==null)
				{
					continue;
				}
				if(ufid.Owner==owner)
				{
					return ufid;
				}
			}
			if(!create)
			{
				return null;
			}
			ufid=new UniqueFileIdentifierFrame(owner,null);
			tag.AddFrame(ufid);
			return ufid;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			ByteVectorCollection fields=ByteVectorCollection.Split(data,(byte)0);
			if(fields.Count!=2)
			{
				return;
			}
			owner=fields[0].ToString(StringType.Latin1);
			identifier=fields[1];
		}
		protected override ByteVector RenderFields(byte version)
		{
			ByteVector data=new ByteVector();
			data.Add(ByteVector.FromString(owner,StringType.Latin1));
			data.Add(ByteVector.TextDelimiter(StringType.Latin1));
			data.Add(identifier);
			return data;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			UniqueFileIdentifierFrame frame=new UniqueFileIdentifierFrame(owner);
			if(identifier!=null)
			{
				frame.identifier=new ByteVector(identifier);
			}
			return frame;
		}
#endregion
	}
}
