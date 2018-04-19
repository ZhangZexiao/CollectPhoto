using System.Linq;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private void copyFile(string sourceFilePath,string destinationFilePath)
		{
			createDirectoryForFile(destinationFilePath);
			System.IO.File.Copy(sourceFilePath,destinationFilePath);
		}
		private void overwriteFile(string sourceFilePath,string destinationFilePath)
		{
			createDirectoryForFile(destinationFilePath);
			System.IO.File.Copy(sourceFilePath,destinationFilePath,true);
		}
		private string getDotString()
		{
			return CultureStrings.MarkerDot;
		}
		private string getDotNumberString(int i)
		{
			return getDotString()+i;
		}
		private System.Text.RegularExpressions.Regex getDotNumberRegularExpressions()
		{
			string pattern=getDotString()+ @"\d+$";
			System.Text.RegularExpressions.Regex regex=new System.Text.RegularExpressions.Regex(pattern);
			return regex;
		}
		private string getFileNameWithoutDotNumber(string fileNameWithoutExtension)
		{
			while(getDotNumberRegularExpressions().IsMatch(fileNameWithoutExtension))
			{
				fileNameWithoutExtension=fileNameWithoutExtension.Substring(0,fileNameWithoutExtension.LastIndexOf(getDotString()));
			}
			return fileNameWithoutExtension;
		}
		private string getUnusedFilePath(string filePath)
		{
			string directoryPath=normalizeDirectoryPath(System.IO.Path.GetDirectoryName(filePath));
			string fileName=System.IO.Path.GetFileNameWithoutExtension(filePath);
			fileName=getFileNameWithoutDotNumber(fileName);
			string fileExtension=System.IO.Path.GetExtension(filePath);
			int i=0;
			while(System.IO.File.Exists(filePath))
			{
				filePath=directoryPath+fileName+getDotNumberString(i)+fileExtension;
				i++;
			}
			return filePath;
		}
		private string copyFileWithUnusedFileName(string sourceFilePath,string destinationFilePath)
		{
			destinationFilePath=getUnusedFilePath(destinationFilePath);
			copyFile(sourceFilePath,destinationFilePath);
			return destinationFilePath;
		}
		private void deleteFileWithoutException(string filePath,ThreadParameters parameters)
		{
			try
			{
				System.IO.FileInfo fileInfo=new System.IO.FileInfo(filePath);
				if(fileInfo.IsReadOnly&&parameters.DeleteReadOnly)
				{
					fileInfo.IsReadOnly=false;
				}
				if(fileInfo.Exists)
				{
					fileInfo.Delete();
				}
				FileWriterWithAppendMode.GlobalWrite(CultureStrings.TextDeleted+" "+singleQuoteText(filePath));
			}
			catch(System.Exception ex)
			{
				FileWriterWithAppendMode.GlobalWrite(CultureStrings.TextFailedInDeleting+" "+singleQuoteText(filePath));
				logException(ex);
			}
		}
		bool isThumbsDbFile(string filePath)
		{
			try
			{
				System.IO.FileInfo info=new System.IO.FileInfo(filePath);
				if(!info.Exists)
				{
					return false;
				}
				else if(!info.Attributes.HasFlag(System.IO.FileAttributes.Hidden))
				{
					return false;
				}
				else if(info.Extension!=".db")
				{
					return false;
				}
				else if(info.Name!="Thumbs.db")
				{
					return false;
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
				return false;
			}
			return true;
		}
		bool onlyOneThumbsDbFile(string[]filePaths)
		{
			if(filePaths.Length!=1)
			{
				return false;
			}
			return isThumbsDbFile(filePaths[0]);
		}
		private void DeleteDirectoryWithoutException(string directoryPath,ThreadParameters parameters)
		{
			try
			{
				FileWriterWithAppendMode.GlobalWrite(CultureStrings.TryDeleting+" "+singleQuoteText(directoryPath));
				string[]subDirectories=System.IO.Directory.GetDirectories(directoryPath);
				foreach(string subDirectory in subDirectories)
				{
					if(!parameters.SourceDirectories.Any(sourceDirectory=>sourceDirectory.StartsWith(subDirectory)))
					{
						DeleteDirectoryWithoutException(subDirectory,parameters);
					}
				}
				subDirectories=System.IO.Directory.GetDirectories(directoryPath);
				string[]files=System.IO.Directory.GetFiles(directoryPath);
				if(onlyOneThumbsDbFile(files))
				{
					deleteFileWithoutException(files[0],parameters);
				}
				files=System.IO.Directory.GetFiles(directoryPath);
				if(subDirectories.Length==0&&files.Length==0)
				{
					System.IO.Directory.Delete(directoryPath);
					FileWriterWithAppendMode.GlobalWrite(CultureStrings.TextDeleted+" "+singleQuoteText(directoryPath));
				}
			}
			catch(System.Exception ex)
			{
				FileWriterWithAppendMode.GlobalWrite(CultureStrings.TextFailedInDeleting+" "+singleQuoteText(directoryPath));
				logException(ex);
			}
		}
		private void DeleteFileAndRelatedDirectoriesWithoutException(string filePath,ThreadParameters parameters)
		{
			try
			{
				deleteFileWithoutException(filePath,parameters);
				string directoryPath=System.IO.Path.GetDirectoryName(filePath);
				while(System.IO.Directory.GetFiles(directoryPath).Length<=1)
				{
					DeleteDirectoryWithoutException(directoryPath,parameters);
					directoryPath=System.IO.Path.GetDirectoryName(directoryPath);
					if(parameters.SourceDirectories.Any(sourceDirectory=>sourceDirectory.StartsWith(directoryPath)))
					{
						break;
					}
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
		}
		private bool areTwoCopies(string sourcePath,string destinationPath)
		{
			try
			{
				System.IO.FileInfo sourceInfo=new System.IO.FileInfo(sourcePath);
				System.IO.FileInfo destinationInfo=new System.IO.FileInfo(destinationPath);
				if(!sourceInfo.Exists|| !destinationInfo.Exists)
				{
					return false;
				}
				else if(sourceInfo.FullName==destinationInfo.FullName)
				{
					return false;
				}
				else if(sourceInfo.Length!=destinationInfo.Length)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return false;
		}
		private bool isOneCopy(string sourcePath,string destinationPath)
		{
			try
			{
				System.IO.FileInfo sourceInfo=new System.IO.FileInfo(sourcePath);
				System.IO.FileInfo destinationInfo=new System.IO.FileInfo(destinationPath);
				if(sourceInfo.Exists&&destinationInfo.Exists&&sourceInfo.FullName==destinationInfo.FullName&&sourceInfo.Length==destinationInfo.Length)
				{
					return true;
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return false;
		}
		private string getDuplicateAutoRenamedFile(string sourcePath,string destinationPath)
		{
			string directoryName=System.IO.Path.GetDirectoryName(destinationPath);
			string fileName=System.IO.Path.GetFileNameWithoutExtension(destinationPath);
			string fileExtension=System.IO.Path.GetExtension(destinationPath);
			string[]similarPaths=getFiles(directoryName,fileName+"*"+fileExtension);
			if(similarPaths!=null)
			{
				foreach(string path in similarPaths)
				{
					if(areTwoCopies(sourcePath,path))
					{
						return path;
					}
				}
			}
			return null;
		}
		private void preCopyFile(ThreadParameters parameters,string sourcePath,ref string destinationPath,ref bool alreadyExists,ref bool isCopy)
		{
			isCopy=false;
			alreadyExists=false;
			string alreadyExistsDestinationPath=getDuplicateAutoRenamedFile(sourcePath,destinationPath);
			if(alreadyExistsDestinationPath!=null)
			{
				destinationPath=alreadyExistsDestinationPath;
				alreadyExists=true;
			}
			else if(isOneCopy(sourcePath,destinationPath))
			{
			}
			else if(parameters.AutoRename)
			{
				destinationPath=getUnusedFilePath(destinationPath);
				isCopy=true;
			}
			else
			{
				isCopy=true;
			}
		}
		private long getFileSize(string filePath)
		{
			try
			{
				if(System.IO.File.Exists(filePath))
				{
					System.IO.FileInfo fileInfo=new System.IO.FileInfo(filePath);
					return fileInfo.Length;
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return 0;
		}
	}
}
