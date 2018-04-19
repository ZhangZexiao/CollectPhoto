using System.Collections.Generic;
using System;
namespace TagLib.Asf
{
	public class HeaderExtensionObject:
	Object
	{
#region Private Fields
		private List<Object>children=new List<Object>();
#endregion
#region Constructors
		public HeaderExtensionObject(Asf.File file,long position):
		base(file,position)
		{
			if(!Guid.Equals(Asf.Guid.AsfHeaderExtensionObject))
			{
				throw new CorruptFileException("Object GUID incorrect.");
			}
			if(file.ReadGuid()!=Asf.Guid.AsfReserved1)
			{
				throw new CorruptFileException("Reserved1 GUID expected.");
			}
			if(file.ReadWord()!=6)
			{
				throw new CorruptFileException("Invalid reserved WORD. Expected '6'.");
			}
			uint size_remaining=file.ReadDWord();
			position+=0x170/8;
			while(size_remaining>0)
			{
				Object obj=file.ReadObject(position);
				position+=(long)obj.OriginalSize;
				size_remaining-=(uint)obj.OriginalSize;
				children.Add(obj);
			}
		}
#endregion
#region Public Properties
		public IEnumerable<Object>Children
		{
			get
			{
				return children;
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render()
		{
			ByteVector output=new ByteVector();
			foreach(Object child in children)
			output.Add(child.Render());
			output.Insert(0,RenderDWord((uint)output.Count));
			output.Insert(0,RenderWord(6));
			output.Insert(0,Asf.Guid.AsfReserved1.ToByteArray());
			return Render(output);
		}
		public void AddObject(Object obj)
		{
			children.Add(obj);
		}
		public void AddUniqueObject(Object obj)
		{
			for(int i=0;i<children.Count;i++)
			{
				if(((Object)children[i]).Guid==obj.Guid)
				{
					children[i]=obj;
					return;
				}
			}
			children.Add(obj);
		}
#endregion
	}
}
