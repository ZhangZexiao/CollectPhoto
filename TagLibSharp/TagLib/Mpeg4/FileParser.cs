using System;
using System.Collections.Generic;
namespace TagLib.Mpeg4
{
	public class FileParser
	{
#region Private Fields
		private TagLib.File file;
		private BoxHeader first_header;
		private IsoMovieHeaderBox mvhd_box;
		private List<IsoUserDataBox>udta_boxes=new List<IsoUserDataBox>();
		private BoxHeader[]moov_tree;
		private BoxHeader[]udta_tree;
		private List<Box>stco_boxes=new List<Box>();
		private List<Box>stsd_boxes=new List<Box>();
		private long mdat_start= -1;
		private long mdat_end= -1;
#endregion
#region Constructors
		public FileParser(TagLib.File file)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			this.file=file;
			first_header=new BoxHeader(file,0);
			if(first_header.BoxType!="ftyp")
			{
				throw new CorruptFileException("File does not start with 'ftyp' box.");
			}
		}
#endregion
#region Public Properties
		public IsoMovieHeaderBox MovieHeaderBox
		{
			get
			{
				return mvhd_box;
			}
		}
		public IsoUserDataBox[]UserDataBoxes
		{
			get
			{
				return udta_boxes.ToArray();
			}
		}
		public IsoUserDataBox UserDataBox
		{
			get
			{
				return UserDataBoxes.Length==0?null:UserDataBoxes[0];
			}
		}
		public IsoAudioSampleEntry AudioSampleEntry
		{
			get
			{
				foreach(IsoSampleDescriptionBox box in stsd_boxes)
				foreach(Box sub in box.Children)
				{
					IsoAudioSampleEntry entry=sub as IsoAudioSampleEntry;
					if(entry!=null)
					{
						return entry;
					}
				}
				return null;
			}
		}
		public IsoVisualSampleEntry VisualSampleEntry
		{
			get
			{
				foreach(IsoSampleDescriptionBox box in stsd_boxes)
				foreach(Box sub in box.Children)
				{
					IsoVisualSampleEntry entry=sub as IsoVisualSampleEntry;
					if(entry!=null)
					{
						return entry;
					}
				}
				return null;
			}
		}
		public BoxHeader[]MoovTree
		{
			get
			{
				return moov_tree;
			}
		}
		public BoxHeader[]UdtaTree
		{
			get
			{
				return udta_tree;
			}
		}
		public Box[]ChunkOffsetBoxes
		{
			get
			{
				return stco_boxes.ToArray();
			}
		}
		public long MdatStartPosition
		{
			get
			{
				return mdat_start;
			}
		}
		public long MdatEndPosition
		{
			get
			{
				return mdat_end;
			}
		}
#endregion
#region Public Methods
		public void ParseBoxHeaders()
		{
			try
			{
				ResetFields();
				ParseBoxHeaders(first_header.TotalBoxSize,file.Length,null);
			}
			catch(CorruptFileException e)
			{
				file.MarkAsCorrupt(e.Message);
			}
		}
		public void ParseTag()
		{
			try
			{
				ResetFields();
				ParseTag(first_header.TotalBoxSize,file.Length,null);
			}
			catch(CorruptFileException e)
			{
				file.MarkAsCorrupt(e.Message);
			}
		}
		public void ParseTagAndProperties()
		{
			try
			{
				ResetFields();
				ParseTagAndProperties(first_header.TotalBoxSize,file.Length,null,null);
			}
			catch(CorruptFileException e)
			{
				file.MarkAsCorrupt(e.Message);
			}
		}
		public void ParseChunkOffsets()
		{
			try
			{
				ResetFields();
				ParseChunkOffsets(first_header.TotalBoxSize,file.Length);
			}
			catch(CorruptFileException e)
			{
				file.MarkAsCorrupt(e.Message);
			}
		}
#endregion
#region Private Methods
		private void ParseBoxHeaders(long start,long end,List<BoxHeader>parents)
		{
			BoxHeader header;
			for(long position=start;position<end;position+=header.TotalBoxSize)
			{
				header=new BoxHeader(file,position);
				if(moov_tree==null&&header.BoxType==BoxType.Moov)
				{
					List<BoxHeader>new_parents=AddParent(parents,header);
					moov_tree=new_parents.ToArray();
					ParseBoxHeaders(header.HeaderSize+position,header.TotalBoxSize+position,new_parents);
				}
				else if(header.BoxType==BoxType.Mdia||header.BoxType==BoxType.Minf||header.BoxType==BoxType.Stbl||header.BoxType==BoxType.Trak)
				{
					ParseBoxHeaders(header.HeaderSize+position,header.TotalBoxSize+position,AddParent(parents,header));
				}
				else if(udta_tree==null&&header.BoxType==BoxType.Udta)
				{
					udta_tree=AddParent(parents,header).ToArray();
				}
				else if(header.BoxType==BoxType.Mdat)
				{
					mdat_start=position;
					mdat_end=position+header.TotalBoxSize;
				}
				if(header.TotalBoxSize==0)
				{
					break;
				}
			}
		}
		private void ParseTag(long start,long end,List<BoxHeader>parents)
		{
			BoxHeader header;
			for(long position=start;position<end;position+=header.TotalBoxSize)
			{
				header=new BoxHeader(file,position);
				if(header.BoxType==BoxType.Moov)
				{
					ParseTag(header.HeaderSize+position,header.TotalBoxSize+position,AddParent(parents,header));
				}
				else if(header.BoxType==BoxType.Mdia||header.BoxType==BoxType.Minf||header.BoxType==BoxType.Stbl||header.BoxType==BoxType.Trak)
				{
					ParseTag(header.HeaderSize+position,header.TotalBoxSize+position,AddParent(parents,header));
				}
				else if(header.BoxType==BoxType.Udta)
				{
					IsoUserDataBox udtaBox=BoxFactory.CreateBox(file,header)
					as IsoUserDataBox;
					List<BoxHeader>new_parents=AddParent(parents,header);
					udtaBox.ParentTree=new_parents.ToArray();
					udta_boxes.Add(udtaBox);
				}
				else if(header.BoxType==BoxType.Mdat)
				{
					mdat_start=position;
					mdat_end=position+header.TotalBoxSize;
				}
				if(header.TotalBoxSize==0)
				{
					break;
				}
			}
		}
		private void ParseTagAndProperties(long start,long end,IsoHandlerBox handler,List<BoxHeader>parents)
		{
			BoxHeader header;
			for(long position=start;position<end;position+=header.TotalBoxSize)
			{
				header=new BoxHeader(file,position);
				ByteVector type=header.BoxType;
				if(type==BoxType.Moov)
				{
					ParseTagAndProperties(header.HeaderSize+position,header.TotalBoxSize+position,handler,AddParent(parents,header));
				}
				else if(type==BoxType.Mdia||type==BoxType.Minf||type==BoxType.Stbl||type==BoxType.Trak)
				{
					ParseTagAndProperties(header.HeaderSize+position,header.TotalBoxSize+position,handler,AddParent(parents,header));
				}
				else if(type==BoxType.Stsd)
				{
					stsd_boxes.Add(BoxFactory.CreateBox(file,header,handler));
				}
				else if(type==BoxType.Hdlr)
				{
					handler=BoxFactory.CreateBox(file,header,handler)
					as IsoHandlerBox;
				}
				else if(mvhd_box==null&&type==BoxType.Mvhd)
				{
					mvhd_box=BoxFactory.CreateBox(file,header,handler)
					as IsoMovieHeaderBox;
				}
				else if(type==BoxType.Udta)
				{
					IsoUserDataBox udtaBox=BoxFactory.CreateBox(file,header,handler)
					as IsoUserDataBox;
					List<BoxHeader>new_parents=AddParent(parents,header);
					udtaBox.ParentTree=new_parents.ToArray();
					udta_boxes.Add(udtaBox);
				}
				else if(type==BoxType.Mdat)
				{
					mdat_start=position;
					mdat_end=position+header.TotalBoxSize;
				}
				if(header.TotalBoxSize==0)
				{
					break;
				}
			}
		}
		private void ParseChunkOffsets(long start,long end)
		{
			BoxHeader header;
			for(long position=start;position<end;position+=header.TotalBoxSize)
			{
				header=new BoxHeader(file,position);
				if(header.BoxType==BoxType.Moov)
				{
					ParseChunkOffsets(header.HeaderSize+position,header.TotalBoxSize+position);
				}
				else if(header.BoxType==BoxType.Moov||header.BoxType==BoxType.Mdia||header.BoxType==BoxType.Minf||header.BoxType==BoxType.Stbl||header.BoxType==BoxType.Trak)
				{
					ParseChunkOffsets(header.HeaderSize+position,header.TotalBoxSize+position);
				}
				else if(header.BoxType==BoxType.Stco||header.BoxType==BoxType.Co64)
				{
					stco_boxes.Add(BoxFactory.CreateBox(file,header));
				}
				else if(header.BoxType==BoxType.Mdat)
				{
					mdat_start=position;
					mdat_end=position+header.TotalBoxSize;
				}
				if(header.TotalBoxSize==0)
				{
					break;
				}
			}
		}
		private void ResetFields()
		{
			mvhd_box=null;
			udta_boxes.Clear();
			moov_tree=null;
			udta_tree=null;
			stco_boxes.Clear();
			stsd_boxes.Clear();
			mdat_start= -1;
			mdat_end= -1;
		}
#endregion
#region Private Static Methods
		private static List<BoxHeader>AddParent(List<BoxHeader>parents,BoxHeader current)
		{
			List<BoxHeader>boxes=new List<BoxHeader>();
			if(parents!=null)
			{
				boxes.AddRange(parents);
			}
			boxes.Add(current);
			return boxes;
		}
#endregion
	}
}
