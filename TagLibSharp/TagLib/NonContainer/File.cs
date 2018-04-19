using System;
namespace TagLib.NonContainer
{
	public abstract class File:
	TagLib.File
	{
#region Private Fields
		private TagLib.NonContainer.Tag tag;
		private Properties properties;
#endregion
#region Constructors
		protected File(string path,ReadStyle propertiesStyle):
		base(path)
		{
			Read(propertiesStyle);
		}
		protected File(string path):
		this(path,ReadStyle.Average)
		{
		}
		protected File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction)
		{
			Read(propertiesStyle);
		}
		protected File(File.IFileAbstraction abstraction):
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
#endregion
#region Public Methods
		public override void Save()
		{
			long start,end;
			Mode=AccessMode.Write;
			try
			{
				tag.Write(out start,out end);
				InvariantStartPosition=start;
				InvariantEndPosition=end;
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		public override void RemoveTags(TagTypes types)
		{
			tag.RemoveTags(types);
		}
#endregion
#region Protected Properties
		protected StartTag StartTag
		{
			get
			{
				return tag.StartTag;
			}
		}
		protected EndTag EndTag
		{
			get
			{
				return tag.EndTag;
			}
		}
#endregion
#region Protected Methods
		protected virtual void ReadStart(long start,ReadStyle propertiesStyle)
		{
		}
		protected virtual void ReadEnd(long end,ReadStyle propertiesStyle)
		{
		}
		protected abstract Properties ReadProperties(long start,long end,ReadStyle propertiesStyle);
#endregion
#region Private Methods
		private void Read(ReadStyle propertiesStyle)
		{
			Mode=AccessMode.Read;
			try
			{
				tag=new Tag(this);
				InvariantStartPosition=tag.ReadStart();
				TagTypesOnDisk|=StartTag.TagTypes;
				ReadStart(InvariantStartPosition,propertiesStyle);
				InvariantEndPosition=(InvariantStartPosition==Length)?Length:tag.ReadEnd();
				TagTypesOnDisk|=EndTag.TagTypes;
				ReadEnd(InvariantEndPosition,propertiesStyle);
				properties=(propertiesStyle!=ReadStyle.None)?ReadProperties(InvariantStartPosition,InvariantEndPosition,propertiesStyle):null;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
#endregion
	}
}
