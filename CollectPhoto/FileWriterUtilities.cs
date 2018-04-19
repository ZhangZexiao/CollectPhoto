namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private void logException(System.Exception ex)
		{
			FileWriterWithAppendMode.GlobalWrite(ex.Message);
			if(!(ex is System.IO.IOException)&& !(ex is System.ArgumentException)&& !(ex is System.UnauthorizedAccessException))
			{
				FileWriterWithAppendMode.GlobalWrite(ex.StackTrace);
			}
		}
		private void appendLines(string filePath,string[]lines)
		{
			if(lines==null||lines.Length==0)
			{
				return;
			}
			FileWriterWithAppendMode writer=FileWriterWithAppendMode.Open(filePath);
			foreach(string line in lines)
			{
				writer.WriteLine(line);
			}
			writer.Close();
		}
	}
	class FileWriterWithAppendMode:
	System.IO.StreamWriter
	{
		public readonly string FilePath;
		public readonly System.DateTime CreateTime;
		public FileWriterWithAppendMode(string path):
		base(path,true,System.Text.Encoding.Unicode)
		{
			FilePath=path;
			CreateTime=System.DateTime.Now;
		}
		public new void Dispose()
		{
			if(globalLogger!=null)
			{
				globalLogger.Flush();
			}
			base.Dispose();
		}
		private static int i=0;
		private static System.Random random=new System.Random();
		private static string buildGlobalLogPath()
		{
			lock(random)
			{
				i++;
				return CultureStrings.ProgramName+CultureStrings.DirecotryLog+CultureStrings.MarkerBackslash+System.DateTime.Now.ToString(CultureStrings.FormatTimeStamp)+random.Next()+i+CultureStrings.TextLogFile+CultureStrings.FileExtensionLog;
			}
		}
		private static void createDirectoriesInFilePath(string path)
		{
			string directoryPath=System.IO.Path.GetDirectoryName(path);
			if(!System.IO.Directory.Exists(directoryPath))
			{
				System.IO.Directory.CreateDirectory(directoryPath);
			}
		}
		private static object globalWriteLock=new object();
		private static FileWriterWithAppendMode globalLogger=null;
		static public void GlobalWrite(string log)
		{
			lock(globalWriteLock)
			{
				if(globalLogger!=null&&(System.DateTime.Now-globalLogger.CreateTime).Hours>0)
				{
					globalLogger.Close();
					globalLogger=null;
				}
				if(globalLogger==null)
				{
					string path=buildGlobalLogPath();
					createDirectoriesInFilePath(path);
					globalLogger=new FileWriterWithAppendMode(path);
				}
				globalLogger.Write(System.DateTime.Now.ToString()+CultureStrings.MarkerColon+CultureStrings.MarkerWhiteSpace+log+System.Environment.NewLine);
				globalLogger.Flush();
			}
		}
		static public FileWriterWithAppendMode Open(string path)
		{
			try
			{
				createDirectoriesInFilePath(path);
				return new FileWriterWithAppendMode(path);
			}
			catch(System.Exception ex)
			{
				GlobalWrite(ex.Message);
				GlobalWrite(ex.StackTrace);
			}
			return null;
		}
	}
}
