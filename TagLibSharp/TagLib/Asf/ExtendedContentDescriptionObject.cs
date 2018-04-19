using System;
using System.Collections.Generic;
namespace TagLib.Asf
{
	public class ExtendedContentDescriptionObject:
	Object,IEnumerable<ContentDescriptor>
	{
#region Private Fields
		private List<ContentDescriptor>descriptors=new List<ContentDescriptor>();
#endregion
#region Constructors
		public ExtendedContentDescriptionObject(Asf.File file,long position):
		base(file,position)
		{
			if(!Guid.Equals(Asf.Guid.AsfExtendedContentDescriptionObject))
			{
				throw new CorruptFileException("Object GUID incorrect.");
			}
			if(OriginalSize<26)
			{
				throw new CorruptFileException("Object size too small.");
			}
			ushort count=file.ReadWord();
			for(ushort i=0;i<count;i++)
			{
				AddDescriptor(new ContentDescriptor(file));
			}
		}
		public ExtendedContentDescriptionObject():
		base(Asf.Guid.AsfExtendedContentDescriptionObject)
		{
		}
#endregion
#region Public Properties
		public bool IsEmpty
		{
			get
			{
				return descriptors.Count==0;
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render()
		{
			ByteVector output=new ByteVector();
			ushort count=0;
			foreach(ContentDescriptor desc in descriptors)
			{
				count++;
				output.Add(desc.Render());
			}
			return Render(RenderWord(count)+output);
		}
		public void RemoveDescriptors(string name)
		{
			for(int i=descriptors.Count-1;i>=0;i--)
			{
				if(name==descriptors[i].Name)
				{
					descriptors.RemoveAt(i);
				}
			}
		}
		public IEnumerable<ContentDescriptor>GetDescriptors(params string[]names)
		{
			if(names==null)
			{
				throw new ArgumentNullException("names");
			}
			foreach(string name in names)
			foreach(ContentDescriptor desc in descriptors)
			if(desc.Name==name)
			{
				yield return desc;
			}
		}
		public void AddDescriptor(ContentDescriptor descriptor)
		{
			if(descriptor==null)
			{
				throw new ArgumentNullException("descriptor");
			}
			descriptors.Add(descriptor);
		}
		public void SetDescriptors(string name,params ContentDescriptor[]descriptors)
		{
			if(name==null)
			{
				throw new ArgumentNullException("name");
			}
			int position=this.descriptors.Count;
			for(int i=this.descriptors.Count-1;i>=0;i--)
			{
				if(name==this.descriptors[i].Name)
				{
					this.descriptors.RemoveAt(i);
					position=i;
				}
			}
			this.descriptors.InsertRange(position,descriptors);
		}
#endregion
#region IEnumerable
		public IEnumerator<ContentDescriptor>GetEnumerator()
		{
			return descriptors.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return descriptors.GetEnumerator();
		}
#endregion
	}
}
