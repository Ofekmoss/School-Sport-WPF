using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SportSite
{
	public partial class balls : System.Web.UI.Page
	{
		private readonly int padding = 5;
		protected void Page_Load(object sender, EventArgs e)
		{
			string imagePath = Server.MapPath("~/Images/basketball.png");
			string borkedPath = Server.MapPath("~/Images/borked.png");
			string key = Request.QueryString["key"] + "";
			int ballCount = SportSite.Common.Tools.GetBallValue(key);
			if (ballCount <= 0)
			{
				SendImage(borkedPath);
				return;
			}

			using (Bitmap ball = new Bitmap(imagePath))
			{
				int singleBallWidth = ball.Width;
				int totalWidth = (singleBallWidth * ballCount) + ((ballCount - 1) * padding);
				int height = ball.Height;
				using (Bitmap bitmap = new Bitmap(totalWidth, height))
				{
					int x = 0;
					using (Graphics g = Graphics.FromImage(bitmap))
					{
						g.InterpolationMode = InterpolationMode.HighQualityBicubic;
						for (int i = 0; i < ballCount; i++)
						{
							g.DrawImage(ball, x, 0);
							x += (singleBallWidth + padding);
						}
						g.Save();
						SendImage(bitmap);
					}
				}
			}
		}

		private void SendImage(Bitmap bitmap)
		{
			Response.Clear();
			ImageConverter converter = new ImageConverter();
			byte[] rawBytes = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
			Response.ContentType = "image/png";
			Response.BinaryWrite(rawBytes);
			Response.End();
		}

		private void SendImage(string imagePath)
		{
			using (Bitmap bitmap = new Bitmap(imagePath))
			{
				SendImage(bitmap);
			}
		}
	}
}