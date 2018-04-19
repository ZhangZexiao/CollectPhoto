using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
namespace TagLib.Mpeg4
{
	public class AppleTag:
	TagLib.Tag,IEnumerable<Box>
	{
#region Private Fields
		private IsoMetaBox meta_box;
		private AppleItemListBox ilst_box;
#endregion
#region Constructors
		public AppleTag(IsoUserDataBox box)
		{
			if(box==null)
			{
				throw new ArgumentNullException("box");
			}
			meta_box=box.GetChild(BoxType.Meta)
			as IsoMetaBox;
			if(meta_box==null)
			{
				meta_box=new IsoMetaBox("mdir",null);
				box.AddChild(meta_box);
			}
			ilst_box=meta_box.GetChild(BoxType.Ilst)
			as AppleItemListBox;
			if(ilst_box==null)
			{
				ilst_box=new AppleItemListBox();
				meta_box.AddChild(ilst_box);
			}
		}
#endregion
#region Public Methods
		public bool IsCompilation
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Cpil))
				return box.Data.ToUInt()!=0;
				return false;
			}
			set
			{
				SetData(BoxType.Cpil,new ByteVector((byte)(value?1:0)),(uint)AppleDataBox.FlagType.ForTempo);
			}
		}
#endregion
#region Public Methods
		public IEnumerable<AppleDataBox>DataBoxes(IEnumerable<ByteVector>types)
		{
			foreach(Box box in ilst_box.Children)
			foreach(ByteVector v in types)
			{
				if(FixId(v)!=box.BoxType)
				{
					continue;
				}
				foreach(Box data_box in box.Children)
				{
					AppleDataBox adb=data_box as AppleDataBox;
					if(adb!=null)
					{
						yield return adb;
					}
				}
			}
		}
		public IEnumerable<AppleDataBox>DataBoxes(params ByteVector[]types)
		{
			return DataBoxes(types as IEnumerable<ByteVector>);
		}
		public IEnumerable<AppleDataBox>DataBoxes(string mean,string name)
		{
			foreach(Box box in ilst_box.Children)
			{
				if(box.BoxType!=BoxType.DASH)
				{
					continue;
				}
				AppleAdditionalInfoBox mean_box=(AppleAdditionalInfoBox)box.GetChild(BoxType.Mean);
				AppleAdditionalInfoBox name_box=(AppleAdditionalInfoBox)box.GetChild(BoxType.Name);
				if(mean_box==null||name_box==null||mean_box.Text!=mean||name_box.Text!=name)
				{
					continue;
				}
				foreach(Box data_box in box.Children)
				{
					AppleDataBox adb=data_box as AppleDataBox;
					if(adb!=null)
					{
						yield return adb;
					}
				}
			}
		}
		public string[]GetText(ByteVector type)
		{
			List<string>result=new List<string>();
			foreach(AppleDataBox box in DataBoxes(type))
			{
				if(box.Text==null)
				{
					continue;
				}
				foreach(string text in box.Text.Split(';'))
				result.Add(text.Trim());
			}
			return result.ToArray();
		}
		public void SetData(ByteVector type,AppleDataBox[]boxes)
		{
			type=FixId(type);
			bool added=false;
			foreach(Box box in ilst_box.Children)
			if(type==box.BoxType)
			{
				box.ClearChildren();
				if(added)
				{
					continue;
				}
				added=true;
				foreach(AppleDataBox b in boxes)
				box.AddChild(b);
			}
			if(added)
			{
				return;
			}
			Box box2=new AppleAnnotationBox(type);
			ilst_box.AddChild(box2);
			foreach(AppleDataBox b in boxes)
			box2.AddChild(b);
		}
		public void SetData(ByteVector type,ByteVectorCollection data,uint flags)
		{
			if(data==null||data.Count==0)
			{
				ClearData(type);
				return;
			}
			AppleDataBox[]boxes=new AppleDataBox[data.Count];
			for(int i=0;i<data.Count;i++)
			{
				boxes[i]=new AppleDataBox(data[i],flags);
			}
			SetData(type,boxes);
		}
		public void SetData(ByteVector type,ByteVector data,uint flags)
		{
			if(data==null||data.Count==0)
			{
				ClearData(type);
			}
			else
			{
				SetData(type,new ByteVectorCollection(data),flags);
			}
		}
		public void SetText(ByteVector type,string[]text)
		{
			if(text==null)
			{
				ilst_box.RemoveChild(FixId(type));
				return;
			}
			SetText(type,string.Join("; ",text));
		}
		public void SetText(ByteVector type,string text)
		{
			if(string.IsNullOrEmpty(text))
			{
				ilst_box.RemoveChild(FixId(type));
				return;
			}
			ByteVectorCollection l=new ByteVectorCollection();
			l.Add(ByteVector.FromString(text,StringType.UTF8));
			SetData(type,l,(uint)AppleDataBox.FlagType.ContainsText);
		}
		public void ClearData(ByteVector type)
		{
			ilst_box.RemoveChild(FixId(type));
		}
		public void DetachIlst()
		{
			meta_box.RemoveChild(ilst_box);
		}
		public string GetDashBox(string meanstring,string namestring)
		{
			AppleDataBox data_box=GetDashAtoms(meanstring,namestring);
			if(data_box!=null)
			{
				return data_box.Text;
			}
			else
			{
				return null;
			}
		}
		public void SetDashBox(string meanstring,string namestring,string datastring)
		{
			AppleDataBox data_box=GetDashAtoms(meanstring,namestring);
			if(data_box!=null&&string.IsNullOrEmpty(datastring))
			{
				AppleAnnotationBox dash_box=GetParentDashBox(meanstring,namestring);
				dash_box.ClearChildren();
				ilst_box.RemoveChild(dash_box);
				return;
			}
			if(data_box!=null)
			{
				data_box.Text=datastring;
			}
			else
			{
				AppleAdditionalInfoBox amean_box=new AppleAdditionalInfoBox(BoxType.Mean);
				AppleAdditionalInfoBox aname_box=new AppleAdditionalInfoBox(BoxType.Name);
				AppleDataBox adata_box=new AppleDataBox(BoxType.Data,1);
				amean_box.Text=meanstring;
				aname_box.Text=namestring;
				adata_box.Text=datastring;
				AppleAnnotationBox whole_box=new AppleAnnotationBox(BoxType.DASH);
				whole_box.AddChild(amean_box);
				whole_box.AddChild(aname_box);
				whole_box.AddChild(adata_box);
				ilst_box.AddChild(whole_box);
			}
		}
		private AppleDataBox GetDashAtoms(string meanstring,string namestring)
		{
			foreach(Box box in ilst_box.Children)
			{
				if(box.BoxType!=BoxType.DASH)
				{
					continue;
				}
				AppleAdditionalInfoBox mean_box=(AppleAdditionalInfoBox)box.GetChild(BoxType.Mean);
				AppleAdditionalInfoBox name_box=(AppleAdditionalInfoBox)box.GetChild(BoxType.Name);
				if(mean_box==null||name_box==null||mean_box.Text!=meanstring||name_box.Text!=namestring)
				{
					continue;
				}
				else
				{
					return(AppleDataBox)box.GetChild(BoxType.Data);
				}
			}
			return null;
		}
		private AppleAnnotationBox GetParentDashBox(string meanstring,string namestring)
		{
			foreach(Box box in ilst_box.Children)
			{
				if(box.BoxType!=BoxType.DASH)
				{
					continue;
				}
				AppleAdditionalInfoBox mean_box=(AppleAdditionalInfoBox)box.GetChild(BoxType.Mean);
				AppleAdditionalInfoBox name_box=(AppleAdditionalInfoBox)box.GetChild(BoxType.Name);
				if(mean_box==null||name_box==null||mean_box.Text!=meanstring||name_box.Text!=namestring)
				{
					continue;
				}
				else
				{
					return(AppleAnnotationBox)box;
				}
			}
			return null;
		}
#endregion
#region Internal Methods
		internal static ReadOnlyByteVector FixId(ByteVector id)
		{
			if(id.Count==4)
			{
				ReadOnlyByteVector roid=id as ReadOnlyByteVector;
				if(roid!=null)
				{
					return roid;
				}
				return new ReadOnlyByteVector(id);
			}
			if(id.Count==3)
			{
				return new ReadOnlyByteVector(0xa9,id[0],id[1],id[2]);
			}
			return null;
		}
#endregion
#region IEnumerable<Box>
		public IEnumerator<Box>GetEnumerator()
		{
			return ilst_box.Children.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ilst_box.Children.GetEnumerator();
		}
#endregion
#region TagLib.Tag
		public override TagTypes TagTypes
		{
			get
			{
				return TagTypes.Apple;
			}
		}
		public override string Title
		{
			get
			{
				string[]text=GetText(BoxType.Nam);
				return text.Length==0?null:text[0];
			}
			set
			{
				SetText(BoxType.Nam,value);
			}
		}
		public override string[]Performers
		{
			get
			{
				return GetText(BoxType.Art);
			}
			set
			{
				SetText(BoxType.Art,value);
			}
		}
		public override string[]AlbumArtists
		{
			get
			{
				return GetText(BoxType.Aart);
			}
			set
			{
				SetText(BoxType.Aart,value);
			}
		}
		public override string[]Composers
		{
			get
			{
				return GetText(BoxType.Wrt);
			}
			set
			{
				SetText(BoxType.Wrt,value);
			}
		}
		public override string Album
		{
			get
			{
				string[]text=GetText(BoxType.Alb);
				return text.Length==0?null:text[0];
			}
			set
			{
				SetText(BoxType.Alb,value);
			}
		}
		public override string Comment
		{
			get
			{
				string[]text=GetText(BoxType.Cmt);
				return text.Length==0?null:text[0];
			}
			set
			{
				SetText(BoxType.Cmt,value);
			}
		}
		public override string[]Genres
		{
			get
			{
				string[]text=GetText(BoxType.Gen);
				if(text.Length>0)
				{
					return text;
				}
				foreach(AppleDataBox box in DataBoxes(BoxType.Gnre))
				{
					if(box.Flags!=(int)AppleDataBox.FlagType.ContainsData)
					{
						continue;
					}
					ushort index=box.Data.ToUShort(true);
					if(index==0)
					{
						continue;
					}
					string str=TagLib.Genres.IndexToAudio((byte)(index-1));
					if(str==null)
					{
						continue;
					}
					text=new string[]
					{
						str
					};
					break;
				}
				return text;
			}
			set
			{
				ClearData(BoxType.Gnre);
				SetText(BoxType.Gen,value);
			}
		}
		public override uint Year
		{
			get
			{
				uint value;
				foreach(AppleDataBox box in DataBoxes(BoxType.Day))
				if(box.Text!=null&&(uint.TryParse(box.Text,out value)||uint.TryParse(box.Text.Length>4?box.Text.Substring(0,4):box.Text,out value)))
				{
					return value;
				}
				return 0;
			}
			set
			{
				if(value==0)
				{
					ClearData(BoxType.Day);
				}
				else
				{
					SetText(BoxType.Day,value.ToString(CultureInfo.InvariantCulture));
				}
			}
		}
		public override uint Track
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Trkn))
				if(box.Flags==(int)AppleDataBox.FlagType.ContainsData&&box.Data.Count>=4)
				{
					return box.Data.Mid(2,2).ToUShort();
				}
				return 0;
			}
			set
			{
				uint count=TrackCount;
				if(value==0&&count==0)
				{
					ClearData(BoxType.Trkn);
					return;
				}
				ByteVector v=ByteVector.FromUShort(0);
				v.Add(ByteVector.FromUShort((ushort)value));
				v.Add(ByteVector.FromUShort((ushort)count));
				v.Add(ByteVector.FromUShort(0));
				SetData(BoxType.Trkn,v,(int)AppleDataBox.FlagType.ContainsData);
			}
		}
		public override uint TrackCount
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Trkn))
				if(box.Flags==(int)AppleDataBox.FlagType.ContainsData&&box.Data.Count>=6)
				{
					return box.Data.Mid(4,2).ToUShort();
				}
				return 0;
			}
			set
			{
				uint track=Track;
				if(value==0&&track==0)
				{
					ClearData(BoxType.Trkn);
					return;
				}
				ByteVector v=ByteVector.FromUShort(0);
				v.Add(ByteVector.FromUShort((ushort)track));
				v.Add(ByteVector.FromUShort((ushort)value));
				v.Add(ByteVector.FromUShort(0));
				SetData(BoxType.Trkn,v,(int)AppleDataBox.FlagType.ContainsData);
			}
		}
		public override uint Disc
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Disk))
				if(box.Flags==(int)AppleDataBox.FlagType.ContainsData&&box.Data.Count>=4)
				{
					return box.Data.Mid(2,2).ToUShort();
				}
				return 0;
			}
			set
			{
				uint count=DiscCount;
				if(value==0&&count==0)
				{
					ClearData(BoxType.Disk);
					return;
				}
				ByteVector v=ByteVector.FromUShort(0);
				v.Add(ByteVector.FromUShort((ushort)value));
				v.Add(ByteVector.FromUShort((ushort)count));
				v.Add(ByteVector.FromUShort(0));
				SetData(BoxType.Disk,v,(int)AppleDataBox.FlagType.ContainsData);
			}
		}
		public override uint DiscCount
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Disk))
				if(box.Flags==(int)AppleDataBox.FlagType.ContainsData&&box.Data.Count>=6)
				{
					return box.Data.Mid(4,2).ToUShort();
				}
				return 0;
			}
			set
			{
				uint disc=Disc;
				if(value==0&&disc==0)
				{
					ClearData(BoxType.Disk);
					return;
				}
				ByteVector v=ByteVector.FromUShort(0);
				v.Add(ByteVector.FromUShort((ushort)disc));
				v.Add(ByteVector.FromUShort((ushort)value));
				v.Add(ByteVector.FromUShort(0));
				SetData(BoxType.Disk,v,(int)AppleDataBox.FlagType.ContainsData);
			}
		}
		public override string Lyrics
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Lyr))
				return box.Text;
				return null;
			}
			set
			{
				SetText(BoxType.Lyr,value);
			}
		}
		public override string Grouping
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Grp))
				return box.Text;
				return null;
			}
			set
			{
				SetText(BoxType.Grp,value);
			}
		}
		public override uint BeatsPerMinute
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Tmpo))
				if(box.Flags==(uint)AppleDataBox.FlagType.ForTempo)
				{
					return box.Data.ToUInt();
				}
				return 0;
			}
			set
			{
				if(value==0)
				{
					ClearData(BoxType.Tmpo);
					return;
				}
				SetData(BoxType.Tmpo,ByteVector.FromUShort((ushort)value),(uint)AppleDataBox.FlagType.ForTempo);
			}
		}
		public override string Conductor
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Cond))
				return box.Text;
				return null;
			}
			set
			{
				SetText(BoxType.Cond,value);
			}
		}
		public override string Copyright
		{
			get
			{
				foreach(AppleDataBox box in DataBoxes(BoxType.Cprt))
				return box.Text;
				return null;
			}
			set
			{
				SetText(BoxType.Cprt,value);
			}
		}
		public override string[]AlbumArtistsSort
		{
			get
			{
				return GetText(BoxType.Soaa);
			}
			set
			{
				SetText(BoxType.Soaa,value);
			}
		}
		public override string[]PerformersSort
		{
			get
			{
				return GetText(BoxType.Soar);
			}
			set
			{
				SetText(BoxType.Soar,value);
			}
		}
		public override string[]ComposersSort
		{
			get
			{
				return GetText(BoxType.Soco);
			}
			set
			{
				SetText(BoxType.Soco,value);
			}
		}
		public override string AlbumSort
		{
			get
			{
				string[]text=GetText(BoxType.Soal);
				return text.Length==0?null:text[0];
			}
			set
			{
				SetText(BoxType.Soal,value);
			}
		}
		public override string TitleSort
		{
			get
			{
				string[]text=GetText(BoxType.Sonm);
				return text.Length==0?null:text[0];
			}
			set
			{
				SetText(BoxType.Sonm,value);
			}
		}
		public override string MusicBrainzArtistId
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Artist Id");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Artist Id",value);
			}
		}
		public override string MusicBrainzReleaseId
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Album Id");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Album Id",value);
			}
		}
		public override string MusicBrainzReleaseArtistId
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Album Artist Id");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Album Artist Id",value);
			}
		}
		public override string MusicBrainzTrackId
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Track Id");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Track Id",value);
			}
		}
		public override string MusicBrainzDiscId
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Disc Id");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Disc Id",value);
			}
		}
		public override string MusicIpId
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicIP PUID");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicIP PUID",value);
			}
		}
		public override string AmazonId
		{
			get
			{
				return GetDashBox("com.apple.iTunes","ASIN");
			}
			set
			{
				SetDashBox("com.apple.iTunes","ASIN",value);
			}
		}
		public override string MusicBrainzReleaseStatus
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Album Status");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Album Status",value);
			}
		}
		public override string MusicBrainzReleaseType
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Album Type");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Album Type",value);
			}
		}
		public override string MusicBrainzReleaseCountry
		{
			get
			{
				return GetDashBox("com.apple.iTunes","MusicBrainz Album Release Country");
			}
			set
			{
				SetDashBox("com.apple.iTunes","MusicBrainz Album Release Country",value);
			}
		}
		public override double ReplayGainTrackGain
		{
			get
			{
				string text=GetDashBox("com.apple.iTunes","REPLAYGAIN_TRACK_GAIN");
				double value;
				if(text==null)
				{
					return double.NaN;
				}
				if(text.ToLower(CultureInfo.InvariantCulture).EndsWith("db"))
				{
					text=text.Substring(0,text.Length-2).Trim();
				}
				if(double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
				SetDashBox("com.apple.iTunes","REPLAYGAIN_TRACK_GAIN",text);
			}
		}
		public override double ReplayGainTrackPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetDashBox("com.apple.iTunes","REPLAYGAIN_TRACK_PEAK"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
				SetDashBox("com.apple.iTunes","REPLAYGAIN_TRACK_PEAK",text);
			}
		}
		public override double ReplayGainAlbumGain
		{
			get
			{
				string text=GetDashBox("com.apple.iTunes","REPLAYGAIN_ALBUM_GAIN");
				double value;
				if(text==null)
				{
					return double.NaN;
				}
				if(text.ToLower(CultureInfo.InvariantCulture).EndsWith("db"))
				{
					text=text.Substring(0,text.Length-2).Trim();
				}
				if(double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				string text=value.ToString("0.00 dB",CultureInfo.InvariantCulture);
				SetDashBox("com.apple.iTunes","REPLAYGAIN_ALBUM_GAIN",text);
			}
		}
		public override double ReplayGainAlbumPeak
		{
			get
			{
				string text;
				double value;
				if((text=GetDashBox("com.apple.iTunes","REPLAYGAIN_ALBUM_PEAK"))!=null&&double.TryParse(text,NumberStyles.Float,CultureInfo.InvariantCulture,out value))
				{
					return value;
				}
				return double.NaN;
			}
			set
			{
				string text=value.ToString("0.000000",CultureInfo.InvariantCulture);
				SetDashBox("com.apple.iTunes","REPLAYGAIN_ALBUM_PEAK",text);
			}
		}
		public override IPicture[]Pictures
		{
			get
			{
				List<Picture>l=new List<Picture>();
				foreach(AppleDataBox box in DataBoxes(BoxType.Covr))
				{
					Picture p=new Picture(box.Data);
					p.Type=PictureType.FrontCover;
					l.Add(p);
				}
				return(Picture[])l.ToArray();
			}
			set
			{
				if(value==null||value.Length==0)
				{
					ClearData(BoxType.Covr);
					return;
				}
				AppleDataBox[]boxes=new AppleDataBox[value.Length];
				for(int i=0;i<value.Length;i++)
				{
					uint type=(uint)AppleDataBox.FlagType.ContainsData;
					if(value[i].MimeType=="image/jpeg")
					{
						type=(uint)AppleDataBox.FlagType.ContainsJpegData;
					}
					else if(value[i].MimeType=="image/png")
					{
						type=(uint)AppleDataBox.FlagType.ContainsPngData;
					}
					boxes[i]=new AppleDataBox(value[i].Data,type);
				}
				SetData(BoxType.Covr,boxes);
			}
		}
		public override bool IsEmpty
		{
			get
			{
				return!ilst_box.HasChildren;
			}
		}
		public override void Clear()
		{
			ilst_box.ClearChildren();
		}
#endregion
	}
}
