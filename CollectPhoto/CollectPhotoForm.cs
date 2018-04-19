using System;
using System.Linq;
using System.Windows.Forms;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		void TabControl_HandleCreated(object sender,System.EventArgs e)
		{
			SendMessage((sender as TabControl).Handle,0x1300+49,IntPtr.Zero,(IntPtr)4);
		}
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd,int msg,IntPtr wp,IntPtr lp);
		public CollectPhotoForm()
		{
			setCulture("zh-Hans");
			InitializeComponent();
			this.tabControl.HandleCreated+=TabControl_HandleCreated;
		}
		private void CollectPhotoForm_Load(object sender,System.EventArgs e)
		{
			initializeControl(sender as System.Windows.Forms.Control);
		}
		private void CollectPhotoForm_Shown(object sender,System.EventArgs e)
		{
			textBox_SearchDirectoriesDeepnessLimitation.Text=Properties.Settings.Default.SearchSourceDirectoriesDeepLimitation;
			if(textBox_SearchDirectoriesDeepnessLimitation.Text.Length==0)
			{
				textBox_SearchDirectoriesDeepnessLimitation.Text="0";
			}
			checkBox_FilterWhileSearching.Checked=Properties.Settings.Default.FilterWhileSearching;
			checkBox_AutoDetermine.Checked=Properties.Settings.Default.AutoDetermine;
			checkBox_AutoStart.Checked=Properties.Settings.Default.AutoStart;
			checkBox_MultipleJobs.Checked=Properties.Settings.Default.MultipleJobs;
			checkBox_CreateSubDestinationRootDirectory.Checked=Properties.Settings.Default.CreateSubDestinationRootDirectory;
			checkBox_CopyLowGradeImage.Checked=Properties.Settings.Default.CopyLowGradeImage;
			checkBox_AutoRename.Checked=Properties.Settings.Default.AutoRename;
			checkBox_DeleteAlreadyExists.Checked=Properties.Settings.Default.DeleteAlreadyExists;
			checkBox_DeleteAfterCopy.Checked=Properties.Settings.Default.DeleteAfterCopy;
			checkBox_DeleteReadOnly.Checked=Properties.Settings.Default.DeleteReadOnly;
			richTextBox_SourceDirectories.Text=Properties.Settings.Default.SourceDirectories;
			richTextBox_DestinationRootDirectory.Text=Properties.Settings.Default.DestinationRootDirectory;
			richTextBox_Command.Text=Properties.Settings.Default.Command;
			richTextBox_CommandOptions.Text=Properties.Settings.Default.CommandOptions;
			richTextBox_IncludeFileExtensions.Text=Properties.Settings.Default.IncludeFileExtensions;
			richTextBox_ExcludeFileExtensions.Text=Properties.Settings.Default.ExcludeFileExtensions;
			richTextBox_ImageFileExtensions.Text=Properties.Settings.Default.ImageFileExtensions;
			richTextBox_SearchDirectoriesBypassPatterns.Text=Properties.Settings.Default.SearchDirectoriesBypassPatterns;
			richTextBox_DestinationSubDirectories.Text=Properties.Settings.Default.DestinationSubDirectories;
			richTextBox_FileNameComponent.Text=Properties.Settings.Default.FileNameComponent;
			checkBox_FilterWhileSearching_CheckedChanged(checkBox_FilterWhileSearching,null);
			checkBox_MultipleJobs_CheckedChanged(checkBox_MultipleJobs,null);
			checkBox_DeleteAlreadyExists_CheckedChanged(checkBox_DeleteAlreadyExists,null);
			loadConfig_SourceDirectories();
			loadConfig_DestinationRootDirectory();
			loadConfig_DestinationSubDirectories();
			loadConfig_FileNameComponent();
			loadConfig_SearchDirectoriesBypassPatterns();
			loadConfig_ImageFileExtensions();
		}
		private void CollectPhotoForm_FormClosing(object sender,System.Windows.Forms.FormClosingEventArgs e)
		{
			Properties.Settings.Default.SearchSourceDirectoriesDeepLimitation=textBox_SearchDirectoriesDeepnessLimitation.Text;
			Properties.Settings.Default.FilterWhileSearching=checkBox_FilterWhileSearching.Checked;
			Properties.Settings.Default.AutoDetermine=checkBox_AutoDetermine.Checked;
			Properties.Settings.Default.AutoStart=checkBox_AutoStart.Checked;
			Properties.Settings.Default.MultipleJobs=checkBox_MultipleJobs.Checked;
			Properties.Settings.Default.CreateSubDestinationRootDirectory=checkBox_CreateSubDestinationRootDirectory.Checked;
			Properties.Settings.Default.CopyLowGradeImage=checkBox_CopyLowGradeImage.Checked;
			Properties.Settings.Default.AutoRename=checkBox_AutoRename.Checked;
			Properties.Settings.Default.DeleteAlreadyExists=checkBox_DeleteAlreadyExists.Checked;
			Properties.Settings.Default.DeleteAfterCopy=checkBox_DeleteAfterCopy.Checked;
			Properties.Settings.Default.DeleteReadOnly=checkBox_DeleteReadOnly.Checked;
			Properties.Settings.Default.SourceDirectories=richTextBox_SourceDirectories.Text;
			Properties.Settings.Default.DestinationRootDirectory=richTextBox_DestinationRootDirectory.Text;
			Properties.Settings.Default.Command=richTextBox_Command.Text;
			Properties.Settings.Default.CommandOptions=richTextBox_CommandOptions.Text;
			Properties.Settings.Default.IncludeFileExtensions=richTextBox_IncludeFileExtensions.Text;
			Properties.Settings.Default.ExcludeFileExtensions=richTextBox_ExcludeFileExtensions.Text;
			Properties.Settings.Default.ImageFileExtensions=richTextBox_ImageFileExtensions.Text;
			Properties.Settings.Default.SearchDirectoriesBypassPatterns=richTextBox_SearchDirectoriesBypassPatterns.Text;
			Properties.Settings.Default.DestinationSubDirectories=richTextBox_DestinationSubDirectories.Text;
			Properties.Settings.Default.FileNameComponent=richTextBox_FileNameComponent.Text;
			Properties.Settings.Default.Save();
		}
		private void tipsToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			showTips();
		}
		private void tipsToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			showTips();
		}
		private void richTextBox_SourceDirectories_TextChanged(object sender,System.EventArgs e)
		{
			if(this.Tag as string==getInitializedText())
			{
				if(!backgroundWorker_Search.IsBusy)
				{
					backgroundWorker_Search.RunWorkerCompleted+=backgroundWorker_Search_RunWorkerCompleted;
					backgroundWorker_Search.RunWorkerAsync(getThreadParameters());
				}
				else
				{
					backgroundWorker_Search.CancelAsync();
				}
			}
		}
		private void button_SelectSourceDirectories_Click(object sender,System.EventArgs e)
		{
			if(folderBrowserDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
			{
				richTextBox_SourceDirectories.AppendLine(folderBrowserDialog.SelectedPath);
			}
		}
		private void pictureBox_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			if(System.IO.File.Exists(pictureBox.ImageLocation))
			{
				System.Diagnostics.Process.Start(pictureBox.ImageLocation);
			}
		}
		private void richTextBox_SourcePaths_MouseClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			showSelectedMediaInformation(e.Location);
		}
		private void button_SelectDestinationRootDirectory_Click(object sender,System.EventArgs e)
		{
			if(folderBrowserDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
			{
				richTextBox_DestinationRootDirectory.Clear();
				richTextBox_DestinationRootDirectory.AppendLine(folderBrowserDialog.SelectedPath);
			}
		}
		private void backgroundWorker_Search_DoWork(object sender,System.ComponentModel.DoWorkEventArgs e)
		{
			ThreadParameters parameters=e.Argument as ThreadParameters;
			if(previousSearchThreadParameters!=null)
			{
				if(!isThreadParametersChanged(parameters,previousSearchThreadParameters))
				{
					e.Result=previousSearchThreadParameters;
					return;
				}
			}
			e.Result=searchDirectories(parameters);
		}
		private void backgroundWorker_Search_ProgressChanged(object sender,System.ComponentModel.ProgressChangedEventArgs e)
		{
			groupBox_SourcePaths.Text=e.UserState as string;
		}
		private void backgroundWorker_Search_RunWorkerCompleted(object sender,System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			backgroundWorker_Search.RunWorkerCompleted-=backgroundWorker_Search_RunWorkerCompleted;
			ThreadParameters parameters=e.Result as ThreadParameters;
			if(parameters.LogFilePath_ThreadResult!=null&&System.IO.File.Exists(parameters.LogFilePath_ThreadResult))
			{
				richTextBox_SourcePaths.Clear();
				richTextBox_LoadFile(ref richTextBox_SourcePaths,parameters.LogFilePath_ThreadResult);
				richTextBox_SourcePaths.AppendLine(parameters.LogFilePath_ThreadResult);
			}
			groupBox_SourcePaths.Text=CultureStrings.TextSourcePaths;
			if(e.Cancelled)
			{
				richTextBox_SourceDirectories_TextChanged(sender,e);
			}
			else
			{
				previousSearchThreadParameters=parameters;
				if(isThreadParametersChanged(parameters,getThreadParameters()))
				{
					richTextBox_SourceDirectories_TextChanged(sender,e);
				}
				else if(!button_FilterSourcePaths.Enabled&& !backgroundWorker_Filter.IsBusy)
				{
					backgroundWorker_Filter.RunWorkerCompleted+=backgroundWorker_Filter_RunWorkerCompleted;
					backgroundWorker_Filter.RunWorkerAsync(getThreadParameters());
				}
			}
		}
		private void button_FilterSourcePaths_Click(object sender,System.EventArgs e)
		{
			button_FilterSourcePaths.Enabled=false;
			if(!backgroundWorker_Search.IsBusy)
			{
				backgroundWorker_Search.RunWorkerCompleted+=backgroundWorker_Search_RunWorkerCompleted;
				backgroundWorker_Search.RunWorkerAsync(getThreadParameters());
			}
		}
		private void backgroundWorker_Filter_DoWork(object sender,System.ComponentModel.DoWorkEventArgs e)
		{
			e.Result=filterSourcePaths(e.Argument as ThreadParameters);
		}
		private void backgroundWorker_Filter_ProgressChanged(object sender,System.ComponentModel.ProgressChangedEventArgs e)
		{
			groupBox_SourcePaths.Text=e.UserState as string;
			progressBar.Value=e.ProgressPercentage;
		}
		private void backgroundWorker_Filter_RunWorkerCompleted(object sender,System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			backgroundWorker_Filter.RunWorkerCompleted-=backgroundWorker_Filter_RunWorkerCompleted;
			ThreadParameters parameters=e.Result as ThreadParameters;
			if(parameters.LogFilePath_ThreadResult!=null&&System.IO.File.Exists(parameters.LogFilePath_ThreadResult))
			{
				richTextBox_SourcePaths.Clear();
				richTextBox_LoadFile(ref richTextBox_SourcePaths,parameters.LogFilePath_ThreadResult);
				richTextBox_SourcePaths.AppendLine(parameters.LogFilePath_ThreadResult);
			}
			richTextBox_ExcludeFileExtensions.Lines=parameters.ExcludeFileExtensions;
			richTextBox_ImageFileExtensions.Lines=parameters.ImageFileExtensions;
			groupBox_SourcePaths.Text=CultureStrings.TextSourcePaths;
			button_FilterSourcePaths.Enabled=true;
			progressBar.Value=100;
			if(checkBox_AutoStart.Checked|| !button_Start.Enabled)
			{
				tabControl.SelectTab(tabPage_Copy);
				lock(copyFilesJobs)
				{
					copyFilesJobs.Push(getThreadParameters());
				}
				button_Start_Click(null,e);
			}
		}
		private void button_Start_Click(object sender,System.EventArgs e)
		{
			lock(copyFilesJobs)
			{
				button_Start.Enabled=false;
				ClearImageViewer();
				if(sender is System.Windows.Forms.Button)
				{
					ThreadParameters threadParameters=getThreadParameters();
					if(checkBox_MultipleJobs.Checked==true)
					{
						ThreadParameters[]jobs=splitCopyFilesJob(threadParameters);
						for(int i=jobs.Length-1;i>=0;i--)
						{
							copyFilesJobs.Push(jobs[i]);
						}
					}
					else
					{
						copyFilesJobs.Push(threadParameters);
					}
				}
				if(!backgroundWorker_Copy.IsBusy&&copyFilesJobs.Count>0)
				{
					richTextBox_CollectedSourcePaths.Clear();
					richTextBox_CollectedDestinationPaths.Clear();
					backgroundWorker_Copy.DoWork+=backgroundWorker_Copy_DoWork;
					if(checkBox_MultipleJobs.Checked==true)
					{
						backgroundWorker_Copy.RunWorkerAsync(null);
					}
					else
					{
						backgroundWorker_Copy.RunWorkerAsync(copyFilesJobs.Pop());
					}
				}
			}
		}
		private void backgroundWorker_Copy_DoWork(object sender,System.ComponentModel.DoWorkEventArgs e)
		{
			if(e.Argument is ThreadParameters)
			{
				copyFilesThread(e.Argument);
			}
			else
			{
				System.Collections.Generic.List<System.Threading.Thread>threadList=new System.Collections.Generic.List<System.Threading.Thread>();
				while(true)
				{
					int threadCount=System.Environment.ProcessorCount;
					lock(copyFilesJobs)
					{
						while(copyFilesJobs.Count>0&&threadCount>0)
						{
							System.Threading.Thread thread=new System.Threading.Thread(copyFilesThread);
							thread.Start(copyFilesJobs.Pop());
							threadList.Add(thread);
							threadCount--;
						}
					}
					foreach(System.Threading.Thread thread in threadList)
					{
						thread.Join();
					}
					lock(copyFilesJobs)
					{
						if(copyFilesJobs.Count==0)
						{
							break;
						}
					}
				}
			}
		}
		private void backgroundWorker_Copy_ProgressChanged(object sender,System.ComponentModel.ProgressChangedEventArgs e)
		{
			progress=e.UserState as ThreadProgress;
			progressBar.Style=progress.ProgressBarStyle;
			if(progress.ProgressBarStyle==System.Windows.Forms.ProgressBarStyle.Marquee)
			{
				return;
			}
			label_ProgressStatistic.Text=progress.GetDescription();
			label_ProgressSourcePath.Text=progress.SourcePath;
			progressBar.Value=progress.GetThreadProgress();
			try
			{
				if(progress.FlagCopy)
				{
					if(richTextBox_CollectedSourcePaths.Lines.Length>2000)
					{
						richTextBox_CollectedSourcePaths.Lines=richTextBox_CollectedSourcePaths.Lines.Where(isGreaterThan1000).ToArray();
						richTextBox_CollectedSourcePaths.SelectionStart=richTextBox_CollectedSourcePaths.Text.Length;
					}
					richTextBox_CollectedSourcePaths.AppendLine(progress.SourcePath);
					if(richTextBox_CollectedDestinationPaths.Lines.Length>2000)
					{
						richTextBox_CollectedDestinationPaths.Lines=richTextBox_CollectedDestinationPaths.Lines.Where(isGreaterThan1000).ToArray();
						richTextBox_CollectedDestinationPaths.SelectionStart=richTextBox_CollectedDestinationPaths.Text.Length;
					}
					richTextBox_CollectedDestinationPaths.AppendLine(progress.DestinationPath);
				}
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
		}
		private void backgroundWorker_Copy_RunWorkerCompleted(object sender,System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			backgroundWorker_Copy.DoWork-=backgroundWorker_Copy_DoWork;
			button_Start.Enabled=true;
			progressBar.Value=100;
			lock(copyFilesJobs)
			{
				if(copyFilesJobs.Count>0&& !backgroundWorker_Copy.IsBusy)
				{
					button_Start.Enabled=false;
					richTextBox_CollectedSourcePaths.Clear();
					richTextBox_CollectedDestinationPaths.Clear();
					backgroundWorker_Copy.DoWork+=backgroundWorker_Copy_DoWork;
					if(checkBox_MultipleJobs.Checked==true)
					{
						backgroundWorker_Copy.RunWorkerAsync(null);
					}
					else
					{
						backgroundWorker_Copy.RunWorkerAsync(copyFilesJobs.Pop());
					}
				}
			}
		}
		private void manufacturerToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentCameraManufacturer);
		}
		private void modelToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentCameraModel);
		}
		private void yearToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateTakenYear);
		}
		private void monthToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateTakenMonth);
		}
		private void dayToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateTakenDay);
		}
		private void hourToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateTakenHour);
		}
		private void minuteToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateTakenMinute);
		}
		private void secondToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateTakenSecond);
		}
		private void separatorToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.MarkerBackslash);
		}
		private void manufactorerToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentCameraManufacturer);
		}
		private void modelToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentCameraModel);
		}
		private void yearToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateTakenYear);
		}
		private void monthToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateTakenMonth);
		}
		private void dayToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateTakenDay);
		}
		private void hourToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateTakenHour);
		}
		private void minuteToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateTakenMinute);
		}
		private void secondToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateTakenSecond);
		}
		private void toolStripMenuItem4_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.MarkerDash);
		}
		private void toolStripMenuItem5_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.MarkerUnderscore);
		}
		private void toolStripMenuItem6_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.MarkerPlus);
		}
		private void fileSizeToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentFileSize);
		}
		private void originalFileNameToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentOriginalFileName);
		}
		private void imageHeightToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentImageHeight);
		}
		private void imageWidthToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentImageWidth);
		}
		private void latitudeLongitudeToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentLatitudeLongitude);
		}
		private void richTextBox_SearchDirectoriesBypassPatterns_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_IncludeFileExtensions_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_ExcludeFileExtensions_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_ImageFileExtensions_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_DestinationRootDirectory_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_DestinationSubDirectories_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_FileNameComponent_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_SourceDirectories_MouseDoubleClick(object sender,System.EventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_ImageProperties_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_SourcePaths_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_CollectedSourcePaths_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void richTextBox_CollectedDestinationPaths_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
			richTextBox_MouseDoubleClicked(sender as System.Windows.Forms.RichTextBox);
		}
		private void checkBox_FilterWhileSearching_CheckedChanged(object sender,System.EventArgs e)
		{
			if((sender as System.Windows.Forms.CheckBox).Checked)
			{
				checkBox_AutoDetermine.Checked=false;
				checkBox_AutoDetermine.Enabled=false;
			}
			else
			{
				checkBox_AutoDetermine.Enabled=true;
			}
		}
		private void checkBox_AutoDetermine_CheckedChanged(object sender,System.EventArgs e)
		{
			if((sender as System.Windows.Forms.CheckBox).Checked)
			{
				checkBox_FilterWhileSearching.Checked=false;
				checkBox_FilterWhileSearching.Enabled=false;
			}
			else
			{
				checkBox_FilterWhileSearching.Enabled=true;
			}
		}
		private void checkBox_MultipleJobs_CheckedChanged(object sender,System.EventArgs e)
		{
			if((sender as System.Windows.Forms.CheckBox).Checked)
			{
				checkBox_CreateSubDestinationRootDirectory.Enabled=true;
			}
			else
			{
				checkBox_CreateSubDestinationRootDirectory.Enabled=false;
			}
		}
		private void richTextBox_CollectedSourcePaths_Enter(object sender,System.EventArgs e)
		{
			if(progress!=null)
			{
				showCollectedPaths(progress.LogFilePath_CollectedSourcePaths,sender as System.Windows.Forms.RichTextBox);
			}
		}
		private void richTextBox_CollectedDestinationPaths_Enter(object sender,System.EventArgs e)
		{
			if(progress!=null)
			{
				showCollectedPaths(progress.LogFilePath_CollectedDestinationPaths,sender as System.Windows.Forms.RichTextBox);
			}
		}
		private void button_SelectCommand_Click(object sender,System.EventArgs e)
		{
			if(openFileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
			{
				richTextBox_Command.Clear();
				richTextBox_Command.AppendLine(openFileDialog.FileName);
			}
		}
		private void commandToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			if(richTextBox_Command.Text.Length>0)
			{
				richTextBox_CommandOptions.Insert(doubleQuoteText(richTextBox_Command.Text));
			}
			else
			{
				richTextBox_CommandOptions.Insert(doubleQuoteText(CultureStrings.ComponentCommand));
			}
		}
		private void sourcePathToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_CommandOptions.Insert(doubleQuoteText(CultureStrings.ComponentCommandSourcePath));
		}
		private void destinationRootDirectoryToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			if(richTextBox_DestinationRootDirectory.Text.Length>0)
			{
				richTextBox_CommandOptions.Insert(doubleQuoteText(richTextBox_DestinationRootDirectory.Text));
			}
			else
			{
				richTextBox_CommandOptions.Insert(doubleQuoteText(CultureStrings.ComponentCommandDestinationRootDirectory));
			}
		}
		private void button_Execute_Click(object sender,System.EventArgs e)
		{
			if(!backgroundWorker_ExecuteCommands.IsBusy)
			{
				button_Execute.Text=CultureStrings.TextCancel;
				backgroundWorker_ExecuteCommands.RunWorkerAsync(getThreadParameters());
			}
			else
			{
				backgroundWorker_ExecuteCommands.CancelAsync();
			}
		}
		private void backgroundWorker_ExecuteCommands_ProgressChanged(object sender,System.ComponentModel.ProgressChangedEventArgs e)
		{
			progressBar_Execution.Value=e.ProgressPercentage;
			richTextBox_ExecutionInformation.AppendLine(e.UserState as string);
		}
		private void backgroundWorker_ExecuteCommands_RunWorkerCompleted(object sender,System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			button_Execute.Text=CultureStrings.TextExecute;
			if(!e.Cancelled)
			{
				progressBar_Execution.Value=100;
			}
		}
		private const int CommandParameterKey_Command=0;
		private const int CommandParameterKey_SourcePath=1;
		private const int CommandParameterKey_DestinationRootDirectory=2;
		private void backgroundWorker_ExecuteCommands_DoWork(object sender,System.ComponentModel.DoWorkEventArgs e)
		{
			ThreadParameters parameters=e.Argument as ThreadParameters;
			e.Result=executeCommands(parameters);
		}
		private void fileSizeToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_CommandOptions.Insert(doubleQuoteText(CultureStrings.ComponentFileSize));
		}
		private void originalFileNameToolStripMenuItem1_Click(object sender,System.EventArgs e)
		{
			richTextBox_CommandOptions.Insert(doubleQuoteText(CultureStrings.ComponentOriginalFileName));
		}
		private void checkBox_DeleteAlreadyExists_CheckedChanged(object sender,System.EventArgs e)
		{
			if(checkBox_DeleteAlreadyExists.Checked||checkBox_DeleteAfterCopy.Checked)
			{
				checkBox_DeleteReadOnly.Enabled=true;
			}
			else
			{
				checkBox_DeleteReadOnly.Enabled=false;
			}
		}
		private void checkBox_DeleteAfterCopy_CheckedChanged(object sender,System.EventArgs e)
		{
			checkBox_DeleteAlreadyExists_CheckedChanged(sender,e);
		}
		private void yearToolStripMenuItem2_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateModifiedYear);
		}
		private void monthToolStripMenuItem2_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateModifiedMonth);
		}
		private void dayToolStripMenuItem2_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateModifiedDay);
		}
		private void hourToolStripMenuItem2_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateModifiedHour);
		}
		private void minuteToolStripMenuItem2_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateModifiedMinute);
		}
		private void secondToolStripMenuItem2_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateModifiedSecond);
		}
		private void yearToolStripMenuItem3_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateCreatedYear);
		}
		private void monthToolStripMenuItem3_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateCreatedMonth);
		}
		private void dayToolStripMenuItem3_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateCreatedDay);
		}
		private void hourToolStripMenuItem3_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateCreatedHour);
		}
		private void minuteToolStripMenuItem3_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateCreatedMinute);
		}
		private void secondToolStripMenuItem3_Click(object sender,System.EventArgs e)
		{
			richTextBox_FileNameComponent.Insert(CultureStrings.ComponentDateCreatedSecond);
		}
		private void yearToolStripMenuItem4_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateModifiedYear);
		}
		private void monthToolStripMenuItem4_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateModifiedMonth);
		}
		private void dayToolStripMenuItem4_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateModifiedDay);
		}
		private void hourToolStripMenuItem4_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateModifiedHour);
		}
		private void minuteToolStripMenuItem4_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateModifiedMinute);
		}
		private void secondToolStripMenuItem4_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateModifiedSecond);
		}
		private void yearToolStripMenuItem5_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateCreatedYear);
		}
		private void monthToolStripMenuItem5_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateCreatedMonth);
		}
		private void dayToolStripMenuItem5_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateCreatedDay);
		}
		private void hourToolStripMenuItem5_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateCreatedHour);
		}
		private void minuteToolStripMenuItem5_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateCreatedMinute);
		}
		private void secondToolStripMenuItem5_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentDateCreatedSecond);
		}
		private void fileExtensionToolStripMenuItem_Click(object sender,System.EventArgs e)
		{
			richTextBox_DestinationSubDirectories.Insert(CultureStrings.ComponentFileExtension);
		}
		private void richTextBox_SourceDirectories_MouseDoubleClick(object sender,System.Windows.Forms.MouseEventArgs e)
		{
		}
	}
}
