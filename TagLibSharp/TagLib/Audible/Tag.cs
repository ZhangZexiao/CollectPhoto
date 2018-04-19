using System;
using System.Collections.Generic;
namespace TagLib.Audible
{
	public class Tag:
	TagLib.Tag
	{
#region Private Fields
		private List<KeyValuePair<string,string>>tags;
#endregion
#region Constructors
		public Tag()
		{
			Clear();
		}
		public Tag(File file,long position)
		{
		}
		public Tag(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			Clear();
			Parse(data);
		}
#endregion
#region Private Methods
		private void Parse(ByteVector data)
		{
			String currentKey,currentValue;
			int keyLen,valueLen;
			try
			{
				do
				{
					keyLen=(int)data.ToUInt(true);
					data.RemoveRange(0,4);
					valueLen=(int)data.ToUInt(true);
					data.RemoveRange(0,4);
					currentKey=data.ToString(TagLib.StringType.UTF8,0,keyLen);
					data.RemoveRange(0,keyLen);
					currentValue=data.ToString(TagLib.StringType.UTF8,0,valueLen);
					data.RemoveRange(0,valueLen);
					tags.Add(new KeyValuePair<string,string>(currentKey,currentValue));
					if(data.Count!=0)
					{
						data.RemoveRange(0,1);
					}
				}
				while(data.Count>=4);
			}
			catch(Exception)
			{
			}
			if(data.Count!=0)
			{
				throw new CorruptFileException();
			}
		}
		void setTag(string tagName,string value)
		{
			for(int i=0;i<tags.Count;i++)
			{
				if(tags[i].Key==tagName)
				{
					tags[i]=new KeyValuePair<string,string>(tags[i].Key,value);
				}
			}
		}
		private string getTag(string tagName)
		{
			foreach(KeyValuePair<string,string>tag in tags)
			{
				if(tag.Key==tagName)
				{
					return tag.Value;
				}
			}
			return null;
		}
#endregion	
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.AudibleMetadata;
			}
		}
		public string Author
		{
			get
			{
				return getTag("author");
			}
		}
		public override string Copyright
		{
			get
			{
				return getTag("copyright");
			}
			set
			{
				setTag("copyright",value);
			}
		}
		public string Description
		{
			get
			{
				return getTag("description");
			}
		}
		public string Narrator
		{
			get
			{
				return getTag("narrator");
			}
		}
		public override string Title
		{
			get
			{
				return getTag("title");
			}
		}
		public override string Album
		{
			get
			{
				return getTag("provider");
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				String artist=getTag("provider");
				return string.IsNullOrEmpty(artist)?null:new string[]
				{
					artist
				};
			}
		}
		public override void Clear()
		{
			tags=new List<KeyValuePair<string,string>>();
		}
#endregion
	}
}
