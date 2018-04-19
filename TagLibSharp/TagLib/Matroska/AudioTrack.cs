using System.Collections.Generic;
using System;
namespace TagLib.Matroska
{
	public class AudioTrack:
	Track,IAudioCodec
	{
#region Private fields
#pragma warning disable 414 
		private double rate;
		private uint channels;
		private uint depth;
#pragma warning restore 414
		private List<EBMLElement>unknown_elems=new List<EBMLElement>();
#endregion
#region Constructors
		public AudioTrack(File _file,EBMLElement element):
		base(_file,element)
		{
			MatroskaID matroska_id;
			foreach(EBMLElement elem in base.UnknownElements)
			{
				matroska_id=(MatroskaID)elem.ID;
				if(matroska_id==MatroskaID.MatroskaTrackAudio)
				{
					ulong i=0;
					while(i<elem.DataSize)
					{
						EBMLElement child=new EBMLElement(_file,elem.DataOffset+i);
						matroska_id=(MatroskaID)child.ID;
						switch(matroska_id)
						{
						case MatroskaID.MatroskaAudioChannels:
							channels=child.ReadUInt();
							break;
						case MatroskaID.MatroskaAudioBitDepth:
							depth=child.ReadUInt();
							break;
						case MatroskaID.MatroskaAudioSamplingFreq:
							rate=child.ReadDouble();
							break;
						default:
							unknown_elems.Add(child);
							break;
						}
						i+=child.Size;
					}
				}
				else
				{
					unknown_elems.Add(elem);
				}
			}
		}
#endregion
#region Public fields
		public new List<EBMLElement>UnknownElements
		{
			get
			{
				return unknown_elems;
			}
		}
#endregion
#region Public methods
#endregion
#region ICodec
		public override MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.Audio;
			}
		}
#endregion
#region IAudioCodec
		public int AudioBitrate
		{
			get
			{
				return 0;
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return(int)rate;
			}
		}
		public int AudioChannels
		{
			get
			{
				return(int)channels;
			}
		}
#endregion
	}
}
