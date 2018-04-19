namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private bool isGreaterThan1000(string path,int index)
		{
			return index>1000;
		}
		static ThreadProgress progress=null;
		private double calculatePerPercentage(int total)
		{
			if(total<=0)
			{
				return 0;
			}
			return 1/(double)total*100;
		}
		private int calculatePercentage(int total,int index)
		{
			if(total<=0)
			{
				return 0;
			}
			return(int)(index*calculatePerPercentage(total));
		}
		private void reportSuccessCopy(ref ThreadProgress threadProgress,string destinationPath,int i)
		{
			threadProgress.DestinationPath=destinationPath;
			threadProgress.CountCopied++;
			threadProgress.CurrentTimeStamp=System.DateTime.Now;
			threadProgress.CopiedBytes+=getFileSize(threadProgress.DestinationPath);
			threadProgress.FlagCopy=true;
			backgroundWorker_Copy.ReportProgress(i,threadProgress);
		}
		private void reportSkippedCopy(ref ThreadProgress threadProgress,string destinationPath,int i)
		{
			threadProgress.DestinationPath=destinationPath==null?threadProgress.SourcePath:destinationPath;
			threadProgress.CountSkipped++;
			threadProgress.CurrentTimeStamp=System.DateTime.Now;
			threadProgress.SkippedBytes+=getFileSize(threadProgress.SourcePath);
			threadProgress.FlagCopy=false;
			backgroundWorker_Copy.ReportProgress(i,threadProgress);
		}
		private void resetThreadProgress(ref ThreadProgress threadProgress,int total,string collectedSourceLogFilePath,string toBeDeletedLogFilePath,string collectedDestinationLogFilePath)
		{
			threadProgress.CountTotal=total;
			threadProgress.CountCopied=0;
			threadProgress.CountSkipped=0;
			threadProgress.CopiedBytes=0;
			threadProgress.SkippedBytes=0;
			threadProgress.StartTimeStamp=System.DateTime.Now;
			threadProgress.CurrentTimeStamp=System.DateTime.Now;
			threadProgress.LogFilePath_CollectedSourcePaths=collectedSourceLogFilePath;
			threadProgress.LogFilePath_ToBeDeleted=toBeDeletedLogFilePath;
			threadProgress.LogFilePath_CollectedDestinationPaths=collectedDestinationLogFilePath;
		}
	}
	class ThreadProgress
	{
		public System.DateTime StartTimeStamp;
		public System.DateTime CurrentTimeStamp;
		public long CountTotal;
		public long CountCopied;
		public long CountSkipped;
		public long CopiedBytes;
		public long SkippedBytes;
		public bool FlagCopy;
		public string SourcePath;
		public string DestinationPath;
		public string LogFilePath_CollectedSourcePaths;
		public string LogFilePath_ToBeDeleted;
		public string LogFilePath_CollectedDestinationPaths;
		public System.Windows.Forms.ProgressBarStyle ProgressBarStyle=System.Windows.Forms.ProgressBarStyle.Blocks;
		System.TimeSpan GetElapsedTime()
		{
			return(CurrentTimeStamp-StartTimeStamp);
		}
		double GetImageFileAverageSeconds()
		{
			double returnValue=GetElapsedTime().TotalSeconds;
			if(GetCountProcessed()>0)
			{
				returnValue/=GetCountProcessed();
			}
			return returnValue;
		}
		string GetTimeSpanText(System.TimeSpan span)
		{
			return span.ToString(CultureStrings.FormatTimeSpan);
		}
		string GetElapsedTimeText()
		{
			return GetTimeSpanText(GetElapsedTime());
		}
		long GetCountProcessed()
		{
			return CountCopied+CountSkipped;
		}
		long GetCountRest()
		{
			return CountTotal-GetCountProcessed();
		}
		long GetImageFileAverageBytes()
		{
			return(CopiedBytes+SkippedBytes)/GetCountProcessed();
		}
		long GetEstimateRestBytes()
		{
			return GetImageFileAverageBytes()*GetCountRest();
		}
		System.TimeSpan GetEstimateRestTime()
		{
			double returnValue=GetImageFileAverageSeconds()*GetCountRest();
			return System.TimeSpan.FromSeconds(returnValue);
		}
		string GetEstimateRestTimeText()
		{
			return GetTimeSpanText(GetEstimateRestTime());
		}
		public string GetDescription()
		{
			if(CountTotal==0)
			{
				return"";
			}
			else
			{
				string line1=string.Format(CultureStrings.InformationWorkerTime,StartTimeStamp.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo),GetElapsedTimeText(),GetEstimateRestTimeText());
				string line2=string.Format(CultureStrings.InformationWorkerFileOperation,CountTotal.ToString(CultureStrings.FormatIntWithComma),CountCopied.ToString(CultureStrings.FormatIntWithComma),CountSkipped.ToString(CultureStrings.FormatIntWithComma));
				string line3=string.Format(CultureStrings.InformationWorkerBytes,CopiedBytes.ToString(CultureStrings.FormatIntWithComma),SkippedBytes.ToString(CultureStrings.FormatIntWithComma),GetEstimateRestBytes().ToString(CultureStrings.FormatIntWithComma));
				return line1+line2+line3;
			}
		}
		override public string ToString()
		{
			return GetDescription();
		}
		public int GetThreadProgress()
		{
			if(CountTotal<=0)
			{
				return 0;
			}
			return(int)(1.0*GetCountProcessed()/CountTotal*100);
		}
	}
}
