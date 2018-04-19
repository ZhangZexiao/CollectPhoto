using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace TagLib.Id3v2
{
	public class TextInformationFrame:
	Frame
	{
#region Private Fields
		private StringType encoding=Id3v2.Tag.DefaultEncoding;
		private string[]text_fields=new string[0];
		private ByteVector raw_data=null;
		private byte raw_version=0;
		private StringType raw_encoding=StringType.Latin1;
#endregion
#region Constructors
		public TextInformationFrame(ByteVector ident,StringType encoding):
		base(ident,4)
		{
			this.encoding=encoding;
		}
		public TextInformationFrame(ByteVector ident):
		this(ident,Id3v2.Tag.DefaultEncoding)
		{
		}
		public TextInformationFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal TextInformationFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(header)
		{
			SetData(data,offset,version,false);
		}
#endregion
#region Public Properties
		[Obsolete("Use TextInformationFrame.Text")]
		public StringCollection FieldList
		{
			get
			{
				ParseRawData();
				return new StringCollection(Text);
			}
		}
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
		[Obsolete("Use TextInformationFrame.Text")]
		public void SetText(StringCollection fields)
		{
			raw_data=null;
			Text=fields!=null?fields.ToArray():null;
		}
		[Obsolete("Use TextInformationFrame.Text")]
		public void SetText(params string[]text)
		{
			raw_data=null;
			Text=text;
		}
		public override string ToString()
		{
			ParseRawData();
			return string.Join("; ",Text);
		}
		public override ByteVector Render(byte version)
		{
			if(version!=3||FrameId!=FrameType.TDRC)
			{
				return base.Render(version);
			}
			string text=ToString();
			if(text.Length<10||text[4]!='-'||text[7]!='-')
			{
				return base.Render(version);
			}
			ByteVector output=new ByteVector();
			TextInformationFrame f;
			f=new TextInformationFrame(FrameType.TYER,encoding);
			f.Text=new string[]
			{
				text.Substring(0,4)
			};
			output.Add(f.Render(version));
			f=new TextInformationFrame(FrameType.TDAT,encoding);
			f.Text=new string[]
			{
				text.Substring(5,2)+text.Substring(8,2)
			};
			output.Add(f.Render(version));
			if(text.Length<16||text[10]!='T'||text[13]!=':')
			{
				return output;
			}
			f=new TextInformationFrame(FrameType.TIME,encoding);
			f.Text=new string[]
			{
				text.Substring(11,2)+text.Substring(14,2)
			};
			output.Add(f.Render(version));
			return output;
		}
#endregion
#region Public Static Methods
		public static TextInformationFrame Get(Tag tag,ByteVector ident,StringType encoding,bool create)
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
			foreach(TextInformationFrame frame in tag.GetFrames<TextInformationFrame>(ident))
			return frame;
			if(!create)
			{
				return null;
			}
			TextInformationFrame new_frame=new TextInformationFrame(ident,encoding);
			tag.AddFrame(new_frame);
			return new_frame;
		}
		public static TextInformationFrame Get(Tag tag,ByteVector ident,bool create)
		{
			return Get(tag,ident,Tag.DefaultEncoding,create);
		}
		[Obsolete("Use TextInformationFrame.Get(Tag,ByteVector,bool)")]
		public static TextInformationFrame Get(Tag tag,ByteVector ident)
		{
			return Get(tag,ident,false);
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			raw_data=data;
			raw_version=version;
			raw_encoding=(StringType)data[0];
		}
		protected void ParseRawData()
		{
			if(raw_data==null)
			{
				return;
			}
			ByteVector data=raw_data;
			raw_data=null;
			encoding=(StringType)data[0];
			List<string>field_list=new List<string>();
			ByteVector delim=ByteVector.TextDelimiter(encoding);
			if(raw_version>3||FrameId==FrameType.TXXX)
			{
				field_list.AddRange(data.ToStrings(encoding,1));
			}
			else if(data.Count>1&& !data.Mid(1,delim.Count).Equals(delim))
			{
				string value=data.ToString(encoding,1,data.Count-1);
				int null_index=value.IndexOf('\x00');
				if(null_index>=0)
				{
					value=value.Substring(0,null_index);
				}
				if(FrameId==FrameType.TCOM||FrameId==FrameType.TEXT||FrameId==FrameType.TOLY||FrameId==FrameType.TOPE||FrameId==FrameType.TPE1||FrameId==FrameType.TPE2||FrameId==FrameType.TPE3||FrameId==FrameType.TPE4)
				{
					field_list.AddRange(value.Split('/'));
				}
				else if(FrameId==FrameType.TCON)
				{
					while(value.Length>1&&value[0]=='(')
					{
						int closing=value.IndexOf(')');
						if(closing<0)
						{
							break;
						}
						string number=value.Substring(1,closing-1);
						field_list.Add(number);
						value=value.Substring(closing+1).TrimStart('/',' ');
						string text=Genres.IndexToAudio(number);
						if(text!=null&&value.StartsWith(text))
						{
							value=value.Substring(text.Length).TrimStart('/',' ');
						}
					}
					if(value.Length>0)
					{
						field_list.AddRange(value.Split(new char[]{'/'}));
					}
				}
				else
				{
					field_list.Add(value);
				}
			}
			while(field_list.Count!=0&&string.IsNullOrEmpty(field_list[field_list.Count-1]))field_list.RemoveAt(field_list.Count-1);
			text_fields=field_list.ToArray();
		}
		protected override ByteVector RenderFields(byte version)
		{
			if(raw_data!=null&&raw_version==version&&raw_encoding==Tag.DefaultEncoding)
			{
				return raw_data;
			}
			StringType encoding=CorrectEncoding(TextEncoding,version);
			ByteVector v=new ByteVector((byte)encoding);
			string[]text=text_fields;
			bool txxx=FrameId==FrameType.TXXX;
			if(version>3||txxx)
			{
				if(txxx)
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
				for(int i=0;i<text.Length;i++)
				{
					if(i!=0)
					{
						v.Add(ByteVector.TextDelimiter(encoding));
					}
					if(text[i]!=null)
					{
						v.Add(ByteVector.FromString(text[i],encoding));
					}
				}
			}
			else if(FrameId==FrameType.TCON)
			{
				byte id;
				bool prev_value_indexed=true;
				StringBuilder data=new StringBuilder();
				foreach(string s in text)
				{
					if(!prev_value_indexed)
					{
						data.Append("/").Append(s);
						continue;
					}
					if(prev_value_indexed=byte.TryParse(s,out id))
					{
						data.AppendFormat(CultureInfo.InvariantCulture,"({0})",id);
					}
					else
					{
						data.Append(s);
					}
				}
				v.Add(ByteVector.FromString(data.ToString(),encoding));
			}
			else
			{
				v.Add(ByteVector.FromString(string.Join("/",text),encoding));
			}
			return v;
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			TextInformationFrame frame=(this is UserTextInformationFrame)?new UserTextInformationFrame(null,encoding):new TextInformationFrame(FrameId,encoding);
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
	public class UserTextInformationFrame:
	TextInformationFrame
	{
#region Constructors
		public UserTextInformationFrame(string description,StringType encoding):
		base(FrameType.TXXX,encoding)
		{
			base.Text=new string[]
			{
				description
			};
		}
		public UserTextInformationFrame(string description):
		base(FrameType.TXXX)
		{
			base.Text=new string[]
			{
				description
			};
		}
		public UserTextInformationFrame(ByteVector data,byte version):
		base(data,version)
		{
		}
		protected internal UserTextInformationFrame(ByteVector data,int offset,FrameHeader header,byte version):
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
		public static UserTextInformationFrame Get(Tag tag,string description,StringType type,bool create,bool caseSensitive)
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
			StringComparison stringComparison=caseSensitive?StringComparison.InvariantCulture:StringComparison.InvariantCultureIgnoreCase;
			foreach(UserTextInformationFrame frame in tag.GetFrames<UserTextInformationFrame>(FrameType.TXXX))
			if(description.Equals(frame.Description,stringComparison))
			{
				return frame;
			}
			if(!create)
			{
				return null;
			}
			UserTextInformationFrame new_frame=new UserTextInformationFrame(description,type);
			tag.AddFrame(new_frame);
			return new_frame;
		}
		public static UserTextInformationFrame Get(Tag tag,string description,StringType type,bool create)
		{
			return Get(tag,description,type,create,true);
		}
		public static UserTextInformationFrame Get(Tag tag,string description,bool create)
		{
			return Get(tag,description,Tag.DefaultEncoding,create);
		}
		[Obsolete("Use UserTextInformationFrame.Get(Tag,string,bool)")]
		public static UserTextInformationFrame Get(Tag tag,string description)
		{
			return Get(tag,description,false);
		}
#endregion
	}
}
