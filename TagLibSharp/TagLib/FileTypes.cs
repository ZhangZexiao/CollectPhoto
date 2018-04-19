using System;
using System.Collections.Generic;
namespace TagLib
{
	public static class FileTypes
	{
		private static Dictionary<string,Type>file_types;
		private static Type[]static_file_types=new Type[]
		{
			typeof(TagLib.Aac.File),
			typeof(TagLib.Aiff.File),
			typeof(TagLib.Ape.File),
			typeof(TagLib.Asf.File),
			typeof(TagLib.Audible.File),
			typeof(TagLib.Dsf.File),
			typeof(TagLib.Flac.File),
			typeof(TagLib.Matroska.File),
			typeof(TagLib.Gif.File),
			typeof(TagLib.Image.NoMetadata.File),
			typeof(TagLib.Jpeg.File),
			typeof(TagLib.Mpeg4.File),
			typeof(TagLib.Mpeg.AudioFile),
			typeof(TagLib.Mpeg.File),
			typeof(TagLib.MusePack.File),
			typeof(TagLib.Ogg.File),
			typeof(TagLib.Png.File),
			typeof(TagLib.Riff.File),
			typeof(TagLib.Tiff.Arw.File),
			typeof(TagLib.Tiff.Cr2.File),
			typeof(TagLib.Tiff.Dng.File),
			typeof(TagLib.Tiff.File),
			typeof(TagLib.Tiff.Nef.File),
			typeof(TagLib.Tiff.Pef.File),
			typeof(TagLib.Tiff.Rw2.File),
			typeof(TagLib.WavPack.File)
		};
		static FileTypes()
		{
			Init();
		}
		internal static void Init()
		{
			if(file_types!=null)
			{
				return;
			}
			file_types=new Dictionary<string,Type>();
			foreach(Type type in static_file_types)
			Register(type);
		}
		public static void Register(Type type)
		{
			Attribute[]attrs=Attribute.GetCustomAttributes(type,typeof(SupportedMimeType),false);
			if(attrs==null||attrs.Length==0)
			{
				return;
			}
			foreach(SupportedMimeType attr in attrs)
			file_types.Add(attr.MimeType,type);
		}
		public static IDictionary<string,Type>AvailableTypes
		{
			get
			{
				return file_types;
			}
		}
	}
}
