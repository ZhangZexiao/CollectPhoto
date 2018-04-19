using System;
using System.Linq;
namespace CollectPhoto
{
	struct PropertyIdAndDescription
	{
		public int id;
		public string description;
		public PropertyIdAndDescription(int id,string description)
		{
			this.id=id;
			this.description=description;
		}
	}
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private System.DateTime parseDateTaken(string value)
		{
			System.DateTime result;
			System.DateTime.TryParseExact(value,CultureStrings.FormatDateTaken,System.Globalization.CultureInfo.CurrentCulture,System.Globalization.DateTimeStyles.None,out result);
			return result;
		}
		private System.DateTime parseTaggedDate(string value)
		{
			System.DateTime result;
			System.DateTime.TryParseExact(value,CultureStrings.FormatTaggedDate,System.Globalization.CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None,out result);
			return result;
		}
		private const int fileSizeId= -1;
		private const int originalFileNameId= -2;
		private const int imageHeightId= -3;
		private const int imageWidthId= -4;
		private const int latitudeLongitudeId= -5;
		private const int dateModifiedId= -6;
		private const int dateCreatedId= -7;
		private const int fileExtensionId= -8;
		private const int manufacturerId=271;
		private const int modelId=272;
		private const int dateTakenId=36867;
		private const int dateTimeId=306;
		private string getPropertyValue(string imagePath,System.Drawing.Image image,int id)
		{
			try
			{
				if(image.PropertyIdList.Contains(id))
				{
					return getPropertyValue(image.GetPropertyItem(id));
				}
				else
				{
					TagLib.Image.File tagImage=TagLib.File.Create(imagePath)
					as TagLib.Image.File;
					string result=null;
					if(id==manufacturerId&&tagImage.ImageTag.Make!=null&&tagImage.ImageTag.Make.Length>0)
					{
						result=tagImage.ImageTag.Make;
					}
					else if(id==modelId&&tagImage.ImageTag.Model!=null&&tagImage.ImageTag.Model.Length>0)
					{
						result=tagImage.ImageTag.Model;
					}
					else if(id==dateTakenId&&tagImage.ImageTag.DateTime.HasValue)
					{
						result=tagImage.ImageTag.DateTime.Value.ToString(CultureStrings.FormatDateTaken);
					}
					else if(id==dateTakenId&& !tagImage.ImageTag.DateTime.HasValue)
					{
						result=getPropertyValue(image.GetPropertyItem(dateTimeId));
					}
					else if(id==latitudeLongitudeId&&tagImage.ImageTag.Latitude.HasValue&&tagImage.ImageTag.Longitude.HasValue)
					{
						result=tagImage.ImageTag.Latitude+","+tagImage.ImageTag.Longitude;
					}
					tagImage.Dispose();
					if(result!=null)
					{
						return result;
					}
					else
					{
						FileWriterWithAppendMode.GlobalWrite(string.Format(CultureStrings.InfomationImageProperties,doubleQuoteText(imagePath),id,convertIntsToText(image.PropertyIdList)));
					}
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return null;
		}
		private string getUInt16Property(ref System.Drawing.Imaging.PropertyItem property)
		{
			int i=0;
			string text="";
			while(i<property.Len)
			{
				text+=System.BitConverter.ToUInt16(property.Value,i).ToString();
				text+=CultureStrings.MarkerWhiteSpace;
				i+=2;
			}
			return text;
		}
		private string getUInt32Property(ref System.Drawing.Imaging.PropertyItem property)
		{
			int i=0;
			string text="";
			while(i<property.Len)
			{
				text+=System.BitConverter.ToUInt32(property.Value,i).ToString();
				text+=CultureStrings.MarkerWhiteSpace;
				i+=4;
			}
			return text;
		}
		private string getAsciiProperty(ref System.Drawing.Imaging.PropertyItem property)
		{
			return new System.Text.ASCIIEncoding().GetString(property.Value,0,property.Len-1);
		}
		private string getUInt32UInt32Property(ref System.Drawing.Imaging.PropertyItem property)
		{
			return System.BitConverter.ToUInt32(property.Value,0).ToString()+CultureStrings.MarkerSlash+System.BitConverter.ToUInt32(property.Value,4).ToString();
		}
		private string getHexProperty(ref System.Drawing.Imaging.PropertyItem property)
		{
			int i=0;
			string text="";
			while(i<property.Len)
			{
				text+=property.Value[i].ToString(CultureStrings.FormatByteToHex);
				text+=" ";
				i+=1;
			}
			return text;
		}
		private string getPropertyValue(System.Drawing.Imaging.PropertyItem property)
		{
			if(property.Type==2)
			{
				return getAsciiProperty(ref property);
			}
			else if(property.Type==3)
			{
				return getUInt16Property(ref property);
			}
			else if(property.Type==4)
			{
				return getUInt32Property(ref property);
			}
			else if(property.Type==5)
			{
				return getUInt32UInt32Property(ref property);
			}
			else
			{
				return getHexProperty(ref property);
			}
		}
		readonly PropertyIdAndDescription[]idDescriptions=
		{
			new PropertyIdAndDescription(0x0000,"PropertyTagGpsVer"),
			new PropertyIdAndDescription(0x0001,"PropertyTagGpsLatitudeRef"),
			new PropertyIdAndDescription(0x0002,"PropertyTagGpsLatitude"),
			new PropertyIdAndDescription(0x0003,"PropertyTagGpsLongitudeRef"),
			new PropertyIdAndDescription(0x0004,"PropertyTagGpsLongitude"),
			new PropertyIdAndDescription(0x0005,"PropertyTagGpsAltitudeRef"),
			new PropertyIdAndDescription(0x0006,"PropertyTagGpsAltitude"),
			new PropertyIdAndDescription(0x0007,"PropertyTagGpsGpsTime"),
			new PropertyIdAndDescription(0x0008,"PropertyTagGpsGpsSatellites"),
			new PropertyIdAndDescription(0x0009,"PropertyTagGpsGpsStatus"),
			new PropertyIdAndDescription(0x000A,"PropertyTagGpsGpsMeasureMode"),
			new PropertyIdAndDescription(0x000B,"PropertyTagGpsGpsDop"),
			new PropertyIdAndDescription(0x000C,"PropertyTagGpsSpeedRef"),
			new PropertyIdAndDescription(0x000D,"PropertyTagGpsSpeed"),
			new PropertyIdAndDescription(0x000E,"PropertyTagGpsTrackRef"),
			new PropertyIdAndDescription(0x000F,"PropertyTagGpsTrack"),
			new PropertyIdAndDescription(0x0010,"PropertyTagGpsImgDirRef"),
			new PropertyIdAndDescription(0x0011,"PropertyTagGpsImgDir"),
			new PropertyIdAndDescription(0x0012,"PropertyTagGpsMapDatum"),
			new PropertyIdAndDescription(0x0013,"PropertyTagGpsDestLatRef"),
			new PropertyIdAndDescription(0x0014,"PropertyTagGpsDestLat"),
			new PropertyIdAndDescription(0x0015,"PropertyTagGpsDestLongRef"),
			new PropertyIdAndDescription(0x0016,"PropertyTagGpsDestLong"),
			new PropertyIdAndDescription(0x0017,"PropertyTagGpsDestBearRef"),
			new PropertyIdAndDescription(0x0018,"PropertyTagGpsDestBear"),
			new PropertyIdAndDescription(0x0019,"PropertyTagGpsDestDistRef"),
			new PropertyIdAndDescription(0x001A,"PropertyTagGpsDestDist"),
			new PropertyIdAndDescription(0x00FE,"PropertyTagNewSubfileType"),
			new PropertyIdAndDescription(0x00FF,"PropertyTagSubfileType"),
			new PropertyIdAndDescription(0x0100,"PropertyTagImageWidth"),
			new PropertyIdAndDescription(0x0101,"PropertyTagImageHeight"),
			new PropertyIdAndDescription(0x0102,"PropertyTagBitsPerSample"),
			new PropertyIdAndDescription(0x0103,"PropertyTagCompression"),
			new PropertyIdAndDescription(0x0106,"PropertyTagPhotometricInterp"),
			new PropertyIdAndDescription(0x0107,"PropertyTagThreshHolding"),
			new PropertyIdAndDescription(0x0108,"PropertyTagCellWidth"),
			new PropertyIdAndDescription(0x0109,"PropertyTagCellHeight"),
			new PropertyIdAndDescription(0x010A,"PropertyTagFillOrder"),
			new PropertyIdAndDescription(0x010D,"PropertyTagDocumentName"),
			new PropertyIdAndDescription(0x010E,"PropertyTagImageDescription"),
			new PropertyIdAndDescription(0x010F,"PropertyTagEquipMake"),
			new PropertyIdAndDescription(0x0110,"PropertyTagEquipModel"),
			new PropertyIdAndDescription(0x0111,"PropertyTagStripOffsets"),
			new PropertyIdAndDescription(0x0112,"PropertyTagOrientation"),
			new PropertyIdAndDescription(0x0115,"PropertyTagSamplesPerPixel"),
			new PropertyIdAndDescription(0x0116,"PropertyTagRowsPerStrip"),
			new PropertyIdAndDescription(0x0117,"PropertyTagStripBytesCount"),
			new PropertyIdAndDescription(0x0118,"PropertyTagMinSampleValue"),
			new PropertyIdAndDescription(0x0119,"PropertyTagMaxSampleValue"),
			new PropertyIdAndDescription(0x011A,"PropertyTagXResolution"),
			new PropertyIdAndDescription(0x011B,"PropertyTagYResolution"),
			new PropertyIdAndDescription(0x011C,"PropertyTagPlanarConfig"),
			new PropertyIdAndDescription(0x011D,"PropertyTagPageName"),
			new PropertyIdAndDescription(0x011E,"PropertyTagXPosition"),
			new PropertyIdAndDescription(0x011F,"PropertyTagYPosition"),
			new PropertyIdAndDescription(0x0120,"PropertyTagFreeOffset"),
			new PropertyIdAndDescription(0x0121,"PropertyTagFreeByteCounts"),
			new PropertyIdAndDescription(0x0122,"PropertyTagGrayResponseUnit"),
			new PropertyIdAndDescription(0x0123,"PropertyTagGrayResponseCurve"),
			new PropertyIdAndDescription(0x0124,"PropertyTagT4Option"),
			new PropertyIdAndDescription(0x0125,"PropertyTagT6Option"),
			new PropertyIdAndDescription(0x0128,"PropertyTagResolutionUnit"),
			new PropertyIdAndDescription(0x0129,"PropertyTagPageNumber"),
			new PropertyIdAndDescription(0x012D,"PropertyTagTransferFunction"),
			new PropertyIdAndDescription(0x0131,"PropertyTagSoftwareUsed"),
			new PropertyIdAndDescription(0x0132,"PropertyTagDateTime"),
			new PropertyIdAndDescription(0x013B,"PropertyTagArtist"),
			new PropertyIdAndDescription(0x013C,"PropertyTagHostComputer"),
			new PropertyIdAndDescription(0x013D,"PropertyTagPredictor"),
			new PropertyIdAndDescription(0x013E,"PropertyTagWhitePoint"),
			new PropertyIdAndDescription(0x013F,"PropertyTagPrimaryChromaticities"),
			new PropertyIdAndDescription(0x0140,"PropertyTagColorMap"),
			new PropertyIdAndDescription(0x0141,"PropertyTagHalftoneHints"),
			new PropertyIdAndDescription(0x0142,"PropertyTagTileWidth"),
			new PropertyIdAndDescription(0x0143,"PropertyTagTileLength"),
			new PropertyIdAndDescription(0x0144,"PropertyTagTileOffset"),
			new PropertyIdAndDescription(0x0145,"PropertyTagTileByteCounts"),
			new PropertyIdAndDescription(0x014C,"PropertyTagInkSet"),
			new PropertyIdAndDescription(0x014D,"PropertyTagInkNames"),
			new PropertyIdAndDescription(0x014E,"PropertyTagNumberOfInks"),
			new PropertyIdAndDescription(0x0150,"PropertyTagDotRange"),
			new PropertyIdAndDescription(0x0151,"PropertyTagTargetPrinter"),
			new PropertyIdAndDescription(0x0152,"PropertyTagExtraSamples"),
			new PropertyIdAndDescription(0x0153,"PropertyTagSampleFormat"),
			new PropertyIdAndDescription(0x0154,"PropertyTagSMinSampleValue"),
			new PropertyIdAndDescription(0x0155,"PropertyTagSMaxSampleValue"),
			new PropertyIdAndDescription(0x0156,"PropertyTagTransferRange"),
			new PropertyIdAndDescription(0x0200,"PropertyTagJPEGProc"),
			new PropertyIdAndDescription(0x0201,"PropertyTagJPEGInterFormat"),
			new PropertyIdAndDescription(0x0202,"PropertyTagJPEGInterLength"),
			new PropertyIdAndDescription(0x0203,"PropertyTagJPEGRestartInterval"),
			new PropertyIdAndDescription(0x0205,"PropertyTagJPEGLosslessPredictors"),
			new PropertyIdAndDescription(0x0206,"PropertyTagJPEGPointTransforms"),
			new PropertyIdAndDescription(0x0207,"PropertyTagJPEGQTables"),
			new PropertyIdAndDescription(0x0208,"PropertyTagJPEGDCTables"),
			new PropertyIdAndDescription(0x0209,"PropertyTagJPEGACTables"),
			new PropertyIdAndDescription(0x0211,"PropertyTagYCbCrCoefficients"),
			new PropertyIdAndDescription(0x0212,"PropertyTagYCbCrSubsampling"),
			new PropertyIdAndDescription(0x0213,"PropertyTagYCbCrPositioning"),
			new PropertyIdAndDescription(0x0214,"PropertyTagREFBlackWhite"),
			new PropertyIdAndDescription(0x0301,"PropertyTagGamma"),
			new PropertyIdAndDescription(0x0302,"PropertyTagICCProfileDescriptor"),
			new PropertyIdAndDescription(0x0303,"PropertyTagSRGBRenderingIntent"),
			new PropertyIdAndDescription(0x0320,"PropertyTagImageTitle"),
			new PropertyIdAndDescription(0x5001,"PropertyTagResolutionXUnit"),
			new PropertyIdAndDescription(0x5002,"PropertyTagResolutionYUnit"),
			new PropertyIdAndDescription(0x5003,"PropertyTagResolutionXLengthUnit"),
			new PropertyIdAndDescription(0x5004,"PropertyTagResolutionYLengthUnit"),
			new PropertyIdAndDescription(0x5005,"PropertyTagPrintFlags"),
			new PropertyIdAndDescription(0x5006,"PropertyTagPrintFlagsVersion"),
			new PropertyIdAndDescription(0x5007,"PropertyTagPrintFlagsCrop"),
			new PropertyIdAndDescription(0x5008,"PropertyTagPrintFlagsBleedWidth"),
			new PropertyIdAndDescription(0x5009,"PropertyTagPrintFlagsBleedWidthScale"),
			new PropertyIdAndDescription(0x500A,"PropertyTagHalftoneLPI"),
			new PropertyIdAndDescription(0x500B,"PropertyTagHalftoneLPIUnit"),
			new PropertyIdAndDescription(0x500C,"PropertyTagHalftoneDegree"),
			new PropertyIdAndDescription(0x500D,"PropertyTagHalftoneShape"),
			new PropertyIdAndDescription(0x500E,"PropertyTagHalftoneMisc"),
			new PropertyIdAndDescription(0x500F,"PropertyTagHalftoneScreen"),
			new PropertyIdAndDescription(0x5010,"PropertyTagJPEGQuality"),
			new PropertyIdAndDescription(0x5011,"PropertyTagGridSize"),
			new PropertyIdAndDescription(0x5012,"PropertyTagThumbnailFormat"),
			new PropertyIdAndDescription(0x5013,"PropertyTagThumbnailWidth"),
			new PropertyIdAndDescription(0x5014,"PropertyTagThumbnailHeight"),
			new PropertyIdAndDescription(0x5015,"PropertyTagThumbnailColorDepth"),
			new PropertyIdAndDescription(0x5016,"PropertyTagThumbnailPlanes"),
			new PropertyIdAndDescription(0x5017,"PropertyTagThumbnailRawBytes"),
			new PropertyIdAndDescription(0x5018,"PropertyTagThumbnailSize"),
			new PropertyIdAndDescription(0x5019,"PropertyTagThumbnailCompressedSize"),
			new PropertyIdAndDescription(0x501A,"PropertyTagColorTransferFunction"),
			new PropertyIdAndDescription(0x501B,"PropertyTagThumbnailData"),
			new PropertyIdAndDescription(0x5020,"PropertyTagThumbnailImageWidth"),
			new PropertyIdAndDescription(0x5021,"PropertyTagThumbnailImageHeight"),
			new PropertyIdAndDescription(0x5022,"PropertyTagThumbnailBitsPerSample"),
			new PropertyIdAndDescription(0x5023,"PropertyTagThumbnailCompression"),
			new PropertyIdAndDescription(0x5024,"PropertyTagThumbnailPhotometricInterp"),
			new PropertyIdAndDescription(0x5025,"PropertyTagThumbnailImageDescription"),
			new PropertyIdAndDescription(0x5026,"PropertyTagThumbnailEquipMake"),
			new PropertyIdAndDescription(0x5027,"PropertyTagThumbnailEquipModel"),
			new PropertyIdAndDescription(0x5028,"PropertyTagThumbnailStripOffsets"),
			new PropertyIdAndDescription(0x5029,"PropertyTagThumbnailOrientation"),
			new PropertyIdAndDescription(0x502A,"PropertyTagThumbnailSamplesPerPixel"),
			new PropertyIdAndDescription(0x502B,"PropertyTagThumbnailRowsPerStrip"),
			new PropertyIdAndDescription(0x502C,"PropertyTagThumbnailStripBytesCount"),
			new PropertyIdAndDescription(0x502D,"PropertyTagThumbnailResolutionX"),
			new PropertyIdAndDescription(0x502E,"PropertyTagThumbnailResolutionY"),
			new PropertyIdAndDescription(0x502F,"PropertyTagThumbnailPlanarConfig"),
			new PropertyIdAndDescription(0x5030,"PropertyTagThumbnailResolutionUnit"),
			new PropertyIdAndDescription(0x5031,"PropertyTagThumbnailTransferFunction"),
			new PropertyIdAndDescription(0x5032,"PropertyTagThumbnailSoftwareUsed"),
			new PropertyIdAndDescription(0x5033,"PropertyTagThumbnailDateTime"),
			new PropertyIdAndDescription(0x5034,"PropertyTagThumbnailArtist"),
			new PropertyIdAndDescription(0x5035,"PropertyTagThumbnailWhitePoint"),
			new PropertyIdAndDescription(0x5036,"PropertyTagThumbnailPrimaryChromaticities"),
			new PropertyIdAndDescription(0x5037,"PropertyTagThumbnailYCbCrCoefficients"),
			new PropertyIdAndDescription(0x5038,"PropertyTagThumbnailYCbCrSubsampling"),
			new PropertyIdAndDescription(0x5039,"PropertyTagThumbnailYCbCrPositioning"),
			new PropertyIdAndDescription(0x503A,"PropertyTagThumbnailRefBlackWhite"),
			new PropertyIdAndDescription(0x503B,"PropertyTagThumbnailCopyRight"),
			new PropertyIdAndDescription(0x5090,"PropertyTagLuminanceTable"),
			new PropertyIdAndDescription(0x5091,"PropertyTagChrominanceTable"),
			new PropertyIdAndDescription(0x5100,"PropertyTagFrameDelay"),
			new PropertyIdAndDescription(0x5101,"PropertyTagLoopCount"),
			new PropertyIdAndDescription(0x5102,"PropertyTagGlobalPalette"),
			new PropertyIdAndDescription(0x5103,"PropertyTagIndexBackground"),
			new PropertyIdAndDescription(0x5104,"PropertyTagIndexTransparent"),
			new PropertyIdAndDescription(0x5110,"PropertyTagPixelUnit"),
			new PropertyIdAndDescription(0x5111,"PropertyTagPixelPerUnitX"),
			new PropertyIdAndDescription(0x5112,"PropertyTagPixelPerUnitY"),
			new PropertyIdAndDescription(0x5113,"PropertyTagPaletteHistogram"),
			new PropertyIdAndDescription(0x8298,"PropertyTagCopyright"),
			new PropertyIdAndDescription(0x829A,"PropertyTagExifExposureTime"),
			new PropertyIdAndDescription(0x829D,"PropertyTagExifFNumber"),
			new PropertyIdAndDescription(0x8769,"PropertyTagExifIFD"),
			new PropertyIdAndDescription(0x8773,"PropertyTagICCProfile"),
			new PropertyIdAndDescription(0x8822,"PropertyTagExifExposureProg"),
			new PropertyIdAndDescription(0x8824,"PropertyTagExifSpectralSense"),
			new PropertyIdAndDescription(0x8825,"PropertyTagGpsIFD"),
			new PropertyIdAndDescription(0x8827,"PropertyTagExifISOSpeed"),
			new PropertyIdAndDescription(0x8828,"PropertyTagExifOECF"),
			new PropertyIdAndDescription(0x9000,"PropertyTagExifVer"),
			new PropertyIdAndDescription(0x9003,"PropertyTagExifDTOrig"),
			new PropertyIdAndDescription(0x9004,"PropertyTagExifDTDigitized"),
			new PropertyIdAndDescription(0x9101,"PropertyTagExifCompConfig"),
			new PropertyIdAndDescription(0x9102,"PropertyTagExifCompBPP"),
			new PropertyIdAndDescription(0x9201,"PropertyTagExifShutterSpeed"),
			new PropertyIdAndDescription(0x9202,"PropertyTagExifAperture"),
			new PropertyIdAndDescription(0x9203,"PropertyTagExifBrightness"),
			new PropertyIdAndDescription(0x9204,"PropertyTagExifExposureBias"),
			new PropertyIdAndDescription(0x9205,"PropertyTagExifMaxAperture"),
			new PropertyIdAndDescription(0x9206,"PropertyTagExifSubjectDist"),
			new PropertyIdAndDescription(0x9207,"PropertyTagExifMeteringMode"),
			new PropertyIdAndDescription(0x9208,"PropertyTagExifLightSource"),
			new PropertyIdAndDescription(0x9209,"PropertyTagExifFlash"),
			new PropertyIdAndDescription(0x920A,"PropertyTagExifFocalLength"),
			new PropertyIdAndDescription(0x927C,"PropertyTagExifMakerNote"),
			new PropertyIdAndDescription(0x9286,"PropertyTagExifUserComment"),
			new PropertyIdAndDescription(0x9290,"PropertyTagExifDTSubsec"),
			new PropertyIdAndDescription(0x9291,"PropertyTagExifDTOrigSS"),
			new PropertyIdAndDescription(0x9292,"PropertyTagExifDTDigSS"),
			new PropertyIdAndDescription(0xA000,"PropertyTagExifFPXVer"),
			new PropertyIdAndDescription(0xA001,"PropertyTagExifColorSpace"),
			new PropertyIdAndDescription(0xA002,"PropertyTagExifPixXDim"),
			new PropertyIdAndDescription(0xA003,"PropertyTagExifPixYDim"),
			new PropertyIdAndDescription(0xA004,"PropertyTagExifRelatedWav"),
			new PropertyIdAndDescription(0xA005,"PropertyTagExifInterop"),
			new PropertyIdAndDescription(0xA20B,"PropertyTagExifFlashEnergy"),
			new PropertyIdAndDescription(0xA20C,"PropertyTagExifSpatialFR"),
			new PropertyIdAndDescription(0xA20E,"PropertyTagExifFocalXRes"),
			new PropertyIdAndDescription(0xA20F,"PropertyTagExifFocalYRes"),
			new PropertyIdAndDescription(0xA210,"PropertyTagExifFocalResUnit"),
			new PropertyIdAndDescription(0xA214,"PropertyTagExifSubjectLoc"),
			new PropertyIdAndDescription(0xA215,"PropertyTagExifExposureIndex"),
			new PropertyIdAndDescription(0xA217,"PropertyTagExifSensingMethod"),
			new PropertyIdAndDescription(0xA300,"PropertyTagExifFileSource"),
			new PropertyIdAndDescription(0xA301,"PropertyTagExifSceneType"),
			new PropertyIdAndDescription(0xA302,"PropertyTagExifCfaPattern")
		};
		private string getPropertyTagDescription(int id)
		{
			foreach(PropertyIdAndDescription idDescription in idDescriptions)
			{
				if(idDescription.id==id)
				{
					return idDescription.description.Substring(11);
				}
			}
			return CultureStrings.TextUnknown;
		}
	}
}
