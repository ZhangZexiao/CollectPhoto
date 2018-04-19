using System;
using System.Globalization;
using System.Linq;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private void setCulture(string cultureName)
		{
			var cultureInfo=new System.Globalization.CultureInfo(cultureName);
			CultureInfo.DefaultThreadCurrentCulture=cultureInfo;
			CultureInfo.DefaultThreadCurrentUICulture=cultureInfo;
			System.Threading.Thread.CurrentThread.CurrentCulture=cultureInfo;
			System.Threading.Thread.CurrentThread.CurrentUICulture=cultureInfo;
		}
		private void initializeControl(System.Windows.Forms.Control control)
		{
			try
			{
				control.Font=new System.Drawing.Font("SimSun",13F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			try
			{
				foreach(System.Windows.Forms.Control ctrl in control.Controls)
				{
					initializeControl(ctrl);
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			splitContainer_SourceDirectories_Other.Panel1MinSize=100;
			splitContainer_SourceDirectories_Other.SplitterDistance=100;
			splitContainer_SourceDirectories_Other.Panel2MinSize=200;
			splitContainer_ImageViewer_SourcePaths.Panel1MinSize=100;
			splitContainer_ImageViewer_SourcePaths.SplitterDistance=100;
			splitContainer_ImageViewer_SourcePaths.Panel2MinSize=100;
			this.Tag=getInitializedText();
		}
		private void showCollectedPaths(string filePath,System.Windows.Forms.RichTextBox richTextBox)
		{
			if(!backgroundWorker_Copy.IsBusy)
			{
				richTextBox_LoadFile(ref richTextBox,filePath);
			}
			else
			{
				System.Diagnostics.Process.Start(filePath);
			}
		}
		private void showMediaInformation(string filePath)
		{
			richTextBox_ImageProperties.AppendLine(filePath);
			try
			{
				MediaInfoLib.MediaInfo mediaInfo=new MediaInfoLib.MediaInfo();
				if(mediaInfo.Open(filePath)==1)
				{
					mediaInfo.Option("Complete","1");
					richTextBox_ImageProperties.AppendLine(CultureStrings.ColonMediaInformation);
					richTextBox_ImageProperties.AppendLine(mediaInfo.Inform());
				}
				mediaInfo.Close();
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
		}
		public void ClearImageViewer()
		{
			richTextBox_ImageProperties.Clear();
			pictureBox.Unload();
		}
		private void showSelectedMediaInformation(System.Drawing.Point point)
		{
			try
			{
				int charIndex=richTextBox_SourcePaths.GetCharIndexFromPosition(point);
				int endAt=richTextBox_SourcePaths.Text.IndexOf('\n',charIndex);
				int beginAt=richTextBox_SourcePaths.Text.LastIndexOf('\n',charIndex);
				beginAt++;
				if(endAt-beginAt<=0)
				{
					beginAt=richTextBox_SourcePaths.Text.LastIndexOf('\n',charIndex-1);
					beginAt++;
				}
				richTextBox_SourcePaths.Select(beginAt,endAt-beginAt);
				string filePath=richTextBox_SourcePaths.SelectedText;
				ClearImageViewer();
				showMediaInformation(filePath);
				using(var fs=System.IO.File.OpenRead(filePath))
				{
					pictureBox.Image=System.Drawing.Image.FromStream(fs);
					pictureBox.ImageLocation=filePath;
				}
				pictureBox.SizeMode=System.Windows.Forms.PictureBoxSizeMode.StretchImage;
				richTextBox_ImageProperties.AppendLine(CultureStrings.ColonCameraManufacturer+getPropertyValue(filePath,pictureBox.Image,manufacturerId));
				richTextBox_ImageProperties.AppendLine(CultureStrings.ColonCameraModel+getPropertyValue(filePath,pictureBox.Image,modelId));
				richTextBox_ImageProperties.AppendLine(CultureStrings.ColonDateTaken+parseDateTaken(getPropertyValue(filePath,pictureBox.Image,dateTakenId)).ToString());
				string destinationPath=buildImageDestinationFilePath(getThreadParameters(),filePath,pictureBox.Image);
				if(destinationPath!=null)
				{
					richTextBox_ImageProperties.AppendLine(CultureStrings.ColonSource+filePath);
					richTextBox_ImageProperties.AppendLine(CultureStrings.ColonDestination+destinationPath);
				}
				richTextBox_ImageProperties.AppendLine(CultureStrings.ColonImagePropertyIds+convertIntsToText(pictureBox.Image.PropertyIdList));
				foreach(System.Drawing.Imaging.PropertyItem item in pictureBox.Image.PropertyItems)
				{
					richTextBox_ImageProperties.AppendLine(string.Format(CultureStrings.InformationImageProperty,item.Id.ToString(CultureStrings.FormatIntToHex),item.Id,item.Type,item.Len,getPropertyTagDescription(item.Id),getPropertyValue(item)));
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
		}
		private void showTips()
		{
			System.Windows.Forms.MessageBox.Show(" The example displays the following output:\n    FullDateTimePattern:              dddd, MMMM dd, yyyy h:mm:ss tt\n"+"                                         Example: Monday, May 28, 2012 11:35:00 AM\n    \n    LongDatePattern:                  dddd, MMMM dd, yyyy\n                                         Example: Monday, May 28, 2012\n    \n    LongTimePattern:                  h:mm:ss tt\n                                         Example: 11:35:00 AM\n    \n    MonthDayPattern:                  MMMM dd\n                                         Example: May 28\n    \n    RFC1123Pattern:                   ddd, dd MMM yyyy HH':'mm':'ss 'GMT'\n                                         Example: Mon, 28 May 2012 11:35:00 GMT\n    \n    ShortDatePattern:                 M/d/yyyy\n                                         Example: 5/28/2012\n    \n"+"    ShortTimePattern:                 h:mm tt\n"+"                                         Example: 11:35 AM\n"+"    \n"+"    SortableDateTimePattern:          yyyy'-'MM'-'dd'T'HH':'mm':'ss\n"+"                                         Example: 2012-05-28T11:35:00\n"+"    \n"+"    UniversalSortableDateTimePattern: yyyy'-'MM'-'dd HH':'mm':'ss'Z'\n"+"                                         Example: 2012-05-28 11:35:00Z\n"+"    \n"+"    YearMonthPattern:                 MMMM, yyyy\n"+"                                         Example: May, 2012\n",CultureStrings.TextTips);
		}
		private string getInitializedText()
		{
			return"Initialized";
		}
		private int getSearchDirectoryDeepnessLimitation()
		{
			try
			{
				return System.Convert.ToInt32(textBox_SearchDirectoriesDeepnessLimitation.Text);
			}
			catch(System.Exception ex)
			{
				logException(ex);
				textBox_SearchDirectoriesDeepnessLimitation.Text="0";
				return 0;
			}
		}
	}
}
