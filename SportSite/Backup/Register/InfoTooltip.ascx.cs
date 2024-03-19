using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SportSite.NewRegister
{
	public partial class InfoTooltip : System.Web.UI.UserControl
	{
		public enum Placement
		{
			Top, 
			Bottom,
			Right,
			Left
		}

		public string Text { get; set; }
		private Placement dataPlacement = Placement.Right;
		public Placement DataPlacement
		{
			get { return dataPlacement; }
			set { dataPlacement = value; }
		}
		protected void Page_Load(object sender, EventArgs e)
		{
			lnkTooltip.Attributes["data-placement"] = this.DataPlacement.ToString().ToLower();
			lnkTooltip.Attributes["title"] = this.Text;
		}
	}
}