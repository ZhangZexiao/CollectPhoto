using System;
using TagLib.Image;
namespace TagLib.Jpeg
{
	public class JpegCommentTag:
	ImageTag
	{
#region Constructors
		public JpegCommentTag(string value)
		{
			Value=value;
		}
		public JpegCommentTag()
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
				return TagTypes.JpegComment;
			}
		}
		public override void Clear()
		{
			Value=null;
		}
#endregion
	}
}
