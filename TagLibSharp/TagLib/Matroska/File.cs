using System.Collections.Generic;
using System;
namespace TagLib.Matroska
{
	public enum TrackType
	{
		Video=0x1,Audio=0x2,Complex=0x3,Logo=0x10,Subtitle=0x11,Buttons=0x12,Control=0x20
	}
	[SupportedMimeType("taglib/mkv","mkv")]
	[SupportedMimeType("taglib/mka","mka")]
	[SupportedMimeType("taglib/mks","mks")]
	[SupportedMimeType("video/webm")]
	[SupportedMimeType("video/x-matroska")]
	public class File:TagLib.File
	{
#region Private Fields
		private Matroska.Tag tag=new Matroska.Tag();
		private Properties properties;
		private UInt64 duration_unscaled;
		private uint time_scale;
		private TimeSpan duration;
#pragma warning disable 414 
		private string title;
#pragma warning restore 414
		private List<Track>tracks=new List<Track>();
#endregion
#region Constructors
		public File(string path,ReadStyle propertiesStyle):
		this(new File.LocalFileAbstraction(path),propertiesStyle)
		{
		}
		public File(string path):
		this(path,ReadStyle.Average)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction)
		{
			Mode=AccessMode.Read;
			try
			{
				Read(propertiesStyle);
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
			List<ICodec>codecs=new List<ICodec>();
			foreach(Track track in tracks)
			{
				codecs.Add(track);
			}
			properties=new Properties(duration,codecs);
		}
		public File(File.IFileAbstraction abstraction):
		this(abstraction,ReadStyle.Average)
		{
		}
#endregion
#region Public Methods
		public override void Save()
		{
			Mode=AccessMode.Write;
			try
			{
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		public override void RemoveTags(TagLib.TagTypes types)
		{
		}
		public override TagLib.Tag GetTag(TagLib.TagTypes type,bool create)
		{
			return null;
		}
#endregion
#region Public Properties
		public override TagLib.Tag Tag
		{
			get
			{
				return tag;
			}
		}
		public override TagLib.Properties Properties
		{
			get
			{
				return properties;
			}
		}
#endregion
#region Private Methods
		private void Read(ReadStyle propertiesStyle)
		{
			ulong offset=0;
			while(offset<(ulong)Length)
			{
				EBMLElement element=new EBMLElement(this,offset);
				EBMLID ebml_id=(EBMLID)element.ID;
				MatroskaID matroska_id=(MatroskaID)element.ID;
				switch(ebml_id)
				{
				case EBMLID.EBMLHeader:
					ReadHeader(element);
					break;
				default:
					break;
				}
				switch(matroska_id)
				{
				case MatroskaID.MatroskaSegment:
					ReadSegment(element);
					break;
				default:
					break;
				}
				offset+=element.Size;
			}
		}
		private void ReadHeader(EBMLElement element)
		{
			string doctype=null;
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				EBMLID ebml_id=(EBMLID)child.ID;
				switch(ebml_id)
				{
				case EBMLID.EBMLDocType:
					doctype=child.ReadString();
					break;
				default:
					break;
				}
				i+=child.Size;
			}
			if(String.IsNullOrEmpty(doctype)||(doctype!="matroska"&&doctype!="webm"))
			{
				throw new UnsupportedFormatException("DocType is not matroska or webm");
			}
		}
		private void ReadSegment(EBMLElement element)
		{
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaTracks:
					ReadTracks(child);
					break;
				case MatroskaID.MatroskaSegmentInfo:
					ReadSegmentInfo(child);
					break;
				case MatroskaID.MatroskaTags:
					ReadTags(child);
					break;
				case MatroskaID.MatroskaCluster:
					return;
				default:
					break;
				}
				i+=child.Size;
			}
		}
		private void ReadTags(EBMLElement element)
		{
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaTag:
					ReadTag(child);
					break;
				default:
					break;
				}
				i+=child.Size;
			}
		}
		private void ReadTag(EBMLElement element)
		{
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaSimpleTag:
					ReadSimpleTag(child);
					break;
				default:
					break;
				}
				i+=child.Size;
			}
		}
		private void ReadSimpleTag(EBMLElement element)
		{
			ulong i=0;
#pragma warning disable 219 
			string tag_name=null,tag_language=null,tag_string=null;
#pragma warning restore 219
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaTagName:
					tag_name=child.ReadString();
					break;
				case MatroskaID.MatroskaTagLanguage:
					tag_language=child.ReadString();
					break;
				case MatroskaID.MatroskaTagString:
					tag_string=child.ReadString();
					break;
				default:
					break;
				}
				i+=child.Size;
			}
			if(tag_name=="AUTHOR")
			{
				tag.Performers=new string[]
				{
					tag_string
				};
			}
			else if(tag_name=="TITLE")
			{
				tag.Title=tag_string;
			}
			else if(tag_name=="ALBUM")
			{
				tag.Album=tag_string;
			}
			else if(tag_name=="COMMENTS")
			{
				tag.Comment=tag_string;
			}
		}
		private void ReadSegmentInfo(EBMLElement element)
		{
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaDuration:
					duration_unscaled=(UInt64)child.ReadDouble();
					if(time_scale>0)
					{
						duration=TimeSpan.FromSeconds(duration_unscaled*time_scale/1000000000);
					}
					break;
				case MatroskaID.MatroskaTimeCodeScale:
					time_scale=child.ReadUInt();
					if(duration_unscaled>0)
					{
						duration=TimeSpan.FromSeconds(duration_unscaled*time_scale/1000000000);
					}
					break;
				case MatroskaID.MatroskaTitle:
					title=child.ReadString();
					break;
				default:
					break;
				}
				i+=child.Size;
			}
		}
		private void ReadTracks(EBMLElement element)
		{
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaTrackEntry:
					ReadTrackEntry(child);
					break;
				default:
					break;
				}
				i+=child.Size;
			}
		}
		private void ReadTrackEntry(EBMLElement element)
		{
			ulong i=0;
			while(i<element.DataSize)
			{
				EBMLElement child=new EBMLElement(this,element.DataOffset+i);
				MatroskaID matroska_id=(MatroskaID)child.ID;
				switch(matroska_id)
				{
				case MatroskaID.MatroskaTrackType:
					{
						TrackType track_type=(TrackType)child.ReadUInt();
						switch(track_type)
						{
						case TrackType.Video:
							{
								VideoTrack track=new VideoTrack(this,element);
								tracks.Add(track);
								break;
							}
						case TrackType.Audio:
							{
								AudioTrack track=new AudioTrack(this,element);
								tracks.Add(track);
								break;
							}
						case TrackType.Subtitle:
							{
								SubtitleTrack track=new SubtitleTrack(this,element);
								tracks.Add(track);
								break;
							}
						default:
							break;
						}
						break;
					}
				default:
					break;
				}
				i+=child.Size;
			}
		}
#endregion
#region Private Properties
#endregion
	}
}
