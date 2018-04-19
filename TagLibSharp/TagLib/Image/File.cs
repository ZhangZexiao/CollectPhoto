using System;
using System.Collections.Generic;
using TagLib.Jpeg;
using TagLib.Gif;
using TagLib.IFD;
using TagLib.Xmp;
using TagLib.Png;
namespace TagLib.Image
{
	public abstract class File:
	TagLib.File
	{
		private CombinedImageTag image_tag;
#region Constructors
		protected File(string path):
		base(path)
		{
		}
		protected File(IFileAbstraction abstraction):
		base(abstraction)
		{
		}
#endregion
#region Public Properties
		public override Tag Tag
		{
			get
			{
				return ImageTag;
			}
		}
		public CombinedImageTag ImageTag
		{
			get
			{
				return image_tag;
			}
			protected set
			{
				image_tag=value;
			}
		}
#endregion
#region Public Methods
		public void EnsureAvailableTags()
		{
			foreach(TagTypes type in Enum.GetValues(typeof(TagTypes)))
			{
				if((type&ImageTag.AllowedTypes)!=0x00&&type!=TagTypes.AllTags)
				{
					GetTag(type,true);
				}
			}
		}
		public override void RemoveTags(TagLib.TagTypes types)
		{
			List<ImageTag>to_delete=new List<ImageTag>();
			foreach(ImageTag tag in ImageTag.AllTags)
			{
				if((tag.TagTypes&types)==tag.TagTypes)
				{
					to_delete.Add(tag);
				}
			}
			foreach(ImageTag tag in to_delete)
			ImageTag.RemoveTag(tag);
		}
		public override TagLib.Tag GetTag(TagLib.TagTypes type,bool create)
		{
			foreach(Tag tag in ImageTag.AllTags)
			{
				if((tag.TagTypes&type)==type)
				{
					return tag;
				}
			}
			if(!create||(type&ImageTag.AllowedTypes)==0)
			{
				return null;
			}
			ImageTag new_tag=null;
			switch(type)
			{
			case TagTypes.JpegComment:
				new_tag=new JpegCommentTag();
				break;
			case TagTypes.GifComment:
				new_tag=new GifCommentTag();
				break;
			case TagTypes.Png:
				new_tag=new PngTag();
				break;
			case TagTypes.TiffIFD:
				new_tag=new IFDTag();
				break;
			case TagTypes.XMP:
				new_tag=new XmpTag();
				break;
			}
			if(new_tag!=null)
			{
				ImageTag.AddTag(new_tag);
				return new_tag;
			}
			throw new NotImplementedException(String.Format("Adding tag of type {0} not supported!",type));
		}
		public void CopyFrom(TagLib.Image.File file)
		{
			EnsureAvailableTags();
			var from_tag=file.ImageTag;
			var to_tag=ImageTag;
			foreach(var prop in typeof(TagLib.Image.ImageTag).GetProperties())
			{
				if(!prop.CanWrite||prop.Name=="TagTypes")
				{
					continue;
				}
				var value=prop.GetValue(from_tag,null);
				prop.SetValue(to_tag,value,null);
			}
		}
#endregion
	}
}
