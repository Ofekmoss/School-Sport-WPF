using System;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for PhasePatternsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports)]
	public class PhasePatternsTableView : Sport.UI.TableView
	{
		public PhasePatternsTableView()
		{
			Items.Add((int) Sport.Entities.PhasePattern.Fields.Name, "שם", 200);
			Items.Add((int) Sport.Entities.PhasePattern.Fields.Range, "תחום", 100);
			this._editorViewName = typeof(Producer.PhasePatternEditorView).Name;
		}

		public override void Open()
		{
			Title = "תבניות שלבים";

			EntityListView = new Sport.UI.EntityListView(Sport.Entities.PhasePattern.TypeName);

			Columns = new int[] { 0, 1 };

			EntityListView.Read(null);

			/*
			if (Sport.Core.PermissionsManager.IsSuperUser(Core.UserManager.CurrentUser.Id))
				EntityListView.Read(null);
			else
				EntityListView.Clear();
			*/
			
			base.Open();
		}

		protected override bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("פעולה זאת תמחוק את תבנית השלבים '" + entity.Name +
				"', האם להמשיך?", "ספורטסמן", System.Windows.Forms.MessageBoxButtons.YesNo);
			return dr == System.Windows.Forms.DialogResult.Yes;
		}
	}
}
