using System;
using System.Collections.Generic;
namespace TagLib
{
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=true)]
	public sealed class SupportedMimeType:Attribute
	{
		private static List<SupportedMimeType>mimetypes=new List<SupportedMimeType>();
		private string mimetype;
		private string extension;
		static SupportedMimeType()
		{
			FileTypes.Init();
		}
		public SupportedMimeType(string mimetype)
		{
			this.mimetype=mimetype;
			mimetypes.Add(this);
		}
		public SupportedMimeType(string mimetype,string extension):
		this(mimetype)
		{
			this.extension=extension;
		}
		public string MimeType
		{
			get
			{
				return mimetype;
			}
		}
		public string Extension
		{
			get
			{
				return extension;
			}
		}
		public static IEnumerable<string>AllMimeTypes
		{
			get
			{
				foreach(SupportedMimeType type in mimetypes)
				yield return type.MimeType;
			}
		}
		public static IEnumerable<string>AllExtensions
		{
			get
			{
				foreach(SupportedMimeType type in mimetypes)
				if(type.Extension!=null)
				{
					yield return type.Extension;
				}
			}
		}
	}
}
