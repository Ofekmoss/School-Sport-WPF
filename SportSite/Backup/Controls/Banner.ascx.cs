namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Collections;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	public enum BannerType
	{
		Unknown = 0,
		IsfGeneral = 1,
		IsfSpecialOffer,
		Advertisement_Main,
		Advertisement_Secondary,
		Advertisement_Small
	}

	/// <summary>
	///		Summary description for Banner.
	/// </summary>
	public class Banner : System.Web.UI.UserControl
	{
		private string _specialOfferContainer = "SpecialOfferFlash";
		private string _mainAdvertisementContainer = "MainAdvertisementFlash";
		private string _TopBannerContainer = "TopBannerFlash";
		private string _secondaryAdvertisementContainer = "SubAdvertisementFlash";
		private string _smallAdvertisementContainer = "SmallAdvertisementFlash";
		private BannerType _type = BannerType.Unknown;
		private Controls.MainView IsfMainView = null;
		private bool _english = false;

		public BannerType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public bool English
		{
			get { return _english; }
			set { _english = value; }
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			IsfMainView = (SportSite.Controls.MainView)
				Common.Tools.FindControlByType(this, "MainView");
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			string strBaseImage = "<img src=\"" + Common.Data.AppPath + "/Images/Banners/%image\" " +
				" width=\"526\" />";
			string strHTML = "";
			switch (_type)
			{
				case BannerType.IsfGeneral:
					if (_english)
					{
						strHTML = "<img id=\"" + _TopBannerContainer + "\" " +
							"src=\"" + Common.Data.AppPath +
							"/Images/banner_english.PNG\" />";
					}
					else
					{
						strHTML = "<div id=\"" + _TopBannerContainer + "\"></div>";
					}
					break;
				case BannerType.Advertisement_Main:
					strHTML = "<div id=\"" + _mainAdvertisementContainer + "\"></div>";
					break;
				case BannerType.Advertisement_Secondary:
					strHTML = "<div id=\"" + _secondaryAdvertisementContainer + "\"></div>";
					break;
				case BannerType.Advertisement_Small:
					//string strImageName=Common.Tools.CheckAndCreateThumbnail(
					//	Common.Data.AppPath+"/Images/default_article_image_new.JPG", 
					//	146, 60, this.Server);
					strHTML = "";
					string strFolderName = Common.Data.AppPath + "/Flash/Advertisements";
					string strMovie = strFolderName + "/" +
						Banner.GetSmallAdvertisementFile(this.Server);
					if (strMovie.ToLower().EndsWith(".swf"))
					{
						strHTML = "<div id=\"" + _smallAdvertisementContainer + "\"></div>";
					}
					else
					{
						string strURL = Common.Tools.CStrDef(
							Banner.GetSmallAdvertisementLink(this.Server), "");
						strHTML = Banner.BuildImageBanner(strMovie, strURL, this.Server);
					}
					break;
				case BannerType.IsfSpecialOffer:
					strHTML = "<div id=\"" + _specialOfferContainer + "\"></div>";
					break;
			}
			if (strHTML.Length > 0)
			{
				writer.Write(strHTML);
			}
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
			this.PreRender += new EventHandler(Page_PreRender);
		}
		#endregion

		private void Page_PreRender(object sender, EventArgs e)
		{
			string strMovie = "";
			string strContainer = "";
			string strBgColor = "";
			int width = 0;
			int height = 0;
			string strFlashName = "";
			string[] arrFlashNames = null;
			int flashIndex = -1;
			switch (_type)
			{
				case BannerType.Advertisement_Small:
					strMovie = Banner.GetSmallAdvertisementFile(this.Server);
					if (strMovie.ToLower().EndsWith(".swf"))
					{
						strMovie = Common.Data.AppPath + "/Flash/Advertisements/" +
							strMovie;
						strContainer = _smallAdvertisementContainer;
						strBgColor = "#f1f1f1";
						width = 146;
						height = 60;
					}
					else
					{
						strMovie = "";
					}
					break;
				case BannerType.IsfGeneral:
					if (_english == false)
					{
						string strFileName = "banner.swf";
						if (Application["TopBannerType"] != null)
						{
							int topBannerType = (int)Application["TopBannerType"];
							if (topBannerType == (int)Core.TopBannerManager.TopBannerType.Basketball)
								strFileName = "basketball1gmar.swf";
							else if (topBannerType == (int)Core.TopBannerManager.TopBannerType.Volleyball)
								strFileName = "aff01.swf";
							else if (topBannerType == (int)Core.TopBannerManager.TopBannerType.BeachVolleyball)
								strFileName = "volleyball01.swf";
							else if (topBannerType == (int)Core.TopBannerManager.TopBannerType.Handball)
								strFileName = "yad-2linesA.swf";
							else if (topBannerType == (int)Core.TopBannerManager.TopBannerType.ZooZoo)
								strFileName = "zuzu01.swf";
						}
						strMovie = String.Format("{0}/Flash/{1}", Common.Data.AppPath, strFileName);
						strContainer = _TopBannerContainer;
						strBgColor = "#f7e495";
						width = 437;
						height = 74;
					}
					break;
				case BannerType.IsfSpecialOffer:
					strMovie = Common.Data.AppPath + "/Flash/box.swf";
					strContainer = _specialOfferContainer;
					strBgColor = "#ffffff";
					width = 169;
					height = 175;
					break;
				case BannerType.Advertisement_Main:
				case BannerType.Advertisement_Secondary:
					arrFlashNames = (_type == BannerType.Advertisement_Main) ?
						Banner.GetMainAdvertisementFlash(this.Server) :
						Banner.GetSubAdvertisementFlash(this.Server);
					flashIndex = (_type == BannerType.Advertisement_Main) ?
						Common.Tools.CIntDef(Application["MainAdvertisementIndex"], 0) :
						Common.Tools.CIntDef(Application["SubAdvertisementIndex"], 0);
					if (flashIndex >= arrFlashNames.Length)
						flashIndex = arrFlashNames.Length - 1;
					if (flashIndex >= 0)
					{
						strFlashName = arrFlashNames[flashIndex];
						if ((strFlashName != null) && (strFlashName.Length > 0))
						{
							strMovie = Common.Data.AppPath + "/Flash/Advertisements/" + strFlashName;
							strContainer = (_type == BannerType.Advertisement_Main) ?
								_mainAdvertisementContainer :
								_secondaryAdvertisementContainer;
							strBgColor = "#f9f9ef";
							width = (IsCaptionDefined()) ? 500 : 546;
							height = 65;
						}
					}
					break;
			}

			if (strMovie.Length > 0)
				IsfMainView.clientSide.RegisterFlashMovie(strContainer, strMovie, width, height, "", strBgColor);
		}

		private bool IsCaptionDefined()
		{
			ArrayList arrMovies = IsfMainView.clientSide.RegisteredFlashMovies;
			foreach (string strMovie in arrMovies)
			{
				if (strMovie.ToLower().IndexOf("red_title_v" + Common.Data.FlashTitlesVersion + ".swf") >= 0)
					return true;
			}
			return false;
		}

		public static string BuildImageBanner(string strImageName, string strURL,
			HttpServerUtility Server)
		{
			if ((strImageName == null) || (strImageName.Length == 0) ||
				(strImageName[strImageName.Length - 1] == '/'))
			{
				strImageName = Common.Data.AppPath +
					"/Images/default_article_image_new.JPG";
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (strURL.Length > 0)
				sb.Append("<a href=\"" + strURL + "\">");
			string strThumbName = Common.Tools.CheckAndCreateThumbnail(
				strImageName, 146, 60, Server);
			sb.Append("<img src=\"" + strThumbName + "\" />");
			if (strURL.Length > 0)
				sb.Append("</a>");
			return sb.ToString();
		}

		#region main advertisement
		private static string[] _mainAdvertisementFlash = null;
		public static string[] GetMainAdvertisementFlash(HttpServerUtility Server)
		{
			if ((_mainAdvertisementFlash == null) || (_mainAdvertisementFlash.Length == 0))
				RebuildMainAdvertisement(Server);
			return _mainAdvertisementFlash;
		}

		public static void RebuildMainAdvertisement(HttpServerUtility Server)
		{
			_mainAdvertisementFlash = Sport.Common.Tools.SplitNoBlank(
				Common.Tools.ReadIniValue("MainAdvertisement", "FlashMovie",
				Server), '|');
		}
		#endregion

		#region secondary/sub advertisement
		private static string[] _secondaryAdvertisementFlash = null;
		public static string[] GetSubAdvertisementFlash(HttpServerUtility Server)
		{
			if ((_secondaryAdvertisementFlash == null) || (_secondaryAdvertisementFlash.Length == 0))
				RebuildSubAdvertisement(Server);
			return _secondaryAdvertisementFlash;
		}

		public static void RebuildSubAdvertisement(HttpServerUtility Server)
		{
			_secondaryAdvertisementFlash = Sport.Common.Tools.SplitNoBlank(
				Common.Tools.ReadIniValue("SecondaryAdvertisement", "FlashMovie",
				Server), '|');
		}
		#endregion

		#region small advertisement
		private static string _smallAdvertisementFile = null;
		private static string _smallAdvertisementLink = null;
		public static string GetSmallAdvertisementFile(HttpServerUtility Server)
		{
			if ((_smallAdvertisementFile == null) || (_smallAdvertisementFile.Length == 0))
				RebuildSmallAdvertisement(Server);
			return _smallAdvertisementFile;
		}

		public static string GetSmallAdvertisementLink(HttpServerUtility Server)
		{
			if ((_smallAdvertisementLink == null) || (_smallAdvertisementLink.Length == 0))
				RebuildSmallAdvertisement(Server);
			return _smallAdvertisementLink;
		}

		public static void RebuildSmallAdvertisement(HttpServerUtility Server)
		{
			_smallAdvertisementFile = Common.Tools.CStrDef(
				Common.Tools.ReadIniValue("SmallAdvertisement", "FileName", Server), "");

			_smallAdvertisementLink = Common.Tools.CStrDef(
				Common.Tools.ReadIniValue("SmallAdvertisement", "URL", Server), "");
		}
		#endregion

		#region comparers
		private class TopBannerFileComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				string strFile1 = System.IO.Path.GetFileNameWithoutExtension(x.ToString());
				string strFile2 = System.IO.Path.GetFileNameWithoutExtension(y.ToString());
				string[] arr1 = strFile1.Split(new char[] { '_' });
				string[] arr2 = strFile2.Split(new char[] { '_' });
				return Int32.Parse(arr1[1]).CompareTo(Int32.Parse(arr2[1]));
			}
		}
		#endregion
	}
}
