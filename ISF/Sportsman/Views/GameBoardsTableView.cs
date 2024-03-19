using System;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for GameBoardsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports)]
	public class GameBoardsTableView : Sport.UI.TableView
	{
		public GameBoardsTableView()
		{
			Items.Add((int) Sport.Entities.GameBoard.Fields.Name, "��", 200);
			Items.Add((int) Sport.Entities.GameBoard.Fields.Range, "����", 100);
			this._editorViewName = typeof(Producer.GameBoardEditorView).Name;
		}

		public override void Open()
		{
			Title = "����� ������";

			EntityListView = new Sport.UI.EntityListView(Sport.Entities.GameBoard.TypeName);

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
			System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("����� ��� ����� �� ��� ������� '" + entity.Name +
				"', ��� ������?", "��������", System.Windows.Forms.MessageBoxButtons.YesNo);
			return dr == System.Windows.Forms.DialogResult.Yes;
		}
	}
}
