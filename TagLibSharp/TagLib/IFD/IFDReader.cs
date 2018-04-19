using System;
using System.IO;
using System.Collections.Generic;
using TagLib.IFD.Entries;
using TagLib.IFD.Makernotes;
using TagLib.IFD.Tags;
namespace TagLib.IFD
{
	public class IFDReader
	{
#region Private Constants
		private static readonly string PANASONIC_HEADER="Panasonic\0\0\0";
		private static readonly string PENTAX_HEADER="AOC\0";
		private static readonly string NIKON_HEADER="Nikon\0";
		private static readonly string OLYMPUS1_HEADER="OLYMP\0";
		private static readonly string OLYMPUS2_HEADER="OLYMPUS\0";
		private static readonly string SONY_HEADER="SONY DSC \0\0\0";
		private static readonly string LEICA_HEADER="LEICA\0\0\0";
#endregion
#region Protected Fields
		protected readonly File file;
		protected readonly bool is_bigendian;
		protected readonly IFDStructure structure;
		protected readonly long base_offset;
		protected readonly uint ifd_offset;
		protected readonly uint max_offset;
		protected bool parse_makernote=true;
#endregion
		internal bool ShouldParseMakernote
		{
			get
			{
				return parse_makernote;
			}
			set
			{
				parse_makernote=value;
			}
		}
#region Constructors
		public IFDReader(File file,bool is_bigendian,IFDStructure structure,long base_offset,uint ifd_offset,uint max_offset)
		{
			this.file=file;
			this.is_bigendian=is_bigendian;
			this.structure=structure;
			this.base_offset=base_offset;
			this.ifd_offset=ifd_offset;
			this.max_offset=max_offset;
		}
#endregion
#region Public Methods
		public void Read()
		{
			Read(-1);
		}
		public void Read(int count)
		{
			if(count==0)
			{
				return;
			}
			uint next_offset=ifd_offset;
			int i=0;
			lock(file)
			{
				StartIFDLoopDetect();
				do
				{
					if(DetectIFDLoop(base_offset+next_offset))
					{
						file.MarkAsCorrupt("IFD loop detected");
						break;
					}
					next_offset=ReadIFD(base_offset,next_offset,max_offset);
				}
				while(next_offset>0&&(count== -1|| ++i<count));
				StopIFDLoopDetect();
			}
		}
#endregion
#region Private Methods
		private void StartIFDLoopDetect()
		{
			if(!ifd_offsets.ContainsKey(file))
			{
				ifd_offsets[file]=new List<long>();
				ifd_loopdetect_refs[file]=1;
			}
			else
			{
				ifd_loopdetect_refs[file]++;
			}
		}
		private bool DetectIFDLoop(long offset)
		{
			if(offset==0)
			{
				return false;
			}
			if(ifd_offsets[file].Contains(offset))
			{
				return true;
			}
			ifd_offsets[file].Add(offset);
			return false;
		}
		private void StopIFDLoopDetect()
		{
			ifd_loopdetect_refs[file]--;
			if(ifd_loopdetect_refs[file]==0)
			{
				ifd_offsets.Remove(file);
				ifd_loopdetect_refs.Remove(file);
			}
		}
		private static Dictionary<File,List<long>>ifd_offsets=new Dictionary<File,List<long>>();
		private static Dictionary<File,int>ifd_loopdetect_refs=new Dictionary<File,int>();
		private uint ReadIFD(long base_offset,uint offset,uint max_offset)
		{
			long length=0;
			try
			{
				length=file.Length;
			}
			catch(Exception)
			{
				length=1073741824L*4;
			}
			if(base_offset+offset>length)
			{
				file.MarkAsCorrupt("Invalid IFD offset");
				return 0;
			}
			var directory=new IFDDirectory();
			file.Seek(base_offset+offset,SeekOrigin.Begin);
			ushort entry_count=ReadUShort();
			if(file.Tell+12*entry_count>base_offset+max_offset)
			{
				file.MarkAsCorrupt("Size of entries exceeds possible data size");
				return 0;
			}
			ByteVector entry_datas=file.ReadBlock(12*entry_count);
			uint next_offset=ReadUInt();
			for(int i=0;i<entry_count;i++)
			{
				ByteVector entry_data=entry_datas.Mid(i*12,12);
				ushort entry_tag=entry_data.Mid(0,2).ToUShort(is_bigendian);
				ushort type=entry_data.Mid(2,2).ToUShort(is_bigendian);
				uint value_count=entry_data.Mid(4,4).ToUInt(is_bigendian);
				ByteVector offset_data=entry_data.Mid(8,4);
				IFDEntry entry=CreateIFDEntry(entry_tag,type,value_count,base_offset,offset_data,max_offset);
				if(entry==null)
				{
					continue;
				}
				if(directory.ContainsKey(entry.Tag))
				{
					directory.Remove(entry.Tag);
				}
				directory.Add(entry.Tag,entry);
			}
			FixupDirectory(base_offset,directory);
			structure.directories.Add(directory);
			return next_offset;
		}
		private IFDEntry CreateIFDEntry(ushort tag,ushort type,uint count,long base_offset,ByteVector offset_data,uint max_offset)
		{
			uint offset=offset_data.ToUInt(is_bigendian);
			if(tag==(ushort)IFDEntryTag.IPTC&&type==(ushort)IFDEntryType.Long)
			{
				type=(ushort)IFDEntryType.Byte;
			}
			var ifd_entry=ParseIFDEntry(tag,type,count,base_offset,offset);
			if(ifd_entry!=null)
			{
				return ifd_entry;
			}
			if(count>0x10000000)
			{
				file.MarkAsCorrupt("Impossibly large item count");
				return null;
			}
			if(count==1)
			{
				if(type==(ushort)IFDEntryType.Byte)
				{
					return new ByteIFDEntry(tag,offset_data[0]);
				}
				if(type==(ushort)IFDEntryType.SByte)
				{
					return new SByteIFDEntry(tag,(sbyte)offset_data[0]);
				}
				if(type==(ushort)IFDEntryType.Short)
				{
					return new ShortIFDEntry(tag,offset_data.Mid(0,2).ToUShort(is_bigendian));
				}
				if(type==(ushort)IFDEntryType.SShort)
				{
					return new SShortIFDEntry(tag,(ushort)offset_data.Mid(0,2).ToUShort(is_bigendian));
				}
				if(type==(ushort)IFDEntryType.Long)
				{
					return new LongIFDEntry(tag,offset_data.ToUInt(is_bigendian));
				}
				if(type==(ushort)IFDEntryType.SLong)
				{
					return new SLongIFDEntry(tag,offset_data.ToInt(is_bigendian));
				}
			}
			if(count==2)
			{
				if(type==(ushort)IFDEntryType.Short)
				{
					ushort[]data=new ushort[]
					{
						offset_data.Mid(0,2).ToUShort(is_bigendian),offset_data.Mid(2,2).ToUShort(is_bigendian)
					};
					return new ShortArrayIFDEntry(tag,data);
				}
				if(type==(ushort)IFDEntryType.SShort)
				{
					short[]data=new short[]
					{
						(short)offset_data.Mid(0,2).ToUShort(is_bigendian),(short)offset_data.Mid(2,2).ToUShort(is_bigendian)
					};
					return new SShortArrayIFDEntry(tag,data);
				}
			}
			if(count<=4)
			{
				if(type==(ushort)IFDEntryType.Undefined)
				{
					return new UndefinedIFDEntry(tag,offset_data.Mid(0,(int)count));
				}
				if(type==(ushort)IFDEntryType.Ascii)
				{
					string data=offset_data.Mid(0,(int)count).ToString();
					int term=data.IndexOf('\0');
					if(term> -1)
					{
						data=data.Substring(0,term);
					}
					return new StringIFDEntry(tag,data);
				}
				if(type==(ushort)IFDEntryType.Byte)
				{
					return new ByteVectorIFDEntry(tag,offset_data.Mid(0,(int)count));
				}
			}
			if(offset>max_offset)
			{
				return new UndefinedIFDEntry(tag,new ByteVector());
			}
			file.Seek(base_offset+offset,SeekOrigin.Begin);
			if(count==1)
			{
				if(type==(ushort)IFDEntryType.Rational)
				{
					return new RationalIFDEntry(tag,ReadRational());
				}
				if(type==(ushort)IFDEntryType.SRational)
				{
					return new SRationalIFDEntry(tag,ReadSRational());
				}
			}
			if(count>1)
			{
				if(type==(ushort)IFDEntryType.Long)
				{
					uint[]data=ReadUIntArray(count);
					return new LongArrayIFDEntry(tag,data);
				}
				if(type==(ushort)IFDEntryType.SLong)
				{
					int[]data=ReadIntArray(count);
					return new SLongArrayIFDEntry(tag,data);
				}
				if(type==(ushort)IFDEntryType.Rational)
				{
					Rational[]entries=new Rational[count];
					for(int i=0;i<count;i++)
					{
						entries[i]=ReadRational();
					}
					return new RationalArrayIFDEntry(tag,entries);
				}
				if(type==(ushort)IFDEntryType.SRational)
				{
					SRational[]entries=new SRational[count];
					for(int i=0;i<count;i++)
					{
						entries[i]=ReadSRational();
					}
					return new SRationalArrayIFDEntry(tag,entries);
				}
			}
			if(count>2)
			{
				if(type==(ushort)IFDEntryType.Short)
				{
					ushort[]data=ReadUShortArray(count);
					return new ShortArrayIFDEntry(tag,data);
				}
				if(type==(ushort)IFDEntryType.SShort)
				{
					short[]data=ReadShortArray(count);
					return new SShortArrayIFDEntry(tag,data);
				}
			}
			if(count>4)
			{
				if(type==(ushort)IFDEntryType.Long)
				{
					uint[]data=ReadUIntArray(count);
					return new LongArrayIFDEntry(tag,data);
				}
				if(type==(ushort)IFDEntryType.Byte)
				{
					ByteVector data=file.ReadBlock((int)count);
					return new ByteVectorIFDEntry(tag,data);
				}
				if(type==(ushort)IFDEntryType.Ascii)
				{
					string data=ReadAsciiString((int)count);
					return new StringIFDEntry(tag,data);
				}
				if(tag==(ushort)ExifEntryTag.UserComment)
				{
					ByteVector data=file.ReadBlock((int)count);
					return new UserCommentIFDEntry(tag,data,file);
				}
				if(type==(ushort)IFDEntryType.Undefined)
				{
					ByteVector data=file.ReadBlock((int)count);
					return new UndefinedIFDEntry(tag,data);
				}
			}
			if(type==(ushort)IFDEntryType.Float)
			{
				return null;
			}
			if(type==0||type>12)
			{
				file.MarkAsCorrupt("Invalid item type");
				return null;
			}
			throw new NotImplementedException(String.Format("Unknown type/count {0}/{1} ({2})",type,count,offset));
		}
		private short ReadShort()
		{
			return file.ReadBlock(2).ToShort(is_bigendian);
		}
		private ushort ReadUShort()
		{
			return file.ReadBlock(2).ToUShort(is_bigendian);
		}
		private int ReadInt()
		{
			return file.ReadBlock(4).ToInt(is_bigendian);
		}
		private uint ReadUInt()
		{
			return file.ReadBlock(4).ToUInt(is_bigendian);
		}
		private Rational ReadRational()
		{
			uint numerator=ReadUInt();
			uint denominator=ReadUInt();
			if(denominator==0)
			{
				numerator=0;
				denominator=1;
			}
			return new Rational(numerator,denominator);
		}
		private SRational ReadSRational()
		{
			int numerator=ReadInt();
			int denominator=ReadInt();
			if(denominator==0)
			{
				numerator=0;
				denominator=1;
			}
			return new SRational(numerator,denominator);
		}
		private ushort[]ReadUShortArray(uint count)
		{
			ushort[]data=new ushort[count];
			for(int i=0;i<count;i++)
			{
				data[i]=ReadUShort();
			}
			return data;
		}
		private short[]ReadShortArray(uint count)
		{
			short[]data=new short[count];
			for(int i=0;i<count;i++)
			{
				data[i]=ReadShort();
			}
			return data;
		}
		private int[]ReadIntArray(uint count)
		{
			int[]data=new int[count];
			for(int i=0;i<count;i++)
			{
				data[i]=ReadInt();
			}
			return data;
		}
		private uint[]ReadUIntArray(uint count)
		{
			uint[]data=new uint[count];
			for(int i=0;i<count;i++)
			{
				data[i]=ReadUInt();
			}
			return data;
		}
		private string ReadAsciiString(int count)
		{
			string str=file.ReadBlock(count).ToString();
			int term=str.IndexOf('\0');
			if(term> -1)
			{
				str=str.Substring(0,term);
			}
			return str;
		}
		private void FixupDirectory(long base_offset,IFDDirectory directory)
		{
			ushort offset_tag=(ushort)IFDEntryTag.JPEGInterchangeFormat;
			ushort length_tag=(ushort)IFDEntryTag.JPEGInterchangeFormatLength;
			if(directory.ContainsKey(offset_tag)&&directory.ContainsKey(length_tag))
			{
				var offset_entry=directory[offset_tag]as LongIFDEntry;
				var length_entry=directory[length_tag]as LongIFDEntry;
				if(offset_entry!=null&&length_entry!=null)
				{
					uint offset=offset_entry.Value;
					uint length=length_entry.Value;
					file.Seek(base_offset+offset,SeekOrigin.Begin);
					ByteVector data=file.ReadBlock((int)length);
					directory.Remove(offset_tag);
					directory.Add(offset_tag,new ThumbnailDataIFDEntry(offset_tag,data));
				}
			}
			ushort strip_offsets_tag=(ushort)IFDEntryTag.StripOffsets;
			ushort strip_byte_counts_tag=(ushort)IFDEntryTag.StripByteCounts;
			if(directory.ContainsKey(strip_offsets_tag)&&directory.ContainsKey(strip_byte_counts_tag))
			{
				uint[]strip_offsets=null;
				uint[]strip_byte_counts=null;
				var strip_offsets_entry=directory[strip_offsets_tag];
				var strip_byte_counts_entry=directory[strip_byte_counts_tag];
				if(strip_offsets_entry is LongIFDEntry)
				{
					strip_offsets=new uint[]
					{
						(strip_offsets_entry as LongIFDEntry).Value
					};
				}
				else if(strip_offsets_entry is LongArrayIFDEntry)
				{
					strip_offsets=(strip_offsets_entry as LongArrayIFDEntry).Values;
				}
				if(strip_offsets==null)
				{
					return;
				}
				if(strip_byte_counts_entry is LongIFDEntry)
				{
					strip_byte_counts=new uint[]
					{
						(strip_byte_counts_entry as LongIFDEntry).Value
					};
				}
				else if(strip_byte_counts_entry is LongArrayIFDEntry)
				{
					strip_byte_counts=(strip_byte_counts_entry as LongArrayIFDEntry).Values;
				}
				if(strip_byte_counts==null)
				{
					return;
				}
				directory.Remove(strip_offsets_tag);
				directory.Add(strip_offsets_tag,new StripOffsetsIFDEntry(strip_offsets_tag,strip_offsets,strip_byte_counts,file));
			}
		}
		private IFDEntry ParseMakernote(ushort tag,ushort type,uint count,long base_offset,uint offset)
		{
			long makernote_offset=base_offset+offset;
			IFDStructure ifd_structure=new IFDStructure();
			int header_size=18;
			long length=0;
			try
			{
				length=file.Length;
			}
			catch(Exception)
			{
				length=1073741824L*4;
			}
			if(makernote_offset>length)
			{
				file.MarkAsCorrupt("offset to makernote is beyond file size");
				return null;
			}
			if(makernote_offset+header_size>length)
			{
				file.MarkAsCorrupt("data is to short to contain a maker note ifd");
				return null;
			}
			file.Seek(makernote_offset,SeekOrigin.Begin);
			ByteVector header=file.ReadBlock(header_size);
			if(header.StartsWith(PANASONIC_HEADER))
			{
				IFDReader reader=new IFDReader(file,is_bigendian,ifd_structure,base_offset,offset+12,max_offset);
				reader.ReadIFD(base_offset,offset+12,max_offset);
				return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Panasonic,PANASONIC_HEADER,12,true,null);
			}
			if(header.StartsWith(PENTAX_HEADER))
			{
				IFDReader reader=new IFDReader(file,is_bigendian,ifd_structure,base_offset,offset+6,max_offset);
				reader.ReadIFD(base_offset,offset+6,max_offset);
				return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Pentax,header.Mid(0,6),6,true,null);
			}
			if(header.StartsWith(OLYMPUS1_HEADER))
			{
				IFDReader reader=new IFDReader(file,is_bigendian,ifd_structure,base_offset,offset+8,max_offset);
				reader.Read();
				return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Olympus1,header.Mid(0,8),8,true,null);
			}
			if(header.StartsWith(OLYMPUS2_HEADER))
			{
				IFDReader reader=new IFDReader(file,is_bigendian,ifd_structure,makernote_offset,12,count);
				reader.Read();
				return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Olympus2,header.Mid(0,12),12,false,null);
			}
			if(header.StartsWith(SONY_HEADER))
			{
				IFDReader reader=new IFDReader(file,is_bigendian,ifd_structure,base_offset,offset+12,max_offset);
				reader.ReadIFD(base_offset,offset+12,max_offset);
				return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Sony,SONY_HEADER,12,true,null);
			}
			if(header.StartsWith(NIKON_HEADER))
			{
				ByteVector endian_bytes=header.Mid(10,2);
				if(endian_bytes.ToString()=="II"||endian_bytes.ToString()=="MM")
				{
					bool makernote_endian=endian_bytes.ToString().Equals("MM");
					ushort magic=header.Mid(12,2).ToUShort(is_bigendian);
					if(magic==42)
					{
						var reader=new Nikon3MakernoteReader(file,makernote_endian,ifd_structure,makernote_offset+10,8,max_offset-offset-10);
						reader.Read();
						return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Nikon3,header.Mid(0,18),8,false,makernote_endian);
					}
				}
			}
			if(header.StartsWith(LEICA_HEADER))
			{
				IFDReader reader=new IFDReader(file,is_bigendian,ifd_structure,makernote_offset,8,count);
				reader.Read();
				return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Leica,header.Mid(0,8),10,false,null);
			}
			try
			{
				IFDReader reader=new IFDReader(file,is_bigendian,ifd_structure,base_offset,offset,max_offset);
				reader.Read();
				return new MakernoteIFDEntry(tag,ifd_structure,MakernoteType.Canon);
			}
			catch
			{
				return null;
			}
		}
#endregion
#region Protected Methods
		protected virtual IFDEntry ParseIFDEntry(ushort tag,ushort type,uint count,long base_offset,uint offset)
		{
			if(tag==(ushort)ExifEntryTag.MakerNote&&parse_makernote)
			{
				return ParseMakernote(tag,type,count,base_offset,offset);
			}
			if(tag==(ushort)IFDEntryTag.SubIFDs)
			{
				var entries=new List<IFDStructure>();
				uint[]data;
				if(count>=2)
				{
					if(base_offset+offset>file.Length)
					{
						file.MarkAsCorrupt("Length of SubIFD is too long");
						return null;
					}
					file.Seek(base_offset+offset,SeekOrigin.Begin);
					data=ReadUIntArray(count);
				}
				else
				{
					data=new uint[]
					{
						offset
					};
				}
				foreach(var sub_offset in data)
				{
					var sub_structure=new IFDStructure();
					var sub_reader=CreateSubIFDReader(file,is_bigendian,sub_structure,base_offset,sub_offset,max_offset);
					sub_reader.Read();
					entries.Add(sub_structure);
				}
				return new SubIFDArrayEntry(tag,entries);
			}
			IFDStructure ifd_structure=new IFDStructure();
			IFDReader reader=CreateSubIFDReader(file,is_bigendian,ifd_structure,base_offset,offset,max_offset);
			if(type==(ushort)IFDEntryType.IFD)
			{
				reader.Read();
				return new SubIFDEntry(tag,type,(uint)ifd_structure.Directories.Length,ifd_structure);
			}
			switch(tag)
			{
				case(ushort)
				IFDEntryTag.ExifIFD:
				case(ushort)
				IFDEntryTag.InteroperabilityIFD:
				case(ushort)
				IFDEntryTag.GPSIFD:
				reader.Read();
				return new SubIFDEntry(tag,(ushort)IFDEntryType.Long,1,ifd_structure);
			default:
				return null;
			}
		}
		protected virtual IFDReader CreateSubIFDReader(File file,bool is_bigendian,IFDStructure structure,long base_offset,uint offset,uint max_offset)
		{
			return new IFDReader(file,is_bigendian,structure,base_offset,offset,max_offset);
		}
#endregion
	}
}
