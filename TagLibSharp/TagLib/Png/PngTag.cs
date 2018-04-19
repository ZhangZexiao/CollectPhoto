using System;
using System.Collections;
using System.Collections.Generic;
using TagLib.Image;
namespace TagLib.Png
{
	public class PngTag:
	ImageTag,IEnumerable
	{
#region defined PNG keywords
		public static readonly string TITLE="Title";
		public static readonly string AUTHOR="Author";
		public static readonly string DESCRIPTION="Description";
		public static readonly string COPYRIGHT="Copyright";
		public static readonly string CREATION_TIME="Creation Time";
		public static readonly string SOFTWARE="Software";
		public static readonly string DISCLAIMER="Disclaimer";
		public static readonly string WARNING="Warning";
		public static readonly string SOURCE="Source";
		public static readonly string COMMENT="Comment";
#endregion
#region Private Fieds
		private Dictionary<string,string>keyword_store=new Dictionary<string,string>();
#endregion
#region Constructors
		public PngTag()
		{
		}
#endregion
#region Public Properties
		public override string Comment
		{
			get
			{
				string description=GetKeyword(DESCRIPTION);
				if(!String.IsNullOrEmpty(description))
				{
					return description;
				}
				return GetKeyword(COMMENT);
			}
			set
			{
				SetKeyword(DESCRIPTION,value);
				SetKeyword(COMMENT,value);
			}
		}
		public override string Title
		{
			get
			{
				return GetKeyword(TITLE);
			}
			set
			{
				SetKeyword(TITLE,value);
			}
		}
		public override string Creator
		{
			get
			{
				return GetKeyword(AUTHOR);
			}
			set
			{
				SetKeyword(AUTHOR,value);
			}
		}
		public override string Copyright
		{
			get
			{
				return GetKeyword(COPYRIGHT);
			}
			set
			{
				SetKeyword(COPYRIGHT,value);
			}
		}
		public override string Software
		{
			get
			{
				return GetKeyword(SOFTWARE);
			}
			set
			{
				SetKeyword(SOFTWARE,value);
			}
		}
		public override DateTime?DateTime
		{
			get
			{
				DateTime ret;
				string date=GetKeyword(CREATION_TIME);
				if(System.DateTime.TryParse(date,out ret))
				{
					return ret;
				}
				return null;
			}
			set
			{
				string date=null;
				if(value!=null)
				{
					date=value.Value.ToString("R");
				}
				SetKeyword(CREATION_TIME,date);
			}
		}
#endregion
#region Public Methods
		public void SetKeyword(string keyword,string value)
		{
			if(String.IsNullOrEmpty(keyword))
			{
				throw new ArgumentException("keyword is null or empty");
			}
			keyword_store.Remove(keyword);
			if(value!=null)
			{
				keyword_store.Add(keyword,value);
			}
		}
		public string GetKeyword(string keyword)
		{
			string ret=null;
			keyword_store.TryGetValue(keyword,out ret);
			return ret;
		}
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.Png;
			}
		}
		public override void Clear()
		{
			keyword_store.Clear();
		}
		public IEnumerator GetEnumerator()
		{
			return keyword_store.GetEnumerator();
		}
#endregion
	}
}
