using System;
namespace TagLib.NonContainer
{
	public class EndTag:
	CombinedTag
	{
#region Private Fields
		private TagLib.File file;
		private static int read_size=(int)Math.Max(Math.Max(TagLib.Ape.Footer.Size,TagLib.Id3v2.Footer.Size),TagLib.Id3v1.Tag.Size);
#endregion
#region Constructors
		public EndTag(TagLib.File file):
		base()
		{
			this.file=file;
		}
#endregion
#region Public Properties
		public long TotalSize
		{
			get
			{
				long start=file.Length;
				while(ReadTagInfo(ref start)!=TagTypes.None);
				return file.Length-start;
			}
		}
#endregion
#region Public Methods
		public long Read()
		{
			TagLib.Tag tag;
			ClearTags();
			long start=file.Length;
			while((tag=ReadTag(ref start))!=null)InsertTag(0,tag);
			return start;
		}
		public ByteVector Render()
		{
			ByteVector data=new ByteVector();
			foreach(TagLib.Tag t in Tags)
			{
				if(t is TagLib.Ape.Tag)
				{
					data.Add((t as TagLib.Ape.Tag).Render());
				}
				else if(t is TagLib.Id3v2.Tag)
				{
					data.Add((t as TagLib.Id3v2.Tag).Render());
				}
				else if(t is TagLib.Id3v1.Tag)
				{
					data.Add((t as TagLib.Id3v1.Tag).Render());
				}
			}
			return data;
		}
		public long Write()
		{
			long total_size=TotalSize;
			ByteVector data=Render();
			file.Insert(data,file.Length-total_size,total_size);
			return file.Length-data.Count;
		}
		public void RemoveTags(TagTypes types)
		{
			for(int i=Tags.Length-1;i>=0;i--)
			{
				var tag=Tags[i];
				if(types==TagTypes.AllTags||(tag.TagTypes&types)==tag.TagTypes)
				{
					RemoveTag(tag);
				}
			}
		}
		public TagLib.Tag AddTag(TagTypes type,TagLib.Tag copy)
		{
			TagLib.Tag tag=null;
			if(type==TagTypes.Id3v1)
			{
				tag=new TagLib.Id3v1.Tag();
			}
			else if(type==TagTypes.Id3v2)
			{
				Id3v2.Tag tag32=new Id3v2.Tag();
				tag32.Version=4;
				tag32.Flags|=Id3v2.HeaderFlags.FooterPresent;
				tag=tag32;
			}
			else if(type==TagTypes.Ape)
			{
				tag=new TagLib.Ape.Tag();
			}
			if(tag!=null)
			{
				if(copy!=null)
				{
					copy.CopyTo(tag,true);
				}
				if(type==TagTypes.Id3v1)
				{
					AddTag(tag);
				}
				else
				{
					InsertTag(0,tag);
				}
			}
			return tag;
		}
#endregion
#region Private Methods
		private TagLib.Tag ReadTag(ref long end)
		{
			long start=end;
			TagTypes type=ReadTagInfo(ref start);
			TagLib.Tag tag=null;
			try
			{
				switch(type)
				{
				case TagTypes.Ape:
					tag=new TagLib.Ape.Tag(file,end-TagLib.Ape.Footer.Size);
					break;
				case TagTypes.Id3v2:
					tag=new TagLib.Id3v2.Tag(file,start);
					break;
				case TagTypes.Id3v1:
					tag=new TagLib.Id3v1.Tag(file,start);
					break;
				}
				end=start;
			}
			catch(CorruptFileException)
			{
			}
			return tag;
		}
		private TagTypes ReadTagInfo(ref long position)
		{
			if(position-read_size<0)
			{
				return TagTypes.None;
			}
			file.Seek(position-read_size);
			ByteVector data=file.ReadBlock(read_size);
			try
			{
				int offset=(int)(data.Count-TagLib.Ape.Footer.Size);
				if(data.ContainsAt(TagLib.Ape.Footer.FileIdentifier,offset))
				{
					TagLib.Ape.Footer footer=new TagLib.Ape.Footer(data.Mid(offset));
					if(footer.CompleteTagSize==0||(footer.Flags&TagLib.Ape.FooterFlags.IsHeader)!=0)
					{
						return TagTypes.None;
					}
					position-=footer.CompleteTagSize;
					return TagTypes.Ape;
				}
				offset=(int)(data.Count-TagLib.Id3v2.Footer.Size);
				if(data.ContainsAt(TagLib.Id3v2.Footer.FileIdentifier,offset))
				{
					TagLib.Id3v2.Footer footer=new TagLib.Id3v2.Footer(data.Mid(offset));
					position-=footer.CompleteTagSize;
					return TagTypes.Id3v2;
				}
				if(data.StartsWith(TagLib.Id3v1.Tag.FileIdentifier))
				{
					position-=TagLib.Id3v1.Tag.Size;
					return TagTypes.Id3v1;
				}
			}
			catch(CorruptFileException)
			{
			}
			return TagTypes.None;
		}
#endregion
	}
}
