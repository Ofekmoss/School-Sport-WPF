namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Simple progress bar.
	/// </summary>
	public class ProgressBar : System.Web.UI.UserControl
	{
//		private double frontBarWidth;
		private double frontBarHeight;
		
		private double nOverall = 1;
		protected System.Web.UI.WebControls.Image imFrontBar;
		private double nProgress = 0;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			double progressPercentage = (nProgress / nOverall);
			imFrontBar.ImageUrl = Common.Data.AppPath+"/Images/PixYellow.bmp";
			imFrontBar.Width = Unit.Percentage(progressPercentage * 100);
			imFrontBar.Height = new Unit(frontBarHeight);
		}

//		public double Width
//		{
//			get { return frontBarWidth; }
//			set { frontBarWidth = value; }
//		}
		
		public double Height
		{
			get { return frontBarHeight; }
			set { frontBarHeight = value; }
		}

		public double Overall
		{
			get { return nOverall; }
			set { nOverall = value; }
		}

		public double Progress
		{
			get { return nProgress; }
			set { nProgress = value; }
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
