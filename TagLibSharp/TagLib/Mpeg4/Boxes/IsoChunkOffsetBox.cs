using System;
namespace TagLib.Mpeg4
{
	public class IsoChunkOffsetBox:
	FullBox
	{
#region Private Fields
		private uint[]offsets;
#endregion
#region Constructors
		public IsoChunkOffsetBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,file,handler)
		{
			ByteVector box_data=file.ReadBlock(DataSize);
			offsets=new uint[(int)box_data.Mid(0,4).ToUInt()];
			for(int i=0;i<offsets.Length;i++)
			{
				offsets[i]=box_data.Mid(4+i*4,4).ToUInt();
			}
		}
#endregion
#region Public Properties
		public override ByteVector Data
		{
			get
			{
				ByteVector output=ByteVector.FromUInt((uint)offsets.Length);
				for(int i=0;i<offsets.Length;i++)
				{
					output.Add(ByteVector.FromUInt(offsets[i]));
				}
				return output;
			}
		}
		public uint[]Offsets
		{
			get
			{
				return offsets;
			}
		}
#endregion
#region Public Methods
		public void Overwrite(File file,long sizeDifference,long after)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			file.Insert(Render(sizeDifference,after),Header.Position,Size);
		}
		public ByteVector Render(long sizeDifference,long after)
		{
			for(int i=0;i<offsets.Length;i++)
			{
				if(offsets[i]>=(uint)after)
				{
					offsets[i]=(uint)(offsets[i]+sizeDifference);
				}
			}
			return Render();
		}
#endregion
	}
}
