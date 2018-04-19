using System;
using TagLib.Image;
namespace TagLib.Gif
{
	public class GifCommentTag:
	ImageTag
	{
#region Constructors
		public GifCommentTag(string value)
		{
			Value=value;
		}
		public GifCommentTag()
		{
			Value=null;
		}
#endregion
#region Public Properties
		public string Value
		{
			get;
			set;
		}
		public override string Comment
		{
			get
			{
				return Value;
			}
			set
			{
				Value=value;
			}
		}
#endregion
#region Public Methods
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.GifComment;
			}
		}
		public override void Clear()
		{
			Value=null;
		}
#endregion
	}
}
