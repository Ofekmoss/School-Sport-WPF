namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;

	/// <summary>
	///		Summary description for Sponsors.
	/// </summary>
	public class Sponsors : System.Web.UI.UserControl
	{

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}
		
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			Core.SponsorManager.SetServer(this.Server);
			Core.SponsorData[] arrSponsors=(Core.SponsorData[]) Cache["sponsors"];
			if (arrSponsors == null)
			{
				arrSponsors = Core.SponsorManager.GetSponsors();
				Cache["sponsors"] = arrSponsors;
			}
			ArrayList arrTemp=new ArrayList(arrSponsors);
			arrTemp.Sort(new SponsorComparer());
			arrSponsors = (Core.SponsorData[]) arrTemp.ToArray(typeof(Core.SponsorData));
			
			//string strBaseFolder=Common.Data.AppPath+"/Images/Sponsors/";
			string strBaseHtml="<img src=\"%1\" class=\"SponsorImage\" />";
			for (int i=0; i<arrSponsors.Length; i++)
			{
				Core.SponsorData curSponsor=arrSponsors[i];
				if (curSponsor.URL.Length > 0)
					writer.Write("<a href=\""+curSponsor.URL+"\">");
				writer.Write(strBaseHtml.Replace("%1", curSponsor.ImageFileName));
				if (curSponsor.URL.Length > 0)
					writer.Write("</a>");
			}
			/*
			writer.Write(strBaseHtml.Replace("%1", "active_logo.gif"));
			writer.Write(strBaseHtml.Replace("%1", "yediot_logo.gif"));
			writer.Write(strBaseHtml.Replace("%1", "straus_logo.gif"));
			writer.Write(strBaseHtml.Replace("%1", "mega_logo.gif"));
			writer.Write(strBaseHtml.Replace("%1", "23_logo.gif"));
			writer.Write(strBaseHtml.Replace("%1", "cola_logo.gif"));
			*/
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
		
		private class SponsorComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Core.SponsorData data1=(Core.SponsorData) x;
				Core.SponsorData data2=(Core.SponsorData) y;
				string strFile1=System.IO.Path.GetFileName(data1.ImageFileName);
				string strFile2=System.IO.Path.GetFileName(data2.ImageFileName);
				return strFile1[0].CompareTo(strFile2[0]);
			}
		}
	}
}
