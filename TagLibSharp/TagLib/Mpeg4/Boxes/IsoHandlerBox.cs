using System;
namespace TagLib.Mpeg4
{
	public class IsoHandlerBox:
	FullBox
	{
#region Private Fields
		private ByteVector handler_type;
		private string name;
#endregion
#region Constructors
		public IsoHandlerBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,file,handler)
		{
			if(file==null)
			{
				throw new System.ArgumentNullException("file");
			}
			file.Seek(DataPosition+4);
			ByteVector box_data=file.ReadBlock(DataSize-4);
			handler_type=box_data.Mid(0,4);
			int end=box_data.Find((byte)0,16);
			if(end<16)
			{
				end=box_data.Count;
			}
			name=box_data.ToString(StringType.UTF8,16,end-16);
		}
		public IsoHandlerBox(ByteVector handlerType,string name):
		base("hdlr",0,0)
		{
			if(handlerType==null)
			{
				throw new ArgumentNullException("handlerType");
			}
			if(handlerType.Count<4)
			{
				throw new ArgumentException("The handler type must be four bytes long.","handlerType");
			}
			this.handler_type=handlerType.Mid(0,4);
			this.name=name;
		}
#endregion
#region Public Properties
		public override ByteVector Data
		{
			get
			{
				ByteVector output=new ByteVector(4);
				output.Add(handler_type);
				output.Add(new ByteVector(12));
				output.Add(ByteVector.FromString(name,StringType.UTF8));
				output.Add(new ByteVector(2));
				return output;
			}
		}
		public ByteVector HandlerType
		{
			get
			{
				return handler_type;
			}
		}
		public string Name
		{
			get
			{
				return name;
			}
		}
#endregion
	}
}
