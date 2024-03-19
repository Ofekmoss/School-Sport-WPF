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
	public partial class ChooseChampionshipsReportDialog : Form
	{
		public ChooseChampionshipsReportDialog()
		{
			InitializeComponent();
		}

		public void DisableRadioButton(Documents.ChampionshipDocumentType report, string reason)
		{
			RadioButton button = GetRadioButton(report);
			if (button != null)
			{
				button.Enabled = false;
				if (!string.IsNullOrEmpty(reason) && button.Tag != null)
				{
					Label label = pnlRadioButtons.Controls[button.Tag.ToString()] as Label;
					if (label != null)
					{
						label.Text = "(" +  reason + ")";
						label.Visible = true;
					}
				}
			}
		}

		public void EnableRadioButton(Documents.ChampionshipDocumentType report)
		{
			RadioButton button = GetRadioButton(report);
			if (button != null)
			{
				button.Enabled = true;
				if (button.Tag != null)
				{
					Label label = pnlRadioButtons.Controls[button.Tag.ToString()] as Label;
					if (label != null)
						label.Visible = false;
				}
			}
		}

		public bool TryGetSelectedReport(out Documents.ChampionshipDocumentType? selectedReport)
		{
			selectedReport = null;
			if (rbClubReport.Checked)
			{
				selectedReport = Documents.ChampionshipDocumentType.ClubReport;
				return true;
			}
			if (rbOtherSportsReport.Checked)
			{
				selectedReport = Documents.ChampionshipDocumentType.OtherSportsReport;
				return true;
			}
			if (rbSportAdminReport.Checked)
			{
				selectedReport = Documents.ChampionshipDocumentType.AdministrationReport;
				return true;
			}
			return false;
		}

		public int MinCategoryCellWidthOverride
		{
			get
			{
				if (nudMinCategoryCellWidth.Visible)
					return (int)nudMinCategoryCellWidth.Value;
				return Documents.ChampionshipDocuments.MinimumCategoryCellWidth;
			}
		}

		private RadioButton GetRadioButton(Documents.ChampionshipDocumentType report)
		{
			switch (report)
			{
				case Documents.ChampionshipDocumentType.ClubReport:
					return rbClubReport;
				case Documents.ChampionshipDocumentType.OtherSportsReport:
					return rbOtherSportsReport;
				case Documents.ChampionshipDocumentType.AdministrationReport:
					return rbSportAdminReport;
			}
			return null;
		}

		private void rbClubReport_CheckedChanged(object sender, EventArgs e)
		{
			btnConfirm.Enabled = true;
		}

		private void ChooseChampionshipsReportDialog_Load(object sender, EventArgs e)
		{
			nudMinCategoryCellWidth.Value = Documents.ChampionshipDocuments.MinimumCategoryCellWidth;
		}

		private void label1_DoubleClick(object sender, EventArgs e)
		{
			nudMinCategoryCellWidth.Visible = true;
		}
	}
}
