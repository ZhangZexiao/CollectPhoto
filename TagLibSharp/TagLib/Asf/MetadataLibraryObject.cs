using System;
using System.Collections.Generic;
namespace TagLib.Asf
{
	public class MetadataLibraryObject:
	Object,IEnumerable<DescriptionRecord>
	{
#region Private Fields
		private List<DescriptionRecord>records=new List<DescriptionRecord>();
#endregion
#region Constructors
		public MetadataLibraryObject(Asf.File file,long position):
		base(file,position)
		{
			if(!Guid.Equals(Asf.Guid.AsfMetadataLibraryObject))
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
				DescriptionRecord rec=new DescriptionRecord(file);
				AddRecord(rec);
			}
		}
		public MetadataLibraryObject():
		base(Asf.Guid.AsfMetadataLibraryObject)
		{
		}
#endregion
#region Public Properties
		public bool IsEmpty
		{
			get
			{
				return records.Count==0;
			}
		}
#endregion
#region Public Methods
		public override ByteVector Render()
		{
			ByteVector output=new ByteVector();
			ushort count=0;
			foreach(DescriptionRecord rec in records)
			{
				count++;
				output.Add(rec.Render());
			}
			return Render(RenderWord(count)+output);
		}
		public void RemoveRecords(ushort languageListIndex,ushort streamNumber,string name)
		{
			for(int i=records.Count-1;i>=0;i--)
			{
				DescriptionRecord rec=records[i];
				if(rec.LanguageListIndex==languageListIndex&&rec.StreamNumber==streamNumber&&rec.Name==name)
				{
					records.RemoveAt(i);
				}
			}
		}
		public IEnumerable<DescriptionRecord>GetRecords(ushort languageListIndex,ushort streamNumber,params string[]names)
		{
			foreach(DescriptionRecord rec in records)
			{
				if(rec.LanguageListIndex!=languageListIndex||rec.StreamNumber!=streamNumber)
				{
					continue;
				}
				foreach(string name in names)
				if(rec.Name==name)
				{
					yield return rec;
				}
			}
		}
		public void AddRecord(DescriptionRecord record)
		{
			records.Add(record);
		}
		public void SetRecords(ushort languageListIndex,ushort streamNumber,string name,params DescriptionRecord[]records)
		{
			int position=this.records.Count;
			for(int i=this.records.Count-1;i>=0;i--)
			{
				DescriptionRecord rec=this.records[i];
				if(rec.LanguageListIndex==languageListIndex&&rec.StreamNumber==streamNumber&&rec.Name==name)
				{
					this.records.RemoveAt(i);
					position=i;
				}
			}
			this.records.InsertRange(position,records);
		}
#endregion
#region IEnumerable
		public IEnumerator<DescriptionRecord>GetEnumerator()
		{
			return records.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return records.GetEnumerator();
		}
#endregion
	}
}
