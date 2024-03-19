using System;

namespace Sportsman.Details
{
	/// <summary>
	/// Inherits DetailView for schools details
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class TeamDetailsView : Sport.UI.DetailsView
	{
		private EntitiesPage playersPage;

		public TeamDetailsView()
			: base (new Entities.TeamView())
		{
		}

		public override void Open()
		{
			FieldsPage generalPage = new FieldsPage("כללי",
				new int[] {
							(int) Sport.Entities.Team.Fields.Sport,
							(int) Sport.Entities.Team.Fields.Championship,
							(int) Sport.Entities.Team.Fields.Category,
							(int) Sport.Entities.Team.Fields.School,
							(int) Sport.Entities.Team.Fields.SchoolSymbol,
							(int) Sport.Entities.Team.Fields.Index,
							(int) Sport.Entities.Team.Fields.Status,
							(int) Sport.Entities.Team.Fields.Supervisor,
							(int) Sport.Entities.Team.Fields.RegisterDate,
							(int) Sport.Entities.Team.Fields.LastModified,
							(int) Sport.Entities.Team.Fields.Charge
						  });
				//(int) Sport.Entities.Team.Fields.PlayerNumberFrom,
				//(int) Sport.Entities.Team.Fields.PlayerNumberTo,

			Pages.Add(generalPage);

			playersPage = new EntitiesPage(new Entities.PlayerView(), (int) Sport.Entities.Player.Fields.Team);

			playersPage.Columns = new int[] {
												(int) Sport.Entities.Player.Fields.Number,
												(int) Sport.Entities.Player.Fields.Student,
												(int) Sport.Entities.Player.Fields.Status
											};
			
			playersPage.NewClick += new EventHandler(NewPlayerClicked);
			playersPage.DeleteClick += new Sport.Data.EntityEventHandler(DeletePlayerClicked);
			Pages.Add(playersPage);

			base.Open();
		}

		private void NewPlayerClicked(object sender, EventArgs e)
		{
			Sport.Entities.Team team = new Sport.Entities.Team(Entity);
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter((int) Sport.Entities.Student.Fields.School, team.School.Id);
			filter.Add(new Sport.Entities.StudentCategoryFilter(team.Category.Category));
			Sport.UI.GeneralTableView studentsTableView = 
				new Sport.UI.GeneralTableView("TeamPlayerSelection", new Entities.StudentView(), 
				new int[] {
							  (int) Sport.Entities.Student.Fields.IdNumber,
							  (int) Sport.Entities.Student.Fields.FirstName, 
							  (int) Sport.Entities.Student.Fields.LastName, 
							  (int) Sport.Entities.Student.Fields.Grade
						  },
				new int[] {
							  (int) Sport.Entities.Student.Fields.Grade,
							  (int) Sport.Entities.Student.Fields.FirstName, 
							  (int) Sport.Entities.Student.Fields.LastName
						  }, filter);

			studentsTableView.Searchers.Add("ת.ז.:", Sport.Entities.Student.Type.Fields[(int) Sport.Entities.Student.Fields.IdNumber], 100);
			studentsTableView.Searchers.Add("שם פרטי:", Sport.Entities.Student.Type.Fields[(int) Sport.Entities.Student.Fields.FirstName], 110);
			studentsTableView.Searchers.Add("שם משפחה:", Sport.Entities.Student.Type.Fields[(int) Sport.Entities.Student.Fields.LastName], 110);

			studentsTableView.Title = "בחירת שחקנים ל'" + team.Name + "'";

			Sport.UI.EntitySelectionDialog studentDialog = new Sport.UI.EntitySelectionDialog(studentsTableView, new System.Drawing.Size(500, 350));

			studentDialog.Multiple = true;
			studentDialog.Entities = null;

			if (studentDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Entities.Player[] players = team.GetPlayers();
				Sport.Data.Entity[] entities = studentDialog.Entities;

				foreach (Sport.Data.Entity entity in entities)
				{
					Sport.Entities.Student student = new Sport.Entities.Student(entity);
					bool exist = false;
					for (int n = 0; n < players.Length && !exist; n++)
					{
						if (players[n].Student.Id == student.Id)
							exist = true;
					}

					if (!exist)
					{
						Sport.Entities.Player player = new Sport.Entities.Player(Sport.Entities.Player.Type.New());
						player.Student = student;
						player.Team = team;
						player.Status = (int) Sport.Types.PlayerStatusType.Confirmed;
						player.Save();
					}
				}
			}
		}

		private void DeletePlayerClicked(object sender, Sport.Data.EntityEventArgs e)
		{
			Sport.UI.MessageBox.Warn("לא ניתן למחוק שחקנים דרך ממשק זה", "מחיקת שחקן");
			/*
			Sport.Entities.Player player = new Sport.Entities.Player(e.Entity);

			if (Sport.UI.MessageBox.Ask("האם למחוק שחקן '" + player.Name +"'?", false))
				e.Entity.Delete();
			*/
		}
	}
}
