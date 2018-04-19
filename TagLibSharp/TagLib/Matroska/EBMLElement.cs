using System.Collections.Generic;
using System;
namespace TagLib.Matroska
{
	public class EBMLElement
	{
#region Private Fields
		private ulong offset=0;
		private ulong data_offset=0;
		private uint ebml_id=0;
		private ulong ebml_size=0;
		private Matroska.File file=null;
#endregion
#region Constructors
		public EBMLElement(Matroska.File _file,ulong position)
		{
			if(_file==null)
			{
				throw new ArgumentNullException("file");
			}
			if(position>(ulong)(_file.Length-4))
			{
				throw new ArgumentOutOfRangeException("position");
			}
			file=_file;
			file.Seek((long)position);
			ByteVector vector=file.ReadBlock(1);
			Byte header_byte=vector[0];
			Byte mask=0x80,id_length=1;
			while(id_length<=4&&(header_byte&mask)==0)
			{
				id_length++;
				mask>>=1;
			}
			if(id_length>4)
			{
				throw new CorruptFileException("invalid EBML id size");
			}
			if(id_length>1)
			{
				vector.Add(file.ReadBlock(id_length-1));
			}
			ebml_id=vector.ToUInt();
			vector.Clear();
			vector=file.ReadBlock(1);
			header_byte=vector[0];
			mask=0x80;
			Byte size_length=1;
			while(size_length<=8&&(header_byte&mask)==0)
			{
				size_length++;
				mask>>=1;
			}
			if(size_length>8)
			{
				throw new CorruptFileException("invalid EBML element size");
			}
			vector[0]&=(Byte)(mask-1);
			if(size_length>1)
			{
				vector.Add(file.ReadBlock(size_length-1));
			}
			ebml_size=vector.ToULong();
			offset=position;
			data_offset=offset+id_length+size_length;
		}
#endregion
#region Public Properties
		public uint ID
		{
			get
			{
				return ebml_id;
			}
		}
		public ulong Size
		{
			get
			{
				return(data_offset-offset)+ebml_size;
			}
		}
		public ulong DataSize
		{
			get
			{
				return ebml_size;
			}
		}
		public ulong DataOffset
		{
			get
			{
				return data_offset;
			}
		}
		public ulong Offset
		{
			get
			{
				return offset;
			}
		}
#endregion
#region Public Methods
		public string ReadString()
		{
			if(file==null)
			{
				return null;
			}
			file.Seek((long)data_offset);
			ByteVector vector=file.ReadBlock((int)ebml_size);
			return vector.ToString();
		}
		public bool ReadBool()
		{
			if(file==null)
			{
				return false;
			}
			file.Seek((long)data_offset);
			ByteVector vector=file.ReadBlock((int)ebml_size);
			if(vector.ToUInt()>0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public double ReadDouble()
		{
			if(file==null)
			{
				return 0;
			}
			if(ebml_size!=4&&ebml_size!=8)
			{
				throw new UnsupportedFormatException("Can not read a Double with sizes differing from 4 or 8");
			}
			file.Seek((long)data_offset);
			ByteVector vector=file.ReadBlock((int)ebml_size);
			double result=0.0;
			if(ebml_size==4)
			{
				result=(double)vector.ToFloat();
			}
			else if(ebml_size==8)
			{
				result=vector.ToDouble();
			}
			return result;
		}
		public uint ReadUInt()
		{
			if(file==null)
			{
				return 0;
			}
			file.Seek((long)data_offset);
			ByteVector vector=file.ReadBlock((int)ebml_size);
			return vector.ToUInt();
		}
		public ByteVector ReadBytes()
		{
			if(file==null)
			{
				return null;
			}
			file.Seek((long)data_offset);
			ByteVector vector=file.ReadBlock((int)ebml_size);
			return vector;
		}
#endregion
	}
}
