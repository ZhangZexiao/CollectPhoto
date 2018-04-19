using System.Collections.Generic;
using System;
namespace TagLib.Matroska
{
	public class Track:
	ICodec
	{
#region Private fields
#pragma warning disable 414 
		private uint track_number;
		private uint track_uid;
		private string track_codec_id;
		private string track_codec_name;
		private string track_name;
		private string track_language;
		private bool track_enabled;
		private bool track_default;
		private ByteVector codec_data;
#pragma warning restore 414
		private List<EBMLElement>unknown_elems=new List<EBMLElement>();
#endregion
#region Constructors
		public Track(File _file,EBMLElement element)
		{
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(_file,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaTrackNumber:
					track_number=child.ReadUInt();
					break;
				case MatroskaID.MatroskaTrackUID:
					track_uid=child.ReadUInt();
					break;
				case MatroskaID.MatroskaCodecID:
					track_codec_id=child.ReadString();
					break;
				case MatroskaID.MatroskaCodecName:
					track_codec_name=child.ReadString();
					break;
				case MatroskaID.MatroskaTrackName:
					track_name=child.ReadString();
					break;
				case MatroskaID.MatroskaTrackLanguage:
					track_language=child.ReadString();
					break;
				case MatroskaID.MatroskaTrackFlagEnabled:
					track_enabled=child.ReadBool();
					break;
				case MatroskaID.MatroskaTrackFlagDefault:
					track_default=child.ReadBool();
					break;
				case MatroskaID.MatroskaCodecPrivate:
					codec_data=child.ReadBytes();
					break;
				default:
					unknown_elems.Add(child);
					break;
				}
				i+=child.Size;
			}
		}
#endregion
#region Public fields
		public List<EBMLElement>UnknownElements
		{
			get
			{
				return unknown_elems;
			}
		}
#endregion
#region Public methods
#endregion
#region ICodec
		public virtual TimeSpan Duration
		{
			get
			{
				return TimeSpan.Zero;
			}
		}
		public virtual MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.None;
			}
		}
		public virtual string Description
		{
			get
			{
				return String.Format("{0} {1}",track_codec_name,track_language);
			}
		}
#endregion
	}
}
