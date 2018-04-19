using System.Linq;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private System.Collections.Generic.Stack<ThreadParameters>copyFilesJobs=new System.Collections.Generic.Stack<ThreadParameters>();
		private ThreadParameters copyFiles(ThreadParameters parameters)
		{
			System.Collections.Generic.List<string>sourcePaths=new System.Collections.Generic.List<string>();
			if(parameters.SourcePaths!=null)
			{
				sourcePaths.AddRange(parameters.SourcePaths);
			}
			FileWriterWithAppendMode sourcePathLogger=null;
			FileWriterWithAppendMode deletedSourcePathLogger=null;
			FileWriterWithAppendMode destinationPathLogger=null;
			lock(copyFilesJobs)
			{
				sourcePathLogger=FileWriterWithAppendMode.Open(buildCollectPhotoLogFilePath(parameters.DestinationRootDirectory,CultureStrings.LogFileCopiedSourcePaths));
			}
			lock(copyFilesJobs)
			{
				destinationPathLogger=FileWriterWithAppendMode.Open(buildCollectPhotoLogFilePath(parameters.DestinationRootDirectory,CultureStrings.LogFileCopiedDestinationPaths));
			}
			lock(copyFilesJobs)
			{
				deletedSourcePathLogger=FileWriterWithAppendMode.Open(buildCollectPhotoLogFilePath(parameters.DestinationRootDirectory,CultureStrings.LogFileToBeDeletedSourcePaths));
			}
			ThreadProgress threadProgress=new ThreadProgress();
			resetThreadProgress(ref threadProgress,sourcePaths.Count,sourcePathLogger.FilePath,deletedSourcePathLogger.FilePath,destinationPathLogger.FilePath);
			if(parameters.LogFilePath_ThreadResult==null)
			{
				threadProgress.ProgressBarStyle=System.Windows.Forms.ProgressBarStyle.Marquee;
				backgroundWorker_Copy.ReportProgress(0,threadProgress);
				parameters=filterSourcePaths(searchDirectories(parameters));
			}
			if(System.IO.File.Exists(parameters.LogFilePath_ThreadResult))
			{
				sourcePaths.AddRange(System.IO.File.ReadAllLines(parameters.LogFilePath_ThreadResult,System.Text.Encoding.Unicode).ToList());
			}
			threadProgress.CountTotal=sourcePaths.Count;
			threadProgress.ProgressBarStyle=System.Windows.Forms.ProgressBarStyle.Blocks;
			for(int i=0;i<sourcePaths.Count;i++)
			{
				string sourcePath=sourcePaths[i];
				string destinationPath=null;
				threadProgress.SourcePath=sourcePath;
				bool isCopy=false;
				bool isOverwriteCopy=false;
				bool alreadyExists=false;
				try
				{
					if(isMatchingFilePath(sourcePath,parameters.IncludeFileExtensions)||isMatchingFilePath(sourcePath,parameters.IncludeFileExtensionAndFileNameMap))
					{
						destinationPath=buildImageDestinationFilePath(parameters,sourcePath,null);
						preCopyFile(parameters,sourcePath,ref destinationPath,ref alreadyExists,ref isCopy);
					}
					else if(isMatchingFilePath(sourcePath,parameters.ExcludeFileExtensions)||isMatchingFilePath(sourcePath,parameters.IncludeFileExtensionAndFileNameMap.Keys.ToArray()))
					{
					}
					else if(isImageFile(sourcePath))
					{
						bool isLowGradeImage=false;
						using(System.Drawing.Image image=getImage(sourcePath))
						{
							isLowGradeImage= !isImageLargerThanMinDimension(parameters.FileNameComponent,image);
							destinationPath=buildImageDestinationFilePath(parameters,sourcePath,image);
						}
						if(parameters.CopyLowGradeImage&&(isLowGradeImage||destinationPath==null))
						{
							destinationPath=buildUniqueDestinationFilePath(parameters,sourcePath);
							preCopyFile(parameters,sourcePath,ref destinationPath,ref alreadyExists,ref isCopy);
						}
						else if(destinationPath!=null)
						{
							preCopyFile(parameters,sourcePath,ref destinationPath,ref alreadyExists,ref isCopy);
							if(!alreadyExists&&isCopy&&System.IO.File.Exists(destinationPath)&&getFileSize(destinationPath)<getFileSize(sourcePath))
							{
								isOverwriteCopy=true;
							}
						}
					}
					if(parameters.DeleteSourceIfDestinationAlreadyExists&&alreadyExists&&areTwoCopies(sourcePath,destinationPath))
					{
						deletedSourcePathLogger.WriteLine(sourcePath);
						deletedSourcePathLogger.WriteLine(destinationPath);
						DeleteFileAndRelatedDirectoriesWithoutException(sourcePath,parameters);
						reportSkippedCopy(ref threadProgress,destinationPath,i);
					}
					else if(isCopy)
					{
						if(isOverwriteCopy)
						{
							overwriteFile(sourcePath,destinationPath);
						}
						else if(parameters.DeleteAfterCopy)
						{
							createDirectoryForFile(destinationPath);
							System.IO.File.Move(sourcePath,destinationPath);
							DeleteFileAndRelatedDirectoriesWithoutException(sourcePath,parameters);
						}
						else
						{
							copyFile(sourcePath,destinationPath);
						}
						sourcePathLogger.WriteLine(sourcePath);
						destinationPathLogger.WriteLine(destinationPath);
						reportSuccessCopy(ref threadProgress,destinationPath,i);
					}
					else
					{
						reportSkippedCopy(ref threadProgress,destinationPath,i);
					}
				}
				catch(System.Exception ex)
				{
					logException(ex);
					reportSkippedCopy(ref threadProgress,destinationPath,i);
				}
				sourcePathLogger.Flush();
				destinationPathLogger.Flush();
				deletedSourcePathLogger.Flush();
			}
			sourcePathLogger.Close();
			destinationPathLogger.Close();
			deletedSourcePathLogger.Close();
			FileWriterWithAppendMode.GlobalWrite(threadProgress.ToString());
			return parameters;
		}
		private void copyFilesThread(object obj)
		{
			copyFiles(obj as ThreadParameters);
		}
		private ThreadParameters searchDirectories(ThreadParameters parameters)
		{
			try
			{
				lock(copyFilesJobs)
				{
					parameters.LogFilePath_ThreadResult=buildCollectPhotoDataFilePath(parameters.DestinationRootDirectory,CultureStrings.ResultFileSearch);
				}
				foreach(string directory in parameters.SourceDirectories)
				{
					getFiles(normalizeDirectoryPath(directory),"*",System.IO.SearchOption.AllDirectories,parameters.SearchDirectoryDeepnessLimitation>0?parameters.SearchDirectoryDeepnessLimitation:int.MaxValue,parameters);
				}
			}
			catch(System.Exception cancellation)
			{
				logException(cancellation);
			}
			return parameters;
		}
		private ThreadParameters executeCommands(ThreadParameters parameters)
		{
			System.Collections.Generic.List<string>sourcePaths=new System.Collections.Generic.List<string>();
			if(parameters.SourcePaths!=null)
			{
				sourcePaths.AddRange(parameters.SourcePaths);
			}
			if(parameters.LogFilePath_ThreadResult!=null&&System.IO.File.Exists(parameters.LogFilePath_ThreadResult))
			{
				sourcePaths.AddRange(System.IO.File.ReadAllLines(parameters.LogFilePath_ThreadResult,System.Text.Encoding.Unicode));
			}
			else if(parameters.SourceDirectories!=null&&parameters.SourceDirectories.Length>0)
			{
				parameters=filterSourcePaths(searchDirectories(parameters));
				sourcePaths.AddRange(System.IO.File.ReadAllLines(parameters.LogFilePath_ThreadResult,System.Text.Encoding.Unicode));
			}
			lock(copyFilesJobs)
			{
				parameters.LogFilePath_ThreadResult=buildCollectPhotoDataFilePath(parameters.DestinationRootDirectory,CultureStrings.LogFileExecuteCommandsResult);
			}
			FileWriterWithAppendMode logger=FileWriterWithAppendMode.Open(parameters.LogFilePath_ThreadResult);
			string sourcePathTemplate=fillCommandParameter(fillCommandParameter(parameters.CommandOptions,CommandParameterKey_Command,parameters.Command,0),CommandParameterKey_DestinationRootDirectory,parameters.DestinationRootDirectory,0);
			string commandName=System.IO.Path.GetFileNameWithoutExtension(parameters.Command);
			if(sourcePathTemplate.IndexOf(commandName,System.StringComparison.InvariantCultureIgnoreCase)== -1)
			{
				sourcePathTemplate=doubleQuoteText(parameters.Command)+" "+sourcePathTemplate;
			}
			for(int i=0;i<sourcePaths.Count;i++)
			{
				string command=fillCommandParameter(sourcePathTemplate,CommandParameterKey_SourcePath,sourcePaths[i],0);
				command=fillCommandParameter(fillCommandParameter(command,fileSizeId,getFileSize(sourcePaths[i]).ToString(),0),originalFileNameId,getCollectPhotoOriginalFileName(sourcePaths[i]),0);
				if(backgroundWorker_ExecuteCommands.CancellationPending)
				{
					return parameters;
				}
				else
				{
					logger.WriteLine(command);
					if(backgroundWorker_ExecuteCommands.IsBusy)
					{
						backgroundWorker_ExecuteCommands.ReportProgress(calculatePercentage(sourcePaths.Count,i),command);
					}
					try
					{
						System.Diagnostics.ProcessStartInfo info=new System.Diagnostics.ProcessStartInfo(command);
						info.CreateNoWindow=true;
						info.UseShellExecute=false;
						info.RedirectStandardOutput=true;
						info.RedirectStandardError=true;
						System.Diagnostics.Process process=new System.Diagnostics.Process();
						process.StartInfo=info;
						process.Start();
						string stdout=process.StandardOutput.ReadToEnd();
						string stderr=process.StandardError.ReadToEnd();
						backgroundWorker_ExecuteCommands.ReportProgress(calculatePercentage(sourcePaths.Count,i),stdout);
						backgroundWorker_ExecuteCommands.ReportProgress(calculatePercentage(sourcePaths.Count,i),stderr);
						logger.WriteLine(stdout);
						logger.WriteLine(stderr);
					}
					catch(System.Exception ex)
					{
						logException(ex);
					}
				}
			}
			logger.Close();
			return parameters;
		}
		private ThreadParameters filterSourcePaths(ThreadParameters parameters)
		{
			System.Collections.Generic.List<string>sourcePaths=new System.Collections.Generic.List<string>();
			if(parameters.LogFilePath_ThreadResult!=null)
			{
				if(System.IO.File.Exists(parameters.LogFilePath_ThreadResult))
				{
					sourcePaths=System.IO.File.ReadAllLines(parameters.LogFilePath_ThreadResult,System.Text.Encoding.Unicode).ToList();
				}
			}
			if(sourcePaths.Count==0)
			{
				return parameters;
			}
			lock(copyFilesJobs)
			{
				parameters.LogFilePath_ThreadResult=buildCollectPhotoDataFilePath(parameters.DestinationRootDirectory,CultureStrings.ResultFileFilter);
			}
			FileWriterWithAppendMode logger=FileWriterWithAppendMode.Open(parameters.LogFilePath_ThreadResult);
			for(int i=0;i<sourcePaths.Count;i++)
			{
				string filePath=sourcePaths[i];
				if(backgroundWorker_Filter.IsBusy)
				{
					backgroundWorker_Filter.ReportProgress(calculatePercentage(sourcePaths.Count,i),filePath);
				}
				if(isMatchingFilePath(filePath,parameters.IncludeFileExtensions)||isMatchingFilePath(filePath,parameters.IncludeFileExtensionAndFileNameMap))
				{
					logger.WriteLine(filePath);
					continue;
				}
				if(isMatchingFilePath(filePath,parameters.ExcludeFileExtensions)||isMatchingFilePath(filePath,parameters.IncludeFileExtensionAndFileNameMap.Keys.ToArray()))
				{
					continue;
				}
				if(isMatchingFilePath(filePath,parameters.ImageFileExtensions))
				{
					logger.WriteLine(filePath);
					continue;
				}
				string extension=System.IO.Path.GetExtension(filePath);
				if(extension.Length==0)
				{
					continue;
				}
				if(parameters.AutoDetermine&&isImageFile(filePath))
				{
					parameters.ImageFileExtensions=parameters.ImageFileExtensions.Concat(new string[]{extension}).ToArray();
					logger.WriteLine(filePath);
					continue;
				}
				parameters.ExcludeFileExtensions=parameters.ExcludeFileExtensions.Concat(new string[]{extension}).ToArray();
			}
			logger.Close();
			return parameters;
		}
	}
}
