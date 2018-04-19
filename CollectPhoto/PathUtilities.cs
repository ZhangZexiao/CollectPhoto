using System;
using System.Linq;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		string[]filterFilePaths(string[]filePaths,string[]fileExtensions)
		{
			if(filePaths==null||filePaths.Length==0)
			{
				return null;
			}
			if(fileExtensions==null||fileExtensions.Length==0)
			{
				return filePaths;
			}
			System.Collections.Generic.List<string>result=new System.Collections.Generic.List<string>();
			foreach(string filePath in filePaths)
			{
				if(isMatchingFilePath(filePath,fileExtensions))
				{
					result.Add(filePath);
				}
			}
			return result.ToArray();
		}
		private bool isMatchingDirectoryPath(string directoryPath,System.Text.RegularExpressions.Regex[]regexes)
		{
			if(regexes==null||regexes.Length==0)
			{
				return false;
			}
			return regexes.Any(regex=>regex.IsMatch(directoryPath));
		}
		private bool isMatchingFilePath(string filePath,string[]fileExtensions)
		{
			if(fileExtensions==null||fileExtensions.Length==0)
			{
				return false;
			}
			return fileExtensions.Any(extension=>filePath.EndsWith(extension,true,System.Globalization.CultureInfo.InvariantCulture));
		}
		private bool isMatchingFilePath(string filePath,System.Collections.Generic.Dictionary<string,System.Text.RegularExpressions.Regex>extensionAndFileNameMap)
		{
			if(extensionAndFileNameMap==null||extensionAndFileNameMap.Count==0)
			{
				return false;
			}
			return extensionAndFileNameMap.Any(extension=>filePath.EndsWith(extension.Key,true,System.Globalization.CultureInfo.InvariantCulture)&&extension.Value.IsMatch(System.IO.Path.GetFileNameWithoutExtension(filePath)));
		}
	}
}
