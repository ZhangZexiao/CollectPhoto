using System;
using System.Collections.Generic;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
namespace TagLib.IFD
{
	public class IFDStructure
	{
#region Private Fields
		private static readonly string DATETIME_FORMAT="yyyy:MM:dd HH:mm:ss";
		internal readonly List<IFDDirectory>directories=new List<IFDDirectory>();
#endregion
#region Public Properties
		public IFDDirectory[]Directories
		{
			get
			{
				return directories.ToArray();
			}
		}
#endregion
#region Public Methods
		public bool ContainsTag(int directory,ushort tag)
		{
			if(directory>=directories.Count)
			{
				return false;
			}
			return directories[directory].ContainsKey(tag);
		}
		public void RemoveTag(int directory,ushort tag)
		{
			if(ContainsTag(directory,tag))
			{
				directories[directory].Remove(tag);
			}
		}
		public void RemoveTag(int directory,IFDEntryTag entry_tag)
		{
			RemoveTag(directory,(ushort)entry_tag);
		}
		public void AddEntry(int directory,IFDEntry entry)
		{
			while(directory>=directories.Count)directories.Add(new IFDDirectory());
			directories[directory].Add(entry.Tag,entry);
		}
		public void SetEntry(int directory,IFDEntry entry)
		{
			if(ContainsTag(directory,entry.Tag))
			{
				RemoveTag(directory,entry.Tag);
			}
			AddEntry(directory,entry);
		}
		public IFDEntry GetEntry(int directory,ushort tag)
		{
			if(!ContainsTag(directory,tag))
			{
				return null;
			}
			return directories[directory][tag];
		}
		public IFDEntry GetEntry(int directory,IFDEntryTag entry_tag)
		{
			return GetEntry(directory,(ushort)entry_tag);
		}
		public string GetStringValue(int directory,ushort entry_tag)
		{
			var entry=GetEntry(directory,entry_tag);
			if(entry!=null&&entry is StringIFDEntry)
			{
				return(entry as StringIFDEntry).Value;
			}
			return null;
		}
		public byte?GetByteValue(int directory,ushort entry_tag)
		{
			var entry=GetEntry(directory,entry_tag);
			if(entry!=null&&entry is ByteIFDEntry)
			{
				return(entry as ByteIFDEntry).Value;
			}
			return null;
		}
		public uint?GetLongValue(int directory,ushort entry_tag)
		{
			var entry=GetEntry(directory,entry_tag);
			if(entry is LongIFDEntry)
			{
				return(entry as LongIFDEntry).Value;
			}
			if(entry is ShortIFDEntry)
			{
				return(entry as ShortIFDEntry).Value;
			}
			return null;
		}
		public double?GetRationalValue(int directory,ushort entry_tag)
		{
			var entry=GetEntry(directory,entry_tag);
			if(entry is RationalIFDEntry)
			{
				return(entry as RationalIFDEntry).Value;
			}
			if(entry is SRationalIFDEntry)
			{
				return(entry as SRationalIFDEntry).Value;
			}
			return null;
		}
		public DateTime?GetDateTimeValue(int directory,ushort entry_tag)
		{
			string date_string=GetStringValue(directory,entry_tag);
			try
			{
				DateTime date_time=DateTime.ParseExact(date_string,DATETIME_FORMAT,System.Globalization.CultureInfo.InvariantCulture);
				return date_time;
			}
			catch
			{
			}
			return null;
		}
		public void SetStringValue(int directory,ushort entry_tag,string value)
		{
			if(value==null)
			{
				RemoveTag(directory,entry_tag);
				return;
			}
			SetEntry(directory,new StringIFDEntry(entry_tag,value));
		}
		public void SetByteValue(int directory,ushort entry_tag,byte value)
		{
			SetEntry(directory,new ByteIFDEntry(entry_tag,value));
		}
		public void SetLongValue(int directory,ushort entry_tag,uint value)
		{
			SetEntry(directory,new LongIFDEntry(entry_tag,value));
		}
		public void SetRationalValue(int directory,ushort entry_tag,double value)
		{
			if(value<0.0d||value>(double)UInt32.MaxValue)
			{
				throw new ArgumentException("value");
			}
			uint scale=(value>=1.0d)?1:UInt32.MaxValue;
			Rational rational=new Rational((uint)(scale*value),scale);
			SetEntry(directory,new RationalIFDEntry(entry_tag,rational));
		}
		public void SetDateTimeValue(int directory,ushort entry_tag,DateTime value)
		{
			string date_string=value.ToString(DATETIME_FORMAT);
			SetStringValue(directory,entry_tag,date_string);
		}
#endregion
	}
}
