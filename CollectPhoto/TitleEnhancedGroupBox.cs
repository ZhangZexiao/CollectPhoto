using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace CollectPhoto
{
	public class CaptionEnhancedGroupBox:
	GroupBox
	{
		Padding originalPadding;
		protected override void OnPaint(PaintEventArgs e)
		{
			if(object.ReferenceEquals(originalPadding,null))
			{
				originalPadding=this.Padding;
			}
			Graphics graphics=e.Graphics;
			Rectangle textRectangle=this.ClientRectangle;
			int textOffset=8;
			Size textSize;
			textRectangle.X+=textOffset;
			textRectangle.Width-=2*textOffset;
			using(StringFormat format=new StringFormat())
			{
				format.HotkeyPrefix=ShowKeyboardCues?System.Drawing.Text.HotkeyPrefix.Show:System.Drawing.Text.HotkeyPrefix.Hide;
				if(RightToLeft==RightToLeft.Yes)
				{
					format.FormatFlags|=StringFormatFlags.DirectionRightToLeft;
				}
				textSize=Size.Ceiling(graphics.MeasureString(Text,Font,textRectangle.Width,format));
			}
			this.Padding=new Padding(originalPadding.Left>2?originalPadding.Left:2,originalPadding.Top+textSize.Height-Font.Height,originalPadding.Right>2?originalPadding.Right:2,originalPadding.Bottom>2?originalPadding.Bottom:2);
			base.OnPaint(e);
		}
	}
}
