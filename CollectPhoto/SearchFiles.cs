using System;
using System.Linq;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private string[]getFiles(string directoryPath,string searchPattern)
		{
			try
			{
				if(System.IO.Directory.Exists(directoryPath))
				{
					return System.IO.Directory.GetFiles(directoryPath,searchPattern);
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return null;
		}
		private void getFiles(string directoryPath,string searchPattern,System.IO.SearchOption searchOption,int deepnessLimitation,ThreadParameters parameters)
		{
			FileWriterWithAppendMode.GlobalWrite(string.Format("getFiles, directory path: {0}, deepness limitation: {1}.",doubleQuoteText(directoryPath),deepnessLimitation));
			string[]fileExtensions=parameters.FilterWhileSearching?parameters.ImageFileExtensions.Concat(parameters.IncludeFileExtensions).ToArray():null;
			string resultFilePath=parameters.LogFilePath_ThreadResult;
			if(backgroundWorker_Search.CancellationPending)
			{
				throw new System.Exception("Cancel getFiles!");
			}
			if(backgroundWorker_Search.IsBusy)
			{
				backgroundWorker_Search.ReportProgress(0,directoryPath);
			}
			if(deepnessLimitation<=0)
			{
				return;
			}
			string[]topDirectoryFiles=getFiles(directoryPath,searchPattern);
			if(topDirectoryFiles!=null)
			{
				appendLines(resultFilePath,filterFilePaths(topDirectoryFiles,fileExtensions));
			}
			if(searchOption==System.IO.SearchOption.AllDirectories)
			{
				string[]directories=getDirectories(directoryPath);
				if(directories!=null)
				{
					foreach(string directory in directories)
					{
						if(isMatchingDirectoryPath(directory,parameters.BypassRegexes))
						{
							continue;
						}
						getFiles(directory,searchPattern,searchOption,deepnessLimitation-1,parameters);
					}
				}
			}
		}
		private string[]getSharedDirectories(string path)
		{
			System.Collections.Generic.List<string>sharePath=new System.Collections.Generic.List<string>();
			try
			{
				Trinet.Networking.ShareCollection shares=Trinet.Networking.ShareCollection.GetShares(path);
				foreach(Trinet.Networking.Share share in shares)
				{
					sharePath.Add(share.ToString());
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return sharePath.ToArray();
		}
		private string[]getDirectories(string directoryPath)
		{
			try
			{
				return System.IO.Directory.GetDirectories(directoryPath);
			}
			catch(System.Exception ex)
			{
				if(ex is System.ArgumentException&&directoryPath.StartsWith(@"\\"))
				{
					return getSharedDirectories(directoryPath.Trim(new char[]{'\\'}));
				}
				else
				{
					logException(ex);
				}
			}
			return null;
		}
	}
}
