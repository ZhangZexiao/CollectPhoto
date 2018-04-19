using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace TagLib
{
	[ComVisible(false)]
	public class ByteVectorCollection:ListBase<ByteVector>
	{
		public ByteVectorCollection()
		{
		}
		public ByteVectorCollection(IEnumerable<ByteVector>list)
		{
			if(list!=null)
			{
				Add(list);
			}
		}
		public ByteVectorCollection(params ByteVector[]list)
		{
			if(list!=null)
			{
				Add(list);
			}
		}
		public override void SortedInsert(ByteVector item,bool unique)
		{
			if(item==null)
			{
				throw new ArgumentNullException("item");
			}
			int i=0;
			for(;i<Count;i++)
			{
				if(item==this[i]&&unique)
				{
					return;
				}
				if(item>=this[i])
				{
					break;
				}
			}
			Insert(i+1,item);
		}
		public ByteVector ToByteVector(ByteVector separator)
		{
			if(separator==null)
			{
				throw new ArgumentNullException("separator");
			}
			ByteVector vector=new ByteVector();
			for(int i=0;i<Count;i++)
			{
				if(i!=0&&separator.Count>0)
				{
					vector.Add(separator);
				}
				vector.Add(this[i]);
			}
			return vector;
		}
		public static ByteVectorCollection Split(ByteVector vector,ByteVector pattern,int byteAlign,int max)
		{
			if(vector==null)
			{
				throw new ArgumentNullException("vector");
			}
			if(pattern==null)
			{
				throw new ArgumentNullException("pattern");
			}
			if(byteAlign<1)
			{
				throw new ArgumentOutOfRangeException("byteAlign","byteAlign must be at least 1.");
			}
			ByteVectorCollection list=new ByteVectorCollection();
			int previous_offset=0;
			for(int offset=vector.Find(pattern,0,byteAlign);offset!= -1&&(max<1||max>list.Count+1);offset=vector.Find(pattern,offset+pattern.Count,byteAlign))
			{
				list.Add(vector.Mid(previous_offset,offset-previous_offset));
				previous_offset=offset+pattern.Count;
			}
			if(previous_offset<vector.Count)
			{
				list.Add(vector.Mid(previous_offset,vector.Count-previous_offset));
			}
			return list;
		}
		public static ByteVectorCollection Split(ByteVector vector,ByteVector pattern,int byteAlign)
		{
			return Split(vector,pattern,byteAlign,0);
		}
		public static ByteVectorCollection Split(ByteVector vector,ByteVector pattern)
		{
			return Split(vector,pattern,1);
		}
	}
}
