using System;
namespace TagLib.Riff
{
	public abstract class ListTag:
	Tag
	{
#region Private Fields
		List fields;
#endregion
#region Constructors
		protected ListTag()
		{
			fields=new List();
		}
		protected ListTag(List fields)
		{
			if(fields==null)
			{
				throw new ArgumentNullException("fields");
			}
			this.fields=fields;
		}
		protected ListTag(ByteVector data)
		{
			fields=new List(data);
		}
		protected ListTag(TagLib.File file,long position,int length)
		{
			if(file==null)
			{
				throw new System.ArgumentNullException("file");
			}
			if(length<0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if(position<0||position>file.Length-length)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			fields=new List(file,position,length);
		}
#endregion
#region Public Methods
		public abstract ByteVector RenderEnclosed();
		protected ByteVector RenderEnclosed(ByteVector id)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			return fields.RenderEnclosed(id);
		}
		public ByteVector Render()
		{
			return fields.Render();
		}
		public ByteVectorCollection GetValues(ByteVector id)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			return fields.GetValues(id);
		}
		public string[]GetValuesAsStrings(ByteVector id)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			return fields.GetValuesAsStrings(id);
		}
		[Obsolete("Use GetValuesAsStrings(ByteVector)")]
		public StringCollection GetValuesAsStringCollection(ByteVector id)
		{
			return new StringCollection(fields.GetValuesAsStrings(id));
		}
		public uint GetValueAsUInt(ByteVector id)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			return fields.GetValueAsUInt(id);
		}
		public void SetValue(ByteVector id,params ByteVector[]value)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			fields.SetValue(id,value);
		}
		public void SetValue(ByteVector id,ByteVectorCollection value)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			fields.SetValue(id,value);
		}
		public void SetValue(ByteVector id,uint value)
		{
			fields.SetValue(id,value);
		}
		[Obsolete("Use SetValue(ByteVector,string[])")]
		public void SetValue(ByteVector id,StringCollection value)
		{
			fields.SetValue(id,value);
		}
		public void SetValue(ByteVector id,params string[]value)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			fields.SetValue(id,value);
		}
		public void RemoveValue(ByteVector id)
		{
			if(id==null)
			{
				throw new ArgumentNullException("id");
			}
			if(id.Count!=4)
			{
				throw new ArgumentException("ID must be 4 bytes long.","id");
			}
			fields.RemoveValue(id);
		}
#endregion
#region TagLib.Tag
		public override bool IsEmpty
		{
			get
			{
				return fields.Count==0;
			}
		}
		public override void Clear()
		{
			fields.Clear();
		}
#endregion
	}
}
