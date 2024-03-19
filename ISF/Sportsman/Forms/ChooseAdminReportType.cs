using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	public partial class ChooseAdminReportType : Form
	{
		public ChooseAdminReportType()
		{
			InitializeComponent();
		}

		Sport.Core.Data.AdministrationReportType reportType = Sport.Core.Data.AdministrationReportType.Undefined;
		public Sport.Core.Data.AdministrationReportType ReportType { get { return reportType; }  }


		private void ChooseAdminReportType_Load(object sender, EventArgs e)
		{
			reportType = Sport.Core.Data.AdministrationReportType.Undefined;
		}

		private void btnPersonal_Click(object sender, EventArgs e)
		{
			reportType = Sport.Core.Data.AdministrationReportType.Personal;
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		private void btnTeam_Click(object sender, EventArgs e)
		{
			reportType = Sport.Core.Data.AdministrationReportType.Team;
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}
	}
}
