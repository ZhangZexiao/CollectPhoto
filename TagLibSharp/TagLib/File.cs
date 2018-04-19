using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
namespace TagLib
{
	public enum ReadStyle
	{
		None=0,Average=2,
	}
	public abstract class File:
	IDisposable
	{
#region Enums
		public enum AccessMode
		{
			Read,Write,Closed
		}
#endregion
#region Delegates
		public delegate File FileTypeResolver(IFileAbstraction abstraction,string mimetype,ReadStyle style);
#endregion
#region Private Properties
		private System.IO.Stream file_stream;
		private IFileAbstraction file_abstraction;
		private string mime_type;
		private TagTypes tags_on_disk=TagTypes.None;
		private static int buffer_size=1024;
		private static List<FileTypeResolver>file_type_resolvers=new List<FileTypeResolver>();
		private long invariant_start_position= -1;
		private long invariant_end_position= -1;
		private List<string>corruption_reasons=null;
#endregion
#region Public Static Properties
		public static uint BufferSize
		{
			get
			{
				return(uint)buffer_size;
			}
		}
#endregion
#region Constructors
		protected File(string path)
		{
			if(path==null)
			{
				throw new ArgumentNullException("path");
			}
			file_abstraction=new LocalFileAbstraction(path);
		}
		protected File(IFileAbstraction abstraction)
		{
			if(abstraction==null)
			{
				throw new ArgumentNullException("abstraction");
			}
			file_abstraction=abstraction;
		}
#endregion
#region Public Properties
		public abstract Tag Tag
		{
			get;
		}
		public abstract Properties Properties
		{
			get;
		}
		public TagTypes TagTypesOnDisk
		{
			get
			{
				return tags_on_disk;
			}
			protected set
			{
				tags_on_disk=value;
			}
		}
		public TagTypes TagTypes
		{
			get
			{
				return Tag!=null?Tag.TagTypes:TagTypes.None;
			}
		}
		public string Name
		{
			get
			{
				return file_abstraction.Name;
			}
		}
		public string MimeType
		{
			get
			{
				return mime_type;
			}
			internal set
			{
				mime_type=value;
			}
		}
		public long Tell
		{
			get
			{
				return(Mode==AccessMode.Closed)?0:file_stream.Position;
			}
		}
		public long Length
		{
			get
			{
				return(Mode==AccessMode.Closed)?0:file_stream.Length;
			}
		}
		public long InvariantStartPosition
		{
			get
			{
				return invariant_start_position;
			}
			protected set
			{
				invariant_start_position=value;
			}
		}
		public long InvariantEndPosition
		{
			get
			{
				return invariant_end_position;
			}
			protected set
			{
				invariant_end_position=value;
			}
		}
		public AccessMode Mode
		{
			get
			{
				return(file_stream==null)?AccessMode.Closed:(file_stream.CanWrite)?AccessMode.Write:AccessMode.Read;
			}
			set
			{
				if(Mode==value||(Mode==AccessMode.Write&&value==AccessMode.Read))
				{
					return;
				}
				if(file_stream!=null)
				{
					file_abstraction.CloseStream(file_stream);
				}
				file_stream=null;
				if(value==AccessMode.Read)
				{
					file_stream=file_abstraction.ReadStream;
				}
				else if(value==AccessMode.Write)
				{
					file_stream=file_abstraction.WriteStream;
				}
				Mode=value;
			}
		}
		public virtual bool Writeable
		{
			get
			{
				return!PossiblyCorrupt;
			}
		}
		public bool PossiblyCorrupt
		{
			get
			{
				return corruption_reasons!=null;
			}
		}
		public IEnumerable<string>CorruptionReasons
		{
			get
			{
				return corruption_reasons;
			}
		}
#endregion
#region Public Methods
		internal void MarkAsCorrupt(string reason)
		{
			if(corruption_reasons==null)
			{
				corruption_reasons=new List<string>();
			}
			corruption_reasons.Add(reason);
		}
		public void Dispose()
		{
			Mode=AccessMode.Closed;
		}
		public abstract void Save();
		public abstract void RemoveTags(TagTypes types);
		public abstract Tag GetTag(TagTypes type,bool create);
		public Tag GetTag(TagTypes type)
		{
			return GetTag(type,false);
		}
		public ByteVector ReadBlock(int length)
		{
			if(length<0)
			{
				throw new ArgumentException("Length must be non-negative","length");
			}
			if(length==0)
			{
				return new ByteVector();
			}
			Mode=AccessMode.Read;
			byte[]buffer=new byte[length];
			int count=0,read=0,needed=length;
			do
			{
				count=file_stream.Read(buffer,read,needed);
				read+=count;
				needed-=count;
			}
			while(needed>0&&count!=0);
			return new ByteVector(buffer,read);
		}
		public void WriteBlock(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			Mode=AccessMode.Write;
			file_stream.Write(data.Data,0,data.Count);
		}
		public long Find(ByteVector pattern,long startPosition,ByteVector before)
		{
			if(pattern==null)
			{
				throw new ArgumentNullException("pattern");
			}
			Mode=AccessMode.Read;
			if(pattern.Count>buffer_size)
			{
				return-1;
			}
			long buffer_offset=startPosition;
			long original_position=file_stream.Position;
			try
			{
				file_stream.Position=startPosition;
				for(var buffer=ReadBlock(buffer_size);buffer.Count>0;buffer=ReadBlock(buffer_size))
				{
					var location=buffer.Find(pattern);
					if(before!=null)
					{
						var beforeLocation=buffer.Find(before);
						if(beforeLocation<location)
						{
							return-1;
						}
					}
					if(location>=0)
					{
						return buffer_offset+location;
					}
					buffer_offset+=buffer_size-pattern.Count;
					if(before!=null&&before.Count>pattern.Count)
					{
						buffer_offset-=before.Count-pattern.Count;
					}
					file_stream.Position=buffer_offset;
				}
				return-1;
			}
			finally
			{
				file_stream.Position=original_position;
			}
		}
		public long Find(ByteVector pattern,long startPosition)
		{
			return Find(pattern,startPosition,null);
		}
		public long Find(ByteVector pattern)
		{
			return Find(pattern,0);
		}
		long RFind(ByteVector pattern,long startPosition,ByteVector after)
		{
			if(pattern==null)
			{
				throw new ArgumentNullException("pattern");
			}
			Mode=AccessMode.Read;
			if(pattern.Count>buffer_size)
			{
				return-1;
			}
			ByteVector buffer;
			long original_position=file_stream.Position;
			long buffer_offset=Length-startPosition;
			int read_size=buffer_size;
			read_size=(int)Math.Min(buffer_offset,buffer_size);
			buffer_offset-=read_size;
			file_stream.Position=buffer_offset;
			for(buffer=ReadBlock(read_size);buffer.Count>0;buffer=ReadBlock(read_size))
			{
				long location=buffer.RFind(pattern);
				if(location>=0)
				{
					file_stream.Position=original_position;
					return buffer_offset+location;
				}
				if(after!=null&&buffer.RFind(after)>=0)
				{
					file_stream.Position=original_position;
					return-1;
				}
				read_size=(int)Math.Min(buffer_offset,buffer_size);
				buffer_offset-=read_size;
				if(read_size+pattern.Count>buffer_size)
				{
					buffer_offset+=pattern.Count;
				}
				file_stream.Position=buffer_offset;
			}
			file_stream.Position=original_position;
			return-1;
		}
		public long RFind(ByteVector pattern,long startPosition)
		{
			return RFind(pattern,startPosition,null);
		}
		public long RFind(ByteVector pattern)
		{
			return RFind(pattern,0);
		}
		public void Insert(ByteVector data,long start,long replace)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			Mode=AccessMode.Write;
			if(data.Count==replace)
			{
				file_stream.Position=start;
				WriteBlock(data);
				return;
			}
			else if(data.Count<replace)
			{
				file_stream.Position=start;
				WriteBlock(data);
				RemoveBlock(start+data.Count,replace-data.Count);
				return;
			}
			int buffer_length=buffer_size;
			while(data.Count-replace>buffer_length)buffer_length+=(int)BufferSize;
			long read_position=start+replace;
			long write_position=start;
			byte[]buffer;
			byte[]about_to_overwrite;
			file_stream.Position=read_position;
			about_to_overwrite=ReadBlock(buffer_length).Data;
			read_position+=buffer_length;
			file_stream.Position=write_position;
			WriteBlock(data);
			write_position+=data.Count;
			buffer=new byte[about_to_overwrite.Length];
			System.Array.Copy(about_to_overwrite,0,buffer,0,about_to_overwrite.Length);
			while(buffer_length!=0)
			{
				file_stream.Position=read_position;
				int bytes_read=file_stream.Read(about_to_overwrite,0,buffer_length<about_to_overwrite.Length?buffer_length:about_to_overwrite.Length);
				read_position+=buffer_length;
				file_stream.Position=write_position;
				file_stream.Write(buffer,0,buffer_length<buffer.Length?buffer_length:buffer.Length);
				write_position+=buffer_length;
				System.Array.Copy(about_to_overwrite,0,buffer,0,bytes_read);
				buffer_length=bytes_read;
			}
		}
		public void Insert(ByteVector data,long start)
		{
			Insert(data,start,0);
		}
		public void RemoveBlock(long start,long length)
		{
			if(length<=0)
			{
				return;
			}
			Mode=AccessMode.Write;
			int buffer_length=buffer_size;
			long read_position=start+length;
			long write_position=start;
			ByteVector buffer=(byte)1;
			while(buffer.Count!=0)
			{
				file_stream.Position=read_position;
				buffer=ReadBlock(buffer_length);
				read_position+=buffer.Count;
				file_stream.Position=write_position;
				WriteBlock(buffer);
				write_position+=buffer.Count;
			}
			Truncate(write_position);
		}
		public void Seek(long offset,System.IO.SeekOrigin origin)
		{
			if(Mode!=AccessMode.Closed)
			{
				file_stream.Seek(offset,origin);
			}
		}
		public void Seek(long offset)
		{
			Seek(offset,System.IO.SeekOrigin.Begin);
		}
#endregion
#region Public Static Methods
		public static File Create(string path)
		{
			return Create(path,null,ReadStyle.Average);
		}
		public static File Create(IFileAbstraction abstraction)
		{
			return Create(abstraction,null,ReadStyle.Average);
		}
		public static File Create(string path,ReadStyle propertiesStyle)
		{
			return Create(path,null,propertiesStyle);
		}
		public static File Create(IFileAbstraction abstraction,ReadStyle propertiesStyle)
		{
			return Create(abstraction,null,propertiesStyle);
		}
		public static File Create(string path,string mimetype,ReadStyle propertiesStyle)
		{
			return Create(new LocalFileAbstraction(path),mimetype,propertiesStyle);
		}
		public static File Create(IFileAbstraction abstraction,string mimetype,ReadStyle propertiesStyle)
		{
			if(mimetype==null)
			{
				string ext=String.Empty;
				int index=abstraction.Name.LastIndexOf(".")+1;
				if(index>=1&&index<abstraction.Name.Length)
				{
					ext=abstraction.Name.Substring(index,abstraction.Name.Length-index);
				}
				mimetype="taglib/"+ext.ToLower(CultureInfo.InvariantCulture);
			}
			foreach(FileTypeResolver resolver in file_type_resolvers)
			{
				File file=resolver(abstraction,mimetype,propertiesStyle);
				if(file!=null)
				{
					return file;
				}
			}
			if(!FileTypes.AvailableTypes.ContainsKey(mimetype))
			{
				throw new UnsupportedFormatException(String.Format(CultureInfo.InvariantCulture,"{0} ({1})",abstraction.Name,mimetype));
			}
			Type file_type=FileTypes.AvailableTypes[mimetype];
			try
			{
				File file=(File)Activator.CreateInstance(file_type,new object[]{abstraction,propertiesStyle});
				file.MimeType=mimetype;
				return file;
			}
			catch(System.Reflection.TargetInvocationException e)
			{
				PrepareExceptionForRethrow(e.InnerException);
				throw e.InnerException;
			}
		}
		public static void AddFileTypeResolver(FileTypeResolver resolver)
		{
			if(resolver!=null)
			{
				file_type_resolvers.Insert(0,resolver);
			}
		}
#endregion
#region Protected Methods
		protected void Truncate(long length)
		{
			AccessMode old_mode=Mode;
			Mode=AccessMode.Write;
			file_stream.SetLength(length);
			Mode=old_mode;
		}
		private static void PrepareExceptionForRethrow(Exception ex)
		{
			var ctx=new StreamingContext(StreamingContextStates.CrossAppDomain);
			var mgr=new ObjectManager(null,ctx);
			var si=new SerializationInfo(ex.GetType(),new FormatterConverter());
			ex.GetObjectData(si,ctx);
			mgr.RegisterObject(ex,1,si);
			mgr.DoFixups();
		}
#endregion
#region Classes
		public class LocalFileAbstraction:
		IFileAbstraction
		{
			private string name;
			public LocalFileAbstraction(string path)
			{
				if(path==null)
				{
					throw new ArgumentNullException("path");
				}
				name=path;
			}
			public string Name
			{
				get
				{
					return name;
				}
			}
			public System.IO.Stream ReadStream
			{
				get
				{
					return System.IO.File.Open(Name,System.IO.FileMode.Open,System.IO.FileAccess.Read,System.IO.FileShare.Read);
				}
			}
			public System.IO.Stream WriteStream
			{
				get
				{
					return System.IO.File.Open(Name,System.IO.FileMode.Open,System.IO.FileAccess.ReadWrite);
				}
			}
			public void CloseStream(System.IO.Stream stream)
			{
				if(stream==null)
				{
					throw new ArgumentNullException("stream");
				}
				stream.Close();
			}
		}
#endregion
#region Interfaces
		public interface IFileAbstraction
		{
			string Name
			{
				get;
			}
			System.IO.Stream ReadStream
			{
				get;
			}
			System.IO.Stream WriteStream
			{
				get;
			}
			void CloseStream(System.IO.Stream stream);
		}
#endregion
	}
}
