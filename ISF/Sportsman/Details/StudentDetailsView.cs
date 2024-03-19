using System;

namespace Sportsman.Details
{
	/// <summary>
	/// Inherits DetailView for schools details
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class StudentDetailsView : Sport.UI.DetailsView
	{
		private EntitiesPage teamsPage;

		public StudentDetailsView()
			: base (new Entities.StudentView())
		{
		}
		
		public override void Open()
		{
			Searchers.Add(new Searcher("ת.ז.:", EntityView.EntityType.Fields[(int) Sport.Entities.Student.Fields.IdNumber], 100));

			FieldsPage generalPage = new FieldsPage("כללי",
				new int[] {
							(int) Sport.Entities.Student.Fields.IdNumber,
							(int) Sport.Entities.Student.Fields.LastName,
							(int) Sport.Entities.Student.Fields.FirstName,
							(int) Sport.Entities.Student.Fields.BirthDate,
							(int) Sport.Entities.Student.Fields.Grade,
							(int) Sport.Entities.Student.Fields.LastModified,
							(int) Sport.Entities.Student.Fields.School,
							(int) Sport.Entities.Student.Fields.SchoolSymbol,
							(int) Sport.Entities.Student.Fields.SexType
						  });

			Pages.Add(generalPage);

			teamsPage = new EntitiesPage(new Entities.PlayerView(), (int) Sport.Entities.Player.Fields.Student);

			teamsPage.Columns = new int[] {
											  (int) Sport.Entities.Player.Fields.Number,
											  (int) Sport.Entities.Player.Fields.Championship,
											  (int) Sport.Entities.Player.Fields.Category
										  };
			
			teamsPage.NewClick += new EventHandler(NewTeamClicked);
			teamsPage.DeleteClick += new Sport.Data.EntityEventHandler(DeleteTeamClicked);

			Pages.Add(teamsPage);

			base.Open();
		}
		
		
		private void NewTeamClicked(object sender, EventArgs e)
		{
			if (this.Entity == null)
				return;
			if (this.Entity.Id < 0)
				return;
			Sport.Entities.Student student = new Sport.Entities.Student(Entity);
			if ((student == null)||(student.Id < 0))
				return;
			if (student.School == null)
				return;
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter((int) Sport.Entities.Team.Fields.School, student.School.Id);
			Sport.Types.Sex sex = student.SexType;
			if (sex == Sport.Types.Sex.None)
				sex = Sport.Types.Sex.Both;
			filter.Add(new Sport.Entities.TeamCategoryFilter(Sport.Types.CategoryTypeLookup.ToCategory(sex, Sport.Types.GradeTypeLookup.ToGrade(student.Grade) - 1)));
			Sport.UI.GeneralTableView teamsTableView = 
				new Sport.UI.GeneralTableView("StudentTeamSelection", new Entities.TeamView(), 
				new int[] {
							  (int) Sport.Entities.Team.Fields.Championship,
							  (int) Sport.Entities.Team.Fields.Category, 
							  (int) Sport.Entities.Team.Fields.Index
						  },
				new int[] {
							  (int) Sport.Entities.Team.Fields.Championship,
							  (int) Sport.Entities.Team.Fields.Category, 
							  (int) Sport.Entities.Team.Fields.Index
						  }, filter);
			Sport.UI.EntitySelectionDialog teamDialog = new Sport.UI.EntitySelectionDialog(teamsTableView, new System.Drawing.Size(500, 350));
			//teamDialog.View.State[Sport.Entities.Region.TypeName] = school.Region.Id.ToString();

			teamDialog.Multiple = true;
			teamDialog.Entities = null;

			if (teamDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Entities.Player[] players = student.GetPlayers();
				Sport.Data.Entity[] entities = teamDialog.Entities;
				if (entities == null)
				{
					return;
				}
				foreach (Sport.Data.Entity entity in entities)
				{
					Sport.Entities.Team team = new Sport.Entities.Team(entity);
					bool exist = false;
					if (players != null)
					{
						for (int n = 0; n < players.Length && !exist; n++)
						{
							if (players[n].Team == null)
								continue;
							if (players[n].Team.Id == team.Id)
							{
								exist = true;
								break;
							}
						}
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
		
		private void DeleteTeamClicked(object sender, Sport.Data.EntityEventArgs e)
		{
			Sport.Entities.Player player = new Sport.Entities.Player(e.Entity);
			Sport.Entities.Team team = player.Team;

			string name = team.Championship.Name + " - " + team.Category.Name;
			if (team.Index > 0)
				name += " - " + Sport.Common.Tools.ToHebLetter(team.Index);

			if (Sport.UI.MessageBox.Ask("האם למחוק שחקן מקבוצה '" + name +"'?", false))
				e.Entity.Delete();
		}
	}
}
