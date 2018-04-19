using System;
namespace CollectPhoto
{
	public partial class CollectPhotoForm:
	System.Windows.Forms.Form
	{
		private System.Drawing.Image getImage(string path)
		{
			try
			{
				System.IO.FileStream reader=new System.IO.FileStream(path,System.IO.FileMode.Open,System.IO.FileAccess.Read,System.IO.FileShare.Read);
				int length=(int)reader.Length;
				byte[]content=new byte[length];
				reader.Read(content,0,length);
				reader.Close();
				return System.Drawing.Image.FromStream(new System.IO.MemoryStream(content));
			}
			catch(System.Exception ex)
			{
				logException(ex);
			}
			return null;
		}
		private bool isImageFile(string path)
		{
			bool result=false;
			System.Drawing.Image image=null;
			System.Drawing.Graphics graphics=null;
			try
			{
				image=System.Drawing.Image.FromFile(path);
				result=true;
				graphics=System.Drawing.Graphics.FromImage(image);
			}
			catch(System.Exception ex)
			{
				logException(ex);
				FileWriterWithAppendMode.GlobalWrite(path);
			}
			finally
			{
				if(graphics!=null)
				{
					graphics.Dispose();
				}
				if(image!=null)
				{
					image.Dispose();
				}
			}
			return result;
		}
		private bool isImageLargerThanMinDimension(string[]formats,System.Drawing.Image image)
		{
			for(int i=0;i<formats.Length;i++)
			{
				char leftCurlyBracket='{',rightCurlyBracket='}',colon=':';
				int beginIndex=formats[i].IndexOf(leftCurlyBracket);
				int endIndex=beginIndex;
				while(beginIndex!= -1&&endIndex!= -1)
				{
					endIndex=formats[i].IndexOf(rightCurlyBracket,beginIndex);
					string parameter=formats[i].Substring(beginIndex+1,endIndex-beginIndex-1);
					string[]parameterComponents=parameter.Split(new char[]{colon});
					int id=int.Parse(parameterComponents[0]);
					if(id==imageHeightId&&parameterComponents.Length==3)
					{
						if(image.Height<int.Parse(parameterComponents[2]))
						{
							return false;
						}
					}
					else if(id==imageWidthId&&parameterComponents.Length==3)
					{
						if(image.Width<int.Parse(parameterComponents[2]))
						{
							return false;
						}
					}
					beginIndex=formats[i].IndexOf(leftCurlyBracket,endIndex);
				}
			}
			return true;
		}
	}
}
