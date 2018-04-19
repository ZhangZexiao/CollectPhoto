using System;
namespace TagLib.NonContainer
{
	public class StartTag:
	CombinedTag
	{
#region Private Fields
		private TagLib.File file;
		int read_size=(int)Math.Max(TagLib.Ape.Footer.Size,TagLib.Id3v2.Header.Size);
#endregion
#region Constructors
		public StartTag(TagLib.File file):
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
				long size=0;
				while(ReadTagInfo(ref size)!=TagTypes.None);
				return size;
			}
		}
#endregion
#region Public Methods
		public long Read()
		{
			TagLib.Tag tag;
			ClearTags();
			long end=0;
			while((tag=ReadTag(ref end))!=null)AddTag(tag);
			return end;
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
			}
			return data;
		}
		public long Write()
		{
			ByteVector data=Render();
			file.Insert(data,0,TotalSize);
			return data.Count;
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
			if(type==TagTypes.Id3v2)
			{
				tag=new TagLib.Id3v2.Tag();
			}
			else if(type==TagTypes.Ape)
			{
				tag=new TagLib.Ape.Tag();
				(tag as Ape.Tag).HeaderPresent=true;
			}
			if(tag!=null)
			{
				if(copy!=null)
				{
					copy.CopyTo(tag,true);
				}
				AddTag(tag);
			}
			return tag;
		}
#endregion
#region Private Methods
		private TagLib.Tag ReadTag(ref long start)
		{
			long end=start;
			TagTypes type=ReadTagInfo(ref end);
			TagLib.Tag tag=null;
			switch(type)
			{
			case TagTypes.Ape:
				tag=new TagLib.Ape.Tag(file,start);
				break;
			case TagTypes.Id3v2:
				tag=new TagLib.Id3v2.Tag(file,start);
				break;
			}
			start=end;
			return tag;
		}
		private TagTypes ReadTagInfo(ref long position)
		{
			file.Seek(position);
			ByteVector data=file.ReadBlock(read_size);
			try
			{
				if(data.StartsWith(TagLib.Ape.Footer.FileIdentifier))
				{
					TagLib.Ape.Footer footer=new TagLib.Ape.Footer(data);
					position+=footer.CompleteTagSize;
					return TagTypes.Ape;
				}
				if(data.StartsWith(TagLib.Id3v2.Header.FileIdentifier))
				{
					TagLib.Id3v2.Header header=new TagLib.Id3v2.Header(data);
					position+=header.CompleteTagSize;
					return TagTypes.Id3v2;
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
