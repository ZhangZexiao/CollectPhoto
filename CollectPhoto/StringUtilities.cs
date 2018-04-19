using System;
using System.Linq;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private string[]stripBlankLines(string[]lines)
		{
			System.Collections.Generic.List<string>result=new System.Collections.Generic.List<string>();
			foreach(string line in lines)
			{
				if(line.Trim().Length>0)
				{
					result.Add(line);
				}
			}
			return result.ToArray();
		}
		private string convertIntsToText(int[]ids)
		{
			string text="";
			foreach(int id in ids)
			{
				text+=(id+"; ");
			}
			return text;
		}
		private string convertStringsToText(System.Collections.Generic.List<string>paths)
		{
			string text="";
			if(paths!=null)
			{
				foreach(string path in paths)
				{
					text+=path+System.Environment.NewLine;
				}
			}
			return text;
		}
		private string convertStringsToText(string[]paths)
		{
			string text="";
			if(paths!=null)
			{
				foreach(string path in paths)
				{
					text+=path+System.Environment.NewLine;
				}
			}
			return text;
		}
		private string stripPreamble(string text)
		{
			if(text==null)
			{
				return null;
			}
			return text.Replace(System.Text.Encoding.Unicode.GetString(System.Text.Encoding.Unicode.GetPreamble()),string.Empty);
		}
		private string doubleQuoteText(string text)
		{
			if(text==null)
			{
				return null;
			}
			return"\""+text+"\"";
		}
		private string singleQuoteText(string text)
		{
			if(text==null)
			{
				return null;
			}
			return"\'"+text+"\'";
		}
		private static string changeDirectorySeparatorChar(string path,char character)
		{
			if(path==null)
			{
				return null;
			}
			return path.Replace(System.IO.Path.AltDirectorySeparatorChar,character).Replace(System.IO.Path.DirectorySeparatorChar,character);
		}
		private static bool endWith(string text,char ch)
		{
			if(text==null)
			{
				return false;
			}
			else if(text.Length==0)
			{
				return false;
			}
			return text[text.Length-1]==ch;
		}
	}
}
