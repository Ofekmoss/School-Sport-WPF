using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SportSite.Core;

namespace SportSite.NewRegister
{
	public partial class Register : System.Web.UI.MasterPage
	{
		private bool requiresAdminAccess = false;
		public bool RequiresAdminAccess
		{
			get { return requiresAdminAccess; }
			set { requiresAdminAccess = value; }
		}

		public bool OutsideFormVisible
		{
			get { return OutsideForm.Visible; }
			set { OutsideForm.Visible = value; }
		}

		public UserData LoggedInUser
		{
			get
			{
				return (Session[UserManager.SessionKey] == null) ? UserData.Empty : (UserData)Session[UserManager.SessionKey];
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session[UserManager.SessionKey] == null)
			{
				string referrer = Request.Path;
				if (Request.QueryString != null && Request.QueryString.ToString().Length > 0)
					referrer += "?" + Request.QueryString.ToString();
				Session["new_interface_referrer"] = referrer;
				Response.Redirect("~/Register.aspx", true);
				return;
			}

			UserData _user = (UserData)Session[UserManager.SessionKey];
			litUserName.Text = _user.Name;
			lbSchoolName.InnerText = (_user.School.Name + "").Length == 0 ? "התאחדות הספורט" : _user.School.Name;

			if (this.RequiresAdminAccess)
			{
				if (_user.Type == (int)Sport.Types.UserType.External && _user.Permissions == 0)
				{
					MainContentPlaceHolder.Visible = false;
					pnlNotAuthorized.Visible = true;
				}
			}
		}
	}
}