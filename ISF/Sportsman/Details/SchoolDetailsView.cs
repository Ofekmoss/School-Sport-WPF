using System;

namespace Sportsman.Details
{
	/// <summary>
	/// Inherits DetailView for schools details
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class SchoolDetailsView : Sport.UI.DetailsView
	{
		private class AccountPage : EntitiesPage
		{
			private System.Windows.Forms.Label labelBalance;
			public AccountPage()
				: base(new Entities.AccountEntryView(), (int) Sport.Entities.AccountEntry.Fields.Account)
			{
			}

			protected override System.Windows.Forms.TabPage CreateTabPage()
			{
				System.Windows.Forms.TabPage tp = base.CreateTabPage ();

				gridControl.Height -= 18;

				labelBalance = new System.Windows.Forms.Label();
				labelBalance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Left)));
				labelBalance.Location = new System.Drawing.Point(gridControl.Left, gridControl.Bottom + 2);
				labelBalance.Name = "labelRegion";
				labelBalance.Size = new System.Drawing.Size(gridControl.Width, 16);
				labelBalance.Text = "מאזן:";

				tp.Controls.Add(labelBalance);

				return tp;
			}


			protected override void ReadEntity(Sport.Data.Entity entity)
			{
				Sport.Entities.Account account = null;
				if (entity != null)
				{
					Sport.Entities.School school = new Sport.Entities.School(entity);
					account = school.GetAccount();
				}

				if (account == null)
				{
					base.ReadEntity(null);
					labelBalance.Text = "מאזן:";
				}
				else
				{
					base.ReadEntity(account.Entity);

					labelBalance.Text = "מאזן: " + account.Balance.ToString();
				}
			}
		}

		private AccountPage accountPage;
		private EntitiesPage teamsPage;

		public SchoolDetailsView()
			: base (new Entities.SchoolView())
		{
		}

		public override void Open()
		{
			Searchers.Add(new Searcher("סמל:", EntityView.EntityType.Fields[(int) Sport.Entities.School.Fields.Symbol], 100));

			FieldsPage generalPage = new FieldsPage("כללי",
				new int[] {
							(int) Sport.Entities.School.Fields.Symbol,
							(int) Sport.Entities.School.Fields.Name,
							(int) Sport.Entities.School.Fields.ManagerName,
							(int) Sport.Entities.School.Fields.ClubStatus,
							(int) Sport.Entities.School.Fields.PlayerNumberFrom,
							(int) Sport.Entities.School.Fields.PlayerNumberTo, 
							(int) Sport.Entities.School.Fields.LastModified
						});

			Pages.Add(generalPage);

			FieldsPage contactPage = new FieldsPage("קשר",
				new int[] {
							(int) Sport.Entities.School.Fields.Region,
							(int) Sport.Entities.School.Fields.City,
							(int) Sport.Entities.School.Fields.Phone,
							(int) Sport.Entities.School.Fields.Fax,
							(int) Sport.Entities.School.Fields.Address,
							(int) Sport.Entities.School.Fields.MailAddress,
							(int) Sport.Entities.School.Fields.MailCity,
							(int) Sport.Entities.School.Fields.ZipCode,
							(int) Sport.Entities.School.Fields.Email
						  });
			
			Pages.Add(contactPage);

			FieldsPage typePage = new FieldsPage("סוג",
				new int[] {
							(int) Sport.Entities.School.Fields.FromGrade,
							(int) Sport.Entities.School.Fields.ToGrade,
							(int) Sport.Entities.School.Fields.Supervision,
							(int) Sport.Entities.School.Fields.Sector,
							(int) Sport.Entities.School.Fields.Category
						  });


			Pages.Add(typePage);

			teamsPage = new EntitiesPage(new Entities.TeamView(), (int) Sport.Entities.Team.Fields.School);

			teamsPage.Columns = new int[] {
											  (int) Sport.Entities.Team.Fields.Championship,
											  (int) Sport.Entities.Team.Fields.Category,
											  (int) Sport.Entities.Team.Fields.Index
										  };

			teamsPage.NewClick += new EventHandler(NewTeamClicked);
			teamsPage.DeleteClick += new Sport.Data.EntityEventHandler(DeleteTeamClicked);

            Pages.Add(teamsPage);

			accountPage = new AccountPage();
			accountPage.Columns = new int[] {
												(int) Sport.Entities.AccountEntry.Fields.EntryType,
												(int) Sport.Entities.AccountEntry.Fields.Description,
												(int) Sport.Entities.AccountEntry.Fields.Sum,
												(int) Sport.Entities.AccountEntry.Fields.EntryDate
											};
			accountPage.Sort = new int[] {
											 (int) Sport.Entities.AccountEntry.Fields.EntryDate
										 };

			Pages.Add(accountPage);

			base.Open();
		}

		private void NewTeamClicked(object sender, EventArgs e)
		{
			Sport.Entities.School school = new Sport.Entities.School(Entity);
			if (school == null || school.Id < 0 || school.Region == null)
				return;
			
			Sport.UI.EntitySelectionDialog categoryDialog = new Sport.UI.EntitySelectionDialog(new Views.ChampionshipCategoryTableView(), new System.Drawing.Size(500, 350));
			categoryDialog.View.State[Sport.Entities.Region.TypeName] = school.Region.Id.ToString();
			
			categoryDialog.Multiple = true;
			categoryDialog.Entities = null;
			
			if (categoryDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Entities.Team[] teams = school.GetTeams();
				Sport.Data.Entity[] entities = categoryDialog.Entities;
				
				foreach (Sport.Data.Entity entity in entities)
				{
					Sport.Entities.ChampionshipCategory category = new Sport.Entities.ChampionshipCategory(entity);
					Sport.Entities.Team team = new Sport.Entities.Team(Sport.Entities.Team.Type.New());
					team.Category = category;
					team.Championship = category.Championship;
					team.School = school;
					team.Status = Sport.Types.TeamStatusType.Confirmed;
					foreach (Sport.Entities.Team t in teams)
					{
						if (t.Category.Id == category.Id && t.Index >= team.Index)
							team.Index = t.Index + 1;
					}
					team.Save();
				}
			}
		}

		private void DeleteTeamClicked(object sender, Sport.Data.EntityEventArgs e)
		{
			Sport.Entities.Team team = new Sport.Entities.Team(e.Entity);

			string name = team.Championship.Name + " - " + team.Category.Name;
			if (team.Index > 0)
				name += " - " + Sport.Common.Tools.ToHebLetter(team.Index);

			if (Sport.UI.MessageBox.Ask("האם למחוק קבוצה '" + name +"'?", false))
				e.Entity.Delete();
		}
	}
}
