using System;
using System.Collections.Generic;
using System.Linq;
namespace TagLib.Mpeg4
{
	[SupportedMimeType("taglib/m4a","m4a")]
	[SupportedMimeType("taglib/m4b","m4b")]
	[SupportedMimeType("taglib/m4v","m4v")]
	[SupportedMimeType("taglib/m4p","m4p")]
	[SupportedMimeType("taglib/mp4","mp4")]
	[SupportedMimeType("audio/mp4")]
	[SupportedMimeType("audio/x-m4a")]
	[SupportedMimeType("video/mp4")]
	[SupportedMimeType("video/x-m4v")]
	public class File:TagLib.File
	{
#region Private Fields
		private AppleTag apple_tag;
		private CombinedTag tag;
		private Properties properties;
		private List<IsoUserDataBox>udta_boxes=new List<IsoUserDataBox>();
#endregion
#region Constructors
		public File(string path,ReadStyle propertiesStyle):
		base(path)
		{
			Read(propertiesStyle);
		}
		public File(string path):
		this(path,ReadStyle.Average)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction)
		{
			Read(propertiesStyle);
		}
		public File(File.IFileAbstraction abstraction):
		this(abstraction,ReadStyle.Average)
		{
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
		protected List<IsoUserDataBox>UdtaBoxes
		{
			get
			{
				return udta_boxes;
			}
		}
#endregion
#region Public Methods
		public override void Save()
		{
			if(udta_boxes.Count==0)
			{
				IsoUserDataBox udtaBox=new IsoUserDataBox();
				udta_boxes.Add(udtaBox);
			}
			Mode=File.AccessMode.Write;
			try
			{
				FileParser parser=new FileParser(this);
				parser.ParseBoxHeaders();
				InvariantStartPosition=parser.MdatStartPosition;
				InvariantEndPosition=parser.MdatEndPosition;
				long size_change=0;
				long write_position=0;
				IsoUserDataBox udtaBox=FindAppleTagUdta();
				if(null==udtaBox)
				{
					udtaBox=new IsoUserDataBox();
				}
				ByteVector tag_data=udtaBox.Render();
				if(udtaBox.ParentTree==null||udtaBox.ParentTree.Length==0)
				{
					BoxHeader moov_header=parser.MoovTree[parser.MoovTree.Length-1];
					size_change=tag_data.Count;
					write_position=moov_header.Position+moov_header.TotalBoxSize;
					Insert(tag_data,write_position,0);
					for(int i=parser.MoovTree.Length-1;i>=0;i--)
					{
						size_change=parser.MoovTree[i].Overwrite(this,size_change);
					}
				}
				else
				{
					BoxHeader udta_header=udtaBox.ParentTree[udtaBox.ParentTree.Length-1];
					size_change=tag_data.Count-udta_header.TotalBoxSize;
					write_position=udta_header.Position;
					Insert(tag_data,write_position,udta_header.TotalBoxSize);
					for(int i=udtaBox.ParentTree.Length-2;i>=0;i--)
					{
						size_change=udtaBox.ParentTree[i].Overwrite(this,size_change);
					}
				}
				if(size_change!=0)
				{
					parser.ParseChunkOffsets();
					InvariantStartPosition=parser.MdatStartPosition;
					InvariantEndPosition=parser.MdatEndPosition;
					foreach(Box box in parser.ChunkOffsetBoxes)
					{
						IsoChunkLargeOffsetBox co64=box as IsoChunkLargeOffsetBox;
						if(co64!=null)
						{
							co64.Overwrite(this,size_change,write_position);
							continue;
						}
						IsoChunkOffsetBox stco=box as IsoChunkOffsetBox;
						if(stco!=null)
						{
							stco.Overwrite(this,size_change,write_position);
							continue;
						}
					}
				}
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=File.AccessMode.Closed;
			}
		}
		public override TagLib.Tag GetTag(TagTypes type,bool create)
		{
			if(type==TagTypes.Apple)
			{
				if(apple_tag==null&&create)
				{
					IsoUserDataBox udtaBox=FindAppleTagUdta();
					if(null==udtaBox)
					{
						udtaBox=new IsoUserDataBox();
					}
					apple_tag=new AppleTag(udtaBox);
					tag.SetTags(apple_tag);
				}
				return apple_tag;
			}
			return null;
		}
		public override void RemoveTags(TagTypes types)
		{
			if((types&TagTypes.Apple)!=TagTypes.Apple||apple_tag==null)
			{
				return;
			}
			apple_tag.DetachIlst();
			apple_tag=null;
			tag.SetTags();
		}
#endregion
#region Private Methods
		private void Read(ReadStyle propertiesStyle)
		{
			tag=new CombinedTag();
			Mode=AccessMode.Read;
			try
			{
				FileParser parser=new FileParser(this);
				if(propertiesStyle==ReadStyle.None)
				{
					parser.ParseTag();
				}
				else
				{
					parser.ParseTagAndProperties();
				}
				InvariantStartPosition=parser.MdatStartPosition;
				InvariantEndPosition=parser.MdatEndPosition;
				udta_boxes.AddRange(parser.UserDataBoxes);
				if(udta_boxes.Count==0)
				{
					IsoUserDataBox dummy=new IsoUserDataBox();
					udta_boxes.Add(dummy);
				}
				if(IsAppleTagUdtaPresent())
				{
					TagTypesOnDisk|=TagTypes.Apple;
				}
				IsoUserDataBox udtaBox=FindAppleTagUdta();
				if(null==udtaBox)
				{
					udtaBox=new IsoUserDataBox();
				}
				apple_tag=new AppleTag(udtaBox);
				tag.SetTags(apple_tag);
				if(propertiesStyle==ReadStyle.None)
				{
					Mode=AccessMode.Closed;
					return;
				}
				IsoMovieHeaderBox mvhd_box=parser.MovieHeaderBox;
				if(mvhd_box==null)
				{
					Mode=AccessMode.Closed;
					throw new CorruptFileException("mvhd box not found.");
				}
				IsoAudioSampleEntry audio_sample_entry=parser.AudioSampleEntry;
				IsoVisualSampleEntry visual_sample_entry=parser.VisualSampleEntry;
				properties=new Properties(mvhd_box.Duration,audio_sample_entry,visual_sample_entry);
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		private IsoUserDataBox FindAppleTagUdta()
		{
			if(udta_boxes.Count==1)
			{
				return udta_boxes[0];
			}
			var udtaBox=udta_boxes.Where(box=>box.GetChildRecursively(BoxType.Ilst)!=null).OrderBy(box=>box.ParentTree.Length).FirstOrDefault();
			return udtaBox;
		}
		private bool IsAppleTagUdtaPresent()
		{
			foreach(IsoUserDataBox udtaBox in udta_boxes)
			{
				if(udtaBox.GetChild(BoxType.Meta)!=null&&udtaBox.GetChild(BoxType.Meta).GetChild(BoxType.Ilst)!=null)
				{
					return true;
				}
			}
			return false;
		}
#endregion
	}
}
