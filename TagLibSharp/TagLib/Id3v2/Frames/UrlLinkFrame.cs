using System;
using System.Collections.Generic;
using System.Text;
namespace TagLib.Id3v2
{
	public class UrlLinkFrame:
	Frame
	{
#region Private Fields
		private StringType encoding=StringType.Latin1;
		private string[]text_fields=new string[0];
		private ByteVector raw_data=null;
		private byte raw_version=0;
#endregion
#region Constructors
		public UrlLinkFrame(ByteVector ident):
		base(ident,4)
		{
		}
		public UrlLinkFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal UrlLinkFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(header)
		{
			SetData(data,offset,version,false);
		}
#endregion
#region Public Properties
		public virtual string[]Text
		{
			get
			{
				ParseRawData();
				return(string[])text_fields.Clone();
			}
			set
			{
				raw_data=null;
				text_fields=value!=null?(string[])value.Clone():new string[0];
			}
		}
		public StringType TextEncoding
		{
			get
			{
				ParseRawData();
				return encoding;
			}
			set
			{
				encoding=value;
			}
		}
#endregion
#region Public Methods
		public override string ToString()
		{
			ParseRawData();
			return string.Join("; ",Text);
		}
		public override ByteVector Render(byte version)
		{
			return base.Render(version);
		}
#endregion
#region Public Static Methods
		public static UrlLinkFrame Get(Tag tag,ByteVector ident,bool create)
		{
			if(tag==null)
			{
				throw new ArgumentNullException("tag");
			}
			if(ident==null)
			{
				throw new ArgumentNullException("ident");
			}
			if(ident.Count!=4)
			{
				throw new ArgumentException("Identifier must be four bytes long.","ident");
			}
			foreach(UrlLinkFrame frame in tag.GetFrames<UrlLinkFrame>(ident))
			return frame;
			if(!create)
			{
				return null;
			}
			UrlLinkFrame new_frame=new UrlLinkFrame(ident);
			tag.AddFrame(new_frame);
			return new_frame;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			raw_data=data;
			raw_version=version;
		}
		protected void ParseRawData()
		{
			if(raw_data==null)
			{
				return;
			}
			ByteVector data=raw_data;
			raw_data=null;
			List<string>field_list=new List<string>();
			ByteVector delim=ByteVector.TextDelimiter(encoding);
			if(FrameId!=FrameType.WXXX)
			{
				field_list.AddRange(data.ToStrings(StringType.Latin1,0));
			}
			else if(data.Count>1&& !data.Mid(0,delim.Count).Equals(delim))
			{
				string value=data.ToString(StringType.Latin1,1,data.Count-1);
				if(value.Length>1&&value[value.Length-1]==0)
				{
					for(int i=value.Length-1;i>=0;i--)
					{
						if(value[i]!=0)
						{
							value=value.Substring(0,i+1);
							break;
						}
					}
				}
				field_list.Add(value);
			}
			while(field_list.Count!=0&&string.IsNullOrEmpty(field_list[field_list.Count-1]))field_list.RemoveAt(field_list.Count-1);
			text_fields=field_list.ToArray();
		}
		protected override ByteVector RenderFields(byte version)
		{
			if(raw_data!=null&&raw_version==version)
			{
				return raw_data;
			}
			StringType encoding=CorrectEncoding(TextEncoding,version);
			bool wxxx=FrameId==FrameType.WXXX;
			ByteVector v;
			if(wxxx)
			{
				v=new ByteVector((byte)encoding);
			}
			else
			{
				v=new ByteVector();
			}
			string[]text=text_fields;
			if(version>3||wxxx)
			{
				if(wxxx)
				{
					if(text.Length==0)
					{
						text=new string[]
						{
							null,null
						};
					}
					else if(text.Length==1)
					{
						text=new string[]
						{
							text[0],null
						};
					}
				}
				v.Add(ByteVector.FromString(string.Join("/",text),StringType.Latin1));
			}
			else
			{
				v.Add(ByteVector.FromString(string.Join("/",text),StringType.Latin1));
			}
			return v;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			UrlLinkFrame frame=(this is UserUrlLinkFrame)?new UserUrlLinkFrame(null,encoding):new UrlLinkFrame(FrameId);
			frame.text_fields=(string[])text_fields.Clone();
			if(raw_data!=null)
			{
				frame.raw_data=new ByteVector(raw_data);
			}
			frame.raw_version=raw_version;
			return frame;
		}
#endregion
	}
	public class UserUrlLinkFrame:
	UrlLinkFrame
	{
#region Constructors
		public UserUrlLinkFrame(string description,StringType encoding):
		base(FrameType.WXXX)
		{
			base.Text=new string[]
			{
				description
			};
		}
		public UserUrlLinkFrame(string description):
		base(FrameType.WXXX)
		{
			base.Text=new string[]
			{
				description
			};
		}
		public UserUrlLinkFrame(ByteVector data,byte version):
		base(data,version)
		{
		}
		protected internal UserUrlLinkFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(data,offset,header,version)
		{
		}
#endregion
#region Public Properties
		public string Description
		{
			get
			{
				string[]text=base.Text;
				return text.Length>0?text[0]:null;
			}
			set
			{
				string[]text=base.Text;
				if(text.Length>0)
				{
					text[0]=value;
				}
				else
				{
					text=new string[]
					{
						value
					};
				}
				base.Text=text;
			}
		}
		public override string[]Text
		{
			get
			{
				string[]text=base.Text;
				if(text.Length<2)
				{
					return new string[0];
				}
				string[]new_text=new string[text.Length-1];
				for(int i=0;i<new_text.Length;i++)
				{
					new_text[i]=text[i+1];
				}
				return new_text;
			}
			set
			{
				string[]new_value=new string[value!=null?(value.Length+1):1];
				new_value[0]=Description;
				for(int i=1;i<new_value.Length;i++)
				{
					new_value[i]=value[i-1];
				}
				base.Text=new_value;
			}
		}
#endregion
#region Public Methods
		public override string ToString()
		{
			return new StringBuilder().Append("[").Append(Description).Append("] ").Append(base.ToString()).ToString();
		}
#endregion
#region Public Static Methods
		public static UserUrlLinkFrame Get(Tag tag,string description,StringType type,bool create)
		{
			if(tag==null)
			{
				throw new ArgumentNullException("tag");
			}
			if(description==null)
			{
				throw new ArgumentNullException("description");
			}
			if(description.Length==0)
			{
				throw new ArgumentException("Description must not be empty.","description");
			}
			foreach(UserUrlLinkFrame frame in tag.GetFrames<UserUrlLinkFrame>(FrameType.WXXX))
			if(description.Equals(frame.Description))
			{
				return frame;
			}
			if(!create)
			{
				return null;
			}
			UserUrlLinkFrame new_frame=new UserUrlLinkFrame(description,type);
			tag.AddFrame(new_frame);
			return new_frame;
		}
		public static UserUrlLinkFrame Get(Tag tag,string description,bool create)
		{
			return Get(tag,description,Tag.DefaultEncoding,create);
		}
#endregion
	}
}
