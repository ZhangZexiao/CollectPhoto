namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private void loadConfig_SourceDirectories()
		{
			if(richTextBox_SourceDirectories.Text.Length==0)
			{
				richTextBox_SourceDirectories.Lines=System.IO.Directory.GetLogicalDrives();
			}
		}
		private void loadConfig_ImageFileExtensions()
		{
			if(richTextBox_ImageFileExtensions.Text.Length==0)
			{
				richTextBox_ImageFileExtensions.AppendLine(".jpg");
				richTextBox_ImageFileExtensions.AppendLine(".jpeg");
				richTextBox_ImageFileExtensions.AppendLine(".jpe");
				richTextBox_ImageFileExtensions.AppendLine(".jfif");
				richTextBox_ImageFileExtensions.AppendLine(".png");
				richTextBox_ImageFileExtensions.AppendLine(".bmp");
				richTextBox_ImageFileExtensions.AppendLine(".dib");
				richTextBox_ImageFileExtensions.AppendLine(".gif");
				richTextBox_ImageFileExtensions.AppendLine(".tif");
				richTextBox_ImageFileExtensions.AppendLine(".tiff");
			}
		}
		private void loadConfig_SearchDirectoriesBypassPatterns()
		{
			if(richTextBox_SearchDirectoriesBypassPatterns.Text.Length==0)
			{
				richTextBox_SearchDirectoriesBypassPatterns.AppendLine(@"\\\.");
			}
		}
		private void loadConfig_DestinationRootDirectory()
		{
			if(richTextBox_DestinationRootDirectory.Text.Length==0)
			{
				richTextBox_DestinationRootDirectory.AppendLine(getExcutableDirectory());
			}
		}
		private void loadConfig_DestinationSubDirectories()
		{
			if(richTextBox_DestinationSubDirectories.Text.Length==0)
			{
				richTextBox_DestinationSubDirectories.Text=CultureStrings.ComponentDateTakenYear+CultureStrings.MarkerBackslash+CultureStrings.ComponentDateTakenLiteralMonth+CultureStrings.MarkerWhiteSpace+CultureStrings.ComponentCameraManufacturer+CultureStrings.MarkerBackslash;
				richTextBox_DestinationSubDirectories.AppendLine(CultureStrings.ComponentFileExtension+CultureStrings.MarkerBackslash+CultureStrings.ComponentDateModifiedYear+CultureStrings.MarkerBackslash+CultureStrings.ComponentDateModifiedLiteralMonth+CultureStrings.MarkerBackslash);
			}
		}
		private void loadConfig_FileNameComponent()
		{
			if(richTextBox_FileNameComponent.Text.Length==0)
			{
				richTextBox_FileNameComponent.Text=CultureStrings.ComponentCameraModel+CultureStrings.MarkerWhiteSpace+CultureStrings.ComponentDateTakenYear+CultureStrings.TextYear+CultureStrings.ComponentDateTakenMonth+CultureStrings.TextMonth+CultureStrings.ComponentDateTakenDay+CultureStrings.TextDay+CultureStrings.ComponentDateTakenHour+CultureStrings.TextHour+CultureStrings.ComponentDateTakenMinute+CultureStrings.TextMinute+CultureStrings.ComponentDateTakenSecond+CultureStrings.TextSecond+CultureStrings.MarkerWhiteSpace+CultureStrings.ComponentDateTakenLiteralDay;
				richTextBox_FileNameComponent.AppendLine(CultureStrings.ComponentDateModifiedYear+CultureStrings.TextYear+CultureStrings.ComponentDateModifiedMonth+CultureStrings.TextMonth+CultureStrings.ComponentDateModifiedDay+CultureStrings.TextDay+CultureStrings.ComponentDateModifiedHour+CultureStrings.TextHour+CultureStrings.ComponentDateModifiedMinute+CultureStrings.TextMinute+CultureStrings.ComponentDateModifiedSecond+CultureStrings.TextSecond+CultureStrings.MarkerWhiteSpace+CultureStrings.ComponentDateModifiedLiteralDay);
			}
		}
	}
}
