using System;
using System.Collections.Generic;
using TagLib.IFD.Entries;
namespace TagLib.IFD
{
	public class IFDRenderer
	{
#region Private Fields
		private readonly IFDStructure structure;
		private readonly bool is_bigendian;
		private readonly uint ifd_offset;
#endregion
#region Constructors
		public IFDRenderer(bool is_bigendian,IFDStructure structure,uint ifd_offset)
		{
			this.is_bigendian=is_bigendian;
			this.structure=structure;
			this.ifd_offset=ifd_offset;
		}
#endregion
#region Public Methods
		public ByteVector Render()
		{
			ByteVector ifd_data=new ByteVector();
			uint current_offset=ifd_offset;
			var directories=structure.directories;
			for(int index=0;index<directories.Count;index++)
			{
				ByteVector data=RenderIFD(directories[index],current_offset,index==directories.Count-1);
				current_offset+=(uint)data.Count;
				ifd_data.Add(data);
			}
			return ifd_data;
		}
#endregion
#region Private Methods
		private ByteVector RenderIFD(IFDDirectory directory,uint ifd_offset,bool last)
		{
			if(directory.Count>(int)UInt16.MaxValue)
			{
				throw new Exception(String.Format("Directory has too much entries: {0}",directory.Count));
			}
			var tags=new List<ushort>(directory.Keys);
			foreach(var tag in tags)
			{
				var entry=directory[tag];
				if(entry is SubIFDEntry&&(entry as SubIFDEntry).ChildCount==0)
				{
					directory.Remove(tag);
				}
			}
			ushort entry_count=(ushort)directory.Count;
			uint data_offset=ifd_offset+2+12*(uint)entry_count+4;
			ByteVector entry_data=new ByteVector();
			ByteVector offset_data=new ByteVector();
			entry_data.Add(ByteVector.FromUShort(entry_count,is_bigendian));
			foreach(IFDEntry entry in directory.Values)
			RenderEntryData(entry,entry_data,offset_data,data_offset);
			if(last)
			{
				entry_data.Add("\0\0\0\0");
			}
			else
			{
				entry_data.Add(ByteVector.FromUInt((uint)(data_offset+offset_data.Count),is_bigendian));
			}
			if(data_offset-ifd_offset!=entry_data.Count)
			{
				throw new Exception(String.Format("Expected IFD data size was {0} but is {1}",data_offset-ifd_offset,entry_data.Count));
			}
			entry_data.Add(offset_data);
			return entry_data;
		}
#endregion
#region Protected Methods
		protected void RenderEntry(ByteVector entry_data,ushort tag,ushort type,uint count,uint offset)
		{
			entry_data.Add(ByteVector.FromUShort(tag,is_bigendian));
			entry_data.Add(ByteVector.FromUShort(type,is_bigendian));
			entry_data.Add(ByteVector.FromUInt(count,is_bigendian));
			entry_data.Add(ByteVector.FromUInt(offset,is_bigendian));
		}
		protected virtual void RenderEntryData(IFDEntry entry,ByteVector entry_data,ByteVector offset_data,uint data_offset)
		{
			ushort tag=(ushort)entry.Tag;
			uint offset=(uint)(data_offset+offset_data.Count);
			ushort type;
			uint count;
			ByteVector data=entry.Render(is_bigendian,offset,out type,out count);
			if(data.Count<=4)
			{
				while(data.Count<4)data.Add("\0");
				offset=data.ToUInt(is_bigendian);
				data=null;
			}
			if(data!=null&&data.Count%2!=0)
			{
				data.Add("\0");
			}
			RenderEntry(entry_data,tag,type,count,offset);
			offset_data.Add(data);
		}
		protected virtual IFDRenderer CreateSubRenderer(bool is_bigendian,IFDStructure structure,uint ifd_offset)
		{
			return new IFDRenderer(is_bigendian,structure,ifd_offset);
		}
#endregion
	}
}
