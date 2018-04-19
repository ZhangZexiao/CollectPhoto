using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CollectPhoto
{
	public static class PictureBoxExtension
	{
		public static void Unload(this System.Windows.Forms.PictureBox pb)
		{
			pb.Image=null;
			pb.ImageLocation=string.Empty;
			pb.Refresh();
		}
	}
}
