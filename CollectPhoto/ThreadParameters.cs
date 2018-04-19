using System;
using System.Linq;
namespace CollectPhoto
{
	[System.Serializable]
	class ThreadParameters
	{
		public string[]SourceDirectories=null;
		public string[]ImageFileExtensions=null;
		public string[]ExcludeFileExtensions=null;
		public string[]IncludeFileExtensions=null;
		public System.Collections.Generic.Dictionary<string,System.Text.RegularExpressions.Regex>IncludeFileExtensionAndFileNameMap=null;
		public string[]SourcePaths=null;
		public string[]SearchDirectoriesBypassPatterns=null;
		public System.Text.RegularExpressions.Regex[]BypassRegexes=null;
		public string DestinationRootDirectory=null;
		public string[]DestinationSubDirectories=null;
		public string[]FileNameComponent=null;
		public int SearchDirectoryDeepnessLimitation=0;
		public bool AutoStart=false;
		public bool AutoDetermine=false;
		public bool FilterWhileSearching=false;
		public bool CopyLowGradeImage=false;
		public bool DeleteAfterCopy=false;
		public bool DeleteSourceIfDestinationAlreadyExists=false;
		public bool DeleteReadOnly=false;
		public bool AutoRename=false;
		public bool MultipleJobs=false;
		public bool CreateSubDestinationRootDirectory=false;
		public string LogFilePath_ThreadResult=null;
		public string Command=null;
		public string CommandOptions=null;
	}
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private bool isThreadParametersChanged(ThreadParameters p1,ThreadParameters p2)
		{
			return GetConfigContent(p1)!=GetConfigContent(p2);
		}
		static private ThreadParameters previousSearchThreadParameters=null;
		private string GetConfigContent(ThreadParameters parameters)
		{
			return("*** Source Directories ***\r\n"+convertStringsToText(parameters.SourceDirectories))+("\r\n*** Image File Extensions ***\r\n"+convertStringsToText(parameters.ImageFileExtensions))+("\r\n*** Exclude File Extensions ***\r\n"+convertStringsToText(parameters.ExcludeFileExtensions))+("\r\n*** Include File Extensions ***\r\n"+convertStringsToText(parameters.IncludeFileExtensions))+("\r\n*** Search Directories Bypass Patterns ***\r\n"+convertStringsToText(parameters.SearchDirectoriesBypassPatterns))+("\r\n*** Source Paths ***\r\n"+convertStringsToText(parameters.SourcePaths))+("\r\n*** Destination Root Directory ***\r\n"+parameters.DestinationRootDirectory)+("\r\n*** Destination Sub Directories ***\r\n"+parameters.DestinationSubDirectories)+("\r\n*** File Name Component ***\r\n"+parameters.FileNameComponent)+("\r\n*** Search Directory Deepness Limitation ***\r\n"+parameters.SearchDirectoryDeepnessLimitation.ToString())+("\r\n*** Auto Start ***\r\n"+parameters.AutoStart.ToString())+("\r\n*** Auto Determine ***\r\n"+parameters.AutoDetermine.ToString())+("\r\n*** Filter While Searching ***\r\n"+parameters.FilterWhileSearching.ToString())+("\r\n*** Copy Low Grade Image ***\r\n"+parameters.CopyLowGradeImage.ToString())+("\r\n*** Delete After Copy ***\r\n"+parameters.DeleteAfterCopy.ToString())+("\r\n*** Delete Already Exists ***\r\n"+parameters.DeleteSourceIfDestinationAlreadyExists.ToString())+("\r\n*** Delete Read Only ***\r\n"+parameters.DeleteReadOnly.ToString())+("\r\n*** Auto Rename ***\r\n"+parameters.AutoRename.ToString())+("\r\n*** Multiple Jobs ***\r\n"+parameters.MultipleJobs.ToString())+("\r\n*** Create Sub Destination Root Directory ***\r\n"+parameters.CreateSubDestinationRootDirectory.ToString());
		}
		static System.Text.RegularExpressions.Regex fileExtensionPattern=new System.Text.RegularExpressions.Regex(@"^\.\w+$");
		private ThreadParameters getThreadParameters()
		{
			ThreadParameters parameters=new ThreadParameters();
			parameters.SourceDirectories=stripBlankLines(richTextBox_SourceDirectories.Lines);
			parameters.ImageFileExtensions=stripBlankLines(richTextBox_ImageFileExtensions.Lines);
			parameters.ExcludeFileExtensions=stripBlankLines(richTextBox_ExcludeFileExtensions.Lines);
			System.Collections.Generic.List<string>includeFileExtensions=new System.Collections.Generic.List<string>();
			System.Collections.Generic.List<string>includeFileNameExtensions=new System.Collections.Generic.List<string>();
			foreach(string line in richTextBox_IncludeFileExtensions.Lines)
			{
				if(line.Length>0&&fileExtensionPattern.IsMatch(line))
				{
					includeFileExtensions.Add(line);
				}
				else
				{
					includeFileNameExtensions.Add(line);
				}
			}
			parameters.IncludeFileExtensions=includeFileExtensions.ToArray();
			parameters.IncludeFileExtensionAndFileNameMap=new System.Collections.Generic.Dictionary<string,System.Text.RegularExpressions.Regex>();
			foreach(string includeFileName in includeFileNameExtensions)
			{
				string pattern=System.IO.Path.GetFileNameWithoutExtension(includeFileName);
				if(pattern=="*")
				{
					includeFileExtensions.Add(System.IO.Path.GetExtension(includeFileName));
					parameters.IncludeFileExtensions=includeFileExtensions.ToArray();
					continue;
				}
				else if(pattern.Length==0)
				{
					continue;
				}
				if(!parameters.IncludeFileExtensionAndFileNameMap.ContainsKey(System.IO.Path.GetExtension(includeFileName)))
				{
					parameters.IncludeFileExtensionAndFileNameMap.Add(System.IO.Path.GetExtension(includeFileName),new System.Text.RegularExpressions.Regex(pattern));
				}
			}
			parameters.SearchDirectoriesBypassPatterns=stripBlankLines(richTextBox_SearchDirectoriesBypassPatterns.Lines);
			System.Collections.Generic.List<System.Text.RegularExpressions.Regex>regexList=new System.Collections.Generic.List<System.Text.RegularExpressions.Regex>();
			foreach(string bypassPattern in parameters.SearchDirectoriesBypassPatterns)
			{
				regexList.Add(new System.Text.RegularExpressions.Regex(bypassPattern));
			}
			parameters.BypassRegexes=regexList.ToArray();
			if(richTextBox_SourcePaths.Text.EndsWith(getCollectPhotoDataExtension()))
			{
				parameters.LogFilePath_ThreadResult=richTextBox_SourcePaths.Lines[richTextBox_SourcePaths.Lines.Length-1];
			}
			else
			{
				parameters.SourcePaths=richTextBox_SourcePaths.Lines;
			}
			parameters.DestinationRootDirectory=richTextBox_DestinationRootDirectory.Text;
			parameters.DestinationSubDirectories=stripBlankLines(richTextBox_DestinationSubDirectories.Lines);
			parameters.FileNameComponent=stripBlankLines(richTextBox_FileNameComponent.Lines);
			parameters.Command=richTextBox_Command.Text;
			parameters.CommandOptions=richTextBox_CommandOptions.Text;
			parameters.SearchDirectoryDeepnessLimitation=getSearchDirectoryDeepnessLimitation();
			parameters.AutoStart=checkBox_AutoStart.Checked;
			parameters.AutoDetermine=checkBox_AutoDetermine.Checked;
			parameters.FilterWhileSearching=checkBox_FilterWhileSearching.Checked;
			parameters.CopyLowGradeImage=checkBox_CopyLowGradeImage.Checked;
			parameters.DeleteAfterCopy=checkBox_DeleteAfterCopy.Checked;
			parameters.DeleteSourceIfDestinationAlreadyExists=checkBox_DeleteAlreadyExists.Checked;
			parameters.DeleteReadOnly=checkBox_DeleteReadOnly.Checked;
			parameters.AutoRename=checkBox_AutoRename.Checked;
			parameters.MultipleJobs=checkBox_MultipleJobs.Checked;
			parameters.CreateSubDestinationRootDirectory=checkBox_CreateSubDestinationRootDirectory.Checked;
			try
			{
				FileWriterWithAppendMode logger=FileWriterWithAppendMode.Open(buildCollectPhotoLogFilePath(richTextBox_DestinationRootDirectory.Text,CultureStrings.TextTaskParameters));
				string configContent=GetConfigContent(parameters);
				logger.Write(configContent);
				logger.Close();
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return parameters;
		}
		public T deepCopy<T>(T obj)
		{
			using(System.IO.MemoryStream ms=new System.IO.MemoryStream())
			{
				System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf=new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				bf.Serialize(ms,obj);
				ms.Position=0;
				return(T)bf.Deserialize(ms);
			}
		}
		private ThreadParameters[]splitCopyFilesJob(ThreadParameters threadParameters)
		{
			System.Collections.Generic.List<ThreadParameters>jobs=new System.Collections.Generic.List<ThreadParameters>();
			foreach(string sourceDirectory in threadParameters.SourceDirectories)
			{
				ThreadParameters newJob=deepCopy(threadParameters);
				newJob.SourceDirectories=threadParameters.SourceDirectories.Where(directory=>sourceDirectory==directory).ToArray();
				if(threadParameters.CreateSubDestinationRootDirectory)
				{
					newJob.DestinationRootDirectory=string.Copy(newJob.DestinationRootDirectory);
					string subDestinationRootDirectory=sourceDirectory.Replace(System.IO.Path.AltDirectorySeparatorChar,'_').Replace(System.IO.Path.DirectorySeparatorChar,'_').Replace(System.IO.Path.VolumeSeparatorChar,'_');
					newJob.DestinationRootDirectory=System.IO.Path.Combine(newJob.DestinationRootDirectory,subDestinationRootDirectory.Trim(new char[]{'_'}));
				}
				jobs.Add(newJob);
			}
			return jobs.ToArray();
		}
	}
}
