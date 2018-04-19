using TagLib.IFD.Tags;
namespace TagLib.Tiff.Rw2
{
	public class IFDTag:
	TagLib.IFD.IFDTag
	{
		private File file;
		internal IFDTag(File file):
		base()
		{
			this.file=file;
		}
		public override uint?ISOSpeedRatings
		{
			get
			{
				return Structure.GetLongValue(0,(ushort)PanasonicMakerNoteEntryTag.ISO);
			}
			set
			{
				Structure.SetLongValue(0,(ushort)PanasonicMakerNoteEntryTag.ISO,value.HasValue?(uint)value:0);
			}
		}
		public override uint?FocalLengthIn35mmFilm
		{
			get
			{
				var jpg=file.JpgFromRaw;
				if(jpg==null)
				{
					return base.FocalLengthIn35mmFilm;
				}
				var tag=jpg.GetTag(TagTypes.TiffIFD,true)
				as Image.ImageTag;
				if(tag==null)
				{
					return base.FocalLengthIn35mmFilm;
				}
				return tag.FocalLengthIn35mmFilm??base.FocalLengthIn35mmFilm;
			}
			set
			{
				(file.JpgFromRaw.GetTag(TagTypes.TiffIFD,true)
				as Image.ImageTag).FocalLengthIn35mmFilm=value;
			}
		}
	}
}
