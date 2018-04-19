using System;
namespace TagLib.Asf
{
	public class ContentDescriptionObject:
	Object
	{
#region Private Fields
		private string title=string.Empty;
		private string author=string.Empty;
		private string copyright=string.Empty;
		private string description=string.Empty;
		private string rating=string.Empty;
#endregion
#region Constructors
		public ContentDescriptionObject(Asf.File file,long position):
		base(file,position)
		{
			if(Guid!=Asf.Guid.AsfContentDescriptionObject)
			{
				throw new CorruptFileException("Object GUID incorrect.");
			}
			if(OriginalSize<34)
			{
				throw new CorruptFileException("Object size too small.");
			}
			ushort title_length=file.ReadWord();
			ushort author_length=file.ReadWord();
			ushort copyright_length=file.ReadWord();
			ushort description_length=file.ReadWord();
			ushort rating_length=file.ReadWord();
			title=file.ReadUnicode(title_length);
			author=file.ReadUnicode(author_length);
			copyright=file.ReadUnicode(copyright_length);
			description=file.ReadUnicode(description_length);
			rating=file.ReadUnicode(rating_length);
		}
		public ContentDescriptionObject():
		base(Asf.Guid.AsfContentDescriptionObject)
		{
		}
#endregion
#region Public Region
		public string Title
		{
			get
			{
				return title.Length==0?null:title;
			}
			set
			{
				title=string.IsNullOrEmpty(value)?string.Empty:value;
			}
		}
		public string Author
		{
			get
			{
				return author.Length==0?null:author;
			}
			set
			{
				author=string.IsNullOrEmpty(value)?string.Empty:value;
			}
		}
		public string Copyright
		{
			get
			{
				return copyright.Length==0?null:copyright;
			}
			set
			{
				copyright=string.IsNullOrEmpty(value)?string.Empty:value;
			}
		}
		public string Description
		{
			get
			{
				return description.Length==0?null:description;
			}
			set
			{
				description=string.IsNullOrEmpty(value)?string.Empty:value;
			}
		}
		public string Rating
		{
			get
			{
				return rating.Length==0?null:rating;
			}
			set
			{
				rating=string.IsNullOrEmpty(value)?string.Empty:value;
			}
		}
		public bool IsEmpty
		{
			get
			{
				return title.Length==0&&author.Length==0&&copyright.Length==0&&description.Length==0&&rating.Length==0;
			}
		}
#endregion
#region Public Region
		public override ByteVector Render()
		{
			ByteVector title_bytes=RenderUnicode(title);
			ByteVector author_bytes=RenderUnicode(author);
			ByteVector copyright_bytes=RenderUnicode(copyright);
			ByteVector description_bytes=RenderUnicode(description);
			ByteVector rating_bytes=RenderUnicode(rating);
			ByteVector output=RenderWord((ushort)title_bytes.Count);
			output.Add(RenderWord((ushort)author_bytes.Count));
			output.Add(RenderWord((ushort)copyright_bytes.Count));
			output.Add(RenderWord((ushort)description_bytes.Count));
			output.Add(RenderWord((ushort)rating_bytes.Count));
			output.Add(title_bytes);
			output.Add(author_bytes);
			output.Add(copyright_bytes);
			output.Add(description_bytes);
			output.Add(rating_bytes);
			return Render(output);
		}
#endregion
	}
}
