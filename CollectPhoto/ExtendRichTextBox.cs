namespace CollectPhoto
{
	public static class RichTextBoxExtension
	{
		public static void AppendLine(this System.Windows.Forms.RichTextBox richTextBox,string text)
		{
			if(richTextBox.Text.Length==0||richTextBox.Text.EndsWith("\n"))
			{
				richTextBox.AppendText(text);
			}
			else
			{
				richTextBox.AppendText(System.Environment.NewLine+text);
			}
		}
		public static void Insert(this System.Windows.Forms.RichTextBox richTextBox,string text)
		{
			int startIndex=richTextBox.SelectionStart;
			string selectedText=richTextBox.SelectedText;
			if(selectedText.Length>0)
			{
				richTextBox.Text=richTextBox.Text.Remove(startIndex,selectedText.Length);
			}
			richTextBox.Text=richTextBox.Text.Insert(startIndex,text);
			richTextBox.Select(startIndex,text.Length);
			richTextBox.Focus();
		}
	}
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private void richTextBox_MouseDoubleClicked(System.Windows.Forms.RichTextBox richTextBox)
		{
			if(richTextBox.Lines.Length==0)
			{
				if(openFileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
				{
					richTextBox_LoadFile(ref richTextBox,openFileDialog.FileName);
				}
			}
			else
			{
				if(saveFileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
				{
					richTextBox.SaveFile(saveFileDialog.FileName,System.Windows.Forms.RichTextBoxStreamType.UnicodePlainText);
				}
			}
		}
		private void richTextBox_LoadConfigFile(ref System.Windows.Forms.RichTextBox richTextBox,string configFilePath)
		{
			if(System.IO.File.Exists(configFilePath))
			{
				richTextBox.Lines=stripBlankLines(System.IO.File.ReadAllLines(configFilePath,System.Text.Encoding.Unicode));
			}
		}
		private void richTextBox_SaveConfigFile(ref System.Windows.Forms.RichTextBox richTextBox,string configFilePath)
		{
			createDirectoryForFile(configFilePath);
			richTextBox.SaveFile(configFilePath,System.Windows.Forms.RichTextBoxStreamType.UnicodePlainText);
		}
		private void richTextBox_LoadFile(ref System.Windows.Forms.RichTextBox richTextBox,string filePath)
		{
			if(System.IO.File.Exists(filePath))
			{
				System.IO.FileStream fileStream=new System.IO.FileStream(filePath,System.IO.FileMode.Open,System.IO.FileAccess.Read,System.IO.FileShare.ReadWrite);
				System.IO.StreamReader streamReader=new System.IO.StreamReader(fileStream,true);
				richTextBox.Text=streamReader.ReadToEnd();
				streamReader.Close();
				fileStream.Close();
			}
		}
	}
}
