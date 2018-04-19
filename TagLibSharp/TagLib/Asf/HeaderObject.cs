using System;
using System.Collections.Generic;
namespace TagLib.Asf
{
	public class HeaderObject:
	Object
	{
#region Private Fields
		private ByteVector reserved;
		private List<Object>children;
#endregion
#region Constructors
		public HeaderObject(Asf.File file,long position):
		base(file,position)
		{
			if(!Guid.Equals(Asf.Guid.AsfHeaderObject))
			{
				throw new CorruptFileException("Object GUID incorrect.");
			}
			if(OriginalSize<26)
			{
				throw new CorruptFileException("Object size too small.");
			}
			children=new List<Object>();
			uint child_count=file.ReadDWord();
			reserved=file.ReadBlock(2);
			children.AddRange(file.ReadObjects(child_count,file.Tell));
		}
#endregion
#region Public Properties
		public HeaderExtensionObject Extension
		{
			get
			{
				foreach(Object child in children)
				if(child is HeaderExtensionObject)
				{
					return child as HeaderExtensionObject;
				}
				return null;
			}
		}
		public IEnumerable<Object>Children
		{
			get
			{
				return children;
			}
		}
		public Properties Properties
		{
			get
			{
				TimeSpan duration=TimeSpan.Zero;
				List<ICodec>codecs=new List<ICodec>();
				foreach(Object obj in Children)
				{
					FilePropertiesObject fpobj=obj as FilePropertiesObject;
					if(fpobj!=null)
					{
						duration=fpobj.PlayDuration-new TimeSpan((long)fpobj.Preroll);
						continue;
					}
					StreamPropertiesObject spobj=obj as StreamPropertiesObject;
					if(spobj!=null)
					{
						codecs.Add(spobj.Codec);
						continue;
					}
				}
				return new Properties(duration,codecs);
			}
		}
		public bool HasContentDescriptors
		{
			get
			{
				foreach(Asf.Object child in children)
				if(child.Guid==Asf.Guid.AsfContentDescriptionObject||child.Guid==Asf.Guid.AsfExtendedContentDescriptionObject)
				{
					return true;
				}
				return false;
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render()
		{
			ByteVector output=new ByteVector();
			uint child_count=0;
			foreach(Object child in children)
			if(child.Guid!=Asf.Guid.AsfPaddingObject)
			{
				output.Add(child.Render());
				child_count++;
			}
			long size_diff=(long)output.Count+30-(long)OriginalSize;
			if(size_diff!=0)
			{
				PaddingObject obj=new PaddingObject((uint)(size_diff>0?4096: -size_diff));
				output.Add(obj.Render());
				child_count++;
			}
			output.Insert(0,reserved);
			output.Insert(0,RenderDWord(child_count));
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
				if(children[i].Guid==obj.Guid)
				{
					children[i]=obj;
					return;
				}
			}
			children.Add(obj);
		}
		public void RemoveContentDescriptors()
		{
			for(int i=children.Count-1;i>=0;i--)
			{
				if(children[i].Guid==Asf.Guid.AsfContentDescriptionObject||children[i].Guid==Asf.Guid.AsfExtendedContentDescriptionObject)
				{
					children.RemoveAt(i);
				}
			}
		}
#endregion
	}
}
