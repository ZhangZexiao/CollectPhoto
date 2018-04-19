using System;
using System.Linq;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private void createDirectoryForFile(string filePath)
		{
			string directoryPath=System.IO.Path.GetDirectoryName(filePath);
			if(!System.IO.Directory.Exists(directoryPath))
			{
				System.IO.Directory.CreateDirectory(directoryPath);
			}
		}
		bool isFileSizeNumber(string value)
		{
			try
			{
				if(value.Contains(','))
				{
					return int.Parse(value.Split(new char[]{','}).Aggregate((result,item)=>result+item))>0;
				}
				else
				{
					return int.Parse(value)>=0;
				}
			}
			catch(System.Exception)
			{
				return false;
			}
		}
		bool isAutoRenamedPart(string value)
		{
			string autoRenamedPart=this.getDotNumberString(0);
			try
			{
				if(value.StartsWith(autoRenamedPart.Substring(0,1))&&value.Length>1)
				{
					int versionNumber=int.Parse(value.Substring(1));
					if(versionNumber>=0&&getDotNumberString(versionNumber)==value)
					{
						return true;
					}
				}
			}
			catch(System.Exception)
			{
			}
			return false;
		}
		string getCollectPhotoOriginalFileName(string filePath)
		{
			string fileName=System.IO.Path.GetFileNameWithoutExtension(filePath);
			try
			{
				if(fileName.Contains(';'))
				{
					string[]originalName=fileName.Split(new char[]{';'});
					string newFileName="";
					for(int i=0;i<originalName.Length;i++)
					{
						if(originalName[i].Length>0&&!isFileSizeNumber(originalName[i])&&!isAutoRenamedPart(originalName[i]))
						{
							if(newFileName.Length>0)
							{
								newFileName+=";";
							}
							newFileName+=originalName[i];
						}
					}
					return newFileName+System.IO.Path.GetExtension(filePath);
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return System.IO.Path.GetFileName(filePath);
		}
		private string buildUniqueDestinationFilePath(ThreadParameters parameters,string sourcePath)
		{
			MediaInfoLib.MediaInfo mediaInfo=new MediaInfoLib.MediaInfo();
			string taggedDate=null;
			if(mediaInfo.Open(sourcePath)==1)
			{
				taggedDate=mediaInfo.Get(MediaInfoLib.StreamKind.General,0,CultureStrings.TextTaggedDate);
			}
			mediaInfo.Close();
			string uniqueDestinationFilePath=null;
			if(taggedDate!=null&&taggedDate.Length>0)
			{
				try
				{
					System.DateTime dateTime=parseTaggedDate(taggedDate);
					uniqueDestinationFilePath=normalizeDirectoryPath(parameters.DestinationRootDirectory)+normalizeDirectoryPath(System.IO.Path.GetExtension(sourcePath).ToUpper().Substring(1))+normalizeDirectoryPath(dateTime.ToString("yyyy"))+normalizeDirectoryPath(dateTime.ToString("MMMM"))+getCollectPhotoOriginalFileName(sourcePath);
				}
				catch(System.Exception ex)
				{
					logException(ex);
				}
			}
			if(uniqueDestinationFilePath==null)
			{
				uniqueDestinationFilePath=normalizeDirectoryPath(parameters.DestinationRootDirectory)+normalizeDirectoryPath(System.IO.Path.GetExtension(sourcePath).ToUpper().Substring(1))+getCollectPhotoOriginalFileName(sourcePath);
			}
			if(getDuplicateAutoRenamedFile(sourcePath,uniqueDestinationFilePath)!=null)
			{
				return getDuplicateAutoRenamedFile(sourcePath,uniqueDestinationFilePath);
			}
			else
			{
				return getUnusedFilePath(uniqueDestinationFilePath);
			}
		}
		private static string normalizeDirectoryPath(string directoryPath)
		{
			if(object.ReferenceEquals(directoryPath,null))
			{
				return string.Empty;
			}
			directoryPath=directoryPath.Trim();
			if(directoryPath.Length==0)
			{
				return string.Empty;
			}
			directoryPath=changeDirectorySeparatorChar(directoryPath,System.IO.Path.DirectorySeparatorChar);
			if(!endWith(directoryPath,System.IO.Path.DirectorySeparatorChar))
			{
				return directoryPath+System.IO.Path.DirectorySeparatorChar;
			}
			return directoryPath;
		}
		private string buildImageDestinationFilePath(ThreadParameters parameters,string sourcePath,System.Drawing.Image image)
		{
			if(parameters.DestinationRootDirectory.Length==0||parameters.DestinationSubDirectories.Length==0||parameters.FileNameComponent.Length==0||sourcePath.Length==0)
			{
				return null;
			}
			for(int i=0;i<parameters.DestinationSubDirectories.Length&&i<parameters.FileNameComponent.Length;i++)
			{
				string directoryPath=fillParameter(parameters.DestinationSubDirectories[i],sourcePath,image);
				if(directoryPath==null)
				{
					continue;
				}
				string fileName=fillParameter(parameters.FileNameComponent[i],sourcePath,image);
				if(fileName==null)
				{
					continue;
				}
				string destinationPath=normalizeDirectoryPath(parameters.DestinationRootDirectory)+normalizeDirectoryPath(directoryPath)+fileName+System.IO.Path.GetExtension(sourcePath);
				string destinationFileName=System.IO.Path.GetFileNameWithoutExtension(destinationPath);
				if(destinationFileName.EndsWith(CultureStrings.MarkerSemicolon)&&parameters.FileNameComponent[i].IndexOf(CultureStrings.ComponentOriginalFileName)!= -1)
				{
					string newDestinationFileName=destinationFileName.Remove(destinationFileName.LastIndexOf(CultureStrings.MarkerSemicolon));
					destinationPath=System.IO.Path.Combine(System.IO.Path.GetDirectoryName(destinationPath),newDestinationFileName+System.IO.Path.GetExtension(destinationPath));
				}
				return destinationPath;
			}
			return null;
		}
		private string fillCommandParameter(string format,int key,string value,int index)
		{
			int beginIndex=format.IndexOf('{',index);
			if(beginIndex!= -1)
			{
				int endIndex=format.IndexOf('}',beginIndex);
				if(endIndex!=-1)
				{
					string parameter=format.Substring(beginIndex+1,endIndex-beginIndex-1);
					string quotedParameter=format.Substring(beginIndex,endIndex-beginIndex+1);
					string[]parameterComponents=parameter.Split(new char[]{':'});
					int id=int.Parse(parameterComponents[0]);
					if(id==key)
					{
						return fillCommandParameter(format.Replace(quotedParameter,value),key,value,0);
					}
					else
					{
						return fillCommandParameter(format,key,value,endIndex);
					}
				}
			}
			return format;
		}
		private string fillParameter(string format,string sourcePath,System.Drawing.Image image)
		{
			int beginIndex=format.IndexOf('{');
			int endIndex=format.IndexOf('}');
			if(beginIndex!= -1&&endIndex!= -1)
			{
				string parameter=format.Substring(beginIndex+1,endIndex-beginIndex-1);
				string quotedParameter=format.Substring(beginIndex,endIndex-beginIndex+1);
				string[]parameterComponents=parameter.Split(new char[]{':'});
				int id=int.Parse(parameterComponents[0]);
				if(id==fileSizeId)
				{
					return fillParameter(format.Replace(quotedParameter,getFileSize(sourcePath).ToString(CultureStrings.FormatIntWithComma)),sourcePath,image);
				}
				else if(id==originalFileNameId)
				{
					return fillParameter(System.IO.Path.GetFileNameWithoutExtension(getCollectPhotoOriginalFileName(sourcePath))+CultureStrings.MarkerSemicolon+format.Remove(format.IndexOf(quotedParameter),quotedParameter.Length),sourcePath,image);
				}
				else if(id==imageHeightId)
				{
					if(image==null)
					{
						return null;
					}
					else
					{
						return fillParameter(format.Replace(quotedParameter,image.Height.ToString(CultureStrings.FormatIntWithComma)),sourcePath,image);
					}
				}
				else if(id==imageWidthId)
				{
					if(image==null)
					{
						return null;
					}
					else
					{
						return fillParameter(format.Replace(quotedParameter,image.Width.ToString(CultureStrings.FormatIntWithComma)),sourcePath,image);
					}
				}
				else if(id==dateTakenId||id==dateTimeId)
				{
					if(image==null)
					{
						return null;
					}
					string dateTaken=getPropertyValue(sourcePath,image,id);
					if(dateTaken==null)
					{
						return null;
					}
					else
					{
						return fillParameter(format.Replace(quotedParameter,parseDateTaken(dateTaken.Trim()).ToString(parameterComponents.Length==3?parameterComponents[2]:"")),sourcePath,image);
					}
				}
				else if(id==dateModifiedId)
				{
					System.DateTime dateTime=new System.IO.FileInfo(sourcePath).LastWriteTime;
					return fillParameter(format.Replace(quotedParameter,dateTime.ToString(parameterComponents.Length==3?parameterComponents[2]:"")),sourcePath,image);
				}
				else if(id==dateCreatedId)
				{
					System.DateTime dateTime=new System.IO.FileInfo(sourcePath).CreationTime;
					return fillParameter(format.Replace(quotedParameter,dateTime.ToString(parameterComponents.Length==3?parameterComponents[2]:"")),sourcePath,image);
				}
				else if(id==fileExtensionId)
				{
					return fillParameter(format.Replace(quotedParameter,System.IO.Path.GetExtension(sourcePath)),sourcePath,image);
				}
				else
				{
					if(image==null)
					{
						return null;
					}
					string cameraInfomation=getPropertyValue(sourcePath,image,id);
					if(cameraInfomation==null)
					{
						return null;
					}
					else
					{
						string[]cameraInfomations=cameraInfomation.Split(System.IO.Path.GetInvalidPathChars());
						cameraInfomation=cameraInfomations.Aggregate((result,item)=>result+item);
						return fillParameter(format.Replace(quotedParameter,changeDirectorySeparatorChar(cameraInfomation.Trim(),' ')),sourcePath,image);
					}
				}
			}
			return format;
		}
		private static string getExcutableDirectory()
		{
			return System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
		}
		private static string getExcutableFileNameWithoutExtension()
		{
			return System.IO.Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath);
		}
		private static System.Random random=new System.Random();
		private static string getDateTimeRandomText()
		{
			lock(random)
			{
				return System.DateTime.Now.ToString(CultureStrings.FormatTimeStamp)+random.Next();
			}
		}
		private static string getCollectPhotoDataExtension()
		{
			return CultureStrings.FileExtensionCollectPhotoData;
		}
		private static string getLogFileExtension()
		{
			return CultureStrings.FileExtensionLog;
		}
		private static string buildCollectPhotoDataFilePath(string directoryPath,string label)
		{
			return normalizeDirectoryPath(directoryPath)+normalizeDirectoryPath(CultureStrings.DirectoryCollectPhotoData)+getDateTimeRandomText()+label+getCollectPhotoDataExtension();
		}
		private static string buildCollectPhotoLogFilePath(string directoryPath,string label)
		{
			return normalizeDirectoryPath(directoryPath)+normalizeDirectoryPath(CultureStrings.DirectoryCollectPhotoData)+getDateTimeRandomText()+label+getLogFileExtension();
		}
		private static string buildCollectPhotoConfigFilePath(string configFileName)
		{
			return getExcutableFileNameWithoutExtension()+normalizeDirectoryPath(CultureStrings.DirectoryConfig)+configFileName+CultureStrings.FileExtensionConfig;
		}
	}
}
