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
	public partial class ConfirmTeamsImportDialog : Form
	{
		private string[] schoolSymbols = null;
		private Dictionary<string, int> schoolMapping = new Dictionary<string, int>();
		public ConfirmTeamsImportDialog(string[] symbols)
		{
			this.schoolSymbols = symbols;
			InitializeComponent();
		}

		public bool TryMapSymbol(string symbol, out int id)
		{
			return schoolMapping.TryGetValue(symbol, out id);
		}

		private void ConfirmTeamsImportDialog_Load(object sender, EventArgs e)
		{
			schoolMapping.Clear();
			List<string> allNames = new List<string>();
			schoolSymbols.ToList().ForEach(currentSymbol =>
			{
				Sport.Data.Entity[] schools = Sport.Entities.School.Type.GetEntities(new Sport.Data.EntityFilter((int)Sport.Entities.School.Fields.Symbol, currentSymbol));
				if (schools != null && schools.Length > 0)
				{
					Sport.Entities.School school = new Sport.Entities.School(schools[0]);
					allNames.Add(string.Format("{0} (סמל בית ספר: {1})", school.Name, currentSymbol));
					schoolMapping.Add(currentSymbol, school.Id);
				}
			});
			allNames.Sort();
			tbSchoolNames.Lines = allNames.ToArray();
			if (tbSchoolNames.Lines.Length == 0)
			{
				lbTitle.ForeColor = Color.Red;
				lbTitle.Text = "אין קבוצות להוספה";
			}
			else
			{
				lbTitle.Text = string.Format(lbTitle.Text, tbSchoolNames.Lines.Length);
			}
		}
	}
}
